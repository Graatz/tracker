namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrackUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tracks", "Description", c => c.String());
            AddColumn("dbo.Tracks", "UploadDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tracks", "UploadDate");
            DropColumn("dbo.Tracks", "Description");
        }
    }
}
