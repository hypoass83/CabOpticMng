namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class table_RptbudgetExpense : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RptbudgetExpenses",
                c => new
                    {
                        RptbudgetExpenseID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        Devise = c.String(),
                        LibDevise = c.String(maxLength: 100),
                        UIBudgetAllocated = c.String(maxLength: 100),
                        PaymentMethodId = c.Int(nullable: false),
                        VoucherAmount = c.Double(nullable: false),
                        DateOperation = c.DateTime(nullable: false),
                        Reference = c.String(maxLength: 30),
                        BeneficiaryName = c.String(maxLength: 100),
                        Justification = c.String(maxLength: 100),
                        BudgetConsumptionID = c.Int(nullable: false),
                        PaymentDate = c.DateTime(nullable: false),
                        CompanyName = c.String(maxLength: 255),
                        RegionCountry = c.String(maxLength: 255),
                        Telephone = c.String(maxLength: 30),
                        Fax = c.String(maxLength: 30),
                        Adresse = c.String(maxLength: 255),
                        LogoBranch = c.Binary(),
                    })
                .PrimaryKey(t => t.RptbudgetExpenseID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RptbudgetExpenses");
        }
    }
}
