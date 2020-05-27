/* (May 17, 2020) Movie/gif */
(function () {
  // namespace for planimation
  window.planimation_downloadMovie = window.planimation_downloadMovie || {};

  // shorthand for namespace object
  var planimation = window.planimation_downloadMovie;
  
  // waits until document is loaded
  window.addEventListener("load", () => {
    
    var canvas = document.getElementById("#canvas");
    var start = document.getElementById("record");
    var stop = document.getElementById("stop");
    const stream = canvas.captureStream();
    // much noise occurs when using vp9 encode
    var recorder = new MediaRecorder(stream, { mimeType: "video/webm;codecs=vp8" });

    /*
    start.addEventListener("click", () => {
      recorder.start();
    });

    stop.addEventListener("click", () => {
      recorder.stop();
    });*/
    //var anchor = document.getElementById('downloadlink');
    recorder.ondataavailable = (e) => {
      var videoBlob = new Blob([e.data], { type: e.data.type });
      //blobUrl = window.URL.createObjectURL(videoBlob);
      //anchor.download = 'movie.webm';
      //anchor.href = blobUrl;
      //anchor.style.display = 'block';
      planimation.convertStreams(videoBlob);
    };

    planimation.startRecording = () => {
      recorder.start();
      // need callback?
    }

    planimation.stopRecording = () => {
      recorder.stop();
    }

    // worker -> asynchronous javascript caller?
    planimation.workerPath = 'https://archive.org/download/ffmpeg_asm/ffmpeg_asm.js';
    
    // should consider this?
    /*
    if(document.domain == 'localhost') {
        workerPath = location.href.replace(location.href.split('/').pop(), '') + 'ffmpeg_asm.js';
    }*/

    planimation.processInWebWorker = () => {
        // URL = window.URL? see browser dependency
        var blob = URL.createObjectURL(new Blob(['importScripts("' + planimation.workerPath + '");var now = Date.now;function print(text) {postMessage({"type" : "stdout","data" : text});};onmessage = function(event) {var message = event.data;if (message.type === "command") {var Module = {print: print,printErr: print,files: message.files || [],arguments: message.arguments || [],TOTAL_MEMORY: message.TOTAL_MEMORY || false};postMessage({"type" : "start","data" : Module.arguments.join(" ")});postMessage({"type" : "stdout","data" : "Received command: " +Module.arguments.join(" ") +((Module.TOTAL_MEMORY) ? ".  Processing with " + Module.TOTAL_MEMORY + " bits." : "")});var time = now();var result = ffmpeg_run(Module);var totalTime = now() - time;postMessage({"type" : "stdout","data" : "Finished processing (took " + totalTime + "ms)"});postMessage({"type" : "done","data" : result,"time" : totalTime});}};postMessage({"type" : "ready"});'], {
            type: 'application/javascript'
        }));

        var worker = new Worker(blob);
        URL.revokeObjectURL(blob);
        return worker;
    }

    var worker;

    // main converter: webm -> mp4
    planimation.convertStreams = (videoBlob) => {
        var aab;
        var buffersReady;
        var workerReady;
        var posted;

        var fileReader = new FileReader();
        fileReader.onload = function() {
            aab = this.result;
            planimation.postMessage();
        };
        fileReader.readAsArrayBuffer(videoBlob);

        if (!planimation.worker) {
            planimation.worker = planimation.processInWebWorker();
        }

        planimation.worker.onmessage = function(event) {
            var message = event.data;
            if (message.type == "ready") {
                console.log('<a href="'+ planimation.workerPath +'" download="ffmpeg-asm.js">ffmpeg-asm.js</a> file has been loaded.');

                workerReady = true;
                if (buffersReady)
                    planimation.postMessage();
            } else if (message.type == "stdout") {
                console.log(message.data);
            } else if (message.type == "start") {
                console.log('<a href="'+ planimation.workerPath +'" download="ffmpeg-asm.js">ffmpeg-asm.js</a> file received ffmpeg command.');
            } else if (message.type == "done") {
                console.log(JSON.stringify(message));

                var result = message.data[0];
                console.log(JSON.stringify(result));

                // converted data?
                var blob = new File([result.data], 'test.mp4', {
                    type: 'video/mp4'
                });

                console.log(JSON.stringify(blob));

                // output mp4 file after asynchronous worker is done 
                planimation.PostBlob(blob);
            }
        };
        planimation.postMessage = () => {
            posted = true;

            planimation.worker.postMessage({
                type: 'command',
                arguments: '-i video.webm -c:v mpeg4 -b:v 6400k -strict experimental output.mp4'.split(' '),
                files: [
                    {
                        data: new Uint8Array(aab),
                        name: 'video.webm'
                    }
                ]
            });
        };
    }

    planimation.PostBlob = (blob) => {
        var blobUrl = window.URL.createObjectURL(blob);
        var anchor = document.createElement('a');
        anchor.download = 'movie.mp4';
        anchor.href = blobUrl;
        anchor.style.display = 'block';
        anchor.click();
        /*
        var video = document.createElement('video');
        video.controls = true;

        var source = document.createElement('source');
        source.src = URL.createObjectURL(blob);
        source.type = 'video/mp4; codecs=mpeg4';
        video.appendChild(source);

        video.download = 'Play mp4 in VLC Player.mp4';

        inner.appendChild(document.createElement('hr'));
        var h2 = document.createElement('h2');
        h2.innerHTML = '<a href="' + source.src + '" target="_blank" download="Play mp4 in VLC Player.mp4" style="font-size:200%;color:red;">Download Converted mp4 and play in VLC player!</a>';
        inner.appendChild(h2);
        h2.style.display = 'block';
        inner.appendChild(video);

        video.tabIndex = 0;
        video.focus();
        video.play();

        document.querySelector('#record-video').disabled = false;
        */
    }

    var logsPreview = document.getElementById('logs-preview');

    function log(message) {
        var li = document.createElement('li');
        li.innerHTML = message;
        logsPreview.appendChild(li);

        li.tabIndex = 0;
        li.focus();
    }

  });  
})();

/** (May 17, 2020) Movie/gif **/
