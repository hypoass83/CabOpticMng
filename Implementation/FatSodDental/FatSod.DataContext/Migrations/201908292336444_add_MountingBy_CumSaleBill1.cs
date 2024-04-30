namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_MountingBy_CumSaleBill1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBills", "DeliverProductByID", c => c.Int());
            CreateIndex("dbo.CumulSaleAndBills", "DeliverProductByID");
            AddForeignKey("dbo.CumulSaleAndBills", "DeliverProductByID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CumulSaleAndBills", "DeliverProductByID", "dbo.Users");
            DropIndex("dbo.CumulSaleAndBills", new[] { "DeliverProductByID" });
            DropColumn("dbo.CumulSaleAndBills", "DeliverProductByID");
        }
    }
}
