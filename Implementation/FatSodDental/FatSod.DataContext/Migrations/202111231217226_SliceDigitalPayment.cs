namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SliceDigitalPayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Slices", "TransactionIdentifier", c => c.String());
            AddColumn("dbo.Slices", "DigitalAccountManagerId", c => c.Int());
            CreateIndex("dbo.Slices", "DigitalAccountManagerId");
            AddForeignKey("dbo.Slices", "DigitalAccountManagerId", "dbo.Users", "GlobalPersonID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Slices", "DigitalAccountManagerId", "dbo.Users");
            DropIndex("dbo.Slices", new[] { "DigitalAccountManagerId" });
            DropColumn("dbo.Slices", "DigitalAccountManagerId");
            DropColumn("dbo.Slices", "TransactionIdentifier");
        }
    }
}
