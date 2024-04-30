namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Prescription_Entity : DbMigration
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
            DropTable("dbo.Prescriptions");
            DropTable("dbo.PrescriptionLines");
        }
    }
}
