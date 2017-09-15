function ConfigureMessage() {
    location.search.replace("?", "").split("&").forEach(function (item) {
        var obj = item.split("=");
        var status = "";
        var message = "";
        var classLabel = ";"

        switch (obj[0]) {
            case "Type":
                status = obj[1];
                break;
            case "Message":
                message = obj[1];
                break;
            default:
        }

        switch (status) {
            case "Success":
                classLabel = "label-success";
                break;
            case "Error":
                classLabel = "label-danger";
                break;
            default:
        }

        $("#message").addClass(classLabel);
        $("#message > label").text(decodeURIComponent(message));
        $("#message").slideDown("slow");
        setTimeout(function () {
            $("#message").slideUp("slow");
        }, 2000);
    });
}

function RegisterPrototype() {
    Date.prototype.toDateInputValue = (function () {
        var local = new Date(this);
        local.setMinutes(this.getMinutes() - this.getTimezoneOffset());
        return local.toJSON().slice(0, 10);
    });
}