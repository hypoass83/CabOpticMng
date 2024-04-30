namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_table_receipt_mntEn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "CustomerDateHours", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "CustomerDateHours");
        }
    }
}
