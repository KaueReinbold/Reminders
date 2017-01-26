(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('createController', createController);

    createController.$inject = ['$location', 'query', 'command', 'reminderrepository'];

    function createController($location, query, command, reminderrepository) {
        
        var vm = this;

        vm.title = 'createController';

        vm.reminder = {};

        function success(data) {
            vm.reminder = data;
        }

        function error(data) {
            console.log(data);
        }

        function activate() {

            
        }

        activate();
    }
})();
