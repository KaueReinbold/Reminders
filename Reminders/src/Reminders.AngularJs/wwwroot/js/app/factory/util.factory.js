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
                        if (isNaN(itemResult[item]) && !isNaN(new Date(itemResult[item])))
                            itemResult[item] = new Date(itemResult[item]);
                    }
                });
            } else {
                for (var item in obj) {
                    if (isNaN(obj[item]) && !isNaN(new Date(obj[item])))
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