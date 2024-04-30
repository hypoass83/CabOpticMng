namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Prescription_detail_Product : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrderLines", "ProductCategoryCode", c => c.String());
            AddColumn("dbo.CustomerOrderLines", "Prescription", c => c.String());
            AddColumn("dbo.Products", "ProductCategoryCode", c => c.String());
            AddColumn("dbo.Products", "Prescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Prescription");
            DropColumn("dbo.Products", "ProductCategoryCode");
            DropColumn("dbo.CustomerOrderLines", "Prescription");
            DropColumn("dbo.CustomerOrderLines", "ProductCategoryCode");
        }
    }
}
