(function () {
    'use strict';

    angular.module('mdQuery', []);
    angular.module('mdCommand', []);

    angular.module('reminderApp', ['ngRoute', 'mdQuery', 'mdCommand']);

})();