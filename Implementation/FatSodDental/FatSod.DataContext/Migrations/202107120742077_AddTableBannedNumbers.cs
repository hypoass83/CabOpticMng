namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableBannedNumbers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BannedNumbers",
                c => new
                    {
                        BannedNumberId = c.Int(nullable: false, identity: true),
                        Number = c.String(),
                    })
                .PrimaryKey(t => t.BannedNumberId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BannedNumbers");
        }
    }
}
