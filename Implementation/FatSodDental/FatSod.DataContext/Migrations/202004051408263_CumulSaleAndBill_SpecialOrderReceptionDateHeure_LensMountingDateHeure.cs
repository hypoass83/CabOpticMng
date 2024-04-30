namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CumulSaleAndBill_SpecialOrderReceptionDateHeure_LensMountingDateHeure : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBills", "SpecialOrderReceptionDateHeure", c => c.DateTime());
            AddColumn("dbo.CumulSaleAndBills", "LensMountingDateHeure", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CumulSaleAndBills", "LensMountingDateHeure");
            DropColumn("dbo.CumulSaleAndBills", "SpecialOrderReceptionDateHeure");
        }
    }
}
