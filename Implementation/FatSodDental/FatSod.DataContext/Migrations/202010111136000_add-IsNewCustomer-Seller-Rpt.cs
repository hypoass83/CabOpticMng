namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIsNewCustomerSellerRpt : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER VIEW dbo.v_Customer_Phone AS
                        SELECT        g.GlobalPersonID, g.Name + ' ' + CASE WHEN isnull(g.Description, '') = '' THEN '' ELSE g.Description END AS Name, LTRIM(RTRIM((CASE WHEN isnull(a.AdressCellNumber, '') 
                         = '' THEN a.AdressPhoneNumber ELSE a.AdressCellNumber END))) AS Phone, (CASE WHEN c.GestionnaireID IS NULL THEN
                             (SELECT        GlobalPersonID
                               FROM            GlobalPeople
                               WHERE        LTRIM(Rtrim(Name)) + ' ' + LTRIM(RTRIM(Description)) LIKE '%HOUSE CUSTOMER%') ELSE c.GestionnaireID END) AS GestionnaireID, c.IsInHouseCustomer
                        FROM            dbo.Customers AS c INNER JOIN
                         dbo.GlobalPeople AS g ON c.GlobalPersonID = g.GlobalPersonID INNER JOIN
                         dbo.Adresses AS a ON g.AdressID = a.AdressID");

            Sql(@"ALTER VIEW dbo.ViewSaleProductPerCustomer AS
                        SELECT        s.SaleID, s.SaleDate, s.CustomerID, p.ProductID, sl.marque, sl.reference, p.Prescription, c.CategoryCode, lc.TypeLens, l.LineID, c.CategoryID, s.SellerID, s.GestionnaireID
                        FROM            dbo.Sales AS s INNER JOIN
                         dbo.SaleLines AS sl ON s.SaleID = sl.SaleID INNER JOIN
                         dbo.Lines AS l ON sl.LineID = l.LineID INNER JOIN
                         dbo.Products AS p ON l.ProductID = p.ProductID INNER JOIN
                         dbo.Categories AS c ON p.CategoryID = c.CategoryID LEFT OUTER JOIN
                         dbo.LensCategories AS lc ON c.CategoryID = lc.CategoryID");

            Sql(@"ALTER VIEW dbo.V_Detail_Sales AS
            SELECT        NEWID() AS ID, vcsp.SaleID, vcsp.SaleDate, vcsp.CustomerID, vcsp.ProductID, vcsp.marque, vcsp.reference, vcsp.Prescription, vcsp.CategoryID, vcsp.CategoryCode, vcsp.TypeLens, vcsp.LineID, vstp.SaleReceiptNumber, 
                         vstp.totPrice, CASE WHEN vcsp.GestionnaireID IS NULL THEN VC.GestionnaireID ELSE vcsp.GestionnaireID END AS MarketerID, UM.Name + CASE WHEN ISNULL(um.Description, '') 
                         = '' THEN '' ELSE ' ' + um.Description END AS MarketerName, VC.Name AS CustomerName, VC.Phone, vcsp.SellerID, US.Name + CASE WHEN ISNULL(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END AS SellerName, 
                         CASE WHEN
                             (SELECT COUNT(*)
                               FROM SALES S INNER JOIN
                                        [BusinessDays] b ON s.BranchID = b.BranchID
                               WHERE    S.CustomerID = vcsp.CustomerID AND DATEDIFF(day, s.SaleDate, b.BDDateOperation) > 7) > 0 THEN 0 ELSE 1 END AS IsNewCustomer, VC.IsInHouseCustomer
            FROM        dbo.ViewSaleProductPerCustomer AS vcsp INNER JOIN
                        dbo.saletotalprice AS vstp ON vcsp.SaleID = vstp.SaleID AND vcsp.CustomerID = vstp.CustomerID AND vcsp.LineID = vstp.LineID INNER JOIN
                        dbo.v_Customer_Phone AS VC ON vcsp.CustomerID = VC.GlobalPersonID LEFT OUTER JOIN
                        dbo.GlobalPeople AS UM ON CASE WHEN vcsp.GestionnaireID IS NULL THEN VC.GestionnaireID ELSE vcsp.GestionnaireID END = UM.GlobalPersonID LEFT OUTER JOIN
                        dbo.GlobalPeople AS US ON vcsp.SellerID = US.GlobalPersonID
            WHERE       (vstp.totPrice > 0)");

            Sql(@"ALTER VIEW dbo.V_Summary_Sales AS
            SELECT        NEWID() AS ID, vcsp.SaleID, vcsp.SaleDate, vcsp.CustomerID, vstp.SaleReceiptNumber, SUM(vstp.totPrice) AS totPrice, CASE WHEN vcsp.GestionnaireID IS NULL 
                         THEN VC.GestionnaireID ELSE vcsp.GestionnaireID END AS MarketerID, MAX(UM.Name + CASE WHEN ISNULL(um.Description, '') = '' THEN '' ELSE ' ' + um.Description END) AS MarketerName, MAX(VC.Name) 
                         AS CustomerName, VC.Phone, vcsp.SellerID, MAX(US.Name + CASE WHEN ISNULL(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END) AS SellerName, CASE WHEN
                             (SELECT    COUNT(*)
                               FROM SALES S INNER JOIN
                                    [BusinessDays] b ON s.BranchID = b.BranchID
                             WHERE  S.CustomerID = vcsp.CustomerID AND DATEDIFF(day, s.SaleDate, b.BDDateOperation) > 7) > 0 THEN 0 ELSE 1 END AS IsNewCustomer, VC.IsInHouseCustomer
            FROM            dbo.ViewSaleProductPerCustomer AS vcsp INNER JOIN
                            dbo.saletotalprice AS vstp ON vcsp.SaleID = vstp.SaleID AND vcsp.CustomerID = vstp.CustomerID AND vcsp.LineID = vstp.LineID INNER JOIN
                            dbo.v_Customer_Phone AS VC ON vcsp.CustomerID = VC.GlobalPersonID LEFT OUTER JOIN
                            dbo.GlobalPeople AS UM ON CASE WHEN vcsp.GestionnaireID IS NULL THEN VC.GestionnaireID ELSE vcsp.GestionnaireID END = UM.GlobalPersonID LEFT OUTER JOIN
                            dbo.GlobalPeople AS US ON vcsp.SellerID = US.GlobalPersonID
            WHERE        (vstp.totPrice > 0)
            GROUP BY vcsp.SaleID, vcsp.SaleDate, vcsp.CustomerID, vstp.SaleReceiptNumber, CASE WHEN vcsp.GestionnaireID IS NULL THEN VC.GestionnaireID ELSE vcsp.GestionnaireID END, VC.Phone, vcsp.SellerID, VC.IsInHouseCustomer");


            Sql(@"ALTER VIEW dbo.V_Detail_Insured_Bill AS
            SELECT        NEWID() AS ID, s.CustomerOrderID, s.CustomerOrderDate, s.CustomerName, s.CompanyName, s.NumeroFacture, s.PhoneNumber, p.ProductID, sl.marque, sl.reference, p.Prescription, c.CategoryCode, l.LineID, c.CategoryID, 
                         s.Plafond, s.SellerID, us.Name + CASE WHEN isnull(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END AS SellerName, (CASE WHEN s.GestionnaireID IS NULL THEN
                             (SELECT        GlobalPersonID
                               FROM            GlobalPeople
                               WHERE        LTRIM(Rtrim(Name)) + ' ' + LTRIM(RTRIM(Description)) LIKE '%HOUSE CUSTOMER%') ELSE s.GestionnaireID END) AS MarketerID, um.Name + CASE WHEN isnull(um.Description, '') 
                         = '' THEN '' ELSE ' ' + um.Description END AS MarketerName, s.ValidateBillDate, CASE WHEN
                             (SELECT        COUNT(*)
                               FROM            CustomerOrders co INNER JOIN
                                                         [BusinessDays] b ON co.BranchID = b.BranchID
                               WHERE        co.CustomerName = s.CustomerName AND DATEDIFF(day, co.CustomerOrderDate, b.BDDateOperation) > 7) > 0 THEN 0 ELSE 1 END AS IsNewCustomer
            FROM            dbo.CustomerOrders AS s INNER JOIN
                                     dbo.CustomerOrderLines AS sl ON s.CustomerOrderID = sl.CustomerOrderID INNER JOIN
                                     dbo.Lines AS l ON sl.LineID = l.LineID INNER JOIN
                                     dbo.Products AS p ON l.ProductID = p.ProductID INNER JOIN
                                     dbo.Categories AS c ON p.CategoryID = c.CategoryID LEFT OUTER JOIN
                                     dbo.LensCategories AS lc ON c.CategoryID = lc.CategoryID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS um ON (CASE WHEN s.GestionnaireID IS NULL THEN
                             (SELECT        GlobalPersonID
                               FROM            GlobalPeople
                               WHERE        LTRIM(Rtrim(Name)) + ' ' + LTRIM(RTRIM(Description)) LIKE '%HOUSE CUSTOMER%') ELSE s.GestionnaireID END) = um.GlobalPersonID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS us ON s.SellerID = us.GlobalPersonID
            WHERE        (s.BillState > 0) AND (s.BillState < 4)");

            Sql(@"ALTER VIEW dbo.V_Summary_Insured_Bill AS
            SELECT        NEWID() AS ID, s.CustomerOrderID, s.CustomerOrderDate, s.CustomerName, MAX(s.CompanyName) AS CompanyName, MAX(s.NumeroFacture) AS NumeroFacture, MAX(s.PhoneNumber) AS PhoneNumber, MAX(s.Plafond) 
                         AS Plafond, s.SellerID, MAX(us.Name + CASE WHEN isnull(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END) AS SellerName, (CASE WHEN s.GestionnaireID IS NULL THEN
                             (SELECT        GlobalPersonID
                               FROM            GlobalPeople
                               WHERE        LTRIM(Rtrim(Name)) + ' ' + LTRIM(RTRIM(Description)) LIKE '%HOUSE CUSTOMER%') ELSE s.GestionnaireID END) AS MarketerID, MAX(um.Name + CASE WHEN isnull(um.Description, '') 
                         = '' THEN '' ELSE ' ' + um.Description END) AS MarketerName, s.ValidateBillDate, CASE WHEN
                             (SELECT        COUNT(*)
                               FROM            CustomerOrders co INNER JOIN
                                                         [BusinessDays] b ON co.BranchID = b.BranchID
                               WHERE        co.CustomerName = s.CustomerName AND DATEDIFF(day, co.CustomerOrderDate, b.BDDateOperation) > 7) > 0 THEN 0 ELSE 1 END AS IsNewCustomer
            FROM            dbo.CustomerOrders AS s INNER JOIN
                                     dbo.CustomerOrderLines AS sl ON s.CustomerOrderID = sl.CustomerOrderID INNER JOIN
                                     dbo.Lines AS l ON sl.LineID = l.LineID INNER JOIN
                                     dbo.Products AS p ON l.ProductID = p.ProductID INNER JOIN
                                     dbo.Categories AS c ON p.CategoryID = c.CategoryID LEFT OUTER JOIN
                                     dbo.LensCategories AS lc ON c.CategoryID = lc.CategoryID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS um ON (CASE WHEN s.GestionnaireID IS NULL THEN
                             (SELECT        GlobalPersonID
                               FROM            GlobalPeople
                               WHERE        LTRIM(Rtrim(Name)) + ' ' + LTRIM(RTRIM(Description)) LIKE '%HOUSE CUSTOMER%') ELSE s.GestionnaireID END) = um.GlobalPersonID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS us ON s.SellerID = us.GlobalPersonID
            WHERE        (s.BillState > 0) AND (s.BillState < 4)
            GROUP BY s.CustomerOrderID, s.CustomerOrderDate, s.SellerID, s.GestionnaireID, s.ValidateBillDate, s.CustomerName");


        }

        public override void Down()
        {
            Sql(@"ALTER VIEW dbo.v_Customer_Phone AS
                        SELECT        g.GlobalPersonID, g.Name + ' ' + CASE WHEN isnull(g.Description, '') = '' THEN '' ELSE g.Description END AS Name, LTRIM(RTRIM((CASE WHEN isnull(a.AdressCellNumber, '') 
                                                 = '' THEN a.AdressPhoneNumber ELSE a.AdressCellNumber END))) AS Phone, c.GestionnaireID
                        FROM            dbo.Customers AS c INNER JOIN
                         dbo.GlobalPeople AS g ON c.GlobalPersonID = g.GlobalPersonID INNER JOIN
                         dbo.Adresses AS a ON g.AdressID = a.AdressID");

            Sql(@"ALTER VIEW dbo.ViewSaleProductPerCustomer AS
                        SELECT        s.SaleID, s.SaleDate, s.CustomerID, p.ProductID, sl.marque, sl.reference, p.Prescription, c.CategoryCode, lc.TypeLens, l.LineID, c.CategoryID, s.SellerID
                        FROM            dbo.Sales AS s INNER JOIN
                         dbo.SaleLines AS sl ON s.SaleID = sl.SaleID INNER JOIN
                         dbo.Lines AS l ON sl.LineID = l.LineID INNER JOIN
                         dbo.Products AS p ON l.ProductID = p.ProductID INNER JOIN
                         dbo.Categories AS c ON p.CategoryID = c.CategoryID LEFT OUTER JOIN
                         dbo.LensCategories AS lc ON c.CategoryID = lc.CategoryID");

            Sql(@"ALTER VIEW dbo.V_Detail_Sales AS
            SELECT        NEWID() AS ID, vcsp.SaleID, vcsp.SaleDate, vcsp.CustomerID, vcsp.ProductID, vcsp.marque, vcsp.reference, vcsp.Prescription, vcsp.CategoryID, vcsp.CategoryCode, vcsp.TypeLens, vcsp.LineID, vstp.SaleReceiptNumber, 
                                     vstp.totPrice, VC.GestionnaireID AS MarketerID, UM.Name + CASE WHEN ISNULL(um.Description, '') = '' THEN '' ELSE ' ' + um.Description END AS MarketerName, VC.Name AS CustomerName, VC.Phone, vcsp.SellerID, 
                                     US.Name + CASE WHEN ISNULL(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END AS SellerName
            FROM            dbo.ViewSaleProductPerCustomer AS vcsp INNER JOIN
                                     dbo.saletotalprice AS vstp ON vcsp.SaleID = vstp.SaleID AND vcsp.CustomerID = vstp.CustomerID AND vcsp.LineID = vstp.LineID INNER JOIN
                                     dbo.v_Customer_Phone AS VC ON vcsp.CustomerID = VC.GlobalPersonID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS UM ON VC.GestionnaireID = UM.GlobalPersonID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS US ON vcsp.SellerID = US.GlobalPersonID
            WHERE        (vstp.totPrice > 0)");

            Sql(@"ALTER VIEW dbo.V_Summary_Sales AS
            SELECT        NEWID() AS ID, vcsp.SaleID, vcsp.SaleDate, vcsp.CustomerID, vstp.SaleReceiptNumber, SUM(vstp.totPrice) AS totPrice, VC.GestionnaireID AS MarketerID, MAX(UM.Name + CASE WHEN ISNULL(um.Description, '') 
                                     = '' THEN '' ELSE ' ' + um.Description END) AS MarketerName, MAX(VC.Name) AS CustomerName, VC.Phone, vcsp.SellerID, MAX(US.Name + CASE WHEN ISNULL(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END) 
                                     AS SellerName
            FROM            dbo.ViewSaleProductPerCustomer AS vcsp INNER JOIN
                                     dbo.saletotalprice AS vstp ON vcsp.SaleID = vstp.SaleID AND vcsp.CustomerID = vstp.CustomerID AND vcsp.LineID = vstp.LineID INNER JOIN
                                     dbo.v_Customer_Phone AS VC ON vcsp.CustomerID = VC.GlobalPersonID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS UM ON VC.GestionnaireID = UM.GlobalPersonID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS US ON vcsp.SellerID = US.GlobalPersonID
            WHERE        (vstp.totPrice > 0)
            GROUP BY vcsp.SaleID, vcsp.SaleDate, vcsp.CustomerID, vstp.SaleReceiptNumber, VC.GestionnaireID, VC.Phone, vcsp.SellerID");


            Sql(@"ALTER VIEW dbo.V_Detail_Insured_Bill AS
            SELECT        NEWID() AS ID, s.CustomerOrderID, s.CustomerOrderDate, s.CustomerName, s.CompanyName, s.NumeroFacture, s.PhoneNumber, p.ProductID, sl.marque, sl.reference, p.Prescription, c.CategoryCode, l.LineID, c.CategoryID, 
                         s.Plafond, s.SellerID, us.Name + CASE WHEN isnull(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END AS SellerName, s.GestionnaireID AS MarketerID, um.Name + CASE WHEN isnull(um.Description, '') 
                         = '' THEN '' ELSE ' ' + um.Description END AS MarketerName, s.ValidateBillDate
            FROM            dbo.CustomerOrders AS s INNER JOIN
                                     dbo.CustomerOrderLines AS sl ON s.CustomerOrderID = sl.CustomerOrderID INNER JOIN
                                     dbo.Lines AS l ON sl.LineID = l.LineID INNER JOIN
                                     dbo.Products AS p ON l.ProductID = p.ProductID INNER JOIN
                                     dbo.Categories AS c ON p.CategoryID = c.CategoryID LEFT OUTER JOIN
                                     dbo.LensCategories AS lc ON c.CategoryID = lc.CategoryID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS um ON s.GestionnaireID = um.GlobalPersonID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS us ON s.SellerID = us.GlobalPersonID
            WHERE        (s.BillState > 0) AND (s.BillState < 4)");

            Sql(@"ALTER VIEW dbo.V_Summary_Insured_Bill AS
            SELECT        NEWID() AS ID, s.CustomerOrderID, s.CustomerOrderDate, MAX(s.CustomerName) as CustomerName, MAX(s.CompanyName) AS CompanyName, MAX(s.NumeroFacture) AS NumeroFacture, MAX(s.PhoneNumber) AS PhoneNumber, MAX(s.Plafond) 
                         AS Plafond, s.SellerID, MAX(us.Name + CASE WHEN isnull(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END) AS SellerName, s.GestionnaireID AS MarketerID, MAX(um.Name + CASE WHEN isnull(um.Description, '') 
                         = '' THEN '' ELSE ' ' + um.Description END) AS MarketerName, s.ValidateBillDate
            FROM            dbo.CustomerOrders AS s INNER JOIN
                                     dbo.CustomerOrderLines AS sl ON s.CustomerOrderID = sl.CustomerOrderID INNER JOIN
                                     dbo.Lines AS l ON sl.LineID = l.LineID INNER JOIN
                                     dbo.Products AS p ON l.ProductID = p.ProductID INNER JOIN
                                     dbo.Categories AS c ON p.CategoryID = c.CategoryID LEFT OUTER JOIN
                                     dbo.LensCategories AS lc ON c.CategoryID = lc.CategoryID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS um ON s.GestionnaireID = um.GlobalPersonID LEFT OUTER JOIN
                                     dbo.GlobalPeople AS us ON s.SellerID = us.GlobalPersonID
            WHERE        (s.BillState > 0) AND (s.BillState < 4)
            GROUP BY s.CustomerOrderID, s.CustomerOrderDate, s.SellerID, s.GestionnaireID, s.ValidateBillDate");

        }
    }
}
