function RegisterEvents() {
    if (typeof (searchTable) !== "undefined")
        $(searchTable).keyup(function (element) {
            var value = $(this).val().toUpperCase().trim();
            $("#tbodyReminders > tr").each(function (index, element) {
                var textElementTitle = $($(element).children()[0]).text().toUpperCase();
                var textElementLimiteDate = $($(element).children()[1]).text().toUpperCase();
                if (textElementTitle.indexOf(value) > -1 || textElementLimiteDate.indexOf(value) > -1)
                    $(element).show();
                else
                    $(element).hide();
            });
        });

    if (typeof (theadReminders) !== "undefined")
        $(theadReminders).click(null, function (element) {
            var asc = eval(element.target.getAttribute("asc-order"));
            var name = element.target.getAttribute("name");
            switch (name) {
                case "title":
                    SortTableText(asc, 0);
                    break;
                case "limitDate":
                    SortTableDate(asc, 1);
                    break;
                case "isDone":
                    SortTableCheckbox(asc, 2);
                    break;
                default: break;
            }
            if (asc) {
                $("#arrow" + name).removeClass("glyphicon-chevron-up");
                $("#arrow" + name).addClass("glyphicon-chevron-down");
            }
            else {
                $("#arrow" + name).removeClass("glyphicon-chevron-down");
                $("#arrow" + name).addClass("glyphicon-chevron-up");
            }
            element.target.setAttribute("asc-order", !asc);
        });

    if (typeof (item_IsDone) !== "undefined")
        $(item_IsDone).click(function (event) {
            idReminderForDone.value = event.target.getAttribute("idReminder");
            idReminderDone.value = event.target.checked;
            event.preventDefault();
            event.stopPropagation();
            $(modal).modal();
            if (eval(idReminderDone.value)) {
                $("#doneReminderHead").removeClass("hideHeadModal");
                $("#doneReminderText").removeClass("hideTextModal");
            } else {
                $("#enableReminderHead").removeClass("hideHeadModal");
                $("#enableReminderText").removeClass("hideTextModal");
            }
        });

    if (typeof (saveDoneReminder) !== "undefined")
        $(saveDoneReminder).click(function (event) {
            PostDoneReminder(idReminderForDone.value, idReminderDone.value, function (result) {
                $(modal).modal('toggle');
                if (result.type === 0) {
                    result.type = "Success";
                } else if (result.type === 1) {
                    result.type = "Error";
                }

                location.href = location.origin + "?Type=" + result.type + "&Message=" + result.message;
            });
        });

    if (typeof (deleteReminderAction) !== "undefined")
        $(deleteReminderAction).click(function (event) {
            actionDelete.value = event.target.getAttribute("idReminder");
            event.preventDefault();
            event.stopPropagation();
            $(modalDelete).modal();
        });

    if (typeof (deleteReminder) !== "undefined")
        $(deleteReminder).click(function (event) {
            PostDeleteReminder(actionDelete.value, function (result) {
                $(modalDelete).modal('toggle');
                if (result.type === 0) {
                    result.type = "Success";
                } else if (result.type === 1) {
                    result.type = "Error";
                }

                location.href = location.origin + "?Type=" + result.type + "&Message=" + result.message;
            });
        });
}

function SortTableText(asc, column) {
    $("#tbodyReminders ").find("tr").sort(function (itemA, itemB) {
        var textA = $($("td", itemA)[column]).text();
        var textB = $($("td", itemB)[column]).text();
        if (asc) return textA.localeCompare(textB);
        else return textB.localeCompare(textA);
    }).appendTo($(tbodyReminders));
}

function SortTableDate(asc, column) {
    $("#tbodyReminders ").find("tr").sort(function (itemA, itemB) {
        var textA = $($("td", itemA)[column]).text().split("/");
        var textB = $($("td", itemB)[column]).text().split("/");
        var textADate = new Date(textA[2], textA[1], textA[0]);
        var textBDate = new Date(textB[2], textB[1], textB[0]);
        if (textADate < textBDate) return asc ? -1 : 1;
        else if (textADate > textBDate) return asc ? 1 : -1;
        return 0;
    }).appendTo($(tbodyReminders));
}

function SortTableCheckbox(asc, column) {
    $("#tbodyReminders ").find("tr").sort(function (itemA, itemB) {
        var textA = $($("td", itemA)[column]).find("input").is(":checked");
        var textB = $($("td", itemB)[column]).find("input").is(":checked");
        if (textA && !textB) return asc ? -1 : 1;
        else if (!textA && textB) return asc ? 1 : -1;
        return 0;
    }).appendTo($(tbodyReminders));
}