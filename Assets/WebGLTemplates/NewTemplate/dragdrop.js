/* (Apr 23, 2020) Drag and Drop update */
(function () {

  // namespace for planimation
  window.planimation_dragdrop = window.planimation_dragdrop || {};

  // shorthand for namespace object
  var planimation = window.planimation_dragdrop;
  
  // namespace for sanitizer
  planimation.sanitize = planimation.sanitize || {};
  
  // sanitizer
  planimation.sanitize.encode = (str) => {
    return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
  };
  
  // processes PDDLFile (for Start.unity)
  planimation.uploadPDDLFile = () => {
    // open the modal window only when multiple files are dropped
    if(planimation.files.length > 1) { 
      planimation.uploadMultipleFiles();
    // when a single file is dropped
    } else {
      var regexp = /\.pddl$/;
      if(!planimation.files[0].name.match(regexp)) {
        //alert("Please put in pddl files!");
        // code by Jayan - 14th May 2020
        planimation.customAlertBox("Incorrect File Type", "<p>Please upload PDDL (.pddl) files only.</p>");
        return;
      }
      planimation.uploadSingleFile();
    }
  };
  
  // processes VFG file (for VFGUploader.unity)
  planimation.uploadVFGFile = () => {
    var regexp = /\.vfg$/;
    if(!planimation.files[0].name.match(regexp)) {
      //alert("Please put in vfg files!");
      // code by Jayan - 14th May 2020
      planimation.customAlertBox("Incorrect File Type", "<p>Please upload VFG (.vfg) file only.</p>");
      return;
    }
    // take the first file and upload it
    planimation.uploadSingleFile();
  };

  // calls file loader for single file drop
  planimation.uploadSingleFile = () => {
    var offsetX = planimation.offsetX;
    var offsetY = window.innerHeight - planimation.offsetY;
    planimation.fileLoaderSingle(planimation.files[0], offsetX, offsetY);
  };

  // calls file loader for multiple file drop
  planimation.uploadMultipleFiles = () => {
    // hyper link id
    var open = document.getElementById("modal-open");
    //table-body
    var typeModal = document.getElementById("file-type");
    typeModal.innerHTML = "";

    // code by Jayan - 14th May 2020
    // clear any error message in upload file modal
    var clearErrorMsg = document.getElementById("upload_error_msg"); 
    clearErrorMsg.innerHTML = "";

    var regexp = /\.pddl$/;
    var fileIndex = 1;
    var files = planimation.files;
    for(var i = 0; i < files.length; i++) {
      var file = files[i];
      if(!file.name.match(regexp)) {
        var sanitizedName = planimation.sanitize.encode(file.name);
        // alert("[Invalid file: " + sanitizedName + " ] " + "Please put in pddl files!");
        // code by Jayan - 14th May 2020
        var msg = 
          '<b>Alert:</b> File <b>"' 
          + sanitizedName 
          + '"</b> invalid format. Please upload PDDL (.pddl) files only.';
        planimation.customErrorDialog("upload_error_msg", msg);
      } else {
        //will append the UI elements
        typeModal.appendChild(planimation.createFileDiv(file, fileIndex));
        // code by Jayan - 14th May 2020
        planimation.btnListeners(file, fileIndex);
        fileIndex++;
      }
    }  
    open.click();
  };

  // loads single file data
  planimation.fileLoaderSingle = (file, x, y) => {
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
  planimation.fileLoaderMultiple = (file) => {
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
  // code by Jayan - 14th May 2020
  planimation.createFileDiv = (file, index) => {
    var table_row = document.createElement('tr');
    var sanitizedName = planimation.sanitize.encode(file.name);
    table_row.innerHTML = 
      "<td ><i class='far fa-file-powerpoint'></i> </td><td class='text-left' id='file_'" 
      + index 
      + "><i>" 
      + sanitizedName 
      + "</i></td><td class='text-center'>" 
      + "<div class='btn-group' role='group'>" 
      + "<button type='button' class='btn btn-secondary' id='file_" 
      + index 
      + "_domain'>Domain</button>" 
      + "<button type='button' class='btn btn-secondary' id='file_" 
      + index 
      + "_problem'>Problem</button>" 
      + "<button type='button' class='btn btn-secondary' id='file_" 
      + index + "_animation'>Animation</button>" 
      + "</div>" 
      + "</td>";
    return table_row;
  };

  // code by Jayan - 14th May 2020
  /* Event listners for the domain, problem and animation file buttons */
  planimation.btnListeners = (file, index) => {
    var buttonDomain = document.getElementById("file_" + index + "_domain");
    var buttonProblem = document.getElementById("file_" + index + "_problem");
    var buttonAnimation = document.getElementById("file_" + index + "_animation");
    
    buttonDomain.addEventListener("click", function(e) {
      buttonDomain.classList.remove('btn-secondary');
      buttonDomain.classList.add('btn-success');

      buttonProblem.classList.remove('btn-success');
      buttonAnimation.classList.remove('btn-success');

      buttonProblem.classList.add('btn-secondary');
      buttonAnimation.classList.add('btn-secondary');
      file.contentType = "Domain";
    });

    buttonProblem.addEventListener("click", function(e) {
      buttonProblem.classList.remove('btn-secondary');
      buttonProblem.classList.add('btn-success');

      buttonDomain.classList.remove('btn-success');
      buttonAnimation.classList.remove('btn-success');

      buttonDomain.classList.add('btn-secondary');
      buttonAnimation.classList.add('btn-secondary');
      file.contentType = "Problem";
    });

    buttonAnimation.addEventListener("click", function(e) {
      buttonAnimation.classList.remove('btn-secondary');
      buttonAnimation.classList.add('btn-success');

      buttonProblem.classList.remove('btn-success');
      buttonDomain.classList.remove('btn-success');

      buttonProblem.classList.add('btn-secondary');
      buttonDomain.classList.add('btn-secondary');
      file.contentType = "Animation";
    });
  };

  // code by Jayan - 14th May 2020
  /* Generate custom alert box for displaying error messages */
  planimation.customAlertBox = (title, message) => {
    var alert_title = document.getElementById('alert_title');
    var alert_message = document.getElementById('alert_msg');

    alert_title.innerHTML = title;
    alert_message.innerHTML = message;

    $("#modal_alert").modal();
  };

  // code by Jayan - 14th May 2020
  // Generate custom eror dialog message
  planimation.customErrorDialog = (id, message) => {
    var error_msg = document.getElementById(id);
    error_msg.innerHTML += "<div class='alert alert-danger' role='alert'><i class='fas fa-exclamation-triangle'></i> " + message + "</div>";
  };

  // waits until document is loaded
  window.addEventListener("load", () => {
  
    // prevents default before dropping item
    document.addEventListener("dragover", (e) => {
      e.stopPropagation();
      e.preventDefault();
    });

    // set file information and call a scene checker
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
