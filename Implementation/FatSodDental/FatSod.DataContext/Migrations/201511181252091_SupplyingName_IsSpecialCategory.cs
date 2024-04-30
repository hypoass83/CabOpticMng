namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SupplyingName_IsSpecialCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrderLines", "SupplyingName", c => c.String());
            AddColumn("dbo.LensCategories", "IsSpecialCategory", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LensCategories", "IsSpecialCategory");
            DropColumn("dbo.CustomerOrderLines", "SupplyingName");
        }
    }
}
