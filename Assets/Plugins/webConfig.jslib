mergeInto(LibraryManager.library, {

    GetUploadApi: function (api_url) {
        console.log("in GetPort function", window.location.hostname);
        var api_url = Pointer_stringify(api_url);

<<<<<<< HEAD
//        if (window.location.hostname == "localhost") {
//            api_url = "http://localhost:8000/upload/pddl";
//        } else {
//            api_url = "/upload/pddl";
//        }
=======
        if (window.location.hostname != "planimation.planning.domain") {
            api_url = "http://" + window.location.hostname + ":8000" + api_url;
        } else {
            api_url = api_url;
        }
>>>>>>> update webCongfig.jslib and standalone capture image

        var bufferSize = lengthBytesUTF8(api_url) + 1
        var buffer = _malloc(bufferSize);
        stringToUTF8(api_url, buffer, bufferSize)
        return buffer;
    },
});