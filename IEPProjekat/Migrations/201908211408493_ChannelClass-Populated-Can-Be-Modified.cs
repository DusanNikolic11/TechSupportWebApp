namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChannelClassPopulatedCanBeModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Channel_Id", c => c.Int());
            AddColumn("dbo.Channel", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Channel", "Price", c => c.Int(nullable: false));
            AddColumn("dbo.Channel", "UserOpener_Id", c => c.Int());
            AddColumn("dbo.Reply", "MyChannel_Id", c => c.Int());
            CreateIndex("dbo.Users", "Channel_Id");
            CreateIndex("dbo.Channel", "UserOpener_Id");
            CreateIndex("dbo.Reply", "MyChannel_Id");
            AddForeignKey("dbo.Users", "Channel_Id", "dbo.Channel", "Id");
            AddForeignKey("dbo.Reply", "MyChannel_Id", "dbo.Channel", "Id");
            AddForeignKey("dbo.Channel", "UserOpener_Id", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Channel", "UserOpener_Id", "dbo.Users");
            DropForeignKey("dbo.Reply", "MyChannel_Id", "dbo.Channel");
            DropForeignKey("dbo.Users", "Channel_Id", "dbo.Channel");
            DropIndex("dbo.Reply", new[] { "MyChannel_Id" });
            DropIndex("dbo.Channel", new[] { "UserOpener_Id" });
            DropIndex("dbo.Users", new[] { "Channel_Id" });
            DropColumn("dbo.Reply", "MyChannel_Id");
            DropColumn("dbo.Channel", "UserOpener_Id");
            DropColumn("dbo.Channel", "Price");
            DropColumn("dbo.Channel", "Status");
            DropColumn("dbo.Users", "Channel_Id");
        }
    }
}
