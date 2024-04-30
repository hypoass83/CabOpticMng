namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_atcd_entity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ATCDFamiliaux",
                c => new
                    {
                        ATCDFamilialID = c.Int(nullable: false, identity: true),
                        ATCDID = c.Int(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        Remarques = c.String(),
                    })
                .PrimaryKey(t => t.ATCDFamilialID)
                .ForeignKey("dbo.ATCDs", t => t.ATCDID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .Index(t => t.ATCDID)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.ATCDs",
                c => new
                    {
                        ATCDID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ATCDID);
            
            CreateTable(
                "dbo.ATCDPersonnels",
                c => new
                    {
                        ATCDPersonnelID = c.Int(nullable: false, identity: true),
                        ATCDID = c.Int(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        Remarques = c.String(),
                    })
                .PrimaryKey(t => t.ATCDPersonnelID)
                .ForeignKey("dbo.ATCDs", t => t.ATCDID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .Index(t => t.ATCDID)
                .Index(t => t.CustomerID);
            
            AddColumn("dbo.Customers", "Profession", c => c.String(maxLength: 250));
            AddColumn("dbo.Prescriptions", "Plainte", c => c.String());
            AddColumn("dbo.Prescriptions", "isDilation", c => c.Boolean(nullable: false));
            AddColumn("dbo.Prescriptions", "CodeDilation", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ATCDPersonnels", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.ATCDPersonnels", "ATCDID", "dbo.ATCDs");
            DropForeignKey("dbo.ATCDFamiliaux", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.ATCDFamiliaux", "ATCDID", "dbo.ATCDs");
            DropIndex("dbo.ATCDPersonnels", new[] { "CustomerID" });
            DropIndex("dbo.ATCDPersonnels", new[] { "ATCDID" });
            DropIndex("dbo.ATCDFamiliaux", new[] { "CustomerID" });
            DropIndex("dbo.ATCDFamiliaux", new[] { "ATCDID" });
            DropColumn("dbo.Prescriptions", "CodeDilation");
            DropColumn("dbo.Prescriptions", "isDilation");
            DropColumn("dbo.Prescriptions", "Plainte");
            DropColumn("dbo.Customers", "Profession");
            DropTable("dbo.ATCDPersonnels");
            DropTable("dbo.ATCDs");
            DropTable("dbo.ATCDFamiliaux");
        }
    }
}
