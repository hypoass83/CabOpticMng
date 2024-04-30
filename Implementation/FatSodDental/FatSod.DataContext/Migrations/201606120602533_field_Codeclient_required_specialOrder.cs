namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class field_Codeclient_required_specialOrder : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RptSpecialOrders", "CodeClient", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RptSpecialOrders", "CodeClient", c => c.String());
        }
    }
}
