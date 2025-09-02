using BugTrackerWithLayout.Models;
using BugTrackerWithLayout.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace BugTrackerWithLayout.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private BugTrackerDbContext db = new BugTrackerDbContext();

        // GET: /User/UserProfile
        public ActionResult UserProfile()
        {
            var currentUser = User.Identity.Name;

            // Username -> UserName
            var user = db.Users.FirstOrDefault(u => u.UserName == currentUser);
            if (user == null)
                return HttpNotFound();

            var bugs = db.Bugs.Where(b => b.ReportedBy == currentUser).ToList();
            var comments = db.Comments.Where(c => c.Author == currentUser).ToList();
            var solutions = db.Solutions.Where(s => s.Author == currentUser).ToList();

            var viewModel = new UserProfileViewModel
            {
                // Username -> UserName
                Username = user.UserName,
                Email = user.Email,

                // ProfileImagePath -> Avatar (bizim User modelindeki alan bu)
                ProfileImagePath = string.IsNullOrEmpty(user.Avatar)
                    ? "~/Content/default-profile.png"
                    : $"~/Content/Images/{user.Avatar}.png",

                TotalBugs = bugs.Count(),
                OpenBugCount = bugs.Count(b => b.Status == "Açık"),
                InProgressBugCount = bugs.Count(b => b.Status == "Devam Ediyor"),
                ResolvedBugCount = bugs.Count(b => b.Status == "Çözüldü"),

                CommentCount = comments.Count(),
                SolutionCount = solutions.Count(),

                Bugs = bugs
            };

            return View(viewModel);
        }

        // POST: /User/UpdateProfileImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfileImage(string selectedImage)
        {
            if (string.IsNullOrEmpty(selectedImage))
            {
                TempData["Error"] = "Lütfen bir karakter seçin.";
                return RedirectToAction("UserProfile");
            }

            var currentUser = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.UserName == currentUser); // Username -> UserName

            if (user == null)
                return HttpNotFound();

            // Profil resmini güncelle -> Avatar alanı
            user.Avatar = selectedImage; // sadece karakter adını tutuyoruz (Tweety, Tom vs.)
            db.SaveChanges();

            TempData["Success"] = "Profil resmi başarıyla güncellendi.";
            return RedirectToAction("UserProfile");
        }
    }
}
