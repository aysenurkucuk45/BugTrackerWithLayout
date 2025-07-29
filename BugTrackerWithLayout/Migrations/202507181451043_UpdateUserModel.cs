namespace BugTrackerWithLayout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bugs", "Solution", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bugs", "Solution");
        }
    }
}
