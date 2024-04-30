namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Table_HistoSMS : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HistoSMS",
                c => new
                    {
                        HistoSMSID = c.Int(nullable: false, identity: true),
                        NbreSMS = c.Int(nullable: false),
                        DateEnvoi = c.DateTime(nullable: false),
                        SmsEnvoye = c.String(),
                        TypeSms = c.String(),
                        OperatorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HistoSMSID)
                .ForeignKey("dbo.Users", t => t.OperatorID)
                .Index(t => t.OperatorID);
            
            AddColumn("dbo.HistoRendezVous", "OperatorID", c => c.Int());
            CreateIndex("dbo.HistoRendezVous", "OperatorID");
            AddForeignKey("dbo.HistoRendezVous", "OperatorID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HistoSMS", "OperatorID", "dbo.Users");
            DropForeignKey("dbo.HistoRendezVous", "OperatorID", "dbo.Users");
            DropIndex("dbo.HistoSMS", new[] { "OperatorID" });
            DropIndex("dbo.HistoRendezVous", new[] { "OperatorID" });
            DropColumn("dbo.HistoRendezVous", "OperatorID");
            DropTable("dbo.HistoSMS");
        }
    }
}
