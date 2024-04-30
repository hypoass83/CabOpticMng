namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_update_index_ref_DepositTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Deposits", "DepositReference", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.AllDeposits", "AllDepositReference", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.Deposits", "DepositReference", unique: true);
            CreateIndex("dbo.AllDeposits", "AllDepositReference", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.AllDeposits", new[] { "AllDepositReference" });
            DropIndex("dbo.Deposits", new[] { "DepositReference" });
            AlterColumn("dbo.AllDeposits", "AllDepositReference", c => c.String());
            AlterColumn("dbo.Deposits", "DepositReference", c => c.String());
        }
    }
}
