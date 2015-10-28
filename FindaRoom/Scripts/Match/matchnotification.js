$(function () {
    var notifications = $.connection.notificationHub;
    $.connection.hub.logging = true;

    notifications.client.broadcastNotification = function () {
        alert("You got an Hello from a stranger!");
    };

    $.connection.hub.start().done(function () { });
});