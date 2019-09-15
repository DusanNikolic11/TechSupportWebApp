namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingSystemSetupClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Setup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PriceSilver = c.Single(nullable: false),
                        PriceGold = c.Single(nullable: false),
                        PricePlat = c.Single(nullable: false),
                        AmountSilver = c.Int(nullable: false),
                        AmountGold = c.Int(nullable: false),
                        AmountPlat = c.Int(nullable: false),
                        ChannelAmountSilver = c.Int(nullable: false),
                        ChannelAmountGold = c.Int(nullable: false),
                        ChannelAmountPlat = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Setup");
        }
    }
}
