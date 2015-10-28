using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FindaRoom.Models;

namespace FindaRoom.Controllers
{
    public class FbInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /FbInfo/
        public ActionResult Index()
        {
            var fbinfoes = db.FbInfoes.Include(f => f.User);
            return View(fbinfoes.ToList());
        }
    }
}
