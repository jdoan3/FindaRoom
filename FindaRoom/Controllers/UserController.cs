using Facebook;
using FindaRoom.Hubs;
using FindaRoom.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FindaRoom.Controllers
{
    public class UserController : Controller
    {

        private ApplicationDbContext db { get; set; }
        private UserManager<ApplicationUser> UserManager { get; set; }

        public UserController()
        {
            db = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: /User/
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var UserId = User.Identity.GetUserId();
                Questions answers = db.Questions.Where(e => e.UserId.Equals(UserId)).FirstOrDefault();
                if (answers == null)
                {
                    return RedirectToAction("Create", "Questions");
                }
                else
                {
                    var curUser = UserManager.FindById(User.Identity.GetUserId());
                    var userQuestions = db.Questions.Where(a => a.UserId.Equals(curUser.Id)).FirstOrDefault();
                    var filterId = db.Questions.Where(quest => quest.formattedAddress.Equals(userQuestions.formattedAddress)).ToList();
                    var filteredUsers = new List<FilterViewModel>();

                    foreach (var user in filterId)
                    {
                        bool hometown = user.UserId.Equals(curUser.Id);
                        bool matches = UserManager.FindById(user.UserId).Matches.Any().Equals(curUser.Matches.Any()) && curUser.Matches.Any();
                        if (!hometown && !matches)
                        {
                            filteredUsers.Add(new FilterViewModel()
                            {
                                Questions = db.Questions.Where(b => b.UserId.Equals(user.UserId)).FirstOrDefault(),
                                FbInfo = db.FbInfoes.Include("friendsList").Include("mutualFriendsList").Include("User").Where(c => c.UserId.Equals(user.UserId)).FirstOrDefault()
                            });
                        }
                    }

                    return View(filteredUsers);
                }
            }
                return new HttpUnauthorizedResult();

        }

        //
        // GET: /User/
        public ActionResult Matches()
        {
            if (User.Identity.IsAuthenticated)
            {
                var curUser = UserManager.FindById(User.Identity.GetUserId());
                var Matches = curUser.Matches;
                var MatchView = new List<FilterViewModel>();
                foreach (var user in Matches)
                {
                    MatchView.Add(new FilterViewModel()
                    {
                        Questions = db.Questions.Where(b => b.UserId.Equals(user.Id)).FirstOrDefault(),
                        FbInfo = db.FbInfoes.Include("friendsList").Include("mutualFriendsList").Include("User").Where(c => c.UserId.Equals(user.Id)).FirstOrDefault()
                    });
                }
                return View(MatchView);
            }
            return new HttpUnauthorizedResult();

        }

        [HttpPost]
        public bool Add(string userid)
        {
            // Deserilize json data 
            // add to your database
            var user = db.Users.Find(userid);
            var curUser = UserManager.FindById(User.Identity.GetUserId());
            curUser.Likes.Add(user);
            db.SaveChanges();
            if (user.Likes.Any().Equals(curUser.Likes.Any()))
            {
                IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                hubContext.Clients.User(user.UserName).broadcastNotification();
                hubContext.Clients.User(curUser.UserName).broadcastNotification();
                curUser.Matches.Add(user);
                user.Matches.Add(curUser);
                curUser.Likes.Remove(user);
                user.Likes.Remove(curUser);
                db.SaveChanges();
                return true;
            }
            else
                return false;
        }
        [HttpPost]
        public void RemoveMatch(string userid)
        {
            // Deserilize json data 
            // add to your database
            var user = db.Users.Find(userid);
            var curUser = UserManager.FindById(User.Identity.GetUserId());
            curUser.Likes.Remove(user);
            db.SaveChanges();
        }

    }
}
