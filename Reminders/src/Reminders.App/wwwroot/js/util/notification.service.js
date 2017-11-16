/**
 * Created by Kaue Reinbold on 16/11/2017.
 */

'use strict';

let NotificationService = Object.create(null, {
    verifyNotificationServer: {
        value: () => {
            if (document.cookie.indexOf("StatusMessage") > -1) {

                var statusMessage = JSON.parse(decodeURIComponent(document.cookie.split("StatusMessage")[1].replace("=", "")))

                switch (statusMessage.type_message) {
                    case 0:
                        $("#messageNotification div").addClass('alert-success');
                        break;
                    default:
                        $("#messageNotification div").addClass('alert-error');
                        break;
                }

                $("#messageNotification div").text(statusMessage.text_message);
                $("#messageNotification").fadeIn('slow');

                setTimeout(() => { $("#messageNotification").fadeOut('slow'); }, 2000);

                document.cookie = 'StatusMessage=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;'
            }
        }
    },
    showNotificationSuccess: {
        value: (text) => {
            $("#messageNotification div").addClass('alert-success');
            $("#messageNotification div").text(text);
            $("#messageNotification").fadeIn('slow');
        }
    },
    showNotificationError: {
        value: (text) => {
            $("#messageNotification div").addClass('alert-error');
            $("#messageNotification div").text(text);
            $("#messageNotification").fadeIn('slow');
        }
    }
})