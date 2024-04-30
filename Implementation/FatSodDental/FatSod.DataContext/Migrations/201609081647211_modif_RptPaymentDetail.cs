namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_RptPaymentDetail : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RptPaymentDetails", "RptReceiptID", "dbo.RptReceipts");
            DropIndex("dbo.RptPaymentDetails", new[] { "RptReceiptID" });
            DropColumn("dbo.RptPaymentDetails", "RptReceiptID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RptPaymentDetails", "RptReceiptID", c => c.Int(nullable: false));
            CreateIndex("dbo.RptPaymentDetails", "RptReceiptID");
            AddForeignKey("dbo.RptPaymentDetails", "RptReceiptID", "dbo.RptReceipts", "RptReceiptID");
        }
    }
}
