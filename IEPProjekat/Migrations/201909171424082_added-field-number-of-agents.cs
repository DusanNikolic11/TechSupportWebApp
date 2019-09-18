namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedfieldnumberofagents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Channel", "NumberOfAgents", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Channel", "NumberOfAgents");
        }
    }
}
