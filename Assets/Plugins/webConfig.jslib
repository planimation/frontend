mergeInto(LibraryManager.library, {

    GetApiPort: function () {
        console.log("in GetPort function", window.location.hostname);
        var api_url = "http://localhost:8080";

        if (window.location.hostname == "localhost") {
            api_url = "http://localhost:8000";
        } else {
            api_url = "https://planning-visualisation-solver.herokuapp.com";
        }

        var bufferSize = lengthBytesUTF8(api_url) + 1
        var buffer = _malloc(bufferSize);
        stringToUTF8(api_url, buffer, bufferSize)
        return buffer;
    },
});