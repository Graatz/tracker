namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DistanceAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tracks", "Distance", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tracks", "Distance");
        }
    }
}
