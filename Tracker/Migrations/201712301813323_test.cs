namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tracks", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Tracks", new[] { "User_Id" });
            DropColumn("dbo.Tracks", "UserId");
            DropColumn("dbo.Tracks", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tracks", "User_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Tracks", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tracks", "User_Id");
            AddForeignKey("dbo.Tracks", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
