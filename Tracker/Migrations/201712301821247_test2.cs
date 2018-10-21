namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tracks", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.Tracks", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Tracks", "User_Id");
            AddForeignKey("dbo.Tracks", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tracks", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Tracks", new[] { "User_Id" });
            DropColumn("dbo.Tracks", "User_Id");
            DropColumn("dbo.Tracks", "UserId");
        }
    }
}
