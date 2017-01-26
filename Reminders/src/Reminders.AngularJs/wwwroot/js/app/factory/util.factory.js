(function () {
    'use strict';

    angular
        .module('reminderApp')
        .factory('util', util);

    util.$inject = ['$http'];

    function util($http) {

        function findAndConvertDateObject(obj) {
            if (!obj) return null;
            if (obj.length) {
                obj.forEach(function (itemResult) {
                    for (var item in itemResult) {
                        if (Date.parse(itemResult[item]) > 0)
                            itemResult[item] = new Date(itemResult[item]);
                    }
                });
            } else {
                for (var item in obj) {
                    if (Date.parse(obj[item]) > 0)
                        obj[item] = new Date(obj[item]);
                }
            }
            return obj;
        }

        return {
            findAndConvertDateObject: findAndConvertDateObject
        };
    }
})();