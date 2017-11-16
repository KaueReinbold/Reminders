/**
    * Created by Kaue Reinbold on 16/11/2017.
*/

'use strict';

let InputEventsService = Object.create(null, {
    create: {
        value: () => {
            if (typeof (limit_date) !== "undefined") {
                limit_date.value = new Date().toDateInputValue();
                $(limit_date).keydown(InputEventsService.keyDownLimitDate);
            }
        }
    },
    keyDownLimitDate: {
        value: (event) => {
            if (event.which == 8 || event.which == 46)  // 8 == backspace
                event.preventDefault();
        }
    }
});