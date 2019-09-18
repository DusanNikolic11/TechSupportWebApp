namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedchannelsclass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Channel", "Moment", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Channel", "Moment");
        }
    }
}
