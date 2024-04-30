using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;

namespace FatSod.DataContext.Initializer
{
    internal static partial class Parameters
    {
        //********** Sale Module
        private static Module _Sale = new Module()
        {
            ModuleCode = "MODULE_1",
            ModuleLabel = "Vente",
            ModuleDescription = "Permet 'administration des ventes dans l'entreprise",
            ModuleArea = "Sale",
            ModuleState = true,
            ModuleImageHeight = 28,
            ModuleImageWeight = 60,
            ModulePressedImagePath = "MOD_PRESSED_IMG_SALE",
            ModuleImagePath = "MOD_IMG_SALE",
            ModuleDisabledImagePath = "MOD_DISABLED_IMG_SALE"
            //ModulePressedImagePath = "~/Content/Images/App/Modules/sale.png",
            //ModuleImagePath = "~/Content/Images/App/Modules/sale.png",
            //ModuleDisabledImagePath = "~/Content/Images/App/Modules/Pressed/salepressed.png",
        };

        //********** Sale Menu
        private static Menu _Sale_Customer = new Menu()
        {
            MenuLabel = "Client",
            MenuCode = CodeValue.Sale.Customer.CODE,
            MenuDescription = "Ensemble des clients de l'entreprise",
            MenuFlat = true,
            MenuIconName = "customers.png",
            MenuController = "Customer",
            MenuPath = "Index",
            MenuState = true,
            Module = _Sale,
            ModuleID = _Sale.ModuleID
        };
        private static Menu _Sale_Sale = new Menu()
        {
            MenuLabel = "Vente",
            MenuCode = CodeValue.Sale.NewSale.CODE,
            MenuDescription = "Définir les plannig importantes",
            MenuFlat = true,
            MenuIconName = "sales.png",
            MenuController = "Null",
            MenuPath = "Null",
            MenuState = true,
            Module = _Sale,
            ModuleID = _Sale.ModuleID
        };
        private static Menu _Sale_Report = new Menu()
        {
            MenuLabel = "Sale Report",
            MenuCode = CodeValue.Sale.Report.CODE,
            MenuDescription = "Définir les etats de client",
            MenuFlat = true,
            MenuIconName = "report.png",
            MenuController = "Null",
            MenuPath = "Null",
            MenuState = true,
            Module = _Sale,
            ModuleID = _Sale.ModuleID
        };

        //Special order Menu for Magasinier 
        private static Menu _Sale_SpecialOrder = new Menu()
        {
            MenuLabel = "Special Order",
            MenuCode = CodeValue.Sale.NewSale.Menu_SaleSpecialOrder,
            MenuDescription = "Post special order and receive product of special order from supplier",
            MenuFlat = true,
            MenuIconName = "valSale.png",
            MenuController = null,
            MenuPath = null,
            MenuState = true,
            Module = _Sale,
            ModuleID = _Sale.ModuleID,
        };

        //Price Menu for ?? 
        private static Menu _Sale_Pricing = new Menu()
        {
            MenuLabel = "Pricing",
            MenuCode = CodeValue.Sale.NewSale.Pricing,
            MenuDescription = "Create Lens Number Ranges and Pricing",
            MenuFlat = true,
            MenuIconName = "valSale.png",
            MenuController = null,
            MenuPath = null,
            MenuState = true,
            Module = _Sale,
            ModuleID = _Sale.ModuleID,
        };

