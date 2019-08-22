namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTableDonePossibleModificationsCanBeDone : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Mail = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Tokens = c.Int(nullable: false, defaultValue: 0),
                        Status = c.String(nullable: false),
                        Role = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Channels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Grades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Replies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Replies", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Questions", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Grades", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Channels", "User_Id", "dbo.Users");
            DropIndex("dbo.Replies", new[] { "User_Id" });
            DropIndex("dbo.Questions", new[] { "User_Id" });
            DropIndex("dbo.Grades", new[] { "User_Id" });
            DropIndex("dbo.Channels", new[] { "User_Id" });
            DropTable("dbo.Replies");
            DropTable("dbo.Questions");
            DropTable("dbo.Grades");
            DropTable("dbo.Channels");
            DropTable("dbo.Users");
        }
    }
}
