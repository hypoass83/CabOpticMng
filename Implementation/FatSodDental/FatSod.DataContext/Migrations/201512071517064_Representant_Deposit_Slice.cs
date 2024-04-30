namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Representant_Deposit_Slice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Slices", "Representant", c => c.String());
            AddColumn("dbo.Deposits", "Representant", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Deposits", "Representant");
            DropColumn("dbo.Slices", "Representant");
        }
    }
}
