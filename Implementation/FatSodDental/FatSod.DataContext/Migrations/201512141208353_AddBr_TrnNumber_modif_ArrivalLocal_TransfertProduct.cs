namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBr_TrnNumber_modif_ArrivalLocal_TransfertProduct : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductTransfertLines", new[] { "ArrivalLocalizationID" });
            AddColumn("dbo.TransactNumbers", "BranchID", c => c.Int(nullable: false));
            AlterColumn("dbo.ProductTransfertLines", "ArrivalLocalizationID", c => c.Int());
            CreateIndex("dbo.ProductTransfertLines", "ArrivalLocalizationID");
            CreateIndex("dbo.TransactNumbers", "BranchID");
            AddForeignKey("dbo.TransactNumbers", "BranchID", "dbo.Branches", "BranchID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactNumbers", "BranchID", "dbo.Branches");
            DropIndex("dbo.TransactNumbers", new[] { "BranchID" });
            DropIndex("dbo.ProductTransfertLines", new[] { "ArrivalLocalizationID" });
            AlterColumn("dbo.ProductTransfertLines", "ArrivalLocalizationID", c => c.Int(nullable: false));
            DropColumn("dbo.TransactNumbers", "BranchID");
            CreateIndex("dbo.ProductTransfertLines", "ArrivalLocalizationID");
        }
    }
}
