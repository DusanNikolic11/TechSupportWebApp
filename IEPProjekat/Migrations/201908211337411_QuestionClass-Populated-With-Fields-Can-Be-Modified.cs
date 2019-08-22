namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuestionClassPopulatedWithFieldsCanBeModified : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Questions", "User_Id", "dbo.Users");
            DropIndex("dbo.Questions", new[] { "User_Id" });
            RenameColumn(table: "dbo.Questions", name: "User_Id", newName: "Author_Id");
            AddColumn("dbo.Questions", "Title", c => c.String(nullable: false));
            AddColumn("dbo.Questions", "Text", c => c.String(nullable: false));
            AddColumn("dbo.Questions", "Picture", c => c.String());
            AddColumn("dbo.Questions", "Category", c => c.String(nullable: false));
            AddColumn("dbo.Questions", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Questions", "MyChannel_Id", c => c.Int());
            AddColumn("dbo.Replies", "Question_Id", c => c.Int());
            AlterColumn("dbo.Questions", "Author_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Questions", "Author_Id");
            CreateIndex("dbo.Questions", "MyChannel_Id");
            CreateIndex("dbo.Replies", "Question_Id");
            AddForeignKey("dbo.Questions", "MyChannel_Id", "dbo.Channels", "Id");
            AddForeignKey("dbo.Replies", "Question_Id", "dbo.Questions", "Id");
            AddForeignKey("dbo.Questions", "Author_Id", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Questions", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.Replies", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.Questions", "MyChannel_Id", "dbo.Channels");
            DropIndex("dbo.Replies", new[] { "Question_Id" });
            DropIndex("dbo.Questions", new[] { "MyChannel_Id" });
            DropIndex("dbo.Questions", new[] { "Author_Id" });
            AlterColumn("dbo.Questions", "Author_Id", c => c.Int());
            DropColumn("dbo.Replies", "Question_Id");
            DropColumn("dbo.Questions", "MyChannel_Id");
            DropColumn("dbo.Questions", "Status");
            DropColumn("dbo.Questions", "Category");
            DropColumn("dbo.Questions", "Picture");
            DropColumn("dbo.Questions", "Text");
            DropColumn("dbo.Questions", "Title");
            RenameColumn(table: "dbo.Questions", name: "Author_Id", newName: "User_Id");
            CreateIndex("dbo.Questions", "User_Id");
            AddForeignKey("dbo.Questions", "User_Id", "dbo.Users", "Id");
        }
    }
}
