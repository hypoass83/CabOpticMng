namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class add_View : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE VIEW [dbo].[viewcumulreturnsalepersale] AS select NEWID() AS Id,sum(crl.LineQuantity) as totretqty,SaleID,ln.ProductID,ln.LocalizationID,crl.SaleLineID,sum(crl.Transport) as retTransport
            from CustomerReturns cr inner join CustomerReturnLines crl on cr.CustomerReturnID=crl.CustomerReturnID
            inner join Lines ln on crl.SaleLineID=ln.LineID
            group by SaleID,ln.ProductID,ln.LocalizationID,crl.SaleLineID");

            Sql(@"CREATE VIEW [dbo].[viewPartialreturn] AS select NEWID() AS Id,sa.SaleID,sa.SaleDate,sa.SaleReceiptNumber,ln.LineQuantity as saleLineQty, ln.ProductID, ln.LocalizationID,ln.LineUnitPrice,
            cr.totretqty as retlineqty,cr.retTransport, sa.Transport,(ln.LineQuantity-cr.totretqty) as SaleQty,sa.RateDiscount,sa.RateReduction,sa.VatRate,
            sa.CustomerID,gp.Name,p.ProductCode,l.LocalizationCode,sa.BranchID,b.BranchCode,b.BranchDescription,sl.LineID,sa.StatutSale,sa.PostByID,sa.OperatorID,sa.SaleDeliveryDate,sa.Remarque,sa.MedecinTraitant
             from sales sa inner join SaleLines sl on sa.SaleID=sl.SaleID
            inner join Lines ln on sl.LineID=ln.LineID
            inner join viewcumulreturnsalepersale cr on sl.SaleID=cr.SaleID and ln.ProductID=cr.ProductID and ln.LocalizationID=cr.LocalizationID
            inner join GlobalPeople gp on sa.CustomerID=gp.GlobalPersonID
            inner join products p on ln.ProductID=p.ProductID
            inner join Localizations l on ln.LocalizationID=l.LocalizationID
            inner join Branches b on sa.BranchID=b.BranchID
            where (ln.LineQuantity-cr.totretqty)>0");

            Sql(@"CREATE VIEW [dbo].[viewSaleWithoutreturn] AS select NEWID() AS Id,sa.SaleID,sa.SaleDate,sa.SaleReceiptNumber,ln.LineQuantity as saleLineQty, ln.ProductID, ln.LocalizationID,ln.LineUnitPrice,
            0 as retlineqty,0 as retTransport,sa.Transport,ln.LineQuantity as SaleQty,sa.RateDiscount,sa.RateReduction,sa.VatRate,
            sa.CustomerID,gp.Name,p.ProductCode,l.LocalizationCode,sa.BranchID,b.BranchCode,b.BranchDescription,sl.LineID,sa.StatutSale,sa.PostByID,sa.OperatorID,sa.SaleDeliveryDate,sa.Remarque,sa.MedecinTraitant
            from sales sa inner join SaleLines sl on sa.SaleID=sl.SaleID
            inner join Lines ln on sl.LineID=ln.LineID
            left outer join dbo.viewcumulreturnsalepersale AS cr ON sa.SaleID=cr.SaleID and ln.ProductID = cr.ProductID AND ln.LocalizationID = cr.LocalizationID
            inner join GlobalPeople gp on sa.CustomerID=gp.GlobalPersonID
            inner join products p on ln.ProductID=p.ProductID
            inner join Localizations l on ln.LocalizationID=l.LocalizationID
            inner join Branches b on sa.BranchID=b.BranchID
            where cr.SaleID is null");

            Sql(@"CREATE VIEW [dbo].[viewRealSales] AS select * from viewSaleWithoutreturn
            union
            select * from viewPartialreturn");

            Sql(@"create view [dbo].[ViewTransportSale] As
                select saleid, Max(vrs.Transport) as Transport, count(*) as toline,round (Max(vrs.Transport)/count(*),2) as divideTransp 
                FROM  dbo.viewRealSales AS vrs
                group by saleid");

            Sql(@"Create VIEW [dbo].[saletotalprice] As SELECT        NEWID() AS Id, vrs.SaleID, vrs.LineID, vrs.CustomerID, vrs.SaleReceiptNumber, vrs.SaleDate, SUM(ROUND(((vrs.SaleQty * vrs.LineUnitPrice - (vrs.RateReduction / 100.0) * (vrs.SaleQty * vrs.LineUnitPrice)) 
                         - (vrs.RateDiscount / 100.0) * (vrs.SaleQty * vrs.LineUnitPrice - (vrs.RateReduction / 100) * (vrs.SaleQty * vrs.LineUnitPrice)) + vs.divideTransp) + ((vrs.SaleQty * vrs.LineUnitPrice - (vrs.RateReduction / 100.0) 
                         * (vrs.SaleQty * vrs.LineUnitPrice)) - (vrs.RateDiscount / 100.0) * (vrs.SaleQty * vrs.LineUnitPrice - (vrs.RateReduction / 100.0) * (vrs.SaleQty * vrs.LineUnitPrice)) + vs.divideTransp) * (vrs.VatRate / 100.0), 2)) 
                         AS totPrice
            FROM            dbo.viewRealSales AS vrs INNER JOIN
                                     dbo.ViewTransportSale AS vs ON vrs.SaleID = vs.saleid
            GROUP BY vrs.SaleID, vrs.LineID, vrs.CustomerID, vrs.SaleReceiptNumber, vrs.SaleDate");

            Sql(@"CREATE VIEW [dbo].[viewcustomerSlice]
            AS
            SELECT        NEWID() AS Id, sa.SaleID, sa.CustomerID, sa.SaleReceiptNumber, SUM(sl.SliceAmount) AS SliceAmount, sa.SaleDate
            FROM            dbo.Sales AS sa INNER JOIN
                                     dbo.CustomerSlices AS csl ON sa.SaleID = csl.SaleID INNER JOIN
                                     dbo.Slices AS sl ON csl.SliceID = sl.SliceID
            GROUP BY sa.SaleID, sa.CustomerID, sa.SaleReceiptNumber, sa.SaleDate");

            Sql(@"CREATE VIEW [dbo].[PendingSale]
            AS
            SELECT        NEWID() AS Id, sp.SaleID, sp.CustomerID, sp.SaleReceiptNumber, ISNULL(vs.SliceAmount, 0) AS SliceAmount, SUM(sp.totPrice) AS SellingPrice, SUM(sp.totPrice) - ISNULL(vs.SliceAmount, 0) AS RemainAmount, 
                                     sp.SaleDate
            FROM            dbo.saletotalprice AS sp LEFT OUTER JOIN
                                     dbo.viewcustomerSlice AS vs ON sp.SaleID = vs.SaleID
            GROUP BY sp.SaleID, sp.CustomerID, sp.SaleReceiptNumber, ISNULL(vs.SliceAmount, 0), sp.SaleDate
            HAVING        (ROUND(ISNULL(vs.SliceAmount, 0), 0) < ROUND(SUM(sp.totPrice), 0))");

            Sql(@"CREATE VIEW [dbo].[CutomertotalpricePerDay] AS select NEWID() AS Id, CustomerID,SaleDate,
            sum(round((((((saleqty*lineunitprice) - ((rateReduction / 100.0) * ((saleqty*lineunitprice)))) - ((rateDiscount / 100.0) * ((saleqty*lineunitprice) - ((rateReduction / 100) * (saleqty*lineunitprice))))) + transport) + (((((saleqty*lineunitprice) - ((rateReduction / 100.0) * (saleqty*lineunitprice))) - ((rateDiscount / 100.0) * ((saleqty*lineunitprice) - ((rateReduction / 100.0) * ((saleqty*lineunitprice)))))) + transport) * (vatRate/100.0))),2)) as totCustDayPrice
            from viewRealSales
            group by CustomerID,SaleDate");
        }

        public override void Down()
        {
            Sql(@"DROP VIEW dbo.CutomertotalpricePerDay");
            Sql(@"DROP VIEW dbo.PendingSale");
            Sql(@"DROP VIEW dbo.viewcustomerSlice");
            Sql(@"DROP VIEW dbo.saletotalprice");
            Sql(@"DROP VIEW dbo.ViewTransportSale");
            Sql(@"DROP VIEW dbo.viewRealSales");
            Sql(@"DROP VIEW dbo.viewSaleWithoutreturn");
            Sql(@"DROP VIEW dbo.viewPartialreturn");
            Sql(@"DROP VIEW dbo.viewcumulreturnsalepersale");
        }
    }
}
