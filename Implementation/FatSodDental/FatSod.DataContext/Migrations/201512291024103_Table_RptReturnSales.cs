namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table_RptReturnSales : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RptReturnSales",
                c => new
                    {
                        RptReturnSaleID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        Devise = c.String(maxLength: 5),
                        LibDevise = c.String(maxLength: 100),
                        CodeClient = c.String(maxLength: 50),
                        NomClient = c.String(maxLength: 100),
                        CustomerReturnCauses = c.String(),
                        LineQuantity = c.Double(nullable: false),
                        LineAmount = c.Double(nullable: false),
                        ReturnAmount = c.Double(nullable: false),
                        LocalizationCode = c.String(),
                        ProductCode = c.String(),
                        OeilDroiteGauche = c.String(),
                        CustomerReturnDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RptReturnSaleID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RptReturnSales");
        }
    }
}
