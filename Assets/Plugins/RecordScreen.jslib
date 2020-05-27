mergeInto(LibraryManager.library, {
  
  // for the page: Start.unity
  StartRecording: function() {
    window.planimation_downloadMovie.startRecording();
  },

  // for the page: VFGUploader.unity
  StopRecording: function() {
  	window.planimation_downloadMovie.stopRecording();
  }

});