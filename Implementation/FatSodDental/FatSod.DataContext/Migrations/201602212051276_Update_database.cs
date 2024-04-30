namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_database : DbMigration
    {
        public override void Up()
        {
            //activation du control de stock
            Sql("UPDATE UserConfigurations SET isStockControl=1");
            //coe version test on initaialise ttes les qtes a 10 en attendant de faire l'inventaire
            Sql("UPDATE ProductLocalizations SET ProductLocalizationStockQuantity=20,ProductLocalizationSafetyStockQuantity=5");
            //ajout des nvo menu
            //-menu customer Balances
            //tab SubMenus
            Sql("SET IDENTITY_INSERT SubMenus ON");
            Sql("INSERT INTO SubMenus([SubMenuID],[SubMenuCode],[SubMenuLabel],[SubMenuDescription],[SubMenuController],[SubMenuPath],[IsChortcut],[MenuID]) " +
                "VALUES (60,'SubMnu21CustoBal','Customers Balance','Customers Balance','CustomerBalance','Index',0,4)");
            //liste of special order
            Sql("INSERT INTO SubMenus([SubMenuID],[SubMenuCode],[SubMenuLabel],[SubMenuDescription],[SubMenuController],[SubMenuPath],[IsChortcut],[MenuID]) " +
                "VALUES (61,'SubMnu41SpecialOrder','Special Order Report','Special Order Report','RptSpecialOrder','Index',0,4)");
            //list of special order by customer
            Sql("INSERT INTO SubMenus([SubMenuID],[SubMenuCode],[SubMenuLabel],[SubMenuDescription],[SubMenuController],[SubMenuPath],[IsChortcut],[MenuID]) " +
                "VALUES (62,'SubMnu42SpecialOrderByCusto','Special Order By Customers Report','Special Order By Customers Report','RptSpecialOrderCustomer','Index',0,4)");
            Sql("SET IDENTITY_INSERT SubMenus OFF");
            //tab ActionSubMenuProfiles
            Sql("INSERT INTO ActionSubMenuProfiles([Delete],[Add],[Update],[SubMenuID],[ProfileID]) VALUES (1,1,1,60,2)");
            Sql("INSERT INTO ActionSubMenuProfiles([Delete],[Add],[Update],[SubMenuID],[ProfileID]) VALUES (1,1,1,61,2)");
            Sql("INSERT INTO ActionSubMenuProfiles([Delete],[Add],[Update],[SubMenuID],[ProfileID]) VALUES (1,1,1,62,2)");

        }
        
        public override void Down()
        {
            Sql("Delete From SubMenus Where SubMenuID=60");
            Sql("Delete From ActionSubMenuProfiles Where SubMenuID in (60,61,62)");
        }
    }
}
