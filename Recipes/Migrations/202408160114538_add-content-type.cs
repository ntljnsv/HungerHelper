namespace Recipes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcontenttype : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.RecipePhotoes", newName: "Contents");
            DropIndex("dbo.RecipeTexts", new[] { "RecipeId" });
            AddColumn("dbo.Contents", "Order", c => c.Int(nullable: false));
            AddColumn("dbo.Contents", "Text", c => c.String());
            AddColumn("dbo.Contents", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropTable("dbo.RecipeTexts");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Contents", "Discriminator");
            DropColumn("dbo.Contents", "Text");
            DropColumn("dbo.Contents", "Order");
            CreateIndex("dbo.RecipeTexts", "RecipeId");
            RenameTable(name: "dbo.Contents", newName: "RecipePhotoes");
        }
    }
}
