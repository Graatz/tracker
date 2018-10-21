namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrackUpdated3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tracks", "StartLocation", c => c.String());
            AddColumn("dbo.Tracks", "EndLocation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tracks", "EndLocation");
            DropColumn("dbo.Tracks", "StartLocation");
        }
    }
}
