(function () {
    'use strict';

    angular.module('mdConsulta', []);
    angular.module('mdComando', []);

    angular.module('reminderApp', ['ngRoute', 'mdComando', 'mdConsulta']);
})();