/**
 * Created by Kaue Reinbold on 16/11/2017.
 */

'use strict';

let RequestApi = Object.create(null, {

    ajaxRequest: {
        value: () => {

            let ajaxRequest = false;

            if (window.XMLHttpRequest) {
                ajaxRequest = new XMLHttpRequest();
            } else if (window.ActiveXObject) {
                try {
                    ajaxRequest = new ActiveXObject('Msxml2.XMLHTTP');
                } catch (e) {
                    try {
                        ajaxRequest = new ActiveXObject('Microsoft.XMLHTTP')
                    } catch (ex) {
                        ajaxRequest = false;
                    }
                }
            }
            return ajaxRequest;
        }
    },
    get: {
        value: (source) => {
            return new Promise((resolve, reject) => {

                let ajaxRequest = RequestApi.ajaxRequest();

                if (ajaxRequest) {
                    ajaxRequest.onreadystatechange = () => {
                        if (ajaxRequest.readyState == 4) {
                            if (ajaxRequest.status == 200 || ajaxRequest.status == 304) {
                                resolve(JSON.parse(ajaxRequest.response));
                            } else {
                                reject(Error(ajaxRequest.statusText));
                            }
                        }
                    };
                    ajaxRequest.open('GET', source, true);
                    ajaxRequest.send(null);
                }
            });
        }
    },
    post: {
        value: (source, data) => {
            return new Promise((resolve, reject) => {

                let ajaxRequest = RequestApi.ajaxRequest();

                if (ajaxRequest) {
                    ajaxRequest.onreadystatechange = () => {
                        if (ajaxRequest.readyState == 4) {
                            if (ajaxRequest.status == 200 || ajaxRequest.status == 304) {

                                if (ajaxRequest.response === "") {
                                    resolve('');
                                    return;
                                }

                                resolve(JSON.parse(ajaxRequest.response));
                            } else {
                                reject(Error(ajaxRequest.statusText));
                            }
                        }
                    };

                    var obj = new FormData();

                    for (const key in data)
                        obj.append(key, data[key]);

                    ajaxRequest.open('POST', source, true);
                    ajaxRequest.send(obj);
                }
            });
        }
    }
});