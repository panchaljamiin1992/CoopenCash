transporterQuote

// #region "Main"

.controller("mainController", function ($scope, $state, $uibModal, $transporterQuote, toaster) {

    // #region "Variables"

    // Password variables
    $scope.password = {
        newPassword: "",
        confirmPassword: "",
        oldPassword: ""
    }

    $scope.pm_personInfo = {
        userID: 0,
        tokenID: ""
    };

    //$scope.companyName = '';
    //$scope.configValues = [];
    $scope.isClick = false;

    //$scope.currentUserRights = {};

    $scope.pm = {
        showHeader: true,
        pageHeader: "",
        productName: "Transporter Quote",
        isMobileBrowser: /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent),
        fromInside: true
    }

    $scope.$watch(function () {
        return $transporterQuote.pm_personInfo;
    }, function (newVal, oldVal) {
        $scope.pm_personInfo = newVal;
    });

    // Check if cookie exists.  If yes, get info from token.
    var goToLogin = true;
    var loginCookie = window.localStorage.getItem('pm_info');
    if (loginCookie != null && loginCookie != "") {
        var loginCookieVal = JSON.parse(loginCookie);
        if (!isObjectEmpty(loginCookie) && loginCookie.tokenID != "") {
            $transporterQuote.getLoginInformation();
            $scope.pm_personInfo = $transporterQuote.pm_personInfo;
            goToLogin = false;
        }
    }

    // #endregion

    // #region "Functions"

    // #region "Change password"

    // closeChangePasswordPop - Close change password popup.
    $scope.closeChangePasswordPop = function () {
        $scope.password.newPassword = "";
        $scope.password.confirmPassword = "";
        $scope.password.oldPassword = "";
        modalChangePassword.close();
    }
    // #endregion

    // Go to login 
    // $state.go("login");

    // openMenu() - Open menu
    $scope.openMenu = function () {
        $scope.showMenu = !$scope.showMenu;
        if ($scope.showMenu && (document.getElementById('menuDD').style.display == 'none' || document.getElementById('menuDD').style.display == "")) {
            document.getElementById('menuDD').style.display = 'block';
            $scope.isClick = false;
        }
        else {
            document.getElementById('menuDD').style.display = 'none';
        }
    }

    // On click hide menu
    $(window).click(function () {
        //Hide the menus if visible
        if ($scope.isClick) {
            if (document.getElementById('menuDD').style.display == 'block') {
                document.getElementById('menuDD').style.display = 'none';
            }
        } else {
            $scope.isClick = true;
            $scope.showMenu = false;
        }
    });

    // actionLogout() - Logout from system
    $scope.actionLogout = function () {

        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        $transporterQuote.callAPI5(
            '/api/login/logout',
            myParams,
            function (apiResponse) {
                $("#loader").hide();
                $transporterQuote.bye();
            },
            function (apiError) {
                $("#loader").hide();
                console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(apiFailure);
            }
        );
    }

    // getConfigValue() - Get config value
    
    

    // Calling
    // getConfigValue();

    //$scope.$watch(function () {
    //    return $scope.companyName;
    //}, function (newVal, oldVal) {
    //    $scope.companyName = newVal;
    //});

    // #endregion

})

// #endregion

// #region "Login"
.controller("loginController", function ($scope, $state, $transporterQuote, toaster) {

    // #region "Variables"
    var infoCookie = window.localStorage.getItem('pm_info');
    if (infoCookie != null) {
        $transporterQuote.bye();
    }
    $scope.userName = '';
    $scope.password = '';

    // #endregion

    // #region "Functions"

    // actionLogin() - Verify user and access into system
    $scope.actionLogin = function () {
        if ($scope.userName == "" || $scope.password == "") {
            toaster.error("Please enter valid credentials.");
            return;
        }

        var myParams = {
            userName: $scope.userName,
            password: $scope.password
        }
        $transporterQuote.callAPI5(
            '/api/login/login',
            myParams,
            function (apiResponse) {

                if (apiResponse.data.isSuperAdmin) {
                    $state.go('adminPanel');
                }
                else {
                    $state.go('reqForQuote');
                }
                var personInfo = apiResponse.data;
                //console.log("PERSONINFO: " + personInfo.toString());
                window.localStorage.setItem('pm_info', JSON.stringify(personInfo));

                $transporterQuote.getLoginInformation();
                $scope.pm_personInfo = $transporterQuote.pm_personInfo;
                $scope.pm_personInfo.userID = $transporterQuote.pm_personInfo.userID;


            },
            function (apiError) {
                toaster.error("Please enter valid credentials.");
                console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(errorStr + "1");
            }
        );
    }

    // forgotPassword() - Reset password 
    $scope.forgotPassword = function () {
        if ($scope.userName == null || $scope.userName == "") {
            toaster.info("Please enter email address.");
            return;
        }

        var myParams = {
            email: $scope.userName
        }

        $transporterQuote.callAPI5('/api/login/password/forgot',
            myParams,
            function (apiResponse) {
                // console.log(apiResponse.data);
                toaster.success(apiResponse.message);
            }, function (apiError) {
                toaster.error(apiError.message);
                console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(errorStr + "2");
            })
    }

    // #endregion

})
// #endregion

