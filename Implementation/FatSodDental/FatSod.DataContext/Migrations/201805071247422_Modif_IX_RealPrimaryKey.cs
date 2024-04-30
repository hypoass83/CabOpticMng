namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modif_IX_RealPrimaryKey : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ExtractSMS", "IX_RealPrimaryKey");
            CreateIndex("dbo.ExtractSMS", new[] { "CustomerID", "AlertDescrip", "TypeAlert", "Condition", "SendSMSDate", "SaleDeliveryDate" }, unique: true, name: "IX_RealPrimaryKey");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ExtractSMS", "IX_RealPrimaryKey");
            CreateIndex("dbo.ExtractSMS", new[] { "CustomerID", "AlertDescrip", "TypeAlert", "Condition", "SendSMSDate" }, unique: true, name: "IX_RealPrimaryKey");
        }
    }
}
