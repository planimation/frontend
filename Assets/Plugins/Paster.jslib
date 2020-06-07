//
mergeInto(LibraryManager.library, {
 
 
  PasteHereWindow: function () {
    var pastedtext = prompt("Please paste here:", "");
    if(!pastedtext) {
    	return false;
    }
    SendMessage("Canvas", "GetPastedText", pastedtext);
  },
 
});