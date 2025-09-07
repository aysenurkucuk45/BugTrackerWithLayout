using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using BugTrackerWithLayout.Models;
using BugTrackerWithLayout.ViewModels;
using BugTrackerWithLayout.Security; // PasswordHelper

namespace BugTrackerWithLayout.Controllers
{
    public class AccountController : Controller
    {
        private readonly BugTrackerDbContext db = new BugTrackerDbContext();

        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel vm, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Username ya da Email ile kullanıcıyı bul (ENTITY: Username)
            var user = db.Users
                .FirstOrDefault(u => u.Username == vm.UserOrEmail || u.Email == vm.UserOrEmail);

            // PBKDF2 doğrulaması
            if (user == null || !PasswordHelper.Verify(vm.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı adı/e-posta veya şifre.");
                return View(vm);
            }

            FormsAuthentication.SetAuthCookie(user.Username, vm.RememberMe);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("About", "Home");
        }

        // GET: /Account/Register
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.Avatars = new[] { "Tweety", "Bugs Bunny", "Tom", "Jerry" };
            return View(new RegisterViewModel { Avatar = "Tweety" });
        }

        // POST: /Account/Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            ViewBag.Avatars = new[] { "Tweety", "Bugs Bunny", "Tom", "Jerry" };

            if (!ModelState.IsValid)
                return View(model);

            // Kullanıcı adı ve e-posta tekil kontrolü (ENTITY: Username)
            if (db.Users.Any(u => u.Username == model.UserName))
            {
                ModelState.AddModelError("", "Bu kullanıcı adı zaten mevcut.");
                return View(model);
            }
            if (db.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Bu e-posta zaten kayıtlı.");
                return View(model);
            }

            var user = new User
            {
                Username = model.UserName.Trim(),   // Entity alanı
                Email = model.Email.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(model.DisplayName)
                                ? model.UserName
                                : model.DisplayName.Trim(),
                Avatar = string.IsNullOrEmpty(model.Avatar) ? "Tweety" : model.Avatar,
                Role = "User",
                PasswordHash = PasswordHelper.HashPassword(model.Password)
            };

            db.Users.Add(user);
            db.SaveChanges();

            // Otomatik login
            FormsAuthentication.SetAuthCookie(user.Username, false);
            return RedirectToAction("About", "Home");
        }

        // GET: /Account/UserProfile
        [Authorize]
        [HttpGet]
        public ActionResult UserProfile()
        {
            var username = User.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return HttpNotFound();

            var bugs = db.Bugs
                         .Where(b => b.ReportedBy == username)
                         .OrderByDescending(b => b.CreatedAt)
                         .ToList();

            var model = new UserProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role, // << EKLENDİ
                ProfileImagePath = string.IsNullOrEmpty(user.Avatar)
                                        ? "~/Content/default-profile.png"
                                        : $"~/Content/Images/{user.Avatar}.png",
                Bugs = bugs,
                TotalBugs = bugs.Count,
                OpenBugCount = bugs.Count(b => b.Status == "Açık"),
                InProgressBugCount = bugs.Count(b => b.Status == "Devam Ediyor"),
                ResolvedBugCount = bugs.Count(b => b.Status == "Çözüldü"),
                CommentCount = db.Comments.Count(c => c.Author == username),
                SolutionCount = db.Solutions.Count(s => s.Author == username)
            };

            return View(model);
        }

        // POST: /Account/UpdateProfileImage
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfileImage(string selectedImage)
        {
            if (string.IsNullOrEmpty(selectedImage))
            {
                TempData["Error"] = "Lütfen bir karakter seçin.";
                return RedirectToAction("UserProfile");
            }

            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return HttpNotFound();

            user.Avatar = selectedImage;
            db.SaveChanges();

            TempData["Success"] = "Profil resmi güncellendi.";
            return RedirectToAction("UserProfile");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPassword() => View();

        // POST: /Account/ForgotPassword
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            TempData["Success"] = "Parola sıfırlama bağlantısı (örnek) gönderildi.";
            return RedirectToAction("Login");
        }

        // /Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
