namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Field_TransactionNumber_require_RptCashOpHist : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RptCashOpHists", "TransactionNumber", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RptCashOpHists", "TransactionNumber", c => c.String());
        }
    }
}
