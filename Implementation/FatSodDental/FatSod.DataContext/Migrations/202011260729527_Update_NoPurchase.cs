namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_NoPurchase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NoPurchases", "DeliveryDeskReason", c => c.String(nullable: false));
            AddColumn("dbo.NoPurchases", "CustomerServiceReason", c => c.String());
            DropColumn("dbo.NoPurchases", "Reason");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NoPurchases", "Reason", c => c.String(nullable: false));
            DropColumn("dbo.NoPurchases", "CustomerServiceReason");
            DropColumn("dbo.NoPurchases", "DeliveryDeskReason");
        }
    }
}
