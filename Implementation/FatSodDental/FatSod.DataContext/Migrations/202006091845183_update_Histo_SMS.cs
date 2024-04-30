namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_Histo_SMS : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HistoSMS", "CashCustomerID", c => c.Int());
            AddColumn("dbo.HistoSMS", "InsuredCustomerID", c => c.Int());
            CreateIndex("dbo.HistoSMS", "CashCustomerID");
            CreateIndex("dbo.HistoSMS", "InsuredCustomerID");
            AddForeignKey("dbo.HistoSMS", "CashCustomerID", "dbo.Customers", "GlobalPersonID");
            AddForeignKey("dbo.HistoSMS", "InsuredCustomerID", "dbo.CustomerOrders", "CustomerOrderID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HistoSMS", "InsuredCustomerID", "dbo.CustomerOrders");
            DropForeignKey("dbo.HistoSMS", "CashCustomerID", "dbo.Customers");
            DropIndex("dbo.HistoSMS", new[] { "InsuredCustomerID" });
            DropIndex("dbo.HistoSMS", new[] { "CashCustomerID" });
            DropColumn("dbo.HistoSMS", "InsuredCustomerID");
            DropColumn("dbo.HistoSMS", "CashCustomerID");
        }
    }
}
