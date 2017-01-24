(function () {
    'use strict';

    angular
        .module('editorRotas')
        .factory('Server', Server);

    Server.$inject = ['$http'];

    function Server($http) {
        function get(action, data, successCallback, errorCallback) {
            var baseUrl = window.location.origin + urlGlobal + '/EditorRotas';
            var req = {
                method: 'GET',
                url: baseUrl + '/' + action,
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
        function getUsuarioLogado(action, data, successCallback, errorCallback) {
            var baseUrl = window.location.origin + urlGlobal;
            var req = {
                method: 'GET',
                url: baseUrl + '/' + action,
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
        function post(action, data, successCallback, errorCallback) {
            var baseUrl = window.location.origin + urlGlobal + '/EditorRotas';
            var req = {
                method: 'POST',
                url: baseUrl + '/' + action,
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

        function postArea(action, data, successCallback, errorCallback) {
            var baseUrl = window.location.origin + urlGlobal;
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

        return {
            get: get,
            getUsuarioLogado: getUsuarioLogado,
            post: post,
            postArea: postArea
        };
    }
})();