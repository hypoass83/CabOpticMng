namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_table_RptPrintStockMvts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RptPrintStockMvts",
                c => new
                    {
                        RptPrintStockMvtID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        LocalizationName = c.String(maxLength: 100),
                        ProductName = c.String(maxLength: 500),
                        Devise = c.String(maxLength: 3),
                        LibDevise = c.String(maxLength: 100),
                        EndDate = c.DateTime(nullable: false),
                        BeginDate = c.DateTime(nullable: false),
                        DateOperation = c.DateTime(nullable: false),
                        RefOperation = c.String(maxLength: 30),
                        Description = c.String(maxLength: 100),
                        RepOutPut = c.Double(nullable: false),
                        RepInput = c.Double(nullable: false),
                        Solde = c.Double(nullable: false),
                        QteOutPut = c.Double(nullable: false),
                        QteInput = c.Double(nullable: false),
                        Sens = c.String(maxLength: 10),
                        CompanyName = c.String(maxLength: 255),
                        RegionCountry = c.String(maxLength: 255),
                        Telephone = c.String(maxLength: 30),
                        Fax = c.String(maxLength: 30),
                        Adresse = c.String(maxLength: 255),
                        LogoBranch = c.Binary(),
                        ProductID = c.Int(nullable: false),
                        LocalizationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RptPrintStockMvtID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RptPrintStockMvts");
        }
    }
}
