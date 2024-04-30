namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modif_Rpt_Receip_Detail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RptReceipts", "Reference", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.RptPaymentDetails", "Reference", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.RptReceipts", "Ref");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RptReceipts", "Ref", c => c.String());
            AlterColumn("dbo.RptPaymentDetails", "Reference", c => c.String());
            DropColumn("dbo.RptReceipts", "Reference");
        }
    }
}
