namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_CSOperationDate_To_NoPurchase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NoPurchases", "CSOperationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.NoPurchases", "CSOperationDate");
        }
    }
}
