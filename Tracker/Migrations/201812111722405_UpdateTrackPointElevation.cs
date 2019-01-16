namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTrackPointElevation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrackPoints", "Elevation", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrackPoints", "Elevation");
        }
    }
}
