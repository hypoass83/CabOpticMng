namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_table_RptSpecialOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RptSpecialOrders", "ProductID", c => c.Int(nullable: false));
            AddColumn("dbo.RptSpecialOrders", "ProductCode", c => c.String());
            AddColumn("dbo.RptSpecialOrders", "Balance", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RptSpecialOrders", "Balance");
            DropColumn("dbo.RptSpecialOrders", "ProductCode");
            DropColumn("dbo.RptSpecialOrders", "ProductID");
        }
    }
}
