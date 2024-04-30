namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_table_Operation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Operations", "MacroOperationID", "dbo.MacroOperations");
            DropForeignKey("dbo.Operations", "ReglementTypeID", "dbo.ReglementTypes");
            DropIndex("dbo.Operations", new[] { "MacroOperationID" });
            DropIndex("dbo.Operations", new[] { "ReglementTypeID" });
            DropIndex("dbo.MacroOperations", new[] { "MacroOperationCode" });
            DropIndex("dbo.ReglementTypes", new[] { "ReglementTypeCode" });
            DropColumn("dbo.Operations", "MacroOperationID");
            DropColumn("dbo.Operations", "ReglementTypeID");
            DropTable("dbo.MacroOperations");
            DropTable("dbo.ReglementTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ReglementTypes",
                c => new
                    {
                        ReglementTypeID = c.Int(nullable: false, identity: true),
                        ReglementTypeCode = c.String(maxLength: 30),
                        ReglementTypeLabel = c.String(),
                        ReglementTypeDescription = c.String(),
                    })
                .PrimaryKey(t => t.ReglementTypeID);
            
            CreateTable(
                "dbo.MacroOperations",
                c => new
                    {
                        MacroOperationID = c.Int(nullable: false, identity: true),
                        MacroOperationCode = c.String(maxLength: 30),
                        MacroOperationLabel = c.String(),
                        MacroOperationDescription = c.String(),
                    })
                .PrimaryKey(t => t.MacroOperationID);
            
            AddColumn("dbo.Operations", "ReglementTypeID", c => c.Int(nullable: false));
            AddColumn("dbo.Operations", "MacroOperationID", c => c.Int(nullable: false));
            CreateIndex("dbo.ReglementTypes", "ReglementTypeCode", unique: true);
            CreateIndex("dbo.MacroOperations", "MacroOperationCode", unique: true);
            CreateIndex("dbo.Operations", "ReglementTypeID");
            CreateIndex("dbo.Operations", "MacroOperationID");
            AddForeignKey("dbo.Operations", "ReglementTypeID", "dbo.ReglementTypes", "ReglementTypeID");
            AddForeignKey("dbo.Operations", "MacroOperationID", "dbo.MacroOperations", "MacroOperationID");
        }
    }
}
