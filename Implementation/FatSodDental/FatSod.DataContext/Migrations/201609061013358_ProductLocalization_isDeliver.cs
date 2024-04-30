namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductLocalization_isDeliver : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductLocalizations", "isDeliver", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductLocalizations", "isDeliver");
        }
    }
}
