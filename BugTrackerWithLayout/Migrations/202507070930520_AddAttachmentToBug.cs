namespace BugTrackerWithLayout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttachmentToBug : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bugs", "Attachment", c => c.String());
            AddColumn("dbo.Bugs", "FilePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bugs", "FilePath");
            DropColumn("dbo.Bugs", "Attachment");
        }
    }
}
