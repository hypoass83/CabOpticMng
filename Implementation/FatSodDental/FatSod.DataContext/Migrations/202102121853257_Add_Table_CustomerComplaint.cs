namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_CustomerComplaint : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerComplaints",
                c => new
                    {
                        CustomerComplaintId = c.Int(nullable: false, identity: true),
                        Complaint = c.String(),
                        ResolverComment = c.String(),
                        ControllerComment = c.String(),
                        Occurrences = c.Int(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                        ResolvedDate = c.DateTime(),
                        ControlledDate = c.DateTime(),
                        CumulSaleAndBillID = c.Int(nullable: false),
                        ComplaintQuotationId = c.Int(nullable: false),
                        ComplaintResolverId = c.Int(),
                        ComplaintControllerId = c.Int(),
                    })
                .PrimaryKey(t => t.CustomerComplaintId)
                .ForeignKey("dbo.Users", t => t.ComplaintControllerId)
                .ForeignKey("dbo.Profiles", t => t.ComplaintQuotationId)
                .ForeignKey("dbo.Users", t => t.ComplaintResolverId)
                .ForeignKey("dbo.CumulSaleAndBills", t => t.CumulSaleAndBillID)
                .Index(t => t.CumulSaleAndBillID)
                .Index(t => t.ComplaintQuotationId)
                .Index(t => t.ComplaintResolverId)
                .Index(t => t.ComplaintControllerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerComplaints", "CumulSaleAndBillID", "dbo.CumulSaleAndBills");
            DropForeignKey("dbo.CustomerComplaints", "ComplaintResolverId", "dbo.Users");
            DropForeignKey("dbo.CustomerComplaints", "ComplaintQuotationId", "dbo.Profiles");
            DropForeignKey("dbo.CustomerComplaints", "ComplaintControllerId", "dbo.Users");
            DropIndex("dbo.CustomerComplaints", new[] { "ComplaintControllerId" });
            DropIndex("dbo.CustomerComplaints", new[] { "ComplaintResolverId" });
            DropIndex("dbo.CustomerComplaints", new[] { "ComplaintQuotationId" });
            DropIndex("dbo.CustomerComplaints", new[] { "CumulSaleAndBillID" });
            DropTable("dbo.CustomerComplaints");
        }
    }
}
