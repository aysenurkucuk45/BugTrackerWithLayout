using BugTrackerWithLayout.Models;
using BugTrackerWithLayout.ViewModels;
using System.Linq;
using System.Web.Mvc;
using System.IO;

namespace BugTrackerWithLayout.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly BugTrackerDbContext db = new BugTrackerDbContext();

        // GET: /User/UserProfile
        public ActionResult UserProfile()
        {
            var currentUser = User.Identity.Name;

            // EF sorgusunda NotMapped olan UserName değil, Username kullan
            var user = db.Users.FirstOrDefault(u => u.Username == currentUser);
            if (user == null) return HttpNotFound();

            var bugs = db.Bugs.Where(b => b.ReportedBy == currentUser).ToList();
            var comments = db.Comments.Where(c => c.Author == currentUser).ToList();
            var solutions = db.Solutions.Where(s => s.Author == currentUser).ToList();

            var avatar = string.IsNullOrWhiteSpace(user.Avatar) ? "Tom" : user.Avatar;
            if (avatar != "Tom" && avatar != "Jerry") avatar = "Tom";

            // Klasör: Content/Image  |  Dosyalar: tom.png, Jerry.png
            var localVirtualPath = avatar == "Tom"
                ? "~/Content/Image/tom.png"
                : "~/Content/Image/Jerry.png";

            var vm = new UserProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,

                Avatar = avatar,
                ProfileImagePath = localVirtualPath,

                TotalBugs = bugs.Count,
                OpenBugCount = bugs.Count(b => b.Status == "Açık"),
                InProgressBugCount = bugs.Count(b => b.Status == "Devam Ediyor"),
                ResolvedBugCount = bugs.Count(b => b.Status == "Çözüldü"),
                CommentCount = comments.Count,
                SolutionCount = solutions.Count,
                Bugs = bugs
            };

            // >>> BURASI DEĞİŞTİ: View'i Account klasöründen yükle
            return View("~/Views/Account/UserProfile.cshtml", vm);
        }

        // POST: /User/UpdateProfileImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfileImage(string selectedImage)
        {
            if (selectedImage != "Tom" && selectedImage != "Jerry")
            {
                TempData["Error"] = "Lütfen Tom veya Jerry seçin.";
                return RedirectToAction("UserProfile");
            }

            var currentUser = User.Identity.Name;

            // EF sorgusunda Username kullan
            var user = db.Users.FirstOrDefault(u => u.Username == currentUser);
            if (user == null) return HttpNotFound();

            user.Avatar = selectedImage;   // "Tom" | "Jerry"
            db.SaveChanges();

            TempData["Success"] = "Profil resmi güncellendi.";
            return RedirectToAction("UserProfile");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
