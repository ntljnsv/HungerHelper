namespace Recipes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class categoriesandcollections : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecipeGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Recipes", "Category", c => c.String());
            AddColumn("dbo.Recipes", "RecipeGroup_Id", c => c.Int());
            CreateIndex("dbo.Recipes", "RecipeGroup_Id");
            AddForeignKey("dbo.Recipes", "RecipeGroup_Id", "dbo.RecipeGroups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecipeGroups", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Recipes", "RecipeGroup_Id", "dbo.RecipeGroups");
            DropIndex("dbo.RecipeGroups", new[] { "UserId" });
            DropIndex("dbo.Recipes", new[] { "RecipeGroup_Id" });
            DropColumn("dbo.Recipes", "RecipeGroup_Id");
            DropColumn("dbo.Recipes", "Category");
            DropTable("dbo.RecipeGroups");
        }
    }
}
