namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_RptPaymentDetail_FK : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RptPaymentDetails", "RptReceiptPaymentDetailID", c => c.Int(nullable: false));
            AddColumn("dbo.RptReceipts", "RptReceiptPaymentDetailID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RptReceipts", "RptReceiptPaymentDetailID");
            DropColumn("dbo.RptPaymentDetails", "RptReceiptPaymentDetailID");
        }
    }
}
