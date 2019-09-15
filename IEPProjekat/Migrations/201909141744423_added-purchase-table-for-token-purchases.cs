namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedpurchasetablefortokenpurchases : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Purchases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                        TotalPrice = c.Single(nullable: false),
                        Type = c.String(nullable: false),
                        Moment = c.DateTime(nullable: false),
                        Customer_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Customer_Id, cascadeDelete: true)
                .Index(t => t.Customer_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Purchases", "Customer_Id", "dbo.Users");
            DropIndex("dbo.Purchases", new[] { "Customer_Id" });
            DropTable("dbo.Purchases");
        }
    }
}
