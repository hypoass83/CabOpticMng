namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsVIPRoom : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lines", "IsVIPRoom", c => c.Int(nullable: false));
            AlterStoredProcedure(
                "dbo.CustomerOrderLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        CustomerOrderID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        FrameCategory = p.String(),
                        Addition = p.String(),
                        Axis = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie], [IsVIPRoom])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie, @IsVIPRoom)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[CustomerOrderLines]([LineID], [CustomerOrderID], [SpecialOrderLineCode], [marque], [reference], [FrameCategory], [Addition], [Axis], [LensNumberCylindricalValue], [LensNumberSphericalValue])
                      VALUES (@LineID, @CustomerOrderID, @SpecialOrderLineCode, @marque, @reference, @FrameCategory, @Addition, @Axis, @LensNumberCylindricalValue, @LensNumberSphericalValue)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            AlterStoredProcedure(
                "dbo.CustomerOrderLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        CustomerOrderID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        FrameCategory = p.String(),
                        Addition = p.String(),
                        Axis = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[CustomerOrderLines]
                      SET [CustomerOrderID] = @CustomerOrderID, [SpecialOrderLineCode] = @SpecialOrderLineCode, [marque] = @marque, [reference] = @reference, [FrameCategory] = @FrameCategory, [Addition] = @Addition, [Axis] = @Axis, [LensNumberCylindricalValue] = @LensNumberCylindricalValue, [LensNumberSphericalValue] = @LensNumberSphericalValue
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie, [IsVIPRoom] = @IsVIPRoom
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.SaleLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        SaleID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        Addition = p.String(),
                        Axis = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie], [IsVIPRoom])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie, @IsVIPRoom)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[SaleLines]([LineID], [SaleID], [SpecialOrderLineCode], [marque], [reference], [Addition], [Axis], [LensNumberCylindricalValue], [LensNumberSphericalValue])
                      VALUES (@LineID, @SaleID, @SpecialOrderLineCode, @marque, @reference, @Addition, @Axis, @LensNumberCylindricalValue, @LensNumberSphericalValue)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            AlterStoredProcedure(
                "dbo.SaleLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        SaleID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        Addition = p.String(),
                        Axis = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie, [IsVIPRoom] = @IsVIPRoom
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[SaleLines]
                      SET [SaleID] = @SaleID, [SpecialOrderLineCode] = @SpecialOrderLineCode, [marque] = @marque, [reference] = @reference, [Addition] = @Addition, [Axis] = @Axis, [LensNumberCylindricalValue] = @LensNumberCylindricalValue, [LensNumberSphericalValue] = @LensNumberSphericalValue
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.AuthoriseSaleLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        Axis = p.String(),
                        Addition = p.String(),
                        Index = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie], [IsVIPRoom])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie, @IsVIPRoom)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[AuthoriseSaleLines]([LineID], [AuthoriseSaleID], [SpecialOrderLineCode], [marque], [reference], [Axis], [Addition], [Index], [LensNumberCylindricalValue], [LensNumberSphericalValue])
                      VALUES (@LineID, @AuthoriseSaleID, @SpecialOrderLineCode, @marque, @reference, @Axis, @Addition, @Index, @LensNumberCylindricalValue, @LensNumberSphericalValue)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            AlterStoredProcedure(
                "dbo.AuthoriseSaleLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        AuthoriseSaleID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        Axis = p.String(),
                        Addition = p.String(),
                        Index = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[AuthoriseSaleLines]
                      SET [AuthoriseSaleID] = @AuthoriseSaleID, [SpecialOrderLineCode] = @SpecialOrderLineCode, [marque] = @marque, [reference] = @reference, [Axis] = @Axis, [Addition] = @Addition, [Index] = @Index, [LensNumberCylindricalValue] = @LensNumberCylindricalValue, [LensNumberSphericalValue] = @LensNumberSphericalValue
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie, [IsVIPRoom] = @IsVIPRoom
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.PurchaseLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        PurchaseID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie], [IsVIPRoom])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie, @IsVIPRoom)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[PurchaseLines]([LineID], [PurchaseID])
                      VALUES (@LineID, @PurchaseID)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            AlterStoredProcedure(
                "dbo.PurchaseLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        PurchaseID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie, [IsVIPRoom] = @IsVIPRoom
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[PurchaseLines]
                      SET [PurchaseID] = @PurchaseID
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.CumulSaleAndBillLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        CumulSaleAndBillID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        IsOrdered = p.Boolean(),
                        Axis = p.String(),
                        Addition = p.String(),
                        Index = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                        ProductCategoryID = p.Int(),
                        PurchaseLineUnitPrice = p.Double(),
                        StockType = p.Int(),
                        Supplier = p.String(),
                        Manufacturer = p.String(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie], [IsVIPRoom])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie, @IsVIPRoom)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[CumulSaleAndBillLines]([LineID], [CumulSaleAndBillID], [SpecialOrderLineCode], [marque], [reference], [IsOrdered], [Axis], [Addition], [Index], [LensNumberCylindricalValue], [LensNumberSphericalValue], [ProductCategoryID], [PurchaseLineUnitPrice], [StockType], [Supplier], [Manufacturer])
                      VALUES (@LineID, @CumulSaleAndBillID, @SpecialOrderLineCode, @marque, @reference, @IsOrdered, @Axis, @Addition, @Index, @LensNumberCylindricalValue, @LensNumberSphericalValue, @ProductCategoryID, @PurchaseLineUnitPrice, @StockType, @Supplier, @Manufacturer)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            AlterStoredProcedure(
                "dbo.CumulSaleAndBillLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        CumulSaleAndBillID = p.Int(),
                        SpecialOrderLineCode = p.String(),
                        marque = p.String(),
                        reference = p.String(),
                        IsOrdered = p.Boolean(),
                        Axis = p.String(),
                        Addition = p.String(),
                        Index = p.String(),
                        LensNumberCylindricalValue = p.String(),
                        LensNumberSphericalValue = p.String(),
                        ProductCategoryID = p.Int(),
                        PurchaseLineUnitPrice = p.Double(),
                        StockType = p.Int(),
                        Supplier = p.String(),
                        Manufacturer = p.String(),
                    },
                body:
                    @"UPDATE [dbo].[CumulSaleAndBillLines]
                      SET [CumulSaleAndBillID] = @CumulSaleAndBillID, [SpecialOrderLineCode] = @SpecialOrderLineCode, [marque] = @marque, [reference] = @reference, [IsOrdered] = @IsOrdered, [Axis] = @Axis, [Addition] = @Addition, [Index] = @Index, [LensNumberCylindricalValue] = @LensNumberCylindricalValue, [LensNumberSphericalValue] = @LensNumberSphericalValue, [ProductCategoryID] = @ProductCategoryID, [PurchaseLineUnitPrice] = @PurchaseLineUnitPrice, [StockType] = @StockType, [Supplier] = @Supplier, [Manufacturer] = @Manufacturer
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie, [IsVIPRoom] = @IsVIPRoom
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
            AlterStoredProcedure(
                "dbo.Line_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie], [IsVIPRoom])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie, @IsVIPRoom)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            AlterStoredProcedure(
                "dbo.Line_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie, [IsVIPRoom] = @IsVIPRoom
                      WHERE ([LineID] = @LineID)"
            );
            
            AlterStoredProcedure(
                "dbo.SupplierOrderLine_Insert",
                p => new
                    {
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        SupplierOrderID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie], [IsVIPRoom])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie, @IsVIPRoom)
                      
                      DECLARE @LineID int
                      SELECT @LineID = [LineID]
                      FROM [dbo].[Lines]
                      WHERE @@ROWCOUNT > 0 AND [LineID] = scope_identity()
                      
                      INSERT [dbo].[SupplierOrderLines]([LineID], [SupplierOrderID])
                      VALUES (@LineID, @SupplierOrderID)
                      
                      SELECT t0.[LineID]
                      FROM [dbo].[Lines] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[LineID] = @LineID"
            );
            
            AlterStoredProcedure(
                "dbo.SupplierOrderLine_Update",
                p => new
                    {
                        LineID = p.Int(),
                        LineUnitPrice = p.Double(),
                        LineQuantity = p.Double(),
                        LocalizationID = p.Int(),
                        ProductID = p.Int(),
                        isPost = p.Boolean(),
                        OeilDroiteGauche = p.Int(),
                        SupplyingName = p.String(),
                        isGift = p.Boolean(),
                        isCommandGlass = p.Boolean(),
                        NumeroSerie = p.String(),
                        IsVIPRoom = p.Int(),
                        SupplierOrderID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Lines]
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie, [IsVIPRoom] = @IsVIPRoom
                      WHERE ([LineID] = @LineID)
                      
                      UPDATE [dbo].[SupplierOrderLines]
                      SET [SupplierOrderID] = @SupplierOrderID
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lines", "IsVIPRoom");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
