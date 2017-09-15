$(function () {
    RegisterPrototype();

    RegisterEvents();

    ConfigureMessage();

    if ($('#limit_date')[0]) limit_date.value = new Date().toDateInputValue();
});