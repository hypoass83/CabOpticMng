namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RptReceipt_MontantLettre : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RptReceipts", "MontantLettre", c => c.String());
            AlterColumn("dbo.RptReceipts", "ProductRef", c => c.String(nullable: false, maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RptReceipts", "ProductRef", c => c.String());
            DropColumn("dbo.RptReceipts", "MontantLettre");
        }
    }
}
