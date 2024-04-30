namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CumulSaleAndBill_IsReceived_IsMounted_DeliverID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBills", "IsReceived", c => c.Boolean(nullable: false));
            AddColumn("dbo.CumulSaleAndBills", "IsMounted", c => c.Boolean(nullable: false));
            AddColumn("dbo.CumulSaleAndBills", "ReceiverID", c => c.Int());
            CreateIndex("dbo.CumulSaleAndBills", "ReceiverID");
            AddForeignKey("dbo.CumulSaleAndBills", "ReceiverID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CumulSaleAndBills", "ReceiverID", "dbo.Users");
            DropIndex("dbo.CumulSaleAndBills", new[] { "ReceiverID" });
            DropColumn("dbo.CumulSaleAndBills", "ReceiverID");
            DropColumn("dbo.CumulSaleAndBills", "IsMounted");
            DropColumn("dbo.CumulSaleAndBills", "IsReceived");
        }
    }
}