// #region "adminPanel"
.controller("adminPanelController", function ($scope, $state, $transporterQuote, toaster, SweetAlert, $uibModal) {

    // #region "Variables"

    Date.prototype.customFormat = function (formatString) {
        var YYYY, YY, MMMM, MMM, MM, M, DDDD, DDD, DD, D, hhhh, hhh, hh, h, mm, m, ss, s, ampm, AMPM, dMod, th;
        YY = ((YYYY = this.getFullYear()) + "").slice(-2);
        MM = (M = this.getMonth() + 1) < 10 ? ('0' + M) : M;
        MMM = (MMMM = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][M - 1]).substring(0, 3);
        DD = (D = this.getDate()) < 10 ? ('0' + D) : D;
        DDD = (DDDD = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"][this.getDay()]).substring(0, 3);
        th = (D >= 10 && D <= 20) ? 'th' : ((dMod = D % 10) == 1) ? 'st' : (dMod == 2) ? 'nd' : (dMod == 3) ? 'rd' : 'th';
        formatString = formatString.replace("#YYYY#", YYYY).replace("#YY#", YY).replace("#MMMM#", MMMM).replace("#MMM#", MMM).replace("#MM#", MM).replace("#M#", M).replace("#DDDD#", DDDD).replace("#DDD#", DDD).replace("#DD#", DD).replace("#D#", D).replace("#th#", th);
        h = (hhh = this.getHours());
        if (h == 0) h = 24;
        if (h > 12) h -= 12;
        hh = h < 10 ? ('0' + h) : h;
        hhhh = h < 10 ? ('0' + hhh) : hhh;
        AMPM = (ampm = hhh < 12 ? 'am' : 'pm').toUpperCase();
        mm = (m = this.getMinutes()) < 10 ? ('0' + m) : m;
        ss = (s = this.getSeconds()) < 10 ? ('0' + s) : s;
        return formatString.replace("#hhhh#", hhhh).replace("#hhh#", hhh).replace("#hh#", hh).replace("#h#", h).replace("#mm#", mm).replace("#m#", m).replace("#ss#", ss).replace("#s#", s).replace("#ampm#", ampm).replace("#AMPM#", AMPM);
    };

    var infoCookie = window.localStorage.getItem('pm_info');
    if (infoCookie == null) {
        $transporterQuote.bye();
    }
    var currentDT = new Date();
    $scope.todayDT = currentDT.customFormat("#DD#-#MMM#-#YY#");
    $scope.showSearch = false;
    $scope.selectedTransporterIndex = -1;
    $scope.transporterList = [];
    $scope.showTransporters = true;
    $scope.showSources = false;
    $scope.executives = false;
    $scope.transporter = {
        companyName: '',
        title: 'Mr.',
        firstName: '',
        lastName: '',
        state: '',
        city: '',
        email: '',
        mobile: '',
        alternateMobile: '',
        notes: '',
        isDomestic: false,
        isExport: false,
        serviceTypeID: ''
    }
    $scope.states = [];
    $scope.newTranporter = true;
    $scope.titles = [{
        id: 1,
        name: 'Mr.'
    }, {
        id: 2,
        name: 'Ms.'
    }];
    $scope.sourceList = [];
    $scope.source = {
        location: '',
        type: '',
        address1: '',
        address2: '',
        city: '',
        state: '',
        notes: ''
    }
    $scope.types = [{
        id: 1,
        name: 'Factory'
    },
    {
        id: 2,
        name: 'Port'
    },
    {
        id: 3,
        name: 'Airport'
    }]
    $scope.selectedSourceIndex = -1;
    $scope.newSource = false;
    $scope.executiveList = [];
    $scope.executive = {
        executiveName: '',
        email: '',
        mobile: '',
        canSelectTransporter: false,
        canCreateRFQ: false,
        canCloseRFQ: false,
        canStart: false,
        canFinish: false,
        canFeedback: false,
        canSelectForCustomer: false,
        designation: '',
        notes: '',
        zoneIDs: '',
        canForward: false,
        canDispatchCreate: false,
        canUpdateStatus: false,
        canScheduled: false,
        canLoading: false,
        canDispatchView: false,
        isSupervisor: false,
        isDomestic: false,
        isExport: false,
        serviceRights: ''
    }
    $scope.selectedExecutiveIndex = -1;
    $scope.newExecutive = false;
    var sourceID = 0;
    var transporterID = 0;
    var executiveID = 0;
    var customerID = 0;
    //$scope.customerList = [
    //          {
    //              companyName: 'Bharat Pvt. Ltd',
    //              email: 'j@gmail.com',
    //              city: 'Ahmedabad',
    //              state: 'Gujarat',
    //              title: 'Mr.',
    //              fname: 'Jay',
    //              lname: 'Patel',
    //              mobile: '23432523325',
    //              alternateMobile: '',
    //              notes: 'dsf'
    //          },
    //          {
    //              companyName: 'Peach Pvt. Ltd',
    //              email: 'p@gmail.com',
    //              city: 'Ahmedabad',
    //              state: 'Gujarat',
    //              title: 'Mr.',
    //              fname: 'Meet',
    //              lname: 'Patel',
    //              mobile: '23432523325',
    //              alternateMobile: '',
    //              notes: 'dsf'
    //          },
    //          {
    //              companyName: 'Colors Pvt. Ltd',
    //              email: 'c@gmail.com',
    //              city: 'Ahmedabad',
    //              state: 'Gujarat',
    //              title: 'Mr.',
    //              fname: 'Jatin',
    //              lname: 'Patel',
    //              mobile: '23432523325',
    //              alternateMobile: '',
    //              notes: 'dsf'
    //          }
    //]
    $scope.customer = {
        companyName: '',
        title: 'Mr.',
        fname: '',
        lname: '',
        state: '',
        city: '',
        email: '',
        mobile: '',
        alternateMobile: '',
        notes: '',
        address1: '',
        address2: '',
        country: ''
    }
    $scope.selectedCustomerIndex = -1;
    $scope.newCustomer = false;
    $scope.showCustomers = false;
    var setRoute = null;
    $scope.route = {
        code: '',
        detail: ''
    }
    $scope.showRoute = false;
    var routeID = 0;
    $scope.routeList = [];
    $scope.transporterDeleted = false;
    $scope.sourceDeleted = false;
    $scope.routeDeleted = false;
    $scope.executiveDeleted = false;
    $scope.customerDeleted = false;
    $scope.zones = [];
    $scope.quoteType = {
        quoteTypeID: 0,
        quoteTypeName: '',
        paidBy: '',
        isDeleted: false,
        isAllowNegotiation: false,
        components: [],
        customer: false,
        quoteby: true,
        term: false,
        file: false,
        vendors: false,
        note: true,
        erpRef: false,
        customFields: [],
        serviceTypeID: '',
        serviceTypeName: '',
        coreField: '',
        deliveryDT: true,
        completionText: 'Delivery Date',
        pickUpDt: false
    }
    $scope.component = {
        ComponentID: 0,
        ComponentName: '',
        Type: '',
        PaidBy: '',
        Currency: '',
        Edit: false,
        IsDeleted: false,
        IsNew: false
    }
    $scope.customField = {
        fieldID: 0,
        fieldName: '',
        typeID: '',
        typeName: '',
        choice: '',
        isEdit: false,
        isDelete: false,
        isNew: false,
        showCustomers: true,
        showVendors: true
    }
    $scope.showQuoteType = false;
    $scope.quoteTypeList = [];
    $scope.copyFrom = {
        value: ''
    };
    $scope.selectedQuoteTypeIndex = -1;
    $scope.serviceList = [];
    $scope.isShowCoreField = false;
    $scope.isShowCustom = false;
    $scope.isShowComponent = false;
    $scope.copyTransportList = [];
    $scope.countryList = [];
    $scope.isIndia = false;
    $scope.quoteTypesforCopy = [];
    var unit = new Date().getHours() >= 12 ? " PM" : " AM";
    var minutes = new Date().getMinutes() < 10 ? "0" + new Date().getMinutes() : "" + new Date().getMinutes();
    var date = new Date().getHours() == 12 ? new Date().getHours() + ":" + minutes + unit : new Date().getHours() % 12 + ":" + minutes + unit;
    console.log(currentDT.getHours());
    var unitTo = new Date(new Date().setHours(new Date().getHours() + 2)).getHours() >= 12 ? " PM" : " AM"
    var dateTo = new Date().getHours() == 12 ? new Date().getHours() + ":" + minutes + unit : new Date(new Date().setHours(new Date().getHours() + 2)).getHours() % 12 + ":" + minutes + unitTo;
    $scope.slot = {
        slotID: 0,
        fromTime: date,
        toTime: dateTo,
        capacity: 0,
        serviceTypeID: 1,
        serviceName: '',
        isDeleted: false
    }
    $scope.showSlot = false;
    $scope.slotList = [];
    $scope.slotDeleted = false;
    $scope.password = {
        oldPassword: '',
        newPassword: '',
        confirmPassword: ''
    }
    // #endregion

    // #region "Functions"

    // searchProduct() - Search product
    $scope.searchInList = function () {
        $scope.showSearch = !$scope.showSearch;
        setTimeout(function () { document.getElementById("search").focus() }, 1000);
        $scope.search = {
            value: ''
        }
    }

    // getTransDetail() - Show detail of trasporter
    $scope.getTransDetail = function (index, transporterID1) {
        for (i in $scope.transporterList) {
            if ($scope.transporterList[i].transporterID == transporterID1)
                index = i;
        }
      
        $scope.newTranporter = false;
        $scope.selectedTransporterIndex = index;
        transporterID = $scope.transporterList[index].transporterID;
        $scope.transporter.companyName = $scope.transporterList[index].companyName;
        $scope.transporter.title = $scope.transporterList[index].title;
        $scope.transporter.firstName = $scope.transporterList[index].firstName;
        $scope.transporter.lastName = $scope.transporterList[index].lastName;
        $scope.transporter.state = $scope.transporterList[index].state;
        $scope.transporter.city = $scope.transporterList[index].city;
        $scope.transporter.email = $scope.transporterList[index].email;
        $scope.transporter.mobile = $scope.transporterList[index].mobile;
        $scope.transporter.alternateMobile = $scope.transporterList[index].alternateMobile;
        $scope.transporter.notes = $scope.transporterList[index].notes;
        $scope.transporter.serviceTypeID = $scope.transporterList[index].serviceTypeID + '';

        angular.forEach($scope.vendorsServices, function (chkItem, chkKey) {
            chkItem.isChecked = false;
        })

        if ($scope.transporter.serviceTypeID != "") {
            subStringService = ($scope.transporter.serviceTypeID).substring(1, ($scope.transporter.serviceTypeID).length - 1);
            var serviceIDs = subStringService.split('~');
            angular.forEach($scope.vendorsServices, function (serviceItem, serviceKey) {
                angular.forEach(serviceIDs, function (idsItem, idsKey) {
                    if (serviceItem.serviceTypeID == idsItem) {
                        serviceItem.isChecked = true;
                    }
                })
            })
        }
        // $scope.transporter.isDomestic = $scope.transporterList[index].isDomestic;
        // $scope.transporter.isExport = $scope.transporterList[index].isExport;
    }

    // getState() - Get all state
    var getState = function () {
        var myParams = {};
        $transporterQuote.callAPI5(
            '/api/gen/states/get',
            myParams,
           function (apiResponse) {
               $scope.states = apiResponse.data;
           }, function (apiError) {
               toaster.error(apiError.message);
           }, function (apiFailure) {
               toaster.warning(errorStr + "1");
           })
    }

    // Calling function
    getState();

    // getZones() - Get zones
    var getZones = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        };
        $transporterQuote.callAPI5(
            '/api/admin/zone/get',
            myParams,
           function (apiResponse) {
               $scope.zones = apiResponse.data;
               angular.forEach($scope.zones, function (item, key) {
                   item.isChecked = false;
               })
           }, function (apiError) {
               toaster.error(apiError.message);
           }, function (apiFailure) {
               toaster.warning(errorStr + "14");
           })
    }

    // Calling function
    getZones();

    // alphaNumSort() - Sort array alphanumerically.
    function alphaNumSort(name) {
        var reA = /[^a-zA-Z]/g; // Remove non-alphabets
        var reN = /[^0-9]/g;    // Remove non-numbers
        var reNStart = /[^0-9]/;
        return function (a, b) {

            var aStr = a[name].toLowerCase();
            var bStr = b[name].toLowerCase();

            //var aANumStart = aStr.match(reNStart).length > 0;
            //console.log(aANumStart);

            var aAName = aStr.replace(reA, "");
            var aBName = bStr.replace(reA, "");
            if (aAName === aBName) {
                var aANum = parseInt(a[name].replace(reN, ""), 10); // 10 is for decimal
                var aBNum = parseInt(b[name].replace(reN, ""), 10);
                return aANum === aBNum ? 0 : aANum > aBNum ? 1 : -1;
            }
            else {
                return aAName > aBName ? 1 : -1;
            }
        }
    }

    // dlNaturalSort() - Sorting
    function dlNaturalSort(myList, columnName) {

        var arrayStartNum = [];
        var arrayNotStartNum = [];

        // First parse the list and classify between ones that start with number and ones that don't.
        for (var i = 0; i < myList.length; i++) {

            if (myList[i][columnName].match(/^[0-9].*$/) == null) {
                arrayNotStartNum.push(myList[i]);
            } else {
                arrayStartNum.push(myList[i]);
            }
        }

        // Sort both arrays independently.
        arrayStartNum.sort(alphaNumSort(columnName));
        arrayNotStartNum.sort(alphaNumSort(columnName));

        // Concat the arrays and return.
        return arrayStartNum.concat(arrayNotStartNum);
    }

    // getExecutives() - Get executives list
    var getExecutives = function () {

        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        // Call api
        $transporterQuote.callAPI5('/api/admin/executives/get',
           myParams,
           function (apiResponse) {
               //console.log(apiResponse.data);
               $scope.executiveList = apiResponse.data;

           }, function (apiError) {
               console.log(apiError.message);
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "2");
           })
    }

    // Calling function
    getExecutives();

    // getTransporter() - Get transporter list
    var getTrasporter = function () {

        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        // Call api
        $transporterQuote.callAPI5('/api/admin/transporters/get',
           myParams,
           function (apiResponse) {
               //console.log(apiResponse.data);
               $scope.transporterList = apiResponse.data;
               console.log($scope.transporterList);
               angular.copy($scope.transporterList, $scope.copyTransportList);
               //$scope.getTransDetail(0);
           }, function (apiError) {
               //console.log(apiError.message);
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "3");
           })
    }

    // Calling function
    getTrasporter();

    // getSources() - Get source list
    var getSources = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        // Call api
        $transporterQuote.callAPI5('/api/admin/sources/get',
           myParams,
           function (apiResponse) {
               //console.log(apiResponse.data);
               $scope.sourceList = apiResponse.data;
           }, function (apiError) {
               console.log(apiError.message);
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "4");
           })
    }

    // Calling function
    getSources();

    // getCustomers() - Get customers
    var getCustomers = function () {

        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        // Call api
        $transporterQuote.callAPI5('/api/admin/customers/get',
           myParams,
           function (apiResponse) {
               //console.log(apiResponse.data);
               $scope.customerList = apiResponse.data;
           }, function (apiError) {
               console.log(apiError.message);
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "5");
           })
    }

    // Calling function
    getCustomers();

    // getRoute() - Get route
    var getRoute = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        // Call api
        $transporterQuote.callAPI5('/api/admin/routes/get',
           myParams,
           function (apiResponse) {
               //console.log(apiResponse.data);
               $scope.routeList = apiResponse.data;
               // console.log($scope.routeList);
           }, function (apiError) {
               console.log(apiError.message);
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "6");
           })
    }

    // getRoute()
    getRoute();

    // openTransporter() - Open transporter
    $scope.openTransporter = function () {
        $scope.showTransporters = true;
        $scope.showSources = false;
        $scope.showExecutives = false;
        $scope.showCustomers = false;
        $scope.newTranporter = true;
        $scope.showRoute = false;
        $scope.showQuoteType = false;
        $scope.showSlot = false;
        $scope.transporter = {
            companyName: '',
            title: 'Mr.',
            firstName: '',
            lastName: '',
            state: '',
            city: '',
            email: '',
            mobile: '',
            alternateMobile: '',
            notes: '',
            isDomestic: false,
            isExport: false,
            serviceTypeID: ''
        }
        $scope.selectedTransporterIndex = -1;
        transporterID = 0;
        document.getElementById('menuDD').style.display = 'none';
        angular.forEach($scope.vendorsServices, function (chkItem, chkKey) {
            chkItem.isChecked = false;
        })
    }

    // openSources() - Open sources
    $scope.openSources = function () {
        $scope.newSource = true;
        $scope.showTransporters = false;
        $scope.showSources = true;
        $scope.showExecutives = false;
        $scope.showCustomers = false;
        $scope.showRoute = false;
        $scope.showQuoteType = false;
        $scope.source = {
            location: '',
            type: '',
            address1: '',
            address2: '',
            city: '',
            state: '',
            notes: ''
        }
        $scope.showSlot = false;
        $scope.selectedSourceIndex = -1;
        sourceID = 0;
        document.getElementById('menuDD').style.display = 'none';
        // $scope.getSourceDetail(0);
    }

    // openExecutives() - Open executives
    $scope.openExecutives = function () {
        $scope.newExecutive = true;
        $scope.showTransporters = false;
        $scope.showSources = false;
        $scope.showExecutives = true;
        $scope.showCustomers = false;
        $scope.showRoute = false;
        $scope.showQuoteType = false;
        $scope.executive = {
            executiveName: '',
            email: '',
            mobile: '',
            canSelectTransporter: false,
            canCreateRFQ: false,
            canCloseRFQ: false,
            canStart: false,
            canFinish: false,
            canFeedback: false,
            canSelectForCustomer: false,
            designation: '',
            notes: '',
            zoneIDs: '',
            canForward: false,
            canDispatchCreate: false,
            canUpdateStatus: false,
            canScheduled: false,
            canLoading: false,
            canDispatchView: false,
            isSupervisor: false,
            isDomestic: false,
            isExport: false,
            serviceRights: ''
        }
        $scope.showSlot = false;
        $scope.selectedExecutiveIndex = -1;
        executiveID = 0;
        document.getElementById('menuDD').style.display = 'none';
        // $scope.getExecutiveDetail(0);
    }

    // openCustomer() - Open customer
    $scope.openCustomer = function () {
        $scope.newCustomer = true;
        $scope.showTransporters = false;
        $scope.showSources = false;
        $scope.showExecutives = false;
        $scope.showRoute = false;
        $scope.showCustomers = true;
        $scope.showQuoteType = false;
        $scope.customer = {
            companyName: '',
            title: 'Mr.',
            fname: '',
            lname: '',
            state: '',
            city: '',
            email: '',
            mobile: '',
            alternateMobile: '',
            notes: '',
            address1: '',
            address2: '',
            country: ''
        }
        $scope.showSlot = false;
        $scope.selectedCustomerIndex = -1;
        customerID = 0;
        document.getElementById('menuDD').style.display = 'none';
        getCustomers();
        //  $scope.getCustomerDetail(0);
    }

    // openRoute() - Open route
    $scope.openRoute = function () {
        $scope.newRoute = true;
        $scope.showTransporters = false;
        $scope.showSources = false;
        $scope.showExecutives = false;
        $scope.showCustomers = false;
        $scope.showRoute = true;
        $scope.route = {
            code: '',
            detail: ''
        }
        $scope.selectedRouteIndex = -1;
        document.getElementById('menuDD').style.display = 'none';
    }

    // addNew() - Add new transporter, source and executive
    $scope.addNew = function (checkAdd) {

        if ($scope.showTransporters == checkAdd) {
            transporterID = 0;
            $scope.newTranporter = true;
            $scope.selectedTransporterIndex = -1;
            $scope.transporter = {
                companyName: '',
                title: 'Mr.',
                firstName: '',
                lastName: '',
                state: '',
                city: '',
                email: '',
                mobile: '',
                alternateMobile: '',
                notes: '',
                isDomestic: false,
                isExport: false,
                serviceTypeID: ''
            }
            angular.forEach($scope.vendorsServices, function (chkItem, chkKey) {
                chkItem.isChecked = false;
            })
        } else if ($scope.showSources == checkAdd) {
            sourceID = 0;
            $scope.newSource = true;
            $scope.selectedSourceIndex = -1;
            $scope.source = {
                location: '',
                type: '',
                address1: '',
                address2: '',
                city: '',
                state: '',
                notes: ''
            }
        } else if ($scope.showExecutives == checkAdd) {
            executiveID = 0;
            $scope.newExecutive = true;
            $scope.selectedExecutiveIndex = -1;
            $scope.executive = {
                executiveName: '',
                email: '',
                mobile: '',
                canSelectTransporter: false,
                canCreateRFQ: false,
                canCloseRFQ: false,
                canStart: false,
                canFinish: false,
                canFeedback: false,
                canSelectForCustomer: false,
                designation: '',
                notes: '',
                zoneIDs: '',
                canForward: false,
                canDispatchCreate: false,
                canUpdateStatus: false,
                canScheduled: false,
                canLoading: false,
                canDispatchView: false,
                isSupervisor: false,
                isDomestic: false,
                isExport: false,
                serviceRights: ''
            }
            angular.forEach($scope.zones, function (item, key) {
                item.isChecked = false;
            })

        } else if ($scope.showRoute == checkAdd) {
            routeID = 0;
            $scope.newRoute = true;
            $scope.selectedRouteIndex = -1;
            $scope.route = {
                code: '',
                detail: ''
            }
        } else if ($scope.showQuoteType == checkAdd) {
            $scope.selectedQuoteTypeIndex = -1;
            $scope.newQuoteType = true;
            $scope.quoteType = {
                quoteTypeID: 0,
                quoteTypeName: '',
                paidBy: false,
                type: false,
                isDeleted: false,
                isAllowNegotiation: false,
                components: [],
                customer: false,
                quoteby: true,
                term: false,
                file: false,
                vendors: false,
                note: true,
                erpRef: false,
                customFields: [],
                serviceTypeID: '',
                serviceTypeName: '',
                deliveryDT: true,
                completionText: 'Delivery Date'
            }
            $scope.copyFrom = {
                value: ''
            };
        } else if ($scope.showSlot = checkAdd) {
            $scope.newSlot = true;
            $scope.slot = {
                slotID: 0,
                fromTime: date,
                toTime: dateTo,
                capacity: 0,
                serviceTypeID: 1,
                serviceName: ''
            }
            // $scope.showSlot = true;
            $scope.selectedSlotIndex = -1;
        }
        else {
            customerID = 0;
            $scope.customer = {
                companyName: '',
                title: 'Mr.',
                fname: '',
                lname: '',
                state: '',
                city: '',
                email: '',
                mobile: '',
                alternateMobile: '',
                notes: '',
                address1: '',
                address2: '',
                country: ''
            }
            $scope.selectedCustomerIndex = -1;
            $scope.newCustomer = true;

        }
    }

    // deleteTransporter() - Delete transporter
    $scope.deleteTransporter = function () {
        confirmSWALPopup(SweetAlert, "Are you sure you want to delete this transporter?", function () {
            $scope.transporterDeleted = true;
            $scope.addUpdateTransPorter();
        },
        function () {
            return;
        })
    }

    // addUpdateTransPorter() - Add or update trasporter info using api calling
    $scope.addUpdateTransPorter = function () {

        var emailRegex = new RegExp("[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}$");

        if ($scope.transporter.companyName == '' || typeof ($scope.transporter.companyName) == undefined) {
            toaster.info("Please enter a company name.");
            return;
        } else if ($scope.transporter.title == '' || $scope.transporter.firstName == '' || $scope.transporter.lastName == '') {
            toaster.info("Please enter a contact person.");
            return;
        } else if ($scope.transporter.city == "") {
            toaster.info('Please enter city.');
            return;
        } else if ($scope.transporter.state == '') {
            toaster.info('Please select a state.');
            return;
        } else if ($scope.transporter.email == '') {
            toaster.info('Please enter a email address.');
            return;
        } else if (!emailRegex.test($scope.transporter.email)) {
            toaster.info("Please enter a valid email address!");
            return;
        } else if ($scope.transporter.mobile == "") {
            toaster.info('Please enter mobile number.');
            return;
        } else if ($scope.transporter.mobile != "" && $scope.transporter.mobile.length != 10) {
            toaster.info('Please enter a valid mobile number.');
            return;
        } else if ($scope.transporter.alternateMobile != "" && $scope.transporter.alternateMobile.length != 10) {
            toaster.info('Please enter a valid  alternate mobile number.');
            return;
        }

        var VendorSelectedSer = '~';
        angular.forEach($scope.vendorsServices, function (serviceItem, serviceKey) {
            if (serviceItem.isChecked == true) {
                VendorSelectedSer += serviceItem.serviceTypeID + '~'
            }
        })

        if (VendorSelectedSer == '~') {
            toaster.info('Please select a service.');
            return;
        }

        $scope.transporter.serviceTypeID = VendorSelectedSer;

        angular.forEach($scope.vendorsServices, function (item, key) {

        });

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            transporterID: transporterID,
            companyName: $scope.transporter.companyName,
            email: $scope.transporter.email,
            city: $scope.transporter.city,
            state: $scope.transporter.state,
            title: $scope.transporter.title,
            fname: $scope.transporter.firstName,
            lname: $scope.transporter.lastName,
            mobile: $scope.transporter.mobile,
            alternateMobile: $scope.transporter.alternateMobile,
            notes: $scope.transporter.notes,
            isDeleted: $scope.transporterDeleted,
            isDomestic: $scope.transporter.isDomestic,
            isExport: $scope.transporter.isExport,
            serviceTypeID: $scope.transporter.serviceTypeID
        }

        $transporterQuote.callAPI5('/api/admin/transporter/set',
            myParams,
            function (apiResponse) {
                toaster.success(apiResponse.message);
                // console.log(apiResponse.data);
                if (apiResponse.data != 0) {
                    $scope.transporterList.push({
                        transporterID: apiResponse.data,
                        companyName: $scope.transporter.companyName,
                        email: $scope.transporter.email,
                        city: $scope.transporter.city,
                        state: $scope.transporter.state,
                        title: $scope.transporter.title,
                        firstName: $scope.transporter.firstName,
                        lastName: $scope.transporter.lastName,
                        mobile: $scope.transporter.mobile,
                        alternateMobile: $scope.transporter.alternateMobile,
                        notes: $scope.transporter.notes,
                        isDomestic: $scope.transporter.isDomestic,
                        isExport: $scope.transporter.isExport,
                        serviceTypeID: $scope.transporter.serviceTypeID
                    })
                    $scope.transporterList = dlNaturalSort($scope.transporterList, 'companyName');
                    angular.forEach($scope.vendorsServices, function (chkItem, chkKey) {
                        chkItem.isChecked = false;
                    })
                    $scope.transporter = {
                        companyName: '',
                        title: 'Mr.',
                        firstName: '',
                        lastName: '',
                        state: '',
                        city: '',
                        email: '',
                        mobile: '',
                        alternateMobile: '',
                        notes: '',
                        isDomestic: false,
                        isExport: false,
                        serviceTypeIDs: ''
                    }
                } else {
                    if (!$scope.transporterDeleted) {
                        $scope.transporterList[$scope.selectedTransporterIndex].companyName = $scope.transporter.companyName;
                        $scope.transporterList[$scope.selectedTransporterIndex].title = $scope.transporter.title;
                        $scope.transporterList[$scope.selectedTransporterIndex].firstName = $scope.transporter.firstName;
                        $scope.transporterList[$scope.selectedTransporterIndex].lastName = $scope.transporter.lastName;
                        $scope.transporterList[$scope.selectedTransporterIndex].state = $scope.transporter.state;
                        $scope.transporterList[$scope.selectedTransporterIndex].city = $scope.transporter.city;
                        $scope.transporterList[$scope.selectedTransporterIndex].email = $scope.transporter.email;
                        $scope.transporterList[$scope.selectedTransporterIndex].mobile = $scope.transporter.mobile;
                        $scope.transporterList[$scope.selectedTransporterIndex].alternateMobile = $scope.transporter.alternateMobile;
                        $scope.transporterList[$scope.selectedTransporterIndex].notes = $scope.transporter.notes;
                        $scope.transporterList[$scope.selectedTransporterIndex].isDomestic = $scope.transporter.isDomestic;
                        $scope.transporterList[$scope.selectedTransporterIndex].isExport = $scope.transporter.isExport;
                        $scope.transporterList[$scope.selectedTransporterIndex].serviceTypeID = $scope.transporter.serviceTypeID;
                    } else {
                        $scope.transporterList.splice($scope.selectedTransporterIndex, 1);
                        $scope.transporterDeleted = false;
                        $scope.addNew(true);
                    }
                }

            }, function (apiError) {
                toaster.error(apiError.message);
                console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(errorStr + "7");
            })
    }

    // getSourceDetail() - Get source detail
    $scope.getSourceDetail = function (index) {
        $scope.newSource = false;
        $scope.selectedSourceIndex = index;
        sourceID = $scope.sourceList[index].sourceID;
        $scope.source.location = $scope.sourceList[index].location;
        $scope.source.type = $scope.sourceList[index].type;
        $scope.source.address1 = $scope.sourceList[index].address1;
        $scope.source.address2 = $scope.sourceList[index].address2;
        $scope.source.city = $scope.sourceList[index].city;
        $scope.source.state = $scope.sourceList[index].state;
        $scope.source.notes = $scope.sourceList[index].notes;
    }

    // deleteSource() - Delete source
    $scope.deleteSource = function () {
        confirmSWALPopup(SweetAlert, "Are you sure you want to delete this source?", function () {
            $scope.sourceDeleted = true;
            $scope.addUpdateSource();
        },
        function () {
            return;
        })

    }

    // addUpdateSource() - Add or update source
    $scope.addUpdateSource = function () {

        // Validation check
        if ($scope.source.location == "") {
            toaster.info("Please enter a location.");
            return;
        }
        if ($scope.source.type == '') {
            toaster.info("Please select a type.");
            return;
        }
        if ($scope.source.address1 == "") {
            toaster.info("Please enter a address.");
            return;
        }
        if ($scope.source.city == "") {
            toaster.info("Please enter a city.");
            return;
        }
        if ($scope.source.state == '') {
            toaster.info("Please select a state.");
            return;
        }

        // Parameters
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            sourceID: sourceID,
            location: $scope.source.location,
            type: $scope.source.type,
            address1: $scope.source.address1,
            address2: $scope.source.address2,
            city: $scope.source.city,
            state: $scope.source.state,
            notes: $scope.source.notes,
            isDeleted: $scope.sourceDeleted
        }

        // Call api
        $transporterQuote.callAPI5('/api/admin/source/set',
           myParams,
           function (apiResponse) {
               // console.log(apiResponse.data);
               toaster.success(apiResponse.message);
               if (apiResponse.data != 0) {
                   $scope.sourceList.push({
                       sourceID: apiResponse.data,
                       location: $scope.source.location,
                       type: $scope.source.type,
                       address1: $scope.source.address1,
                       address2: $scope.source.address2,
                       city: $scope.source.city,
                       state: $scope.source.state,
                       notes: $scope.source.notes
                   })
                   $scope.sourceList = dlNaturalSort($scope.sourceList, 'location');
                   $scope.source = {
                       location: '',
                       type: '',
                       address1: '',
                       address2: '',
                       city: '',
                       state: '',
                       notes: ''
                   }
               } else {
                   if (!$scope.sourceDeleted) {
                       $scope.sourceList[$scope.selectedSourceIndex].location = $scope.source.location
                       $scope.sourceList[$scope.selectedSourceIndex].type = $scope.source.type;
                       $scope.sourceList[$scope.selectedSourceIndex].address1 = $scope.source.address1;
                       $scope.sourceList[$scope.selectedSourceIndex].address2 = $scope.source.address2;
                       $scope.sourceList[$scope.selectedSourceIndex].city = $scope.source.city;
                       $scope.sourceList[$scope.selectedSourceIndex].state = $scope.source.state;
                       $scope.sourceList[$scope.selectedSourceIndex].notes = $scope.source.notes;
                   } else {
                       $scope.sourceList.splice($scope.selectedSourceIndex, 1);
                       $scope.addNew(true);
                       $scope.sourceDeleted = false;
                   }
               }

           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "8");
           })

    }

    // getExecutiveDetail() - Get executive detail
    $scope.getExecutiveDetail = function (index) {
        var subStringZoneID = ''
        $scope.newExecutive = false;
        $scope.selectedExecutiveIndex = index;
        executiveID = $scope.executiveList[index].executiveID;
        $scope.executive.executiveName = $scope.executiveList[index].executiveName;
        $scope.executive.email = $scope.executiveList[index].email;
        $scope.executive.mobile = $scope.executiveList[index].mobile;
        $scope.executive.canSelectTransporter = $scope.executiveList[index].canSelectTransporter;
        $scope.executive.canCreateRFQ = $scope.executiveList[index].canCreateRFQ;
        $scope.executive.canCloseRFQ = $scope.executiveList[index].canCloseRFQ;
        $scope.executive.canStart = $scope.executiveList[index].canStart;
        $scope.executive.canFinish = $scope.executiveList[index].canFinish;
        $scope.executive.canFeedback = $scope.executiveList[index].canFeedback;
        $scope.executive.canSelectForCustomer = $scope.executiveList[index].canSelectForCustomer;
        $scope.executive.designation = $scope.executiveList[index].designation;
        $scope.executive.notes = $scope.executiveList[index].notes;
        $scope.executive.zoneIDs = $scope.executiveList[index].zoneIDs;
        $scope.executive.canForward = $scope.executiveList[index].canForward;
        $scope.executive.canDispatchCreate = $scope.executiveList[index].canDispatchCreate;
        $scope.executive.canUpdateStatus = $scope.executiveList[index].canUpdateStatus;
        $scope.executive.canScheduled = $scope.executiveList[index].canScheduled;
        $scope.executive.canLoading = $scope.executiveList[index].canLoading;
        $scope.executive.canDispatchView = $scope.executiveList[index].canDispatchView;
        $scope.executive.isSupervisor = $scope.executiveList[index].isSupervisor;
        $scope.executive.isDomestic = $scope.executiveList[index].isDomestic;
        $scope.executive.isExport = $scope.executiveList[index].isExport;
        $scope.executive.serviceRights = $scope.executiveList[index].serviceRights;

        // if ($scope.executive.zoneIDs == "") {
        angular.forEach($scope.zones, function (item, key) {
            item.isChecked = false;
        })
        //  }

        if ($scope.executive.zoneIDs == '~') {
            angular.forEach($scope.zones, function (subitem, subkey) {
                subitem.isChecked = true;
            })
        } else {
            if ($scope.executive.zoneIDs != "") {
                subStringZoneID = ($scope.executive.zoneIDs).substring(1, ($scope.executive.zoneIDs).length - 1);
                var executiveZoneIDs = subStringZoneID.split('~');
                angular.forEach($scope.zones, function (subitem, subkey) {
                    angular.forEach(executiveZoneIDs, function (item, key) {
                        if (subitem.zoneID == item) {
                            subitem.isChecked = true;
                        }
                    })
                })
            }
        }

        angular.forEach($scope.serviceList, function (chkItem, chkKey) {
            chkItem.isChecked = false;
        })

        if ($scope.executive.serviceRights != "") {
            subStringService = ($scope.executive.serviceRights).substring(1, ($scope.executive.serviceRights).length - 1);
            var serviceIDs = subStringService.split('~');
            angular.forEach($scope.serviceList, function (serviceItem, serviceKey) {
                angular.forEach(serviceIDs, function (idsItem, idsKey) {
                    if (serviceItem.serviceTypeID == idsItem) {
                        serviceItem.isChecked = true;
                    }
                })
            })
        }

    }

    // deleteExecutive() - Delete executive
    $scope.deleteExecutive = function () {
        confirmSWALPopup(SweetAlert, "Are you sure you want to delete this executive?", function () {
            $scope.executiveDeleted = true;
            $scope.addUpdateExecutive();
        },
         function () {
             return;
         })

    }

    // addUpdateExecutive() - Add executive 
    $scope.addUpdateExecutive = function () {

        // Validation check
        var emailRegex = new RegExp("[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}$");
        var zoneIDs = ''
        var cnt = 0;
        var rights = ""
        if ($scope.executive.executiveName == '') {
            toaster.info("Please enter an executive name.");
            return;
        } else if ($scope.executive.email == '') {
            toaster.info('Please enter an email address.');
            return;
        } else if (!emailRegex.test($scope.executive.email)) {
            toaster.info("Please enter a valid email address!");
            return;
        } else if ($scope.executive.mobile == "") {
            toaster.info('Please enter mobile number.');
            return;
        } else if ($scope.executive.mobile != "" && $scope.executive.mobile.length != 10) {
            toaster.info('Please enter a valid mobile number.');
            return;
        } else if ($scope.executive.designation == "") {
            toaster.info('Please enter a designation.');
            return;
        }

       
        // Check zone for executive
        //angular.forEach($scope.zones, function (item, key) {
        //    if (item.isChecked == true) {
        //        zoneIDs += zoneIDs == '' ? '~' + item.zoneID + '~' : item.zoneID + '~'
        //        cnt++;
        //    }
        //})

        //if ($scope.zones.length == cnt) {
        //    zoneIDs = '~';
        //}

        //if (zoneIDs == '') {
        //    toaster.info('Please select a zone.');
        //    return;
        //}



        $scope.executive.zoneIDs = zoneIDs;

        if ($scope.executive.canForward == true) {
            if (rights == "") {
                rights += '~'
            }
            rights += '2~'
        }

        if ($scope.executive.canDispatchCreate == true) {
            if (rights == "") {
                rights += '~'
            }
            rights += '4~'
        }

        if ($scope.executive.canUpdateStatus == true) {
            if (rights == "") {
                rights += '~'
            }
            rights += '5~'
        }

        if ($scope.executive.canScheduled == true) {
            if (rights == "") {
                rights += '~'
            }
            rights += '6~'
        }

        if ($scope.executive.canLoading == true) {
            if (rights == "") {
                rights += '~'
            }
            rights += '7~'
        }

        if ($scope.executive.canDispatchView == true) {
            if (rights == "") {
                rights += '~'
            }
            rights += '3~'
        }

        if ($scope.executive.canStart == true) {
            if (rights == "") {
                rights += '~';
            }
            rights += '11~';
        }

        if ($scope.executive.canFinish == true) {
            if (rights == "") {
                rights += '~';
            }
            rights += '12~';
        }

        if ($scope.executive.canFeedback == true) {
            if (rights == "") {
                rights += '~';
            }
            rights += '13~';
        }
        //if ($scope.executive.isSupervisor == true) {
        //    if (rights == "") {
        //        rights += '~'
        //    }
        //    rights += '8~'
        //}

        //if ($scope.executive.isDomestic == true) {
        //    if (rights == "") {
        //        rights += '~'
        //    }
        //    rights += '9~'
        //}

        //if ($scope.executive.isExport == true) {
        //    if (rights == "") {
        //        rights += '~'
        //    }
        //    rights += '10~'
        //}

        var serviceRights = '~';
        angular.forEach($scope.serviceList, function (serviceItem, serviceKey) {
            if (serviceItem.isChecked == true) {
                serviceRights += serviceItem.serviceTypeID + '~'
            }
        })

        if (serviceRights == '~') {
            toaster.info('Please select a service.');
            return;
        }

        $scope.executive.serviceRights = serviceRights;

        // Parameters
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            executiveID: executiveID,
            executiveName: $scope.executive.executiveName,
            email: $scope.executive.email,
            mobile: $scope.executive.mobile,
            canSelectTransporter: $scope.executive.canSelectTransporter,
            canCreateRFQ: $scope.executive.canCreateRFQ,
            canCloseRFQ: $scope.executive.canCloseRFQ,
            canSelectforCustomer: $scope.executive.canSelectForCustomer,
            designation: $scope.executive.designation,
            notes: $scope.executive.notes,
            isDeleted: $scope.executiveDeleted,
            zoneIDs: zoneIDs,
            rights: rights,
            serviceRights: serviceRights
        }        

        // Call api
        $transporterQuote.callAPI5('/api/admin/executive/set',
           myParams,
           function (apiResponse) {
               // console.log(apiResponse.data);
               toaster.success(apiResponse.message);
               if (apiResponse.data != 0) {
                   $scope.executiveList.push({
                       executiveID: apiResponse.data,
                       executiveName: $scope.executive.executiveName,
                       email: $scope.executive.email,
                       mobile: $scope.executive.mobile,
                       canSelectTransporter: $scope.executive.canSelectTransporter,
                       canCreateRFQ: $scope.executive.canCreateRFQ,
                       canCloseRFQ: $scope.executive.canCloseRFQ,
                       canStart: $scope.executive.canStart,
                       canFinish: $scope.executive.canFinish,
                       canFeedback: $scope.executive.canFeedback,
                       canSelectForCustomer: $scope.executive.canSelectForCustomer,
                       designation: $scope.executive.designation,
                       notes: $scope.executive.notes,
                       zoneIDs: zoneIDs,
                       canForward: $scope.executive.canForward,
                       canClear: $scope.executive.canDispatchCreate,
                       canReady: $scope.executive.canUpdateStatus,
                       canScheduled: $scope.executive.canScheduled,
                       canLoading: $scope.executive.canLoading,
                       canDispatch: $scope.executive.canDispatchView,
                       isSupervisor: $scope.executive.isSupervisor,
                       isDomestic: $scope.executive.isDomestic,
                       isExport: $scope.executive.isExport,
                       serviceRights: $scope.executive.serviceRights
                   })
                   $scope.executiveList = dlNaturalSort($scope.executiveList, 'executiveName');
                   $scope.executive = {
                       executiveName: '',
                       email: '',
                       mobile: '',
                       canSelectTransporter: false,
                       canCreateRFQ: false,
                       canCloseRFQ: false,
                       canStart: false,
                       canFinish: false,
                       canFeedback: false,
                       canSelectForCustomer: false,
                       designation: '',
                       notes: '',
                       zoneIDs: '',
                       canForward: false,
                       canDispatchCreate: false,
                       canUpdateStatus: false,
                       canScheduled: false,
                       canLoading: false,
                       canDispatchView: false,
                       isSupervisor: false,
                       isDomestic: false,
                       isExport: false,
                       serviceRights: ''
                   }
                   angular.forEach($scope.zones, function (item, key) {
                       item.isChecked = false;
                   })
               } else {
                   if (!$scope.executiveDeleted) {
                       $scope.executiveList[$scope.selectedExecutiveIndex].executiveName = $scope.executive.executiveName;
                       $scope.executiveList[$scope.selectedExecutiveIndex].email = $scope.executive.email;
                       $scope.executiveList[$scope.selectedExecutiveIndex].mobile = $scope.executive.mobile;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canSelectTransporter = $scope.executive.canSelectTransporter;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canCreateRFQ = $scope.executive.canCreateRFQ;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canCloseRFQ = $scope.executive.canCloseRFQ;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canStart = $scope.executive.canStart;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canFinish = $scope.executive.canFinish;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canFeedback = $scope.executive.canFeedback;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canSelectForCustomer = $scope.executive.canSelectForCustomer;
                       $scope.executiveList[$scope.selectedExecutiveIndex].designation = $scope.executive.designation;
                       $scope.executiveList[$scope.selectedExecutiveIndex].notes = $scope.executive.notes;
                       $scope.executiveList[$scope.selectedExecutiveIndex].zoneIDs = $scope.executive.zoneIDs;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canForward = $scope.executive.canForward;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canDispatchCreate = $scope.executive.canDispatchCreate;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canUpdateStatus = $scope.executive.canUpdateStatus;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canSchedule = $scope.executive.canSchedule;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canLoading = $scope.executive.canLoading;
                       $scope.executiveList[$scope.selectedExecutiveIndex].canDispatchView = $scope.executive.canDispatchView;
                       $scope.executiveList[$scope.selectedExecutiveIndex].isSupervisor = $scope.executive.isSupervisor;
                       $scope.executiveList[$scope.selectedExecutiveIndex].isDomestic = $scope.executive.isDomestic;
                       $scope.executiveList[$scope.selectedExecutiveIndex].isExport = $scope.executive.isExport;
                       $scope.executiveList[$scope.selectedExecutiveIndex].serviceRights = $scope.executive.serviceRights;
                   } else {
                       $scope.executiveList.splice($scope.selectedExecutiveIndex, 1);
                       // $scope.showExecutives = true;
                       $scope.addNew(true);
                       $scope.executiveDeleted = false;
                   }
               }

           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "9");
           })
    }

    // getTransDetail() - Show detail of trasporter
    $scope.getCustomerDetail = function (index) {
        $scope.newCustomer = false;
        $scope.selectedCustomerIndex = index;
        customerID = $scope.customerList[index].customerID;
        $scope.customer.companyName = $scope.customerList[index].companyName;
        $scope.customer.title = $scope.customerList[index].title;
        $scope.customer.fname = $scope.customerList[index].fname;
        $scope.customer.lname = $scope.customerList[index].lname;
        $scope.customer.state = $scope.customerList[index].state;
        $scope.customer.city = $scope.customerList[index].city;
        $scope.customer.email = $scope.customerList[index].email;
        $scope.customer.mobile = $scope.customerList[index].mobile;
        $scope.customer.alternateMobile = $scope.customerList[index].alternateMobile;
        $scope.customer.notes = $scope.customerList[index].notes;
        $scope.customer.address1 = $scope.customerList[index].address1;
        $scope.customer.address2 = $scope.customerList[index].address2;
        $scope.customer.country = $scope.customerList[index].country;
    }

    // deleteCustomer() - Delete customer
    $scope.deleteCustomer = function () {
        confirmSWALPopup(SweetAlert, "Are you sure you want to delete this customer?", function () {
            $scope.customerDeleted = true;
            $scope.addUpdateCustomer();
        },
          function () {
              return;
          })
    }

    // addUpdateTransPorter() - Add or update trasporter info using api calling
    $scope.addUpdateCustomer = function () {

        var emailRegex = new RegExp("[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}$");

        if ($scope.customer.companyName == '' || typeof ($scope.customer.companyName) == undefined) {
            toaster.info("Please enter a company name.");
            return;
        } else if ($scope.customer.title == '' || $scope.customer.fname == '' || $scope.customer.lname == '') {
            toaster.info("Please enter a contact person.")
            return;
        } else if ($scope.customer.address1 == "") {
            toaster.info("Please enter address.");
            return;
        } else if ($scope.customer.city == "") {
            toaster.info('Please enter city.');
            return;

        } else if ($scope.customer.state == '') {
            toaster.info('Please select a state.');
            return;
        } else if ($scope.customer.email == '') {
            toaster.info('Please enter a email address.');
            return;
        } else if (!emailRegex.test($scope.customer.email)) {
            toaster.info("Please enter a valid email address!");
            return;
        } else if ($scope.customer.mobile == "") {
            toaster.info('Please enter mobile number.');
            return;
        } else if ($scope.customer.mobile != "" && $scope.customer.mobile.length != 10) {
            toaster.info('Please enter a valid mobile number.');
            return;
        } else if ($scope.customer.alternateMobile != "" && $scope.customer.alternateMobile.length != 10) {
            toaster.info('Please enter a valid mobile number.');
            return;
        }

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            customerID: customerID,
            companyName: $scope.customer.companyName,
            email: $scope.customer.email,
            city: $scope.customer.city,
            state: $scope.customer.state,
            title: $scope.customer.title,
            fname: $scope.customer.fname,
            lname: $scope.customer.lname,
            mobile: $scope.customer.mobile,
            alternateMobile: $scope.customer.alternateMobile,
            notes: $scope.customer.notes,
            address1: $scope.customer.address1,
            address2: $scope.customer.address2,
            isDeleted: $scope.customerDeleted,
            country: $scope.customer.country
        }
        $transporterQuote.callAPI5('/api/admin/customer/set',
            myParams,
            function (apiResponse) {
                toaster.success(apiResponse.message);
                //console.log(apiResponse.data);
                if (apiResponse.data != 0) {
                    $scope.customerList.push({
                        customerID: apiResponse.data,
                        companyName: $scope.customer.companyName,
                        email: $scope.customer.email,
                        city: $scope.customer.city,
                        state: $scope.customer.state,
                        title: $scope.customer.title,
                        fname: $scope.customer.fname,
                        lname: $scope.customer.lname,
                        mobile: $scope.customer.mobile,
                        alternateMobile: $scope.customer.alternateMobile,
                        notes: $scope.customer.notes,
                        address1: $scope.customer.address1,
                        address2: $scope.customer.address2,
                        country: $scope.customer.country
                    })
                    $scope.customerList = dlNaturalSort($scope.customerList, 'companyName');
                    $scope.customer = {
                        companyName: '',
                        title: 'Mr.',
                        fname: '',
                        lname: '',
                        state: '',
                        city: '',
                        email: '',
                        mobile: '',
                        alternateMobile: '',
                        notes: '',
                        address1: '',
                        address2: '',
                        country: ''
                    }
                } else {

                    if (!$scope.customerDeleted) {
                        $scope.customerList[$scope.selectedCustomerIndex].companyName = $scope.customer.companyName;
                        $scope.customerList[$scope.selectedCustomerIndex].title = $scope.customer.title;
                        $scope.customerList[$scope.selectedCustomerIndex].firstName = $scope.customer.fname;
                        $scope.customerList[$scope.selectedCustomerIndex].lastName = $scope.customer.lname;
                        $scope.customerList[$scope.selectedCustomerIndex].state = $scope.customer.state;
                        $scope.customerList[$scope.selectedCustomerIndex].city = $scope.customer.city;
                        $scope.customerList[$scope.selectedCustomerIndex].email = $scope.customer.email;
                        $scope.customerList[$scope.selectedCustomerIndex].mobile = $scope.customer.mobile;
                        $scope.customerList[$scope.selectedCustomerIndex].alternateMobile = $scope.customer.alternateMobile;
                        $scope.customerList[$scope.selectedCustomerIndex].notes = $scope.customer.notes;
                        $scope.customerList[$scope.selectedCustomerIndex].address1 = $scope.customer.address1;
                        $scope.customerList[$scope.selectedCustomerIndex].address2 = $scope.customer.address2;
                    } else {
                        $scope.customerList.splice($scope.selectedCustomerIndex, 1);
                        $scope.addNew(true);
                        $scope.customerDeleted = false;
                    }
                }
            }, function (apiError) {
                toaster.error(apiError.message);
                console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(errorStr + "10");
            })
    }

    // resetPassword() - Reset password
    $scope.resetPassword = function (mobile) {
        if (mobile == null || mobile == "") {
            toaster.info("Please enter mobile number.");
        }

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            mobile: mobile
        }

        $transporterQuote.callAPI5('/api/admin/password/reset',
            myParams,
            function (apiResponse) {
                //console.log(apiResponse.data);
                toaster.success(apiResponse.message);
            }, function (apiError) {
                toaster.error(apiError.message);
                console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(errorStr + "11");
            })
    }

    // confirmReset() -  Confirm swal popup
    $scope.confirmReset = function (mobile) {
        confirmSWALPopup(SweetAlert, "Are you sure you wish to reset password?", function () {
            $scope.resetPassword(mobile)
        },
            function () {
                return;
            })
    }

    // sendEmailToUser() - Open window to send an email to use
    $scope.sendEmailToUser = function (userEmail) {
        window.location.href = "mailto:" + userEmail;
    }

    // getRouteDetail() - Get route detail.
    $scope.getRouteDetail = function (index) {
        $scope.newRoute = false;
        $scope.selectedRouteIndex = index;
        routeID = $scope.routeList[index].routeID;
        $scope.route.code = $scope.routeList[index].routeCode;
        $scope.route.detail = $scope.routeList[index].routeDetail;
    }

    // deleteRoute() - Delete route
    $scope.deleteRoute = function () {
        confirmSWALPopup(SweetAlert, "Are you sure you want to delete this route?", function () {
            $scope.routeDeleted = true;
            $scope.addUpdateRoute();
        },
           function () {
               return;
           })
    }

    // addUpdateRoute() - Add/Update route.
    $scope.addUpdateRoute = function () {
        if ($scope.route.code == '') {
            toaster.info("Please enter a code.");
            return;
        }
        if ($scope.route.detail == '') {
            toaster.info("Please enter a detail.");
            return;
        }

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            routeID: routeID,
            code: $scope.route.code,
            detail: $scope.route.detail,
            isDeleted: $scope.routeDeleted
        }

        // Call api
        $transporterQuote.callAPI5('/api/admin/route/set',
          myParams,
          function (apiResponse) {
              // console.log(apiResponse.data);
              toaster.success(apiResponse.message);
              if (apiResponse.data != 0) {
                  $scope.routeList.push({
                      routeID: apiResponse.data,
                      routeCode: $scope.route.code,
                      routeDetail: $scope.route.detail,
                      routeInfo: $scope.route.code + '-' + $scope.route.detail
                  })
                  $scope.routeList = dlNaturalSort($scope.routeList, 'routeCode');
                  $scope.route = {
                      code: '',
                      detail: ''
                  }
              } else {
                  if (!$scope.routeDeleted) {
                      $scope.routeList[$scope.selectedRouteIndex].routeCode = $scope.route.code;
                      $scope.routeList[$scope.selectedRouteIndex].routeDetail = $scope.route.detail;
                      $scope.routeList[$scope.selectedRouteIndex].routeInfo = $scope.route.code + '-' + $scope.route.detail;
                  } else {
                      $scope.routeList.splice($scope.selectedRouteIndex, 1);
                      $scope.showRoute = true;
                      $scope.addNew(true);
                      $scope.routeDeleted = false;
                  }
              }


              // $scope.closeSetRouteModal();
          }, function (apiError) {
              toaster.error(apiError.message);
          },
          function (apiFailure) {
              toaster.warning(errorStr + "12");
          })
    }

    // openRouteSetPop() - Open route set popup.
    $scope.openRouteSetPop = function () {
        setRoute = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popSetRoute.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
    }

    // closeSetRouteModal() - Close set route popup
    $scope.closeSetRouteModal = function () {
        $scope.route = {
            code: '',
            detail: ''
        }
        setRoute.close();
    }

    // setRoute() - Set route
    $scope.setRoute = function () {
        if ($scope.route.code == '') {
            toaster.info("Please enter a code.");
            return;
        }
        if ($scope.route.detail == '') {
            toaster.info("Please enter a detail.");
            return;
        }

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            code: $scope.route.code,
            detail: $scope.route.detail
        }

        //Call api
        $transporterQuote.callAPI5('/api/admin/route/set',
          myParams,
          function (apiResponse) {
              //console.log(apiResponse.data);
              toaster.success(apiResponse.message);
              $scope.closeSetRouteModal();
          }, function (apiError) {
              toaster.error(apiError.message);
          },
          function (apiFailure) {
              toaster.warning(errorStr + "13");
          })
    }

    // openQuoteType() - Quote type
    $scope.openQuoteType = function () {
        $scope.showTransporters = false;
        $scope.showSources = false;
        $scope.showExecutives = false;
        $scope.showCustomers = false;
        $scope.showRoute = false;
        $scope.showQuoteType = true;
        $scope.selectedQuoteTypeIndex = -1;
        $scope.newQuoteType = true;
        $scope.showSlot = false;
        $scope.quoteType = {
            quoteTypeID: 0,
            quoteTypeName: '',
            paidBy: false,
            isDeleted: false,
            isAllowNegotiation: false,
            components: [],
            customer: false,
            quoteby: true,
            term: false,
            file: false,
            vendors: false,
            note: true,
            erpRef: false,
            customFields: [],
            serviceTypeID: '',
            serviceTypeName: '',
            deliveryDT: true,
            completionText: 'Delivery Date'
        }
        document.getElementById('menuDD').style.display = 'none';
    }

    // getQuoteTypes() - Get quote type
    var getQuoteTypes = function () {

        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        $transporterQuote.callAPI5('/api/admin/quoteType/get',
         myParams,
           function (apiResponse) {
               $scope.quoteTypeList = apiResponse.data;
               console.log(apiResponse.data);
           }, function (apiError) {
               toaster.error(apiError.message);
               console.log(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "14");
           })
    }

    // Calling function
    getQuoteTypes();

    // getQuoteTypeDetail() - Get quote type detail
    $scope.getQuoteTypeDetail = function (index) {
        $scope.quoteType.customFields = [];
        $scope.selectedQuoteTypeIndex = index;
        $scope.newQuoteType = false;

        $scope.quoteType = {
            quoteTypeID: $scope.quoteTypeList[index].quoteTypeID,
            quoteTypeName: $scope.quoteTypeList[index].quoteTypeName,
            paidBy: $scope.quoteTypeList[index].paidBy,
            type: $scope.quoteTypeList[index].type,
            isDeleted: $scope.quoteTypeList[index].isDeleted,
            isAllowNegotiation: $scope.quoteTypeList[index].isAllowNegotiation,
            components: $scope.quoteTypeList[index].components,
            customer: $scope.quoteTypeList[index].customer,
            quoteby: $scope.quoteTypeList[index].quoteby,
            term: $scope.quoteTypeList[index].term,
            file: $scope.quoteTypeList[index].file,
            vendors: $scope.quoteTypeList[index].vendors,
            note: $scope.quoteTypeList[index].note,
            erpRef: $scope.quoteTypeList[index].erpRef,
            customFields: [],
            serviceTypeID: $scope.quoteTypeList[index].serviceTypeID + '',
            serviceTypeName: '',
            coreField: $scope.quoteTypeList[index].coreFields,
            deliveryDT: $scope.quoteTypeList[index].deliveryDT,
            completionText: 'Delivery Date'
        }

        angular.forEach($scope.quoteTypeList[index].fields, function (item, key) {
            $scope.customField = {
                fieldID: item.FieldID,
                fieldName: item.FieldName,
                typeID: item.FieldType + '',
                typeName: '',
                choice: item.Choices,
                isEdit: false,
                isDelete: false,
                isNew: false,
                showCustomers: item.ShowCustomer,
                showVendors: item.ShowVendors
            }
            $scope.quoteType.customFields.push($scope.customField);
        })

        $scope.customField = {
            fieldName: '',
            typeID: '',
            typeName: '',
            choice: '',
            isEdit: false,
            isDelete: false,
            isNew: false,
            showCustomers: true,
            showVendors: true
        }

    }

    // addComponent() - Add new component
    $scope.addComponent = function () {
        if ($scope.component.ComponentName == "") {
            toaster.info("Please enter a component name.");
            return;
        }

        if ($scope.component.Type == "" || $scope.component.Type == 'Select') {
            toaster.info("Please select type.");
            return;
        }

        if ($scope.component.Type != 'Text' && ($scope.component.PaidBy == "" || $scope.component.PaidBy == 'Select')) {
            toaster.info("Please select paid by.");
            return;
        }

        if ($scope.component.Type != 'Text' && ($scope.component.Currency == "" || $scope.component.Currency == 'Select')) {
            toaster.info("Please select currency.");
            return;
        }
        $scope.component.IsNew = true;
        $scope.quoteType.components.push($scope.component);
        $scope.component = {
            ComponentID: 0,
            ComponentName: '',
            Type: '',
            PaidBy: '',
            Currency: '',
            Edit: false,
            IsDeleted: false,
            IsNew: false
        }
    }

    // submitQuoteType() - Submit quote  type
    $scope.submitQuoteType = function () {

        var coreFields = '';
        if ($scope.quoteType.quoteTypeName == "") {
            toaster.info("Please enter quote type name.");
            return;
        }

        if ($scope.quoteType.components.length == 0) {
            toaster.info("Please add at least one component.");
            return;
        }

        if ($scope.quoteType.paidBy == "") {
            toaster.info("Please select paid by.");
            return;
        }

        if ($scope.quoteType.customer == true) {
            if (coreFields == "") {
                coreFields += '~'
            }
            coreFields += '1~'
        }

        //if ($scope.quoteType.quoteby == true) {
        //    if (coreFields == "") {
        //        coreFields += '~'
        //    }
        //    coreFields += '2~'
        //}

        if ($scope.quoteType.term == true) {
            if (coreFields == "") {
                coreFields += '~'
            }
            coreFields += '3~'
        }

        if ($scope.quoteType.file == true) {
            if (coreFields == "") {
                coreFields += '~'
            }
            coreFields += '4~'
        }

        //if ($scope.quoteType.vendors == true) {
        //    if (coreFields == "") {
        //        coreFields += '~'
        //    }
        //    coreFields += '5~'
        //}

        //if ($scope.quoteType.note == true) {
        //    if (coreFields == "") {
        //        coreFields += '~'
        //    }
        //    coreFields += '6~'
        //}

        if ($scope.quoteType.erpRef == true) {
            if (coreFields == "") {
                coreFields += '~'
            }
            coreFields += '7~'
        }

        //if ($scope.quoteType.deliveryDT == true) {
        //    if (coreFields == "") {
        //        coreFields += '~'
        //    }
        //    coreFields += '8~'
        //}

        $scope.quoteType.coreField = coreFields;
        $transporterQuote.callAPI5('/api/admin/quoteType/set',
          $scope.quoteType,
           function (apiResponse) {
               $scope.quoteType = {
                   quoteTypeID: 0,
                   quoteTypeName: '',
                   paidBy: '',
                   type: '',
                   isDeleted: false,
                   isAllowNegotiation: false,
                   components: [],
                   customer: false,
                   quoteby: true,
                   term: false,
                   file: false,
                   vendors: false,
                   note: true,
                   erpRef: false,
                   customFields: [],
                   serviceTypeID: '',
                   serviceTypeName: '',
                   coreField: '',
                   deliveryDT: true,
                   completionText: 'Delivery Date'
               }
               toaster.success(apiResponse.message);
               getQuoteTypes();
               //console.log(apiResponse.data);
           }, function (apiError) {
               toaster.error(apiError.message);
               console.log(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "14");
           })
    }

    // editComponent() - Edit component
    $scope.editComponent = function (componentIndex) {
        if ($scope.quoteType.components[componentIndex].Edit == true && $scope.quoteType.components[componentIndex].ComponentID > 0) {
            $scope.updateComponent(componentIndex, false);
        }
        $scope.quoteType.components[componentIndex].Edit = !$scope.quoteType.components[componentIndex].Edit;
    }

    // changeTypeName() - Change type
    $scope.changeTypeName = function (typeNameIndex) {
        angular.forEach($scope.quoteTypeList, function (item, key) {
            if ($scope.copyFrom.value == item.quoteTypeName) {
                angular.forEach($scope.quoteTypeList[key].components, function (subitem, subkey) {
                    subitem.IsNew = true;
                    subitem.ComponentID = 0;
                    $scope.quoteType.components.push(subitem);
                })
            }
        })
    }

    // removeComponent() - Remove component
    $scope.removeComponent = function (componentIndex) {
        if ($scope.quoteType.components[componentIndex].ComponentID > 0) {
            $scope.updateComponent(componentIndex, true);
        } else {
            $scope.quoteType.components.splice(componentIndex, 1);
        }
    }

    // updateComponent() - Update component
    $scope.updateComponent = function (componentIndex, isDeleted) {

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            componentID: $scope.quoteType.components[componentIndex].ComponentID,
            componentName: $scope.quoteType.components[componentIndex].ComponentName,
            type: $scope.quoteType.components[componentIndex].Type,
            paidBy: $scope.quoteType.components[componentIndex].PaidBy,
            currency: $scope.quoteType.components[componentIndex].Currency,
            isDeleted: isDeleted
        }

        $transporterQuote.callAPI5('/api/admin/component/set',
         myParams,
          function (apiResponse) {
              toaster.success(apiResponse.message);
              if (isDeleted == true) {
                  $scope.quoteType.components.splice(componentIndex, 1);
              }
              getQuoteTypes();
              //console.log(apiResponse.data);
          }, function (apiError) {
              toaster.error(apiError.message);
              console.log(apiError.message);
          },
          function (apiFailure) {
              toaster.warning(errorStr + "15");
          })
    }

    // getService() - Get service
    var getService = function () {

        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        $transporterQuote.callAPI5('/api/admin/serviceType/get',
         myParams,
          function (apiResponse) {
              $scope.serviceList = apiResponse.data;
              angular.forEach($scope.serviceList, function (item, key) {
                  item.isChecked = false;
              })
              $scope.vendorsServices = angular.copy($scope.serviceList);
              console.log($scope.serviceList);
          }, function (apiError) {
              toaster.error(apiError.message);
              console.log(apiError.message);
          },
          function (apiFailure) {
              toaster.warning(errorStr + "17");
          })
    }

    // Calling
    getService();

    // addCustomField() - Add custom field
    $scope.addCustomField = function () {

        if ($scope.customField.fieldName == "") {
            toaster.info("Please enter field name.");
            return;
        }

        if ($scope.customField.typeID == "0" || $scope.customField.typeName == "Select") {
            toaster.info("Please select field type.");
            return;
        }
        $scope.customField.isNew = true;
        $scope.quoteType.customFields.push($scope.customField);
        $scope.customField = {
            fieldName: '',
            typeID: '',
            typeName: '',
            choice: '',
            isEdit: false,
            isDelete: false,
            isNew: false,
            showCustomers: true,
            showVendors: true
        }
    }

    // updateField() - Update custom field
    $scope.updateField = function (customIndex, isDeleted) {

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            fieldID: $scope.quoteType.customFields[customIndex].fieldID,
            fieldName: $scope.quoteType.customFields[customIndex].fieldName,
            typeID: $scope.quoteType.customFields[customIndex].typeID,
            choice: $scope.quoteType.customFields[customIndex].choice,
            isDeleted: isDeleted,
            showCustomers: $scope.quoteType.customFields[customIndex].showCustomers,
            showVendors: $scope.quoteType.customFields[customIndex].showVendors,
            quoteTypeID: $scope.quoteType.quoteTypeID
        }

        $transporterQuote.callAPI5('/api/admin/customField/set',
         myParams,
          function (apiResponse) {
              toaster.success(apiResponse.message);
              if (isDeleted == true) {
                  $scope.quoteType.customFields.splice(customIndex, 1);
              }
              getQuoteTypes();
              //console.log(apiResponse.data);
          }, function (apiError) {
              toaster.error(apiError.message);
              console.log(apiError.message);
          },
          function (apiFailure) {
              toaster.warning(errorStr + "18");
          })
    }

    // editCutomField() - Edit custom field
    $scope.editCutomField = function (customIndex) {
        if ($scope.quoteType.customFields[customIndex].isEdit == true && $scope.quoteType.customFields[customIndex].fieldID > 0) {
            $scope.updateField(customIndex, false);
        }
        $scope.quoteType.customFields[customIndex].isEdit = !$scope.quoteType.customFields[customIndex].isEdit;
    }

    // removeCutomField() - Remove custom field
    $scope.removeCutomField = function (customIndex) {
        if ($scope.quoteType.customFields[customIndex].fieldID > 0) {
            $scope.updateField(customIndex, true);
        } else {
            $scope.quoteType.customFields.splice(customIndex, 1);
        }
    }

    // showCoreField() - Show core field
    $scope.showCoreField = function () {
        $scope.isShowCoreField = !$scope.isShowCoreField
    }

    // showCustomField() - Show custom field
    $scope.showCustomField = function () {
        $scope.isShowCustom = !$scope.isShowCustom
    }

    // showComponent() - Show component
    $scope.showComponent = function () {
        $scope.isShowComponent = !$scope.isShowComponent
    }

    // changeService() - Change service
    $scope.changeService = function (serviceTypeID) {
        $scope.quoteTypesforCopy = [];
        angular.forEach($scope.quoteTypeList, function (subItem, subKey) {
            if (serviceTypeID == subItem.serviceTypeID) {
                $scope.quoteTypesforCopy.push(subItem);
            }
        })
    }

    // openSubMenu() - Open sub menu
    $scope.openSubMenu = function () {
        if ((document.getElementById('subMenu').style.display == 'none' || document.getElementById('subMenu').style.display == "")) {
            document.getElementById('subMenu').style.display = 'block';
        }
        else {
            document.getElementById('subMenu').style.display = 'none';
        }
    }

    // filterTransportList() - Filter transport list
    $scope.filterTransportList = function (serviceID) {

        $scope.transporterList = $scope.copyTransportList;

        if (serviceID > 0) {
            $scope.transporterList = $.grep($scope.transporterList, function (i) { if ($.inArray(serviceID + "", i.serviceTypeID.split("~")) != -1) { return i.serviceTypeID } });
        }
        document.getElementById('subMenu').style.display = 'none';
    }

    // getCountries() - Get all countires
    var getCountries = function () {
        var myParams = {};
        $transporterQuote.callAPI5(
            '/api/gen/countries/get',
            myParams,
           function (apiResponse) {
               $scope.countryList = apiResponse.data;
           }, function (apiError) {
               toaster.error(apiError.message);
           }, function (apiFailure) {
               toaster.warning(errorStr + "18");
           })
    }

    // Calling function
    getCountries();

    // changeCountry() - Change country
    $scope.changeCountry = function () {

        if ($scope.customer.country == "India") {
            $scope.isIndia = true;
        } else {
            $scope.isIndia = false;
        }
    }

    // openSlot() - Open slot
    $scope.openSlot = function () {
        $scope.newSource = false;
        $scope.showTransporters = false;
        $scope.showSources = false;
        $scope.showExecutives = false;
        $scope.showCustomers = false;
        $scope.showRoute = false;
        $scope.showQuoteType = false;
        $scope.showSlot = true;
        $scope.newSlot = true;
        $scope.selectedSlotIndex = -1;
        $scope.slot = {
            slotID: 0,
            fromTime: date,
            toTime: dateTo,
            capacity: 0,
            serviceTypeID: 1,
            serviceName: '',
            isDeleted: false
        }
        document.getElementById('menuDD').style.display = 'none';
    }

    // changeFromTime() - Change from time according set to time
    $scope.changeFromTime = function () {
        console.log(Number($scope.timeSlot.fromTime.split(':')[0]) + 2);
        var copyFromTime = $scope.timeSlot.fromTime
        var addtoTime = (Number($scope.timeSlot.fromTime.split(':')[0]) + 2) > 12 ? (Number($scope.timeSlot.fromTime.split(':')[0]) + 2) % 12 : Number($scope.timeSlot.fromTime.split(':')[0]) + 2
        if (Number(copyFromTime.split(':')[0]) >= 10) {
            copyFromTime = copyFromTime.replace("AM", 'PM')
            //console.log(copyFromTime);
        }
        $scope.timeSlot.toTime = copyFromTime.replace($scope.timeSlot.fromTime.split(':')[0], Number(addtoTime))
        console.log($scope.timeSlot.toTime);
    }

    // addUpdateSlot() - Add or update slot
    $scope.addUpdateSlot = function () {

        if ($scope.slot.capacity == 0) {
            toaster.info("Please enter a capacity.");
            return;
        }

        //if ($scope.slot.serviceTypeID == '') {
        //    toaster.info("Please select serviceType.");
        //    return;
        //}


        // Parameters
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            slotID: $scope.slot.slotID,
            slotFrom: convertTimeFormat($scope.slot.fromTime),
            slotTo: convertTimeFormat($scope.slot.toTime),
            capacity: $scope.slot.capacity,
            isDeleted: $scope.slotDeleted
        }

        // Call api
        $transporterQuote.callAPI5('/api/admin/slot/set',
           myParams,
           function (apiResponse) {
               // console.log(apiResponse.data);
               toaster.success(apiResponse.message);
               if (apiResponse.data != 0) {
                   $scope.slotList.push({
                       slotID: apiResponse.data,
                       fromTime: $scope.slot.fromTime + '',
                       toTime: $scope.slot.toTime + '',
                       capacity: $scope.slot.capacity,
                       serviceTypeID: $scope.slot.serviceTypeID,
                       serviceName: $scope.slot.serviceName
                   })

                   $scope.slot = {
                       slotID: 0,
                       fromTime: date,
                       toTime: dateTo,
                       capacity: 0,
                       serviceTypeID: 1,
                       serviceName: '',
                       isDeleted: false
                   }
               } else {
                   if (!$scope.slotDeleted) {
                       $scope.slotList[$scope.selectedSlotIndex].fromTime = $scope.slot.fromTime;
                       $scope.slotList[$scope.selectedSlotIndex].toTime = $scope.slot.toTime;
                       $scope.slotList[$scope.selectedSlotIndex].capacity = $scope.slot.capacity;
                       $scope.slotList[$scope.selectedSlotIndex].serviceTypeID = $scope.slot.serviceTypeID + '';
                       $scope.slotList[$scope.selectedSlotIndex].serviceName = $scope.slot.serviceName;
                   } else {
                       $scope.slotList.splice($scope.selectedSlotIndex, 1);
                       $scope.addNew(true);
                       $scope.slotDeleted = false;
                   }
               }

           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "19");
           })
    }

    // getSlot() - Get list of slot
    var getSlot = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        $transporterQuote.callAPI5('/api/admin/slot/get',
         myParams,
          function (apiResponse) {
              $scope.slotList = apiResponse.data;

              angular.forEach($scope.slotList, function (item, key) {
                  item.fromTime = convertTwelveHours((item.fromTime).slice(0, 5));
                  item.toTime = convertTwelveHours((item.toTime).slice(0, 5));
              })

              console.log($scope.slotList);
          }, function (apiError) {
              toaster.error(apiError.message);
              console.log(apiError.message);
          },
          function (apiFailure) {
              toaster.warning(errorStr + "20");
          })
    }

    // Calling
    getSlot();

    // changeSlotService() - Change service 
    $scope.changeSlotService = function (serviceTypeID) {
        angular.forEach($scope.serviceList, function (subItem, subKey) {
            if (serviceTypeID == subItem.serviceTypeID) {
                $scope.slot.serviceName = subItem.serviceTypeName;
            }
        })
    }

    // convertTimeFormat() - Convert to 24 hours time format
    function convertTimeFormat(str) {
        var time = str;
        var hours = Number(time.match(/^(\d+)/)[1]);
        var minutes = Number(time.match(/:(\d+)/)[1]);
        var AMPM = time.match(/\s(.*)$/)[1];
        if (AMPM == "PM" && hours < 12) hours = hours + 12;
        if (AMPM == "AM" && hours == 12) hours = hours - 12;
        var sHours = hours.toString();
        var sMinutes = minutes.toString();
        if (hours < 10) sHours = "0" + sHours;
        if (minutes < 10) sMinutes = "0" + sMinutes;
        return sHours + ":" + sMinutes;
    }

    // convertTwelveHours() - Convert to 12 hours time format
    function convertTwelveHours(time) {
        // Check correct time format and split into components
        time = time.toString().match(/^([01]\d|2[0-3])(:)([0-5]\d)(:[0-5]\d)?$/) || [time];

        if (time.length > 1) { // If time format correct
            time = time.slice(1);  // Remove full string match value
            time[5] = +time[0] < 12 ? ' AM' : ' PM'; // Set AM/PM
            time[0] = +time[0] % 12 || 12; // Adjust hours
        }
        return time.join(''); // return adjusted time or original string
    }

    // getSlotDetail() - Get slot detail
    $scope.getSlotDetail = function (index) {
        $scope.selectedSlotIndex = index;
        $scope.newSlot = false;
        $scope.slot.slotID = $scope.slotList[index].slotID;
        $scope.slot.fromTime = $scope.slotList[index].fromTime;
        $scope.slot.toTime = $scope.slotList[index].toTime;
        $scope.slot.capacity = $scope.slotList[index].capacity;
        $scope.slot.serviceTypeID = $scope.slotList[index].serviceTypeID + '';
        $scope.slot.serviceName = $scope.slotList[index].serviceName;
    }

    // deleteSlot() - Delete slot
    $scope.deleteSlot = function () {
        confirmSWALPopup(SweetAlert, "Are you sure you want to delete this slot?", function () {
            $scope.slotDeleted = true;
            $scope.addUpdateSlot();
        },
          function () {
              return;
          })
    }

    // openChangePasswordPop() - Open change password popup
    $scope.openChangePasswordPop = function () {
        changePasswordModal = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popChangePassword.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
    }

    // closeChangePassModal() - close Change password
    $scope.closeChangePassModal = function () {
        changePasswordModal.close();
        $scope.password = {
            oldPassword: '',
            newPassword: '',
            confirmPassword: ''
        }
    }

    // changePassword() - Change executive password
    $scope.changePassword = function () {
        if ($scope.password.oldPassword == '' || $scope.password.oldPassword == undefined) {
            toaster.info("Please enter your old password.");
            return;
        }
        if ($scope.password.newPassword == '' || $scope.password.newPassword == undefined) {
            toaster.info("Please enter your new password.");
            return;
        }
        //if ($scope.password.oldPassword == $scope.password.newPassword) {
        //    toaster.info("Your old password is same as new password.");
        //    return;
        //}
        if ($scope.password.confirmPassword == '' || $scope.password.confirmPassword == undefined) {
            toaster.info("Please enter your confirm password.");
            return;
        }
        if ($scope.password.newPassword != $scope.password.confirmPassword) {
            toaster.info("Passwords not match.");
            return;
        }

        confirmSWALPopup(SweetAlert, "Are you sure you wish to change password?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                oldPassword: $scope.password.oldPassword,
                newPassword: $scope.password.newPassword
            }
            // Call api
            $transporterQuote.callAPI5('/api/reqForQuote/password/change',
               myParams,
               function (apiResponse) {
                   //  console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   $scope.closeChangePassModal();
               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(apiFailure);
               })
        },
                         function () {
                             return;
                         })
    }

    // #endregion
})
// #endregion

