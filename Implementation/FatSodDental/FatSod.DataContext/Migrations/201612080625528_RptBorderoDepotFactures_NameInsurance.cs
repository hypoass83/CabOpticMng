namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RptBorderoDepotFactures_NameInsurance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RptBorderoDepotFactures", "InsuranceCompany", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RptBorderoDepotFactures", "InsuranceCompany");
        }
    }
}
