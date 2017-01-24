(function () {
    'use strict';

    angular
        .module('reminderApp')
        .factory('reminder', reminder);

    reminder.$inject = ['$http'];

    function reminder($http) {
        var service = {
            getData: getData
        };

        return service;

        function getData() { }
    }
})();