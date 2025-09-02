namespace BugTrackerWithLayout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bugs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        Priority = c.String(),
                        Status = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        ReportedBy = c.String(),
                        CategoryId = c.Int(nullable: false),
                        Attachment = c.String(),
                        FilePath = c.String(),
                        Solution = c.String(),
                        Project_ProjectId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.Project_ProjectId)
                .Index(t => t.CategoryId)
                .Index(t => t.Project_ProjectId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        BugId = c.Int(nullable: false),
                        Author = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bugs", t => t.BugId, cascadeDelete: true)
                .Index(t => t.BugId);
            
            CreateTable(
                "dbo.Solutions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Author = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        BugId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bugs", t => t.BugId, cascadeDelete: true)
                .Index(t => t.BugId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ProjectId);
            
            CreateTable(
                "dbo.ProjectUsers",
                c => new
                    {
                        ProjectUserId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectUserId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        Role = c.String(),
                        ProfileImagePath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Bugs", "Project_ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Solutions", "BugId", "dbo.Bugs");
            DropForeignKey("dbo.Comments", "BugId", "dbo.Bugs");
            DropForeignKey("dbo.Bugs", "CategoryId", "dbo.Categories");
            DropIndex("dbo.ProjectUsers", new[] { "UserId" });
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropIndex("dbo.Solutions", new[] { "BugId" });
            DropIndex("dbo.Comments", new[] { "BugId" });
            DropIndex("dbo.Bugs", new[] { "Project_ProjectId" });
            DropIndex("dbo.Bugs", new[] { "CategoryId" });
            DropTable("dbo.Users");
            DropTable("dbo.ProjectUsers");
            DropTable("dbo.Projects");
            DropTable("dbo.Solutions");
            DropTable("dbo.Comments");
            DropTable("dbo.Categories");
            DropTable("dbo.Bugs");
        }
    }
}
