namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_ComplaintFeedBack : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ComplaintFeedBacks",
                c => new
                    {
                        ComplaintFeedBackId = c.Int(nullable: false, identity: true),
                        OperationDate = c.DateTime(nullable: false),
                        IsSatisfied = c.Boolean(nullable: false),
                        Comment = c.String(),
                        ContactChannel = c.String(),
                        CustomerComplaintId = c.Int(nullable: false),
                        OperatorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ComplaintFeedBackId)
                .ForeignKey("dbo.CustomerComplaints", t => t.CustomerComplaintId)
                .ForeignKey("dbo.Users", t => t.OperatorID)
                .Index(t => t.CustomerComplaintId)
                .Index(t => t.OperatorID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ComplaintFeedBacks", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.ComplaintFeedBacks", "CustomerComplaintId", "dbo.CustomerComplaints");
            DropIndex("dbo.ComplaintFeedBacks", new[] { "OperatorID" });
            DropIndex("dbo.ComplaintFeedBacks", new[] { "CustomerComplaintId" });
            DropTable("dbo.ComplaintFeedBacks");
        }
    }
}
