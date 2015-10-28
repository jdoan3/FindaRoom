using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace FindaRoom.Hubs
{
    public class MailHub : Hub
    {
        public void Send(string userId)
        {
            Clients.User(userId).checkMail();
        }
    }
}