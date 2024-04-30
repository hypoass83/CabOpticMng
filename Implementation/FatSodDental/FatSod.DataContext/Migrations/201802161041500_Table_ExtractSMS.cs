namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table_ExtractSMS : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExtractSMS",
                c => new
                    {
                        ExtractSMSID = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        CustomerPhone = c.String(),
                        CustomerQuater = c.String(),
                        isSmsSent = c.Boolean(nullable: false),
                        SaleDeliveryDate = c.DateTime(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        AlertDescrip = c.String(nullable: false, maxLength: 50),
                        TypeAlert = c.String(nullable: false, maxLength: 50),
                        Condition = c.String(nullable: false, maxLength: 50),
                        SendSMSDate = c.DateTime(nullable: false),
                        isDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ExtractSMSID)
                .Index(t => new { t.CustomerID, t.AlertDescrip, t.TypeAlert, t.Condition, t.SendSMSDate }, unique: true, name: "IX_RealPrimaryKey");
            
            AddColumn("dbo.Lines", "SupplyingName", c => c.String());
        }
        
        public override void Down()
        {
            DropIndex("dbo.ExtractSMS", "IX_RealPrimaryKey");
            DropColumn("dbo.Lines", "SupplyingName");
            DropTable("dbo.ExtractSMS");
        }
    }
}
