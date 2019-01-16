namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableDateTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TrackPoints", "Date", c => c.DateTime());
            AlterColumn("dbo.Tracks", "TrackDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tracks", "TrackDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrackPoints", "Date", c => c.DateTime(nullable: false));
        }
    }
}
