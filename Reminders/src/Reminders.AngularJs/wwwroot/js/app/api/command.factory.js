(function () {
    'use strict';

    angular
        .module('reminderApp')
            .factory('command', function ($injector) {
                var moduleName = 'mdCommand';
                var mod = angular.module(moduleName);
                var contents = mod._invokeQueue;
                var out = {};
                contents.forEach(function (item) {
                    var name = item[2][0];
                    var instance = $injector.get(name);
                    for (var i in instance) { out[i] = instance[i]; }
                });
                return out;
            })
})();