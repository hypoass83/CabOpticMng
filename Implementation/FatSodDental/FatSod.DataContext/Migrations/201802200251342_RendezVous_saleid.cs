namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RendezVous_saleid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RendezVous", "SaleID", c => c.Int());
            CreateIndex("dbo.RendezVous", "SaleID");
            AddForeignKey("dbo.RendezVous", "SaleID", "dbo.Sales", "SaleID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RendezVous", "SaleID", "dbo.Sales");
            DropIndex("dbo.RendezVous", new[] { "SaleID" });
            DropColumn("dbo.RendezVous", "SaleID");
        }
    }
}