// #region "ReqForQuote"
.controller("reqForQuoteController", function ($scope, $state, $transporterQuote, toaster, $uibModal, SweetAlert, cfpLoadingBar, $rootScope) {

    // #region "Variables"
    var infoCookie = window.localStorage.getItem('pm_info');
    if (infoCookie == null) {
        $transporterQuote.bye();
    }
    Date.prototype.customFormat = function (formatString) {
        var YYYY, YY, MMMM, MMM, MM, M, DDDD, DDD, DD, D, hhhh, hhh, hh, h, mm, m, ss, s, ampm, AMPM, dMod, th;
        YY = ((YYYY = this.getFullYear()) + "").slice(-2);
        MM = (M = this.getMonth() + 1) < 10 ? ('0' + M) : M;
        MMM = (MMMM = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][M - 1]).substring(0, 3);
        DD = (D = this.getDate()) < 10 ? ('0' + D) : D;
        DDD = (DDDD = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"][this.getDay()]).substring(0, 3);
        th = (D >= 10 && D <= 20) ? 'th' : ((dMod = D % 10) == 1) ? 'st' : (dMod == 2) ? 'nd' : (dMod == 3) ? 'rd' : 'th';
        formatString = formatString.replace("#YYYY#", YYYY).replace("#YY#", YY).replace("#MMMM#", MMMM).replace("#MMM#", MMM).replace("#MM#", MM).replace("#M#", M).replace("#DDDD#", DDDD).replace("#DDD#", DDD).replace("#DD#", DD).replace("#D#", D).replace("#th#", th);
        h = (hhh = this.getHours());
        if (h == 0) h = 24;
        if (h > 12) h -= 12;
        hh = h < 10 ? ('0' + h) : h;
        hhhh = h < 10 ? ('0' + hhh) : hhh;
        AMPM = (ampm = hhh < 12 ? 'am' : 'pm').toUpperCase();
        mm = (m = this.getMinutes()) < 10 ? ('0' + m) : m;
        ss = (s = this.getSeconds()) < 10 ? ('0' + s) : s;
        return formatString.replace("#hhhh#", hhhh).replace("#hhh#", hhh).replace("#hh#", hh).replace("#h#", h).replace("#mm#", mm).replace("#m#", m).replace("#ss#", ss).replace("#s#", s).replace("#ampm#", ampm).replace("#AMPM#", AMPM);
    };

    // Object
    $scope.search = {
        value: ''
    }
    $scope.reqForQuote = {
        reqForQuoteID: 0,
        executiveID: 0,
        customerID: 0,
        sourceID: 0,
        companyName: '-Select-',
        source: '-Select-',
        pickUpDT: '',
        deliveryDT: '',
        product: '',
        quoteBy: '',
        allowLaterDelivery: false,
        transpoters: '',
        details: '',
        designation: '',
        status: "0",
        isCloseEarly: false,
        destinationState: '',
        sourceState: '',
        sourceName: '',
        sourceAddress: '',
        destinationAddress: '',
        orderNo: '',
        routeID: 0,
        routeName: '',
        fileName: '',
        createdBy: '',
        dispatchedBy: '',
        zoneID: 0,
        zoneName: '-Select-',
        timeSlotFromDT: '',
        timeSlotToDT: '',
        vehicleEntryDT: '',
        vehicleReadyDT: '',
        clearanceDT: '',
        vehicleNumber: '',
        quoteTypeID: 0,
        quoteType: '-Select-',
        paymentTerms: 0,
        fixedTerm: '',
        requestSentDate: null,
        fields: [],
        budget: '',
        shipmentID: 0,
        serviceName: ''
    }
    $scope.password = {
        oldPassword: '',
        newPassword: '',
        confirmPassword: ''
    }

    $scope.titles = [{
        id: 1,
        name: 'Mr.'
    }, {
        id: 2,
        name: 'Ms.'
    }];
    $scope.customer = {
        companyName: '',
        title: 'Mr.',
        fname: '',
        lname: '',
        state: '',
        city: '',
        email: '',
        mobile: '',
        alternateMobile: '',
        notes: '',
        address1: '',
        address2: ''
    }
    $scope.transporter = {
        companyName: ''
    }

    // Arrays
    $scope.sourceList = [];
    $scope.customerList = [];
    $scope.reqTranspoterList = [];
    $scope.reqForQuoteList = [];
    $scope.transpoterList = [];
    $scope.selectedTranspoterName = [];
    $scope.quoteList = [];
    $scope.RFQCListopy = [];
    // bit
    $scope.showSearch = false;
    $scope.activeDeliveryDtLater = true;
    $scope.newReqForQuote = true;
    $scope.ShowTranspoterDetails = false;
    //  $scope.showCloseReq = false;
    $scope.showQuote = false;
    var subIRShow = null;
    $scope.showExpire = false;
    $scope.showAcceptedQuotr = false;
    $scope.showAcceptedBtn = true;
    $scope.showAutoComplete = false;
    $scope.selectAll = {
        isChecked: true
    }
    var cnt = 0;

    // date 
    var currentDT = new Date();
    var yesterday = currentDT;
    yesterday.setDate(yesterday.getDate() - 1);
    $scope.todayDT = {
        value: yesterday
    };
    //$scope.todayDT = currentDT.customFormat("#DD#-#MMM#-#YY#");
    var deliveryDT = new Date($scope.maxDeliveryDT);
    $scope.maxDeliveryDT = new Date(deliveryDT);

    $scope.selectedReqIndex = -1;
    $scope.quoteStatus = "0";
    $scope.quoteIndex = -1;
    var fileFolderPath = "";
    $scope.routeList = [];
    $scope.showRouteAuto = false;
    $scope.pendingCount = 0;
    $scope.readyCount = 0;
    $scope.forwardCount = 0;
    $scope.filterMenu = true;
    $scope.headerHeight = '100';
    var setVehicleReady = null;
    $scope.vehicle = {
        number: ''
    }
    var setTimeSlot = null;
    var unit = new Date().getHours() >= 12 ? " PM" : " AM";
    var minutes = new Date().getMinutes() < 10 ? "0" + new Date().getMinutes() : "" + new Date().getMinutes();
    var date = new Date().getHours() == 12 ? new Date().getHours() + ":" + minutes + unit : new Date().getHours() % 12 + ":" + minutes + unit;
    console.log(currentDT.getHours());
    var dateTo = new Date().getHours() == 12 ? new Date().getHours() + ":" + minutes + unit : new Date(new Date().setHours(new Date().getHours() + 2)).getHours() % 12 + ":" + minutes + unit;
    // console.log(new Date().setHours(new Date().getHours() + 2));
    console.log(dateTo);
    $scope.timeSlot = {
        loadingDT: currentDT.customFormat("#DD#-#MMM#-#YY#"),
        fromTime: date,
        toTime: dateTo
    }
    $scope.startCount = 0;
    $scope.completeCount = 0;
    $scope.dispatchedCount = 0;
    $scope.feedbackCount = 0;
    $scope.enableList = 8;
    $scope.showShcedule = null;
    $scope.quotationReceivedList = [];
    $scope.quotationPendingList = [];
    $scope.hideRemider = false;
    $scope.isShowInputandBtn = false;
    var popQuoteType = null;
    $scope.newQuoteType = true;
    var openDetailQuote = null;
    $scope.showQuotes = true;
    $scope.showLogistics = false;
    $scope.configValues = [];
    $scope.companyName = "";
    var addNotePopup = null;
    $scope.quote = {
        acceptNote: ''
    }
    $scope.startedCount = 0;
    $scope.dispatchPlanList = [];
    $scope.showDispatchPlan = true;
    $scope.selectedPlanIndex = -1;
    $scope.dispatchRFQList = [];
    var feedBackAndRates = null;
    $scope.componentsDetail = [];
    $scope.actualRates = [];
    $scope.id = 0;
    // #endregion

    // #region "Functions"

    // changePickUp() - Change pickup date 
    $scope.changePickUp = function (DT) {

        if (new Date(DT) < new Date($scope.reqForQuote.quoteBy)) {
            $scope.reqForQuote.quoteBy = "";
        }

        if (new Date(DT) > new Date($scope.reqForQuote.deliveryDT)) {
            $scope.reqForQuote.deliveryDT = "";
        }
    }

    // changeQuoteBy() - Change quote type
    $scope.changeQuoteBy = function (dt) {

        if (new Date(dt) > new Date($scope.reqForQuote.deliveryDT)) {
            $scope.reqForQuote.deliveryDT = "";
        }

    }

    // On click hide menu
    //$(window).click(function () {
    //    //Hide the menus if visible
    //    if ($scope.isSubMenu) {
    //        if (document.getElementById('subMenu').style.display == 'block') {
    //            document.getElementById('subMenu').style.display = 'none';
    //        }
    //    } else {
    //        $scope.isSubMenu = true;
    //       // $scope.showMenu = false;
    //    }
    //});

    // getProduct() - get the product list from api(Auto Complete)
    var getProduct = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            //product: $scope.reqForQuote.product
        }
        // Call api
        $transporterQuote.callAPI5('/api/reqForQuote/product/get',
           myParams,
           function (apiResponse) {
               // console.log(apiResponse.data);
               $scope.productList = apiResponse.data;
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "6");
           });
    }

    // getRoute() - Get routes
    var getRouteList = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            //product: $scope.reqForQuote.product
        }
        // Call api
        $transporterQuote.callAPI5('/api/reqForQuote/route/get',
           myParams,
           function (apiResponse) {
               //console.log(apiResponse.data);
               $scope.routeList = apiResponse.data;
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "5");
           });
    }

    // Calling
    getRouteList();

    // getExecQuoteType() - Get quote type according executive rigths of domestic and export
    var getExecQuoteType = function (serviceID, isDispatchPlan) {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            serviceRights: $scope.pm_personInfo.serviceRights,
            serviceID: serviceID,
            isDispatchPlan: isDispatchPlan
            // isDomestic: $scope.pm_personInfo.isDomestic,
            // isExport: $scope.pm_personInfo.isExport
        }
        // Call api
        $transporterQuote.callAPI5('/api/reqForQuote/execQType/get',
           myParams,
           function (apiResponse) {
               $scope.execQuoteTypes = apiResponse.data;

               if ($scope.execQuoteTypes.length == 1) {
                   $scope.reqForQuote.quoteTypeID = $scope.execQuoteTypes[0].quoteTypeID;
                   $scope.reqForQuote.quoteType = $scope.execQuoteTypes[0].quoteTypeName;
                   $scope.getFields($scope.execQuoteTypes[0].quoteTypeID);
               }
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "20");
           })
    }

    // Calling function
    getExecQuoteType(0, false);

    // changeQuoteType() - Change quote type
    $scope.changeQuoteType = function (index) {
        //  $scope.addNew();
        $scope.reqForQuote.quoteTypeID = $scope.execQuoteTypes[index].quoteTypeID;
        $scope.reqForQuote.quoteType = $scope.execQuoteTypes[index].quoteTypeName;
        // $scope.getFields($scope.reqForQuote.quoteTypeID);
        // $scope.closeTypeModal();
    }

    // openChangePasswordPop() - Open change password popup
    $scope.openChangePasswordPop = function () {
        changePasswordModal = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popChangePassword.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
    }

    // closeChangePassModal() - close Change password
    $scope.closeChangePassModal = function () {
        changePasswordModal.close();
        $scope.password = {
            oldPassword: '',
            newPassword: '',
            confirmPassword: ''
        }
    }

    // changePassword() - Change executive password
    $scope.changePassword = function () {
        if ($scope.password.oldPassword == '' || $scope.password.oldPassword == undefined) {
            toaster.info("Please enter your old password.");
            return;
        }
        if ($scope.password.newPassword == '' || $scope.password.newPassword == undefined) {
            toaster.info("Please enter your new password.");
            return;
        }
        //if ($scope.password.oldPassword == $scope.password.newPassword) {
        //    toaster.info("Your old password is same as new password.");
        //    return;
        //}
        if ($scope.password.confirmPassword == '' || $scope.password.confirmPassword == undefined) {
            toaster.info("Please enter your confirm password.");
            return;
        }
        if ($scope.password.newPassword != $scope.password.confirmPassword) {
            toaster.info("Passwords not match.");
            return;
        }

        confirmSWALPopup(SweetAlert, "Are you sure you wish to change password?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                oldPassword: $scope.password.oldPassword,
                newPassword: $scope.password.newPassword
            }
            // Call api
            $transporterQuote.callAPI5('/api/reqForQuote/password/change',
               myParams,
               function (apiResponse) {
                   //  console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   $scope.closeChangePassModal();
               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(apiFailure);
               })
        },
                         function () {
                             return;
                         })
    }

    // extendQuoteBy() - extend quoteby date
    $scope.reqCloseEarly = function (reqForQuoteID) {

        confirmSWALPopup(SweetAlert, "Are you sure you want to cancel this RFQ?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                reqForQuoteID: reqForQuoteID,
            }
            // Call api
            $transporterQuote.callAPI5('/api/reqForQuote/req/closeEarly',
               myParams,
               function (apiResponse) {
                   // console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   $scope.reqForQuoteList[$scope.selectedReqIndex].isCloseEarly = true;
                   angular.forEach($scope.RFQCListopy, function (item, key) {
                       if ($scope.reqForQuoteList[$scope.selectedReqIndex].reqForQuoteID == item.reqForQuoteID) {
                           item.isCloseEarly = true;
                       }
                   })
                   //scope.showCloseReq = true;
               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(errorStr + "4");
               });
        },
                   function () {
                       return;
                   })

    }

    // extendQuoteBy() - extend quoteby date
    $scope.extendQuoteBy = function (reqRequestInfo) {

        confirmSWALPopup(SweetAlert, "Are you sure wish to extend quote by date?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                reqForQuoteID: reqRequestInfo.reqForQuoteID,
                quoteByDT: reqRequestInfo.quoteBy
            }
            // Call api
            $transporterQuote.callAPI5('/api/reqForQuote/extend/QuoteBy',
               myParams,
               function (apiResponse) {
                   //  console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   $scope.getReqForQuote();
               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(errorStr + "3");
               })
        },
                   function () {
                       return;
                   })

    }

    // acceptQuote() - Accept quote
    $scope.acceptQuote = function () {
        confirmSWALPopup(SweetAlert, "Are you sure wish to accept this quote?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                reqForQuoteID: $scope.quoteList[$scope.quoteIndex].reqForQuoteID,
                quoteID: $scope.quoteList[$scope.quoteIndex].quoteID,
                transporterID: $scope.quoteList[$scope.quoteIndex].transporterID,
                note: $scope.quote.acceptNote
                //quoteID: $scope.
            }
            // Call api
            $transporterQuote.callAPI5('/api/reqForQuote/quote/accept',
               myParams,
               function (apiResponse) {
                   // console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   // $scope.quoteIndex = index;
                   if (apiResponse.data) {
                       $scope.reqForQuoteList[$scope.selectedReqIndex].status = "1";
                       $scope.reqForQuoteList[$scope.selectedReqIndex].statusStr = 'Accepted';
                       $scope.quoteList[$scope.quoteIndex].status = "1";
                       angular.forEach($scope.RFQCListopy, function (item, key) {
                           if ($scope.reqForQuoteList[$scope.selectedReqIndex].reqForQuoteID == item.reqForQuoteID) {
                               item.status = "1";
                               item.statusStr = 'Accepted';
                           }
                       })

                       if ($scope.enableList == 0) { //&& item.isCloseEarly == false
                           $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (item) { if (item.status == '0') { return item } });
                           $scope.pendingCount = $scope.reqForQuoteList.length;
                       }

                       if ($scope.enableList == 2) { //&& item.isCloseEarly == false
                           $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.status == '2') { return i } });
                           $scope.forwardCount = $scope.reqForQuoteList.length;
                       }
                       // && i.isCloseEarly == false
                       $scope.readyCount = $.grep($scope.RFQCListopy, function (i) { if (i.status == 1) { return i } }).length;
                       $scope.addNew();
                       $scope.closeNoteModal();
                       // $scope.$apply();
                   }
               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(errorStr + "2");
               })
        },
                           function () {
                               return;
                           })

    }

    // ToggleIRItem() - list in item expandable
    //$scope.ToggleIRItem = function (Response) {
    //    if ($scope.IsIRShown(Response)) {
    //        subIRShow = null;
    //    } else {
    //        subIRShow = Response;
    //    }
    //};

    // IsIRShown() - item expandable show or not check
    //$scope.IsIRShown = function (Response) {
    //    return subIRShow === Response;
    //};

    // getState() - Get all state
    var getState = function () {
        var myParams = {};
        $transporterQuote.callAPI5(
            '/api/gen/states/get',
            myParams,
           function (apiResponse) {
               $scope.states = apiResponse.data;
           }, function (apiError) {
               toaster.error(apiError.message);
           }, function (apiFailure) {
               toaster.warning(errorStr + "1");
           })
    }

    // Calling function
    getState();

    // getRoute() - Get route
    //var getRoute = function () {
    //    var myParams = {
    //        userID: $scope.pm_personInfo.userID
    //    }

    //    // Call api
    //    $transporterQuote.callAPI5('/api/admin/routes/get',
    //       myParams,
    //       function (apiResponse) {
    //           //console.log(apiResponse.data);
    //           $scope.routeList = apiResponse.data;
    //           console.log($scope.routeList);
    //       }, function (apiError) {
    //           console.log(apiError.message);
    //           toaster.error(apiError.message);
    //       },
    //       function (apiFailure) {
    //           toaster.warning(apiFailure);
    //       })
    //}

    // getRoute()
    //getRoute();

    // getQuote() - Get all request
    var getQuote = function (reqForQuoteID) {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        }
        // Call api
        $transporterQuote.callAPI5('/api/reqForQuote/quote/get',
           myParams,
           function (apiResponse) {
               // console.log(apiResponse.data);

               var tempApiResponseData = apiResponse.data;
               var nameObjectPairs = {};
               apiResponse.data = [];
               tempApiResponseData.forEach(function (quote) {
                   if (quote.reqForQuoteID == reqForQuoteID) {
                       if (quote.transCompanyName in nameObjectPairs) {
                           if (nameObjectPairs[quote.transCompanyName].roundNumber < quote.roundNumber)
                               nameObjectPairs[quote.transCompanyName] = quote;
                       }
                       else
                           nameObjectPairs[quote.transCompanyName] = quote;
                   }
               });

               for (var name in nameObjectPairs) {
                   apiResponse.data.push(nameObjectPairs[name]);
               }

               //compare() - to sort Quotations by charges
               function compare(a, b) {
                   return a.totalCharges - b.totalCharges;
               }
               //end

               angular.forEach(apiResponse.data, function (value, key) {

                   if (value.reqForQuoteID == reqForQuoteID) {
                       value.totalCharges = 0;
                       value.totalDollers = 0;
                       // $scope.quoteList = apiResponse.data;
                       if (value.components != "") {

                           value.components = JSON.parse(value.components);
                           angular.forEach(value.components, function (item, key) {
                               if (item.componentType == "Charge" && item.currency == "INR") {
                                   value.totalCharges = value.totalCharges + Number(item.componentValue);
                               } else if (item.componentType == "Charge" && item.currency == "USD") {
                                   value.totalCharges = value.totalCharges + (Number(item.componentValue) * value.exchangeRate);
                                   value.totalDollers = value.totalDollers + Number(item.componentValue);
                               }
                           });
                       }
                       $scope.quoteList.push(value);
                   }
               })
               //sort quoteList orderby smallest charges quotation first.
               $scope.quoteList.sort(compare);
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "7");
           })
    }

    // getTranspoterDetails() - Get transpoter details
    $scope.getTranspoterDetails = function (index) {
        $scope.ShowTranspoterDetails = true;
        $scope.newReqForQuote = false;
        $scope.hideRemider = false;
        //console.log(selectedReq);
        $scope.selectedReqIndex = index;
        $scope.reqForQuote.companyName = $scope.reqForQuoteList[index].customerName;
        $scope.reqForQuote.source = $scope.reqForQuoteList[index].sourceName;
        $scope.reqForQuote.pickUpDT = $scope.reqForQuoteList[index].pickUpDT;
        $scope.reqForQuote.deliveryDT = $scope.reqForQuoteList[index].deliveryDT;
        $scope.reqForQuote.product = $scope.reqForQuoteList[index].product;
        $scope.reqForQuote.quoteBy = $scope.reqForQuoteList[index].quoteByDT;
        $scope.reqForQuote.allowLaterDelivery = $scope.reqForQuoteList[index].allowLaterDelivery;
        $scope.reqForQuote.transpoters = $scope.reqForQuoteList[index].transporterIDs;
        $scope.reqForQuote.details = $scope.reqForQuoteList[index].details;
        $scope.reqForQuote.reqForQuoteID = $scope.reqForQuoteList[index].reqForQuoteID;
        $scope.reqForQuote.status = $scope.reqForQuoteList[index].status;
        $scope.reqForQuote.sourceName = $scope.reqForQuoteList[index].sourceName;
        $scope.reqForQuote.destination = $scope.reqForQuoteList[index].destination;
        $scope.reqForQuote.sourceAddress = $scope.reqForQuoteList[index].sourceAddress;
        $scope.reqForQuote.destinationAddress = $scope.reqForQuoteList[index].destinationAddress;
        $scope.reqForQuote.isCloseEarly = $scope.reqForQuoteList[index].isCloseEarly;
        $scope.reqForQuote.sourceState = $scope.reqForQuoteList[index].sourceState;
        $scope.reqForQuote.destinationState = $scope.reqForQuoteList[index].destinationState;
        $scope.reqForQuote.orderNo = $scope.reqForQuoteList[index].orderNo;
        $scope.reqForQuote.routeName = $scope.reqForQuoteList[index].routeName;
        $scope.reqForQuote.fileName = $scope.reqForQuoteList[index].fileName;
        $scope.reqForQuote.createdBy = $scope.reqForQuoteList[index].createdBy;
        $scope.reqForQuote.dispatchedBy = $scope.reqForQuoteList[index].dispatchedBy;
        $scope.reqForQuote.timeSlotFromDT = $scope.reqForQuoteList[index].timeSlotFromDT;
        $scope.reqForQuote.timeSlotToDT = $scope.reqForQuoteList[index].timeSlotToDT;
        $scope.reqForQuote.vehicleEntryDT = $scope.reqForQuoteList[index].vehicleEntryDT;
        $scope.reqForQuote.vehicleReadyDT = $scope.reqForQuoteList[index].vehicleReadyDT;
        $scope.reqForQuote.clearanceDT = $scope.reqForQuoteList[index].clearanceDT;
        $scope.reqForQuote.vehicleNumber = $scope.reqForQuoteList[index].vehicleNumber;
        $scope.reqForQuote.paymentTerms = $scope.reqForQuoteList[index].paymentTerm;
        $scope.reqForQuote.fixedTerm = $scope.reqForQuoteList[index].fixedTerm;
        $scope.reqForQuote.requestSentDate = $scope.reqForQuoteList[index].reminderSentDate;
        $scope.reqForQuote.fields = $scope.reqForQuoteList[index].fields;
        $scope.reqForQuote.budget = $scope.reqForQuoteList[index].budget;
        $scope.reqForQuote.serviceName = $scope.reqForQuoteList[index].serviceName

        if ($scope.reqForQuoteList[index].status == "1") { //|| new Date($scope.todayDT) > new Date($scope.reqForQuote.quoteBy)
            $scope.showAcceptedBtn = false;
        } else {
            $scope.showAcceptedBtn = true;
        }
        $scope.quotationReceivedList = [];
        $scope.quotationPendingList = [];
        $scope.quotationPendingList = angular.copy($scope.reqForQuoteList[index].transporterDetail);
        if ($scope.reqForQuoteList[index].response == $scope.reqForQuoteList[index].transporterDetail) {
            $scope.quotationReceivedList = angular.copy($scope.reqForQuoteList[index].transporterDetail);
        } else {
            angular.forEach($scope.reqForQuoteList[index].response, function (item, key) {
                angular.forEach($scope.quotationPendingList, function (subItem, subKey) {
                    if (item.TransporterID == subItem.transporterID) {
                        $scope.quotationReceivedList.push(subItem);
                        $scope.quotationPendingList.splice(subKey, 1);
                        // $scope.reqForQuoteList[index].transporterDetail.splice(subKey, 1);
                    }
                })
            })

        }


        if ($scope.reqForQuote.requestSentDate != null && new Date($scope.reqForQuote.requestSentDate).setHours(0, 0, 0, 0) == currentDT.setHours(0, 0, 0, 0)) {
            $scope.hideRemider = true;
        }
        $scope.quoteList = [];
        // call
        //getQuote()
        getQuote($scope.reqForQuote.reqForQuoteID);
        $scope.showQuote = true;
        //$scope.showAcceptedQuotr = false;

    }

    // getReqForQuote() - Get all request
    $scope.getReqForQuote = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        }
        // Call api
        $transporterQuote.callAPI5('/api/reqForQuote/get',
           myParams,
           function (apiResponse) {
               $scope.forwardCount = 0;
               $scope.readyCount = 0;
               $scope.startCount = 0;
               $scope.completeCount = 0;
               $scope.dispatchedCount = 0;
               $scope.pendingCount = 0;
               $scope.enableList = 8;
               for (var i = 0; i < apiResponse.data.length; i++) {                                                                  
                   apiResponse.data[i].quoteByDT = new Date(apiResponse.data[i].quoteByDT).customFormat("#DD#-#MMM#-#YY#");
                   apiResponse.data[i].statusStr = (apiResponse.data[i].status == '1') ? 'Accepted' : (apiResponse.data[i].isCloseEarly && apiResponse.data[i].status == '0') ? 'Cancelled' : (apiResponse.data[i].status == '2') ? 'Forwarded' : (apiResponse.data[i].status == '3') ? 'Started' : (apiResponse.data[i].status == '4') ? 'Completed' : (apiResponse.data[i].status == '5') ? 'Feedback' : '';
                   apiResponse.data[i].expired = new Date($scope.todayDT) > new Date(apiResponse.data[i].quoteByDT) && apiResponse.data[i].status == '0' && !apiResponse.data[i].isCloseEarly
                   apiResponse.data[i].transporterDetail = JSON.parse(apiResponse.data[i].transporterDetail);
                   apiResponse.data[i].fields = JSON.parse(apiResponse.data[i].fields);
                   apiResponse.data[i].response = JSON.parse(apiResponse.data[i].response);

                   var keyValPairs = {};
                   if (apiResponse.data[i].response.length) {
                       var tempArr = apiResponse.data[i].response;
                       apiResponse.data[i].response = [];
                       tempArr.forEach(function (obj) {
                           keyValPairs[obj.TransporterID] = obj;
                       });
                       for (var key in keyValPairs) {
                           apiResponse.data[i].response.push(keyValPairs[key]);
                       }
                   }

                   if (apiResponse.data[i].status == '2') { // && apiResponse.data[i].isCloseEarly == false
                       $scope.forwardCount++;
                   }

                   if (apiResponse.data[i].status == '1') { // && apiResponse.data[i].isCloseEarly == false
                       $scope.readyCount++;
                   }

                   if (apiResponse.data[i].status == '4') { // && apiResponse.data[i].isCloseEarly == false
                       $scope.dispatchedCount++;
                   }

                   if (apiResponse.data[i].status == '5') { // && apiResponse.data[i].isCloseEarly == false
                       $scope.feedbackCount++;
                       //$scope.vehicalReadyCount++;
                   }

                   if (apiResponse.data[i].status == '6') { // && apiResponse.data[i].isCloseEarly == false
                       $scope.timeSlotCount++;
                   }

                   if (apiResponse.data[i].status == '7') { // && apiResponse.data[i].isCloseEarly == false
                       $scope.enterVehCount++;
                   }

                   if (apiResponse.data[i].status == '3') { // && apiResponse.data[i].isCloseEarly == false
                       $scope.startCount++;
                   }

                   if (apiResponse.data[i].status == '0' && apiResponse.data[i].isCloseEarly == false) { // 
                       $scope.pendingCount++;
                   }

                   //if (Number(apiResponse.data[i].status) > 3 ) {
                   //    $scope.startedCount++;
                   //}

                   //if (Number(apiResponse.data[i].status) == '0' ) { 
                   //    $scope.startedCount++;
                   //}

               }
               $scope.reqForQuoteList = apiResponse.data;
               console.log($scope.reqForQuoteList);
               angular.copy($scope.reqForQuoteList, $scope.RFQCListopy);
               $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (item) { if (item.status == '0' && item.isCloseEarly == false) { return item } }); // 
               if (!$scope.pm_personInfo.canCreateRFQ && $scope.reqForQuoteList.length > 0) {
                   $scope.getTranspoterDetails(0);
               }
               $scope.isSelected = true;
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "8");
           })
    }

    // Call getReqForQuote()
    $scope.getReqForQuote();

    // Call getProduct()
    getProduct();

    // uploadFile() - Upload file
    $scope.uploadFile = function (selectedFile, evt) {
        cfpLoadingBar.start();
        $scope.showCancel = false;

        if (selectedFile.size > 5242880) {
            toaster.info("Please select a smaller file.");
            $scope.cleanup();
            return;
        }
        if (selectedFile.type != "application/pdf" && selectedFile.type != "application/msword" && selectedFile.type != "application/vnd.ms-excel") {
            toaster.info("Please upload pdf, word or excel file.");
            $scope.cleanup();
            return;
        }
       
        var data = new FormData();
        data.append(selectedFile.name, selectedFile);
        $.ajax({
            url: getFilePath() + "/FileUploadHandler.ashx?path=" + fileFolderPath + "&ID=0" + "&typeID=4",
            type: 'POST',
            data: data,
            contentType: false,
            async: false,
            processData: false,
            success: function (jResponse) {
                console.log(jResponse);
                $scope.showName = selectedFile.name;
                console.log($scope.showName);
                $rootScope.filePath = getFilePath() + JSON.parse(jResponse).FileName;
                if (jResponse.status == 200 && jResponse.responseText != "-") {
                    okCB(jResponse.responseText);
                } else {
                    //errorOpenSWALPopup("Please upload zip file!");
                }
                cfpLoadingBar.complete();
                // setActivity();
                $scope.showCancel = true;
                //overCB();
            },
            error: function (jError) {
                console.log(jError);
                if (jError.status == 200 && jError.responseText != "-") {
                    okCB(jError.responseText);
                } else {
                    //errorOpenSWALPopup("Please upload zip file!");
                }
                cfpLoadingBar.complete();
                //overCB();
                return;
            },
        })
    }

    // cleanup() - Clean
    $scope.cleanup = function () {
        var m = document.getElementsByClassName("file-path validate");
        var i;
        for (i = 0; i < m.length; i++) {
            m[i].value = null;
        }
        $scope.showName = "";
        $rootScope.filePath = "";
        $scope.showCancel = false;
    }

    // setCustomer() - Set Customer
    $scope.setCustomer = function () {
        var emailRegex = new RegExp("[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}$");

        if ($scope.customer.companyName == '' || typeof ($scope.customer.companyName) == undefined) {
            toaster.info("Please enter a company name.");
            return;
        } else if ($scope.customer.title == '' || $scope.customer.fname == '' || $scope.customer.lname == '') {
            toaster.info("Please enter a contact person.");
            return;
        } else if ($scope.customer.address1 == "") {
            toaster.info("Please enter address.");
            return;
        } else if ($scope.customer.state == '') {
            toaster.info('Please select a state.');
            return;
        } else if ($scope.customer.email == '') {
            toaster.info('Please enter a email address.');
            return;
        } else if (!emailRegex.test($scope.customer.email)) {
            toaster.info("Please enter a valid email address!");
            return;
        } else if ($scope.customer.mobile == "") {
            toaster.info('Please enter mobile number.');
            return;
        } else if ($scope.customer.mobile != "" && $scope.customer.mobile.length != 10) {
            toaster.info('Please enter a valid mobile number.');
            return;
        } else if ($scope.customer.alternateMobile != "" && $scope.customer.alternateMobile.length != 10) {
            toaster.info('Please enter a valid mobile number.');
            return;
        } else if ($scope.customer.city == "") {
            toaster.info('Please enter city.');
            return;
        }

        confirmSWALPopup(SweetAlert, "Are you sure you want to add this customer?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                companyName: $scope.customer.companyName,
                email: $scope.customer.email,
                city: $scope.customer.city,
                state: $scope.customer.state,
                title: $scope.customer.title,
                fname: $scope.customer.fname,
                lname: $scope.customer.lname,
                mobile: $scope.customer.mobile,
                alternateMobile: $scope.customer.alternateMobile,
                notes: $scope.customer.notes,
                address1: $scope.customer.address1,
                address2: $scope.customer.address2,
                isDeleted: false
            }
            $transporterQuote.callAPI5('/api/admin/customer/set',
                myParams,
                function (apiResponse) {
                    //  console.log(apiResponse.data);
                    if (apiResponse.data != 0) {
                        $scope.customerList.push({
                            customerID: apiResponse.data,
                            companyName: $scope.customer.companyName,
                            email: $scope.customer.email,
                            city: $scope.customer.city,
                            state: $scope.customer.state,
                            title: $scope.customer.title,
                            fname: $scope.customer.fname,
                            lname: $scope.customer.lname,
                            mobile: $scope.customer.mobile,
                            alternateMobile: $scope.customer.alternateMobile,
                            notes: $scope.customer.notes,
                            address1: $scope.customer.address1,
                            address2: $scope.customer.address2
                        })

                        $scope.reqForQuote.customerID = apiResponse.data;
                        $scope.reqForQuote.companyName = $scope.customer.companyName;
                        $scope.reqForQuote.destination = $scope.customer.city;
                    }
                    toaster.success(apiResponse.message);
                    $scope.customer = {
                        companyName: '',
                        title: 'Mr.',
                        fname: '',
                        lname: '',
                        state: '',
                        city: '',
                        email: '',
                        mobile: '',
                        alternateMobile: '',
                        notes: '',
                        address1: '',
                        address2: ''
                    }
                    $scope.closeCustomerModal();
                }, function (apiError) {
                    toaster.error(apiError.message);
                    console.log(apiError.message);
                },
                function (apiFailure) {
                    toaster.warning(errorStr + "9");
                })
        },
                   function () {
                       return;
                   })


    }

    // openCustomer() - Open Cutomer popup
    $scope.openCustomer = function () {
        customerModal = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popAddCustomer.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
        $scope.customer = {
            companyName: '',
            title: 'Mr.',
            fname: '',
            lname: '',
            state: '',
            city: '',
            email: '',
            mobile: '',
            alternateMobile: '',
            notes: '',
            address1: '',
            address2: ''
        }
    }

    // closeCustomerModal() - Close Cutomer popup
    $scope.closeCustomerModal = function () {
        customerModal.close();
    }

    // changeSource() - Change source
    $scope.changeSource = function (index) {
        $scope.reqForQuote.sourceID = $scope.sourceList[index].sourceID;
        $scope.reqForQuote.source = $scope.sourceList[index].location;
    }

    // changeCustomer() - Change customer
    $scope.changeCustomer = function (index) {
        $scope.reqForQuote.customerID = $scope.customerList[index].customerID;
        $scope.reqForQuote.companyName = $scope.customerList[index].companyName;
        $scope.reqForQuote.destination = $scope.customerList[index].city;
        $scope.dispatchPlan.destination = $scope.customerList[index].address1 + " " + $scope.customerList[index].address2;
    }

    // changeRoute() - change route
    $scope.changeRoute = function (index) {
        $scope.reqForQuote.routeID = $scope.routeList[index].routeID;
        $scope.reqForQuote.routeName = $scope.routeList[index].routeInfo;
    }

    // routeFocus() - Focus route
    $scope.routeFocus = function () {
        $scope.showAutoComplete = false;
        $scope.showRouteAuto = true;
    }

    // focusProduct() - Focus product
    $scope.focusProduct = function () {
        $scope.showAutoComplete = true;
        $scope.showRouteAuto = false;
    }

    // getTranspoter() - Get all customer
    var getTranspoter = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            serviceTypeID: $scope.fieldList.serviceTypeID
            // isDomestic: $scope.pm_personInfo.isDomestic,
            //  isExport: $scope.pm_personInfo.isExport
        }
        // Call api
        $transporterQuote.callAPI5('/api/reqForQuote/execTransporters/get',
           myParams,
           function (apiResponse) {
               //console.log(apiResponse.data);
               $scope.transpoterList = apiResponse.data;
               angular.forEach($scope.transpoterList, function (value, key) {
                   value.isChecked = true;
               });
               $scope.setTranspoeter(true);
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "10");
           })
    }

    // getCustomer() - Get all customer
    var getCustomer = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        }
        // Call api
        $transporterQuote.callAPI5('/api/admin/customers/get',
           myParams,
           function (apiResponse) {
               //console.log(apiResponse.data);
               $scope.customerList = apiResponse.data;
               if ($scope.customerList.length == 1) {
                   $scope.reqForQuote.customerID = $scope.customerList[0].customerID;
                   $scope.reqForQuote.companyName = $scope.customerList[0].companyName;
                   $scope.reqForQuote.destination = $scope.customerList[0].city;
               }
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "11");
           })
    }

    // Call getCustomer()
    getCustomer();

    // getSource() - Get all source.
    var getSource = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        }
        // Call api
        $transporterQuote.callAPI5('/api/admin/sources/get',
           myParams,
           function (apiResponse) {
               //console.log(apiResponse.data);
               $scope.sourceList = apiResponse.data;
               if ($scope.sourceList.length == 1) {
                   $scope.reqForQuote.sourceID = $scope.sourceList[0].sourceID;
                   $scope.reqForQuote.source = $scope.sourceList[0].location;
               }
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "12");
           })
    }

    // Call getSource()
    getSource();

    // setTranspoeter() - Selected transpoter
    $scope.setTranspoeter = function (flag) {
        $scope.selectedTranspoterName = $.grep($scope.transpoterList, function (i) { return i.isChecked; });
        $scope.subSelectedTransIDs = "~";
        angular.forEach($scope.selectedTranspoterName, function (value, key) {
            if (value.isChecked) {
                $scope.subSelectedTransIDs = $scope.subSelectedTransIDs + value.transporterID + "~";
            }
        });
        if (!flag) {
            $scope.closeTranspoterModal();
        }
        // $scope.selectAll.isChecked = $scope.transpoterList.length == $scope.selectedTranspoterName.length;

        // console.log($scope.selectedTranspoterName);
    }

    // actionSubmitReq() - Submit request for quote
    $scope.actionSubmitReq = function (reqObj) {

        var isValid = true;

        if ($scope.fieldList.customer && ($scope.reqForQuote.customerID == 0 || $scope.reqForQuote.customerID == 0)) {
            toaster.info("Please select company name.");
            return;
        }
        //if ($scope.reqForQuote.sourceID == 0 || $scope.reqForQuote.sourceID == 0) {
        //    toaster.info("Please selct source name.");
        //    return;
        //}

        //if ($scope.reqForQuote.product == "" || $scope.reqForQuote.product == undefined) {
        //    toaster.info("Please enter product.");
        //    return;
        //}
        if ($scope.reqForQuote.pickUpDT == "" || $scope.reqForQuote.pickUpDT == undefined) {
            toaster.info("Please select pick up date.");
            return;
        }
        //if ($scope.reqForQuote.pickUpDT > $scope.reqForQuote.deliveryDT) {
        //    toaster.info("Please select valid pick up date.");
        //    return;
        //}
        if ($scope.reqForQuote.deliveryDT == "" || $scope.reqForQuote.deliveryDT == undefined) {
            toaster.info("Please select delivery date.");
            return;
        }
        //if ($scope.reqForQuote.deliveryDT < $scope.reqForQuote.pickUpDT) {
        //    toaster.info("Please select valid delivery date.");
        //    return;
        //}
        if ($scope.reqForQuote.quoteBy == "" || $scope.reqForQuote.quoteBy == undefined) {
            toaster.info("Please select quote by.");
            return;
        }

        if (new Date($scope.reqForQuote.quoteBy) > new Date($scope.reqForQuote.deliveryDT)) {
            toaster.info("Please select valid quoteBy date.");
            return;
        }

        //if ($scope.reqForQuote.source == "" || $scope.reqForQuote.source == undefined) {
        //    toaster.info("Please select source.");
        //    return;
        //}

        if ($scope.reqForQuote.details == "" || $scope.reqForQuote.details == undefined) {
            toaster.info("Please enter details.");
            return;
        }

        if ($scope.selectedTranspoterName.length == 0) {
            toaster.info("Please select transporters.");
            return;
        }

        if ($scope.fieldList.erpRef && ($scope.reqForQuote.orderNo == '' || $scope.reqForQuote.product == undefined)) {
            toaster.info("Please enter Ref No.");
            return;
        }

        //if ($scope.reqForQuote.routeName == '') {
        //    toaster.info("Please enter route.");
        //    return;
        //}
        //if ($scope.reqForQuote.zoneID == 0) {
        //    toaster.info("Please select zone.");
        //    return;
        //}

        if ($scope.reqForQuote.quoteTypeID == 0) {
            toaster.info("Please select quote type.");
            return;
        }

        // Check validation for dynamic field
        angular.forEach($scope.fieldList.fields, function (item, key) {
            if (isValid && (item.FieldValue == "" || item.FieldValue == "Select")) {
                toaster.info("Please enter " + item.FieldName + ".");
                isValid = false;
            }
            if (!isValid) {
                return;
            }

        })

        if (!isValid) {
            return;
        }

        // Check select vendors
        if ($scope.fieldList.vendors && $scope.selectedTranspoterName.length == 0) {
            toaster.info("Please select vendors.");
            return;
        }

        // Parameters
        confirmSWALPopup(SweetAlert, "Are you sure you want to submit this RFQ?", function () {

            var myParams = {
                executiveID: $scope.pm_personInfo.userID,
                customerID: $scope.reqForQuote.customerID,
                quoteBy: $scope.reqForQuote.quoteBy,
                details: $scope.reqForQuote.details,
                transpoters: $scope.subSelectedTransIDs,
                status: $scope.reqForQuote.status,
                orderNo: $scope.reqForQuote.orderNo,
                fileName: $scope.showName == null ? "" : $scope.showName,
                quoteTypeID: $scope.reqForQuote.quoteTypeID,
                paymentTerms: $scope.reqForQuote.paymentTerms,
                fixedTerm: $scope.reqForQuote.fixedTerm,
                dynamicFields: $scope.fieldList.fields,
                deliveryDT: $scope.reqForQuote.deliveryDT,
                pickUpDT: $scope.reqForQuote.pickUpDT,
                budget: $scope.reqForQuote.budget ? $scope.reqForQuote.budget : 0,
                shipmentID: $scope.selectedPlanIndex > -1 ? $scope.dispatchPlanList[$scope.selectedPlanIndex].shipmentID : 0
            }

            // Call api
            $transporterQuote.callAPI5('/api/reqForQuote/set',
               myParams,
               function (apiResponse) {
                   // console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   if (apiResponse.data != 0) {
                       //$scope.reqForQuoteList.push({
                       //    reqForQuoteID: apiResponse.data,
                       //    executiveID: $scope.reqForQuote.executiveID,
                       //    customerID: $scope.reqForQuote.customerID,
                       //    sourceID: $scope.reqForQuote.sourceID,
                       //    companyName: $scope.reqForQuote.companyName,
                       //    source: $scope.reqForQuote.source,
                       //    pickUpDT: $scope.reqForQuote.pickUpDT,
                       //    deliveryDT: $scope.reqForQuote.deliveryDT,
                       //    product: $scope.reqForQuote.product,
                       //    quoteBy: $scope.reqForQuote.quoteBy,
                       //    allowLaterDelivery: $scope.reqForQuote.allowLaterDelivery,
                       //    transpoters: $scope.reqForQuote.transporterDetail,
                       //    details: $scope.reqForQuote.details,
                       //    designation: $scope.reqForQuote.designation,
                       //    status: $scope.reqForQuote.status
                       //})
                       $scope.showName = "";
                       $scope.reqForQuote = {
                           reqForQuoteID: 0,
                           executiveID: 0,
                           customerID: 0,
                           sourceID: 0,
                           companyName: '-Select-',
                           source: '-Select-',
                           pickUpDT: '',
                           deliveryDT: '',
                           product: '',
                           quoteBy: '',
                           allowLaterDelivery: false,
                           transpoters: '',
                           details: '',
                           designation: '',
                           status: "0",
                           isCloseEarly: false,
                           destinationState: '',
                           sourceState: '',
                           sourceName: '',
                           sourceAddress: '',
                           destinationAddress: '',
                           orderNo: '',
                           routeID: 0,
                           routeName: '',
                           fileName: '',
                           createdBy: '',
                           dispatchedBy: '',
                           zoneID: 0,
                           zoneName: '-Select-',
                           timeSlotFromDT: '',
                           timeSlotToDT: '',
                           vehicleEntryDT: '',
                           vehicleReadyDT: '',
                           clearanceDT: '',
                           vehicleNumber: '',
                           quoteTypeID: 0,
                           quoteType: '-Select-',
                           paymentTerms: 0,
                           fixedTerm: '',
                           requestSentDate: null,
                           fields: [],
                           budget: '',
                           serviceName: ''
                       }
                       $scope.fieldList = [];
                       $scope.newQuoteType = true;
                       $scope.isShowInputandBtn = false;
                       $scope.selectedTranspoterName = [];

                       // For dispatch plan
                       if ($scope.selectedPlanIndex != -1) {
                           $scope.dispatchPlanList.splice($scope.selectedPlanIndex, 1);
                       }
                   }
                   // call getReqForQuote()
                   $scope.getReqForQuote();

               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(errorStr + "13");
               })
        },
                    function () {
                        return;
                    })


        // $scope.reqForQuoteList.push(reqObj);
    }

    // searchProduct() - Search product
    $scope.searchInList = function () {
        $scope.showSearch = !$scope.showSearch;
        setTimeout(function () { document.getElementById("search").focus() }, 1000);
        $scope.search = {
            value: ''
        }
        $scope.headerHeight = '100';
    }

    // openTranspoterModal() - open transpoter modal
    $scope.openTranspoterModal = function (selectedTranspoterList) {

        angular.forEach($scope.transpoterList, function (value, key) {
            if (selectedTranspoterList.length == 0) {
                value.isChecked = true;
            }
            angular.forEach(selectedTranspoterList, function (item, key1) {
                if (value == item) {
                    value.isChecked = true;
                }
            });
        });

        $scope.selectAll.isChecked = selectedTranspoterList.length == 0 ? true : $scope.transpoterList.length == selectedTranspoterList.length;
        //if (selectedTranspoterList.length == 0) {
        //    $scope.selectedTranspoterName = $.grep($scope.transpoterList, function (i) { return i.isChecked; });
        //}

        transpoterModal = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popAddTranspoter.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
        //$scope.setTranspoeter();
    }

    // closeTranspoterModal() - Close transpoter modal 
    $scope.closeTranspoterModal = function () {

        transpoterModal.close();
        // angular.forEach($scope.transpoterList, function (value, key) { value.isChecked = false; });
    }

    // allowLaterDelivery() - True/False allow later dilivery
    $scope.allowLaterDelivery = function () {
        $scope.reqForQuote.allowLaterDelivery = !$scope.reqForQuote.allowLaterDelivery;
    }

    // openQuoteType() - Open quote type
    $scope.openQuoteType = function () {
        popQuoteType = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popQuoteType.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
    }

    // closeTypeModal() - Close type model 
    $scope.closeTypeModal = function () {
        popQuoteType.close();
    }

    // adduser() - Add request for quote
    $scope.addNew = function () {
        $scope.reqForQuote = {
            reqForQuoteID: 0,
            executiveID: 0,
            customerID: 0,
            sourceID: 0,
            companyName: '-Select-',
            source: '-Select-',
            pickUpDT: '',
            deliveryDT: '',
            product: '',
            quoteBy: '',
            allowLaterDelivery: false,
            transpoters: '',
            details: '',
            designation: '',
            status: "0",
            isCloseEarly: false,
            destinationState: '',
            sourceState: '',
            sourceName: '',
            sourceAddress: '',
            destinationAddress: '',
            orderNo: '',
            routeID: 0,
            routeName: '',
            fileName: '',
            createdBy: '',
            dispatchedBy: '',
            zoneID: 0,
            zoneName: '-Select',
            timeSlotFromDT: '',
            timeSlotToDT: '',
            vehicleEntryDT: '',
            vehicleReadyDT: '',
            clearanceDT: '',
            vehicleNumber: '',
            quoteTypeID: 0,
            quoteType: '-Select-',
            paymentTerms: 0,
            fixedTerm: '',
            requestSentDate: null,
            fields: [],
            budget: '',
            serviceName: ''
        }
        getExecQuoteType(0, false);
        $scope.ShowTranspoterDetails = false;
        $scope.selectedTranspoterName = [];
        $scope.newReqForQuote = true;
        $scope.showQuote = false;
        $scope.selectedReqIndex = -1;
        $scope.selectedPlanIndex = -1;
        angular.forEach($scope.transpoterList, function (value, key) {
            value.isChecked = true;
        });
        $scope.setTranspoeter(true);
        $scope.fieldList = [];
        $scope.isShowInputandBtn = false;
        $scope.newQuoteType = true;
    }

    // openSubMenu() - Open sub menu
    $scope.openSubMenu = function () {
        if ((document.getElementById('subMenu').style.display == 'none' || document.getElementById('subMenu').style.display == "")) {
            document.getElementById('subMenu').style.display = 'block';
            $scope.isSubMenu = false;
        }
        else {
            document.getElementById('subMenu').style.display = 'none';
        }
    }

    // filterList() - Filter list by accepting, pending and expired
    $scope.filterList = function (id) {
        $scope.id = id;
        $scope.reqForQuoteList = [];
        $scope.reqForQuoteList = $scope.RFQCListopy;
        $scope.showDispatchPlan = false;
        if (id == 1) {
            $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.status == 1) { return i } }); // && i.isCloseEarly == false
            $scope.enableList = 1;
        } else if (id == 2) {
            $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.isCloseEarly == true && ((i.status == '0'))) { return i } }); // 
            $scope.enableList = -1;
        } else if (id == 0) {
            $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (item) { if (item.status == '0' && item.isCloseEarly == false) { return item } }); //&& new Date(item.quoteByDT) >= new Date($scope.todayDT) && item.isCloseEarly == false
            $scope.enableList = 0;
        } else if (id == 3) {
            $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.status == '2') { return i } });
            $scope.enableList = 2;
        } else if (id == 4) {
            $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.status == '3') { return i } });
            $scope.enableList = 3;
        } else if (id == 5) {
            $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.status == '4') { return i } });
            $scope.enableList = 4;
            $scope.feedbackBtnName = 'Feedback & Actuals';
        } else if (id == 6) {
            $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.status == '5') { return i } });
            $scope.enableList = 5;
        } else if (id == 7) {
            $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.status == '6') { return i } });
            $scope.enableList = 6;
        } else if (id == 8) {
            $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.status == '7') { return i } });
            $scope.enableList = 7;
        } else if (id == 9) {
            $scope.showDispatchPlan = true;
            $scope.enableList = 8;
            $scope.addNew();
        } else if (id == 10) {
            $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.status == '5') { return i } });
            $scope.enableList = 9;
            $scope.feedbackBtnName = 'Show Feedback & Rates';
        }
        //document.getElementById('subMenu').style.display = 'none';
        if (!$scope.showDispatchPlan) {
            if ($scope.reqForQuoteList.length > 0) { // $scope.pm_personInfo.canCreateRFQ &&
                $scope.getTranspoterDetails(0);
            } else {
                $scope.addNew();
            }
        }
    }

    // forwardRFQ() - RFQ forward to customer
    $scope.forwardRFQ = function (reqForQuoteID) {
        confirmSWALPopup(SweetAlert, "You will not be able to recieve any quote thereafter.", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                reqForQuoteID: reqForQuoteID,
            }
            // Call api
            $transporterQuote.callAPI5('/api/reqForQuote/req/forward',
               myParams,
               function (apiResponse) {
                   // console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   $scope.reqForQuoteList[$scope.selectedReqIndex].status = '2';
                   $scope.reqForQuoteList[$scope.selectedReqIndex].statusStr = 'Forwarded';
                   $scope.reqForQuoteList[$scope.selectedReqIndex].isCloseEarly = true;
                   angular.forEach($scope.RFQCListopy, function (item, key) {
                       if ($scope.reqForQuoteList[$scope.selectedReqIndex].reqForQuoteID == item.reqForQuoteID) {
                           item.status = '2';
                           item.statusStr = 'Forwarded';
                           item.isCloseEarly = true;
                       }
                   })

                   $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (item) { if (item.status == '0') { return item } });
                   $scope.pendingCount = $scope.reqForQuoteList.length;
                   $scope.forwardCount = $.grep($scope.RFQCListopy, function (i) { if (i.status == '2') { return i } }).length;

                   $scope.addNew();
                   //scope.showCloseReq = true;
               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(errorStr + "14");
               });
        },
                 function () {
                     return;
                 })
    }

    // dispatchRFQ() - RFQ dispatch
    $scope.dispatchRFQ = function (reqForQuoteID) {
        confirmSWALPopup(SweetAlert, "Are you sure you want to dispatch this order?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                reqForQuoteID: reqForQuoteID,
            }
            // Call api
            $transporterQuote.callAPI5('/api/reqForQuote/req/dispatch',
               myParams,
               function (apiResponse) {
                   // console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   $scope.reqForQuoteList[$scope.selectedReqIndex].status = '3';
                   $scope.reqForQuoteList[$scope.selectedReqIndex].statusStr = 'Dispatched';
                   $scope.reqForQuoteList[$scope.selectedReqIndex].dispatchDT = new Date();
                   angular.forEach($scope.RFQCListopy, function (item, key) {
                       if ($scope.reqForQuoteList[$scope.selectedReqIndex].reqForQuoteID == item.reqForQuoteID) {
                           item.status = '3';
                           item.statusStr = 'Dispatched';
                           item.dispatchDT = new Date();
                       }
                   })
                   //  $scope.reqForQuoteList = [];
                   //$scope.reqForQuoteList = $scope.RFQCListopy
                   $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (i) { if (i.status == '7') { return i } });
                   $scope.enterVehCount = $scope.reqForQuoteList.length;
                   $scope.dispatchedCount = $.grep($scope.RFQCListopy, function (i) { if (i.status == '3') { return i } }).length;

                   $scope.addNew();
                   //scope.showCloseReq = true;
               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(errorStr + "15");
               });
        },
                 function () {
                     return;
                 })
    }

    // productList() - Search product.
    $scope.productList = function () {
        setTimeout(function () { document.getElementById("product").focus() }, 1000);
        $scope.showAutoComplete = true;
        $scope.reqForQuote = {
            product: ''
        }
    }

    // setProduct() - Set product in textbox.
    $scope.setProduct = function (selectedProduct) {
        $scope.reqForQuote.product = selectedProduct;
        $scope.showAutoComplete = false;
    }

    // setRoute() - Set route
    $scope.setRoute = function (selectedRoute) {
        $scope.reqForQuote.routeName = selectedRoute;
        $scope.showRouteAuto = false;
    }

    // addAll() - Add all transporter
    $scope.addAll = function () {
        // Check for selectAll is checked or not
        for (var i = 0; i < $scope.transpoterList.length; i++) {
            if ($scope.selectAll.isChecked == true) {

                if (!$scope.transpoterList[i].isChecked) {
                    $scope.transpoterList[i].isChecked = true;
                }

            } else {
                $scope.transpoterList[i].isChecked = false;
            }
        }
    }

    // addTransporter() - Transporter selection 
    $scope.addTransporter = function () {
        cnt = 0;
        for (var i = 0; i < $scope.transpoterList.length; i++) {
            if ($scope.transpoterList[i].isChecked) {
                cnt++;
            }
        }

        $scope.selectAll.isChecked = $scope.transpoterList.length == cnt;

    }

    // getExecZones() - Get zones
    var getExecZones = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        };
        $transporterQuote.callAPI5(
            '/api/reqForQuote/execZone/get',
            myParams,
           function (apiResponse) {
               $scope.executiveZones = apiResponse.data;
               if ($scope.executiveZones.length == 1) {
                   $scope.reqForQuote.zoneID = $scope.executiveZones[0].zoneID;
                   $scope.reqForQuote.zoneName = $scope.executiveZones[0].zoneName;
               }
           }, function (apiError) {
               toaster.error(apiError.message);
           }, function (apiFailure) {
               toaster.warning(errorStr + "14");
           })
    }

    // Calling function
    getExecZones();

    // changeZone() - Change zone
    $scope.changeZone = function (index) {
        $scope.reqForQuote.zoneID = $scope.executiveZones[index].zoneID;
        $scope.reqForQuote.zoneName = $scope.executiveZones[index].zoneName;
    }

    // closeFilterMenu() - Close filter menu
    $scope.closeFilterMenu = function () {
        $scope.filterMenu = !$scope.filterMenu;
        if ($scope.filterMenu) {
            $scope.headerHeight = '100';
        } else {
            $scope.headerHeight = '50';
        }
    }

    // changeFromTime() - Change from time according set to time
    $scope.changeFromTime = function () {
        console.log(Number($scope.timeSlot.fromTime.split(':')[0]) + 2);
        var copyFromTime = $scope.timeSlot.fromTime
        var addtoTime = (Number($scope.timeSlot.fromTime.split(':')[0]) + 2) > 12 ? (Number($scope.timeSlot.fromTime.split(':')[0]) + 2) % 12 : Number($scope.timeSlot.fromTime.split(':')[0]) + 2
        if (Number(copyFromTime.split(':')[0]) >= 10) {
            copyFromTime = copyFromTime.replace("AM", 'PM')
            //console.log(copyFromTime);
        }
        $scope.timeSlot.toTime = copyFromTime.replace($scope.timeSlot.fromTime.split(':')[0], Number(addtoTime))
        console.log($scope.timeSlot.toTime);
    }

    // openTimeSlotPop() - Open time slot popup
    $scope.openSchedulePop = function () {
        $scope.showShcedule = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popShowSchedule.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
    }

    // openReport() - Open report
    $scope.openReport = function () {
        $state.go('report');
    }

    // remiderSent() - Remider sent to transporter
    $scope.remiderSent = function () {
        var reminderTransID = "~";
        angular.forEach($scope.quotationPendingList, function (item, key) {
            reminderTransID = reminderTransID + item.transporterID + "~";
        })

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            rfqID: $scope.reqForQuote.reqForQuoteID,
            transporters: reminderTransID,
            dynamicFields: $scope.reqForQuote.fields
        }
        confirmSWALPopup(SweetAlert, "Are you sure you want to send reminder?", function () {
            $transporterQuote.callAPI5(
              '/api/reqForQuote/reminder/set',
              myParams,
             function (apiResponse) {
                 $scope.hideRemider = true;
                 toaster.success(apiResponse.message);
             }, function (apiError) {
                 toaster.error(apiError.message);
             }, function (apiFailure) {
                 toaster.warning(errorStr + "17");
             })
        }, function () {
            return;
        })
    }

    // getFields() - Get field value
    $scope.getFields = function (quoteTypeID) {

        if (quoteTypeID == 0) {
            toaster.info("Please select quote type.");
            return;
        }

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            quoteTypeID: quoteTypeID
        }

        // Call api
        $transporterQuote.callAPI5('/api/reqForQuote/fields/get',
           myParams,
           function (apiResponse) {
               $scope.fieldList = apiResponse.data;
               $scope.completionText = $scope.fieldList.completionText;
               $scope.pickupText = $scope.fieldList.pickupText;
               angular.forEach($scope.fieldList.fields, function (item, key) {

                   if (item.FieldType == 4) {
                       item.FieldValue = 'Select';
                       item.Choices = item.Choices.split(",");
                   } else {
                       item.FieldValue = '';
                   }

                   // Check for dispatch plan
                   if (item.FieldName == "Containers" && $scope.selectedPlanIndex != -1) {
                       item.FieldValue = $scope.dispatchPlanList[$scope.selectedPlanIndex].container + '';
                   }

                   if (item.FieldName == "Source Location" && $scope.selectedPlanIndex != -1) {
                       item.FieldValue = $scope.dispatchPlanList[$scope.selectedPlanIndex].source + '';
                   }

                   if (item.FieldName == "Product / Plant" && $scope.selectedPlanIndex != -1) {
                       item.FieldValue = $scope.dispatchPlanList[$scope.selectedPlanIndex].product + '';
                   }

                   if (item.FieldName == "Source Address" && $scope.selectedPlanIndex != -1) {
                       item.FieldValue = $scope.dispatchPlanList[$scope.selectedPlanIndex].sourceAddress + '';
                   }

                   if (item.FieldName == "Destination Address" && $scope.selectedPlanIndex != -1) {
                       item.FieldValue = $scope.dispatchPlanList[$scope.selectedPlanIndex].destination + '';
                   }

                   if (item.FieldName == "Port of Loading" && $scope.selectedPlanIndex != -1) {
                       item.FieldValue = $scope.dispatchPlanList[$scope.selectedPlanIndex].portofLoading + ''
                   }

                   if (item.FieldName == "Port of Discharge" && $scope.selectedPlanIndex != -1) {
                       item.FieldValue = $scope.dispatchPlanList[$scope.selectedPlanIndex].portofDischarge + ''
                   }

                   if (item.FieldName == "Empty Container Pickup" && $scope.selectedPlanIndex != -1) {
                       item.FieldValue = $scope.dispatchPlanList[$scope.selectedPlanIndex].emptyContainer + ''
                   }
               })
               console.log($scope.fieldList);
               $scope.isShowInputandBtn = true;
               $scope.newQuoteType = false;

               // Check for dispatch plan
               if ($scope.fieldList.customer && $scope.selectedPlanIndex != -1) {
                   $scope.reqForQuote.customerID = $scope.dispatchPlanList[$scope.selectedPlanIndex].customerID;
                   $scope.reqForQuote.companyName = $scope.dispatchPlanList[$scope.selectedPlanIndex].companyName;
               }

               if ($scope.fieldList.file && $scope.selectedPlanIndex != -1) {
                   $scope.reqForQuote.fileName = $scope.dispatchPlanList[$scope.selectedPlanIndex].fileName;
                   $scope.showName = $scope.dispatchPlanList[$scope.selectedPlanIndex].fileName;
               }

               // Call getTranspoter()
               getTranspoter();
               // $scope.$apply();
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "18");
           })
    }

    // changeField() - Choice set index
    $scope.changeField = function (parentIndex, choiceIndex) {
        $scope.fieldList.fields[parentIndex].FieldValue = $scope.fieldList.fields[parentIndex].Choices[choiceIndex];
    }

    // showQuotesDetail() - Show other detail of quotes
    $scope.showQuotesDetail = function () {
        var a = Number($scope.reqForQuote.reqForQuoteID) * 2018 + '543~' + Number($scope.pm_personInfo.userID) * 2018 + '730~0&id=' + $scope.id;
        window.open("customer.html?q=" + a, "#2", "Height= 600px, Width= 1250px, Top= 50px, Left= 100px, edge= Raised, center= Yes, help= No, resizable= No, status= No, scrollbars=1");
        //openDetailQuote = $uibModal.open({
        //    animation: true,
        //    templateUrl: 'Html/Popups/popShowQuoteDetail.html',
        //    windowClass: 'AddUser',
        //    size: '1g',
        //    backdrop: 'static',
        //    keyboard: false,
        //    scope: $scope
        //});
    }

    // closeQuoteList() - Close popup 
    $scope.closeQuoteList = function () {
        openDetailQuote.close();
    }

    // openQuotes() - Open quotes
    $scope.openQuotes = function () {
        $state.go('reqForQuote');
    }

    // openLogistics() - Open logistics
    $scope.openLogistics = function () {  
        $state.go('transportActivity');
    }

    

    // getConfigValue() - Get config value
    var getConfigValue = function () {
        var myParams = {}

        $transporterQuote.callAPI5(
            '/api/gen/config/get',
            myParams,
            function (apiResponse) {
                $scope.configValues = apiResponse.data;
                console.log($scope.configValues);

                angular.forEach($scope.configValues, function (item, key) {
                    if (String(item.Field).toLowerCase() == String("CompanyName").toLowerCase()) {
                        $scope.companyName = item.Value;
                    }
                })
            },
            function (apiError) {
                console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(apiFailure);
            }
        );
    }

    // Calling
    getConfigValue();

    // openTimeSlotPop() - Open time slot popup
    $scope.openAddNotePop = function (index) {

        $scope.quote = {
            acceptNote: ''
        }

        addNotePopup = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popAddNote.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });

        $scope.quoteIndex = index;
    }

    // closeNoteModal() - Close add note pop
    $scope.closeNoteModal = function () {
        addNotePopup.close();
    }

    // getDispatchPlan() - Get dispatch plan 
    var getDispatchPlan = function () {

        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        // Call api
        $transporterQuote.callAPI5('/api/reqForQuote/dispatchPlan/get',
           myParams,
           function (apiResponse) {
               if (apiResponse.data.length != 0) {
                   $scope.dispatchPlanList = apiResponse.data;
               }
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "19");
           });
    }

    // Calling
    getDispatchPlan();

    // getDispatchDetail() - Get dispatch detail
    $scope.getDispatchDetail = function (index) {
        $scope.reqForQuote = {
            reqForQuoteID: 0,
            executiveID: 0,
            customerID: 0,
            sourceID: 0,
            companyName: '-Select-',
            source: '-Select-',
            pickUpDT: '',
            deliveryDT: '',
            product: '',
            quoteBy: '',
            allowLaterDelivery: false,
            transpoters: '',
            details: '',
            designation: '',
            status: "0",
            isCloseEarly: false,
            destinationState: '',
            sourceState: '',
            sourceName: '',
            sourceAddress: '',
            destinationAddress: '',
            orderNo: '',
            routeID: 0,
            routeName: '',
            fileName: '',
            createdBy: '',
            dispatchedBy: '',
            zoneID: 0,
            zoneName: '-Select',
            timeSlotFromDT: '',
            timeSlotToDT: '',
            vehicleEntryDT: '',
            vehicleReadyDT: '',
            clearanceDT: '',
            vehicleNumber: '',
            quoteTypeID: 0,
            quoteType: '-Select-',
            paymentTerms: 0,
            fixedTerm: '',
            requestSentDate: null,
            fields: [],
            budget: '',
            serviceName: ''
        }
        $scope.ShowTranspoterDetails = false;
        $scope.selectedTranspoterName = [];
        $scope.newReqForQuote = true;
        $scope.showQuote = false;
        $scope.fieldList = [];
        $scope.isShowInputandBtn = false;
        $scope.newQuoteType = true;
        $scope.selectedReqIndex = -1;
        $scope.selectedPlanIndex = index;
        getExecQuoteType($scope.dispatchPlanList[index].serviceTypeID, true);

        $scope.reqForQuote.pickUpDate = new Date();
        $scope.reqForQuote.deliveryDate = new Date();
        $scope.reqForQuote.pickUpDT = new Date($scope.dispatchPlanList[index].pickup).customFormat("#DD#-#MMM#-#YY#");
        $scope.reqForQuote.deliveryDT = new Date($scope.dispatchPlanList[index].delivery).customFormat("#DD#-#MMM#-#YY#");

    }

    //customDateDdMmmYyyy(dateString) - Convert string date to appropriate date
    var customDateDdMmmYyyy = function (dateString) {
        var dateParts = dateString.split(/-/);
        return new Date((('20' + dateParts[2]) * 1), ($.inArray(dateParts[1].toUpperCase(), ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"]) * 1), dateParts[0] * 1 + 1);
    };

    //set pickUpDate(date fomat) according to on change of pickUpDT(string format)
    $scope.$watch("reqForQuote.pickUpDT", function (newValue, oldValue) {
        $scope.reqForQuote.pickUpDate = customDateDdMmmYyyy($scope.reqForQuote.pickUpDT);
        $scope.reqForQuote.pickUpDate.setDate($scope.reqForQuote.pickUpDate.getDate() - 1);
    });

    //set deliveryDate(date fomat) according to on change of deliveryDT(string format)
    $scope.$watch("reqForQuote.deliveryDT", function (newValue, oldValue) {
        $scope.reqForQuote.deliveryDate = customDateDdMmmYyyy($scope.reqForQuote.deliveryDT);
        $scope.reqForQuote.deliveryDate.setDate($scope.reqForQuote.deliveryDate.getDate() - 1);
    });

    // openCustContainer() - Open customer wise container list
    $scope.openCustContainer = function () {
        $state.go('customerContainers');
    }

    // startRFQ() - RFQ status update for start
    $scope.startRFQ = function (reqForQuoteID) {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            reqForQuoteID: reqForQuoteID
        }

        // Call api
        confirmSWALPopup(SweetAlert, "Are you sure you wish to proceed?", function () {
            $transporterQuote.callAPI5('/api/reqForQuote/rfq/start',
               myParams,
               function (apiResponse) {
                   $scope.reqForQuoteList[$scope.selectedReqIndex].status = '3';
                   $scope.reqForQuoteList[$scope.selectedReqIndex].statusStr = 'Started';

                   angular.forEach($scope.RFQCListopy, function (item, key) {
                       if ($scope.reqForQuoteList[$scope.selectedReqIndex].reqForQuoteID == item.reqForQuoteID) {
                           item.status = '3';
                           item.statusStr = 'Started';
                       }
                   })
                   $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (item) { if (item.status == '1') { return item } });
                   $scope.readyCount = $scope.reqForQuoteList.length;
                   $scope.startCount = $.grep($scope.RFQCListopy, function (i) { if (i.status == '3') { return i } }).length;

                   $scope.addNew();
                   toaster.success(apiResponse.message);
               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(errorStr + "20");
               });
        } ,
                 function () {
                     return;
                 })
    }

    // getRFQList() - Get start rfq list
    var getRFQList = function () {

        var myParams = {
            userID: $scope.pm_personInfo.userID
        }
        
        // Call api
        $transporterQuote.callAPI5('/api/reqForQuote/start/get',
           myParams,
           function (apiResponse) {
               if (apiResponse.data.length != 0) {
                   $scope.dispatchPlanList = apiResponse.data;
               }
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "21");
           });

    }

    // completeRFQ() - Complete rfq for status update
    $scope.completeRFQ = function (reqForQuoteID) {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            reqForQuoteID: reqForQuoteID
        }

        // Call api
        confirmSWALPopup(SweetAlert, "Are you sure you wish to proceed?", function () {
        $transporterQuote.callAPI5('/api/reqForQuote/rfq/complete',
           myParams,
           function (apiResponse) {
               $scope.reqForQuoteList[$scope.selectedReqIndex].status = '4';
               $scope.reqForQuoteList[$scope.selectedReqIndex].statusStr = 'Completed';

               angular.forEach($scope.RFQCListopy, function (item, key) {
                   if ($scope.reqForQuoteList[$scope.selectedReqIndex].reqForQuoteID == item.reqForQuoteID) {
                       item.status = '4';
                       item.statusStr = 'Completed';
                   }
               })

               $scope.reqForQuoteList = $.grep($scope.reqForQuoteList, function (item) { if (item.status == '3') { return item } });
               $scope.startCount = $scope.reqForQuoteList.length;
               $scope.dispatchedCount = $.grep($scope.RFQCListopy, function (i) { if (i.status == '4') { return i } }).length;

               $scope.addNew();
               toaster.success(apiResponse.message);
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "21");
           });
        },
                 function () {
                     return;
                 })
    }

    // getAcceptedComponents() - Get accepted components
    $scope.getAcceptedComponents = function (reqForQuoteID) {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            reqForQuoteID: reqForQuoteID,
            status: "1"
        }

        $transporterQuote.callAPI5('/api/reqForQuote/quoteComponent/get',
           myParams,
           function (apiResponse) {
               console.log(apiResponse.data.Components);
               $scope.componentsDetail = JSON.parse(apiResponse.data.Components);
               try {
                   var temp = JSON.parse(apiResponse.data.Rates);                   
                   for (var i = 0; i < $scope.componentsDetail.length; i++) {
                       $scope.componentsDetail[i].value = temp[i].actualCost;
                   }
                   temp = JSON.parse(apiResponse.data.RatingData);
                   $scope.rating.current = temp.rating;
                   fillStars($scope.rating.current);
                   $scope.rating.reason = temp.reason;
                   $scope.addOrShowFeedback = 'Feedback & Rates';
               } catch (ex) {
                   $scope.rating.current = 5;
                   $scope.rating.reason = "";
                   $scope.addOrShowFeedback = 'Add feedback & actual';
               }
                             
               $scope.openFeedAndRate();
              
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "22");
           });
    }
    
    // openFeedAndRate() - Feedback and Rates
    $scope.openFeedAndRate = function () {        
        feedBackAndRates = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popFeedbackAndActualRates.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
    }            

    //#region "for rating"
    //declare variables
    $scope.rating = { current: 5, max: 5, reason: "" };
    $scope.stars = [];    
    for (var i = 0; i < $scope.rating.max; i++)
        $scope.stars.push({});    

    //fill(index) - fill relevant colors according to number of stars
    $scope.fill = function (index) {
        var setColor = "OrangeRed";
        switch ($scope.rating.current) {
            case 5:
                setColor = "DarkGreen";
                break;
            case 4:
                setColor = "Green";
                break;
            case 3:
                setColor = "Orange";
                break;
            case 2:
                setColor = "DarkGoldenRod";
                break;
        }
        if (index < $scope.rating.current)
            return { "color": setColor };
    };

    //fillStars(trueLength) - decide how many stars to fill
    var fillStars = function (trueLength) {
        for (var i = 0; i < trueLength; i++) {
            $scope.stars[i].filled = true;
        }
        for (var i = trueLength; i < 5; i++) {
            $scope.stars[i].filled = false;
        }
    };

    //default fill 1 star
    fillStars(1);

    //toggle(index) - onclick of star
    $scope.toggle = function (index) {
        if ($scope.addOrShowFeedback != 'Feedback & Rates') {
            $scope.rating.current = index + 1;
            fillStars(index + 1);
        }
    };
    //#endregion

    // submitFeedAndRate() - Submit feedback and give actual rates
    $scope.submitFeedAndRate = function () {
        if ($scope.rating.reason == "") {
            toaster.info("Please enter a reason for above given rating.");
            return;
        }
        $scope.totalOfActualCost = 0;
        $scope.componentIDs = [];
        $scope.componentAcceptedCosts = [];
        $scope.componentActualCosts = [];
        $scope.RFQID = $scope.reqForQuoteList[$scope.selectedReqIndex].reqForQuoteID;

        $scope.componentsDetail.forEach(function (component) {
            $scope.componentIDs.push(component.componentID);
            $scope.componentAcceptedCosts.push(component.componentValue);
            if (!component.value)
                component.value = 0;
            else
                component.value = Number(component.value);
            $scope.componentActualCosts.push(component.value);
            $scope.totalOfActualCost += component.value;            
        });

        if ($scope.totalOfActualCost == 0) {
            toaster.info("Total of actual costs can't be zero! Please enter appropriate actual cost.");
            return;
        }

        var rates = [];
        for (var i = 0; i < $scope.componentIDs.length; i++) {
            var rate = {
                componentID: $scope.componentIDs[i],
                actualCost: $scope.componentActualCosts[i]
            }
            rates.push(rate);
        }

        var ratingData = {
            rating: $scope.rating.current,
            reason: $scope.rating.reason,
        }

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            rating: $scope.rating.current,
            reason: $scope.rating.reason,
            ratingData: JSON.stringify(ratingData),
            rates: JSON.stringify(rates),
            components: $scope.componentIDs.length,
            componentIDs: $scope.componentIDs,
            componentAcceptedCosts: $scope.componentAcceptedCosts,
            componentActualCosts: $scope.componentActualCosts,
            RFQID: $scope.RFQID
        };

        $transporterQuote.callAPI5('/api/reqForQuote/finalRates/set',
        myParams,
        function (apiResponse) {
            angular.forEach($scope.RFQCListopy, function (item, key) {
                if ($scope.reqForQuoteList[$scope.selectedReqIndex].reqForQuoteID == item.reqForQuoteID) {
                    console.log("key: " + key);
                    item.status = '5';
                    item.statusStr = 'Feedback';
                }
            })

            toaster.success(apiResponse.message);
            feedBackAndRates.close();

            $scope.reqForQuoteList.splice($scope.selectedReqIndex, 1);
            $scope.dispatchedCount--;
            $scope.feedbackCount++;

            //$scope.addNew();
        }, function (apiError) {
            toaster.error(apiError.message);
        }, function (apiFailure) {
            toaster.warning(errorStr + "23");
        });
    }


    // closeFeedAndRate() - Feedback and Rates
    $scope.closeFeedAndRate = function () {                
        feedBackAndRates.close();
    }

    // $scope.getTranspoterDetails(0);
    // #endregion

})
// #endregion

