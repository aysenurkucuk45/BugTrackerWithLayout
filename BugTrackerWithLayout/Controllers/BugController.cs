using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using BugTrackerWithLayout.Models;
using BugTrackerWithLayout.ViewModels;
using ClosedXML.Excel;
using System.Collections.Generic;
using Rotativa;                  // İstersen kullanmaya devam
using iTextSharp.text;          // iTextSharp (view olmadan PDF)
using iTextSharp.text.pdf;

namespace BugTrackerWithLayout.Controllers
{
    [Authorize]
    public class BugController : Controller
    {
        private readonly BugTrackerDbContext db = new BugTrackerDbContext();

        // --- Sayfa boyutu tercihini (kullanıcı bazlı) hatırla ---
        private int ResolvePageSize(int? pageSize)
        {
            if (pageSize.HasValue)
            {
                Session["BugListPageSize"] = pageSize.Value;
                return pageSize.Value;
            }
            if (Session["BugListPageSize"] is int ps && ps > 0) return ps;
            return 10;
        }

        // ------------------------ CREATE ------------------------
        public ActionResult Create()
        {
            var categories = db.Categories.ToList();
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "Name");
            ViewBag.PriorityList = new SelectList(new[] { "Düşük", "Orta", "Yüksek" });
            ViewBag.StatusList = new SelectList(new[] { "Açık", "Devam Ediyor", "Çözüldü" });
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Priority,Status,CreatedAt,CategoryId")] Bug bug, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                bug.ReportedBy = User.Identity.Name;
                bug.CreatedAt = DateTime.Now;

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

            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "Name", bug.CategoryId);
            ViewBag.PriorityList = new SelectList(new[] { "Düşük", "Orta", "Yüksek" }, bug.Priority);
            ViewBag.StatusList = new SelectList(new[] { "Açık", "Devam Ediyor", "Çözüldü" }, bug.Status);
            return View(bug);
        }

        // ------------------------ INDEX (filtre + sayfalama + export param) ------------------------
        public ActionResult Index(BugListFilterViewModel vm, int? page, int? pageSize)
        {
            vm.PageSize = ResolvePageSize(pageSize);
            vm.Page = page ?? 1;

            // Dropdown verileri
            vm.Categories = db.Categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                .ToList();

            vm.Statuses = new[]
            {
                new SelectListItem{Value="Açık",          Text="Açık"},
                new SelectListItem{Value="Devam Ediyor", Text="Devam Ediyor"},
                new SelectListItem{Value="Çözüldü",      Text="Çözüldü"}
            };

            vm.Priorities = new[]
            {
                new SelectListItem{Value="Düşük",  Text="Düşük"},
                new SelectListItem{Value="Orta",   Text="Orta"},
                new SelectListItem{Value="Yüksek", Text="Yüksek"}
            };

            // Sorgu + filtreler
            var q = db.Bugs
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(vm.Keyword))
                q = q.Where(b => b.Title.Contains(vm.Keyword) || b.Description.Contains(vm.Keyword));

            if (!string.IsNullOrWhiteSpace(vm.Status))
                q = q.Where(b => b.Status == vm.Status);

            if (!string.IsNullOrWhiteSpace(vm.Priority))
                q = q.Where(b => b.Priority == vm.Priority);

            if (vm.CategoryId.HasValue)
                q = q.Where(b => b.CategoryId == vm.CategoryId.Value);

            if (vm.FromDate.HasValue)
                q = q.Where(b => b.CreatedAt >= vm.FromDate.Value);

            if (vm.ToDate.HasValue)
            {
                var eod = vm.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                q = q.Where(b => b.CreatedAt <= eod);
            }

            // Export için tam filtreli liste
            vm.AllBugsForExport = q.ToList();

            // "Hepsi" seçeneği (örn. 99999)
            var effectivePageSize = vm.PageSize >= 99999
                ? Math.Max(vm.AllBugsForExport.Count(), 1)
                : vm.PageSize;

            vm.PagedBugs = q.ToPagedList(vm.Page, effectivePageSize);

            return View(vm);
        }

        // ------------------------ MY BUGS ------------------------
        public ActionResult MyBugs()
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.UserName == username); // Username -> UserName
            if (user == null) return RedirectToAction("Index");

            var bugs = db.Bugs.Where(b => b.ReportedBy == username)
                              .OrderByDescending(b => b.CreatedAt)
                              .ToList();

            return View("MyBugs", new UserProfileViewModel
            {
                Username = user.UserName,               // Username -> UserName
                Email = user.Email,
                ProfileImagePath = user.Avatar,         // ProfileImagePath -> Avatar
                Bugs = bugs
            });
        }

        // ------------------------ USER PROFILE ------------------------
        public ActionResult UserProfile()
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.UserName == username); // Username -> UserName
            if (user == null) return RedirectToAction("Index");

            var bugs = db.Bugs.Where(b => b.ReportedBy == username)
                              .OrderByDescending(b => b.CreatedAt)
                              .ToList();

            return View(new UserProfileViewModel
            {
                Username = user.UserName,               // Username -> UserName
                Email = user.Email,
                ProfileImagePath = user.Avatar,         // ProfileImagePath -> Avatar
                Bugs = bugs,
                TotalBugs = bugs.Count,
                OpenBugCount = bugs.Count(b => b.Status == "Açık"),
                InProgressBugCount = bugs.Count(b => b.Status == "Devam Ediyor"),
                ResolvedBugCount = bugs.Count(b => b.Status == "Çözüldü"),
                CommentCount = bugs.Sum(b => b.Comments != null ? b.Comments.Count : 0),
                SolutionCount = bugs.Sum(b => b.Solutions != null ? b.Solutions.Count : 0)
            });
        }

        // ------------------------ EDIT ------------------------
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

            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "Name", bug.CategoryId);
            ViewBag.PriorityList = new SelectList(new[] { "Düşük", "Orta", "Yüksek" }, bug.Priority);
            ViewBag.StatusList = new SelectList(new[] { "Açık", "Devam Ediyor", "Çözüldü" }, bug.Status);
            return View(bug);
        }

        // ------------------------ DELETE ------------------------
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

        // ------------------------ DETAILS ------------------------
        public ActionResult Details(int? id)
        {
            if (id == null) return HttpNotFound();
            var bug = db.Bugs.Find(id);
            return bug == null ? (ActionResult)HttpNotFound() : View(bug);
        }

        // ------------------------ SOLUTION ADD/CLEAR ------------------------
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

        // ------------------------ (Opsiyonel) AllBugs ------------------------
        public ActionResult AllBugs(BugListFilterViewModel vm, int? page, int? pageSize)
        {
            vm.PageSize = ResolvePageSize(pageSize);
            vm.Page = page ?? 1;

            vm.Categories = db.Categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                .ToList();

            vm.Statuses = new[]
            {
                new SelectListItem{Value="Açık",          Text="Açık"},
                new SelectListItem{Value="Devam Ediyor", Text="Devam Ediyor"},
                new SelectListItem{Value="Çözüldü",      Text="Çözüldü"}
            };

            vm.Priorities = new[]
            {
                new SelectListItem{Value="Düşük",  Text="Düşük"},
                new SelectListItem{Value="Orta",   Text="Orta"},
                new SelectListItem{Value="Yüksek", Text="Yüksek"}
            };

            var q = db.Bugs.Include(b => b.Category).OrderByDescending(b => b.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(vm.Keyword))
                q = q.Where(b => b.Title.Contains(vm.Keyword) || b.Description.Contains(vm.Keyword));
            if (!string.IsNullOrWhiteSpace(vm.Status))
                q = q.Where(b => b.Status == vm.Status);
            if (!string.IsNullOrWhiteSpace(vm.Priority))
                q = q.Where(b => b.Priority == vm.Priority);
            if (vm.CategoryId.HasValue)
                q = q.Where(b => b.CategoryId == vm.CategoryId.Value);
            if (vm.FromDate.HasValue)
                q = q.Where(b => b.CreatedAt >= vm.FromDate.Value);
            if (vm.ToDate.HasValue)
            {
                var eod = vm.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                q = q.Where(b => b.CreatedAt <= eod);
            }

            vm.AllBugsForExport = q.ToList();
            var effectivePageSize = vm.PageSize >= 99999 ? Math.Max(vm.AllBugsForExport.Count(), 1) : vm.PageSize;
            vm.PagedBugs = q.ToPagedList(vm.Page, effectivePageSize);
            return View(vm);
        }

        // ------------------------ Excel Export (ClosedXML) ------------------------
        public FileResult ExportExcel(string keyword, string status, string priority, int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            var q = db.Bugs.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(b => b.Title.Contains(keyword) || b.Description.Contains(keyword));
            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(b => b.Status == status);
            if (!string.IsNullOrWhiteSpace(priority))
                q = q.Where(b => b.Priority == priority);
            if (categoryId.HasValue)
                q = q.Where(b => b.CategoryId == categoryId.Value);
            if (fromDate.HasValue)
                q = q.Where(b => b.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
            {
                var eod = toDate.Value.Date.AddDays(1).AddTicks(-1);
                q = q.Where(b => b.CreatedAt <= eod);
            }

            var list = q.OrderByDescending(b => b.CreatedAt).ToList();

            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Bugs");
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Başlık";
                ws.Cell(1, 3).Value = "Açıklama";
                ws.Cell(1, 4).Value = "Durum";
                ws.Cell(1, 5).Value = "Öncelik";
                ws.Cell(1, 6).Value = "Kategori";
                ws.Cell(1, 7).Value = "Oluşturulma";

                int row = 2;
                foreach (var b in list)
                {
                    ws.Cell(row, 1).Value = b.Id;
                    ws.Cell(row, 2).Value = b.Title;
                    ws.Cell(row, 3).Value = b.Description;
                    ws.Cell(row, 4).Value = b.Status;
                    ws.Cell(row, 5).Value = b.Priority;
                    ws.Cell(row, 6).Value = b.Category != null ? b.Category.Name : "";
                    ws.Cell(row, 7).Value = b.CreatedAt;
                    row++;
                }

                ws.Columns().AdjustToContents();

                using (var ms = new System.IO.MemoryStream())
                {
                    wb.SaveAs(ms);
                    return File(ms.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Bugs_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
                }
            }
        }

        // ------------------------ (Opsiyonel) Rotativa PDF ------------------------
        public ActionResult ExportPdf(string keyword, string status, string priority, int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            var q = db.Bugs.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(b => b.Title.Contains(keyword) || b.Description.Contains(keyword));
            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(b => b.Status == status);
            if (!string.IsNullOrWhiteSpace(priority))
                q = q.Where(b => b.Priority == priority);
            if (categoryId.HasValue)
                q = q.Where(b => b.CategoryId == categoryId.Value);
            if (fromDate.HasValue)
                q = q.Where(b => b.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
            {
                var eod = toDate.Value.Date.AddDays(1).AddTicks(-1);
                q = q.Where(b => b.CreatedAt <= eod);
            }

            var data = q.OrderByDescending(b => b.CreatedAt).ToList();

            return new Rotativa.ViewAsPdf("AllBugsPdf", data)
            {
                FileName = $"Bugs_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                CustomSwitches = "--print-media-type"
            };
        }

        // ------------------------ iTextSharp ile View olmadan PDF ------------------------
        public FileResult ExportPdfDirect(string keyword, string status, string priority, int? categoryId, DateTime? fromDate, DateTime? toDate)
        {
            var q = db.Bugs.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(b => b.Title.Contains(keyword) || b.Description.Contains(keyword));
            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(b => b.Status == status);
            if (!string.IsNullOrWhiteSpace(priority))
                q = q.Where(b => b.Priority == priority);
            if (categoryId.HasValue)
                q = q.Where(b => b.CategoryId == categoryId.Value);
            if (fromDate.HasValue)
                q = q.Where(b => b.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
            {
                var eod = toDate.Value.Date.AddDays(1).AddTicks(-1);
                q = q.Where(b => b.CreatedAt <= eod);
            }

            var list = q.OrderByDescending(b => b.CreatedAt).ToList();

            using (var ms = new System.IO.MemoryStream())
            {
                var doc = new Document(PageSize.A4, 36f, 36f, 36f, 36f);
                var writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Türkçe karakter için font
                var fontPath = Server.MapPath("~/Content/fonts/DejaVuSans.ttf");
                BaseFont bf;
                if (System.IO.File.Exists(fontPath))
                    bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                else
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, "Cp1254", BaseFont.NOT_EMBEDDED); // <-- FIX

                var fTitle = new Font(bf, 14, Font.BOLD);
                var fHeader = new Font(bf, 10, Font.BOLD);
                var fCell = new Font(bf, 10, Font.NORMAL);

                var title = new Paragraph("Hata Listesi", fTitle) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 10f };
                doc.Add(title);
                var sub = new Paragraph("Oluşturulma: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"), fCell) { SpacingAfter = 8f };
                doc.Add(sub);

                var table = new PdfPTable(6) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 2.0f, 3.0f, 1.2f, 1.2f, 1.6f, 1.6f });

                PdfPCell H(string t) => new PdfPCell(new Phrase(t, fHeader))
                {
                    BackgroundColor = new BaseColor(243, 243, 243),
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Padding = 6f
                };
                PdfPCell C(string t, int align = Element.ALIGN_LEFT) => new PdfPCell(new Phrase(t ?? "", fCell))
                {
                    HorizontalAlignment = align,
                    Padding = 6f
                };

                table.AddCell(H("Başlık"));
                table.AddCell(H("Açıklama"));
                table.AddCell(H("Durum"));
                table.AddCell(H("Öncelik"));
                table.AddCell(H("Kategori"));
                table.AddCell(H("Tarih"));

                foreach (var b in list)
                {
                    table.AddCell(C(b.Title));
                    table.AddCell(C(b.Description));
                    table.AddCell(C(b.Status, Element.ALIGN_CENTER));
                    table.AddCell(C(b.Priority, Element.ALIGN_CENTER));
                    table.AddCell(C(b.Category != null ? b.Category.Name : "—"));
                    table.AddCell(C(b.CreatedAt.ToString("dd.MM.yyyy HH:mm"), Element.ALIGN_CENTER));
                }

                if (list.Count == 0)
                {
                    var cell = new PdfPCell(new Phrase("Kayıt bulunamadı.", fCell))
                    {
                        Colspan = 6,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 10f
                    };
                    table.AddCell(cell);
                }

                doc.Add(table);
                doc.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", $"Bugs_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
            }
        }

        // ------------------------ JSON API ------------------------
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
