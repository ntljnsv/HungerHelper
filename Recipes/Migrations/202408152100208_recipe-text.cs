namespace Recipes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipetext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecipeTexts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Order = c.Int(nullable: false),
                        RecipeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Recipes", t => t.RecipeId, cascadeDelete: true)
                .Index(t => t.RecipeId);
            
            DropColumn("dbo.Recipes", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "Description", c => c.String());
            DropForeignKey("dbo.RecipeTexts", "RecipeId", "dbo.Recipes");
            DropIndex("dbo.RecipeTexts", new[] { "RecipeId" });
            DropTable("dbo.RecipeTexts");
        }
    }
}
