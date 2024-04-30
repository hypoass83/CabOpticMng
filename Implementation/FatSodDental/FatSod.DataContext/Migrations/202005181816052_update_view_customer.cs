namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_view_customer : DbMigration
    {
        public override void Up()
        {
            Sql(@"Alter VIEW [dbo].[viewCustomers] AS SELECT NEWID() AS Id, g.GlobalPersonID, g.Name, g.Description, g.CNI, c.CustomerNumber, c.Dateregister, a.AdressPOBox, a.AdressEmail, a.AdressPhoneNumber, q.QuarterLabel, a.AdressID, q.QuarterCode, q.QuarterID, 
                         c.IsBillCustomer, c.AccountID, ac.AccountNumber, ac.AccountLabel, p.SexID, s.SexCode, s.SexLabel,
						 CASE WHEN ISNULL(CustomerValue,0)=0 THEN 'ECO' ELSE 'VIP' END AS CustomerValue,
						 CASE WHEN ISNULL(LastCustomerValue,0)=0 THEN 'ECO' ELSE 'VIP' END AS LastCustomerValue
                         FROM dbo.GlobalPeople AS g INNER JOIN
                         dbo.People AS p ON g.GlobalPersonID = p.GlobalPersonID INNER JOIN
                         dbo.Customers AS c ON c.GlobalPersonID = p.GlobalPersonID INNER JOIN
                         dbo.Adresses AS a ON g.AdressID = a.AdressID INNER JOIN
                         dbo.Quarters AS q ON a.QuarterID = q.QuarterID INNER JOIN
                         dbo.Accounts AS ac ON c.AccountID = ac.AccountID INNER JOIN
                         dbo.Sexes AS s ON p.SexID = s.SexID");
        }
        
        public override void Down()
        {
            Sql(@"CREATE VIEW [dbo].[viewCustomers] AS select NEWID() AS Id,g.GlobalPersonID,g.Name,g.Description,g.CNI,c.CustomerNumber,c.DateRegister,a.AdressPOBox,
            a.AdressEmail,a.AdressPhoneNumber,q.QuarterLabel,a.AdressID,q.QuarterCode,q.QuarterID,c.IsBillCustomer,
			c.AccountID,ac.AccountNumber,ac.AccountLabel,p.SexID,s.SexCode,s.SexLabel
            from GlobalPeople g inner join People p on g.GlobalPersonID=p.GlobalPersonID
            inner join Customers c on c.GlobalPersonID=p.GlobalPersonID
            inner join Adresses a on g.AdressID=a.AdressID
            inner join Quarters q on a.QuarterID=q.QuarterID
            inner join Accounts ac on c.AccountID=ac.AccountID
			inner join Sexes s on p.SexID=s.SexID");
        }
    }
}
