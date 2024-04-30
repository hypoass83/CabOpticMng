namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class New_modif_customernum_with_Prescription : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrescriptionLines",
                c => new
                    {
                        PrescriptionLineID = c.Int(nullable: false, identity: true),
                        PrescriptionID = c.Int(nullable: false),
                        SpecialOrderLineCode = c.String(),
                        Axis = c.String(),
                        Addition = c.String(),
                        Index = c.String(),
                        LensNumberCylindricalValue = c.String(),
                        LensNumberSphericalValue = c.String(),
                        ProductID = c.Int(nullable: false),
                        OeilDroiteGauche = c.Int(nullable: false),
                        SupplyingName = c.String(),
                    })
                .PrimaryKey(t => t.PrescriptionLineID)
                .ForeignKey("dbo.Prescriptions", t => t.PrescriptionID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .Index(t => t.PrescriptionID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Prescriptions",
                c => new
                    {
                        PrescriptionID = c.Int(nullable: false, identity: true),
                        DateHeurePrescription = c.DateTime(nullable: false),
                        DatePrescription = c.DateTime(nullable: false),
                        isPrescriptionValidate = c.Boolean(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        OperatorID = c.Int(),
                        PostByID = c.Int(),
                        Remarque = c.String(),
                        MedecinTraitant = c.String(),
                    })
                .PrimaryKey(t => t.PrescriptionID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .ForeignKey("dbo.Users", t => t.OperatorID)
                .ForeignKey("dbo.Users", t => t.PostByID)
                .Index(t => t.CustomerID)
                .Index(t => t.OperatorID)
                .Index(t => t.PostByID);
            
            AddColumn("dbo.Customers", "CustomerNumber", c => c.String(maxLength: 10));
            AddColumn("dbo.Customers", "isPrescritionValidate", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sales", "DateRdv", c => c.DateTime());
            AddColumn("dbo.AuthoriseSales", "DateRdv", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PrescriptionLines", "ProductID", "dbo.Products");
            DropForeignKey("dbo.PrescriptionLines", "PrescriptionID", "dbo.Prescriptions");
            DropForeignKey("dbo.Prescriptions", "PostByID", "dbo.Users");
            DropForeignKey("dbo.Prescriptions", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.Prescriptions", "CustomerID", "dbo.Customers");
            DropIndex("dbo.Prescriptions", new[] { "PostByID" });
            DropIndex("dbo.Prescriptions", new[] { "OperatorID" });
            DropIndex("dbo.Prescriptions", new[] { "CustomerID" });
            DropIndex("dbo.PrescriptionLines", new[] { "ProductID" });
            DropIndex("dbo.PrescriptionLines", new[] { "PrescriptionID" });
            DropColumn("dbo.AuthoriseSales", "DateRdv");
            DropColumn("dbo.Sales", "DateRdv");
            DropColumn("dbo.Customers", "isPrescritionValidate");
            DropColumn("dbo.Customers", "CustomerNumber");
            DropTable("dbo.Prescriptions");
            DropTable("dbo.PrescriptionLines");
        }
    }
}
