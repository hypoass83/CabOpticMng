namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_CustomerSatisfaction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerSatisfactions",
                c => new
                    {
                        CustomerSatisfactionId = c.Int(nullable: false, identity: true),
                        OperationDate = c.DateTime(nullable: false),
                        SaleDate = c.DateTime(nullable: false),
                        IsSatisfied = c.Boolean(nullable: false),
                        Comment = c.String(),
                        ContactChannel = c.String(),
                        CumulSaleAndBillID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerSatisfactionId)
                .ForeignKey("dbo.CumulSaleAndBills", t => t.CumulSaleAndBillID)
                .Index(t => t.CumulSaleAndBillID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerSatisfactions", "CumulSaleAndBillID", "dbo.CumulSaleAndBills");
            DropIndex("dbo.CustomerSatisfactions", new[] { "CumulSaleAndBillID" });
            DropTable("dbo.CustomerSatisfactions");
        }
    }
}
