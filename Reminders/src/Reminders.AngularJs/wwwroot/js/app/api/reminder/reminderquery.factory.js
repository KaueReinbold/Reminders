(function () {
    'use strict';

    angular
        .module('mdQuery')
        .factory('reminderquery', reminderquery);

    reminderquery.$inject = ['server'];

    function reminderquery(server) {
        
        function getRequest(data, successCallback, errorCallback) {
            server.getRequest("Reminder", data, successCallback, errorCallback);
        }
                

        return {
            getRequest: getRequest
        };
    }
})();