var transporterQuote = angular.module('transporterQuote',
                                            ['oitozero.ngSweetAlert',
                                                'cfp.loadingBar',
                                                'ui.bootstrap',
                                                'ngFileUpload',
                                                'toaster',
                                                '720kb.datepicker'])

transporterQuote

  /*
     * .run()
     * Handle common AJAX stuff
  */
.run(function (cfpLoadingBar, $transporterQuote, $rootScope) {

     /* Show loading spinner whenever an AJAX API request starts. */
     $(document).ajaxStart(function () {
         cfpLoadingBar.start();
     });

     /* Hide loading spinner whenever an AJAX API request is completed. */
     $(document).ajaxComplete(function () {
         cfpLoadingBar.complete();
     });

     /* Show loading spinner whenever a state change starts. */
     $rootScope.$on('$stateChangeStart',
         function (event, toState, toParams, fromState, fromParams) {
             cfpLoadingBar.start();
         }
     );

     /* Hide loading spinner whenever a state change ends. */
     $rootScope.$on('$stateChangeSuccess',
         function (event, toState, toParams, fromState, fromParams) {
             cfpLoadingBar.complete();
         }
     );

 })

.factory('$transporterQuote', extFactory)

// Contoller
.controller('transportersCtrl', function ($scope, $transporterQuote, SweetAlert, $timeout, $rootScope, toaster, $uibModal) {
    
    // #region "Variables"

    var getHref =decodeURIComponent(window.location.href);
    var link = "";
    if (getHref.indexOf("?q=") > -1) {
        link = getHref.split('=')[1];
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
    $scope.pm = {
        productName: "Transporter Quote",
        isMobileBrowser: /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent),
    }
   
    $scope.quotation = {
        charges: '',
        pickupDT: '',
        ExtendDeliveryDT: '',
        paymentTerms: '',
        notes: '',
        exchangeRate: 0,
    }
    $scope.isMobile = false;
    $scope.externalParam = [];
    $scope.showHistory = false;
    $scope.showSearch = false;
    $scope.showMsg = false;
    $scope.search = {
        value: ''
    }
    $scope.selectedReqIndex = -1;
    $scope.completedRFQ = false;
    $scope.rfqObj = {};
    $scope.quoteIssued = false;
    $scope.quoteObj = {};
    var setVehicleReady = null;
    $scope.vehicle = {
        number: ''
    }
    $scope.isExpired = false;
    $scope.compObj = {
        componentID: 0,
        componentName: '',
        componentValue: '',
        paidBy: '',
        currency: ''
    }
    $scope.componentList = [];
    $scope.exchangeRateShow = false;
    $scope.componentTotal = 0;

    // #endregion

    // #region "Functions"
   
    // getDetail() - Get detail of request. -- Not in use
    $scope.getDetail = function (index) {
        $scope.showMsg = false;
        $scope.quotation = {
            charges: '',
            pickupDT: '',
            ExtendDeliveryDT: '',
            paymentTerms: '',
            notes: '',
            exchangeRate: 0
        }
        $scope.selectedReqIndex = index;
        $scope.isMobile = $scope.pm.isMobileBrowser ? true : false;
        //$scope.isMobile = true;
        $scope.quotation.charges = $scope.RFQList[index].Charges;
        $scope.quotation.paymentTerms = $scope.RFQList[index].PaymentTerms;
        $scope.quotation.notes = $scope.RFQList[index].Notes;
        $scope.quotation.pickupDT = new Date($scope.RFQList[index].PickUp).customFormat("#D#-#MMM#-#YY#");
        $scope.quotation.ExtendDeliveryDT = new Date($scope.RFQList[index].ExtendDeliveryDT).customFormat("#D#-#MMM#-#YY#");
        $scope.maxDeliveryDT = $scope.quotation.ExtendDeliveryDT;

        if ($scope.RFQList[index].AllowLaterDelivery) {
            $scope.maxDeliveryDT = new Date($scope.quotation.ExtendDeliveryDT).addDays(7).customFormat("#D#-#MMM#-#YY#");
        }
    }
    // $scope.getDetail(0);

    // showList() - Show list -- Not in use
    $scope.showList = function () {
        $scope.isMobile = false;
    }

    // verifyGetReq() - Verify and get list of 
    $scope.verifyGetReq = function () 
    {
        var myParams = {
            link: link
        }

        $transporterQuote.callAPI5('/api/transporter/verify/transporterURL',
            myParams,
            function (apiResponse) {
               // $scope.RFQList = [];
               // $scope.RFQList = apiResponse.data.RFQList;
                $scope.externalParam = apiResponse.data.parameter;
                $scope.rfqObj = apiResponse.data.RFQobj;
                $scope.rfqObj.components = JSON.parse($scope.rfqObj.components);
                $scope.rfqObj.fields = JSON.parse($scope.rfqObj.fields);
                $scope.quoteObj = apiResponse.data.quoteObj;
                $scope.quoteIssued = apiResponse.data.quoteAns;
                $scope.componentTotal = 0;

                // Check length of component 
                if ($scope.rfqObj.components.length > 0) {
                    //if (!$scope.quoteIssued) {
                    angular.forEach($scope.rfqObj.components, function (item, key) {
                        if (!$scope.quoteIssued) {
                            item.ComponentValue = "";
                            if (item.Currency == "USD") {
                                $scope.exchangeRateShow = true;
                            }
                        } else {
                            $scope.componentList = JSON.parse($scope.quoteObj.Components);
                            angular.forEach($scope.componentList, function (subItem, subKey) {
                                if (item.ComponentID == subItem.componentID && item.Currency == "INR") {
                                    item.ComponentValue = subItem.componentValue;
                                    if (item.Type == "Charge") {
                                        $scope.componentTotal = $scope.componentTotal + Number(item.ComponentValue)
                                    }
                                    
                                } else if (item.ComponentID == subItem.componentID && item.Currency == "USD") {
                                    item.ComponentValue = subItem.componentValue;
                                    if (item.Type == "Charge") {
                                        $scope.componentTotal = $scope.componentTotal + (Number(item.ComponentValue) * Number($scope.quoteObj.ExchangeRate))
                                    }
                                }
                                if (subItem.currency == "USD") {
                                    $scope.exchangeRateShow = true;
                                }
                            })
                        }
                    })
                   // }
                }
              
               
             
                $scope.isMobile = $scope.pm.isMobileBrowser ? true : false;
                $scope.quotation = {
                    charges: '',
                    pickupDT: '',
                    ExtendDeliveryDT: '',
                    paymentTerms: '',
                    notes: '',
                    exchangeRate: 0
                }

               // $scope.quotation.pickupDT = new Date($scope.rfqObj.pickUpDT).customFormat("#D#-#MMM#-#YY#");
                $scope.quotation.ExtendDeliveryDT = new Date($scope.rfqObj.deliveryDT).customFormat("#D#-#MMM#-#YY#");
               

                if ($scope.quoteObj != null) {
                    //$scope.quotation.charges = $scope.quoteObj.Charges;
                    $scope.quotation.exchangeRate = $scope.quoteObj.ExchangeRate;
                    $scope.quotation.paymentTerms = $scope.quoteObj.PaymentTerms;
                    $scope.quotation.notes = $scope.quoteObj.Notes;
                }
               // $scope.maxDeliveryDT = $scope.quotation.ExtendDeliveryDT;
                if ($scope.rfqObj.paymentTerm == 1) {
                    $scope.quotation.paymentTerms = $scope.rfqObj.fixedTerm;
                }

                if (new Date($scope.todayDT) > new Date($scope.rfqObj.quoteByDT)) {
                    $scope.isExpired = true;
                }

                if (($scope.rfqObj.status == '1' || $scope.rfqObj.status == '3'))
                {
                    if ($scope.quoteObj != null) {
                        if($scope.quoteObj.Status == '1' || $scope.quoteIssued) {
                            $scope.completedRFQ = true;
                            return;
                        }
                    } else {
                        $scope.completedRFQ = true;
                        return;
                    }
                   
                }


                console.log($scope.rfqObj);
                //for (var i = 0; i < $scope.RFQList.length; i++) {
                //    $scope.RFQList[i].QuoteBy = new Date($scope.RFQList[i].QuoteBy).customFormat("#DD#-#MMM#-#YY#");
                //    $scope.RFQList[i].statusStr = ($scope.RFQList[i].Status == '1') ? 'Accepted' : new Date($scope.todayDT) > new Date($scope.RFQList[i].QuoteBy) ? 'Expired' : $scope.RFQList[i].CloseEarly ? 'Closed' : ''
                //}

               // RFQ.QuoteStatus == "2" ? 'Rejected' : RFQ.QuoteBy < todayDT ? 'Expired' : showHistory ? (RFQ.CloseEarly ? 'Closed' : '') :
                //console.log($scope.RFQList);
                //if (!$scope.pm.isMobileBrowser) {
                //    $scope.getDetail(0);
                //}
            }, function (apiError) {
                toaster.error(apiError.message);
                console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(errorStr + "1");
            })
    }

    // changeCharges() - Change charge after submit charges
    $scope.changeCharges = function (compID, compCharge) {
        angular.forEach($scope.componentList, function (subItem, subKey) {
            if (compID == subItem.componentID) {
                if (compCharge > subItem.componentValue) {
                    compCharge = subItem.componentValue;
                    toaster.info("You can reduce quote or keep it same. you can not increase the rate!");
                    return;
                }
            }
        })
    }

    // Calling function
    $scope.verifyGetReq();

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
                    if (String(item.Field).toLowerCase() == String("MinExchangeRate").toLowerCase()) {
                        $scope.minExchangeRate = item.Value;
                    }

                    if (String(item.Field).toLowerCase() == String("MaxExhangeRate").toLowerCase()) {
                        $scope.maxExchangeRate = item.Value;
                    }

                })
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

    // Calling
    getConfigValue();

    // getHistory() - Get history of RFQ -- Not in use
    $scope.getHistory = function () {

        var myParams = {
            transporterID: $scope.externalParam[0]
        }

        $transporterQuote.callAPI5('/api/transporter/rfqHistory/get',
            myParams, function (apiResponse) {
                $scope.RFQList = [];
                $scope.RFQList = apiResponse.data.RFQList;
                $scope.showHistory = true;

                for (var i = 0; i < $scope.RFQList.length; i++) {
                    $scope.RFQList[i].QuoteBy = new Date($scope.RFQList[i].QuoteBy).customFormat("#DD#-#MMM#-#YY#");
                    $scope.RFQList[i].statusStr = ($scope.RFQList[i].Status == '1') ? 'Accepted' : new Date($scope.todayDT) > new Date($scope.RFQList[i].QuoteBy) ? 'Expired' : $scope.RFQList[i].CloseEarly ? 'Closed' : ''
                }
                console.log($scope.RFQList);
                if (!$scope.pm.isMobileBrowser) {
                    $scope.getDetail(0);
                }
            }, function (apiError) {
                toaster.error(apiError.message);
                console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(errorStr + "3");
            })
    }

    // setQuote() - Add quotation for request
    $scope.setQuote = function () {

        var isReturn = false;

        if (new Date($scope.maxDeliveryDT) < new Date($scope.quotation.ExtendDeliveryDT) || new Date($scope.quotation.pickupDT) > new Date($scope.quotation.ExtendDeliveryDT)) {
            toaster.info("Please select valid delivery date between " + $scope.quotation.pickupDT + " and " + $scope.maxDeliveryDT + ".");
            return;
        }

        if ($scope.quotation.paymentTerms == "" || $scope.quotation.paymentTerms == null) {
            toaster.info("Please enter payment terms.");
            return;
        }

        angular.forEach($scope.rfqObj.components, function (item, key) {
            angular.forEach($scope.componentList, function (subItem, subKey) {
                if (item.ComponentID == subItem.componentID) {
                    if (item.ComponentValue > subItem.componentValue) {
                        toaster.info("You can reduce quote or keep it same. you can not increase the rate!");
                        isReturn = true;
                        return;
                    }
                }
            })
        })

        if (isReturn) {
            return;
        }

        if ($scope.rfqObj.components.length > 0) {
            $scope.componentList = [];
            angular.forEach($scope.rfqObj.components, function (item, key) {
                if (item.ComponentValue == "") {
                    toaster.info("Please enter " + item.ComponentName + ".");
                    isReturn = true;
                    return;
                } else {
                    $scope.compObj = {
                        componentID: item.ComponentID,
                        componentName: item.ComponentName,
                        componentType: item.Type,
                        componentValue: item.ComponentValue,
                        paidBy: item.Paidby,
                        currency: item.Currency
                    }
                    $scope.componentList.push($scope.compObj);
                }
            })
        }

        if (isReturn) {
            return;
        }

        if ($scope.exchangeRateShow && (Number($scope.quotation.exchangeRate) < $scope.minExchangeRate || Number($scope.quotation.exchangeRate) > $scope.maxExchangeRate)) {
            toaster.info("Please enter exchange rate between 50 and 80.");
            return;
        }

        console.log($scope.componentList);

        confirmSWALPopup(SweetAlert, "Are you sure you wish to submit this quote?", function () {
            var myParams = {
                transporterID: $scope.externalParam[0],
                RFQID: $scope.rfqObj.reqForQuoteID,
                customerID: $scope.rfqObj.customerID == null ? 0 : $scope.rfqObj.customerID,
                charges: $scope.quotation.charges,
                paymentTerms: $scope.quotation.paymentTerms,
                notes: $scope.quotation.notes,
                //deliveryDT: new Date($scope.RFQList[$scope.selectedReqIndex].DeliveryBy).customFormat("#YYYY#-#MM#-#D#") == new Date($scope.quotation.deliveryDT).customFormat("#YYYY#-#MM#-#D#") ? '' : $scope.quotation.deliveryDT
                deliveryDT: $scope.quotation.ExtendDeliveryDT,
                components: JSON.stringify($scope.componentList),
                quoteID: $scope.quoteObj == null ? 0 : $scope.quoteObj.QuoteID,
                exchangeRate: $scope.quotation.exchangeRate
            }

            // Call Api
            $transporterQuote.callAPI5('/api/transporter/quote/set',
                myParams, function (apiResponse) {
                    toaster.success(apiResponse.message);
                    $scope.verifyGetReq();
                }, function (apiError) {
                    toaster.error(apiError.message);
                    console.log(apiError.message);
                },
                function (apiFailure) {
                    toaster.warning(errorStr + "2");
                })
        },
           function () {
               return;
           })
    }

    // showSearch() - Show search
    $scope.searchInList = function () {
        $scope.showSearch = !$scope.showSearch;
        $scope.search = {
            value: ''
        }
    }

    // openReadyPop() - Open vehicle ready popup for vehical number entry.
    $scope.openReadyPop = function () {
        setVehicleReady = $uibModal.open({
            animation: true,
            templateUrl: 'Html/Popups/popAddVehicalReady.html',
            windowClass: 'AddUser',
            size: '1g',
            backdrop: 'static',
            keyboard: false,
            scope: $scope
        });
    }

    // closeVehicalModal() - Close vehicle modal
    $scope.closeVehicalModal = function () {
        $scope.vehicle.number = "";
        setVehicleReady.close();
    }

    // readyRFQ() - vehical ready
    $scope.readyRFQ = function () {

        if ($scope.vehicle.number == "") {
            toaster.info("Please enter a vehicle number.");
            return;
        }

        confirmSWALPopup(SweetAlert, "Are you sure your vehicle is ready?", function () {
            var myParams = {
                userID: $scope.externalParam[0],
                reqForQuoteID: $scope.rfqObj.reqForQuoteID,
                vehicleNumber: $scope.vehicle.number
            }

            // Call api
            $transporterQuote.callAPI5('/api/transporter/req/readyVeh',
               myParams,
               function (apiResponse) {
                   // console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   $scope.vehicle.number = "";
                   $scope.closeVehicalModal();
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

    // countComponentValue() - Count for component value
    $scope.countComponentValue = function () {
        $scope.componentTotal = 0;
        angular.forEach($scope.rfqObj.components, function (item, key) {
            if (item.ComponentValue != "" && item.Currency == "INR" && item.Type == "Charge") {
               $scope.componentTotal = $scope.componentTotal + Number(item.ComponentValue)
            } else if (item.ComponentValue != "" && item.Currency == "USD" && item.Type == "Charge") {
                $scope.componentTotal = $scope.componentTotal + (Number(item.ComponentValue) * Number($scope.quotation.exchangeRate))
            }
         
        })
    }

    // #endregion

});