namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_RptCashOpHist_PaymentMethod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RptCashOpHists", "PaymentMethod", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RptCashOpHists", "PaymentMethod");
        }
    }
}
