/* (May 17, 2020) Movie Download update */
(function () {

    // namespace for planimation
    window.planimation_downloadMovie = window.planimation_downloadMovie || {};

    // shorthand for namespace object
    var planimation = window.planimation_downloadMovie;

    planimation.workerPath = 'https://archive.org/download/ffmpeg_asm/ffmpeg_asm.js';

    window.addEventListener("load", () => {
        var canvas = document.getElementById('#canvas');
        // using RecordRTC library
        planimation.canvasRecorder = RecordRTC(canvas, {
            type: 'canvas'
        });
    });

    planimation.startRecording = () => {
        planimation.canvasRecorder.startRecording();
        gameInstance.SendMessage("Canvas", "RecordPlayback");
    };
    
    planimation.outputGIF = (filetype) => {
        planimation.canvasRecorder.stopRecording(function() {
            planimation.convertStreams(planimation.canvasRecorder.getBlob());
        });
    }

    planimation.outputWebM = (filetype) => {
        planimation.canvasRecorder.stopRecording(function() {
            planimation.postBlob(planimation.canvasRecorder.getBlob(), "planimation.webm");
        });
    }

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

    planimation.convertStreams = (videoBlob) => {
        
        console.log("called converter");

        var aab;
        var buffersReady;
        var workerReady;
        var posted;

        var postMessage = function() {
            posted = true;

            planimation.worker.postMessage({
                type: 'command',
                arguments: '-i video.webm -b:v 64k -strict experimental output.gif'.split(' '),
                files: [
                    {
                        data: new Uint8Array(aab),
                        name: 'video.webm'
                    }
                ]
            });
        };

        var fileReader = new FileReader();
        fileReader.onload = function() {
            aab = this.result;
            postMessage();
        };
        fileReader.readAsArrayBuffer(videoBlob);

        if (!planimation.worker) {
            planimation.worker = planimation.processInWebWorker();
        }

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
            } else if (message.type == "done") {
                console.log(JSON.stringify(message));

                var result = message.data[0];
                console.log(JSON.stringify(result));

                var blob = new File(
                    [result.data], 
                    'test.gif', 
                    { type: 'image/gif'}
                );

                console.log(JSON.stringify(blob));

                planimation.postBlob(blob, "planimation.gif");
            }
        };
    }

    planimation.postBlob = (blob, filename) => {
        var blobUrl = window.URL.createObjectURL(blob);
        var anchor = document.createElement('a');
        anchor.download = filename;
        anchor.href = blobUrl;
        anchor.click();
    };
  
})();
/** (May 17, 2020) Movie Download update **/