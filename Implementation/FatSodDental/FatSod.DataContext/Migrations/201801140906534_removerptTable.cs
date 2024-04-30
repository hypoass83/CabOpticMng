namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removerptTable : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.RptInventories");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RptInventories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BranchName = c.String(),
                        BranchAdress = c.String(),
                        BranchTel = c.String(),
                        CompanyName = c.String(),
                        CompanyAdress = c.String(),
                        CompanyTel = c.String(),
                        CompanyCNI = c.String(),
                        CompanyLogo = c.Binary(),
                        ProductLabel = c.String(),
                        ProductQty = c.Double(nullable: false),
                        ProductUnitPrice = c.Double(nullable: false),
                        Localization = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
    }
}
