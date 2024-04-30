namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Product_On_RDV : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RendezVous", "ProductID", c => c.Int());
            CreateIndex("dbo.RendezVous", "ProductID");
            AddForeignKey("dbo.RendezVous", "ProductID", "dbo.Products", "ProductID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RendezVous", "ProductID", "dbo.Products");
            DropIndex("dbo.RendezVous", new[] { "ProductID" });
            DropColumn("dbo.RendezVous", "ProductID");
        }
    }
}
