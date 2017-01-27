(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('editController', editController);

    editController.$inject = ['$scope', '$location', 'query', 'command', 'repository', 'util'];

    function editController($scope, $location, query, command, repository, util) {

        var vm = this;

        vm.title = 'editController';

        vm.reminder = {};
        vm.editReminder = editReminder;

        function successLoad(data) {
            vm.reminder = util.findAndConvertDateObject(data);
        }

        function successEdit(data) {

            $scope.$parent.mc.showMessage(data);
            
            if (data.type === 0)
                $location.path("Index");
        }

        function error(data) {
            $(modal).modal('toggle');
            $scope.$parent.mc.showMessage(data);
        }

        function editReminder(formInvalid) {

            if (formInvalid) return;

            command.putReminder(vm.reminder, successEdit, error);
        }

        function activate() {
                       
            vm.reminder = repository.getReminder();

            if (vm.reminder.id)
                query.getReminder(vm.reminder, successLoad, error);
            else
                $location.path("Index");
        }

        activate();
    }
})();
