/* (May 17, 2020) Movie Download update */
(function () {

    // namespace for planimation
    window.planimation_downloadMovie = window.planimation_downloadMovie || {};

    // shorthand for namespace object
    var planimation = window.planimation_downloadMovie;

    // reference to ffmpeg_asm.js 
    planimation.workerPath = 'https://archive.org/download/ffmpeg_asm/ffmpeg_asm.js';

    window.addEventListener("load", () => {
        planimation.canvas = document.getElementById('#canvas');
        // using RecordRTC library
        planimation.canvasRecorder = RecordRTC(planimation.canvas, {
            type: 'canvas'
        } );
        planimation.lockpanel = document.createElement('screen-lock');
    } );

    window.addEventListener("click", (e) => {
        var userAgent = window.navigator.userAgent.toLowerCase();
        var browser = false;
        var json = {
            "x": e.offsetX,
            "y": window.innerHeight - e.offsetY
        };
        
        // GoogleChrome, Firefox, Opera
        if(userAgent.indexOf('chrome') != -1 || userAgent.indexOf('firefox') != -1 || userAgent.indexOf('opera') != -1) {
            browser = true;
        }
        json.browser = browser;
        
        // call browser check function
        gameInstance.SendMessage("Controller", "CheckBrowser", JSON.stringify(json));
    });

    planimation.sleep = (ms) => {
  		return new Promise(resolve => setTimeout(resolve, ms));
	};


    // screen lock function during recording
	planimation.lockScreen = () => {
		// element of lock screen
	    var div = document.createElement("div");
	    div.id = "lock-screen";
	    div.style.position = "fixed";
	    div.style.left = "0px"; 
	    div.style.top = "0px";
	    div.style.width = "100vw";
	    div.style.height = "100vh"; 
	    div.style.zIndex = "9999";
	    div.style.backgroundColor = "black";
	    div.style.opacity = 0.6;
	    div.style.textAlign = "center";
	    div.innerHTML = '<div style="width:300px; height:200px; position: absolute; left: 0; right: 0; top: 0; bottom :0; margin: auto; border-radius: 1px; background:#428bca;">'
        div.innerHTML +='<div style="color: white; position: absolute; top: 50%; left: 50%; -webkit-transform : translate(-50%,-50%); transform : translate(-50%,-50%); width: 300px; text-align: center;"><span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Now Recording</div></div>';
  		document.body.appendChild(div);
	};

    // stop screen locking after recording
	planimation.unlockScreen = () => {
		var div = document.getElementById("lock-screen");
  		document.body.removeChild(div);
	};

    // call screen recorder
    planimation.startRecording = () => {
        planimation.canvasRecorder.startRecording();
        gameInstance.SendMessage("Canvas", "RecordPlayback");
    };
    
    // call GIF converter
    planimation.outputGIF = (filetype) => {
    	planimation.sleep(500).then(() => {
		    planimation.canvasRecorder.stopRecording(function() {
                planimation.convertGIF(planimation.canvasRecorder.getBlob());
            });
		});
    };

    //Added by Mengyi Fan
    // call PNG converter
    planimation.outputPNG = (filetype) => {
    	planimation.sleep(500).then(() => {
		    planimation.canvasRecorder.stopRecording(function() {
                planimation.convertPNG(planimation.canvasRecorder.getBlob());
            });
		});
    };


    // call MP4 converter
    planimation.outputMP4 = (filetype) => {
        planimation.sleep(500).then(() => {
		    planimation.canvasRecorder.stopRecording(function() {
                planimation.convertMP4(planimation.canvasRecorder.getBlob());
            });
		});
    }

    // call WebM downloader
    planimation.outputWebM = (filetype) => {
        planimation.sleep(500).then(() => {
		    planimation.canvasRecorder.stopRecording(function() {
                planimation.postBlob(planimation.canvasRecorder.getBlob(), "planimation.webm");
            });
		});
    }

    // asynchronous process worker for format conversion
    planimation.processInWebWorker = () => {
        var blob = URL.createObjectURL(
            new Blob(
                ['importScripts("' + planimation.workerPath + '");var now = Date.now;function print(text) {postMessage({"type" : "stdout","data" : text});};onmessage = function(event) {var message = event.data;if (message.type === "command") {var Module = {print: print,printErr: print,files: message.files || [],arguments: message.arguments || [],TOTAL_MEMORY: 268435456};postMessage({"type" : "start","data" : Module.arguments.join(" ")});postMessage({"type" : "stdout","data" : "Received command: " +Module.arguments.join(" ") +((Module.TOTAL_MEMORY) ? ".  Processing with " + Module.TOTAL_MEMORY + " bits." : "")});var time = now();var result = ffmpeg_run(Module);var totalTime = now() - time;postMessage({"type" : "stdout","data" : "Finished processing (took " + totalTime + "ms)"});postMessage({"type" : "done","data" : result,"time" : totalTime});}};postMessage({"type" : "ready"});'],
                { type: 'application/javascript'}
            )
        );

        var worker = new Worker(blob);
        URL.revokeObjectURL(blob);
        return worker;
    }

    // converter for GIF from WebM
    planimation.convertGIF = (videoBlob) => {
        
        console.log("called converter");

        var aab;
        var buffersReady;
        var workerReady;
        var posted;

        // pass data to worker thread
        var postMessage = function() {
            posted = true;

            planimation.worker.postMessage({
                type: 'command',
                //arguments: '-i video.webm -b:v 6400k -strict experimental output.gif'.split(' '),
                arguments: '-i video.webm -r 10 -vf scale=640:-1 -f gif output.gif'.split(' '),
                files: [
                    {
                        data: new Uint8Array(aab),
                        name: 'video.webm'
                    }
                ]
            });
        };

        // read file data
        var fileReader = new FileReader();
        fileReader.onload = function() {
            aab = this.result;
            postMessage();
        };
        fileReader.readAsArrayBuffer(videoBlob);

        // get asynchrounous process
        if (!planimation.worker) {
            planimation.worker = planimation.processInWebWorker();
        }

        // asynchronous event from worker
        planimation.worker.onmessage = (event) => {
            var message = event.data;
            if (message.type == "ready") {
                console.log('<a href="'+ planimation.workerPath +'" download="ffmpeg-asm.js">ffmpeg-asm.js</a> file has been loaded.');

                workerReady = true;
                if (buffersReady)
                    postMessage();
            } else if (message.type == "stdout") {
                console.log(message.data);
            } else if (message.type == "start") {
                console.log('<a href="'+ planimation.workerPath +'" download="ffmpeg-asm.js">ffmpeg-asm.js</a> file received ffmpeg command.');
            
            // when conversion is done
            } else if (message.type == "done") {
                console.log(JSON.stringify(message));

                var result = message.data[0];
                console.log(JSON.stringify(result));

                // set up for GIF format
                var blob = new File(
                    [result.data], 
                    'test.gif', 
                    { type: 'image/gif'}
                );

                console.log(JSON.stringify(blob));

                // set up for downloading
                planimation.postBlob(blob, "planimation.gif");
            }
        };
    }

    // converter for MP4 from WebM
    planimation.convertMP4 = (videoBlob) => {
        
        console.log("called converter");

        var aab;
        var buffersReady;
        var workerReady;
        var posted;

        // pass data to worker thread
        var postMessage = function() {
            posted = true;

            planimation.worker.postMessage({
                type: 'command',
                arguments: '-i video.webm -c:v mpeg4 -b:v 16k -strict experimental output.mp4'.split(' '),
                files: [
                    {
                        data: new Uint8Array(aab),
                        name: 'video.webm'
                    }
                ]
            });
        };

        // read file data
        var fileReader = new FileReader();
        fileReader.onload = function() {
            aab = this.result;
            postMessage();
        };
        fileReader.readAsArrayBuffer(videoBlob);

        // get asynchrounous process
        if (!planimation.worker) {
            planimation.worker = planimation.processInWebWorker();
        }

        // asynchronous event from worker
        planimation.worker.onmessage = (event) => {
            var message = event.data;
            if (message.type == "ready") {
                console.log('<a href="'+ planimation.workerPath +'" download="ffmpeg-asm.js">ffmpeg-asm.js</a> file has been loaded.');

                workerReady = true;
                if (buffersReady)
                    postMessage();
            } else if (message.type == "stdout") {
                console.log(message.data);
            } else if (message.type == "start") {
                console.log('<a href="'+ planimation.workerPath +'" download="ffmpeg-asm.js">ffmpeg-asm.js</a> file received ffmpeg command.');
            
            // when conversion is done
            } else if (message.type == "done") {
                console.log(JSON.stringify(message));

                var result = message.data[0];
                console.log(JSON.stringify(result));

                // set up for MP4 format
                var blob = new File(
                    [result.data], 
                    'test.mp4', 
                    { type: 'video/mp4'}
                );

                console.log(JSON.stringify(blob));

                // set up for downloading
                planimation.postBlob(blob, "planimation.mp4");
            }
        };
    }

    //Added by Mengyi Fan
    //Edit by Changyuan Liu
    // converter for PNG from WebM
    planimation.convertPNG = (videoBlob) => {

        console.log("called converter");

        var aab;
        var buffersReady;
        var workerReady;
        var posted;

        // pass data to worker thread
        var postMessage = function() {
            posted = true;

            planimation.worker.postMessage({
                type: 'command',
                //arguments: '-i video.webm -b:v 6400k -strict experimental output.gif'.split(' '),
                //arguments: '-i video.webm -r 10 -vf scale=640:-1 -f gif output.gif'.split(' '),
                arguments: '-i video.webm -r 10 -vf scale=640:-1 -f image2 output%03d.jpg'.split(' '),
                files: [
                    {
                        data: new Uint8Array(aab),
                        name: 'video.webm'
                    }
                ]
            });
        };

        // read file data
        var fileReader = new FileReader();
        fileReader.onload = function() {
            aab = this.result;
            postMessage();
        };
        fileReader.readAsArrayBuffer(videoBlob);

        // get asynchrounous process
        if (!planimation.worker) {
            planimation.worker = planimation.processInWebWorker();
        }

        // asynchronous event from worker
        planimation.worker.onmessage = (event) => {
            var message = event.data;
            if (message.type == "ready") {
                console.log('<a href="'+ planimation.workerPath +'" download="ffmpeg-asm.js">ffmpeg-asm.js</a> file has been loaded.');

                workerReady = true;
                if (buffersReady)
                    postMessage();
            } else if (message.type == "stdout") {
                console.log(message.data);
            } else if (message.type == "start") {
                console.log('<a href="'+ planimation.workerPath +'" download="ffmpeg-asm.js">ffmpeg-asm.js</a> file received ffmpeg command.');

            // when conversion is done
            } else if (message.type == "done") {
                 //store image data to the zip file
                var pngLength = Object.keys(message.data).length;
                var zip = new JSZip();
                var img = zip.folder("planimation");
                var i=0;
                for(i;i<pngLength;i++){

                    var result = message.data[i];
                    img.file("output "+ i +".jpg", result.data , { base64:true } );

                }
                zip.generateAsync({type:"blob"}).then(function(content){

                var blobUrl = window.URL.createObjectURL(content);
                var anchor = document.createElement('a');
                anchor.download = "planimation.zip";
                anchor.href = blobUrl;
                anchor.click();
                });
                planimation.canvasRecorder = RecordRTC(planimation.canvas,{type:'canvas'});
                planimation.unlockScreen();
            }
        };
    }


    // download files automatically 
    planimation.postBlob = (blob, filename) => {
        var blobUrl = window.URL.createObjectURL(blob);
        var anchor = document.createElement('a');
        anchor.download = filename;
        anchor.href = blobUrl;
        anchor.click();
        planimation.canvasRecorder = RecordRTC(planimation.canvas, {
            type: 'canvas'
        });
        planimation.unlockScreen();
    };
  
})();
/** (May 17, 2020) Movie Download update **/