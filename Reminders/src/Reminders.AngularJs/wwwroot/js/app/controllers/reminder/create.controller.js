(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('createController', createController);

    createController.$inject = ['$scope', '$location', 'query', 'command', 'repository'];

    function createController($scope, $location, query, command, repository) {

        var vm = this;

        vm.title = 'createController';

        vm.reminder = {};
        vm.saveReminder = saveReminder;

        function success(data) {

            $scope.$parent.mc.showMessage(data);

            if (data.type === 0)
                $location.path("Index");
        }

        function error(data) {
            console.log(data);
        }

        function saveReminder(formInvalid) {

            if (formInvalid) return;

            command.postReminder(vm.reminder, success, error);
        }

        function activate() {

        }

        activate();
    }
})();
