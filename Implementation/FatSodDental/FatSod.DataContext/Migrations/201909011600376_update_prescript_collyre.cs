namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_prescript_collyre : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prescriptions", "PrescriptionCollyre", c => c.Boolean(nullable: false));
            AddColumn("dbo.Prescriptions", "CollyreName", c => c.String());
            DropColumn("dbo.Prescriptions", "PrescriptionColique");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Prescriptions", "PrescriptionColique", c => c.Boolean(nullable: false));
            DropColumn("dbo.Prescriptions", "CollyreName");
            DropColumn("dbo.Prescriptions", "PrescriptionCollyre");
        }
    }
}
