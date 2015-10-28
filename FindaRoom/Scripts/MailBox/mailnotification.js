/*$(function () {
    var mailHub = $.connection['mailHub'];
    $.connection.hub.logging = true;

    mailHub.client.checkMail = function () {
        $('#userInfo').append('<li>' + '<a href = "/Mailbox/Index">Check your mail</a>' + '</li>');
    };
    $.connection.hub.start();
});*/
$(function () {
    var mailHub = $.connection['mailHub'];
    $.connection.hub.logging = true;
    mailHub.client.checkMail = function () {
        $.ajax({
            url: "/MailBox/GetNewMessages",
            success: function (data, status) {
                prependNewMessagesToList(data);
                increaseInboxUnreadCounter();
            },
            data: { receiver: getLoggedUserId() }
        });
    };
    $.connection.hub.start();
    var getLoggedUserId = function () {
        var userId = $('#loggedUser').text();
        return userId;
    };

    var prependNewMessagesToList = function (li) {
        $('#messages').prepend(li);
    };

    var increaseInboxUnreadCounter = function () {
        var inboxElement = $('#InboxSubNav');
        var inbox = inboxElement.text();
        var positionOpenBrace = inbox.indexOf('(');
        if (positionOpenBrace == -1) {
            inboxElement.text(inbox + '(1)');
            return;
        }
        var linkLength = inbox.length;
        var unreadMessages = inbox.substring(positionOpenBrace + 1, linkLength - 1);
        var unreadMessagesParced = parseInt(unreadMessages);
        unreadMessagesParced++;
        inboxElement.text('Inbox (' + unreadMessagesParced + ')');
    };


});
