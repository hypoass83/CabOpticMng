namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_AuthoriseSales : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuthoriseSales",
                c => new
                    {
                        AuthoriseSaleID = c.Int(nullable: false, identity: true),
                        CompteurFacture = c.Int(nullable: false),
                        SaleDeliver = c.Boolean(nullable: false),
                        VatRate = c.Double(nullable: false),
                        RateReduction = c.Double(nullable: false),
                        RateDiscount = c.Double(nullable: false),
                        Transport = c.Double(nullable: false),
                        SaleDeliveryDate = c.DateTime(nullable: false),
                        SaleDate = c.DateTime(nullable: false),
                        SaleDateHours = c.DateTime(nullable: false),
                        SaleValidate = c.Boolean(nullable: false),
                        PoliceAssurance = c.String(),
                        PaymentDelay = c.Int(nullable: false),
                        Guaranteed = c.Int(nullable: false),
                        Patient = c.String(),
                        DeviseID = c.Int(),
                        IsPaid = c.Boolean(nullable: false),
                        SaleReceiptNumber = c.String(maxLength: 100),
                        BranchID = c.Int(nullable: false),
                        CustomerID = c.Int(),
                        CustomerName = c.String(),
                        isReturn = c.Boolean(nullable: false),
                        StatutSale = c.Int(nullable: false),
                        OperatorID = c.Int(),
                        PostByID = c.Int(),
                        IsSpecialOrder = c.Boolean(nullable: false),
                        Remarque = c.String(),
                        MedecinTraitant = c.String(),
                        IsDelivered = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AuthoriseSaleID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.Users", t => t.OperatorID)
                .ForeignKey("dbo.Users", t => t.PostByID)
                .Index(t => t.DeviseID)
                .Index(t => t.SaleReceiptNumber, unique: true)
                .Index(t => t.BranchID)
                .Index(t => t.CustomerID)
                .Index(t => t.OperatorID)
                .Index(t => t.PostByID);
            
            CreateTable(
                "dbo.AuthoriseSaleLines",
                c => new
                    {
                        LineID = c.Int(nullable: false),
                        AuthoriseSaleID = c.Int(nullable: false),
                        SpecialOrderLineCode = c.String(),
                        marque = c.String(),
                        reference = c.String(),
                        Axis = c.String(),
                        Addition = c.String(),
                        Index = c.String(),
                        LensNumberCylindricalValue = c.String(),
                        LensNumberSphericalValue = c.String(),
                    })
                .PrimaryKey(t => t.LineID)
                .ForeignKey("dbo.Lines", t => t.LineID)
                .ForeignKey("dbo.AuthoriseSales", t => t.AuthoriseSaleID)
                .Index(t => t.LineID)
                .Index(t => t.AuthoriseSaleID);
            
            AddColumn("dbo.Sales", "AuthoriseSaleID", c => c.Int());
            CreateIndex("dbo.Sales", "AuthoriseSaleID");
            AddForeignKey("dbo.Sales", "AuthoriseSaleID", "dbo.AuthoriseSales", "AuthoriseSaleID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuthoriseSaleLines", "AuthoriseSaleID", "dbo.AuthoriseSales");
            DropForeignKey("dbo.AuthoriseSaleLines", "LineID", "dbo.Lines");
            DropForeignKey("dbo.Sales", "AuthoriseSaleID", "dbo.AuthoriseSales");
            DropForeignKey("dbo.AuthoriseSales", "PostByID", "dbo.Users");
            DropForeignKey("dbo.AuthoriseSales", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.AuthoriseSales", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.AuthoriseSales", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.AuthoriseSales", "BranchID", "dbo.Branches");
            DropIndex("dbo.AuthoriseSaleLines", new[] { "AuthoriseSaleID" });
            DropIndex("dbo.AuthoriseSaleLines", new[] { "LineID" });
            DropIndex("dbo.AuthoriseSales", new[] { "PostByID" });
            DropIndex("dbo.AuthoriseSales", new[] { "OperatorID" });
            DropIndex("dbo.AuthoriseSales", new[] { "CustomerID" });
            DropIndex("dbo.AuthoriseSales", new[] { "BranchID" });
            DropIndex("dbo.AuthoriseSales", new[] { "SaleReceiptNumber" });
            DropIndex("dbo.AuthoriseSales", new[] { "DeviseID" });
            DropIndex("dbo.Sales", new[] { "AuthoriseSaleID" });
            DropColumn("dbo.Sales", "AuthoriseSaleID");
            DropTable("dbo.AuthoriseSaleLines");
            DropTable("dbo.AuthoriseSales");
        }
    }
}