// #region "Logistics"
.controller("transportController", function ($scope, $state, $transporterQuote, toaster, $uibModal, SweetAlert, cfpLoadingBar, $rootScope) {

    // #region "Variables"

    var infoCookie = window.localStorage.getItem('pm_info');
    if (infoCookie == null) {
        $transporterQuote.bye();
    }
    Date.prototype.customFormat = function (formatString) {
        var YYYY, YY, MMMM, MMM, MM, M, DDDD, DDD, DD, D, hhhh, hhh, hh, h, mm, m, ss, s, ampm, AMPM, dMod, th;
        YY = ((YYYY = this.getFullYear()) + "").slice(-2);
        MM = (M = this.getMonth() + 1) < 10 ? ('0' + M) : M;
        MMM = (MMMM = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][M - 1]).substring(0, 3);
        DD = (D = this.getDate()) < 10 ? ('0' + D) : D;
        DDD = (DDDD = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"][this.getDay()]).substring(0, 3);
        th = (D >= 10 && D <= 20) ? 'th' : ((dMod = D % 10) == 1) ? 'st' : (dMod == 2) ? 'nd' : (dMod == 3) ? 'rd' : 'th';
        formatString = formatString.replace("#YYYY#", YYYY).replace("#YY#", YY).replace("#MMMM#", MMMM).replace("#MMM#", MMM).replace("#MM#", MM).replace("#M#", M).replace("#DDDD#", DDDD).replace("#DDD#", DDD).replace("#DD#", DD).replace("#D#", D).replace("#th#", th);
        h = (hhh = this.getHours());
        if (h == 0) h = 24;
        if (h > 12) h -= 12;
        hh = h < 10 ? ('0' + h) : h;
        hhhh = h < 10 ? ('0' + hhh) : hhh;
        AMPM = (ampm = hhh < 12 ? 'am' : 'pm').toUpperCase();
        mm = (m = this.getMinutes()) < 10 ? ('0' + m) : m;
        ss = (s = this.getSeconds()) < 10 ? ('0' + s) : s;
        return formatString.replace("#hhhh#", hhhh).replace("#hhh#", hhh).replace("#hh#", hh).replace("#h#", h).replace("#mm#", mm).replace("#m#", m).replace("#ss#", ss).replace("#s#", s).replace("#ampm#", ampm).replace("#AMPM#", AMPM);
    };

    // date 
    var currentDT = new Date();
    var yesterday = currentDT;
    yesterday.setDate(yesterday.getDate() - 1);
    $scope.todayDT = {
        value: yesterday
    };
        //currentDT.customFormat("#DD#-#MMM#-#YY#");

    $scope.logisticsList = [];
    $scope.rfqLogistics = {
        shipmentID: 0,
        reqForQuoteID: 0,
        quoteID: 0,
        //  quoteTypeID: 0,
        //  executiveID: 0,
        customerID: 0,
        transporterCompany: '',
        customerCompany: '',
        pickUpDT: '',
        deliveryDT: '',
        product: '',
        quoteBy: '',
        // transpoters: '',
        details: '',
        status: "0",
        sourceLocation: '',
        sourceAddress: '',
        destinationAddress: '',
        orderNo: '',
        // routeID: 0,
        // routeName: '',
        fileName: '',
        createdBy: '',
        dispatchedBy: '',
        timeSlotFromDT: '',
        timeSlotToDT: '',
        vehicleEntryDT: '',
        vehicleReadyDT: '',
        clearanceDT: '',
        vehicleNumber: '',
        quoteTypeID: 0,
        quoteType: '-Select-',
        paymentTerms: '',
        fixedTerm: '',
        components: [],
        vendorName: '',
        totalINR: 0,
        exchangeRate: 0,
        totalDoller: 0
    }
    $scope.rfqCountList = [];
    var index = -1;
    $scope.readyCount = 0;
    $scope.clearCount = 0;
    $scope.vehicalReadyCount = 0;
    $scope.timeSlotCount = 0;
    $scope.enterVehCount = 0;
    $scope.dispatchedCount = 0;
    $scope.vehicle = {
        number: ''
    }
    var setVehicleArrival = null;
    var setTimeSlot = null;
    var unit = new Date().getHours() >= 12 ? " PM" : " AM";
    var minutes = new Date().getMinutes() < 10 ? "0" + new Date().getMinutes() : "" + new Date().getMinutes();
    var date = new Date().getHours() == 12 ? new Date().getHours() + ":" + minutes + unit : new Date().getHours() % 12 + ":" + minutes + unit;
    console.log(currentDT.getHours());
    var dateTo = new Date().getHours() == 12 ? new Date().getHours() + ":" + minutes + unit : new Date(new Date().setHours(new Date().getHours() + 2)).getHours() % 12 + ":" + minutes + unit;
    // console.log(new Date().setHours(new Date().getHours() + 2));
    console.log(dateTo);
    $scope.timeSlot = {
        loadingDT: currentDT.customFormat("#DD#-#MMM#-#YY#"),
        fromTime: date,
        toTime: dateTo
    }

    $scope.configValues = [];
    $scope.companyName = "";
    $scope.addDispatchPlan = true;
    $scope.dispatchPlan = {
        customerID: 0,
        customerName: '-Select-',
        source: '',
        containers: 0,
        file: '',
        pickUpDT: '',
        deliveryDT: '',
        serviceIDs: '',
        portofLoading: '',
        portofDispatch: '',
        product: '',
        destination: '',
        emptyContainerPick: '',
        sourceAddress: '',
        containerNames: ''
    }
    $scope.serviceList = [];
    var fileFolderPath = "";
    $scope.dispatchPlanList = [];
    $scope.dispatchPlanDetail = {
        shipmentID: 0,
        customerName: '',
        container: 0,
        pickup: '',
        delivery: '',
        fileName: '',
        services: [],
        source: '',
        product: '',
        sourceAdress: '',
        destination: '',
        portofLoading: '',
        portofDischarge: '',
        emptyCntainerPick: ''
    }
    $scope.dispatchRFQList = [];
    $scope.readyforSchedule = false;
    $scope.headerHeight = '100';
    $scope.enablePlanList = 0;
    var popScheduleSlot = null;
    $scope.containerList = [];
    $scope.container = {
        containerID: 0,
        loadingDT: '',
        loadingTime: '',
        slotID: '',
        slotValue: '',
        notes: ''
    }
    $scope.slotList = [];
    $scope.containerIndex = -1;
    var copyContainer = [];        

    // #endregion

    // #region "Functions"

    // searchProduct() - Search product
    $scope.searchInList = function () {
        $scope.showSearch = !$scope.showSearch;
        setTimeout(function () { document.getElementById("search").focus() }, 1000);
        $scope.search = {
            value: ''
        }
    }

    // getCount() - Get count
    var getCount = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
        };
        // Call api
        $transporterQuote.callAPI5('/api/logistics/count/get',
            myParams,
            function (apiResponse) {
                $scope.pendingDispatchCount = apiResponse.data[0].RFQcount;
                $scope.readyDispatchCount = apiResponse.data[1].RFQcount;
                $scope.scheduleCount = apiResponse.data[2].RFQcount;
                $scope.deliveredCount = apiResponse.data[3].RFQcount;
            }, function (apiError) {
                console.error(apiError);
                //toaster.error(apiError.message);
            },
            function (apiFailure) {
                console.error(apiFailure);
                //toaster.warning(errorStr + "8");
            });
    }

    // Calling
    getCount();

    // openQuotes() - Open quotes
    $scope.openQuotes = function () {
        $state.go('reqForQuote');
    }

    // reset() - reset values
    var reset = function () {
        $scope.rfqLogistics = {
            shipmentID: 0,
            reqForQuoteID: 0,
            quoteID: 0,
            //  quoteTypeID: 0,
            //  executiveID: 0,
            customerID: 0,
            transporterCompany: '',
            customerCompany: '',
            pickUpDT: '',
            deliveryDT: '',
            product: '',
            quoteBy: '',
            // transpoters: '',
            details: '',
            status: "0",
            sourceLocation: '',
            sourceAddress: '',
            destinationAddress: '',
            orderNo: '',
            // routeID: 0,
            // routeName: '',
            fileName: '',
            createdBy: '',
            dispatchedBy: '',
            timeSlotFromDT: '',
            timeSlotToDT: '',
            vehicleEntryDT: '',
            vehicleReadyDT: '',
            clearanceDT: '',
            vehicleNumber: '',
            quoteTypeID: 0,
            quoteType: '-Select-',
            paymentTerms: '',
            fixedTerm: '',
            components: [],
            vendorName: ''
        }
    }

    // clearRFQ() - RFQ clear for loading
    $scope.clearRFQ = function (reqForQuoteID) {
        confirmSWALPopup(SweetAlert, "Are you sure you wish to proceed?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                reqForQuoteID: reqForQuoteID
            }
            // Call api
            $transporterQuote.callAPI5('/api/logistics/rfq/clear',
               myParams,
               function (apiResponse) {
                   // console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   $scope.logisticsList.splice($scope.selectedReqIndex, 1);
                   $scope.readyCount = $scope.readyCount - 1;
                   $scope.clearCount = $scope.clearCount + 1;

               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(errorStr + "3");
               });
        },
        function () {
            return;
        })
    }

    // openReadyPop() - Open vehicle arrival popup for vehical number entry
    $scope.openVehicleArrivalPop = function (index) {
        setVehicleArrival = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popAddVehicalReady.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
        $scope.containerIndex = index;
    }

    // closeVehicalModal() - Close vehicle modal
    $scope.closeVehicalModal = function () {
        $scope.vehicle = {
            number: ''
        }
        setVehicleArrival.close();
    }

    // openChangePasswordPop() - Open change password popup
    $scope.openChangePasswordPop = function () {
        changePasswordModal = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popChangePassword.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
    }

    // closeChangePassModal() - close Change password
    $scope.closeChangePassModal = function () {
        changePasswordModal.close();
        $scope.password = {
            oldPassword: '',
            newPassword: '',
            confirmPassword: ''
        }
    }

    // getConfigValue() - Get config value
    var getConfigValue = function () {
        var myParams = {}

        $transporterQuote.callAPI5(
            '/api/gen/config/get',
            myParams,
            function (apiResponse) {
                $scope.configValues = apiResponse.data;
                console.log($scope.configValues);

                angular.forEach($scope.configValues, function (item, key) {
                    if (String(item.Field).toLowerCase() == String("CompanyName").toLowerCase()) {
                        $scope.companyName = item.Value;
                    }
                })
            },
            function (apiError) {
                console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(apiFailure);
            }
        );
    }

    // Calling
    getConfigValue();

    // changePassword() - Change executive password
    $scope.changePassword = function () {
        if ($scope.password.oldPassword == '' || $scope.password.oldPassword == undefined) {
            toaster.info("Please enter your old password.");
            return;
        }
        if ($scope.password.newPassword == '' || $scope.password.newPassword == undefined) {
            toaster.info("Please enter your new password.");
            return;
        }

        if ($scope.password.confirmPassword == '' || $scope.password.confirmPassword == undefined) {
            toaster.info("Please enter your confirm password.");
            return;
        }

        if ($scope.password.newPassword != $scope.password.confirmPassword) {
            toaster.info("Passwords not match.");
            return;
        }

        confirmSWALPopup(SweetAlert, "Are you sure you wish to change password?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                oldPassword: $scope.password.oldPassword,
                newPassword: $scope.password.newPassword
            }
            // Call api
            $transporterQuote.callAPI5('/api/reqForQuote/password/change',
               myParams,
               function (apiResponse) {
                   //  console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   $scope.closeChangePassModal();
               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(apiFailure);
               })
        },
            function () {
                return;
            })
    }

    // addNewDispatch() - Add new dispatch plan
    $scope.addNewDispatch = function () {
        $scope.addDispatchPlan = true;
        $scope.dispatchPlan = {
            customerID: 0,
            customerName: '-Select-',
            source: '',
            containers: 0,
            file: '',
            pickUpDT: '',
            deliveryDT: '',
            serviceIDs: '',
            portofLoading: '',
            portofDispatch: '',
            product: '',
            destination: '',
            emptyContainerPick: '',
            sourceAddress: '',
            containerNames: ''
        }
        angular.forEach($scope.serviceList, function (item, key) {
            item.isChecked = false;
        })
    }

    $scope.closeNewDispatch = function () {
        $scope.addDispatchPlan = false;
    }

    // getService() - Get service
    var getService = function () {

        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        $transporterQuote.callAPI5('/api/admin/serviceType/get',
         myParams,
          function (apiResponse) {
              $scope.serviceList = apiResponse.data;
              angular.forEach($scope.serviceList, function (item, key) {
                  item.isChecked = false;
              })
              $scope.vendorsServices = angular.copy($scope.serviceList);
              console.log($scope.serviceList);
          }, function (apiError) {
              toaster.error(apiError.message);
              console.log(apiError.message);
          },
          function (apiFailure) {
              toaster.warning(errorStr + "8");
          })
    }

    // Calling
    getService();

    // uploadFile() - Upload file
    $scope.uploadFile = function (selectedFile, evt) {
        $scope.showCancel = false;

        if (selectedFile.size > 5242880) {
            toaster.info("Please select a smaller file.");
            $scope.cleanup();
            return;
        }
        if (selectedFile.type != "application/pdf" && selectedFile.type != "application/msword" && selectedFile.type != "application/vnd.ms-excel") {
            toaster.info("Please upload pdf, word or excel file.");
            $scope.cleanup();
            return;
        }
        cfpLoadingBar.start();
        var data = new FormData();
        data.append(selectedFile.name, selectedFile);
        $.ajax({
            url: getFilePath() + "/FileUploadHandler.ashx?path=" + fileFolderPath + "&ID=0" + "&typeID=4",
            type: 'POST',
            data: data,
            contentType: false,
            async: false,
            processData: false,
            success: function (jResponse) {
                console.log(jResponse);
                $scope.dispatchPlan.file = selectedFile.name;
                console.log($scope.dispatchPlan.file);
                $rootScope.filePath = getFilePath() + JSON.parse(jResponse).FileName;
                if (jResponse.status == 200 && jResponse.responseText != "-") {
                    okCB(jResponse.responseText);
                } else {
                    //errorOpenSWALPopup("Please upload zip file!");
                }
                cfpLoadingBar.complete();
                // setActivity();
                $scope.showCancel = true;
                //overCB();
            },
            error: function (jError) {
                console.log(jError);
                if (jError.status == 200 && jError.responseText != "-") {
                    okCB(jError.responseText);
                } else {
                    //errorOpenSWALPopup("Please upload zip file!");
                }
                cfpLoadingBar.complete();
                //overCB();
                return;
            },
        })
    }

    // cleanup() - Clean
    $scope.cleanup = function () {
        var m = document.getElementsByClassName("file-path validate");
        var i;
        for (i = 0; i < m.length; i++) {
            m[i].value = null;
        }
        $scope.dispatchPlan.file = "";
        $rootScope.filePath = "";
        $scope.showCancel = false;
    }

    // getCustomer() - Get all customer
    var getCustomer = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        }
        // Call api
        $transporterQuote.callAPI5('/api/admin/customers/get',
           myParams,
           function (apiResponse) {
               //console.log(apiResponse.data);
               $scope.customerList = apiResponse.data;
               if ($scope.customerList.length == 1) {
                   $scope.dispatchPlan.customerID = $scope.customerList[0].customerID;
                   $scope.dispatchPlan.customerName = $scope.customerList[0].companyName;
                   $scope.dispatchPlan.address = $scope.customerList[0].address1 + " " + $scope.customerList[0].address2;
               }
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "11");
           })
    }

    // Call getCustomer()
    getCustomer();

    // submitDispatchPlan() - Add new dispatch plan 
    $scope.submitDispatchPlan = function () {
        var ctrNames = [];
        if ($scope.dispatchPlan.customerID == 0) {
            toaster.info("Please select company name.");
            return;
        }

        if ($scope.dispatchPlan.source == "") {
            toaster.info("Please enter source.");
            return;
        }

        if ($scope.dispatchPlan.pickUpDT == "") {
            toaster.info("Please enter pickup date.");
            return;
        }

        if ($scope.dispatchPlan.deliveryDT == "") {
            toaster.info("Please enter delivery date.");
            return;
        }

        if ($scope.dispatchPlan.containers == 0) {
            toaster.info("Please enter containers.");
            return;
        }

        if ($scope.dispatchPlan.product == "") {
            toaster.info("Please enter product.");
            return;
        }

        if ($scope.dispatchPlan.destination == "") {
            toaster.info("Please enter destination.");
            return;
        }

        if ($scope.dispatchPlan.containerNames == "") {
            toaster.info("Please enter container name.");
            return;
        }

        if ($scope.dispatchPlan.containerNames != "") {
            var ctrNames = $scope.dispatchPlan.containerNames.split(",");
            if (ctrNames.length != Number($scope.dispatchPlan.containers)) {
                toaster.info("Please enter " + $scope.dispatchPlan.containers + " unique container name in comma-seperated formate.");
                return;
            }
            var copyContainer = [];
            for (i = 0; i < ctrNames.length; i++) {
                var value = ctrNames[i];
                if (copyContainer.indexOf(value) !== -1) {
                    toaster.info("Please enter " + $scope.dispatchPlan.containers + " unique container name in comma-seperated formate.");
                    return;
                }
                copyContainer.push(value);
            }
        }

        var selectedService = '~';
        angular.forEach($scope.serviceList, function (serviceItem, serviceKey) {
            if (serviceItem.isChecked == true) {
                selectedService += serviceItem.serviceTypeID + '~'
            }
        })

        if (selectedService == '~') {
            toaster.info('Please select a service.');
            return;
        }

        $scope.dispatchPlan.serviceIDs = selectedService;

        console.log($scope.dispatchPlan);

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            customerID: $scope.dispatchPlan.customerID,
            source: $scope.dispatchPlan.source,
            pickUpDT: $scope.dispatchPlan.pickUpDT,
            deliveryDT: $scope.dispatchPlan.deliveryDT,
            containers: $scope.dispatchPlan.containers,
            serviceIDs: $scope.dispatchPlan.serviceIDs,
            file: $scope.dispatchPlan.file == null ? '' : $scope.dispatchPlan.file,
            sourceAddress: $scope.dispatchPlan.sourceAddress,
            destination: $scope.dispatchPlan.destination,
            product: $scope.dispatchPlan.product,
            portofLoading: $scope.dispatchPlan.portofLoading,
            portofDispatch: $scope.dispatchPlan.portofDispatch,
            emptyContainerPick: $scope.dispatchPlan.emptyContainerPick,
            containersNames: ctrNames
        }

        // Call api
        $transporterQuote.callAPI5('/api/logistics/dispatchPlan/set',
           myParams,
           function (apiResponse) {
               toaster.success(apiResponse.message);
               $scope.getDispatchPlan(0);
               $scope.addNewDispatch();
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "6");
           });
    }

    // changeCustomer() - Change customer
    $scope.changeCustomer = function (index) {
        $scope.dispatchPlan.customerID = $scope.customerList[index].customerID;
        $scope.dispatchPlan.customerName = $scope.customerList[index].companyName;
        $scope.dispatchPlan.destination = $scope.customerList[index].address1 + " " + $scope.customerList[index].address2;
    }

    // getDispatchPlan() - Get dispatch plan 
    $scope.getDispatchPlan = function (status) {

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            status: status + ''
        }

        // Call api
        $transporterQuote.callAPI5('/api/logistics/dispatchPlan/get',
           myParams,
           function (apiResponse) {
               $scope.dispatchPlanList = [];
               $scope.enablePlanList = status;
               console.log("apiResponse");
               console.log(apiResponse.data);
               if (apiResponse.data.length != 0) {                   
                   $scope.dispatchPlanList = apiResponse.data;
                   $scope.dispatchPlanList.forEach(function (dispatchPlan) {
                       dispatchPlan.showShipmentString = 'Shipment ID: #' + dispatchPlan.shipmentID;
                   });
                   
                   $scope.getDetailofDispatch(0);
               }
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "4");
           });

        //Schedule button's name
        $scope.scheduleTitle = (status == 1) ? "Schedule" : "Change Schedule";        
    }

    // Calling
    $scope.getDispatchPlan(0);

    //customDateDdMmmYyyy(dateString) - Convert string date to appropriate date
    var customDateDdMmmYyyy = function (dateString) {
        var dateParts = dateString.split(/-/);
        return new Date((('20' + dateParts[2]) * 1), ($.inArray(dateParts[1].toUpperCase(), ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"]) * 1), dateParts[0] * 1 + 1);
    };

    //set pickUpDate(date fomat) according to on change of pickUpDT(string format)
    $scope.$watch("reqForQuote.pickUpDT", function (newValue, oldValue) {
        $scope.dispatchPlan.pickUpDate = customDateDdMmmYyyy($scope.dispatchPlan.pickUpDT);
        $scope.dispatchPlan.pickUpDate.setDate($scope.dispatchPlan.pickUpDate.getDate() - 1);
    });

    // getDetailofDispatch () - Detail of dispatch plan
    $scope.getDetailofDispatch = function (shipmentIndex) {

        $scope.addDispatchPlan = false;
        $scope.selectedPlanIndex = shipmentIndex;
        $scope.dispatchPlanDetail.shipmentID = $scope.dispatchPlanList[shipmentIndex].shipmentID;
        $scope.dispatchPlanDetail.customerName = $scope.dispatchPlanList[shipmentIndex].companyName;
        $scope.dispatchPlanDetail.container = $scope.dispatchPlanList[shipmentIndex].container;
        $scope.dispatchPlanDetail.pickup = $scope.dispatchPlanList[shipmentIndex].pickup;
        $scope.dispatchPlanDetail.delivery = $scope.dispatchPlanList[shipmentIndex].delivery;
        $scope.dispatchPlanDetail.fileName = $scope.dispatchPlanList[shipmentIndex].fileName;
        $scope.dispatchPlanDetail.product = $scope.dispatchPlanList[shipmentIndex].product;
        $scope.dispatchPlanDetail.sourceAddress = $scope.dispatchPlanList[shipmentIndex].sourceAddress;
        $scope.dispatchPlanDetail.destination = $scope.dispatchPlanList[shipmentIndex].destinationAddress;
        $scope.dispatchPlanDetail.portofLoading = $scope.dispatchPlanList[shipmentIndex].portofLoading;
        $scope.dispatchPlanDetail.portofDischarge = $scope.dispatchPlanList[shipmentIndex].portofDischarge;
        $scope.dispatchPlanDetail.source = $scope.dispatchPlanList[shipmentIndex].source;
        $scope.dispatchPlanDetail.status = $scope.dispatchPlanList[shipmentIndex].status;

        var serviceIDList = ($scope.dispatchPlanList[shipmentIndex].serviceIDs).substring(1, ($scope.dispatchPlanList[shipmentIndex].serviceIDs).length - 1);
        $scope.dispatchPlanDetail.services = serviceIDList.split("~")

        // Calling get RFQs
        getDispatchPlanRFQs($scope.dispatchPlanList[shipmentIndex].shipmentID);

        // Get container list
        getContainer();        
    }

    // getDispatchPlanRFQs() - Get dispatch plan rfq list
    var getDispatchPlanRFQs = function (shipmentID) {

        var myParams = {
            userID: $scope.pm_personInfo.userID,
            shipmentID: shipmentID
        }

        // Call api
        $transporterQuote.callAPI5('/api/logistics/dispatchPlanRFQs/get',
           myParams,
           function (apiResponse) {
               $scope.dispatchRFQList = apiResponse.data;
               var count = 0;
               $scope.readyforSchedule = false;
               if ($scope.dispatchPlanDetail.services.length == $scope.dispatchRFQList.length) {

                   angular.forEach($scope.dispatchRFQList, function (item, key) {
                       if (item.status == "1") {
                           count++;
                       }
                   })

                   if (count == $scope.dispatchRFQList.length) {
                       $scope.readyforSchedule = true;
                   }
               }

               console.log($scope.dispatchRFQList);
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "5");
           })
    }

    // readyForDispatch() - Ready for dispatch
    $scope.readyForDispatch = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            shipmentID: $scope.dispatchPlanDetail.shipmentID
        }
        // Call api
        $transporterQuote.callAPI5('/api/logistics/dispatch/ready',
           myParams,
           function (apiResponse) {
               $scope.dispatchPlanList.splice($scope.selectedPlanIndex, 1);
               toaster.success(apiResponse.message);
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "6");
           })
    }

    // openSlotandSchedule() - Open slot and schedule popup
    $scope.openSlotandSchedule = function () {
        
        //for (i = 0; i < $scope.dispatchPlanDetail.container; i++) {
        //    $scope.container = {
        //        containerID: i + 1,
        //        loadingDT: currentDT.customFormat("#DD#-#MMM#-#YY#"),
        //        loadingTime: date,
        //        slotID: '',
        //        slotValue: '',
        //        notes: ''
        //    }
        //    $scope.containersList.push($scope.container);
        //}

        popScheduleSlot = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popShowScheduleAndSlot.html',
            windowClass: 'showSlotSchedule',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
    }

    // openScheduleSlotPop() - Open schedule slot popup
    $scope.openScheduleSlotPop = function () {
        popScheduleSlot = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popScheduleSlot.html',
            windowClass: 'addSchedule',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
    }

    // closeSchedulePop() - Close schedule popup
    $scope.closeSchedulePop = function () {
        popScheduleSlot.close();
    }

    // getSlots() - Get slot list
    var getSlots = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            serviceTypeID: 1
        }
        // Call api
        $transporterQuote.callAPI5('/api/logistics/slots/get',
           myParams,
           function (apiResponse) {
               $scope.slotList = apiResponse.data;
               angular.forEach($scope.slotList, function (item, key) {
                   item.slotFrom = convertTwelveHours(item.slotFrom);
                   item.slotTo = convertTwelveHours(item.slotTo);
               })
               console.log($scope.slotList);
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "7");
           })
    }

    // Calling
    getSlots();

    // convertTimeFormat() - Convert to 24 hours time format
    function convertTimeFormat(str) {
        var time = str;
        var hours = Number(time.match(/^(\d+)/)[1]);
        var minutes = Number(time.match(/:(\d+)/)[1]);
        var AMPM = time.match(/\s(.*)$/)[1];
        if (AMPM == "PM" && hours < 12) hours = hours + 12;
        if (AMPM == "AM" && hours == 12) hours = hours - 12;
        var sHours = hours.toString();
        var sMinutes = minutes.toString();
        if (hours < 10) sHours = "0" + sHours;
        if (minutes < 10) sMinutes = "0" + sMinutes;
        return sHours + ":" + sMinutes;
    }

    // convertTwelveHours() - Convert to 12 hours time format
    function convertTwelveHours(time) {
        // Check correct time format and split into components
        time = time.toString().match(/^([01]\d|2[0-3])(:)([0-5]\d)(:[0-5]\d)?$/) || [time];

        if (time.length > 1) { // If time format correct
            time = time.slice(1);  // Remove full string match value
            time[5] = +time[0] < 12 ? ' AM' : ' PM'; // Set AM/PM
            time[0] = +time[0] % 12 || 12; // Adjust hours
        }
        return time.join(''); // return adjusted time or original string
    }

    // getContainer() - Get conatiner
    var getContainer = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            shipmentID: $scope.dispatchPlanDetail.shipmentID
        }
        // Call api
        $transporterQuote.callAPI5('/api/logistics/container/get',
           myParams,
           function (apiResponse) {
               $scope.containerList = apiResponse.data;
               angular.forEach($scope.containerList, function (item, key) {
                   if (item.scheduleDT != null) {
                       item.scheduleDT = new Date(item.scheduleDT).customFormat("#DD#-#MMM#-#YY#");
                       item.scheduleTime = convertTwelveHours((item.scheduleTime).slice(0, 5));
                   }
                   else {
                       // item.scheduleDT = currentDT.customFormat("#DD#-#MMM#-#YY#");
                       item.scheduleTime = date;
                   }
                   item.slotID = item.slotID + '';
               })
               console.log($scope.containerList);
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "8");
           })
    }

    // initTimePicker() - init timepicker
    $scope.initTimePicker = function () {
        $('.time-init').timepicker().on('hide.timepicker', function (e) {
            angular.forEach($scope.slotList, function (item, key) {
                if (Date.parse(new Date('01/01/2011 ' + convertTimeFormat(e.time.value))) > Date.parse(new Date('01/01/2011 ' + convertTimeFormat(item.slotFrom)))
                    && Date.parse(new Date('01/01/2011 ' + convertTimeFormat(e.time.value))) < Date.parse(new Date('01/01/2011 ' + convertTimeFormat(item.slotTo)))) {
                    $scope.containerList[$scope.containerIndex].slotID = item.slotID + '';
                    $scope.$apply();
                }
            })
        });
    }

    // setSchedule() - Submit schedule
    $scope.setSchedule = function () {

        if ($scope.containerList == null || $scope.containerList == [] || $scope.containerList == "") {
            return;
        }

        copyContainer = angular.copy($scope.containerList);
        angular.forEach(copyContainer, function (item, key) {
            item.scheduleTime = convertTimeFormat(item.scheduleTime);
        })


        var myParams = {
            userID: $scope.pm_personInfo.userID,
            containers: JSON.stringify(copyContainer),
            shipmentID: $scope.dispatchPlanDetail.shipmentID
        }

        $transporterQuote.callAPI5('/api/logistics/scheduleSlot/set',
           myParams,
           function (apiResponse) {
               if ($scope.dispatchPlanDetail.status == '1') {
                   $scope.dispatchPlanList.splice($scope.selectedPlanIndex, 1);
               }
               $scope.closeSchedulePop();
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "9");
           })
    }

    // changeTime() - Change time 
    $scope.changeTime = function (index) {
        $scope.containerIndex = index;
    }

    // changeSlot() - On change slot set time
    $scope.changeSlot = function (index) {
        angular.forEach($scope.slotList, function (item, key) {
            if (item.slotID == Number($scope.containerList[index].slotID)) {
                $scope.containerList[index].scheduleTime = item.slotFrom;
            }
        })
    }

    // setVehicalArrival() - Vehical arrival status update 
    $scope.setVehicleArrival = function () {                    

        if ($scope.vehicle.number == '') {
            toaster.info("Please enter vehicle number.");
            return;
        }

        confirmSWALPopup(SweetAlert, "Are you sure you wish to proceed?", function () {
            $scope.firstVehicleArrived = true;
            $scope.RFQID = 0;

            //check whether any vehicle arrived before
            $scope.containerList.forEach(function (container) {
                if (container.status != '0') {
                    $scope.firstVehicleArrived = false;
                    return;
                }
            });

            //if this is the first vehicle arrived
            if ($scope.firstVehicleArrived) {
                $scope.dispatchRFQList.forEach(function (rfq) {
                    if (rfq.serviceName.trim().toUpperCase() == 'TRANSPORT') {
                        $scope.RFQID = rfq.RFQID;
                    }
                });
            }

            var myParams = {
                userID: $scope.pm_personInfo.userID,
                containerID: $scope.containerList[$scope.containerIndex].containerID,
                vehicleNumber: $scope.vehicle.number,
                firstVehicleArrived: $scope.firstVehicleArrived,
                RFQID: $scope.RFQID
            }

            $transporterQuote.callAPI5('/api/logistics/vehicalArrival/set',
              myParams,
              function (apiResponse) {
                  $scope.containerList[$scope.containerIndex].status = '1'
                  toaster.success(apiResponse.message);
                  $scope.closeVehicalModal();
              }, function (apiError) {
                  toaster.error(apiError.message);
              },
              function (apiFailure) {
                  toaster.warning(errorStr + "10");
              })
        },
        function () {
            return;
        })
    }

    // setLoading() - Load start into container
    $scope.setLoading = function (index) {
        confirmSWALPopup(SweetAlert, "Are you sure you wish to proceed?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                containerID: $scope.containerList[index].containerID
            }

            $transporterQuote.callAPI5('/api/logistics/load/set',
              myParams,
              function (apiResponse) {
                  $scope.containerList[index].status = '2';
                  toaster.success(apiResponse.message);
              }, function (apiError) {
                  toaster.error(apiError.message);
              },
              function (apiFailure) {
                  toaster.warning(errorStr + "11");
              })
        },
    function () {
        return;
    })
    }

    // setDispatch() - Dispatch container status update
    $scope.setDispatch = function (index) {
        confirmSWALPopup(SweetAlert, "Are you sure you wish to proceed?", function () {
            $scope.allVehicleDispatched = true;
            $scope.RFQID = 0;

            $scope.containerList.forEach(function (container, i) {
                if (container.status < '3' && index != i) {
                    $scope.allVehicleDispatched = false;
                    return;
                }
            });

            if ($scope.allVehicleDispatched) {
                $scope.dispatchRFQList.forEach(function (rfq) {
                    if (rfq.serviceName.trim().toUpperCase() == 'TRANSPORT') {
                        $scope.RFQID = rfq.RFQID;
                    }
                });
            }

            var myParams = {
                userID: $scope.pm_personInfo.userID,
                containerID: $scope.containerList[index].containerID,
                allVehicleDispatched: $scope.allVehicleDispatched,
                RFQID: $scope.RFQID
            }

            $transporterQuote.callAPI5('/api/logistics/dispatch/set',
              myParams,
              function (apiResponse) {
                  $scope.containerList[index].status = '3';
                  toaster.success(apiResponse.message);
              }, function (apiError) {
                  toaster.error(apiError.message);
              },
              function (apiFailure) {
                  toaster.warning(errorStr + "12");
              })
        },
        function () {
            return;
        })
    }

    // openCustContainer() - Open customer wise container list
    $scope.openCustContainer = function () {
        $state.go('customerContainers');
    }
    
    // setDelivery() - Set Delivery
    $scope.setDeilvery = function (index) {
        confirmSWALPopup(SweetAlert, "Are you sure you wish to proceed?", function () {
            $scope.allVehicleDelivered = true;
            $scope.shipmentID = $scope.dispatchPlanDetail.shipmentID;

            $scope.containerList.forEach(function (container, i) {
                if (container.status < '4' && index != i) {
                    $scope.allVehicleDelivered = false;
                    return;
                }
            });                        

            var myParams = {
                userID: $scope.pm_personInfo.userID,
                containerID: $scope.containerList[index].containerID,
                allVehicleDelivered: $scope.allVehicleDelivered,
                shipmentID: $scope.shipmentID
            }

            $transporterQuote.callAPI5('/api/logistics/delivery/set',
              myParams,
              function (apiResponse) {
                  //if ($scope.allVehicleDelivered) {
                  //    $scope.dispatchPlanList.splice(index, 1);
                  //    $scope.scheduleCount--;
                  //    $scope.deliveredCount++;
                  //}
                  $scope.containerList[index].status = '4';
                  toaster.success(apiResponse.message);
              }, function (apiError) {
                  toaster.error(apiError.message);
              },
              function (apiFailure) {
                  toaster.warning(errorStr + "13");
              })
        },
       function () {
           return;
       })
    }

    // #endregion

})
// #endregion

