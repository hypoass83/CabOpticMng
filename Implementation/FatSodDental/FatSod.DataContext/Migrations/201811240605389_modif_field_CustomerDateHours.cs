namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_field_CustomerDateHours : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CustomerOrders", "CustomerDateHours", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CustomerOrders", "CustomerDateHours", c => c.DateTime(nullable: false));
        }
    }
}
