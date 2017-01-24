(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('indexController', indexController);

    indexController.$inject = ['$scope', 'query'];

    function indexController($scope, query) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'indexController';

        activate();

        function activate() { }
    }
})();
