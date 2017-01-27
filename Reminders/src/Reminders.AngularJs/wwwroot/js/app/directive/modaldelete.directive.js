(function() {
    'use strict';

    angular
        .module('reminderApp')
        .directive('modalDelete', modaldelete);

    modaldelete.$inject = ['$window'];
    
    function modaldelete ($window) {
        // Usage:
        //     <modaldelete></modaldelete>
        // Creates:
        // 
        var directive = {
            link: link,
            restrict: 'E',
            templateUrl: 'js/app/directive/modaldelete.html',
            scope: []
        };
        return directive;

        function link(scope, element, attrs) {

        }
    }

})();