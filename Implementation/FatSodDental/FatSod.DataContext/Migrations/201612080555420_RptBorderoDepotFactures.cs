namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RptBorderoDepotFactures : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RptBorderoDepotFactures",
                c => new
                    {
                        RptBorderoDepotFactureID = c.Int(nullable: false, identity: true),
                        CustomerOrderID = c.Int(nullable: false),
                        BranchID = c.Int(nullable: false),
                        UIBranchCode = c.String(maxLength: 100),
                        CustomerName = c.String(maxLength: 100),
                        CompanyName = c.String(maxLength: 100),
                        CustomerOrderNumber = c.String(maxLength: 50),
                        NumeroBonPriseEnCharge = c.String(maxLength: 100),
                        CustomerOrderDate = c.DateTime(nullable: false),
                        NumeroFacture = c.String(maxLength: 30),
                        PhoneNumber = c.String(maxLength: 30),
                        MntAssureur = c.Double(nullable: false),
                        ReductionAmount = c.Double(nullable: false),
                        LogoBranch = c.Binary(),
                    })
                .PrimaryKey(t => t.RptBorderoDepotFactureID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RptBorderoDepotFactures");
        }
    }
}
