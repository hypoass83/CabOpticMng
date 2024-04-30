namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table_RptSpecialOrder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RptSpecialOrders",
                c => new
                    {
                        RptSpecialOrderID = c.Int(nullable: false, identity: true),
                        Agence = c.String(maxLength: 50),
                        LibAgence = c.String(maxLength: 100),
                        Devise = c.String(maxLength: 3),
                        LibDevise = c.String(maxLength: 100),
                        CustomerOrderDate = c.DateTime(nullable: false),
                        CustomerOrderTotalPrice = c.Double(nullable: false),
                        CustomerOrderNumber = c.String(),
                        CustomerName = c.String(),
                        CodeClient = c.String(),
                        NomClient = c.String(),
                        OrderStatut = c.String(),
                        Code = c.Int(nullable: false),
                        ValidatedDate = c.DateTime(),
                        DeliveredDate = c.DateTime(),
                        PostedToSupplierDate = c.DateTime(),
                        SaleDate = c.DateTime(),
                        ReceivedDate = c.DateTime(),
                        AdvancedAmount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.RptSpecialOrderID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RptSpecialOrders");
        }
    }
}
