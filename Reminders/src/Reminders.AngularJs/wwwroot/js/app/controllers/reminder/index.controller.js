(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('indexController', indexController);

    indexController.$inject = ['$location', 'query', 'command', 'reminderrepository'];

    function indexController($location, query, command, reminderrepository) {

        var vm = this;

        vm.title = "indexController"
        
        vm.reminders = [];
        vm.doneReminder = doneReminder;
        vm.deleteReminder = deleteReminder;
        vm.editReminder = editReminder;
        vm.detailsReminder = detailsReminder;

        function success(data) {
            vm.reminders = data;
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

        }

        function detailsReminder(reminder) {
            reminderrepository.setReminder(reminder);
            $location.path("Details");
        }

        activate();
    }
})();
