/* (May 17, 2020) Movie Download update */
mergeInto(LibraryManager.library, {
  
  // start screen recording 
  StartRecording: function() {
    window.planimation_downloadMovie.startRecording();
  },

  // lock screen during downloading
  LockScreen: function() {
    window.planimation_downloadMovie.lockScreen();
  },

  // stop recording and generate gif file
  OutputGIF: function() {
  	window.planimation_downloadMovie.outputGIF();
  },

  //Added by Mengyi Fan
  // stop recording and generate png file
  OutputPNG: function() {
  	window.planimation_downloadMovie.outputPNG();
  },



  // stop recording and generate webm file
  OutputWebM: function() {
  	window.planimation_downloadMovie.outputWebM();
  },

  // stop recording and generate mp4 file (currently not used)
  OutputMP4: function() {
  	window.planimation_downloadMovie.outputMP4();
  }
});
/** (May 17, 2020) Movie Download update **/