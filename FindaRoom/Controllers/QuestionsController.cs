using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FindaRoom.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FindaRoom.Controllers
{
    public class QuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Questions/Create
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                bool exist = db.Questions.Any(user => user.UserId == userId);
       //         if (exist != true)
       //         {
                    return View();
       //         }
       //         else
       //             return RedirectToAction("Index", "Manage");
            }
            return new HttpUnauthorizedResult();
        }
        [HttpPost]
        public void dataQuestions(Questions dataAnswers)
        {

            var userId = User.Identity.GetUserId();
            bool exist = db.Questions.Any(user => user.UserId == userId);
            if (exist)
            {
                Questions questions = db.Questions.Where(user => user.UserId == userId).FirstOrDefault();
                db.Entry(questions).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                dataAnswers.UserId = userId;
                db.Questions.Add(dataAnswers);
                db.SaveChanges();
            }
        }
        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Questions questions = db.Questions.Find(id);
                if (questions == null)
                {
                    return HttpNotFound();
                }
                return View(questions);
            }
            return new HttpUnauthorizedResult();
        }
        // POST: Questions/Edit/5
        // To protect from overpostingbattacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,formattedAddress,priceRange,genderInterest,occupation,moveInDate,aboutMe")] Questions questions)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(questions).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(questions);
            }
            return new HttpUnauthorizedResult();
        }
    }
}
