namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedgradestoaddids : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Grade", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Grade", "Reply_Id", "dbo.Reply");
            DropIndex("dbo.Grade", new[] { "Reply_Id" });
            DropIndex("dbo.Grade", new[] { "User_Id" });
            RenameColumn(table: "dbo.Grade", name: "User_Id", newName: "UserId");
            RenameColumn(table: "dbo.Grade", name: "Reply_Id", newName: "ReplyId");
            AlterColumn("dbo.Grade", "ReplyId", c => c.Int(nullable: false));
            AlterColumn("dbo.Grade", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Grade", new[] { "UserId", "ReplyId" }, unique: true, name: "IX_UserReplyUQ");
            AddForeignKey("dbo.Grade", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Grade", "ReplyId", "dbo.Reply", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Grade", "ReplyId", "dbo.Reply");
            DropForeignKey("dbo.Grade", "UserId", "dbo.Users");
            DropIndex("dbo.Grade", "IX_UserReplyUQ");
            AlterColumn("dbo.Grade", "UserId", c => c.Int());
            AlterColumn("dbo.Grade", "ReplyId", c => c.Int());
            RenameColumn(table: "dbo.Grade", name: "ReplyId", newName: "Reply_Id");
            RenameColumn(table: "dbo.Grade", name: "UserId", newName: "User_Id");
            CreateIndex("dbo.Grade", "User_Id");
            CreateIndex("dbo.Grade", "Reply_Id");
            AddForeignKey("dbo.Grade", "Reply_Id", "dbo.Reply", "Id");
            AddForeignKey("dbo.Grade", "User_Id", "dbo.Users", "Id");
        }
    }
}
