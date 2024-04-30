namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_table_regProductNumber : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RegProductNumberLines",
                c => new
                    {
                        RegProductNumberLineID = c.Int(nullable: false, identity: true),
                        OldProductID = c.Int(nullable: false),
                        NewProductID = c.Int(nullable: false),
                        OldLineQuantity = c.Double(nullable: false),
                        NewLineQuantity = c.Double(nullable: false),
                        LocalizationID = c.Int(nullable: false),
                        RegProductNumberID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RegProductNumberLineID)
                .ForeignKey("dbo.Localizations", t => t.LocalizationID)
                .ForeignKey("dbo.Products", t => t.NewProductID)
                .ForeignKey("dbo.Products", t => t.OldProductID)
                .ForeignKey("dbo.RegProductNumbers", t => t.RegProductNumberID)
                .Index(t => t.OldProductID)
                .Index(t => t.NewProductID)
                .Index(t => t.LocalizationID)
                .Index(t => t.RegProductNumberID);
            
            CreateTable(
                "dbo.RegProductNumbers",
                c => new
                    {
                        RegProductNumberID = c.Int(nullable: false, identity: true),
                        RegProductNumberDate = c.DateTime(nullable: false),
                        RegProductNumberReference = c.String(nullable: false, maxLength: 50),
                        BranchID = c.Int(nullable: false),
                        AutorizedByID = c.Int(),
                        RegisteredByID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RegProductNumberID)
                .ForeignKey("dbo.Users", t => t.AutorizedByID)
                .ForeignKey("dbo.Branches", t => t.BranchID)
                .ForeignKey("dbo.Users", t => t.RegisteredByID)
                .Index(t => t.RegProductNumberReference, unique: true, name: "RegProductNumberReference")
                .Index(t => t.BranchID)
                .Index(t => t.AutorizedByID)
                .Index(t => t.RegisteredByID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegProductNumberLines", "RegProductNumberID", "dbo.RegProductNumbers");
            DropForeignKey("dbo.RegProductNumbers", "RegisteredByID", "dbo.Users");
            DropForeignKey("dbo.RegProductNumbers", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.RegProductNumbers", "AutorizedByID", "dbo.Users");
            DropForeignKey("dbo.RegProductNumberLines", "OldProductID", "dbo.Products");
            DropForeignKey("dbo.RegProductNumberLines", "NewProductID", "dbo.Products");
            DropForeignKey("dbo.RegProductNumberLines", "LocalizationID", "dbo.Localizations");
            DropIndex("dbo.RegProductNumbers", new[] { "RegisteredByID" });
            DropIndex("dbo.RegProductNumbers", new[] { "AutorizedByID" });
            DropIndex("dbo.RegProductNumbers", new[] { "BranchID" });
            DropIndex("dbo.RegProductNumbers", "RegProductNumberReference");
            DropIndex("dbo.RegProductNumberLines", new[] { "RegProductNumberID" });
            DropIndex("dbo.RegProductNumberLines", new[] { "LocalizationID" });
            DropIndex("dbo.RegProductNumberLines", new[] { "NewProductID" });
            DropIndex("dbo.RegProductNumberLines", new[] { "OldProductID" });
            DropTable("dbo.RegProductNumbers");
            DropTable("dbo.RegProductNumberLines");
        }
    }
}