// #region "Customer Container"
.controller("custContainersController", function ($scope, $state, $transporterQuote, toaster, $uibModal, SweetAlert, cfpLoadingBar, $rootScope, ngTableParams, $filter) {    

    // #region "Variables"

    $scope.customerContainers = [];
    var setVehicleArrival = null;
    $scope.vehicle = {
        number: ''
    }
    $scope.containerIndex = -1;
    $scope.customerContainerList = [];
    var currentDT = new Date();   
   // $scope.todayDT = currentDT.customFormat("#DD#-#MMM#-#YY#");

    // #endregion

    // #region "Functions"

    // getCustContainers() - Get customer container list
    function getCustContainers() {

        var myParams = {
            userID: $scope.pm_personInfo.userID
        }

        // Api call
        $transporterQuote.callAPI5('/api/logistics/custWiseContainers/get',
          myParams,
          function (apiResponse) {
              $scope.customerContainers = apiResponse.data;

              angular.forEach($scope.customerContainers, function (item, key) {
                  item.scheduleTime = convertTwelveHours((item.scheduleTime).slice(0, 5));
                  item.dateColor = new Date(item.scheduleDT) < currentDT.setHours(0, 0, 0, 0) ? 'red' : 'black';
                //  item.timeColor = new Date(item.scheduleTime) < currentDT ? 'red' : 'black';
              })

              showContainerList();
             // console.log($scope.customerContainers);
             
          }, function (apiError) {
              toaster.error(apiError.message);
          },
          function (apiFailure) {
              toaster.warning(errorStr + "1");
          })
    }

    //angular.element(document).ready(function () {
    //    alert();
    //    $('#example').DataTable().destroy();
    //    $('#example').DataTable({
    //        "paging": true
    //    });
    //});

    // Calling 
    getCustContainers();

    // convertTwelveHours() - Convert to 12 hours time format
    function convertTwelveHours(time) {
        // Check correct time format and split into components
        time = time.toString().match(/^([01]\d|2[0-3])(:)([0-5]\d)(:[0-5]\d)?$/) || [time];

        if (time.length > 1) { // If time format correct
            time = time.slice(1);  // Remove full string match value
            time[5] = +time[0] < 12 ? ' AM' : ' PM'; // Set AM/PM
            time[0] = +time[0] % 12 || 12; // Adjust hours
        }
        return time.join(''); // return adjusted time or original string
    }

    // openReadyPop() - Open vehicle arrival popup for vehical number entry
    $scope.openVehicleArrivalPop = function (index) {
        setVehicleArrival = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popAddVehicalReady.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
        $scope.containerIndex = index;
    }

    // closeVehicalModal() - Close vehicle modal
    $scope.closeVehicalModal = function () {
        $scope.vehicle = {
            number: ''
        }
        setVehicleArrival.close();
    }

    // setVehicalArrival() - Vehical arrival status update 
    $scope.setVehicleArrival = function () {

        if ($scope.vehicle.number == '') {
            toaster.info("Please enter vehicle number.");
            return;
        }

        confirmSWALPopup(SweetAlert, "Are you sure you wish to proceed?", function () {
            $scope.firstVehicleArrived = true;
            $scope.RFQID = 0;

            //check whether any vehicle arrived before
            $scope.customerContainerList.forEach(function (container) {
                if (container.status != '0') {
                    $scope.firstVehicleArrived = false;
                    return;
                }
            });

            //if this is the first vehicle arrived
            if ($scope.firstVehicleArrived) {
                $scope.dispatchRFQList.forEach(function (rfq) {
                    if (rfq.serviceName.trim().toUpperCase() == 'TRANSPORT') {
                        $scope.RFQID = rfq.RFQID;                        
                    }
                });
            }

            var myParams = {
                userID: $scope.pm_personInfo.userID,
                containerID: $scope.customerContainerList[$scope.containerIndex].containerID,
                vehicleNumber: $scope.vehicle.number,
                firstVehicleArrived: $scope.firstVehicleArrived,
                RFQID: $scope.RFQID
            }

            $transporterQuote.callAPI5('/api/logistics/vehicalArrival/set',
              myParams,
              function (apiResponse) {
                  $scope.customerContainerList[$scope.containerIndex].status = '1'
                  toaster.success(apiResponse.message);
                  $scope.closeVehicalModal();
              }, function (apiError) {
                  toaster.error(apiError.message);
              },
              function (apiFailure) {
                  toaster.warning(errorStr + "10");
              })
        },
        function () {
            return;
        })
    }

    // setLoading() - Load start into container
    $scope.setLoading = function (index) {
        confirmSWALPopup(SweetAlert, "Are you sure you wish to proceed?", function () {
            var myParams = {
                userID: $scope.pm_personInfo.userID,
                containerID: $scope.customerContainerList[index].containerID
            }

            $transporterQuote.callAPI5('/api/logistics/load/set',
              myParams,
              function (apiResponse) {
                  $scope.customerContainerList[index].status = '2';
                  toaster.success(apiResponse.message);
              }, function (apiError) {
                  toaster.error(apiError.message);
              },
              function (apiFailure) {
                  toaster.warning(errorStr + "11");
              })
        },
    function () {
        return;
    })
    }

    // setDispatch() - Dispatch container status update
    $scope.setDispatch = function (index) {
        confirmSWALPopup(SweetAlert, "Are you sure you wish to proceed?", function () {
            $scope.allVehicleDispatched = true;
            $scope.RFQID = 0;

            $scope.customerContainerList.forEach(function (container, i) {
                if (container.status < '3' && index != i) {
                    $scope.allVehicleDispatched = false;
                    return;
                }
            });
            if ($scope.allVehicleDispatched) {
                $scope.dispatchRFQList.forEach(function (rfq) {
                    if (rfq.serviceName.trim().toUpperCase() == 'TRANSPORT') {
                        $scope.RFQID = rfq.RFQID;
                    }
                });
            }

            var myParams = {
                userID: $scope.pm_personInfo.userID,
                containerID: $scope.customerContainerList[index].containerID,
                allVehicleDispatched: $scope.allVehicleDispatched,
                RFQID: $scope.RFQID
            }

            $transporterQuote.callAPI5('/api/logistics/dispatch/set',
              myParams,
              function (apiResponse) {
                  $scope.customerContainerList[index].status = '3';
                  toaster.success(apiResponse.message);
              }, function (apiError) {
                  toaster.error(apiError.message);
              },
              function (apiFailure) {
                  toaster.warning(errorStr + "12");
              })
        },
        function () {
            return;
        })
    }

    // showPages() - Display pages in table.
    var showContainerList = function () {
        $scope.custWiseContainerTable = new ngTableParams({
            page: 1,
            count: 10,
        }, {
            total: $scope.customerContainers.length,
            getData: function ($defer, params) {
                $scope.customerContainerList = params.sorting() ? $filter('orderBy')($scope.customerContainers, params.orderBy()) : $scope.customerContainers;
                $scope.customerContainerList = $scope.customerContainerList.slice((params.page() - 1) * params.count(), params.page() * params.count());
                $defer.resolve($scope.customerContainerList);
            }
        });
    }

    // openQuotes() - Open quotes
    $scope.openQuotes = function () {
        $state.go('reqForQuote');
    }

    // openLogistics() - Open logistics
    $scope.openLogistics = function () {        
        $state.go('transportActivity');
    }



    // #endregion

})
// #endregion

