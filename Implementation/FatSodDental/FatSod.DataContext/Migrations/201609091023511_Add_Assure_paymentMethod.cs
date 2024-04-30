namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Assure_paymentMethod : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssureurPMs",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        AssureurID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PaymentMethods", t => t.ID)
                .ForeignKey("dbo.Assureurs", t => t.AssureurID)
                .Index(t => t.ID)
                .Index(t => t.AssureurID);
            
            CreateTable(
                "dbo.AssureurSales",
                c => new
                    {
                        SaleID = c.Int(nullable: false),
                        AssureurPMID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SaleID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .ForeignKey("dbo.AssureurPMs", t => t.AssureurPMID)
                .Index(t => t.SaleID)
                .Index(t => t.AssureurPMID);
            
            AddColumn("dbo.Assureurs", "AccountID", c => c.Int(nullable: false));
            AddColumn("dbo.CustomerOrders", "CompteurFacture", c => c.Int(nullable: false));
            CreateIndex("dbo.Assureurs", "AccountID");
            AddForeignKey("dbo.Assureurs", "AccountID", "dbo.Accounts", "AccountID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssureurSales", "AssureurPMID", "dbo.AssureurPMs");
            DropForeignKey("dbo.AssureurSales", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.Assureurs", "AccountID", "dbo.Accounts");
            DropForeignKey("dbo.AssureurPMs", "AssureurID", "dbo.Assureurs");
            DropForeignKey("dbo.AssureurPMs", "ID", "dbo.PaymentMethods");
            DropIndex("dbo.AssureurSales", new[] { "AssureurPMID" });
            DropIndex("dbo.AssureurSales", new[] { "SaleID" });
            DropIndex("dbo.Assureurs", new[] { "AccountID" });
            DropIndex("dbo.AssureurPMs", new[] { "AssureurID" });
            DropIndex("dbo.AssureurPMs", new[] { "ID" });
            DropColumn("dbo.CustomerOrders", "CompteurFacture");
            DropColumn("dbo.Assureurs", "AccountID");
            DropTable("dbo.AssureurSales");
            DropTable("dbo.AssureurPMs");
        }
    }
}
