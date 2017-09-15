$(function () {
    RegisterPrototype();

    RegisterEvents();

    ConfigureMessage();

    if ($('#LimitDate')[0]) LimitDate.value = new Date().toDateInputValue();
});