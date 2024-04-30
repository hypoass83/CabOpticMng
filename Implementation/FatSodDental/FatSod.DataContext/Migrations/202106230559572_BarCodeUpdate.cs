namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BarCodeUpdate : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TransactNumbers", new[] { "BranchID" });
            AddColumn("dbo.ProductLocalizations", "BarCode", c => c.String());
            AlterColumn("dbo.TransactNumbers", "DateOperation", c => c.DateTime());
            AlterColumn("dbo.TransactNumbers", "BranchID", c => c.Int());
            CreateIndex("dbo.TransactNumbers", "BranchID");
            AlterStoredProcedure(
                "dbo.ProductLocalization_Insert",
                p => new
                    {
                        BarCode = p.String(),
                        ProductLocalizationStockQuantity = p.Double(),
                        ProductLocalizationSafetyStockQuantity = p.Double(),
                        ProductLocalizationStockSellingPrice = p.Double(),
                        AveragePurchasePrice = p.Double(),
                        ProductLocalizationDate = p.DateTime(),
                        ProductID = p.Int(),
                        LocalizationID = p.Int(),
                        NumeroSerie = p.String(),
                        Marque = p.String(),
                        isDeliver = p.Boolean(),
                        ProductBrand_ProductBrandID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[ProductLocalizations]([BarCode], [ProductLocalizationStockQuantity], [ProductLocalizationSafetyStockQuantity], [ProductLocalizationStockSellingPrice], [AveragePurchasePrice], [ProductLocalizationDate], [ProductID], [LocalizationID], [NumeroSerie], [Marque], [isDeliver], [ProductBrand_ProductBrandID])
                      VALUES (@BarCode, @ProductLocalizationStockQuantity, @ProductLocalizationSafetyStockQuantity, @ProductLocalizationStockSellingPrice, @AveragePurchasePrice, @ProductLocalizationDate, @ProductID, @LocalizationID, @NumeroSerie, @Marque, @isDeliver, @ProductBrand_ProductBrandID)
                      
                      DECLARE @ProductLocalizationID int
                      SELECT @ProductLocalizationID = [ProductLocalizationID]
                      FROM [dbo].[ProductLocalizations]
                      WHERE @@ROWCOUNT > 0 AND [ProductLocalizationID] = scope_identity()
                      
                      SELECT t0.[ProductLocalizationID]
                      FROM [dbo].[ProductLocalizations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ProductLocalizationID] = @ProductLocalizationID"
            );
            
            AlterStoredProcedure(
                "dbo.ProductLocalization_Update",
                p => new
                    {
                        ProductLocalizationID = p.Int(),
                        BarCode = p.String(),
                        ProductLocalizationStockQuantity = p.Double(),
                        ProductLocalizationSafetyStockQuantity = p.Double(),
                        ProductLocalizationStockSellingPrice = p.Double(),
                        AveragePurchasePrice = p.Double(),
                        ProductLocalizationDate = p.DateTime(),
                        ProductID = p.Int(),
                        LocalizationID = p.Int(),
                        NumeroSerie = p.String(),
                        Marque = p.String(),
                        isDeliver = p.Boolean(),
                        ProductBrand_ProductBrandID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[ProductLocalizations]
                      SET [BarCode] = @BarCode, [ProductLocalizationStockQuantity] = @ProductLocalizationStockQuantity, [ProductLocalizationSafetyStockQuantity] = @ProductLocalizationSafetyStockQuantity, [ProductLocalizationStockSellingPrice] = @ProductLocalizationStockSellingPrice, [AveragePurchasePrice] = @AveragePurchasePrice, [ProductLocalizationDate] = @ProductLocalizationDate, [ProductID] = @ProductID, [LocalizationID] = @LocalizationID, [NumeroSerie] = @NumeroSerie, [Marque] = @Marque, [isDeliver] = @isDeliver, [ProductBrand_ProductBrandID] = @ProductBrand_ProductBrandID
                      WHERE ([ProductLocalizationID] = @ProductLocalizationID)"
            );
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TransactNumbers", new[] { "BranchID" });
            AlterColumn("dbo.TransactNumbers", "BranchID", c => c.Int(nullable: false));
            AlterColumn("dbo.TransactNumbers", "DateOperation", c => c.DateTime(nullable: false));
            DropColumn("dbo.ProductLocalizations", "BarCode");
            CreateIndex("dbo.TransactNumbers", "BranchID");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
