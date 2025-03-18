namespace Recipes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipegroups : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Recipes", "RecipeGroup_Id", "dbo.RecipeGroups");
            DropIndex("dbo.Recipes", new[] { "RecipeGroup_Id" });
            CreateTable(
                "dbo.RecipeGroupRecipes",
                c => new
                    {
                        RecipeGroup_Id = c.Int(nullable: false),
                        Recipe_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RecipeGroup_Id, t.Recipe_Id })
                .ForeignKey("dbo.RecipeGroups", t => t.RecipeGroup_Id, cascadeDelete: true)
                .ForeignKey("dbo.Recipes", t => t.Recipe_Id, cascadeDelete: true)
                .Index(t => t.RecipeGroup_Id)
                .Index(t => t.Recipe_Id);
            
            DropColumn("dbo.Recipes", "RecipeGroup_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "RecipeGroup_Id", c => c.Int());
            DropForeignKey("dbo.RecipeGroupRecipes", "Recipe_Id", "dbo.Recipes");
            DropForeignKey("dbo.RecipeGroupRecipes", "RecipeGroup_Id", "dbo.RecipeGroups");
            DropIndex("dbo.RecipeGroupRecipes", new[] { "Recipe_Id" });
            DropIndex("dbo.RecipeGroupRecipes", new[] { "RecipeGroup_Id" });
            DropTable("dbo.RecipeGroupRecipes");
            CreateIndex("dbo.Recipes", "RecipeGroup_Id");
            AddForeignKey("dbo.Recipes", "RecipeGroup_Id", "dbo.RecipeGroups", "Id");
        }
    }
}
