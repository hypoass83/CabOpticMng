namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modif_table_cumulsaleandbill_DateOperationHours : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBills", "DateOperationHours", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CumulSaleAndBills", "DateOperationHours");
        }
    }
}
