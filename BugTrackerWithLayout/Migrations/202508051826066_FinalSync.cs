namespace BugTrackerWithLayout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinalSync : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ProfileImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ProfileImage");
        }
    }
}
