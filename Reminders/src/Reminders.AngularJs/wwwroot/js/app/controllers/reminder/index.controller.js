(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('indexController', indexController);

    indexController.$inject = ['$scope'];

    function indexController($scope) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'indexController';

        activate();

        function activate() { }
    }
})();
