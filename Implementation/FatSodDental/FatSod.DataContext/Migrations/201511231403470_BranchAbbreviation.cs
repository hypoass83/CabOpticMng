namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BranchAbbreviation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Branches", "BranchAbbreviation", c => c.String(nullable: false, maxLength: 10));
            AddColumn("dbo.RptBills", "BranchAbbreviation", c => c.String());
            AddColumn("dbo.RptReceipts", "BranchAbbreviation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RptReceipts", "BranchAbbreviation");
            DropColumn("dbo.RptBills", "BranchAbbreviation");
            DropColumn("dbo.Branches", "BranchAbbreviation");
        }
    }
}
