namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PT_AskedByID_Nullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductTransferts", new[] { "AskedByID" });
            AlterColumn("dbo.ProductTransferts", "AskedByID", c => c.Int());
            CreateIndex("dbo.ProductTransferts", "AskedByID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductTransferts", new[] { "AskedByID" });
            AlterColumn("dbo.ProductTransferts", "AskedByID", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductTransferts", "AskedByID");
        }
    }
}
