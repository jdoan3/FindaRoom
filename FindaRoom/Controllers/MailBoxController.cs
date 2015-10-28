using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using FindaRoom.Models;
using Microsoft.AspNet.SignalR;
using FindaRoom.Hubs;

namespace FindaRoom.Controllers
{
    public class MailBoxController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /MailBox/
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                return View(db.Messages.Where(e => e.ReceiverId.Equals(userId)).OrderByDescending(e => e.SentAt));
            }
            return new HttpUnauthorizedResult();
        }
        // GET: /MailBox/Create
        public ActionResult Create(string userId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user1Id = User.Identity.GetUserId();
                var UserName = db.Users.Where(e => e.Id.Equals(userId)).FirstOrDefault().UserName;
                Message message = new Message();
                message.UserReceiver = UserName;
                message.UserSender = db.Users.Where(e => e.Id.Equals(user1Id)).FirstOrDefault().UserName;
                message.ReceiverId = userId;
                return View(message);
            }
            return new HttpUnauthorizedResult();
        }

        // POST: /MailBox/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserSender,ReceiverId,UserReceiver,Subject,MessageBody")] Message message)
        {
            if (ModelState.IsValid)
            {
                message.SenderId = User.Identity.GetUserId();
                message.SentAt = DateTime.Now;
                message.Unread = true;
                db.Messages.Add(message);
                db.SaveChanges();

                IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<MailHub>();
                hubContext.Clients.User(message.UserReceiver).checkMail();
                return RedirectToAction("Index");
            }

            return View(message);

        }


        // POST: /MailBox/Delete
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAction(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //Get Method input for view into Respond
        public ActionResult Respond(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                Message newMessage = new Message();
                Message message = db.Messages.Find(id);
                message.Unread = false;
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
                newMessage.SenderId = message.ReceiverId;
                newMessage.ReceiverId = message.SenderId;
                newMessage.UserSender = message.UserReceiver;
                newMessage.UserReceiver = message.UserSender;
                return View(newMessage);
            }
            return new HttpUnauthorizedResult();

        }

        //Post method enables user to respond to an email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Respond(Message message)
        {
            if (ModelState.IsValid)
            {
                message.SentAt = DateTime.Now;
                message.Unread = true;
                db.Messages.Add(message);
                db.SaveChanges();
                IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<MailHub>();
                hubContext.Clients.User(message.UserReceiver).checkMail();
                return RedirectToAction("Index");
            }
            return View(message);
        }
        public ActionResult InboxHeader()
        { 
            var userId = User.Identity.GetUserId();
            var messages = db.Messages.Where(e => e.UserReceiver.Equals(userId));
            return PartialView(messages);
       }
        //Get message when a user sends a message to you
        public ActionResult GetNewMessages(string receiver)
        {
            var messages = db.Messages.Where(e => e.UserReceiver.Equals(receiver) && e.Unread).OrderByDescending(e => e.SentAt).FirstOrDefault();
            return PartialView(messages);
        }
    }
}
