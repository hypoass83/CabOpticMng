namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sale_RendezVous_SO_Variable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "IsRendezVous", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sales", "AwaitingDay", c => c.Int());
            AddColumn("dbo.Sales", "IsProductReveive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sales", "EffectiveReceiveDate", c => c.DateTime());
            AddColumn("dbo.Sales", "IsCustomerRceive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sales", "CustomerDeliverDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "CustomerDeliverDate");
            DropColumn("dbo.Sales", "IsCustomerRceive");
            DropColumn("dbo.Sales", "EffectiveReceiveDate");
            DropColumn("dbo.Sales", "IsProductReveive");
            DropColumn("dbo.Sales", "AwaitingDay");
            DropColumn("dbo.Sales", "IsRendezVous");
        }
    }
}
