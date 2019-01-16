namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OpenStreetMapsupport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tracks", "ExternalSignature", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tracks", "ExternalSignature");
        }
    }
}
