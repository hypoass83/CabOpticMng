namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_RptCashOpHist_PaymentMethod : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RptCashOpHists", "PaymentMethod", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RptCashOpHists", "PaymentMethod", c => c.String());
        }
    }
}
