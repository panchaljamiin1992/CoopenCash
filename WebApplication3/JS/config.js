function isObjectEmpty(obj) {
    var isEmpty = Object.keys(obj).length === 0 && obj.constructor === Object;
    return isEmpty;
}

transporterQuote

/*
    * .run()
    * Handle common AJAX stuff
    */
.run(function (cfpLoadingBar, $transporterQuote, $rootScope) {

    ///* Show loading spinner whenever an AJAX API request starts. */
    //$(document).ajaxStart(function () {
    //    cfpLoadingBar.start();
    //});

    ///* Hide loading spinner whenever an AJAX API request is completed. */
    //$(document).ajaxComplete(function () {
    //    cfpLoadingBar.complete();
    //});

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
            window.scrollTo(0, 0);
        }
    );

})

// Config provider
.config(function ($stateProvider, $urlRouterProvider) {

    $stateProvider

     // Login
    .state('login', {
        url: '/login',
        templateUrl: 'Html/Pages/login.html',
        controller: 'loginController'
    })
    

    // admin
    .state('admin', {
        url: '/admin',
        templateUrl: 'Html/Pages/admin.html',
        controller: 'adminController'
    })

     // Dashboard
    .state('dashboard', {
        url: '/dashboard',
        templateUrl: 'Html/Pages/DashBoard.html',
        controller: 'dashboardController'
    })

    //// transport activity
    //.state('transportActivity', {
    //    url: '/transportActivity',
    //    templateUrl: 'Html/Pages/transportActivity.html',
    //    controller: 'transportController'
    //})

    
    //// customer container
    //.state('customerContainers', {
    //    url: '/customerContainers',
    //    templateUrl: 'Html/Pages/customerContainers.html',
    //    controller: 'custContainersController'
    //})

    //// report - Not in use
    //.state('report', {
    //    url: '/report',
    //    templateUrl: 'Html/Pages/reportForRFQ.html',
    //    controller: 'reportController'
    //})
   
    var defaultState = '/login';
    var loginCookie = window.localStorage.getItem('pm_info');
    if (loginCookie != null && loginCookie != "") {
        var loginCookieVal = JSON.parse(loginCookie);
        if (!isObjectEmpty(loginCookie) && loginCookie.tokenID != "") {
            defaultState = loginCookieVal ? '/adminPanel' : '/login'
            //: loginCookieVal.isCompanyAdmin
            //    ? '/profile/' + loginCookieVal.companyID
            //    : '/inquiries';
        }
    }

    $urlRouterProvider.otherwise(defaultState);

});