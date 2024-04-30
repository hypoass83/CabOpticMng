namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_MountingBy_CumSaleBill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBills", "IsProductDeliver", c => c.Boolean(nullable: false));
            AddColumn("dbo.CumulSaleAndBills", "ProductDeliverDate", c => c.DateTime());
            AddColumn("dbo.CumulSaleAndBills", "MountingBy", c => c.String());
            AddColumn("dbo.CumulSaleAndBills", "ProductDeliverDateHeure", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CumulSaleAndBills", "ProductDeliverDateHeure");
            DropColumn("dbo.CumulSaleAndBills", "MountingBy");
            DropColumn("dbo.CumulSaleAndBills", "ProductDeliverDate");
            DropColumn("dbo.CumulSaleAndBills", "IsProductDeliver");
        }
    }
}
