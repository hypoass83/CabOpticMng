namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rptTranfertForm_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RptTransfertForms",
                c => new
                    {
                        RptTransfertFormID = c.Int(nullable: false, identity: true),
                        Ref = c.String(),
                        Title = c.String(),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        BranchAbbreviation = c.String(),
                        CompanyLogo = c.Binary(),
                        TransfertDate = c.DateTime(nullable: false),
                        ProductLabel = c.String(),
                        ProductRef = c.String(),
                        LineUnitPrice = c.Double(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                        SendindBranchCode = c.String(),
                        SendindBranchName = c.String(),
                        ReceivingBranchCode = c.String(),
                        ReceivingBranchName = c.String(),
                        ReceiveAmount = c.Double(nullable: false),
                        TotalAmount = c.Double(nullable: false),
                        Operator = c.String(),
                        DeviseLabel = c.String(),
                        Transport = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.RptTransfertFormID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RptTransfertForms");
        }
    }
}
