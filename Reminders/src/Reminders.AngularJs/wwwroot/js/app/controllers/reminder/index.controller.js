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
        vm.showModalDone = showModalDone;
        vm.filterTable = filterTable;
        vm.orderBy = orderBy;
        vm.asc = false;
        vm.order = 'title';

        function success(data) {
            vm.reminders = util.findAndConvertDateObject(data);
            vm.remindersSource = vm.reminders;
        }

        function error(data) {
            $(modal).modal('toggle');
            $scope.$parent.mc.showMessage(data);
        }

        function successDelete(data) {
            $scope.$parent.mc.showMessage(data);
            $(modal).modal('toggle');
            activate();
        }

        function activate() {
            query.getReminders(success, error);
        }

        function doneReminder(reminder) {
            command.putReminder(vm.reminderDone, successDelete, error);
        }

        function deleteReminder() {
            command.deleteReminder(vm.reminderDelete.id, successDelete, error);
        }

        function editReminder(reminder) {
            repository.setReminder(reminder);
            $location.path("Edit");
        }

        function detailsReminder(reminder) {
            repository.setReminder(reminder);
            $location.path("Details");
        }

        function showModalDone(reminder) {
            event.preventDefault();
            event.stopPropagation();
            $(modal).modal();
            vm.reminderDone = reminder;
            vm.confirmModal = vm.doneReminder;
            vm.titleModal = !reminder.isDone ? "Habilitar Lembrete" : "Concluir Lembrete";
            vm.bodyModal = "Tem certeza que deseja " + (!reminder.isDone ? 'habilitar' : 'concluir') + " o lembrete?"
        }

        function showModalDelete(reminder) {
            vm.reminderDelete = reminder;
            vm.confirmModal = vm.deleteReminder;
            vm.titleModal = "Excluir Lembrete"
            vm.bodyModal = "Tem certeza que deseja excluir o lembrete?"
            $(modal).modal();
        }

        function filterTable() {
            vm.reminders = vm.remindersSource;
            if (vm.keyFilter.trim().length > 0) {
                vm.reminders = vm.reminders.filter(function (item) {
                    return item.title.toUpperCase().indexOf(vm.keyFilter.toUpperCase()) > -1 || item.limitDate.toLocaleDateString().indexOf(vm.keyFilter) > -1;
                });
            }
        }

        function orderBy(type) {
            vm.order = type;
            vm.reminders.sort(function (itemA, itemB) {
                switch (type) {
                    case 'title':
                        if (vm.asc) return itemA.title.localeCompare(itemB.title);
                        else return itemB.title.localeCompare(itemA.title);
                    case 'limitDate':
                        if (itemA.limitDate < itemB.limitDate) return vm.asc ? -1 : 1;
                        else if (itemA.limitDate > itemB.limitDate) return vm.asc ? 1 : -1;
                        return 0;
                    case 'isDone':
                        if (itemA.isDone && !itemB.isDone) return vm.asc ? -1 : 1;
                        else if (!itemA.isDone && itemB.isDone) return vm.asc ? 1 : -1;
                        return 0;
                    default:
                }
            });

            vm.asc = !vm.asc;
        }

        activate();
    }
})();
