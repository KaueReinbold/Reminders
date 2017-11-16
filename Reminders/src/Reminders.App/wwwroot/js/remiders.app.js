/**
 * Created by Kaue Reinbold on 16/11/2017.
 */

'use strict';

let RemidersApp = Object.create(null, {
    init: {
        value: () => {

            PrototypeService.registerAll();

            EventsService.register();

            NotificationService.verifyNotificationServer();
        }
    }
});