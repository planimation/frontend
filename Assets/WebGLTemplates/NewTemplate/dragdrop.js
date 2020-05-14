/* (Apr 23, 2020) Drag and Drop update */
(function () {

  // namespace for planimation
  window.planimation_dragdrop = window.planimation_dragdrop || {};

  // shorthand for namespace object
  var planimation = window.planimation_dragdrop;
  
  // namespace for sanitizer
  planimation.sanitize = planimation.sanitize || {};
  
  // sanitizer
  planimation.sanitize.encode = function (str) {
    return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
  };

  // shorthand for namespace object
  var planimation = window.planimation_dragdrop;
  
  planimation.uploadPDDLFile = function () {
    // open the modal window only when multiple files are dropped
    if(planimation.files.length > 1) { 
      planimation.uploadMultipleFiles();
    // when a single file is dropped
    } else {
      var regexp = /\.pddl$/;
      if(!planimation.files[0].name.match(regexp)) {
        alert("Please put in pddl files!");
        return;
      }
      planimation.uploadSingleFile();
    }
  };
  
  planimation.uploadVFGFile = function () {
    var regexp = /\.vfg$/;
    if(!planimation.files[0].name.match(regexp)) {
      alert("Please put in vfg files!");
      return;
    }
    // take the first file and upload it
    planimation.uploadSingleFile();
  };

  // calls file loader for single file drop
  planimation.uploadSingleFile = function () {
    var offsetX = planimation.offsetX;
    var offsetY = window.innerHeight - planimation.offsetY;
    planimation.fileLoaderSingle(planimation.files[0], offsetX, offsetY);
  };

  // calls file loader for multiple file drop
  planimation.uploadMultipleFiles = function () {
    var open = document.getElementById("modal-open");
    var typeModal = document.getElementById("file-type");
    typeModal.innerHTML = "";
    var regexp = /\.pddl$/;
    var fileIndex = 1;
    var files = planimation.files;
    for(var i = 0; i < files.length; i++) {
      var file = files[i];
      if(!file.name.match(regexp)) {
        var sanitizedName = planimation.sanitize.encode(file.name);
        alert("[Invalid file: " + sanitizedName + " ] " + "Please put in pddl files!");
      } else {
        typeModal.appendChild(planimation.createFileDiv(file, fileIndex));
        fileIndex++;
      }
    }

    if(typeModal.hasChildNodes()) {
      open.click();
    }
  };

  // loads single file data
  planimation.fileLoaderSingle = function (file, x, y) {
    var reader = new FileReader();
    reader.readAsText(file);
    reader.addEventListener("load", (e) => {
      var json = {
        "name": file.name,
        "x": x,
        "y": y,
        "data": e.target.result,
      };
      gameInstance.SendMessage("Canvas", "DropSingleFile", JSON.stringify(json));
    });
  };

  // loads multiple file data
  planimation.fileLoaderMultiple = function (file) {
    var reader = new FileReader();
    reader.readAsText(file);
    reader.addEventListener("load", (e) => {
      var type = file.contentType;
      var json = {
        "name": file.name,
        "data": e.target.result,
        "type": type
      };
      gameInstance.SendMessage("Canvas", "DropMultipleFiles", JSON.stringify(json));  
    });
  };

  // appends type-select menu on the modal window according to file(s)
  planimation.createFileDiv = function (file, index) {
    var sanitizedName = planimation.sanitize.encode(file.name);
    var divFileName = document.createElement('div');
    divFileName.setAttribute("id", "file_" + index);
    divFileName.setAttribute("class", "divFile");
    divFileName.textContent = "[ File " + index + " ] " +  sanitizedName;
    divFileName.setAttribute("style", "font-weight: bold;");

    var divFileType = document.createElement('div');
    divFileType.setAttribute("class", "divFile");
    
    var buttonDomain = document.createElement('span');
    var buttonProblem = document.createElement('span');
    var buttonAnimation = document.createElement('span');
    
    buttonDomain.textContent = "Domain";
    buttonProblem.textContent = "Problem";
    buttonAnimation.textContent = "Animation";
    
    buttonDomain.classList.add("typeButton");
    buttonProblem.classList.add("typeButton");
    buttonAnimation.classList.add("typeButton");
    
    buttonDomain.setAttribute("id", "file_" + index + "_domain");
    buttonProblem.setAttribute("id", "file_" + index + "_problem");
    buttonAnimation.setAttribute("id", "file_" + index + "_animation");
    
    buttonDomain.addEventListener("click", () => {
      buttonDomain.setAttribute("style", "background-color: #117AC8;");
      buttonProblem.setAttribute("style", "background-color: #9C9C9C;");
      buttonAnimation.setAttribute("style", "background-color: #9C9C9C;");
      file.contentType = "Domain";
    });

    buttonProblem.addEventListener("click", () => {
      buttonDomain.setAttribute("style", "background-color: #9C9C9C;");
      buttonProblem.setAttribute("style", "background-color: #117AC8;");
      buttonAnimation.setAttribute("style", "background-color: #9C9C9C;");
      file.contentType = "Problem";
    });

    buttonAnimation.addEventListener("click", () => {
      buttonDomain.setAttribute("style", "background-color: #9C9C9C;");
      buttonProblem.setAttribute("style", "background-color: #9C9C9C;");
      buttonAnimation.setAttribute("style", "background-color: #117AC8;");
      file.contentType = "Animation";
    });
    
    divFileType.appendChild(buttonDomain);
    divFileType.appendChild(buttonProblem);
    divFileType.appendChild(buttonAnimation);
    divFileName.appendChild(divFileType);
    return divFileName;
  };

  // waits until document is loaded
  window.addEventListener("load", () => {
  
    // prevents default before dropping item
    document.addEventListener("dragover", (e) => {
      e.stopPropagation();
      e.preventDefault();
    });

    // opens the modal window of type-select
    document.addEventListener("drop", (e) => {
      e.stopPropagation();
      e.preventDefault();
      planimation.files = e.dataTransfer.files;
      planimation.offsetX = e.offsetX;
      planimation.offsetY = e.offsetY;
      
      // check the current Unity scene
      gameInstance.SendMessage("Canvas", "CheckCurrentScene");
    });

    // represents the close button on the modal window
    var close = document.getElementById("modal-close");

    // represents the submit button on the modal window
    var submitButton = document.getElementById("modal-submit");
    
    submitButton.addEventListener("click", () => {
      var files = planimation.files;
      for(var i = 0; i < files.length; i++) {
        if(files[i].contentType != null) {
          planimation.fileLoaderMultiple(files[i]);
        }
      }
      close.click();
    });
    
    // represents the cancel button on the modal window
    var cancelButton = document.getElementById("modal-cancel");
    cancelButton.addEventListener("click", () => {
      close.click();
    });
  });  
})();
/** (Apr 23, 2020) Drag and Drop update **/