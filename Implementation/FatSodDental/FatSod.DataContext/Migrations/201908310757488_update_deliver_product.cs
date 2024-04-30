namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_deliver_product : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE [dbo].[CumulSaleAndBills] SET IsProductDeliver=1 WHERE IsDeliver=1");
        }
        
        public override void Down()
        {
            Sql(@"UPDATE [dbo].[CumulSaleAndBills] SET IsProductDeliver=0 WHERE IsDeliver=1");
        }
    }
}
