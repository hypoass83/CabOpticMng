namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Table_RDV : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HistoRendezVous",
                c => new
                    {
                        HistoRendezVousID = c.Int(nullable: false, identity: true),
                        SaleID = c.Int(nullable: false),
                        RendezVousID = c.Int(nullable: false),
                        DateRdv = c.DateTime(nullable: false),
                        Remarque = c.String(),
                    })
                .PrimaryKey(t => t.HistoRendezVousID)
                .ForeignKey("dbo.RendezVous", t => t.RendezVousID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .Index(t => t.SaleID)
                .Index(t => t.RendezVousID);
            
            CreateTable(
                "dbo.RendezVous",
                c => new
                    {
                        RendezVousID = c.Int(nullable: false, identity: true),
                        CustomerID = c.Int(nullable: false),
                        DateRdv = c.DateTime(nullable: false),
                        RaisonRdv = c.String(),
                    })
                .PrimaryKey(t => t.RendezVousID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .Index(t => t.CustomerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HistoRendezVous", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.HistoRendezVous", "RendezVousID", "dbo.RendezVous");
            DropForeignKey("dbo.RendezVous", "CustomerID", "dbo.Customers");
            DropIndex("dbo.RendezVous", new[] { "CustomerID" });
            DropIndex("dbo.HistoRendezVous", new[] { "RendezVousID" });
            DropIndex("dbo.HistoRendezVous", new[] { "SaleID" });
            DropTable("dbo.RendezVous");
            DropTable("dbo.HistoRendezVous");
        }
    }
}
