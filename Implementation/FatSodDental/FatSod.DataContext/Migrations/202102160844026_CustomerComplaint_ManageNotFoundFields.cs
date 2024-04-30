namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerComplaint_ManageNotFoundFields : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CustomerComplaints", new[] { "CumulSaleAndBillID" });
            AddColumn("dbo.CustomerComplaints", "Customer", c => c.String());
            AddColumn("dbo.CustomerComplaints", "PhoneNumber", c => c.String());
            AddColumn("dbo.CustomerComplaints", "Insurance", c => c.String());
            AddColumn("dbo.CustomerComplaints", "InsuredCompany", c => c.String());
            AddColumn("dbo.CustomerComplaints", "IsCashCustomer", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerComplaints", "IsCashOtherCustomer", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerComplaints", "IsInsuredCustomer", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerComplaints", "PurchaseDate", c => c.DateTime());
            AlterColumn("dbo.CustomerComplaints", "CumulSaleAndBillID", c => c.Int());
            CreateIndex("dbo.CustomerComplaints", "CumulSaleAndBillID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CustomerComplaints", new[] { "CumulSaleAndBillID" });
            AlterColumn("dbo.CustomerComplaints", "CumulSaleAndBillID", c => c.Int(nullable: false));
            DropColumn("dbo.CustomerComplaints", "PurchaseDate");
            DropColumn("dbo.CustomerComplaints", "IsInsuredCustomer");
            DropColumn("dbo.CustomerComplaints", "IsCashOtherCustomer");
            DropColumn("dbo.CustomerComplaints", "IsCashCustomer");
            DropColumn("dbo.CustomerComplaints", "InsuredCompany");
            DropColumn("dbo.CustomerComplaints", "Insurance");
            DropColumn("dbo.CustomerComplaints", "PhoneNumber");
            DropColumn("dbo.CustomerComplaints", "Customer");
            CreateIndex("dbo.CustomerComplaints", "CumulSaleAndBillID");
        }
    }
}
