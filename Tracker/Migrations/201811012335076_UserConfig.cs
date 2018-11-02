namespace Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserConfigs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        TimeSpanDays = c.Int(nullable: false),
                        TimeSpanHours = c.Int(nullable: false),
                        TimeSpanMinutes = c.Int(nullable: false),
                        SearchingDistance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserConfigs", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserConfigs", new[] { "UserId" });
            DropTable("dbo.UserConfigs");
        }
    }
}
