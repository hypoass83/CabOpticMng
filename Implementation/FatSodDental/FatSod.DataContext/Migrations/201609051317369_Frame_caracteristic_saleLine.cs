namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Frame_caracteristic_saleLine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SaleLines", "marque", c => c.String());
            AddColumn("dbo.SaleLines", "reference", c => c.String());
            DropColumn("dbo.SaleLines", "SupplyingName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SaleLines", "SupplyingName", c => c.String());
            DropColumn("dbo.SaleLines", "reference");
            DropColumn("dbo.SaleLines", "marque");
        }
    }
}
