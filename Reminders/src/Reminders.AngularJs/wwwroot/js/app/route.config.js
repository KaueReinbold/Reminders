(function () {
    'use strict';

    angular
        .module('reminderApp')
            .config(['$routeProvider', '$locationProvider',
        function ($routeProvider, $locationProvider) {
            
            $routeProvider.
                when('/', {
                    templateUrl: 'js/app/controllers/reminder/index.html',
                    controller: 'indexController'
                }).
                when('/Create', {
                    templateUrl: 'js/app/controllers/reminder/create.html',
                    controller: 'createController'
                }).
                when('/Details', {
                    templateUrl: 'js/app/controllers/reminder/details.html',
                    controller: 'detailsController'
                }).
                when('/Edit', {
                    templateUrl: 'js/app/controllers/reminder/edit.html',
                    controller: 'editController'
                }).
                otherwise({ redirectTo: '/' });

            $locationProvider.html5Mode({ enabled: false });
        }]);
})();