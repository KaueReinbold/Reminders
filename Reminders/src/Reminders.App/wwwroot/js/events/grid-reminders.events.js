/**
 * Created by Kaue Reinbold on 16/11/2017.
 */

'use strict';

let GridEventsService = Object.create(null, {
    asc: { value: true },
    column: { value: '' },
    create: {
        value: () => {
            if (typeof (searchTable) !== "undefined")
                $(searchTable).keyup(GridEventsService.keyUpSearchTable);

            if (typeof (theadReminders) !== "undefined")
                $(theadReminders).click(GridEventsService.clickSearchTable);

            if (typeof (item_is_done) !== "undefined")
                $(item_is_done).click(GridEventsService.clickItemIsDone);
        }
    },
    keyUpSearchTable: {
        value: function (event) {
            var value = $(this).val().toUpperCase().trim();
            $("#tbodyReminders > tr").each((index, element) => {
                var textElementTitle = $($(element).children()[0]).text().toUpperCase();
                var textElementLimiteDate = $($(element).children()[1]).text().toUpperCase();
                if (textElementTitle.indexOf(value) > -1 || textElementLimiteDate.indexOf(value) > -1)
                    $(element).show();
                else
                    $(element).hide();
            });
        }
    },
    clickSearchTable: {
        value: function (event) {
            EventsService.asc = eval(event.target.getAttribute("asc-order"));
            var name = event.target.getAttribute("name");
            switch (name) {
                case "title":
                    EventsService.column = 0;
                    $("#tbodyReminders ").find("tr").sort(GridEventsService.sortTableText).appendTo($(tbodyReminders));
                    break;
                case "limitDate":
                    EventsService.column = 1;
                    $("#tbodyReminders ").find("tr").sort(GridEventsService.sortTableDate).appendTo($(tbodyReminders));
                    break;
                case "isDone":
                    EventsService.column = 2;
                    $("#tbodyReminders ").find("tr").sort(GridEventsService.sortTableCheckbox).appendTo($(tbodyReminders));
                    break;
                default: break;
            }

            if (EventsService.asc) {
                $("#arrow" + name).removeClass("glyphicon-chevron-up");
                $("#arrow" + name).addClass("glyphicon-chevron-down");
            }
            else {
                $("#arrow" + name).removeClass("glyphicon-chevron-down");
                $("#arrow" + name).addClass("glyphicon-chevron-up");
            }

            event.target.setAttribute("asc-order", !EventsService.asc);
        }
    },
    clickItemIsDone: {
        value: function (event) {
            idReminderForDone.value = event.target.getAttribute("idReminder");
            idReminderDone.value = event.target.checked;
            event.preventDefault();
            event.stopPropagation();
            $(modalDone).modal();
            if (eval(idReminderDone.value)) {
                $("#doneReminderHead").removeClass("hideHeadModal");
                $("#doneReminderText").removeClass("hideTextModal");
            } else {
                $("#enableReminderHead").removeClass("hideHeadModal");
                $("#enableReminderText").removeClass("hideTextModal");
            }
        }
    },
    sortTableText: {
        value: function (itemA, itemB) {
            var textA = $($("td", itemA)[EventsService.column]).text();
            var textB = $($("td", itemB)[EventsService.column]).text();
            if (EventsService.asc) return textA.localeCompare(textB);
            else return textB.localeCompare(textA);
        }
    },
    sortTableDate: {
        value: function (itemA, itemB) {
            var textA = $($("td", itemA)[EventsService.column]).text().split("/");
            var textB = $($("td", itemB)[EventsService.column]).text().split("/");
            var textADate = new Date(textA[2], textA[1], textA[0]);
            var textBDate = new Date(textB[2], textB[1], textB[0]);
            if (textADate < textBDate) return EventsService.asc ? -1 : 1;
            else if (textADate > textBDate) return EventsService.asc ? 1 : -1;
            return 0;
        }
    },
    sortTableCheckbox: {
        value: function (itemA, itemB) {
            var textA = $($("td", itemA)[EventsService.column]).find("input").is(":checked");
            var textB = $($("td", itemB)[EventsService.column]).find("input").is(":checked");
            if (textA && !textB) return EventsService.asc ? -1 : 1;
            else if (!textA && textB) return EventsService.asc ? 1 : -1;
            return 0;
        }
    }
});