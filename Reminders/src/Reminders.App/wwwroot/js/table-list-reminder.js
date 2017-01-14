$(function () {

    $(searchTable).keyup(function (element) {
        var value = $(this).val().toUpperCase();
        $("#tbodyReminders > tr").each(function (index, element) {
            var textElementTitle = $($(element).children()[0]).text().toUpperCase();
            var textElementLimiteDate = $($(element).children()[1]).text().toUpperCase();
            if (textElementTitle.indexOf(value) > -1 || textElementLimiteDate.indexOf(value) > -1)
                $(element).show();
            else
                $(element).hide();
        });
    });

    $(theadReminders).click(null, function (element) {
        var asc = eval(element.target.getAttribute("asc-order"));
        var name = element.target.getAttribute("name");
        switch (name) {
            case "title":
                SortTable(asc, 0);
                break;
            case "limitDate":
                SortTable(asc, 1);
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

    function SortTable(asc, column) {
        $("#tbodyReminders ").find("tr").sort(function (itemA, itemB) {
            var textA = $($("td", itemA)[column]).text();
            var textB = $($("td", itemB)[column]).text();
            if (asc) return textA.localeCompare(textB);
            else return textB.localeCompare(textA);
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

    $(item_IsDone).click(
        function (event) {
            idReminderForDone.value = event.target.getAttribute("idReminder");
            idReminderDone.value = event.target.checked;
            event.preventDefault();
            event.stopPropagation();
            $(modal).modal();
        }
    );

    $(saveDoneReminder).click(function (event) {
        $.post("Reminder/DoneReminder", { id: idReminderForDone.value, isDone: idReminderDone.value }, function (data, status) {
            $(modal).modal('toggle');
            location.reload();
        });
    });
});