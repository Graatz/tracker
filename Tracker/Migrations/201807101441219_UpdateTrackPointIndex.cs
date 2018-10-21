namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTrackPointIndex : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrackPoints", "Index", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrackPoints", "Index");
        }
    }
}
