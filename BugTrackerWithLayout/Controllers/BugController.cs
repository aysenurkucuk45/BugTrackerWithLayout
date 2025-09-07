using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using BugTrackerWithLayout.Models;
using BugTrackerWithLayout.ViewModels;

// PDF için
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BugTrackerWithLayout.Controllers
{
    [Authorize]
    public class BugController : Controller
    {
        private readonly BugTrackerDbContext db = new BugTrackerDbContext();

        /* ================================================================
           Helpers
        ================================================================= */
        private IQueryable<Bug> QueryBugs()
        {
            return db.Bugs.Include(b => b.Category).OrderByDescending(b => b.CreatedAt);
        }

        private int ResolvePageSize(int? pageSize)
        {
            if (pageSize.HasValue && pageSize.Value > 0)
            {
                Session["BugPageSize"] = pageSize.Value;
                return pageSize.Value;
            }
            if (Session["BugPageSize"] is int s && s > 0) return s;
            return 5; // varsayılan
        }

        /* ========== CREATE ================================================= */

        // GET: Bug/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "Name");
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

        /* ========== INDEX (liste + sayfalama + pageSize) =================== */
        public ActionResult Index(int? page, int? pageSize)
        {
            int size = ResolvePageSize(pageSize);
            int pageNumber = page ?? 1;

            var bugs = QueryBugs().ToPagedList(pageNumber, size);

            ViewBag.CurrentPageSize = size; // View'da "kaçarlı gösterilsin" için
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
        public ActionResult AllBugs(int? page, int? pageSize)
        {
            int size = ResolvePageSize(pageSize);
            int pageNumber = page ?? 1;

            var bugs = QueryBugs().ToPagedList(pageNumber, size);

            ViewBag.CurrentPageSize = size;
            return View(bugs);
        }

        /* ========== EXPORT: Excel (CSV) =================================== */
        [HttpGet]
        public FileResult ExportExcel()
        {
            var data = QueryBugs().ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Id;Başlık;Açıklama;Öncelik;Durum;Bildiren;Tarih;Kategori");

            foreach (var b in data)
            {
                var title = (b.Title ?? "").Replace(";", ",");
                var desc = (b.Description ?? "").Replace(";", ",");
                var cat = b.Category != null ? b.Category.Name : "Kategori Yok";
                sb.AppendLine($"{b.Id};{title};{desc};{b.Priority};{b.Status};{b.ReportedBy};{b.CreatedAt:yyyy-MM-dd};{cat}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"Hatalar_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }

        /* ========== EXPORT: PDF (iTextSharp) ============================== */
        [HttpGet]
        public FileResult ExportPdf()
        {
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                var writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var fontHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var fontCell = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                var title = new Paragraph("Hata Listesi", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16));
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 10f;
                doc.Add(title);

                var table = new PdfPTable(8) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 5, 18, 28, 8, 10, 12, 10, 12 });

                // header
                string[] headers = { "Id", "Başlık", "Açıklama", "Öncelik", "Durum", "Bildiren", "Tarih", "Kategori" };
                foreach (var h in headers)
                {
                    var cell = new PdfPCell(new Phrase(h, fontHeader))
                    {
                        BackgroundColor = new BaseColor(235, 235, 235),
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                // rows
                var data = QueryBugs().ToList();
                foreach (var b in data)
                {
                    table.AddCell(new Phrase(b.Id.ToString(), fontCell));
                    table.AddCell(new Phrase(b.Title ?? "", fontCell));
                    table.AddCell(new Phrase(b.Description ?? "", fontCell));
                    table.AddCell(new Phrase(b.Priority ?? "", fontCell));
                    table.AddCell(new Phrase(b.Status ?? "", fontCell));
                    table.AddCell(new Phrase(b.ReportedBy ?? "", fontCell));
                    table.AddCell(new Phrase(b.CreatedAt.ToString("yyyy-MM-dd"), fontCell));
                    table.AddCell(new Phrase(b.Category != null ? b.Category.Name : "Kategori Yok", fontCell));
                }

                doc.Add(table);
                doc.Close();
                writer.Close();

                var bytes = ms.ToArray();
                return File(bytes, "application/pdf", $"Hatalar_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
            }
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
