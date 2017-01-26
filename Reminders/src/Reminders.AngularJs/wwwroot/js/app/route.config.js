(function () {
    'use strict';

    angular
        .module('reminderApp')
            .config(['$routeProvider', '$locationProvider',
        function ($routeProvider, $locationProvider) {
            
            $routeProvider.
                when('/', {
                    templateUrl: 'js/app/controllers/reminder/index.html',
                    controller: 'indexController as vm'
                }).
                when('/Create', {
                    templateUrl: 'js/app/controllers/reminder/create.html',
                    controller: 'createController as vm'
                }).
                when('/Details', {
                    templateUrl: 'js/app/controllers/reminder/details.html',
                    controller: 'detailsController as vm'
                }).
                when('/Edit', {
                    templateUrl: 'js/app/controllers/reminder/edit.html',
                    controller: 'editController as vm'
                }).
                otherwise({ redirectTo: '/' });

            $locationProvider.html5Mode({
                enabled: true,
                requireBase: false
            });
        }]);
})();