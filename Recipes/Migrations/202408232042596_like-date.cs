namespace Recipes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class likedate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Likes", "DateLiked", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Likes", "DateLiked");
        }
    }
}
