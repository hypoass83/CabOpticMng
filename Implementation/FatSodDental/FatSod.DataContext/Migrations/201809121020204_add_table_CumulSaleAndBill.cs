namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_CumulSaleAndBill : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CumulSaleAndBills",
                c => new
                    {
                        CumulSaleAndBillID = c.Int(nullable: false, identity: true),
                        VatRate = c.Double(nullable: false),
                        RateReduction = c.Double(nullable: false),
                        RateDiscount = c.Double(nullable: false),
                        Transport = c.Double(nullable: false),
                        SaleDate = c.DateTime(nullable: false),
                        SaleReceiptNumber = c.String(maxLength: 100),
                        BranchID = c.Int(nullable: false),
                        CustomerName = c.String(),
                        CustomerID = c.Int(),
                        isReturn = c.Boolean(nullable: false),
                        OperatorID = c.Int(),
                        OriginSale = c.Int(nullable: false),
                        Remarque = c.String(),
                        MedecinTraitant = c.String(),
                        IsRendezVous = c.Boolean(nullable: false),
                        AwaitingDay = c.Int(),
                        IsProductReveive = c.Boolean(nullable: false),
                        EffectiveReceiveDate = c.DateTime(),
                        IsCustomerRceive = c.Boolean(nullable: false),
                        CustomerDeliverDate = c.DateTime(),
                        PostSOByID = c.Int(),
                        ReceiveSOByID = c.Int(),
                    })
                .PrimaryKey(t => t.CumulSaleAndBillID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .ForeignKey("dbo.Users", t => t.OperatorID)
                .ForeignKey("dbo.Users", t => t.PostSOByID)
                .ForeignKey("dbo.Users", t => t.ReceiveSOByID)
                .Index(t => t.SaleReceiptNumber, unique: true)
                .Index(t => t.BranchID)
                .Index(t => t.CustomerID)
                .Index(t => t.OperatorID)
                .Index(t => t.PostSOByID)
                .Index(t => t.ReceiveSOByID);
            
            CreateTable(
                "dbo.CumulSaleAndBillLines",
                c => new
                    {
                        LineID = c.Int(nullable: false),
                        CumulSaleAndBillID = c.Int(nullable: false),
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
                .ForeignKey("dbo.CumulSaleAndBills", t => t.CumulSaleAndBillID)
                .Index(t => t.LineID)
                .Index(t => t.CumulSaleAndBillID);
            
            AddColumn("dbo.Customers", "IsBillCustomer", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CumulSaleAndBillLines", "CumulSaleAndBillID", "dbo.CumulSaleAndBills");
            DropForeignKey("dbo.CumulSaleAndBillLines", "LineID", "dbo.Lines");
            DropForeignKey("dbo.CumulSaleAndBills", "ReceiveSOByID", "dbo.Users");
            DropForeignKey("dbo.CumulSaleAndBills", "PostSOByID", "dbo.Users");
            DropForeignKey("dbo.CumulSaleAndBills", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.CumulSaleAndBills", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.CumulSaleAndBills", "BranchID", "dbo.Branches");
            DropIndex("dbo.CumulSaleAndBillLines", new[] { "CumulSaleAndBillID" });
            DropIndex("dbo.CumulSaleAndBillLines", new[] { "LineID" });
            DropIndex("dbo.CumulSaleAndBills", new[] { "ReceiveSOByID" });
            DropIndex("dbo.CumulSaleAndBills", new[] { "PostSOByID" });
            DropIndex("dbo.CumulSaleAndBills", new[] { "OperatorID" });
            DropIndex("dbo.CumulSaleAndBills", new[] { "CustomerID" });
            DropIndex("dbo.CumulSaleAndBills", new[] { "BranchID" });
            DropIndex("dbo.CumulSaleAndBills", new[] { "SaleReceiptNumber" });
            DropColumn("dbo.Customers", "IsBillCustomer");
            DropTable("dbo.CumulSaleAndBillLines");
            DropTable("dbo.CumulSaleAndBills");
        }
    }
}
