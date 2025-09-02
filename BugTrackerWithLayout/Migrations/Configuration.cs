namespace BugTrackerWithLayout.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using BugTrackerWithLayout.Models;
    using BugTrackerWithLayout.Security; // PasswordHelper için

    internal sealed class Configuration : DbMigrationsConfiguration<BugTrackerWithLayout.Models.BugTrackerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BugTrackerWithLayout.Models.BugTrackerDbContext context)
        {
            // ---- ONE-TIME FIX: Eksik veya hatalı kullanıcıları düzelt ----
            var needFix = context.Users
                                 .Where(u => u.PasswordHash == null || u.PasswordHash == "")
                                 .ToList();

            foreach (var u in needFix)
            {
                if (string.IsNullOrWhiteSpace(u.DisplayName))
                    u.DisplayName = u.UserName;

                if (string.IsNullOrEmpty(u.Avatar))
                    u.Avatar = "Tweety";

                if (string.IsNullOrEmpty(u.Role))
                    u.Role = "User";

                // Geçici parola: Reset1234!
                u.PasswordHash = PasswordHelper.HashPassword("Reset1234!");
            }

            // ---- Eksik kullanıcıları oluştur ----
            if (!context.Users.Any(u => u.UserName == "kaankck45"))
            {
                context.Users.Add(new User
                {
                    UserName = "kaankck45",
                    Email = "mehmetkaan@example.com",
                    DisplayName = "Kaan",
                    Avatar = "Tweety",
                    Role = "User",
                    PasswordHash = PasswordHelper.HashPassword("Reset1234!")
                });
            }

            if (!context.Users.Any(u => u.UserName == "aysenurkucuk45"))
            {
                context.Users.Add(new User
                {
                    UserName = "aysenurkucuk45",
                    Email = "aysenur@example.com",
                    DisplayName = "Ayşenur",
                    Avatar = "Jerry",
                    Role = "User",
                    PasswordHash = PasswordHelper.HashPassword("Reset1234!")
                });
            }

            context.SaveChanges();
            // ---- /ONE-TIME FIX ----
        }
    }
}
