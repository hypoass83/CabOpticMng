namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_PaymentDate_BudgetConsumption : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BudgetConsumptions", "PaymentDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BudgetConsumptions", "PaymentDate", c => c.DateTime(nullable: false));
        }
    }
}
