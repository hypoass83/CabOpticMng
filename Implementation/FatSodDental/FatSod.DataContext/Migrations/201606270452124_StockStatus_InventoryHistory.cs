namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StockStatus_InventoryHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryHistorics", "StockStatus", c => c.String(nullable: false, maxLength: 6));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryHistorics", "StockStatus");
        }
    }
}
