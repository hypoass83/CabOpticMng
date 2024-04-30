namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_table_cumulsaleandbill_PurchasePrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBillLines", "PurchaseLineUnitPrice", c => c.Double(nullable: false));
            AddColumn("dbo.Lines", "isCommandGlass", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lines", "isCommandGlass");
            DropColumn("dbo.CumulSaleAndBillLines", "PurchaseLineUnitPrice");
        }
    }
}
