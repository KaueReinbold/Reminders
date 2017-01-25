(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('detailsController', detailsController);

    detailsController.$inject = ['$location', 'query', 'command', 'reminderrepository'];

    function detailsController($location, query, command, reminderrepository) {

        var vm = this;

        vm.title = 'detailsController';

        vm.reminder = {};

        function success(data) {
            vm.reminder = data;
        }

        function error(data) {
            console.log(data);
        }

        function activate() {

            vm.reminder = reminderrepository.getReminder();

            if (vm.reminder.id)
                query.getReminder(vm.reminder, success, error);
            else
                $location.path("Index");
        }

        activate();
    }
})();
