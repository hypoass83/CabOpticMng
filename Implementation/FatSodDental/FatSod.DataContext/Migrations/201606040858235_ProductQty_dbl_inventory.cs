namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductQty_dbl_inventory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RptInventories", "ProductQty", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RptInventories", "ProductQty", c => c.Int(nullable: false));
        }
    }
}
