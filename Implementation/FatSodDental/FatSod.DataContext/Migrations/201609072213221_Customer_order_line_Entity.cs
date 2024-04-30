namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer_order_line_Entity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerOrders",
                c => new
                    {
                        CustomerOrderID = c.Int(nullable: false, identity: true),
                        CustomerOrderDate = c.DateTime(nullable: false),
                        VatRate = c.Double(nullable: false),
                        RateReduction = c.Double(nullable: false),
                        RateDiscount = c.Double(nullable: false),
                        Patient = c.String(),
                        CustomerOrderNumber = c.String(maxLength: 100),
                        IsDelivered = c.Boolean(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        DeviseID = c.Int(nullable: false),
                        BranchID = c.Int(nullable: false),
                        OperatorID = c.Int(nullable: false),
                        Transport = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerOrderID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .ForeignKey("dbo.Devises", t => t.DeviseID)
                .ForeignKey("dbo.Users", t => t.OperatorID)
                .Index(t => t.CustomerOrderNumber, unique: true)
                .Index(t => t.CustomerID)
                .Index(t => t.DeviseID)
                .Index(t => t.BranchID)
                .Index(t => t.OperatorID);
            
            CreateTable(
                "dbo.CustomerOrderLines",
                c => new
                    {
                        LineID = c.Int(nullable: false),
                        CustomerOrderID = c.Int(nullable: false),
                        SpecialOrderLineCode = c.String(),
                        marque = c.String(),
                        reference = c.String(),
                    })
                .PrimaryKey(t => t.LineID)
                .ForeignKey("dbo.Lines", t => t.LineID)
                .ForeignKey("dbo.CustomerOrders", t => t.CustomerOrderID)
                .Index(t => t.LineID)
                .Index(t => t.CustomerOrderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerOrderLines", "CustomerOrderID", "dbo.CustomerOrders");
            DropForeignKey("dbo.CustomerOrderLines", "LineID", "dbo.Lines");
            DropForeignKey("dbo.CustomerOrders", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.CustomerOrders", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.CustomerOrders", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.CustomerOrders", "BranchID", "dbo.Branches");
            DropIndex("dbo.CustomerOrderLines", new[] { "CustomerOrderID" });
            DropIndex("dbo.CustomerOrderLines", new[] { "LineID" });
            DropIndex("dbo.CustomerOrders", new[] { "OperatorID" });
            DropIndex("dbo.CustomerOrders", new[] { "BranchID" });
            DropIndex("dbo.CustomerOrders", new[] { "DeviseID" });
            DropIndex("dbo.CustomerOrders", new[] { "CustomerID" });
            DropIndex("dbo.CustomerOrders", new[] { "CustomerOrderNumber" });
            DropTable("dbo.CustomerOrderLines");
            DropTable("dbo.CustomerOrders");
        }
    }
}
