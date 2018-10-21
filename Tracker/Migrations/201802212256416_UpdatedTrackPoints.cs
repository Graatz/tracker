namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedTrackPoints : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrackPoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrackId = c.Int(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tracks", t => t.TrackId, cascadeDelete: true)
                .Index(t => t.TrackId);
            
            AddColumn("dbo.Tracks", "MinLatitude", c => c.Double(nullable: false));
            AddColumn("dbo.Tracks", "MaxLatitude", c => c.Double(nullable: false));
            AddColumn("dbo.Tracks", "MinLongitude", c => c.Double(nullable: false));
            AddColumn("dbo.Tracks", "MaxLongitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrackPoints", "TrackId", "dbo.Tracks");
            DropIndex("dbo.TrackPoints", new[] { "TrackId" });
            DropColumn("dbo.Tracks", "MaxLongitude");
            DropColumn("dbo.Tracks", "MinLongitude");
            DropColumn("dbo.Tracks", "MaxLatitude");
            DropColumn("dbo.Tracks", "MinLatitude");
            DropTable("dbo.TrackPoints");
        }
    }
}
