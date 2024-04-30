namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Partners : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Partners",
                c => new
                    {
                        PartnerId = c.Int(nullable: false, identity: true),
                        PartnerCode = c.String(),
                        FullName = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                        Function = c.String(),
                        Company = c.String(),
                        ProductsAndServices = c.String(),
                    })
                .PrimaryKey(t => t.PartnerId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Partners");
        }
    }
}
