namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_DateRdv_Prescript : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prescriptions", "DateRdv", c => c.DateTime());
            AddColumn("dbo.Prescriptions", "PrescriptionColique", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Prescriptions", "PrescriptionColique");
            DropColumn("dbo.Prescriptions", "DateRdv");
        }
    }
}
