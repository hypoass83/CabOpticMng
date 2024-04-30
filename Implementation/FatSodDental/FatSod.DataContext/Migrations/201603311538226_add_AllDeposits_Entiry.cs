namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_AllDeposits_Entiry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AllDeposits",
                c => new
                    {
                        AllDepositID = c.Int(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        AllDepositDate = c.DateTime(nullable: false),
                        PaymentMethodID = c.Int(nullable: false),
                        DeviseID = c.Int(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        Representant = c.String(),
                        AllDepositReference = c.String(),
                        BranchID = c.Int(nullable: false),
                        AllDepositReason = c.String(),
                    })
                .PrimaryKey(t => t.AllDepositID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.PaymentMethods", t => t.PaymentMethodID)
                .Index(t => t.PaymentMethodID)
                .Index(t => t.DeviseID)
                .Index(t => t.CustomerID)
                .Index(t => t.BranchID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AllDeposits", "PaymentMethodID", "dbo.PaymentMethods");
            DropForeignKey("dbo.AllDeposits", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.AllDeposits", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.AllDeposits", "BranchID", "dbo.Branches");
            DropIndex("dbo.AllDeposits", new[] { "BranchID" });
            DropIndex("dbo.AllDeposits", new[] { "CustomerID" });
            DropIndex("dbo.AllDeposits", new[] { "DeviseID" });
            DropIndex("dbo.AllDeposits", new[] { "PaymentMethodID" });
            DropTable("dbo.AllDeposits");
        }
    }
}
