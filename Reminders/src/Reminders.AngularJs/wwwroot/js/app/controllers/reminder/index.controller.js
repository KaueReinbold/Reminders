(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('indexController', indexController);

    indexController.$inject = ['$scope', '$location', 'query', 'command', 'repository', 'util'];

    function indexController($scope, $location, query, command, repository, util) {

        var vm = this;

        vm.title = "indexController"
        
        vm.message = {};
        vm.reminder = {};
        vm.reminders = [];
        vm.doneReminder = doneReminder;
        vm.deleteReminder = deleteReminder;
        vm.editReminder = editReminder;
        vm.detailsReminder = detailsReminder;
        vm.showModalDelete = showModalDelete;

        function success(data) {
            vm.reminders = util.findAndConvertDateObject(data);
        }

        function error(data) {
            console.log(data);
        }

        function successDelete(data) {
            $scope.$parent.mc.showMessage(data);
            activate();
        }
        
        function activate() {
            query.getReminders(success, error);
        }

        function doneReminder(reminder) {

        }

        function deleteReminder(reminder) {
            command.deleteReminder(reminder.id, successDelete, error);
        }

        function editReminder(reminder) {
            repository.setReminder(reminder);
            $location.path("Edit");
        }

        function detailsReminder(reminder) {
            repository.setReminder(reminder);
            $location.path("Details");
        }

        function showModalDelete(event, reminder) {
            vm.reminder = reminder;
            event.preventDefault();
            event.stopPropagation();
            $(modalDelete).modal();
        }

        activate();
    }
})();
