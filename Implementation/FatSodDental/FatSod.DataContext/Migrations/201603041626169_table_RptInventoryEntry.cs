namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class table_RptInventoryEntry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RptInventoryEntries",
                c => new
                    {
                        RptInventoryEntryID = c.Int(nullable: false, identity: true),
                        Ref = c.String(),
                        Title = c.String(),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        CompanyLogo = c.Binary(),
                        InventoryDirectoryDate = c.DateTime(nullable: false),
                        ProductLabel = c.String(),
                        ProductRef = c.String(),
                        StockDifference = c.Double(nullable: false),
                        OldStockQuantity = c.Double(nullable: false),
                        NewStockQuantity = c.Double(nullable: false),
                        AveragePurchasePrice = c.Double(nullable: false),
                        Operator = c.String(),
                        DeviseLabel = c.String(),
                    })
                .PrimaryKey(t => t.RptInventoryEntryID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RptInventoryEntries");
        }
    }
}
