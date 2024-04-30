namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Order_Fields_To_CumulSaleAndBill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBillLines", "IsOrdered", c => c.Boolean(nullable: false));
            AddColumn("dbo.CumulSaleAndBills", "IsOrdered", c => c.Boolean(nullable: false));
            AddColumn("dbo.CumulSaleAndBills", "OrderById", c => c.Int());
            AddColumn("dbo.CumulSaleAndBills", "ReOrderReasons", c => c.String());
            AddColumn("dbo.CumulSaleAndBills", "ReOrderDates", c => c.String());
            AddColumn("dbo.CumulSaleAndBills", "OrderDate", c => c.DateTime());
            AddColumn("dbo.CumulSaleAndBills", "NumberOfTimesOrdered", c => c.Int(nullable: false));
            CreateIndex("dbo.CumulSaleAndBills", "OrderById");
            AddForeignKey("dbo.CumulSaleAndBills", "OrderById", "dbo.Users", "GlobalPersonID");
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
                    @"INSERT [dbo].[Lines]([LineUnitPrice], [LineQuantity], [LocalizationID], [ProductID], [isPost], [OeilDroiteGauche], [SupplyingName], [isGift], [isCommandGlass], [NumeroSerie])
                      VALUES (@LineUnitPrice, @LineQuantity, @LocalizationID, @ProductID, @isPost, @OeilDroiteGauche, @SupplyingName, @isGift, @isCommandGlass, @NumeroSerie)
                      
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
                      SET [LineUnitPrice] = @LineUnitPrice, [LineQuantity] = @LineQuantity, [LocalizationID] = @LocalizationID, [ProductID] = @ProductID, [isPost] = @isPost, [OeilDroiteGauche] = @OeilDroiteGauche, [SupplyingName] = @SupplyingName, [isGift] = @isGift, [isCommandGlass] = @isCommandGlass, [NumeroSerie] = @NumeroSerie
                      WHERE ([LineID] = @LineID)
                      AND @@ROWCOUNT > 0"
            );
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CumulSaleAndBills", "OrderById", "dbo.Users");
            DropIndex("dbo.CumulSaleAndBills", new[] { "OrderById" });
            DropColumn("dbo.CumulSaleAndBills", "NumberOfTimesOrdered");
            DropColumn("dbo.CumulSaleAndBills", "OrderDate");
            DropColumn("dbo.CumulSaleAndBills", "ReOrderDates");
            DropColumn("dbo.CumulSaleAndBills", "ReOrderReasons");
            DropColumn("dbo.CumulSaleAndBills", "OrderById");
            DropColumn("dbo.CumulSaleAndBills", "IsOrdered");
            DropColumn("dbo.CumulSaleAndBillLines", "IsOrdered");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
