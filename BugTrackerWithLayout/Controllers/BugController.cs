using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using BugTrackerWithLayout.Models;
using BugTrackerWithLayout.ViewModels;

namespace BugTrackerWithLayout.Controllers
{
    [Authorize]
    public class BugController : Controller
    {
        private readonly BugTrackerDbContext db = new BugTrackerDbContext();

        /* ========== CREATE ================================================= */

        // GET: Bug/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(),
                                                 "CategoryId",
                                                 "Name");                 // ✅ düzeltildi
            ViewBag.PriorityList = new SelectList(new[] { "Düşük", "Orta", "Yüksek" });
            ViewBag.StatusList = new SelectList(new[] { "Açık", "Devam Ediyor", "Çözüldü" });
            return View();
        }

        // POST: Bug/Create
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Id,Title,Description,Priority,Status,CreatedAt,CategoryId")] Bug bug,
            HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                bug.ReportedBy = User.Identity.Name;
                bug.CreatedAt = DateTime.Now;

                /* dosya yükleme */
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var uploadPath = Server.MapPath("~/Uploads");
                    Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);
                    file.SaveAs(filePath);

                    bug.Attachment = fileName;
                    bug.FilePath = "/Uploads/" + fileName;
                }

                db.Bugs.Add(bug);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            /* ModelState invalid ise listeleri tekrar yolla */
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "Name", bug.CategoryId);
            ViewBag.PriorityList = new SelectList(new[] { "Düşük", "Orta", "Yüksek" }, bug.Priority);
            ViewBag.StatusList = new SelectList(new[] { "Açık", "Devam Ediyor", "Çözüldü" }, bug.Status);
            return View(bug);
        }

        /* ========== INDEX (liste + sayfalama) =============================== */
        public ActionResult Index(int? page)
        {
            const int pageSize = 5;
            int pageNumber = page ?? 1;

            var bugs = db.Bugs.Include(b => b.Category)
                              .OrderByDescending(b => b.CreatedAt)
                              .ToPagedList(pageNumber, pageSize);

            return View(bugs);
        }

        /* ========== KULLANICI ÖZEL / PROFİL ================================ */
        public ActionResult MyBugs()
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            var bugs = db.Bugs.Where(b => b.ReportedBy == username)
                              .OrderByDescending(b => b.CreatedAt)
                              .ToList();

            return View("MyBugs", new UserProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Bugs = bugs
            });
        }

        public ActionResult UserProfile()
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            var bugs = db.Bugs.Where(b => b.ReportedBy == username)
                              .OrderByDescending(b => b.CreatedAt)
                              .ToList();

            return View(new UserProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Bugs = bugs
            });
        }

        /* ========== EDIT =================================================== */
        public ActionResult Edit(int? id)
        {
            if (id == null) return HttpNotFound();
            var bug = db.Bugs.Find(id);
            if (bug == null) return HttpNotFound();

            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "Name", bug.CategoryId);
            ViewBag.PriorityList = new SelectList(new[] { "Düşük", "Orta", "Yüksek" }, bug.Priority);
            ViewBag.StatusList = new SelectList(new[] { "Açık", "Devam Ediyor", "Çözüldü" }, bug.Status);
            return View(bug);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Bug bug, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var uploadPath = Server.MapPath("~/Uploads");
                    Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);
                    file.SaveAs(filePath);

                    bug.Attachment = fileName;
                    bug.FilePath = "/Uploads/" + fileName;
                }

                db.Entry(bug).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyBugs");
            }

            /* ModelState invalid */
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "Name", bug.CategoryId);
            ViewBag.PriorityList = new SelectList(new[] { "Düşük", "Orta", "Yüksek" }, bug.Priority);
            ViewBag.StatusList = new SelectList(new[] { "Açık", "Devam Ediyor", "Çözüldü" }, bug.Status);
            return View(bug);
        }

        /* ========== DELETE, DETAILS, ÇÖZÜM vb. ============================ */

        public ActionResult Delete(int? id)
        {
            if (id == null) return HttpNotFound();
            var bug = db.Bugs.Find(id);
            return bug == null ? (ActionResult)HttpNotFound() : View(bug);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var bug = db.Bugs.Find(id);
            if (bug != null)
            {
                db.Bugs.Remove(bug);
                db.SaveChanges();
            }
            return RedirectToAction("MyBugs");
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return HttpNotFound();
            var bug = db.Bugs.Find(id);
            return bug == null ? (ActionResult)HttpNotFound() : View(bug);
        }

        /* Çözüm ekleme / temizleme ---------------------------------------- */
        public ActionResult AddSolution(int id)
        {
            var bug = db.Bugs.Find(id);
            return bug == null ? (ActionResult)HttpNotFound() : View(bug);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddSolution(int id, string solution, string status)
        {
            var bug = db.Bugs.Find(id);
            if (bug == null) return HttpNotFound();

            bug.Solution = solution;
            bug.Status = status;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ClearSolution(int id)
        {
            var bug = db.Bugs.Find(id);
            if (bug == null) return HttpNotFound();

            bug.Solution = null;
            bug.Status = "Açık";
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        /* ========== Paged AllBugs (ayrı görünüm) ========================== */
        public ActionResult AllBugs(int? page)
        {
            int pageNumber = page ?? 1;
            const int pageSize = 5;

            var bugs = db.Bugs.Include(b => b.Category)
                              .OrderByDescending(b => b.CreatedAt)
                              .ToPagedList(pageNumber, pageSize);

            return View(bugs);
        }

        /* ========== JSON --------------------------------------------------- */
        [HttpGet]
        public JsonResult GetBugs()
        {
            var bugs = db.Bugs.Select(b => new
            {
                b.Id,
                b.Title,
                b.Description,
                b.Priority,
                b.Status,
                b.ReportedBy,
                CreatedAt = b.CreatedAt.ToString("yyyy-MM-dd"),
                Category = b.Category != null ? b.Category.Name : "Kategori Yok"
            }).ToList();

            return Json(bugs, JsonRequestBehavior.AllowGet);
        }
    }
}
