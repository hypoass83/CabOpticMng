namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_SaleIDAndCustomerorderID_CumulSaleAndBill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBills", "SaleID", c => c.Int());
            AddColumn("dbo.CumulSaleAndBills", "CustomerOrderID", c => c.Int());
            CreateIndex("dbo.CumulSaleAndBills", "SaleID");
            CreateIndex("dbo.CumulSaleAndBills", "CustomerOrderID");
            AddForeignKey("dbo.CumulSaleAndBills", "CustomerOrderID", "dbo.CustomerOrders", "CustomerOrderID");
            AddForeignKey("dbo.CumulSaleAndBills", "SaleID", "dbo.Sales", "SaleID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CumulSaleAndBills", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.CumulSaleAndBills", "CustomerOrderID", "dbo.CustomerOrders");
            DropIndex("dbo.CumulSaleAndBills", new[] { "CustomerOrderID" });
            DropIndex("dbo.CumulSaleAndBills", new[] { "SaleID" });
            DropColumn("dbo.CumulSaleAndBills", "CustomerOrderID");
            DropColumn("dbo.CumulSaleAndBills", "SaleID");
        }
    }
}
