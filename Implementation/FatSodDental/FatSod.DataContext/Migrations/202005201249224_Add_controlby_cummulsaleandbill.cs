namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_controlby_cummulsaleandbill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBills", "ControlBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CumulSaleAndBills", "ControlBy");
        }
    }
}
