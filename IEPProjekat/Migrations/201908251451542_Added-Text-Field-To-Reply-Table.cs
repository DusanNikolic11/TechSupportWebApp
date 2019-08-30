namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTextFieldToReplyTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reply", "Text", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reply", "Text");
        }
    }
}
