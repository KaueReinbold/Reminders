(function () {
    'use strict';

    angular
        .module('reminderApp')
            .factory('server', server);

    server.$inject = ['$http'];

    function server($http) {

        var baseUrl = 'http://reminderscoreapi.azurewebsites.net/api/';

        function getRequest(action, data, successCallback, errorCallback) {
            var req = {
                method: 'GET',
                url: baseUrl + action,
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                data: data ? data : null
            };
            $http(req).then(function (data) {
                successCallback(data);
            }, function (error) {
                if (error)
                    errorCallback(error);
            });
        }

        function postRequest(action, data, successCallback, errorCallback) {
            var req = {
                method: 'POST',
                url: baseUrl + action,
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                data: data
            };
            $http(req).then(function (data) {
                successCallback(data);
            }, function (error) {
                if (error)
                    errorCallback(error);
            });
        }

        function putRequest(action, data, successCallback, errorCallback) {
            var req = {
                method: 'PUT',
                url: baseUrl + action,
                headers: {
                    'Content-Type': 'application/json; charset=utf-8',
                },
                data: data ? data : null
            };
            $http(req).then(function (data) {
                successCallback(data);
            }, function (error) {
                if (error)
                    errorCallback(error);
            });
        }

        function deleteRequest(action, data, successCallback, errorCallback) {
            var req = {
                method: 'DELETE',
                url: baseUrl + action,
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                data: data
            };
            $http(req).then(function (data) {
                successCallback(data);
            }, function (error) {
                if (error)
                    errorCallback(error);
            });
        }

        return {
            getRequest: getRequest,
            postRequest: postRequest,
            putRequest: putRequest,
            deleteRequest: deleteRequest
        };
    }
})();