// #region "Schedule"
.controller("scheduleController", function ($scope, $state, $transporterQuote, toaster, $uibModal, SweetAlert, cfpLoadingBar, $rootScope) {

    // #region "Variables"

    var infoCookie = window.localStorage.getItem('pm_info');
    if (infoCookie == null) {
        $transporterQuote.bye();
    }
    Date.prototype.customFormat = function (formatString) {
        var YYYY, YY, MMMM, MMM, MM, M, DDDD, DDD, DD, D, hhhh, hhh, hh, h, mm, m, ss, s, ampm, AMPM, dMod, th;
        YY = ((YYYY = this.getFullYear()) + "").slice(-2);
        MM = (M = this.getMonth() + 1) < 10 ? ('0' + M) : M;
        MMM = (MMMM = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][M - 1]).substring(0, 3);
        DD = (D = this.getDate()) < 10 ? ('0' + D) : D;
        DDD = (DDDD = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"][this.getDay()]).substring(0, 3);
        th = (D >= 10 && D <= 20) ? 'th' : ((dMod = D % 10) == 1) ? 'st' : (dMod == 2) ? 'nd' : (dMod == 3) ? 'rd' : 'th';
        formatString = formatString.replace("#YYYY#", YYYY).replace("#YY#", YY).replace("#MMMM#", MMMM).replace("#MMM#", MMM).replace("#MM#", MM).replace("#M#", M).replace("#DDDD#", DDDD).replace("#DDD#", DDD).replace("#DD#", DD).replace("#D#", D).replace("#th#", th);
        h = (hhh = this.getHours());
        if (h == 0) h = 24;
        if (h > 12) h -= 12;
        hh = h < 10 ? ('0' + h) : h;
        hhhh = h < 10 ? ('0' + hhh) : hhh;
        AMPM = (ampm = hhh < 12 ? 'am' : 'pm').toUpperCase();
        mm = (m = this.getMinutes()) < 10 ? ('0' + m) : m;
        ss = (s = this.getSeconds()) < 10 ? ('0' + s) : s;
        return formatString.replace("#hhhh#", hhhh).replace("#hhh#", hhh).replace("#hh#", hh).replace("#h#", h).replace("#mm#", mm).replace("#m#", m).replace("#ss#", ss).replace("#s#", s).replace("#ampm#", ampm).replace("#AMPM#", AMPM);
    };
    var currentDT = new Date();
    $scope.todayDT = currentDT.customFormat("#DD#-#MMM#-#YY#");
    $scope.scheduleList = [];
    $scope.schedule = {
        date: new Date().customFormat("#DD#-#MMM#-#YY#")
    }
    $scope.scheduleHeader = [];
    $scope.scheduleData = [];
    var popAllocateDetail = null;

    // #endregion

    // #region "Functions"

    // getScheduleSlot() - Get schedule slot date wise
    $scope.getScheduleSlot = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            serviceTypeID: 1
        }
        // Call api
        $transporterQuote.callAPI5('/api/logistics/schedule/get',
           myParams,
           function (apiResponse) {
               console.log(JSON.parse(apiResponse.data));
               var result = JSON.parse(apiResponse.data);
               $scope.scheduleHeader = result.Table;
               $scope.scheduleData = result.Table1;

           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "1");
           })
    }

    // Calling
    $scope.getScheduleSlot();

    // closeScheduleModal() - Close modal
    $scope.closeScheduleModal = function () {
        $scope.showShcedule.close();
    }

    // openSlotAllocateDetail() - Slot allocate detail
    $scope.openSlotAllocateDetail = function (index, date) {
        var year = currentDT.getFullYear();
        var scheduleDT = date + '-' + year;
        console.log(scheduleDT);
        var myParams = {
            userID: $scope.pm_personInfo.userID,
            slotID: $scope.scheduleData[index].SlotID,
            scheduleDate: scheduleDT
        }
        // Call api
        $transporterQuote.callAPI5('/api/logistics/allocateSlot/get',
           myParams,
           function (apiResponse) {
               $scope.allocateSlotList = apiResponse.data;
               angular.forEach($scope.allocateSlotList, function (item, key) {
                   item.scheduleTime = convertTwelveHours((item.scheduleTime).slice(0, 5));
               })
               popAllocateDetail = $uibModal.open({
                   animation: true,
                   templateUrl: 'Html/Popups/popShowSlotAllocateDetail.html',
                   windowClass: 'addScheduleDetail',
                   size: '1g',
                   backdrop: 'static',
                   keyboard: false,
                   scope: $scope
               });
               console.log($scope.allocateSlotList);
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "8");
           })
    }

    // closeAllocateDetailPop() - close detail popup
    $scope.closeAllocateDetailPop = function () {
        popAllocateDetail.close();
    }

    // convertTwelveHours() - Convert to 12 hours time format
    function convertTwelveHours(time) {
        // Check correct time format and split into components
        time = time.toString().match(/^([01]\d|2[0-3])(:)([0-5]\d)(:[0-5]\d)?$/) || [time];

        if (time.length > 1) { // If time format correct
            time = time.slice(1);  // Remove full string match value
            time[5] = +time[0] < 12 ? ' AM' : ' PM'; // Set AM/PM
            time[0] = +time[0] % 12 || 12; // Adjust hours
        }
        return time.join(''); // return adjusted time or original string
    }

    // #endregion

})
// #endregion

