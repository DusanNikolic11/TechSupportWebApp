namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GradeClassPopulatedCanBeModified : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Channels", newName: "Channel");
            RenameTable(name: "dbo.Grades", newName: "Grade");
            RenameTable(name: "dbo.Questions", newName: "Question");
            RenameTable(name: "dbo.Replies", newName: "Reply");
            RenameColumn(table: "dbo.Reply", name: "User_Id", newName: "ReplyAuthor_Id");
            RenameColumn(table: "dbo.Reply", name: "Question_Id", newName: "ReplyToWhichQuestion_Id");
            RenameIndex(table: "dbo.Reply", name: "IX_User_Id", newName: "IX_ReplyAuthor_Id");
            RenameIndex(table: "dbo.Reply", name: "IX_Question_Id", newName: "IX_ReplyToWhichQuestion_Id");
            AddColumn("dbo.Grade", "Value", c => c.Int(nullable: false));
            AddColumn("dbo.Grade", "Reply_Id", c => c.Int());
            AddColumn("dbo.Reply", "Moment", c => c.DateTime(nullable: false));
            AddColumn("dbo.Reply", "PlusGrades", c => c.Int(nullable: false));
            AddColumn("dbo.Reply", "MinusGrades", c => c.Int(nullable: false));
            AddColumn("dbo.Reply", "ReplyToWhichReply_Id", c => c.Int());
            CreateIndex("dbo.Grade", "Reply_Id");
            CreateIndex("dbo.Reply", "ReplyToWhichReply_Id");
            AddForeignKey("dbo.Grade", "Reply_Id", "dbo.Reply", "Id");
            AddForeignKey("dbo.Reply", "ReplyToWhichReply_Id", "dbo.Reply", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reply", "ReplyToWhichReply_Id", "dbo.Reply");
            DropForeignKey("dbo.Grade", "Reply_Id", "dbo.Reply");
            DropIndex("dbo.Reply", new[] { "ReplyToWhichReply_Id" });
            DropIndex("dbo.Grade", new[] { "Reply_Id" });
            DropColumn("dbo.Reply", "ReplyToWhichReply_Id");
            DropColumn("dbo.Reply", "MinusGrades");
            DropColumn("dbo.Reply", "PlusGrades");
            DropColumn("dbo.Reply", "Moment");
            DropColumn("dbo.Grade", "Reply_Id");
            DropColumn("dbo.Grade", "Value");
            RenameIndex(table: "dbo.Reply", name: "IX_ReplyToWhichQuestion_Id", newName: "IX_Question_Id");
            RenameIndex(table: "dbo.Reply", name: "IX_ReplyAuthor_Id", newName: "IX_User_Id");
            RenameColumn(table: "dbo.Reply", name: "ReplyToWhichQuestion_Id", newName: "Question_Id");
            RenameColumn(table: "dbo.Reply", name: "ReplyAuthor_Id", newName: "User_Id");
            RenameTable(name: "dbo.Reply", newName: "Replies");
            RenameTable(name: "dbo.Question", newName: "Questions");
            RenameTable(name: "dbo.Grade", newName: "Grades");
            RenameTable(name: "dbo.Channel", newName: "Channels");
        }
    }
}
