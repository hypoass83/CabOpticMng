namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modif_View_SaleWithoutreturn : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER VIEW [dbo].[viewSaleWithoutreturn] AS SELECT        NEWID() AS Id, sa.SaleID, sa.SaleDate, sa.SaleReceiptNumber, ln.LineQuantity AS saleLineQty, ln.ProductID, ln.LocalizationID, ln.LineUnitPrice, 0 AS retlineqty, 0 AS retTransport, sa.Transport, ln.LineQuantity AS SaleQty, 
                         sa.RateDiscount, sa.RateReduction, sa.VatRate, sa.CustomerID, gp.Name, p.ProductCode, l.LocalizationCode, sa.BranchID, b.BranchCode, b.BranchDescription, sl.LineID, sa.StatutSale, sa.PostByID, sa.OperatorID, 
                         sa.SaleDeliveryDate, sa.Remarque, sa.MedecinTraitant
FROM            dbo.Sales AS sa INNER JOIN
                         dbo.SaleLines AS sl ON sa.SaleID = sl.SaleID INNER JOIN
                         dbo.Lines AS ln ON sl.LineID = ln.LineID LEFT OUTER JOIN
                         dbo.viewcumulreturnsalepersale AS cr ON sa.SaleID = cr.SaleID AND ln.ProductID = cr.ProductID AND ln.LocalizationID = cr.LocalizationID INNER JOIN
                         dbo.GlobalPeople AS gp ON sa.CustomerID = gp.GlobalPersonID INNER JOIN
                         dbo.Products AS p ON ln.ProductID = p.ProductID INNER JOIN
                         dbo.Localizations AS l ON ln.LocalizationID = l.LocalizationID INNER JOIN
                         dbo.Branches AS b ON sa.BranchID = b.BranchID
WHERE        (cr.SaleID IS NULL) AND (UPPER(sa.SaleReceiptNumber) NOT LIKE 'SABI%')");
        }
        
        public override void Down()
        {
            Sql(@"ALTER VIEW [dbo].[viewSaleWithoutreturn] AS SELECT        NEWID() AS Id, sa.SaleID, sa.SaleDate, sa.SaleReceiptNumber, ln.LineQuantity AS saleLineQty, ln.ProductID, ln.LocalizationID, ln.LineUnitPrice, 0 AS retlineqty, 0 AS retTransport, sa.Transport, ln.LineQuantity AS SaleQty, 
                         sa.RateDiscount, sa.RateReduction, sa.VatRate, sa.CustomerID, gp.Name, p.ProductCode, l.LocalizationCode, sa.BranchID, b.BranchCode, b.BranchDescription, sl.LineID, sa.StatutSale, sa.PostByID, sa.OperatorID, 
                         sa.SaleDeliveryDate, sa.Remarque, sa.MedecinTraitant
FROM            dbo.Sales AS sa INNER JOIN
                         dbo.SaleLines AS sl ON sa.SaleID = sl.SaleID INNER JOIN
                         dbo.Lines AS ln ON sl.LineID = ln.LineID LEFT OUTER JOIN
                         dbo.viewcumulreturnsalepersale AS cr ON sa.SaleID = cr.SaleID AND ln.ProductID = cr.ProductID AND ln.LocalizationID = cr.LocalizationID INNER JOIN
                         dbo.GlobalPeople AS gp ON sa.CustomerID = gp.GlobalPersonID INNER JOIN
                         dbo.Products AS p ON ln.ProductID = p.ProductID INNER JOIN
                         dbo.Localizations AS l ON ln.LocalizationID = l.LocalizationID INNER JOIN
                         dbo.Branches AS b ON sa.BranchID = b.BranchID
WHERE        (cr.SaleID IS NULL)");
        }
    }
}
