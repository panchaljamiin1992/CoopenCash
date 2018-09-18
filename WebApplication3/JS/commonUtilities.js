
transporterQuote

    .factory('$commonUtilities', function ($rootScope, $timeout, cfpLoadingBar) {

        var root = {};

        root.uploadFile = function (serverPath, imageFolder, fileName, file, taskID, typeID, okCB, overCB) {
            cfpLoadingBar.start();
            var data = new FormData();
            data.append(fileName, file);
            if (file) {
                $.ajax({
                    url: getFilePath() + "/FileUploadHandler.ashx?path=" + imageFolder + "&taskID=" + taskID + "&typeID=" + typeID + "",// serverPath + "FileUploadHandler.ashx?path=" + imageFolder + "&companyID=" + companyID + "&typeID=5",//isProfilePic,
                    type: 'POST',
                    data: data,
                    contentType: false,
                    async: false,
                    processData: false,
                    success: function (jResponse) {
                        console.log(jResponse);
                        var resObj = JSON.parse(jResponse);
                        if (resObj.Error == "0") {
                            okCB(resObj.FileName);
                        }
                        cfpLoadingBar.complete();
                        overCB();
                    },
                    error: function (jError) {
                        console.log(jError);
                        if (jError.status == 200 && jError.responseText != "-") {
                            okCB(jError.responseText);
                        } else {
                            //errorOpenSWALPopup("Please upload an image!");
                        }
                        cfpLoadingBar.complete();
                        overCB();
                    },
                })
            }
        }

        root.uploadCroppedURI = function (serverPath, imageFolder, fileName, dataURI, companyID, typeID, okCB, overCB) {
            // Cropped image comes in the form of URI. So convert URI -> blob -> file.
            cfpLoadingBar.start();
            var myBlob = root.dataURItoBlob(dataURI);
            var myFile = new File([myBlob], fileName);
            cfpLoadingBar.complete();

            // Upload the file. Profile pic will also be updated in database.
            root.uploadFile(
                serverPath, imageFolder, fileName, myFile, companyID, typeID, okCB, overCB
            );
        }

        root.dataURItoBlob = function (dataURI) {
            //console.log(dataURI);
            var binary = atob(dataURI.split(',')[1]);
            var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];
            var array = [];
            for (var i = 0; i < binary.length; i++) {
                array.push(binary.charCodeAt(i));
            }
            return new Blob([new Uint8Array(array)], { type: mimeString });
        };

        root.getAndCheckCookie = function (cookieName) {

            if (cookieName == "") {
                return null;
            }

            var cookieVal = getCookie(cookieName);
            if (cookieVal == null || cookieVal == "null" || cookieVal == "" || cookieVal == undefined) {
                return null;
            }

            return cookieVal;
        }

        return root;

    });