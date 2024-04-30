namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ViewCustomers : DbMigration
    {
        public override void Up()
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

        public override void Down()
        {
            Sql(@"DROP VIEW dbo.viewCustomers");
        }
    }
}
