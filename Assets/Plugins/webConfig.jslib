mergeInto(LibraryManager.library, {

    GetUploadApi: function () {
        console.log("in GetPort function", window.location.hostname);
        var api_url = "/upload/pddl";

        if (window.location.hostname == "localhost") {
            api_url = "http://localhost:8000/upload/pddl";
        } else {
            api_url = "/upload/pddl";
        }

        var bufferSize = lengthBytesUTF8(api_url) + 1
        var buffer = _malloc(bufferSize);
        stringToUTF8(api_url, buffer, bufferSize)
        return buffer;
    },
});