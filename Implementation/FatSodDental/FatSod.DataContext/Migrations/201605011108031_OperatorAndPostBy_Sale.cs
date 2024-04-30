namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OperatorAndPostBy_Sale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "OperatorID", c => c.Int());
            AddColumn("dbo.Sales", "PostByID", c => c.Int());
            CreateIndex("dbo.Sales", "OperatorID");
            CreateIndex("dbo.Sales", "PostByID");
            AddForeignKey("dbo.Sales", "OperatorID", "dbo.Users", "GlobalPersonID");
            AddForeignKey("dbo.Sales", "PostByID", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "PostByID", "dbo.Users");
            DropForeignKey("dbo.Sales", "OperatorID", "dbo.Users");
            DropIndex("dbo.Sales", new[] { "PostByID" });
            DropIndex("dbo.Sales", new[] { "OperatorID" });
            DropColumn("dbo.Sales", "PostByID");
            DropColumn("dbo.Sales", "OperatorID");
        }
    }
}
