(function() {
    'use strict';

    angular
        .module('reminderApp')
        .directive('modal', modal);

    modal.$inject = ['$window'];
    
    function modal ($window) {
        // Usage:
        //     <modal></modal>
        // Creates:
        // 
        var directive = {
            link: link,
            restrict: 'E',
            templateUrl: 'js/app/directive/modal.html',
            scope: false
        };
        return directive;

        function link(scope, element, attrs) {
        }
    }

})();