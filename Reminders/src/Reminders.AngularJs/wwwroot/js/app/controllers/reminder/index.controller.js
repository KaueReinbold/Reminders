(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('indexController', indexController);

    indexController.$inject = ['$location', 'query', 'command', 'reminderrepository', 'util'];

    function indexController($location, query, command, reminderrepository, util) {

        var vm = this;

        vm.title = "indexController"
        
        vm.reminders = [];
        vm.doneReminder = doneReminder;
        vm.deleteReminder = deleteReminder;
        vm.editReminder = editReminder;
        vm.detailsReminder = detailsReminder;

        function success(data) {
            vm.reminders = util.findAndConvertDateObject(data);
        }

        function error(data) {
            console.log(data);
        }
        
        function activate() {
            query.getReminders(success, error);
        }

        function doneReminder(reminder) {

        }

        function deleteReminder(reminder) {

        }

        function editReminder(reminder) {
            reminderrepository.setReminder(reminder);
            $location.path("Edit");
        }

        function detailsReminder(reminder) {
            reminderrepository.setReminder(reminder);
            $location.path("Details");
        }

        activate();
    }
})();
