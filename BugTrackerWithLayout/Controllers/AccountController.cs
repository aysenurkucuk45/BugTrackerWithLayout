using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using BugTrackerWithLayout.Models;
using BugTrackerWithLayout.ViewModels;

public class AccountController : Controller
{
    // GET: /Account/Login
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    // POST: Giriş kontrolü
    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        using (var db = new BugTrackerDbContext())
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Giriş başarılı → oturumu başlat
                FormsAuthentication.SetAuthCookie(user.Username, false);
                return RedirectToAction("About", "Home");

                // Rolüne göre yönlendirme yapılabilir
                

            }
            else
            {
                ViewBag.Error = "Geçersiz kullanıcı adı veya şifre.";
                return View();
            }
        }
    }

    // GET: /Account/Register
    public ActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            using (var db = new BugTrackerDbContext())
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ViewBag.Error = "Bu kullanıcı adı zaten mevcut.";
                    return View();
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role
                };
                db.Users.Add(user);
                db.SaveChanges();

                // ✅ Otomatik giriş
                FormsAuthentication.SetAuthCookie(user.Username, false);

                return RedirectToAction("Index", "Bug");
            }
        }

        return View(model);
    }


    // HESABIM
    [Authorize]
    [Authorize]

    public ActionResult UserProfile()
    {
        var username = User.Identity.Name;

        using (var db = new BugTrackerDbContext())
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return HttpNotFound();

            var bugs = db.Bugs
                         .Where(b => b.ReportedBy == username)
                         .OrderByDescending(b => b.CreatedAt)
                         .ToList();

            var model = new UserProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Bugs = bugs
            };

            return View(model);
        }
    }






    // TEMA
    public ActionResult SetTheme(string theme)
    {
        if (theme == "dark" || theme == "light")
        {
            Session["Theme"] = theme;
        }

        return Redirect(Request.UrlReferrer?.ToString() ?? "/");
    }

    // Şifremi Unuttum - GET
    public ActionResult ForgotPassword()
    {
        return View();
    }

    // Şifremi Unuttum - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            TempData["Success"] = "Parola sıfırlama bağlantısı gönderildi (örnek).";
            return RedirectToAction("Login");
        }

        return View(model);
    }

    // ÇIKIŞ
    public ActionResult Logout()
    {
        FormsAuthentication.SignOut();
        return RedirectToAction("Login");
    }
}
