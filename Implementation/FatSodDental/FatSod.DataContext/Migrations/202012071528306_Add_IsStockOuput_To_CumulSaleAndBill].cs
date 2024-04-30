namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IsStockOuput_To_CumulSaleAndBill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBills", "IsStockOutPut", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CumulSaleAndBills", "IsStockOutPut");
        }
    }
}
