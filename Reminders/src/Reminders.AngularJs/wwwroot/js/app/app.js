/// <reference path="index.html" />
/// <reference path="index.html" />
/// <reference path="index.html" />
(function () {
    'use strict';

    var app = angular.module('reminderApp', ['ngRoute']);

    app.config(function ($routeProvider, $locationProvider) {

        $locationProvider.html5Mode(false);

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
    });
})();