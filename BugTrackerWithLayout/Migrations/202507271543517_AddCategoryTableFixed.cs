namespace BugTrackerWithLayout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategoryTableFixed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            AddColumn("dbo.Bugs", "CategoryId", c => c.Int());
            CreateIndex("dbo.Bugs", "CategoryId");
            AddForeignKey("dbo.Bugs", "CategoryId", "dbo.Categories", "CategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bugs", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Bugs", new[] { "CategoryId" });
            DropColumn("dbo.Bugs", "CategoryId");
            DropTable("dbo.Categories");
        }
    }
}
