namespace Recipes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contentmapping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contents", "ContentType", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Contents", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contents", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Contents", "ContentType");
        }
    }
}
