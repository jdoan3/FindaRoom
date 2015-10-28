using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace FindaRoom.Hubs
{

    public class NotificationHub : Hub
    {
        public void SendNotification(string userId)
        {
            Clients.User(userId).broadcastNotification();
        }
    }
}