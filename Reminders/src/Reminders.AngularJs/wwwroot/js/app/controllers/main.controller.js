(function () {
    'use strict';

    angular
        .module('reminderApp')
        .controller('mainController', mainController);

    mainController.$inject = ['$scope', '$location', '$timeout'];

    function mainController($scope, $location, $timeout) {
        
        var vm = this;

        vm.title = 'mainController';
        vm.messageObj = { type: 0, message: "", classCss: "" };
        vm.showMessage = showMessage;

        function activate() { }

        function showMessage(message) {

            switch (message.type) {
                case 0: message.classCss = "label-success"; break;
                case 1: message.classCss = "label-danger"; break;
                default: message.classCss = ""; break;
            }

            vm.messageObj = message;

            $("#message").slideDown("slow");
            $timeout(function () {
                $("#message").slideUp("slow");
            }, 2000);
        }

        activate();
    }
})();
