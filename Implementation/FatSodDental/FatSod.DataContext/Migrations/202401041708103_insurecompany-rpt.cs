namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class insurecompanyrpt : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER VIEW [dbo].[V_Summary_Insured_Bill] AS
            SELECT        NEWID() AS ID, s.CustomerOrderID, s.CustomerOrderDate, s.CustomerName, MAX(s.CompanyName) AS CompanyName,max(i.CompanySigle) as InsuredCompany, MAX(s.NumeroFacture) AS NumeroFacture, MAX(s.PhoneNumber) AS PhoneNumber, MAX(s.Plafond) 
                         AS Plafond, s.SellerID, MAX(us.Name + CASE WHEN isnull(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END) AS SellerName, (CASE WHEN s.GestionnaireID IS NULL THEN
                             (SELECT        GlobalPersonID
                               FROM            GlobalPeople
                               WHERE        LTRIM(Rtrim(Name)) + ' ' + LTRIM(RTRIM(Description)) LIKE '%HOUSE CUSTOMER%') ELSE s.GestionnaireID END) AS MarketerID, MAX(um.Name + CASE WHEN isnull(um.Description, '') 
                         = '' THEN '' ELSE ' ' + um.Description END) AS MarketerName, s.ValidateBillDate, CASE WHEN
                             (SELECT        COUNT(*)
                               FROM            CustomerOrders co INNER JOIN
                                                         [BusinessDays] b ON co.BranchID = b.BranchID
                               WHERE        co.CustomerName = s.CustomerName AND DATEDIFF(day, co.CustomerOrderDate, b.BDDateOperation) > 7) > 0 THEN 0 ELSE 1 END AS IsNewCustomer
            FROM            dbo.CustomerOrders AS s left outer JOIN dbo.Assureurs i on s.AssureurID=i.GlobalPersonID INNER JOIN
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
            
            Sql(@"ALTER VIEW [dbo].[V_Detail_Insured_Bill] AS
            SELECT        NEWID() AS ID, s.CustomerOrderID, s.CustomerOrderDate, s.CustomerName, s.CompanyName,i.[CompanySigle] as InsuredCompany, s.NumeroFacture, s.PhoneNumber, p.ProductID, sl.marque, sl.reference, p.Prescription, c.CategoryCode, l.LineID, c.CategoryID, 
                         s.Plafond, s.SellerID, us.Name + CASE WHEN isnull(us.Description, '') = '' THEN '' ELSE ' ' + us.Description END AS SellerName, (CASE WHEN s.GestionnaireID IS NULL THEN
                             (SELECT        GlobalPersonID
                               FROM            GlobalPeople
                               WHERE        LTRIM(Rtrim(Name)) + ' ' + LTRIM(RTRIM(Description)) LIKE '%HOUSE CUSTOMER%') ELSE s.GestionnaireID END) AS MarketerID, um.Name + CASE WHEN isnull(um.Description, '') 
                         = '' THEN '' ELSE ' ' + um.Description END AS MarketerName, s.ValidateBillDate, CASE WHEN
                             (SELECT        COUNT(*)
                               FROM            CustomerOrders co INNER JOIN
                                                         [BusinessDays] b ON co.BranchID = b.BranchID
                               WHERE        co.CustomerName = s.CustomerName AND DATEDIFF(day, co.CustomerOrderDate, b.BDDateOperation) > 7) > 0 THEN 0 ELSE 1 END AS IsNewCustomer
            FROM            dbo.CustomerOrders AS s  left outer JOIN dbo.Assureurs i on s.AssureurID=i.GlobalPersonID INNER JOIN 
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

        }
        
        public override void Down()
        {
            Sql(@"ALTER VIEW [dbo].[V_Detail_Insured_Bill] AS
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

            Sql(@"ALTER VIEW [dbo].[V_Summary_Insured_Bill] AS
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
    }
}
