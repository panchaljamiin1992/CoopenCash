function getPath() {
     var path = "http://localhost:53770/";
   // var path = "http://gap3.actplease.com/";
    //var path = "http://portal2.actplease.com/";
    //var path = "http://192.168.100.110:181/warmdorm/";
    return path;
}

function getFilePath() {
    var path = "http://localhost:53770";
   // var path = "http://gap3.actplease.com/";
    //var path = "http://portal.actplease.com";
    //var path = "http://192.168.100.102:181/PCBSite";
    return path;
}

function setCookie(Name, Value) {
    window.localStorage.setItem(Name, Value);
}

function getCookie(Name) {
    return window.localStorage.getItem(Name);
}

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
        title: "Uh oh!",
        text: message,
        type: "warning",
        confirmButtonClass: "btn-danger",
        backdrop: true,
        confirmButtonText: "OK",
        closeOnConfirm: true
    });
}

var errorStr = "Something went wrong.  Please try again later.";

Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
}

function testRegexDecimal(testVal) {
    var decimalRegex = new RegExp("^[0-9]{1,5}(.[0-9]{1,2})?$"); // ^[0-9]{1,5}+(\.[0-9]{1,2})?$
    var isMatching = decimalRegex.test(testVal);
    return isMatching;
}

function testRegexInteger(testVal, maxLength) {
    if (maxLength == null) {
        maxLength = 7;
    }
    var integerRegex = new RegExp("^[0-9]{1,7}?$");
    var isMatching = integerRegex.test(testVal);
    return isMatching;
}
