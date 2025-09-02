namespace BugTrackerWithLayout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfileImagePathFix : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "ProfileImage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "ProfileImage", c => c.String());
        }
    }
}
