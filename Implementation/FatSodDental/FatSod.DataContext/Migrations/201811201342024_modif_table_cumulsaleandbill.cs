namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_table_cumulsaleandbill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBillLines", "ProductCategoryID", c => c.Int(nullable: false));
            AddColumn("dbo.CumulSaleAndBills", "IsDeliver", c => c.Boolean(nullable: false));
            AddColumn("dbo.CumulSaleAndBills", "DeliverDate", c => c.DateTime());
            AddColumn("dbo.CumulSaleAndBills", "DeliverByID", c => c.Int());
            AlterColumn("dbo.Sales", "SaleDeliveryDate", c => c.DateTime());
            CreateIndex("dbo.CumulSaleAndBills", "DeliverByID");
            AddForeignKey("dbo.CumulSaleAndBills", "DeliverByID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CumulSaleAndBills", "DeliverByID", "dbo.Users");
            DropIndex("dbo.CumulSaleAndBills", new[] { "DeliverByID" });
            AlterColumn("dbo.Sales", "SaleDeliveryDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.CumulSaleAndBills", "DeliverByID");
            DropColumn("dbo.CumulSaleAndBills", "DeliverDate");
            DropColumn("dbo.CumulSaleAndBills", "IsDeliver");
            DropColumn("dbo.CumulSaleAndBillLines", "ProductCategoryID");
        }
    }
}
