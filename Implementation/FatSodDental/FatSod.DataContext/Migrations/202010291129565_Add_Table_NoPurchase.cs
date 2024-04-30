namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_NoPurchase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NoPurchases",
                c => new
                    {
                        NoPurchaseId = c.Int(nullable: false, identity: true),
                        ConsultDilatationId = c.Int(),
                        ConsultLensPrescriptionID = c.Int(),
                        OperationDate = c.DateTime(nullable: false),
                        ConsultationDate = c.DateTime(nullable: false),
                        Reason = c.String(nullable: false),
                        HasBeenPurchased = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.NoPurchaseId)
                .ForeignKey("dbo.ConsultDilatations", t => t.ConsultDilatationId)
                .ForeignKey("dbo.ConsultLensPrescriptions", t => t.ConsultLensPrescriptionID)
                .Index(t => t.ConsultDilatationId)
                .Index(t => t.ConsultLensPrescriptionID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NoPurchases", "ConsultLensPrescriptionID", "dbo.ConsultLensPrescriptions");
            DropForeignKey("dbo.NoPurchases", "ConsultDilatationId", "dbo.ConsultDilatations");
            DropIndex("dbo.NoPurchases", new[] { "ConsultLensPrescriptionID" });
            DropIndex("dbo.NoPurchases", new[] { "ConsultDilatationId" });
            DropTable("dbo.NoPurchases");
        }
    }
}
