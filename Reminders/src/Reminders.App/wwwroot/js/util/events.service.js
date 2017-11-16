/**
 * Created by Kaue Reinbold on 16/11/2017.
 */

'use strict';

let EventsService = Object.create(null, {
    register: {
        value: () => {
            GridEventsService.create();

            ButtonEventsService.create();

            InputEventsService.create();

            ButtonEventsService.create();
        }
    }
})