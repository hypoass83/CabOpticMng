namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_OrderControllerId_To_CumulSaleAndBill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBills", "OrderControllerId", c => c.Int());
            CreateIndex("dbo.CumulSaleAndBills", "OrderControllerId");
            AddForeignKey("dbo.CumulSaleAndBills", "OrderControllerId", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CumulSaleAndBills", "OrderControllerId", "dbo.Users");
            DropIndex("dbo.CumulSaleAndBills", new[] { "OrderControllerId" });
            DropColumn("dbo.CumulSaleAndBills", "OrderControllerId");
        }
    }
}
