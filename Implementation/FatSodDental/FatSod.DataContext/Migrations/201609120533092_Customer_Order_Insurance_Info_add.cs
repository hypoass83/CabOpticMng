namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer_Order_Insurance_Info_add : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerOrders", "CustomerID", "dbo.Customers");
            DropIndex("dbo.CustomerOrders", new[] { "CustomerID" });
            AddColumn("dbo.CustomerOrders", "CustomerName", c => c.String(nullable: false));
            AddColumn("dbo.CustomerOrders", "PhoneNumber", c => c.String());
            AddColumn("dbo.CustomerOrders", "PoliceAssurance", c => c.String(maxLength: 250));
            AddColumn("dbo.CustomerOrders", "CompanyName", c => c.String(maxLength: 250));
            AddColumn("dbo.CustomerOrders", "AssureurID", c => c.Int(nullable: false));
            AddColumn("dbo.CustomerOrders", "PlafondAssurance", c => c.Double(nullable: false));
            AddColumn("dbo.CustomerOrders", "NumeroBonPriseEnCharge", c => c.String());
            AddColumn("dbo.CustomerOrders", "BillState", c => c.String());
            CreateIndex("dbo.CustomerOrders", "AssureurID");
            AddForeignKey("dbo.CustomerOrders", "AssureurID", "dbo.Assureurs", "GlobalPersonID");
            DropColumn("dbo.CustomerOrders", "CustomerID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrders", "CustomerID", c => c.Int(nullable: false));
            DropForeignKey("dbo.CustomerOrders", "AssureurID", "dbo.Assureurs");
            DropIndex("dbo.CustomerOrders", new[] { "AssureurID" });
            DropColumn("dbo.CustomerOrders", "BillState");
            DropColumn("dbo.CustomerOrders", "NumeroBonPriseEnCharge");
            DropColumn("dbo.CustomerOrders", "PlafondAssurance");
            DropColumn("dbo.CustomerOrders", "AssureurID");
            DropColumn("dbo.CustomerOrders", "CompanyName");
            DropColumn("dbo.CustomerOrders", "PoliceAssurance");
            DropColumn("dbo.CustomerOrders", "PhoneNumber");
            DropColumn("dbo.CustomerOrders", "CustomerName");
            CreateIndex("dbo.CustomerOrders", "CustomerID");
            AddForeignKey("dbo.CustomerOrders", "CustomerID", "dbo.Customers", "GlobalPersonID");
        }
    }
}
