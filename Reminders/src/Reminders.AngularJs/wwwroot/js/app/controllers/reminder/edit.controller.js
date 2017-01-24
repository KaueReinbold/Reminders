(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('editController', editController);

    editController.$inject = ['$location']; 

    function editController($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'editController';

        activate();

        function activate() { }
    }
})();
