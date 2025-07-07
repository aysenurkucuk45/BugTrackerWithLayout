using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTrackerWithLayout.Models;
using System.IO;

namespace BugTrackerWithLayout.Controllers
{
    public class BugController : Controller
    {
        private BugTrackerDbContext db = new BugTrackerDbContext();

        // GET: Bug
        public ActionResult Index()
        {
            return View(db.Bugs.ToList());
        }

        // GET: Bug/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Bug bug = db.Bugs.Find(id);
            if (bug == null)
                return HttpNotFound();

            return View(bug);
        }

        // GET: Bug/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bug/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Priority")] Bug bug)
        {
            if (ModelState.IsValid)
            {
                bug.ReportedBy = User.Identity.Name ?? "Anonim";
                bug.CreatedAt = DateTime.Now;
                bug.Status = "Açık";

                db.Bugs.Add(bug);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bug);
        }

        // GET: Bug/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Bug bug = db.Bugs.Find(id);
            if (bug == null)
                return HttpNotFound();

            return View(bug);
        }

        // POST: Bug/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Priority,Status,ReportedBy,CreatedAt")] Bug bug)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bug).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bug);
        }

        // GET: Bug/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Bug bug = db.Bugs.Find(id);
            if (bug == null)
                return HttpNotFound();

            return View(bug);
        }

        // POST: Bug/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bug bug = db.Bugs.Find(id);
            db.Bugs.Remove(bug);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
