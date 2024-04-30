namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NumeroFacture_CustOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "NumeroFacture", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrders", "NumeroFacture");
        }
    }
}
