namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_cust_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "IsCashCustomer", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "IsCashCustomer");
        }
    }
}
