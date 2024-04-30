namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BarcodeInventory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InventoryCountingLines",
                c => new
                    {
                        InventoryCountingLineId = c.Int(nullable: false, identity: true),
                        CountedQuantity = c.Double(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                        StockId = c.Int(nullable: false),
                        AuthorizedById = c.Int(nullable: false),
                        CountedById = c.Int(nullable: false),
                        RegisteredById = c.Int(nullable: false),
                        InventoryCountingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryCountingLineId)
                .ForeignKey("dbo.Users", t => t.AuthorizedById)
                .ForeignKey("dbo.Users", t => t.CountedById)
                .ForeignKey("dbo.InventoryCountings", t => t.InventoryCountingId)
                .ForeignKey("dbo.Users", t => t.RegisteredById)
                .ForeignKey("dbo.ProductLocalizations", t => t.StockId)
                .Index(t => t.StockId)
                .Index(t => t.AuthorizedById)
                .Index(t => t.CountedById)
                .Index(t => t.RegisteredById)
                .Index(t => t.InventoryCountingId);
            
            CreateTable(
                "dbo.InventoryCountings",
                c => new
                    {
                        InventoryCountingId = c.Int(nullable: false, identity: true),
                        Reference = c.String(),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ClosedDate = c.DateTime(),
                        BranchId = c.Int(nullable: false),
                        AuthorizedById = c.Int(nullable: false),
                        CreatedById = c.Int(),
                    })
                .PrimaryKey(t => t.InventoryCountingId)
                .ForeignKey("dbo.Users", t => t.AuthorizedById)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Users", t => t.CreatedById)
                .Index(t => t.BranchId)
                .Index(t => t.AuthorizedById)
                .Index(t => t.CreatedById);
            
            CreateTable(
                "dbo.InventoryReconciliationLines",
                c => new
                    {
                        InventoryReconciliationLineId = c.Int(nullable: false, identity: true),
                        ReconciliationQuantity = c.Double(nullable: false),
                        ReconciliationComment = c.String(),
                        StockId = c.Int(nullable: false),
                        InventoryReconciliationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryReconciliationLineId)
                .ForeignKey("dbo.InventoryReconciliations", t => t.InventoryReconciliationId)
                .ForeignKey("dbo.ProductLocalizations", t => t.StockId)
                .Index(t => t.StockId)
                .Index(t => t.InventoryReconciliationId);
            
            CreateTable(
                "dbo.InventoryReconciliations",
                c => new
                    {
                        InventoryReconciliationId = c.Int(nullable: false, identity: true),
                        ReconciliationDate = c.DateTime(nullable: false),
                        AuthorizedById = c.Int(nullable: false),
                        RegisteredById = c.Int(nullable: false),
                        InventoryCountingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryReconciliationId)
                .ForeignKey("dbo.Users", t => t.AuthorizedById)
                .ForeignKey("dbo.InventoryCountings", t => t.InventoryCountingId)
                .ForeignKey("dbo.Users", t => t.RegisteredById)
                .Index(t => t.AuthorizedById)
                .Index(t => t.RegisteredById)
                .Index(t => t.InventoryCountingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryReconciliationLines", "StockId", "dbo.ProductLocalizations");
            DropForeignKey("dbo.InventoryReconciliations", "RegisteredById", "dbo.Users");
            DropForeignKey("dbo.InventoryReconciliationLines", "InventoryReconciliationId", "dbo.InventoryReconciliations");
            DropForeignKey("dbo.InventoryReconciliations", "InventoryCountingId", "dbo.InventoryCountings");
            DropForeignKey("dbo.InventoryReconciliations", "AuthorizedById", "dbo.Users");
            DropForeignKey("dbo.InventoryCountingLines", "StockId", "dbo.ProductLocalizations");
            DropForeignKey("dbo.InventoryCountingLines", "RegisteredById", "dbo.Users");
            DropForeignKey("dbo.InventoryCountingLines", "InventoryCountingId", "dbo.InventoryCountings");
            DropForeignKey("dbo.InventoryCountings", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.InventoryCountings", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.InventoryCountings", "AuthorizedById", "dbo.Users");
            DropForeignKey("dbo.InventoryCountingLines", "CountedById", "dbo.Users");
            DropForeignKey("dbo.InventoryCountingLines", "AuthorizedById", "dbo.Users");
            DropIndex("dbo.InventoryReconciliations", new[] { "InventoryCountingId" });
            DropIndex("dbo.InventoryReconciliations", new[] { "RegisteredById" });
            DropIndex("dbo.InventoryReconciliations", new[] { "AuthorizedById" });
            DropIndex("dbo.InventoryReconciliationLines", new[] { "InventoryReconciliationId" });
            DropIndex("dbo.InventoryReconciliationLines", new[] { "StockId" });
            DropIndex("dbo.InventoryCountings", new[] { "CreatedById" });
            DropIndex("dbo.InventoryCountings", new[] { "AuthorizedById" });
            DropIndex("dbo.InventoryCountings", new[] { "BranchId" });
            DropIndex("dbo.InventoryCountingLines", new[] { "InventoryCountingId" });
            DropIndex("dbo.InventoryCountingLines", new[] { "RegisteredById" });
            DropIndex("dbo.InventoryCountingLines", new[] { "CountedById" });
            DropIndex("dbo.InventoryCountingLines", new[] { "AuthorizedById" });
            DropIndex("dbo.InventoryCountingLines", new[] { "StockId" });
            DropTable("dbo.InventoryReconciliations");
            DropTable("dbo.InventoryReconciliationLines");
            DropTable("dbo.InventoryCountings");
            DropTable("dbo.InventoryCountingLines");
        }
    }
}
