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
        //********** Cash Register Module
        private static Module _CashRegister = new Module()
        {

            ModuleCode = CodeValue.CashRegister.MODULE_CODE,
            ModuleLabel = CodeValue.CashRegister.MODULE_CODE,
            ModuleDescription = "Permet 'administration des ventes dans l'entreprise",
            ModuleArea = "CashRegister",
            ModuleState = true,
            ModuleImageHeight = 28,
            ModuleImageWeight = 60,
            ModulePressedImagePath = "MOD_PRESSED_IMG_CAISSE",
            ModuleImagePath = "MOD_IMG_CAISSE",
            ModuleDisabledImagePath = "MOD_DISABLED_IMG_CAISSE"
        };
        //********** Open cash Menu
        private static Menu _CashRegister_Open = new Menu()
        {
            MenuLabel = "CashRegister",
            MenuCode = CodeValue.CashRegister.MENU_OPEN_CODE,
            MenuDescription = "Ensemble des clients de l'entreprise",
            MenuFlat = true,
            MenuIconName = "openTill.png",
            MenuController = "CashRegister",
            MenuPath = "Open",
            MenuState = true,
            Module = _CashRegister,
            ModuleID = _CashRegister.ModuleID
        };
        //********** Open cash Menu
        private static Menu _CashRegister_Close = new Menu()
        {
            MenuLabel = "CashRegister",
            MenuCode = CodeValue.CashRegister.MENU_CLOSE_CODE,
            MenuDescription = "Ensemble des clients de l'entreprise",
            MenuFlat = true,
            MenuIconName = "closeTill.png",
            MenuController = "CashRegister",
            MenuPath = "Close",
            IsChortcut = true,
            MenuState = true,
            Module = _CashRegister,
            ModuleID = _CashRegister.ModuleID
        };
        //********* Validate sale
        private static Menu _CashRegister_Validate_Sale = new Menu()
        {
            MenuLabel = "CashRegister",
            MenuCode = CodeValue.CashRegister.MENU_VALIDATE_SALE,
            MenuDescription = "Validation de la vente",
            MenuFlat = true,
            MenuIconName = "valSale.png",
            MenuController = "Validate",
            MenuPath = "Sale",
            IsChortcut = true,
            MenuState = true,
            Module = _CashRegister,
            ModuleID = _CashRegister.ModuleID
        };
        //********* Validate purchase
        private static Menu _CashRegister_Validate_Purchase = new Menu()
        {
            MenuLabel = "Validate A Purchase",
            MenuCode = CodeValue.CashRegister.MENU_VALIDATE_PURCHASE,
            MenuDescription = "Validate purchase",
            MenuFlat = true,
            MenuIconName = "product.png",
            MenuController = "PurchaseValidation",
            MenuPath = "PurchaseValidation",
            IsChortcut = true,
            MenuState = true,
            Module = _CashRegister,
            ModuleID = _CashRegister.ModuleID
        };
        private static Menu _CashRegister_State = new Menu()
        {
            MenuLabel = "CashRegister",
            MenuCode = CodeValue.CashRegister.MENU_STATE_CASH,
            MenuDescription = "State of cash register",
            MenuFlat = true,
            MenuIconName = "report.png",
            MenuController = "State",
            MenuPath = "Index",
            IsChortcut = true,
            MenuState = true,
            Module = _CashRegister,
            ModuleID = _CashRegister.ModuleID
        };
        
        //********* Dépôt d'argent sur le compte d'un client en vue du règlement d'une vente
        private static Menu _CashRegister_Deposit = new Menu()
        {
            MenuLabel = CodeValue.Supply.DepositMenu.LABEL,
            MenuCode = CodeValue.Supply.DepositMenu.CODE,
            MenuDescription = CodeValue.Supply.DepositMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.DepositMenu.FLAT,
            MenuIconName = CodeValue.Supply.DepositMenu.ICON_NAME,
            MenuController = CodeValue.Supply.DepositMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.DepositMenu.PATH,
            //IsChortcut = CodeValue.Supply.DepositMenu.sh,
            MenuState = CodeValue.Supply.DepositMenu.STATE,
            Module = _CashRegister,
            ModuleID = _CashRegister.ModuleID
        };
        private static Menu _CashRegister_Budget = new Menu()
        {
            MenuLabel = "Budget Expense",
            MenuCode = CodeValue.CashRegister.CODEBudgetExpense,
            MenuDescription = "Budget Expense",
            MenuFlat = true,
            MenuIconName = "business.png",
            MenuController = "BudgetExpense",
            MenuPath = "Index",
            MenuState = true,
            Module = _CashRegister,
            ModuleID = _CashRegister.ModuleID
        };

        //Treasury
        private static Menu _CashRegister_Treasury = new Menu()
        {
            MenuLabel = "Treasury Operation",
            MenuCode = CodeValue.CashRegister.CODETreasury,
            MenuDescription = "Treasury Operation",
            MenuFlat = true,
            MenuIconName = "product.png",
            MenuController = CodeValue.Supply.ProductTransfertMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.ProductTransfertMenu.PATH,
            MenuState = true,
            Module = _CashRegister,
            ModuleID = _CashRegister.ModuleID,
        };
        private static Menu _TillAjustment = new Menu()
        {
            MenuLabel = "Till Adjustment",
            MenuCode = CodeValue.CashRegister.CODETILLADJUST,
            MenuDescription = "Till Adjustment",
            MenuFlat = true,
            MenuIconName = "closeTill.png",
            MenuController = "TillAdjust",
            MenuPath = "Index",
            MenuState = true,
            Module = _CashRegister,
            ModuleID = _CashRegister.ModuleID
        };
        //Special order Menu for cash Reception
        private static Menu _CashRegister_CashierSpecialOrder = new Menu()
        {
            MenuLabel = "Special Order",
            MenuCode = CodeValue.CashRegister.Menu_CashierSpecialOrder,
            MenuDescription = "Money reception from customer for special order and product delivrery",
            MenuFlat = true,
            MenuIconName = "valSale.png",
            MenuController = null,
            MenuPath = null,
            MenuState = true,
            Module = _CashRegister,
            ModuleID = _CashRegister.ModuleID,
        };

        //=============== Sub menu of report
        static SubMenu _CashRegister_State_Hisotric = new SubMenu()
        {
            Menu = _CashRegister_State,
            MenuID = _CashRegister_State.MenuID,
            SubMenuCode = CodeValue.CashRegister.SUBMENU_STATE_CASH_HISOTRIC,
            SubMenuDescription = "Enable to show all cash historic operations",
            SubMenuLabel = "Cash historic",
            SubMenuController = "State",
            SubMenuPath = "Index"

        };
        static SubMenu _CashRegister_State_Bill = new SubMenu()
        {
            Menu = _CashRegister_State,
            MenuID = _CashRegister_State.MenuID,
            SubMenuCode =  CodeValue.CashRegister.SUBMENU_STATE_CASH_BILL,
            SubMenuDescription = "Enable to print customers's bill",
            SubMenuLabel = "Customer bill",
            SubMenuController = "State",
            SubMenuPath = "Bill"

        };

        static SubMenu _CashRegister_CashTransmission = new SubMenu()
        {
            Menu = _CashRegister_Treasury,
            MenuID = _CashRegister_Treasury.MenuID,
            SubMenuCode = CodeValue.CashRegister.SUBMENU_CASHTRANSMISSION,
            SubMenuDescription = "To Send Money from one Cash Register to another",
            SubMenuLabel = "Cash Transmission",
            SubMenuController = "CashTransmission",
            SubMenuPath = "Index"

        };

        static SubMenu _CashRegister_CashReception = new SubMenu()
        {
            Menu = _CashRegister_Treasury,
            MenuID = _CashRegister_Treasury.MenuID,
            SubMenuCode = CodeValue.CashRegister.SUBMENU_CASHRECEPTION,
            SubMenuDescription = "To Receive Money from one Cash Register to another",
            SubMenuLabel = "Cash Reception",
            SubMenuController = "CashReception",
            SubMenuPath = "Index"

        };

        static SubMenu _CashRegister_BankTransmission = new SubMenu()
        {
            Menu = _CashRegister_Treasury,
            MenuID = _CashRegister_Treasury.MenuID,
            SubMenuCode = CodeValue.CashRegister.SUBMENU_BANKTRANSMISSION,
            SubMenuDescription = "To Send Money from a Cash Register to Bank",
            SubMenuLabel = "Bank Transmission",
            SubMenuController = "BankTransmission",
            SubMenuPath = "Index"

        };

        static SubMenu _CashRegister_BankReception = new SubMenu()
        {
            Menu = _CashRegister_Treasury,
            MenuID = _CashRegister_Treasury.MenuID,
            SubMenuCode = CodeValue.CashRegister.SUBMENU_BANKRECEPTION,
            SubMenuDescription = "To Receive Money from one Bank to put it inside a Cash Register",
            SubMenuLabel = "Bank Reception",
            SubMenuController = "BankReception",
            SubMenuPath = "Index"

        };

        static SubMenu _CashRegister_ValidateSpecialOrder = new SubMenu()
        {
            Menu = _CashRegister_CashierSpecialOrder,
            MenuID = _CashRegister_CashierSpecialOrder.MenuID,
            SubMenuCode = CodeValue.CashRegister.ValidateSpecialOrder,
            SubMenuDescription = "To validate a Special Order and take an advance if possible",
            SubMenuLabel = "Validate",
            SubMenuController = "ValidateSpecialOrder",
            SubMenuPath = "ValidateSpecialOrder"

        };

        static SubMenu _CashRegister_DeliverSpecialOrder = new SubMenu()
        {
            Menu = _CashRegister_CashierSpecialOrder,
            MenuID = _CashRegister_CashierSpecialOrder.MenuID,
            SubMenuCode = CodeValue.CashRegister.DeliverSpecialOrder,
            SubMenuDescription = "To Give Special Order to customer and take money if necessary",
            SubMenuLabel = "DeliverS pecial Order",
            SubMenuController = "DeliverSpecialOrder",
            SubMenuPath = "DeliverSpecialOrder"

        };

    }
}
