namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_SaleID_RptPrintStm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RptPrintStmts", "SaleID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RptPrintStmts", "SaleID");
        }
    }
}
