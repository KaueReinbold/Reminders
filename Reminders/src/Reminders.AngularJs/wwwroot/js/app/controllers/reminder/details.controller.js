(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('detailsController', detailsController);

    detailsController.$inject = ['$location']; 

    function detailsController($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'detailsController';

        activate();

        function activate() { }
    }
})();
