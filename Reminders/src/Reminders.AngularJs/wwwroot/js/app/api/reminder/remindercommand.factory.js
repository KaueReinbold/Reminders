(function () {
    'use strict';

    angular
        .module('mdCommand')
        .factory('remindercommand', remindercommand);

    remindercommand.$inject = ['$http'];

    function remindercommand($http) {
        
        function postRequest(data, successCallback, errorCallback) {
            server.postRequest("Reminder", data, successCallback, errorCallback);
        }

        function putRequest(data, successCallback, errorCallback) {
            server.putRequest("Reminder", data, successCallback, errorCallback);
        }

        function deleteRequest(data, successCallback, errorCallback) {
            server.deleteRequest("Reminder", data, successCallback, errorCallback);
        }

        return {
            postRequest: postRequest,
            putRequest: putRequest,
            deleteRequest: deleteRequest
        };
    }
})();