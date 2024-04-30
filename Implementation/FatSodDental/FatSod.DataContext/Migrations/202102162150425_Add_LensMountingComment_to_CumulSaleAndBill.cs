namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_LensMountingComment_to_CumulSaleAndBill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CumulSaleAndBills", "LensMountingComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CumulSaleAndBills", "LensMountingComment");
        }
    }
}
