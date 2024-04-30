namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IX_Consultation_Unique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Consultations", new[] { "CustomerID" });
            CreateIndex("dbo.Consultations", new[] { "CustomerID", "DateConsultation" }, unique: true, name: "IX_Consultation");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Consultations", "IX_Consultation");
            CreateIndex("dbo.Consultations", "CustomerID");
        }
    }
}
