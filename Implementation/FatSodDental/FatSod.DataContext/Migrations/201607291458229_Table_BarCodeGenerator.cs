namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table_BarCodeGenerator : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BarCodeGenerators",
                c => new
                    {
                        BarCodeGeneratorID = c.Int(nullable: false, identity: true),
                        CodeBar = c.String(maxLength: 30),
                        ProductID = c.Int(nullable: false),
                        GeneratedByID = c.Int(nullable: false),
                        GenerateDate = c.DateTime(nullable: false),
                        QtyGenerate = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BarCodeGeneratorID)
                .ForeignKey("dbo.Users", t => t.GeneratedByID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .Index(t => t.ProductID)
                .Index(t => t.GeneratedByID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BarCodeGenerators", "ProductID", "dbo.Products");
            DropForeignKey("dbo.BarCodeGenerators", "GeneratedByID", "dbo.Users");
            DropIndex("dbo.BarCodeGenerators", new[] { "GeneratedByID" });
            DropIndex("dbo.BarCodeGenerators", new[] { "ProductID" });
            DropTable("dbo.BarCodeGenerators");
        }
    }
}
