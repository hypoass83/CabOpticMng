namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BarcodeOptimisation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductLocalizations", "ReservedBarCode", c => c.Int(nullable: false));
            AddColumn("dbo.InventoryDirectories", "IsBarcodePrinted", c => c.Boolean(nullable: false));
            AlterStoredProcedure(
                "dbo.ProductLocalization_Insert",
                p => new
                    {
                        BarCode = p.String(),
                        ReservedBarCode = p.Int(),
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
                    @"INSERT [dbo].[ProductLocalizations]([BarCode], [ReservedBarCode], [ProductLocalizationStockQuantity], [ProductLocalizationSafetyStockQuantity], [ProductLocalizationStockSellingPrice], [AveragePurchasePrice], [ProductLocalizationDate], [ProductID], [LocalizationID], [NumeroSerie], [Marque], [isDeliver], [ProductBrand_ProductBrandID])
                      VALUES (@BarCode, @ReservedBarCode, @ProductLocalizationStockQuantity, @ProductLocalizationSafetyStockQuantity, @ProductLocalizationStockSellingPrice, @AveragePurchasePrice, @ProductLocalizationDate, @ProductID, @LocalizationID, @NumeroSerie, @Marque, @isDeliver, @ProductBrand_ProductBrandID)
                      
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
                        ReservedBarCode = p.Int(),
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
                      SET [BarCode] = @BarCode, [ReservedBarCode] = @ReservedBarCode, [ProductLocalizationStockQuantity] = @ProductLocalizationStockQuantity, [ProductLocalizationSafetyStockQuantity] = @ProductLocalizationSafetyStockQuantity, [ProductLocalizationStockSellingPrice] = @ProductLocalizationStockSellingPrice, [AveragePurchasePrice] = @AveragePurchasePrice, [ProductLocalizationDate] = @ProductLocalizationDate, [ProductID] = @ProductID, [LocalizationID] = @LocalizationID, [NumeroSerie] = @NumeroSerie, [Marque] = @Marque, [isDeliver] = @isDeliver, [ProductBrand_ProductBrandID] = @ProductBrand_ProductBrandID
                      WHERE ([ProductLocalizationID] = @ProductLocalizationID)"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryDirectories", "IsBarcodePrinted");
            DropColumn("dbo.ProductLocalizations", "ReservedBarCode");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
