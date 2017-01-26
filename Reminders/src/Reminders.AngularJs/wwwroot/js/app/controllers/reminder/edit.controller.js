(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('editController', editController);

    editController.$inject = ['$location', 'query', 'command', 'reminderrepository', 'util'];

    function editController($location, query, command, reminderrepository, util) {

        var vm = this;

        vm.title = 'editController';

        vm.reminder = {};

        function success(data) {
            vm.reminder = util.findAndConvertDateObject(data);
        }

        function error(data) {
            console.log(data);
        }

        function saveReminder() {

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
