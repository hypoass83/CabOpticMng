namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_deletebilldate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CustomerOrders", "DeleteBillDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CustomerOrders", "DeleteBillDate", c => c.DateTime(nullable: false));
        }
    }
}
