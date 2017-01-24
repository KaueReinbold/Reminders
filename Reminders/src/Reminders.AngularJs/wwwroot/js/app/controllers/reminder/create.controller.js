(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('createController', createController);

    createController.$inject = ['$location']; 

    function createController($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'createController';

        activate();

        function activate() { }
    }
})();
