namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class add_view_for_sale_status : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE VIEW dbo.ViewSaleProductPerCustomer AS
SELECT        s.SaleID, s.SaleDate, s.CustomerID, p.ProductID, sl.marque, sl.reference, p.Prescription, c.CategoryCode, lc.TypeLens, l.LineID, c.CategoryID, s.SellerID
FROM            dbo.Sales AS s INNER JOIN
                         dbo.SaleLines AS sl ON s.SaleID = sl.SaleID INNER JOIN
                         dbo.Lines AS l ON sl.LineID = l.LineID INNER JOIN
                         dbo.Products AS p ON l.ProductID = p.ProductID INNER JOIN
                         dbo.Categories AS c ON p.CategoryID = c.CategoryID LEFT OUTER JOIN
                         dbo.LensCategories AS lc ON c.CategoryID = lc.CategoryID");

            Sql(@"CREATE VIEW dbo.v_SaleCustProdWithPrice AS
SELECT        vcsp.SaleID, vcsp.SaleDate, vcsp.CustomerID, vcsp.ProductID, vcsp.marque, vcsp.reference, vcsp.Prescription, vcsp.CategoryID, vcsp.CategoryCode, vcsp.TypeLens, vcsp.LineID, vstp.SaleReceiptNumber, vstp.totPrice, 
                         vcsp.SellerID
FROM            dbo.ViewSaleProductPerCustomer AS vcsp INNER JOIN
                         dbo.saletotalprice AS vstp ON vcsp.SaleID = vstp.SaleID AND vcsp.CustomerID = vstp.CustomerID AND vcsp.LineID = vstp.LineID");

            Sql(@"CREATE VIEW dbo.v_Customer_Phone AS
SELECT        g.GlobalPersonID, g.Name + ' ' + CASE WHEN isnull(g.Description, '') = '' THEN '' ELSE g.Description END AS Name, LTRIM(RTRIM((CASE WHEN isnull(a.AdressCellNumber, '') 
                         = '' THEN a.AdressPhoneNumber ELSE a.AdressCellNumber END))) AS Phone, c.GestionnaireID
FROM            dbo.Customers AS c INNER JOIN
                         dbo.GlobalPeople AS g ON c.GlobalPersonID = g.GlobalPersonID INNER JOIN
                         dbo.Adresses AS a ON g.AdressID = a.AdressID");

            Sql(@"CREATE VIEW dbo.v_CumulSalePerCustomer AS
SELECT        vs.SaleID, vs.SaleDate, vs.CustomerID, vs.marque, vs.reference, vs.CategoryID, vs.CategoryCode, vs.TypeLens, vs.SaleReceiptNumber, SUM(vs.totPrice) AS TotalPrice, MAX(vc.Name) AS CustomerName, vc.Phone, 
                         vs.SellerID, vc.GestionnaireID
FROM            dbo.v_SaleCustProdWithPrice AS vs INNER JOIN
                         dbo.v_Customer_Phone AS vc ON vs.CustomerID = vc.GlobalPersonID
GROUP BY vs.SaleID, vs.SaleDate, vs.CustomerID, vs.marque, vs.reference, vs.CategoryID, vs.CategoryCode, vs.TypeLens, vs.SaleReceiptNumber, vc.Phone, vs.SellerID, vc.GestionnaireID");

            Sql(@"CREATE VIEW dbo.v_CumulSalePerCustomerFrame AS
SELECT        v.SaleID, v.SaleDate, v.CustomerID, v.marque, v.reference, v.CategoryID, v.CategoryCode, v.TypeLens, v.SaleReceiptNumber, v.TotalPrice, v.CustomerName, v.Phone, v.SellerID, 
                         US.Name + CASE WHEN ISNULL(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END AS SellerName, v.GestionnaireID AS MarketerID, Um.Name + CASE WHEN ISNULL(um.Description, '') 
                         = '' THEN '' ELSE ' ' + um.Description END AS MarketerName
FROM            dbo.v_CumulSalePerCustomer AS v LEFT OUTER JOIN
                         dbo.GlobalPeople AS US ON v.SellerID = US.GlobalPersonID LEFT OUTER JOIN
                         dbo.GlobalPeople AS Um ON v.GestionnaireID = Um.GlobalPersonID
WHERE        (v.CategoryID = 1)");

            Sql(@"CREATE VIEW dbo.v_CumulSalePerCustomerLens AS
SELECT        v.SaleID, v.SaleDate, v.CustomerID, v.marque, v.reference, v.CategoryID, v.CategoryCode, v.TypeLens, v.SaleReceiptNumber, v.TotalPrice, v.CustomerName, v.Phone, v.SellerID, 
                         US.Name + CASE WHEN ISNULL(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END AS SellerName, v.GestionnaireID AS MarketerID, Um.Name + CASE WHEN ISNULL(um.Description, '') 
                         = '' THEN '' ELSE ' ' + um.Description END AS MarketerName
FROM            dbo.v_CumulSalePerCustomer AS v LEFT OUTER JOIN
                         dbo.GlobalPeople AS US ON v.SellerID = US.GlobalPersonID LEFT OUTER JOIN
                         dbo.GlobalPeople AS Um ON v.GestionnaireID = Um.GlobalPersonID
WHERE        (ISNULL(v.TypeLens, N'') <> '')");

            Sql(@"CREATE VIEW dbo.v_CumulSalePerCustomerFrameAndLens AS
SELECT        (CASE WHEN isnull(vf.SaleID, 0) = 0 THEN vl.SaleID ELSE vf.SaleID END) AS SaleID, (CASE WHEN isnull(vf.SaleDate, 0) = 0 THEN vl.SaleDate ELSE vf.SaleDate END) AS SaleDate, (CASE WHEN isnull(vf.CustomerID, 0) 
                         = 0 THEN vl.CustomerID ELSE vf.CustomerID END) AS CustomerID, (CASE WHEN isnull(vf.CustomerName, '') = '' THEN vl.CustomerName ELSE vf.CustomerName END) AS CustomerName, (CASE WHEN isnull(vf.Phone, '') 
                         = '' THEN vl.Phone ELSE vf.Phone END) AS PhoneNumber, vf.marque, vf.reference, vf.TotalPrice AS FramePrice, vl.CategoryCode, vl.TypeLens, vl.TotalPrice AS LensPrice, (CASE WHEN isnull(vf.SaleReceiptNumber, '') 
                         = '' THEN vl.SaleReceiptNumber ELSE vf.SaleReceiptNumber END) AS SaleReceiptNumber, (CASE WHEN isnull(vf.MarketerID, 0) = 0 THEN vl.MarketerID ELSE vf.MarketerID END) AS MarketerID, 
                         (CASE WHEN isnull(vf.MarketerName, '') = '' THEN vl.MarketerName ELSE vf.MarketerName END) AS MarketerName, (CASE WHEN isnull(vf.SellerID, 0) = 0 THEN vl.SellerID ELSE vf.SellerID END) AS SellerID, 
                         (CASE WHEN isnull(vf.SellerName, '') = '' THEN vl.SellerName ELSE vf.SellerName END) AS SellerName
FROM            dbo.v_CumulSalePerCustomerFrame AS vf FULL OUTER JOIN
                         dbo.v_CumulSalePerCustomerLens AS vl ON vf.SaleID = vl.SaleID AND vf.CustomerID = vl.CustomerID");

            Sql(@"CREATE VIEW dbo.V_CustomerStatus AS
SELECT        NEWID() AS ID, CustomerID, CustomerName, PhoneNumber, SaleDate, SaleReceiptNumber, reference, marque, FramePrice, TypeLens, LensPrice, (CASE WHEN (TypeLens IN ('PROG', 'BIFOCAL') AND (isnull(FramePrice, 0) 
                         + isnull(LensPrice, 0)) >= 120000) THEN 'VIP' WHEN (TypeLens IN ('SV') AND (isnull(FramePrice, 0) + isnull(LensPrice, 0)) >= 60000) THEN 'VIP' ELSE 'ECO' END) AS CustomerStatus, SellerID, SellerName, MarketerID, 
                         MarketerName
FROM            dbo.v_CumulSalePerCustomerFrameAndLens");

            Sql(@"CREATE VIEW dbo.v_CumulInsurePerCustomerLens AS
SELECT DISTINCT 
                         s.CustomerOrderID, s.CustomerOrderDate, s.ValidateBillDate, s.CustomerName, s.CompanyName, s.NumeroFacture, s.PhoneNumber, sl.marque, sl.reference, c.CategoryCode, lc.TypeLens, c.CategoryID, s.MntValidate, 
                         s.TotalMalade, s.Plafond, s.VerreAssurance, s.MontureAssurance, s.InsurreName, s.SellerID, us.Name + CASE WHEN isnull(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END AS SellerName, 
                         s.GestionnaireID AS MarketerID, um.Name + CASE WHEN isnull(um.Description, '') = '' THEN '' ELSE ' ' + um.Description END AS MarketerName, s.ValidateBillDate AS Expr1
FROM            dbo.CustomerOrders AS s INNER JOIN
                         dbo.CustomerOrderLines AS sl ON s.CustomerOrderID = sl.CustomerOrderID INNER JOIN
                         dbo.Lines AS l ON sl.LineID = l.LineID INNER JOIN
                         dbo.Products AS p ON l.ProductID = p.ProductID INNER JOIN
                         dbo.Categories AS c ON p.CategoryID = c.CategoryID LEFT OUTER JOIN
                         dbo.LensCategories AS lc ON c.CategoryID = lc.CategoryID LEFT OUTER JOIN
                         dbo.GlobalPeople AS um ON s.GestionnaireID = um.GlobalPersonID LEFT OUTER JOIN
                         dbo.GlobalPeople AS us ON s.SellerID = us.GlobalPersonID
WHERE        (s.BillState > 0) AND (s.BillState < 4) AND (c.CategoryID NOT IN (1, 2))");

            Sql(@"CREATE VIEW dbo.v_CumulInsurePerCustomerFrame AS
SELECT DISTINCT 
                         s.CustomerOrderID, s.CustomerOrderDate, s.ValidateBillDate, s.CustomerName, s.CompanyName, s.NumeroFacture, s.PhoneNumber, sl.marque, sl.reference, c.CategoryCode, lc.TypeLens, c.CategoryID, s.MntValidate, 
                         s.TotalMalade, s.Plafond, s.VerreAssurance, s.MontureAssurance, s.InsurreName, s.SellerID, us.Name + CASE WHEN isnull(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END AS SellerName, 
                         s.GestionnaireID AS MarketerID, um.Name + CASE WHEN isnull(um.Description, '') = '' THEN '' ELSE ' ' + um.Description END AS MarketerName
FROM            dbo.CustomerOrders AS s INNER JOIN
                         dbo.CustomerOrderLines AS sl ON s.CustomerOrderID = sl.CustomerOrderID INNER JOIN
                         dbo.Lines AS l ON sl.LineID = l.LineID INNER JOIN
                         dbo.Products AS p ON l.ProductID = p.ProductID INNER JOIN
                         dbo.Categories AS c ON p.CategoryID = c.CategoryID LEFT OUTER JOIN
                         dbo.LensCategories AS lc ON c.CategoryID = lc.CategoryID LEFT OUTER JOIN
                         dbo.GlobalPeople AS um ON s.GestionnaireID = um.GlobalPersonID LEFT OUTER JOIN
                         dbo.GlobalPeople AS us ON s.SellerID = us.GlobalPersonID
WHERE        (s.BillState > 0) AND (s.BillState < 4) AND (c.CategoryID = 1)");

            Sql(@"CREATE VIEW dbo.v_CumulInsurePerCustomerFrameAndLens AS
SELECT        (CASE WHEN isnull(vf.[CustomerOrderID], 0) = 0 THEN VL.[CustomerOrderID] ELSE VF.[CustomerOrderID] END) AS CustomerOrderID, (CASE WHEN isnull(vf.[CustomerOrderDate], '') 
                         = '' THEN VL.[CustomerOrderDate] ELSE VF.[CustomerOrderDate] END) AS CustomerOrderDate, (CASE WHEN isnull(vf.[ValidateBillDate], '') = '' THEN VL.[ValidateBillDate] ELSE VF.[ValidateBillDate] END) AS ValidateBillDate, 
                         (CASE WHEN isnull(vf.[CustomerName], '') = '' THEN VL.[CustomerName] ELSE VF.[CustomerName] END) AS CustomerName, (CASE WHEN isnull(vf.[CompanyName], '') 
                         = '' THEN VL.[CompanyName] ELSE VF.[CompanyName] END) AS CompanyName, (CASE WHEN isnull(vf.InsurreName, '') = '' THEN VL.InsurreName ELSE VF.InsurreName END) AS InsurreName, 
                         (CASE WHEN isnull(vf.[NumeroFacture], '') = '' THEN VL.[NumeroFacture] ELSE VF.[NumeroFacture] END) AS NumeroFacture, (CASE WHEN isnull(vf.[PhoneNumber], '') = '' THEN VL.[PhoneNumber] ELSE VF.[PhoneNumber] END) 
                         AS PhoneNumber, vf.marque, vf.reference, vl.CategoryCode, vl.TypeLens, vl.CategoryID, (CASE WHEN isnull(vf.[MntValidate], 0) = 0 THEN VL.MntValidate ELSE VF.MntValidate END) AS MntValidate, 
                         (CASE WHEN isnull(vf.[TotalMalade], 0) = 0 THEN VL.[TotalMalade] ELSE VF.[TotalMalade] END) AS TotalMalade, (CASE WHEN isnull(vf.[Plafond], 0) = 0 THEN VL.[Plafond] ELSE VF.[Plafond] END) AS Plafond, 
                         (CASE WHEN isnull(vf.[VerreAssurance], 0) = 0 THEN VL.[VerreAssurance] ELSE VF.[VerreAssurance] END) AS VerreAssurance, (CASE WHEN isnull(vf.[MontureAssurance], 0) 
                         = 0 THEN VL.[MontureAssurance] ELSE VF.[MontureAssurance] END) AS MontureAssurance, (CASE WHEN isnull(vf.[MarketerID], 0) = 0 THEN VL.[MarketerID] ELSE VF.[MarketerID] END) AS MarketerID, 
                         (CASE WHEN isnull(vf.[MarketerName], '') = '' THEN VL.[MarketerName] ELSE VF.[MarketerName] END) AS MarketerName, (CASE WHEN isnull(vf.[SellerID], 0) = 0 THEN VL.[SellerID] ELSE VF.[SellerID] END) AS SellerID, 
                         (CASE WHEN isnull(vf.[SellerName], '') = '' THEN VL.[SellerName] ELSE VF.[SellerName] END) AS SellerName
FROM            dbo.v_CumulInsurePerCustomerFrame AS vf FULL OUTER JOIN
                         dbo.v_CumulInsurePerCustomerLens AS vl ON vf.CustomerOrderID = vl.CustomerOrderID");

            Sql(@"CREATE VIEW dbo.V_InsureStatus AS
SELECT        NEWID() AS ID, CustomerName, CompanyName, InsurreName, PhoneNumber, CustomerOrderDate, ValidateBillDate, NumeroFacture, reference, marque, Plafond, TypeLens, (CASE WHEN ((isnull(Plafond, 0)) >= 150000) 
                         THEN 'VIP' ELSE 'ECO' END) AS CustomerStatus, MarketerID, MarketerName, SellerID, SellerName
FROM            dbo.v_CumulInsurePerCustomerFrameAndLens");

            Sql(@"CREATE VIEW dbo.V_Detail_Sales AS
SELECT        NEWID() AS ID, vcsp.SaleID, vcsp.SaleDate, vcsp.CustomerID, vcsp.ProductID, vcsp.marque, vcsp.reference, vcsp.Prescription, vcsp.CategoryID, vcsp.CategoryCode, vcsp.TypeLens, vcsp.LineID, vstp.SaleReceiptNumber, 
                         vstp.totPrice, VC.GestionnaireID AS MarketerID, UM.Name + CASE WHEN ISNULL(um.Description, '') = '' THEN '' ELSE ' ' + um.Description END AS MarketerName, VC.Name AS CustomerName, VC.Phone, vcsp.SellerID, 
                         US.Name + CASE WHEN ISNULL(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END AS SellerName
FROM            dbo.ViewSaleProductPerCustomer AS vcsp INNER JOIN
                         dbo.saletotalprice AS vstp ON vcsp.SaleID = vstp.SaleID AND vcsp.CustomerID = vstp.CustomerID AND vcsp.LineID = vstp.LineID INNER JOIN
                         dbo.v_Customer_Phone AS VC ON vcsp.CustomerID = VC.GlobalPersonID LEFT OUTER JOIN
                         dbo.GlobalPeople AS UM ON VC.GestionnaireID = UM.GlobalPersonID LEFT OUTER JOIN
                         dbo.GlobalPeople AS US ON vcsp.SellerID = US.GlobalPersonID
WHERE        (vstp.totPrice > 0)");

            Sql(@"CREATE VIEW dbo.V_Summary_Sales AS
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

            Sql(@"CREATE VIEW dbo.V_Detail_Insured_Bill AS
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

            Sql(@"CREATE VIEW dbo.V_Summary_Insured_Bill AS
SELECT        NEWID() AS ID, s.CustomerOrderID, s.CustomerOrderDate, MAX(s.CustomerName) AS CustomerName, MAX(s.CompanyName) AS CompanyName, MAX(s.NumeroFacture) AS NumeroFacture, MAX(s.PhoneNumber) 
                         AS PhoneNumber, MAX(s.Plafond) AS Plafond, s.SellerID, MAX(us.Name + CASE WHEN isnull(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END) AS SellerName, s.GestionnaireID AS MarketerID, 
                         MAX(um.Name + CASE WHEN isnull(um.Description, '') = '' THEN '' ELSE ' ' + um.Description END) AS MarketerName, s.ValidateBillDate
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

        public override void Down()
        {
            Sql(@"DROP VIEW dbo.ViewSaleProductPerCustomer");
            Sql(@"DROP VIEW dbo.v_SaleCustProdWithPrice");
            Sql(@"DROP VIEW dbo.v_Customer_Phone");
            Sql(@"DROP VIEW dbo.v_CumulSalePerCustomer");
            Sql(@"DROP VIEW dbo.v_CumulSalePerCustomerFrame");
            Sql(@"DROP VIEW dbo.v_CumulSalePerCustomerLens");
            Sql(@"DROP VIEW dbo.v_CumulSalePerCustomerFrameAndLens");
            Sql(@"DROP VIEW dbo.V_CustomerStatus");
            Sql(@"DROP VIEW dbo.v_CumulInsurePerCustomerLens");
            Sql(@"DROP VIEW dbo.v_CumulInsurePerCustomerFrame");
            Sql(@"DROP VIEW dbo.v_CumulInsurePerCustomerFrameAndLens");
            Sql(@"DROP VIEW dbo.V_InsureStatus");
            Sql(@"DROP VIEW dbo.V_Detail_Sales");
            Sql(@"DROP VIEW dbo.V_Summary_Sales");
            Sql(@"DROP VIEW dbo.V_Detail_Insured_Bill");
            Sql(@"DROP VIEW dbo.V_Summary_Insured_Bill");
        }
    }
}
