namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_customer_order_add_assure : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerOrders", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.CustomerOrders", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.CustomerOrders", "DeviseID", "dbo.Devises");
            DropForeignKey("dbo.CustomerOrders", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.CustomerOrderLines", "LineID", "dbo.Lines");
            DropForeignKey("dbo.CustomerOrderLines", "CustomerOrderID", "dbo.CustomerOrders");
            DropIndex("dbo.CustomerOrders", new[] { "CustomerOrderNumber" });
            DropIndex("dbo.CustomerOrders", new[] { "CustomerID" });
            DropIndex("dbo.CustomerOrders", new[] { "DeviseID" });
            DropIndex("dbo.CustomerOrders", new[] { "BranchID" });
            DropIndex("dbo.CustomerOrders", new[] { "OperatorID" });
            DropIndex("dbo.CustomerOrderLines", new[] { "LineID" });
            DropIndex("dbo.CustomerOrderLines", new[] { "CustomerOrderID" });
            CreateTable(
                "dbo.Assureurs",
                c => new
                    {
                        GlobalPersonID = c.Int(nullable: false),
                        AssureurID = c.Int(nullable: false),
                        AssureurNumber = c.String(maxLength: 250),
                        CompanyCapital = c.Int(nullable: false),
                        CompanySigle = c.String(),
                        CompanyTradeRegister = c.String(),
                        CompanySlogan = c.String(),
                        CompanyIsDeletable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.GlobalPersonID)
                .ForeignKey("dbo.People", t => t.GlobalPersonID)
                .Index(t => t.GlobalPersonID)
                .Index(t => t.AssureurNumber, unique: true, name: "AssureurNumber");
            
            AddColumn("dbo.Customers", "PoliceAssurance", c => c.String(maxLength: 250));
            AddColumn("dbo.Customers", "AssureurID", c => c.Int(nullable: false));
            CreateIndex("dbo.Customers", "AssureurID");
            AddForeignKey("dbo.Customers", "AssureurID", "dbo.Assureurs", "GlobalPersonID");
            DropColumn("dbo.Customers", "CompanyCapital");
            DropColumn("dbo.Customers", "CompanySigle");
            DropColumn("dbo.Customers", "CompanyTradeRegister");
            DropColumn("dbo.Customers", "CompanySlogan");
            DropColumn("dbo.Customers", "CompanyIsDeletable");
            DropColumn("dbo.Customers", "IsCashCustomer");
            DropTable("dbo.CustomerOrders");
            DropTable("dbo.CustomerOrderLines");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CustomerOrderLines",
                c => new
                    {
                        LineID = c.Int(nullable: false),
                        CustomerOrderID = c.Int(nullable: false),
                        SpecialOrderLineCode = c.String(),
                        SupplyingName = c.String(),
                    })
                .PrimaryKey(t => t.LineID);
            
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
                .PrimaryKey(t => t.CustomerOrderID);
            
            AddColumn("dbo.Customers", "IsCashCustomer", c => c.Int(nullable: false));
            AddColumn("dbo.Customers", "CompanyIsDeletable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Customers", "CompanySlogan", c => c.String());
            AddColumn("dbo.Customers", "CompanyTradeRegister", c => c.String());
            AddColumn("dbo.Customers", "CompanySigle", c => c.String());
            AddColumn("dbo.Customers", "CompanyCapital", c => c.Int(nullable: false));
            DropForeignKey("dbo.Customers", "AssureurID", "dbo.Assureurs");
            DropForeignKey("dbo.Assureurs", "GlobalPersonID", "dbo.People");
            DropIndex("dbo.Customers", new[] { "AssureurID" });
            DropIndex("dbo.Assureurs", "AssureurNumber");
            DropIndex("dbo.Assureurs", new[] { "GlobalPersonID" });
            DropColumn("dbo.Customers", "AssureurID");
            DropColumn("dbo.Customers", "PoliceAssurance");
            DropTable("dbo.Assureurs");
            CreateIndex("dbo.CustomerOrderLines", "CustomerOrderID");
            CreateIndex("dbo.CustomerOrderLines", "LineID");
            CreateIndex("dbo.CustomerOrders", "OperatorID");
            CreateIndex("dbo.CustomerOrders", "BranchID");
            CreateIndex("dbo.CustomerOrders", "DeviseID");
            CreateIndex("dbo.CustomerOrders", "CustomerID");
            CreateIndex("dbo.CustomerOrders", "CustomerOrderNumber", unique: true);
            AddForeignKey("dbo.CustomerOrderLines", "CustomerOrderID", "dbo.CustomerOrders", "CustomerOrderID");
            AddForeignKey("dbo.CustomerOrderLines", "LineID", "dbo.Lines", "LineID");
            AddForeignKey("dbo.CustomerOrders", "OperatorID", "dbo.Users", "GlobalPersonID");
            AddForeignKey("dbo.CustomerOrders", "DeviseID", "dbo.Devises", "DeviseID");
            AddForeignKey("dbo.CustomerOrders", "CustomerID", "dbo.Customers", "GlobalPersonID");
            AddForeignKey("dbo.CustomerOrders", "BranchID", "dbo.Branches", "BranchID");
        }
    }
}
