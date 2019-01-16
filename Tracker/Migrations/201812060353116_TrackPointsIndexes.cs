namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrackPointsIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.TrackPoints", "Latitude", name: "LatitudeIndex");
            CreateIndex("dbo.TrackPoints", "Longitude", name: "LongitudeIndex");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TrackPoints", "LongitudeIndex");
            DropIndex("dbo.TrackPoints", "LatitudeIndex");
        }
    }
}
