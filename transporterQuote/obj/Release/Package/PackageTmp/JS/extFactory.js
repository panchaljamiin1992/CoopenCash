function getUrl() {
    return "http://localhost:53770";
   // return "http://gap3.actplease.com/"
}


function extFactory($rootScope, $timeout, $q) {

    var root = {};
    /* 
     * successCB = callback function when HTTP STATUS = 200, and jResponse.error = false
     * apiFailureCB = callback function when HTTP STATUS = 200, and jResponse.error = true
     * failureCB = callback function when HTTP STATUS != 200
     */
    root.callAPI5 = function (apiName, data, successCB, apiFailureCB, failureCB, myToken) {

        if (myToken == null || myToken == "") {
            myToken = "SubmitInquiry";
        }

        if (data == null) {
            data = {};
        }

        if (apiFailureCB == null) {
            apiFailureCB = function (apiResponse) {
                console.log(JSON.stringify(apiResponse.message));
            }
        }

        if (failureCB == null) {
            failureCB = function (jError) {
                console.log(JSON.stringify(jError));
            }
        }

        $.ajax({
            url: getUrl() + apiName,
            type: 'POST',
            data: JSON.stringify(data),
            //async: false,
            contentType: 'application/json; charset=utf-8',
           // headers: { "majama60": myToken, "6agan": myToken },
            success: function (jResponse) {
                if (jResponse.error == false) {
                    successCB(jResponse);
                } else {
                    apiFailureCB(jResponse);
                }
            },
            error: function (jError) {
                failureCB(jError);
            }
        });
    }

    return root;

}