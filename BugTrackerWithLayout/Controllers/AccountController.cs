using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using BugTrackerWithLayout.Models;
using BugTrackerWithLayout.Security;
using BugTrackerWithLayout.ViewModels;

// ViewModel alias'ları (adı çakışmalarını önler)
using LoginVM = BugTrackerWithLayout.ViewModels.LoginViewModel;
using RegisterVM = BugTrackerWithLayout.ViewModels.RegisterViewModel;

namespace BugTrackerWithLayout.Controllers
{
    public class AccountController : Controller
    {
        private readonly BugTrackerDbContext db = new BugTrackerDbContext();

        // ------ LOGIN ------
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginVM());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM vm, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Kullanıcı adı / e-posta için küçük/büyük harf duyarsız arama
            var user = db.Users.FirstOrDefault(u =>
                u.UserName.Equals(vm.UserOrEmail, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Equals(vm.UserOrEmail, StringComparison.OrdinalIgnoreCase));

            if (user == null || !PasswordHelper.Verify(vm.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı adı/e-posta veya parola.");
                return View(vm);
            }

            FormsAuthentication.SetAuthCookie(user.UserName, vm.RememberMe);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Daha doğal yönlendirme
            return RedirectToAction("Index", "Home");
        }

        // ------ REGISTER ------
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.Avatars = new[] { "Tweety", "Bugs Bunny", "Tom", "Jerry" };
            return View(new RegisterVM { Avatar = "Tweety" });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            ViewBag.Avatars = new[] { "Tweety", "Bugs Bunny", "Tom", "Jerry" };

            if (!ModelState.IsValid)
                return View(model);

            if (db.Users.Any(u => u.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("", "Bu kullanıcı adı zaten mevcut.");
                return View(model);
            }

            if (db.Users.Any(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("", "Bu e-posta zaten kayıtlı.");
                return View(model);
            }

            // Avatar doğrulama
            var allowedAvatars = new[] { "Tweety", "Bugs Bunny", "Tom", "Jerry" };
            var avatar = allowedAvatars.Contains(model.Avatar) ? model.Avatar : "Tweety";

            var user = new User
            {
                UserName = model.UserName.Trim(),
                Email = model.Email.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(model.DisplayName)
                                ? model.UserName
                                : model.DisplayName.Trim(),
                Avatar = avatar,
                Role = "User",
                PasswordHash = PasswordHelper.HashPassword(model.Password)
            };

            db.Users.Add(user);
            db.SaveChanges();

            FormsAuthentication.SetAuthCookie(user.UserName, false);
            return RedirectToAction("Index", "Home");
        }

        // ------ PROFILE ------
        [Authorize]
        [HttpGet]
        public ActionResult UserProfile()
        {
            var username = User.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null) return HttpNotFound();

            var userBugs = db.Bugs
                .Where(b => b.ReportedBy == username)
                .ToList();

            var model = new UserProfileViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                ProfileImagePath = user.Avatar, // avatar adı; view tarafında uygun görsel eşle
                Bugs = userBugs.OrderByDescending(b => b.CreatedAt).ToList(),
                TotalBugs = userBugs.Count,
                OpenBugCount = userBugs.Count(b => b.Status == "Açık"),
                InProgressBugCount = userBugs.Count(b => b.Status == "Devam Ediyor"),
                ResolvedBugCount = userBugs.Count(b => b.Status == "Çözüldü"),
                CommentCount = userBugs.Sum(b => b.Comments != null ? b.Comments.Count : 0),
                SolutionCount = userBugs.Sum(b => b.Solutions != null ? b.Solutions.Count : 0)
            };

            return View(model);
        }

        // ------ FORGOT PASSWORD (örnek) ------
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPassword() => View();

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            TempData["Success"] = "Parola sıfırlama bağlantısı (örnek) gönderildi.";
            return RedirectToAction("Login");
        }

        // ------ LOGOUT ------
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        // ------ Dispose ------
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