        //************* _Sale_Sale subMenus
        static SubMenu _Sale_Sale_Command = new SubMenu()
        {
            Menu = _Sale_Sale,
            MenuID = _Sale_Sale.MenuID,
            SubMenuCode = CodeValue.Sale.NewSale.C_CODE,
            SubMenuDescription = "Effectuer une nouvelle vente",
            SubMenuLabel = "Nouvelle Vente",
            SubMenuController = "Command",
            SubMenuPath = "Index"

        };
        public static SubMenu _Sale_Sale_OrderLensOrder = new SubMenu()
        {
            Menu = _Sale_SpecialOrder,
            MenuID = _Sale_SpecialOrder.MenuID,
            SubMenuCode = CodeValue.Sale.NewSale.OrderLensOrder,
            SubMenuDescription = "Effectuer une nouvelle Commande d'un verre de commande",
            SubMenuLabel = "Nouvelle Commande d'un Verre de Commande",
            SubMenuController = "OrderLensOrder",
            SubMenuPath = "OrderLensOrder"
        };
        public static SubMenu _Sale_Sale_ReceiveSpecialOrder = new SubMenu()
        {
            Menu = _Sale_SpecialOrder,
            MenuID = _Sale_SpecialOrder.MenuID,
            SubMenuCode = CodeValue.Sale.NewSale.ReceiveSpecialOrder,
            SubMenuDescription = "Reception des commande de verre de commande venant du fournisseur",
            SubMenuLabel = "Reception des verres de commande du fiurnisseur",
            SubMenuController = "ReceiveSpecialOrder",
            SubMenuPath = "ReceiveSpecialOrder"
        };
        public static SubMenu _Sale_Sale_PostedSpecialOrderToSupplier = new SubMenu()
        {
            Menu = _Sale_SpecialOrder,
            MenuID = _Sale_SpecialOrder.MenuID,
            SubMenuCode = CodeValue.Sale.NewSale.PostedSpecialOrderToSupplier,
            SubMenuDescription = "Permet d'envoyer la commande chez le fournisseur",
            SubMenuLabel = "Envoie de la commande des verres de commande au fourrnisseur",
            SubMenuController = "PostedToSupplier",
            SubMenuPath = "PostedToSupplier"
        };

        public static SubMenu _Sale_Pricing_Range = new SubMenu()
        {
            Menu = _Sale_Pricing,
            MenuID = _Sale_Pricing.MenuID,
            SubMenuCode = CodeValue.Sale.NewSale.RangeNumber,
            SubMenuDescription = "Permet de crééer les intervalles de prix qui seront utilisées pour la fixation des prix de vente",
            SubMenuLabel = "Range Number",
            SubMenuController = "RangeNumber",
            SubMenuPath = "RangeNumber"
        };

        public static SubMenu _Sale_Pricing_LensPrice = new SubMenu()
        {
            Menu = _Sale_Pricing,
            MenuID = _Sale_Pricing.MenuID,
            SubMenuCode = CodeValue.Sale.NewSale.Price,
            SubMenuDescription = "Permet de fixer le prix d'une intervalle de numéro",
            SubMenuLabel = "Price",
            SubMenuController = "LensPrice",
            SubMenuPath = "LensPrice"
        };

        static SubMenu _Sale_Sale_NewSale = new SubMenu()
        {
            Menu = _Sale_Sale,
            MenuID = _Sale_Sale.MenuID,
            SubMenuCode = CodeValue.Sale.NewSale.S_CODE,
            SubMenuDescription = "Effectuer une nouvelle vente",
            SubMenuLabel = "Nouvelle Vente",
            IsChortcut = true,
            SubMenuController = "Sale",
            SubMenuPath = "Index"
        };
        static SubMenu _Sale_Sale_SaleReturn = new SubMenu()
        {
            Menu = _Sale_Sale,
            MenuID = _Sale_Sale.MenuID,
            SubMenuCode = CodeValue.Sale.NewSale.R_CODE,
            SubMenuDescription = "Permet la gestion des retours de marchandises",
            SubMenuLabel = "Retour Marchandise",
            SubMenuController = "SaleReturn",
            SubMenuPath = "Index"

        };
        private static SubMenu _Sale_Report_ListClt = new SubMenu()
        {
            SubMenuLabel = "List of Customer",
            SubMenuCode = CodeValue.Sale.Report.SMCODECLT,
            SubMenuDescription = "List of Customer",
            SubMenuController = "ListCustomer",
            SubMenuPath = "Index",
            Menu = _Sale_Report,
            MenuID = _Sale_Report.MenuID
        };
        private static SubMenu _Sale_Report_HistoClt = new SubMenu()
        {
            SubMenuLabel = "Customer History",
            SubMenuCode = CodeValue.Sale.Report.SMCODECUSTHIST,
            SubMenuDescription = "Customer History",
            SubMenuController = "CustomerHistory",
            SubMenuPath = "Index",
            Menu = _Sale_Report,
            MenuID = _Sale_Report.MenuID
        };
        private static SubMenu _Sale_Report_ReturnSale = new SubMenu()
        {
            SubMenuLabel = "Return Sale",
            SubMenuCode = CodeValue.Sale.Report.SRCODERETSALE,
            SubMenuDescription = "Return Sale",
            SubMenuController = "RptReturnSale",
            SubMenuPath = "Index",
            Menu = _Sale_Report,
            MenuID = _Sale_Report.MenuID
        };
    }
}
