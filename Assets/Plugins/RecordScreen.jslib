/* (May 17, 2020) Movie Download update */
mergeInto(LibraryManager.library, {
  
  // start screen recording
  StartRecording: function() {
    window.planimation_downloadMovie.startRecording();
  },

  // stop recording and generate gif file
  OutputGIF: function() {
  	window.planimation_downloadMovie.outputGIF();
  },

  // stop recording and generate webm file
  OutputWebM: function() {
  	window.planimation_downloadMovie.outputWebM();
  }
});
/** (May 17, 2020) Movie Download update **/