var customerQuote = angular.module('customerQuote',
                                            ['oitozero.ngSweetAlert',
                                                'cfp.loadingBar',
                                                'ui.bootstrap',
                                                'ngFileUpload',
                                                'toaster',
                                                '720kb.datepicker'])

customerQuote

 /*
     * .run()
     * Handle common AJAX stuff
     */
.run(function (cfpLoadingBar, $customerQuote, $rootScope) {

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

.factory('$customerQuote', extFactory)

// Fiter for convert into INR
.filter('INR', function () {
    return function (input) {
        if (!isNaN(input)) {
            //var output = Number(input).toLocaleString('en-IN');   <-- This method is not working fine in all browsers!           
            var result = input.toString().split('.');

            var lastThree = result[0].substring(result[0].length - 3);
            var otherNumbers = result[0].substring(0, result[0].length - 3);
            if (otherNumbers != '')
                lastThree = ',' + lastThree;
            var output = otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",") + lastThree;

            if (result.length > 1) {
                output += "." + result[1].substring(0, 2);
            } else {
                // output += ".00";
            }

            return output;
        }
    }
})


// Controller
.controller('customerCtrl', function ($scope, $customerQuote, $timeout, $rootScope, toaster, SweetAlert, $uibModal) {

    // #region "Variables"

    // Array
    $scope.rfqDetail = [];

    // Bit
    $scope.showCloseReq = false;
    $scope.showAcceptedQuotr = false;

    // Date
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
    $scope.todayDT = currentDT.customFormat("#DD# #MMM#, #YY#");

    $scope.isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);

    var getHref = window.location.href;
    var link = "";
    if (getHref.indexOf("?q=") > -1) {
        link = getHref.split('=')[1];
    }
    $scope.isCustomer = 0;
    $scope.showVertical = true;
    $scope.viewName = 'View Horizontal Comparison'
    $scope.quoteVerticalDetail = [];
    var verticalQuoteDetail = {};
    var quoteIndex = -1;
    $scope.quote = {
        acceptNote: ''
    }
    // #endregion

    // #region "functions"

    // reqCloseEarly() - Request close early
    $scope.reqCloseEarly = function (reqForQuoteID) {
        confirmSWALPopup(SweetAlert, "Are you sure you want to cancel this request?", function () {
            var myParams = {
                customerID: $scope.rfqDetail.customerID,
                reqForQuoteID: $scope.rfqDetail.RFQID,
            }
            // Call api
            $customerQuote.callAPI5('/api/customer/req/closeEarly',
               myParams,
               function (apiResponse) {
                   //console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   // $scope.reqForQuoteList[$scope.selectedReqIndex].isCloseEarly = true;
                   $scope.showCloseReq = true;
                   getRFQwithQuote();
                   // $state.reload();
               }, function (apiError) {
                   toaster.error(apiError.message);
               },
               function (apiFailure) {
                   toaster.warning(errorStr + "1");
               })
        },
            function () {
                return;
            })
       
    }

    // acceptQuote() - Accept quote.
    $scope.acceptQuote = function () {

        confirmSWALPopup(SweetAlert, "Are you sure wish to accept this quote?", function () {
            var myParams = {
                customerID: $scope.rfqDetail.customerID == null ? 0 : $scope.rfqDetail.customerID,
                reqForQuoteID: $scope.rfqDetail.RFQID,
                quoteID: $scope.rfqDetail.quoteDetail[quoteIndex].quoteID,
                isCustomer: $scope.isCustomer,
                note: $scope.quote.acceptNote
            }
            // Call api
            $customerQuote.callAPI5('/api/customer/quote/accept',
               myParams,
               function (apiResponse) {
                   console.log(apiResponse.data);
                   toaster.success(apiResponse.message);
                   if ($scope.isCustomer == 0) {
                       window.opener.location.reload();
                       window.close();
                       return;
                   }

                   if (apiResponse.data) {
                       //$scope.reqForQuoteList[$scope.selectedReqIndex].status = "1";
                       //$scope.showAcceptedQuotr = true;
                       getRFQwithQuote();
                   }
                   $scope.closeNoteModal();
                   $state.reload();
                 
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

    // extendQuoteBy() - Extend quoteby date.
    $scope.extendQuoteBy = function (reqRequestInfo) {

        confirmSWALPopup(SweetAlert, "Are you sure wish to extend quote by date?", function () {
            var myParams = {
                userID: $scope.rfqDetail.customerID,
                reqForQuoteID: $scope.rfqDetail.RFQID,
                quoteByDT: reqRequestInfo.quoteBy
            }
            // Call api
            $customerQuote.callAPI5('/api/customer/extend/QuoteBy',
               myParams,
               function (apiResponse) {
                   //console.log(apiResponse.data);
                   toaster.success(apiResponse.message);

                  
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

    // crddbtRupees() - Convert to comma seperate
    function crddbtRupees(amount) {
        var Amt = Number(String(amount).replace(/\,/g, ""));
        var SetAMt = Number(Amt).toLocaleString('hi-IN', {
            //style: "currency",  // It's uncomment when display in Currancy Symbol
            currency: "INR"
        });
        return SetAMt;
    }

    // getRFQwithQuote() - Get request with quatation detail
    var getRFQwithQuote = function () {
        var myParams = {
            link: link
        }

        $customerQuote.callAPI5('/api/customer/get',
            myParams,
            function (apiResponse) {
                apiResponse.data.requestDetail['quoteBy'] = new Date(apiResponse.data.requestDetail['quoteBy']).customFormat("#DD#-#MMM#-#YY#");
                apiResponse.data.requestDetail['deliveryBy'] = new Date(apiResponse.data.requestDetail['deliveryBy']).customFormat("#DD#-#MMM#-#YY#");
               // apiResponse.data.requestDetail['pickUp'] = new Date(apiResponse.data.requestDetail['pickUp']).customFormat("#DD# #MMM#, #YY#");
                $scope.rfqDetail = apiResponse.data;
                angular.forEach($scope.rfqDetail.quoteDetail, function (item, key) {
                    item.components = JSON.parse(item.components);
                    var totalRupee = 0;
                    var totalDollar = 0;
                    verticalQuoteDetail = {};
                    verticalQuoteDetail.Vendor = item.companyName;
                    if (item.exchangeRate > 0) {
                        verticalQuoteDetail.ExchangeRate = item.exchangeRate + ' /USD';
                    }
                   
                    angular.forEach(item.components, function (subItem, subKey) {
                        
                        if (subItem.componentType == "Charge" && subItem.currency == "INR") {
                            totalRupee = totalRupee + Number(subItem.componentValue);
                        } else if (subItem.componentType == "Charge" && subItem.currency == "USD") {
                          //  subItem.componentValue = (subItem.componentValue * item.exchangeRate);
                            totalRupee = totalRupee + (Number(subItem.componentValue) * item.exchangeRate);
                            totalDollar = totalDollar + Number(subItem.componentValue)
                        }

                        var dollar = subItem.currency == 'USD' ? '($)' : ''
                        if (subItem.componentType != "Text") {
                             verticalQuoteDetail[subItem.componentName] = crddbtRupees(subItem.componentValue) + dollar;
                        } else {
                            verticalQuoteDetail[subItem.componentName] =subItem.componentValue;
                        }
                       
                        //obj[subItem.componentName] = subItem.componentValue
                    })

                    item.totalRupee = totalRupee;
                    verticalQuoteDetail.TotalINR = crddbtRupees(totalRupee);
                    item.totalDollar = totalDollar;
                    if (totalDollar > 0) {
                        verticalQuoteDetail.TotalDollar = crddbtRupees(totalDollar);
                    }
                  
                    verticalQuoteDetail.Term = item.paymentTerms;
                    verticalQuoteDetail.Note = item.notes;
                    verticalQuoteDetail.Receivedon = new Date(item.createdDT).customFormat("#DD#-#MMM#-#YY#");
                    verticalQuoteDetail.Email = item.email;
                    verticalQuoteDetail.Mobile = item.mobile;
                    verticalQuoteDetail.Status = item.status;
                    if (($scope.rfqDetail.requestDetail.status == '0' || $scope.rfqDetail.requestDetail.status == '2')
                                                && item.status == '0') {
                        verticalQuoteDetail.Accept = true;
                    }
                    $scope.quoteVerticalDetail.push(verticalQuoteDetail);
                    console.log($scope.quoteVerticalDetail);
                })

                $scope.isCustomer = $scope.rfqDetail.isCustomer;
                console.log($scope.rfqDetail);
                // $scope.rfqDetail.requestDetail['quoteBy'] = new Date['quoteBy'].customFormat("#DD# #MMM#, #YY#");
                $scope.externalParam = apiResponse.data.parameter;
                //console.log($scope.rfqDetail);

               

            }, function (apiError) {
                toaster.error(apiError.message);
                //console.log(apiError.message);
            },
            function (apiFailure) {
                toaster.warning(errorStr + "4");
            })
    }

    // Calling function
    getRFQwithQuote();

    // changeView() - Chnage view 
    $scope.changeView = function () {
        $scope.showVertical = !$scope.showVertical;

        if ($scope.showVertical) {
            $scope.viewName = 'View Horizontal Comparison'
        } else {
            $scope.viewName = 'View Vertical Comparison'
        }
    }

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

       quoteIndex = index;
    }

    // closeNoteModal() - Close add note pop
    $scope.closeNoteModal = function () {
        addNotePopup.close();
    }

    // #endregion
});