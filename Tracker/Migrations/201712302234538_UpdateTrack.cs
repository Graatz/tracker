namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTrack : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Tracks", new[] { "User_Id" });
            DropColumn("dbo.Tracks", "UserId");
            RenameColumn(table: "dbo.Tracks", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.Tracks", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Tracks", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tracks", new[] { "UserId" });
            AlterColumn("dbo.Tracks", "UserId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Tracks", name: "UserId", newName: "User_Id");
            AddColumn("dbo.Tracks", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tracks", "User_Id");
        }
    }
}
