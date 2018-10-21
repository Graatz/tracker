namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTrackAvarageSpeed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tracks", "AvarageSpeed", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tracks", "AvarageSpeed");
        }
    }
}
