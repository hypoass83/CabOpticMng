namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Table_RptBareCodes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RptBareCodes",
                c => new
                    {
                        RptBareCodeID = c.Int(nullable: false, identity: true),
                        BareCode = c.String(maxLength: 10),
                        ProductName = c.String(maxLength: 100),
                        ProductDescription = c.String(maxLength: 100),
                        Price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.RptBareCodeID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RptBareCodes");
        }
    }
}
