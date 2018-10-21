namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrackTableUpdateStrava : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tracks", "ExternalId", c => c.Long(nullable: false));
            AddColumn("dbo.Tracks", "TrackDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tracks", "TrackDate");
            DropColumn("dbo.Tracks", "ExternalId");
        }
    }
}
