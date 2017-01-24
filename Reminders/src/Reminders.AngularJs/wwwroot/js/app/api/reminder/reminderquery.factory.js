(function () {
    'use strict';

    angular
        .module('reminderApp')
        .factory('reminderquery', reminderquery);

    reminderquery.$inject = ['server'];

    function reminderquery(server) {
        
        function getRequest(data, successCallback, errorCallback) {
            server.getRequest("", data, successCallback, errorCallback);
        }

        function postRequest(data, successCallback, errorCallback) {
            server.postRequest("", data, successCallback, errorCallback);
        }

        function putRequest(data, successCallback, errorCallback) {
            server.putRequest("", data, successCallback, errorCallback);
        }

        function deleteRequest(data, successCallback, errorCallback) {
            server.deleteRequest("", data, successCallback, errorCallback);
        }

        return {
            getRequest: getRequest,
            postRequest: postRequest,
            putRequest: putRequest,
            deleteRequest: deleteRequest
        };
    }
})();