namespace BugTrackerWithLayout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Add_User_NewColumns : DbMigration
    {
        public override void Up()
        {
            // Şifre hash'ı için daha güvenli bir uzunluk
            AddColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false, maxLength: 512));

            AddColumn("dbo.Users", "DisplayName", c => c.String(maxLength: 60));
            AddColumn("dbo.Users", "Avatar", c => c.String(maxLength: 20));
            AddColumn("dbo.Users", "Role", c => c.String(maxLength: 20));

            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 120));

            CreateIndex("dbo.Users", "UserName", unique: true, name: "IX_User_UserName");
            CreateIndex("dbo.Users", "Email", unique: true, name: "IX_User_Email");

            // Eski kolonları kaldırıyoruz (varsa)
            DropColumn("dbo.Users", "Password");
            DropColumn("dbo.Users", "ProfileImagePath");
        }

        public override void Down()
        {
            // Geri alma sırası tersine
            AddColumn("dbo.Users", "ProfileImagePath", c => c.String());
            AddColumn("dbo.Users", "Password", c => c.String());

            DropIndex("dbo.Users", "IX_User_Email");
            DropIndex("dbo.Users", "IX_User_UserName");

            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Users", "UserName", c => c.String());

            DropColumn("dbo.Users", "Role");
            DropColumn("dbo.Users", "Avatar");
            DropColumn("dbo.Users", "DisplayName");
            DropColumn("dbo.Users", "PasswordHash");
        }
    }
}
