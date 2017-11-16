/**
 * Created by Kaue Reinbold on 16/11/2017.
 */

'use strict';

let PrototypeService = Object.create(null, {
    registerAll: {
        value: () => {
            Date.prototype.toDateInputValue = (function () {
                var local = new Date(this);
                local.setMinutes(this.getMinutes() - this.getTimezoneOffset());
                return local.toJSON().slice(0, 10);
            });
        }
    }
});