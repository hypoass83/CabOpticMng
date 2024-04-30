namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerReturnSlice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerReturnSlices",
                c => new
                    {
                        SliceID = c.Int(nullable: false),
                        CustomerReturnID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SliceID)
                .ForeignKey("dbo.Slices", t => t.SliceID)
                .ForeignKey("dbo.CustomerReturns", t => t.CustomerReturnID)
                .Index(t => t.SliceID)
                .Index(t => t.CustomerReturnID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerReturnSlices", "CustomerReturnID", "dbo.CustomerReturns");
            DropForeignKey("dbo.CustomerReturnSlices", "SliceID", "dbo.Slices");
            DropIndex("dbo.CustomerReturnSlices", new[] { "CustomerReturnID" });
            DropIndex("dbo.CustomerReturnSlices", new[] { "SliceID" });
            DropTable("dbo.CustomerReturnSlices");
        }
    }
}
