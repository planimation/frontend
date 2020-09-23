var DownloaderPlugin ={
    Download : function(textptr, fileTypeptr,fileNameptr) {
    var text = Pointer_stringify(textptr);
    var fileType = Pointer_stringify(fileTypeptr);
    var fileName = Pointer_stringify(fileNameptr);
    var blob = new Blob([text], { type: fileType });

    var a = document.createElement('a');
    a.download = fileName;
    a.href = URL.createObjectURL(blob);
    a.dataset.downloadurl = [fileType, a.download, a.href].join(':');
    a.style.display = "none";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    setTimeout(function() { URL.revokeObjectURL(a.href); }, 1500);
    }
};
mergeInto(LibraryManager.library, DownloaderPlugin);

var ZipDownloaderPlugin = {
    ZipDownload: function(str, fn, fileType) {
        var msg = Pointer_stringify(str);
        var fname = Pointer_stringify(fn);
        var contentType;
        if(fileType == "png"){
            contentType = "application/zip";
        } else if (fileType == "webm"){
            contentType = "video/webm";
        } else if (fileType == "gif"){
            contentType = "image/gif";
        }
        
        function fixBinary (bin)
        {
            var length = bin.length;
            var buf = new ArrayBuffer(length);
            var arr = new Uint8Array(buf);
            for (var i = 0; i < length; i++)
            {
                arr[i] = bin.charCodeAt(i);
            }
            return buf;
        }
        var binary = fixBinary(atob(msg));
        var data = new Blob([binary], {type: contentType});
        var link = document.createElement('a');
        link.download = fname;
        link.innerHTML = 'DownloadFile';
        link.setAttribute('id', 'DownloaderLink');
        if(window.webkitURL != null)
        {
            link.href = window.webkitURL.createObjectURL(data);
        }
        else
        {
            link.href = window.URL.createObjectURL(data);
            link.onclick = function()
            {
                var child = document.getElementById('DownloaderLink');
                child.parentNode.removeChild(child);
            };
            link.style.display = 'none';
            document.body.appendChild(link);
        }
        link.click();
    }
};
mergeInto(LibraryManager.library, ZipDownloaderPlugin);