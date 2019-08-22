namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedQuestionClassAddedDateTimesForCreationAndLock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Question", "CreationTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Question", "LastLockTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Question", "LastLockTime");
            DropColumn("dbo.Question", "CreationTime");
        }
    }
}
