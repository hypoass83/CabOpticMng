namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Stock_price_InventoryDirectoryLine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryDirectoryLines", "AveragePurchasePrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryDirectoryLines", "AveragePurchasePrice");
        }
    }
}
