namespace IEPProjekat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedcategoriestableAndChangedQuestionsToHaveCategoriesAsFieldInsteadOfStringForCategories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Question", "Category_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Question", "Category_Id");
            AddForeignKey("dbo.Question", "Category_Id", "dbo.categories", "Id", cascadeDelete: true);
            DropColumn("dbo.Question", "Category");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Question", "Category", c => c.String(nullable: false));
            DropForeignKey("dbo.Question", "Category_Id", "dbo.categories");
            DropIndex("dbo.Question", new[] { "Category_Id" });
            DropColumn("dbo.Question", "Category_Id");
            DropTable("dbo.categories");
        }
    }
}
