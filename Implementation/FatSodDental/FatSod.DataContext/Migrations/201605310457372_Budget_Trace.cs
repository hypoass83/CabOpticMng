namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Budget_Trace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserConfigurations", "isLimitAmountControl", c => c.Boolean(nullable: false));
            AddColumn("dbo.BudgetConsumptions", "AutorizeByID", c => c.Int());
            AddColumn("dbo.BudgetConsumptions", "ValidateByID", c => c.Int());
            CreateIndex("dbo.BudgetConsumptions", "AutorizeByID");
            CreateIndex("dbo.BudgetConsumptions", "ValidateByID");
            AddForeignKey("dbo.BudgetConsumptions", "AutorizeByID", "dbo.Users", "GlobalPersonID");
            AddForeignKey("dbo.BudgetConsumptions", "ValidateByID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BudgetConsumptions", "ValidateByID", "dbo.Users");
            DropForeignKey("dbo.BudgetConsumptions", "AutorizeByID", "dbo.Users");
            DropIndex("dbo.BudgetConsumptions", new[] { "ValidateByID" });
            DropIndex("dbo.BudgetConsumptions", new[] { "AutorizeByID" });
            DropColumn("dbo.BudgetConsumptions", "ValidateByID");
            DropColumn("dbo.BudgetConsumptions", "AutorizeByID");
            DropColumn("dbo.UserConfigurations", "isLimitAmountControl");
        }
    }
}
