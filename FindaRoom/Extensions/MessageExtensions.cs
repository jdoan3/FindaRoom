using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using FindaRoom.Models;
using Microsoft.Web.Mvc;
using FindaRoom.Controllers;

namespace FindaRoom.Extensions
{
    public static class MessageExtensions
    {
        public static IHtmlString MessageDetailsLink(this HtmlHelper htmlHelper, Message message)
        {
            string linkText = message.UserSender + "---------------" + message.SentAt;
            if (message.Unread)
            {
                return htmlHelper.ActionLink(linkText, "Respond", "MailBox", new { id = message.Id }, new {style="font-weight:bold;"});
            }
            return htmlHelper.ActionLink(linkText, "Respond", "MailBox", new { id = message.Id }, null);
        }
        public static IHtmlString InboxLink(this HtmlHelper htmlHelper, IEnumerable<Message> message)
        {
            var unreadMessages = message.Count(e => e.Unread);
            string linktext = unreadMessages == 0 ? "Inbox" : "Inbox" + "(" + unreadMessages + ")";
            return htmlHelper.ActionLink<MailBoxController>(e => e.Index(), linktext, new { @id = "InboxSubNav" });
        
        }
    }
}