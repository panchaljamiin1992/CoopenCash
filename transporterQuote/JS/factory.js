
// #region "General Functions"

function isObjectEmpty(obj) {
    var isEmpty = Object.keys(obj).length === 0 && obj.constructor === Object;
    return isEmpty;
}

function getUrl() {    
     return "http://localhost:53770";
    //return "http://gap4.actplease.com";
}

// #region "Sweet Alert"

function openSWALPopup(message) {
    swal({
        title: "",
        text: message,
        type: "warning",
        confirmButtonClass: "btn-danger",
        backdrop: true,
        confirmButtonText: "Ok",
        closeOnConfirm: true
    });
}

function confirmSWALPopup(SweetAlert, text, yesCB, noCB) {
    SweetAlert.swal({
        title: "Sure?",
        text: text,
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes",
        cancelButtonText: "No",
        closeOnConfirm: true
    }, function (isConfirm) {
        if (isConfirm) {
            yesCB();
        } else {
            noCB();
        }
    });
}

function confirmSWALPopupCloseTrue(SweetAlert, text, yesCB, noCB) {
    SweetAlert.swal({
        title: 'Sure?',
        text: text,
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes",
        cancelButtonText: "No",
        closeOnConfirm: false,
        closeOnCancel: true
    }, function (isConfirm) {
        if (isConfirm) {
            yesCB();
        } else {
            noCB();
        }
    });
}


function successOpenSWALPopup(message) {
    swal({
        title: "Success",
        text: message,
        timer: 3000,
        type: "success",
        confirmButtonClass: "btn-success",
        backdrop: 'static',
        confirmButtonText: "OK",
        closeOnConfirm: true
    });
}

function errorOpenSWALPopup(message) {
    swal({
        title: "",
        text: message,
        type: "warning",
        confirmButtonClass: "btn-danger",
        backdrop: true,
        confirmButtonText: "OK",
        closeOnConfirm: true
    });
}

// #endregion

// #endregion

// General function for api calling
function mainFactory($rootScope, $timeout, $q, cfpLoadingBar) {
    var root = {};
    var myMjm = "";

    //root.pm_personInfo = {
    //    tokenID: '',
    //    userID: 0
    //};

    root.bye = function () {
        window.localStorage.removeItem('pm_info');
        window.location.href = 'main.html';
    }

    //root.setCookie = function (cookieName, value) {
    //    window.localStorage.setItem(cookieName, value);
    //}

    //root.getCookie = function (cookieName) {
    //    return window.localStorage.getItem(cookieName);
    //}

    root.getLoginInformation = function () {
        var LoginInfo = JSON.parse(window.localStorage.getItem('pm_info'));
        if (LoginInfo != null) {
            // Coming from login
            root.pm_personInfo = LoginInfo;
            //window.localStorage.removeItem('LoginInformation');
        } else {
            root.bye();
        }
    }

    /* 
     * successCB = callback function when HTTP STATUS = 200, and jResponse.error = false
     * apiFailureCB = callback function when HTTP STATUS = 200, and jResponse.error = true
     * failureCB = callback function when HTTP STATUS != 200
     */
    root.callAPI5 = function (apiName, data, successCB, apiFailureCB, failureCB) {

        var myToken = "";
        var infoCookie = window.localStorage.getItem('pm_info');

        if (apiName != "/api/login/login" && apiName != "/api/login/password/forgot") {
            if (infoCookie == null) {
                root.bye();
                return;
            } else {
                var infoCookieVal = JSON.parse(infoCookie);
                if (infoCookieVal != null && !isObjectEmpty(infoCookieVal)) {
                    myToken = infoCookieVal.tokenID;
                } else {
                    root.bye();
                    return;
                }
            }
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
        cfpLoadingBar.start();
        $.ajax({
            url: getUrl() + apiName,
            type: 'POST',
            data: JSON.stringify(data),
            //async: false,
            contentType: 'application/json; charset=utf-8',
            headers: { "majama60": myToken },
            success: function (jResponse) {
                if (jResponse.error == false) {
                    cfpLoadingBar.complete();
                    successCB(jResponse);
                } else {
                    apiFailureCB(jResponse);
                    cfpLoadingBar.complete();
                }
            },
            error: function (jError) {
                if (jError.status == 403 && jError.statusText == 'Token Expired') {
                    root.bye();
                    cfpLoadingBar.complete();
                } else {
                    failureCB(jError);
                    cfpLoadingBar.complete();
                }
            }
        });
    }

    return root;
}
//)