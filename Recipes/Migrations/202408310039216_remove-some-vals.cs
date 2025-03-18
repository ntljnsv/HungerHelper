namespace Recipes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removesomevals : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Comments", "Content", c => c.String(maxLength: 350));
            AlterColumn("dbo.RecipeIngredients", "Quantity", c => c.String());
            AlterColumn("dbo.RecipeIngredients", "Unit", c => c.String());
            AlterColumn("dbo.Ingredients", "Name", c => c.String());
            AlterColumn("dbo.RecipeSteps", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RecipeSteps", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Ingredients", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.RecipeIngredients", "Unit", c => c.String(nullable: false));
            AlterColumn("dbo.RecipeIngredients", "Quantity", c => c.String(nullable: false));
            AlterColumn("dbo.Comments", "Content", c => c.String(nullable: false, maxLength: 350));
        }
    }
}
