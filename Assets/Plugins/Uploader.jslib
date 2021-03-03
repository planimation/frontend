var UploaderPlugin = {
  UploaderCaptureClick: function(extensionptr) {
    var extension = Pointer_stringify(extensionptr);
    if (!document.getElementById('UploaderInput')) {
      var fileInput = document.createElement('input');
      fileInput.setAttribute('type', 'file');
      fileInput.setAttribute('id', 'UploaderInput');
      fileInput.setAttribute('accept', extension);
      fileInput.style.visibility = 'hidden';
      fileInput.onclick = function (event) {
        this.value = null;
      };
      fileInput.onchange = function (event) {
        SendMessage('Canvas', 'SetFileName',event.target.files[0].name);
        SendMessage('Canvas', 'FileSelected', URL.createObjectURL(event.target.files[0]));
      }
      document.body.appendChild(fileInput);
    } else{
      document.getElementById('UploaderInput').setAttribute('accept', extension);
    }
    var OpenFileDialog = function() {
      document.getElementById('UploaderInput').click();
      document.getElementById('#canvas').removeEventListener('click', OpenFileDialog);
    };
    document.getElementById('#canvas').addEventListener('click', OpenFileDialog, false);
  }
};
mergeInto(LibraryManager.library, UploaderPlugin);
