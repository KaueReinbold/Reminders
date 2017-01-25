(function () {
    'use strict';

    angular
        .module('mdQuery')
        .factory('reminderquery', reminderquery);

    reminderquery.$inject = ['server'];

    function reminderquery(server) {
        
        function getReminders(successCallback, errorCallback) {
            server.getRequest("Reminder", null, successCallback, errorCallback);
        }
        
        function getReminder(data, successCallback, errorCallback) {
            server.getRequest("Reminder/" + data.id, null, successCallback, errorCallback);
        }

        return {
            getReminders: getReminders,
            getReminder: getReminder
        };
    }
})();