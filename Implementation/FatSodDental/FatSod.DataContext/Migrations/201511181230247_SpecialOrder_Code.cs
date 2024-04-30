namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpecialOrder_Code : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SpecialOrders", "Code", c => c.Int(nullable: false));
            CreateIndex("dbo.SpecialOrders", "Code", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.SpecialOrders", new[] { "Code" });
            DropColumn("dbo.SpecialOrders", "Code");
        }
    }
}
