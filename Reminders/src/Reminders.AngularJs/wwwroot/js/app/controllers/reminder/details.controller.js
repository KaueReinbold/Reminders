(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('detailsController', detailsController);

    detailsController.$inject = ['$location', 'query', 'command', 'repository'];

    function detailsController($location, query, command, repository) {

        var vm = this;

        vm.title = 'detailsController';

        vm.reminder = {};

        function success(data) {
            vm.reminder = data;
        }

        function error(data) {
            $(modal).modal('toggle');
            $scope.$parent.mc.showMessage(data);
        }

        function activate() {

            vm.reminder = repository.getReminder();

            if (vm.reminder.id)
                query.getReminder(vm.reminder, success, error);
            else
                $location.path("");
        }

        activate();
    }
})();
