/* (Apr 23, 2020) Drag and Drop update */
// an object to hold drop event properties
var planimation_dragdrop = {};

window.addEventListener("load", () => {
  // prevents default before dropping item
  document.addEventListener("dragover", function(e) {
    e.stopPropagation();
    e.preventDefault();
  });

  // opens the modal window of type-select
  document.addEventListener("drop", function (e) {
    e.stopPropagation();
    e.preventDefault();
    planimation_dragdrop.files = e.dataTransfer.files;
    planimation_dragdrop.offsetX = e.offsetX;
    planimation_dragdrop.offsetY = e.offsetY;
    
    // check the current Unity scene
    gameInstance.SendMessage("Canvas", "CheckCurrentScene");  
  });

  // represents the close button on the modal window
  var close = document.getElementById("modal-close");

  // represents the submit button on the modal window
  var submitButton = document.getElementById("modal-submit");
  submitButton.addEventListener("click", function(e) {
    for(var i = 0; i < planimation_dragdrop.files.length; i++) {
      if(planimation_dragdrop.files[i].contentType != null) {
        fileLoaderMultiple(planimation_dragdrop.files[i]);
      }
    }
    close.click();
    planimation_dragdrop = {};
  });

  // represents the cancel button on the modal window
  var cancelButton = document.getElementById("modal-cancel");
  cancelButton.addEventListener("click", function(e) {
    close.click();
    planimation_dragdrop = {};
  });
});


function uploadPDDLFile() {
  // open the modal window only when multiple files are dropped
  if(planimation_dragdrop.files.length > 1) { 
    uploadMultipleFiles();
  // when a single file is dropped
  } else {
    uploadSingleFile();
  }
}

function uploadVFGFile() {
  // take the first file and upload it
  uploadSingleFile();
}

function uploadSingleFile() {
  var offsetX = planimation_dragdrop.offsetX;
  var offsetY = planimation_dragdrop.offsetY;
  fileLoaderSingle(planimation_dragdrop.files[0], offsetX, window.innerHeight - offsetY);  
}

function uploadMultipleFiles() {
  var open = document.getElementById("modal-open");
  var typeModal = document.getElementById("file-type");
  typeModal.innerHTML = "";
  for(var i = 0; i < planimation_dragdrop.files.length; i++) {
    typeModal.appendChild(createFileDiv(planimation_dragdrop.files[i], i));
  }
  open.click();
}

function fileLoaderSingle(file, x, y) {
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
}

// loads file data
function fileLoaderMultiple(file) {
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
}

// appends type-select menu on the modal window according to file(s)
function createFileDiv(file, index) {
  var divFileName = document.createElement('div');
  divFileName.setAttribute("id", "file_" + index);
  divFileName.setAttribute("class", "divFile");
  divFileName.textContent = "[ File " + index + " ] " +  file.name;
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
  
  buttonDomain.addEventListener("click", function(e) {
    buttonDomain.setAttribute("style", "background-color: #117AC8;");
    buttonProblem.setAttribute("style", "background-color: #9C9C9C;");
    buttonAnimation.setAttribute("style", "background-color: #9C9C9C;");
    file.contentType = "Domain";
  });

  buttonProblem.addEventListener("click", function(e) {
    buttonDomain.setAttribute("style", "background-color: #9C9C9C;");
    buttonProblem.setAttribute("style", "background-color: #117AC8;");
    buttonAnimation.setAttribute("style", "background-color: #9C9C9C;");
    file.contentType = "Problem";
  });

  buttonAnimation.addEventListener("click", function(e) {
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
}
/** (Apr 23, 2020) Drag and Drop update **/