// #region "Report"
.controller("reportController", function ($scope, $state, $transporterQuote, toaster, $uibModal, SweetAlert, cfpLoadingBar, $rootScope) {

    // #region "Variables"

    var infoCookie = window.localStorage.getItem('pm_info');
    if (infoCookie == null) {
        $transporterQuote.bye();
    }
    Date.prototype.customFormat = function (formatString) {
        var YYYY, YY, MMMM, MMM, MM, M, DDDD, DDD, DD, D, hhhh, hhh, hh, h, mm, m, ss, s, ampm, AMPM, dMod, th;
        YY = ((YYYY = this.getFullYear()) + "").slice(-2);
        MM = (M = this.getMonth() + 1) < 10 ? ('0' + M) : M;
        MMM = (MMMM = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][M - 1]).substring(0, 3);
        DD = (D = this.getDate()) < 10 ? ('0' + D) : D;
        DDD = (DDDD = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"][this.getDay()]).substring(0, 3);
        th = (D >= 10 && D <= 20) ? 'th' : ((dMod = D % 10) == 1) ? 'st' : (dMod == 2) ? 'nd' : (dMod == 3) ? 'rd' : 'th';
        formatString = formatString.replace("#YYYY#", YYYY).replace("#YY#", YY).replace("#MMMM#", MMMM).replace("#MMM#", MMM).replace("#MM#", MM).replace("#M#", M).replace("#DDDD#", DDDD).replace("#DDD#", DDD).replace("#DD#", DD).replace("#D#", D).replace("#th#", th);
        h = (hhh = this.getHours());
        if (h == 0) h = 24;
        if (h > 12) h -= 12;
        hh = h < 10 ? ('0' + h) : h;
        hhhh = h < 10 ? ('0' + hhh) : hhh;
        AMPM = (ampm = hhh < 12 ? 'am' : 'pm').toUpperCase();
        mm = (m = this.getMinutes()) < 10 ? ('0' + m) : m;
        ss = (s = this.getSeconds()) < 10 ? ('0' + s) : s;
        return formatString.replace("#hhhh#", hhhh).replace("#hhh#", hhh).replace("#hh#", hh).replace("#h#", h).replace("#mm#", mm).replace("#m#", m).replace("#ss#", ss).replace("#s#", s).replace("#ampm#", ampm).replace("#AMPM#", AMPM);
    };

    $scope.currentWorkLoad = [];

    // #endregion

    // #region "Functions"

    // backToRFQ() - Back to previous page
    $scope.backToRFQ = function () {
        $state.go('reqForQuote');
    }

    // getSchedule() - Get schedule list.
    var getReport = function () {
        var myParams = {
            userID: $scope.pm_personInfo.userID
        }
        // Call api
        $transporterQuote.callAPI5('/api/reqForQuote/report/get',
           myParams,
           function (apiResponse) {
               // console.log(apiResponse.data);
               $scope.currentWorkLoad = apiResponse.data.currentWorkLoadList;
           }, function (apiError) {
               toaster.error(apiError.message);
           },
           function (apiFailure) {
               toaster.warning(errorStr + "1");
           });
    }

    // Calling
    getReport();

    // #endregion

})
// #endregion



