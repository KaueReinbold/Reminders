(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('indexController', indexController);

    indexController.$inject = ['$scope', 'query', 'command'];

    function indexController($scope, query, command) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'indexController';

        
        function success(data) {
            console.log(data);
        }

        function error(data) {
            console.log(data);
        }

        query.getRequest(null, success, error);
    }
})();
