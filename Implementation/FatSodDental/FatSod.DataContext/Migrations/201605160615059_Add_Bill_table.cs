namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Bill_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillDetails",
                c => new
                    {
                        BillDetailID = c.Int(nullable: false, identity: true),
                        BillID = c.Int(nullable: false),
                        DateVente = c.DateTime(nullable: false),
                        DateCommande = c.DateTime(nullable: false),
                        NumeroCommande = c.String(),
                        LineUnitPrice = c.Double(nullable: false),
                        LineQuantity = c.Double(nullable: false),
                        ProductID = c.Int(nullable: false),
                        SaleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BillDetailID)
                .ForeignKey("dbo.Bills", t => t.BillID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .Index(t => t.BillID)
                .Index(t => t.ProductID)
                .Index(t => t.SaleID);
            
            CreateTable(
                "dbo.Bills",
                c => new
                    {
                        BillID = c.Int(nullable: false, identity: true),
                        BillNumber = c.String(maxLength: 100),
                        BillDate = c.DateTime(nullable: false),
                        BeginDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        MontantRemise = c.Double(nullable: false),
                        MontantEscompte = c.Double(nullable: false),
                        Transport = c.Double(nullable: false),
                        TauxTva = c.Double(nullable: false),
                        BalanceBefore = c.Double(nullable: false),
                        TotalDepot = c.Double(nullable: false),
                        IsNegoce = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BillID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .Index(t => t.BillNumber, unique: true)
                .Index(t => t.CustomerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BillDetails", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.BillDetails", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Bills", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.BillDetails", "BillID", "dbo.Bills");
            DropIndex("dbo.Bills", new[] { "CustomerID" });
            DropIndex("dbo.Bills", new[] { "BillNumber" });
            DropIndex("dbo.BillDetails", new[] { "SaleID" });
            DropIndex("dbo.BillDetails", new[] { "ProductID" });
            DropIndex("dbo.BillDetails", new[] { "BillID" });
            DropTable("dbo.Bills");
            DropTable("dbo.BillDetails");
        }
    }
}
