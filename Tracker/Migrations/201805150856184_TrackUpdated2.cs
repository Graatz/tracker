namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrackUpdated2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tracks", "Name", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Tracks", "Description", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tracks", "Description", c => c.String());
            AlterColumn("dbo.Tracks", "Name", c => c.String());
        }
    }
}
