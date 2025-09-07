namespace BugTrackerWithLayout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_2025_09_02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "BugId", "dbo.Bugs");
            DropForeignKey("dbo.Solutions", "BugId", "dbo.Bugs");
            DropForeignKey("dbo.Bugs", "Project_ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.Bugs", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Bugs", new[] { "CategoryId" });
            DropIndex("dbo.Bugs", new[] { "Project_ProjectId" });
            DropIndex("dbo.Comments", new[] { "BugId" });
            DropIndex("dbo.Solutions", new[] { "BugId" });
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropIndex("dbo.ProjectUsers", new[] { "UserId" });
            DropIndex("dbo.Users", "IX_User_UserName");
            DropIndex("dbo.Users", "IX_User_Email");
            AddColumn("dbo.Comments", "Text", c => c.String(nullable: false));
            AddColumn("dbo.Solutions", "Text", c => c.String(nullable: false));
            AlterColumn("dbo.Bugs", "CategoryId", c => c.Int());
            AlterColumn("dbo.Comments", "Author", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.Solutions", "Author", c => c.String(nullable: false, maxLength: 64));
            CreateIndex("dbo.Bugs", "CategoryId");
            AddForeignKey("dbo.Bugs", "CategoryId", "dbo.Categories", "CategoryId");
            DropColumn("dbo.Bugs", "Project_ProjectId");
            DropColumn("dbo.Comments", "Content");
            DropColumn("dbo.Solutions", "Description");
            DropTable("dbo.Projects");
            DropTable("dbo.ProjectUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProjectUsers",
                c => new
                    {
                        ProjectUserId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectUserId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ProjectId);
            
            AddColumn("dbo.Solutions", "Description", c => c.String());
            AddColumn("dbo.Comments", "Content", c => c.String());
            AddColumn("dbo.Bugs", "Project_ProjectId", c => c.Int());
            DropForeignKey("dbo.Bugs", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Bugs", new[] { "CategoryId" });
            AlterColumn("dbo.Solutions", "Author", c => c.String());
            AlterColumn("dbo.Comments", "Author", c => c.String());
            AlterColumn("dbo.Bugs", "CategoryId", c => c.Int(nullable: false));
            DropColumn("dbo.Solutions", "Text");
            DropColumn("dbo.Comments", "Text");
            CreateIndex("dbo.Users", "Email", unique: true, name: "IX_User_Email");
            CreateIndex("dbo.Users", "UserName", unique: true, name: "IX_User_UserName");
            CreateIndex("dbo.ProjectUsers", "UserId");
            CreateIndex("dbo.ProjectUsers", "ProjectId");
            CreateIndex("dbo.Solutions", "BugId");
            CreateIndex("dbo.Comments", "BugId");
            CreateIndex("dbo.Bugs", "Project_ProjectId");
            CreateIndex("dbo.Bugs", "CategoryId");
            AddForeignKey("dbo.Bugs", "CategoryId", "dbo.Categories", "CategoryId", cascadeDelete: true);
            AddForeignKey("dbo.ProjectUsers", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects", "ProjectId", cascadeDelete: true);
            AddForeignKey("dbo.Bugs", "Project_ProjectId", "dbo.Projects", "ProjectId");
            AddForeignKey("dbo.Solutions", "BugId", "dbo.Bugs", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Comments", "BugId", "dbo.Bugs", "Id", cascadeDelete: true);
        }
    }
}
