(function () {
    'use strict';

    angular
        .module('reminderApp')
        .factory('reminderrepository', reminderrepository);

    function reminderrepository() {

        var _reminder = {};

        function setReminder(reminder) {
            _reminder = reminder
        }

        function getReminder(reminder) {
            return _reminder;
        }

        return {
            setReminder: setReminder,
            getReminder: getReminder
        }
    }
})();