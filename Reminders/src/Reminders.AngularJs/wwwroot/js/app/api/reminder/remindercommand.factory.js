(function () {
    'use strict';

    angular
        .module('mdCommand')
        .factory('remindercommand', remindercommand);

    remindercommand.$inject = ['server'];

    function remindercommand(server) {
        
        function postReminder(data, successCallback, errorCallback) {
            server.postRequest("Reminder", data, successCallback, errorCallback);
        }

        function putReminder(data, successCallback, errorCallback) {
            server.putRequest("Reminder", data, successCallback, errorCallback);
        }

        function deleteReminder(data, successCallback, errorCallback) {
            server.deleteRequest("Reminder/" + data, null, successCallback, errorCallback);
        }

        return {
            postReminder: postReminder,
            putReminder: putReminder,
            deleteReminder: deleteReminder
        };
    }
})();