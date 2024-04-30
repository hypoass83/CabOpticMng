namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_Entity_RptPaymentDetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RptPaymentDetails",
                c => new
                    {
                        RptPaymentDetailID = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        DepositDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        LineUnitPrice = c.Double(nullable: false),
                        RptReceiptID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RptPaymentDetailID)
                .ForeignKey("dbo.RptReceipts", t => t.RptReceiptID)
                .Index(t => t.RptReceiptID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RptPaymentDetails", "RptReceiptID", "dbo.RptReceipts");
            DropIndex("dbo.RptPaymentDetails", new[] { "RptReceiptID" });
            DropTable("dbo.RptPaymentDetails");
        }
    }
}
