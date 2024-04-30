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

        /*====================== Modules parameters ==========================================*/
        public static Module _Security = new Module()
        {
            ModuleCode = "MODULE_5",
            ModuleLabel = "Security",
            ModuleDescription = "Bien fait",
            ModuleArea = "Administration",
            ModuleState = true,
            ModuleImageHeight = 28,
            ModuleImageWeight = 60,
            ModulePressedImagePath = "MOD_PRESSED_IMG_SECURITY",
            ModuleImagePath = "MOD_IMG_SECURITY",
            ModuleDisabledImagePath = "MOD_DISABLED_IMG_SECURITY"

        };
        /*Ce module est commun à tous les autres modules. On l'utilisera afin de définir les paramètres (menu de paramétrage) de chaque module*/
        private static Module _Parameters = new Module()
        {
            ModuleCode = "MODULE_6",
            ModuleLabel = "Paramètres",
            ModuleDescription = "Bien fait",
            ModuleArea = "Administration",
            ModuleState = true,
            ModuleImageHeight = 28,
            ModuleImageWeight = 60,
            ModulePressedImagePath = "MOD_PRESSED_IMG_PARAMETERS",
            ModuleImagePath = "MOD_IMG_PARAMETERS",
            ModuleDisabledImagePath = "MOD_DISABLED_IMG_PARAMETERS"
        };
        public static List<Module> Modules
        {
            get
            {
                return new List<Module>() { _Security, _Sale, _Parameters, supply, _Accounting, _CashRegister };
            }
        }
        /*=====================================================================================*/
        /*=========================== Menus initialization ====================================*/
        //**** Security Module
        private static Menu _Security_Profile = new Menu()
        {
            MenuLabel = "Profile",
            MenuCode = CodeValue.Security.Profile.CODE,
            MenuDescription = "Définir les profiles d'accès à l'application",
            MenuFlat = true,
            MenuIconName = "profile.png",
            MenuController = "Profile",
            MenuPath = "Profile",
            MenuState = true,
            Module = _Security,
            ModuleID = _Security.ModuleID
        };
        private static Menu _Security_User = new Menu()
        {
            MenuLabel = "Utilisateur",
            MenuCode = CodeValue.Security.User.CODE,
            MenuDescription = "Définir les utilisateurs de l'application",
            MenuFlat = true,
            MenuIconName = "users.png",
            MenuController = "User",
            MenuPath = "Utilisateur",
            MenuState = true,
            Module = _Security,
            ModuleID = _Security.ModuleID
        };


       

        private static Menu _SuspendedUser = new Menu()
        {
            IsChortcut = false,
            MenuCode = "SuspendedUser",
            MenuController = "SuspendedUser",
            MenuDescription = "Suspended User",
            MenuFlat = false,
            MenuIconName = "SuspendedUser",
            MenuLabel = "SuspendedUser",
            MenuPath = "Index",
            MenuState = true,
            ModuleID = 1
        };

        //********* Module Paramètres
        private static Menu localization = new Menu()
        {
            MenuLabel = "Localité",
            MenuCode = CodeValue.Parameter.Localization.MenuCODE,
            MenuDescription = "Définir les lieux de localisation",
            MenuFlat = true,
            MenuIconName = "localization.png",
            MenuController = "Parameter",
            MenuPath = "Parameter",
            MenuState = true,
            Module = _Parameters,
            ModuleID = _Parameters.ModuleID
        };
        private static Menu Parameter_Company = new Menu()
        {
            MenuLabel = "Company",
            MenuCode = CodeValue.Parameter.CompanyJobCode,
            MenuDescription = "Définir les agences de l'entrprise",
            MenuFlat = true,
            MenuIconName = "company.png",
            MenuController = "Parameter",
            MenuPath = "Company",
            MenuState = true,
            Module = _Parameters,
            ModuleID = _Parameters.ModuleID
        };

        private static Menu moneyManagerParameter = new Menu()
        {
            MenuLabel = "Money Management",
            MenuCode = CodeValue.Parameter.MoneyManagement.MENUCODE,
            MenuDescription = "Définir les agences de l'entrprise",
            MenuFlat = true,
            MenuIconName = "money.png",
            MenuController = "Parameter",
            MenuPath = "NullMoney",
            MenuState = true,
            Module = _Parameters,
            ModuleID = _Parameters.ModuleID
        };

        //menu Bussiness day du module supply
        private static Menu _Parameters_BusinessDay = new Menu()
        {
            MenuLabel = CodeValue.Supply.BusinessDayMenu.LABEL,
            MenuCode = CodeValue.Supply.BusinessDayMenu.CODE,
            MenuDescription = CodeValue.Supply.BusinessDayMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.BusinessDayMenu.FLAT,
            MenuIconName = CodeValue.Supply.BusinessDayMenu.ICON_NAME,
            MenuController = CodeValue.Supply.BusinessDayMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.BusinessDayMenu.PATH,
            MenuState = CodeValue.Supply.BusinessDayMenu.STATE,
            Module = _Parameters,
            ModuleID = _Parameters.ModuleID
        };
        //menu Budget
        private static Menu _Parameters_Budget = new Menu()
        {
            MenuLabel = CodeValue.Parameter.Budget.LABEL,
            MenuCode = CodeValue.Parameter.Budget.CODE,
            MenuDescription = CodeValue.Parameter.Budget.DESCRIPTION,
            MenuFlat = CodeValue.Parameter.Budget.FLAT,
            MenuIconName = CodeValue.Parameter.Budget.ICON_NAME,
            MenuController = CodeValue.Parameter.Budget.CONTROLLER,
            MenuPath = CodeValue.Parameter.Budget.PATH,
            MenuState = CodeValue.Parameter.Budget.STATE,
            Module = _Parameters,
            ModuleID = _Parameters.ModuleID
        };

        public static List<Menu> Menus
        {
            get
            {
                return new List<Menu>()
                    {
                        _Security_Profile, _Security_User, _SuspendedUser, _Sale_Customer, _Sale_Sale, localization, Parameter_Company, _Accounting_AccountingSection, _Accounting_CollectifAccount,_Accounting_Account, _Accounting_AccountingTask, _Accounting_OperationType, _Accounting_AccountOperation,
                        _Supply_Category, _Supply_Product, _Parameters_BusinessDay,_Supply_Stock,_Supply_ProductTransfert,
                        _CashRegister_Open,_CashRegister_Close,_CashRegister_Validate_Sale,_CashRegister_Validate_Purchase,_CashRegister_Deposit,_CashRegister_State,
                        moneyManagerParameter, _Supply_Supply, _Supply_Supplier,_Supply_SupplierReturn, _Supply_SupplierOrder,_Accounting_State, _Supply_LensCriteria,
                        _Parameters_Budget,_CashRegister_Budget,_CashRegister_Treasury,_Sale_Report,_AuhoriseBudgetConsumtion,_TillAjustment,
                        _Sale_SpecialOrder, _CashRegister_CashierSpecialOrder,
                        _Sale_Pricing
                    };

            }
        }
        /*=====================================================================================*/
        /*=========================== SubMenus initialization =================================*/
        //****** Utilisateur subMenus
        static SubMenu _Security_Profile_Profile = new SubMenu()
        {
            Menu = _Security_Profile,
            MenuID = _Security_Profile.MenuID,
            SubMenuCode = CodeValue.Security.Profile.CODE,
            SubMenuDescription = "sous menu",
            SubMenuLabel = "Profile",
            SubMenuController = "Profile",
            SubMenuPath = "Profile"

        };
        static SubMenu _Security_Profile_Advanced = new SubMenu()
        {
            Menu = _Security_Profile,
            MenuID = _Security_Profile.MenuID,
            SubMenuCode = CodeValue.Security.User.ProAvancedCODE,
            SubMenuDescription = "sous menu",
            SubMenuLabel = "Profile Avancé",
            SubMenuController = "Profile",
            SubMenuPath = "AvancedProfile"

        };

        //****** Localization subMenus
        //==== Sub menu country
        static SubMenu country = new SubMenu()
        {
            Menu = localization,
            MenuID = localization.MenuID,
            SubMenuCode = CodeValue.Parameter.COUNTRYCODE,
            SubMenuDescription = "sous menu",
            SubMenuLabel = "Pays",
            SubMenuController = "Parameter",
            SubMenuPath = "Country"

        };
        //Sub menu region
        static SubMenu region = new SubMenu()
        {
            Menu = localization,
            MenuID = localization.MenuID,
            SubMenuCode = CodeValue.Parameter.REGIONCode,
            SubMenuDescription = "sous menu",
            SubMenuLabel = "Région",
            SubMenuController = "Parameter",
            SubMenuPath = "Region"

        };
        //Sub menu town
        static SubMenu town = new SubMenu()
        {
            Menu = localization,
            MenuID = localization.MenuID,
            SubMenuCode = CodeValue.Parameter.TOWNCode,
            SubMenuDescription = "sous menu",
            SubMenuLabel = "Ville",
            SubMenuController = "Parameter",
            SubMenuPath = "Town"

        };
        //Sub menu quarter
        static SubMenu quarter = new SubMenu()
        {
            Menu = localization,
            MenuID = localization.MenuID,
            SubMenuCode = CodeValue.Parameter.QTER,
            SubMenuDescription = "sous menu",
            SubMenuLabel = "Quartier",
            SubMenuController = "Parameter",
            SubMenuPath = "Quarter"

        };
        //****** moneyManagerParameter_ subMenus
        //==== Sub menu country
        static SubMenu moneyManagerParameter_bank = new SubMenu()
        {
            Menu = moneyManagerParameter,
            MenuID = moneyManagerParameter.MenuID,
            SubMenuCode = CodeValue.Parameter.MoneyManagement.BankCODE,
            SubMenuDescription = "sous menu",
            SubMenuLabel = "Bank",
            SubMenuController = "Parameter",
            SubMenuPath = "Bank"

        };
        //Sub menu moneyManagerParameter_Till
        static SubMenu moneyManagerParameter_Till = new SubMenu()
        {
            Menu = moneyManagerParameter,
            MenuID = moneyManagerParameter.MenuID,
            SubMenuCode = CodeValue.Parameter.MoneyManagement.TillCODE,
            SubMenuDescription = "sous menu",
            SubMenuLabel = "Till",
            SubMenuController = "Parameter",
            SubMenuPath = "Till"

        };
        //Sub menu moneyManagerParameter_OtherPaymentMethod
        //static SubMenu moneyManagerParameter_OtherPaymentMethod = new SubMenu()
        //{
        //    Menu = moneyManagerParameter,
        //    MenuID = moneyManagerParameter.MenuID,
        //    SubMenuCode = CodeValue.Parameter.MoneyManagement.OTCODE,
        //    SubMenuDescription = "sous menu",
        //    SubMenuLabel = "OtherPaymentMethod",
        //    SubMenuController = "Parameter",
        //    SubMenuPath = "OtherPaymentMethod"

        //};

        //sous menu close business day du menu BusinessDay du module parameter
        static SubMenu _Parameters_BusinessDay_OpenBD = new SubMenu()
        {
            Menu = _Parameters_BusinessDay,
            MenuID = _Parameters_BusinessDay.MenuID,
            SubMenuCode = CodeValue.Supply.OpenBD_SM.CODE,
            SubMenuLabel = CodeValue.Supply.OpenBD_SM.LABEL,
            IsChortcut = true,
            SubMenuDescription = CodeValue.Supply.OpenBD_SM.DESCRIPTION,
            SubMenuController = CodeValue.Supply.OpenBD_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.OpenBD_SM.PATH
        };

        //sous menu open business day du menu BusinessDay du module parameter
        static SubMenu _Parameters_BusinessDay_CloseBD = new SubMenu()
        {
            Menu = _Parameters_BusinessDay,
            MenuID = _Parameters_BusinessDay.MenuID,
            SubMenuCode = CodeValue.Supply.CloseBD_SM.CODE,
            SubMenuDescription = CodeValue.Supply.CloseBD_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.CloseBD_SM.LABEL,
            SubMenuController = CodeValue.Supply.CloseBD_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.CloseBD_SM.PATH
        };

        //sous menu Closing Day Tasks du menu BusinessDay du module parameter
        static SubMenu _Parameters_BusinessDay_ClosingDayTask = new SubMenu()
        {
            Menu = _Parameters_BusinessDay,
            MenuID = _Parameters_BusinessDay.MenuID,
            SubMenuCode = CodeValue.Supply.ClosingDayTask_SM.CODE,
            SubMenuDescription = CodeValue.Supply.ClosingDayTask_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.ClosingDayTask_SM.LABEL,
            SubMenuController = CodeValue.Supply.ClosingDayTask_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.ClosingDayTask_SM.PATH
        };

        /*=========================== Sub Menu AccountOperation  initialization =================================*/
        static SubMenu _AccountOperation_SingleEntry = new SubMenu()
        {
            Menu = _Accounting_AccountOperation,
            MenuID = _Accounting_AccountOperation.MenuID,
            SubMenuCode = CodeValue.Accounting.AccountOperation_singleEntry.CODE,
            SubMenuDescription = "Single Entry",
            SubMenuLabel = "Single Entry",
            SubMenuController = "SingleEntry",
            SubMenuPath = "Index"

        };
        static SubMenu _AccountOperation_MultiEntries = new SubMenu()
        {
            Menu = _Accounting_AccountOperation,
            MenuID = _Accounting_AccountOperation.MenuID,
            SubMenuCode = CodeValue.Accounting.AccountOperation_multiEntries.CODE,
            SubMenuDescription = "Multiple Entries",
            SubMenuLabel = "Multiple Entries",
            SubMenuController = "MultipleEntries",
            SubMenuPath = "Index"

        };
        static SubMenu _Accounting_State_AcctEntry = new SubMenu()
        {
            Menu = _Accounting_State,
            MenuID = _Accounting_State.MenuID,
            SubMenuCode = CodeValue.Accounting.Report.CODESUBMENU1,
            SubMenuDescription = "Accouting Entries",
            SubMenuLabel = "Accouting Entries",
            SubMenuController = "rptAccoutingEntries",
            SubMenuPath = "Index"
        };
        static SubMenu _Accounting_State_AcctHistory = new SubMenu()
        {
            Menu = _Accounting_State,
            MenuID = _Accounting_State.MenuID,
            SubMenuCode = CodeValue.Accounting.Report.CODESUBMENU2,
            SubMenuDescription = "Accouting History",
            SubMenuLabel = "Accouting History",
            SubMenuController = "RptPrintStmt",
            SubMenuPath = "Index"
        };
        static SubMenu _Accounting_State_TrialBalance = new SubMenu()
        {
            Menu = _Accounting_State,
            MenuID = _Accounting_State.MenuID,
            SubMenuCode = CodeValue.Accounting.Report.CODESUBMENU3,
            SubMenuDescription = "Trial Balance",
            SubMenuLabel = "Trial Balance",
            SubMenuController = "RptBalanceGenerale",
            SubMenuPath = "Index"
        };
        static SubMenu _Accounting_State_IncomeExpense = new SubMenu()
        {
            Menu = _Accounting_State,
            MenuID = _Accounting_State.MenuID,
            SubMenuCode = CodeValue.Accounting.Report.CODESUBMENU4,
            SubMenuDescription = "Statement of Income and Expense",
            SubMenuLabel = "Statement of Income and Expense",
            SubMenuController = "RptIncomeExpense",
            SubMenuPath = "Index"
        };
        static SubMenu _Parameter_Company_AboutCompany = new SubMenu()
        {
            Menu = Parameter_Company,
            MenuID = Parameter_Company.MenuID,
            SubMenuCode = CodeValue.Parameter.CompanyCode,
            SubMenuDescription = "Company",
            SubMenuLabel = "Company",
            SubMenuController = "Parameter",
            SubMenuPath = "Company"
        };
        static SubMenu _Parameter_Company_Job = new SubMenu()
        {
            Menu = Parameter_Company,
            MenuID = Parameter_Company.MenuID,
            SubMenuCode = CodeValue.Parameter.JobCode,
            SubMenuDescription = "",
            SubMenuLabel = "",
            SubMenuController = "Parameter",
            SubMenuPath = "Job"
        };
        private static SubMenu _Parameter_Company_Branch = new SubMenu()
        {
            SubMenuLabel = "Agence",
            SubMenuCode = CodeValue.Parameter.Branch.MenuCODE,
            SubMenuDescription = "Définir les agences de l'entrprise",
            SubMenuController = "Parameter",
            SubMenuPath = "Branch",
            Menu = Parameter_Company,
            MenuID = Parameter_Company.MenuID
        };
        //sub menu de budget
        private static SubMenu _Parameter_Budget_FiscalYear = new SubMenu()
        {
            SubMenuLabel = "Fiscal Year",
            SubMenuCode = CodeValue.Parameter.Budget.SubMenuCODEFY,
            SubMenuDescription = "Define Fiscal exercice",
            SubMenuController = "FiscalYear",
            SubMenuPath = "Index",
            Menu = _Parameters_Budget,
            MenuID = _Parameters_Budget.MenuID
        };
        private static SubMenu _Parameter_Budget_BudgetLine = new SubMenu()
        {
            SubMenuLabel = "Budget Line",
            SubMenuCode = CodeValue.Parameter.Budget.SubMenuCODEBL,
            SubMenuDescription = "Define Budget Line",
            SubMenuController = "BudgetLine",
            SubMenuPath = "Index",
            Menu = _Parameters_Budget,
            MenuID = _Parameters_Budget.MenuID
        };
        private static SubMenu _Parameter_Budget_BudgetAllocated = new SubMenu()
        {
            SubMenuLabel = "Budget Allocated",
            SubMenuCode = CodeValue.Parameter.Budget.SubMenuCODEBA,
            SubMenuDescription = "Define Budget Allocated",
            SubMenuController = "BudgetAllocated",
            SubMenuPath = "Index",
            Menu = _Parameters_Budget,
            MenuID = _Parameters_Budget.MenuID
        };
        private static SubMenu _Parameter_Budget_BudgetAllocatedUpdate = new SubMenu()
        {
            SubMenuLabel = "Budget Allocated Update",
            SubMenuCode = CodeValue.Parameter.Budget.SubMenuCODEBAU,
            SubMenuDescription = "Modify Budget Allocated",
            SubMenuController = "BudgetAllocatedUpdate",
            SubMenuPath = "Index",
            Menu = _Parameters_Budget,
            MenuID = _Parameters_Budget.MenuID
        };
        private static SubMenu _CashRegister_State_SaleReceipt = new SubMenu()
        {
            SubMenuLabel = "Sale Receipt",
            SubMenuCode = CodeValue.Parameter.Budget.Sale.SaleReceipt,
            SubMenuDescription = "Print Sale Receipt",
            SubMenuController = CodeValue.Parameter.Budget.Sale.SaleReceipt,
            SubMenuPath = CodeValue.Parameter.Budget.Sale.SaleReceipt,
            Menu = _CashRegister_State,
            MenuID = _CashRegister_State.MenuID
        };

        public static List<SubMenu> SubMenus
        {
            get
            {
                return new List<SubMenu>() { _Security_Profile_Profile, _Security_Profile_Advanced, _Sale_Sale_NewSale, _Sale_Sale_SaleReturn, country, region, town, quarter,
                                             _Supply_Product_Lens, _Supply_Product_GenericProduct, _Parameters_BusinessDay_CloseBD, _Parameters_BusinessDay_OpenBD,
                                             _Parameters_BusinessDay_ClosingDayTask, _Supply_Stock_Location, _Supply_Stock_InventoryDirectory, _Supply_Stock_InventoryEntry, _Supply_Stock_Inventory,_Parameter_Company_AboutCompany,_Parameter_Company_Job,_Parameter_Company_Branch,
                                             _Supply_Transfert_ProductTransmission, _Supply_Transfert_ProductReception_SM,_Supply_Transfert_ProductReception_BR, _Sale_Sale_Command,moneyManagerParameter_bank,moneyManagerParameter_Till,//moneyManagerParameter_OtherPaymentMethod,
                                             _AccountOperation_SingleEntry,_AccountOperation_MultiEntries,_Accounting_State_AcctEntry,_CashRegister_State_Bill,_CashRegister_State_Hisotric,
                                             _Accounting_State_AcctHistory,_Accounting_State_TrialBalance,_Accounting_State_IncomeExpense,_Parameter_Budget_FiscalYear,_Parameter_Budget_BudgetLine,
                                             _Parameter_Budget_BudgetAllocated,_Parameter_Budget_BudgetAllocatedUpdate,_Sale_Report_ListClt,_Sale_Report_HistoClt,
                                             _Supply_Category_GenericCategory, _Supply_Category_LensCategory, _Sale_Pricing_Range, _Sale_Pricing_LensPrice,
                                             _CashRegister_CashTransmission, _CashRegister_CashReception, _CashRegister_BankTransmission, _CashRegister_BankReception,
                                             _Sale_Sale_OrderLensOrder, _Sale_Sale_PostedSpecialOrderToSupplier, _Sale_Sale_ReceiveSpecialOrder, _CashRegister_ValidateSpecialOrder, _CashRegister_DeliverSpecialOrder,
                                             _CashRegister_State_SaleReceipt,_Sale_Report_ReturnSale
                };
            }
        }

        /*=====================================================================================*/
        /*============================== Adress initialization ================================*/
        //country
        private static Country cameroon = new Country() { CountryCode = "CMR", CountryLabel = "Cameroon" };
        //regions
        private static Region west = new Region()
        {
            RegionCode = "W",
            RegionLabel = "West",
            Country = cameroon,
            CountryID = cameroon.CountryID

        };
        private static Region north = new Region()
        {
            RegionCode = "N",
            RegionLabel = "North",
            Country = cameroon,
            CountryID = cameroon.CountryID
        };
        private static Region center = new Region()
        {
            RegionCode = "CE",
            RegionLabel = "Center",
            Country = cameroon,
            CountryID = cameroon.CountryID
        };
        private static Region east = new Region()
        {
            RegionCode = "E",
            RegionLabel = "East",
            Country = cameroon,
            CountryID = cameroon.CountryID
        };
        private static Region littoral = new Region()
        {
            RegionCode = "LT",
            RegionLabel = "Litoral",
            Country = cameroon,
            CountryID = cameroon.CountryID
        };
        private static Region south = new Region()
        {
            RegionCode = "STH",
            RegionLabel = "South",
            Country = cameroon,
            CountryID = cameroon.CountryID
        };
        public static List<Region> Regions
        {
            get
            {
                return new List<Region>() { west, north, center, east, south, littoral };
            }
        }
        /*======================================================================================*/

        /*============================== Towns initialization ==================================*/
        private static Town bafoussam = new Town() { Region = west, RegionID = west.RegionID, TownCode = "BFS", TownLabel = "Bafoussam" };
        private static Town yaounde = new Town() { Region = center, RegionID = center.RegionID, TownCode = "YDE", TownLabel = "Yaounde" };
        private static Town douala = new Town() { Region = littoral, RegionID = littoral.RegionID, TownCode = "DLA", TownLabel = "Douala" };
        private static Town bertoua = new Town() { Region = east, RegionID = east.RegionID, TownCode = "BTA", TownLabel = "Bertoua" };
        private static Town ebolowa = new Town() { Region = south, RegionID = south.RegionID, TownCode = "EBW", TownLabel = "Ebolowa" };
        private static Town maroua = new Town() { Region = north, RegionID = north.RegionID, TownCode = "MRA", TownLabel = "Maroua" };
        public static List<Town> Towns
        {
            get
            {
                return new List<Town>()
                {
                     bafoussam,yaounde,douala,bertoua,ebolowa,maroua
                };
            }
        }
        /*=======================================================================================*/

        /*============================== Quarters initialization ===============================*/
        private static Quarter marche_A_bfs = new Quarter() { QuarterCode = "A", QuarterLabel = "Marché A", Town = bafoussam, TownID = bafoussam.TownID };
        private static Quarter dlaAkwa = new Quarter() { QuarterCode = "AKW", QuarterLabel = "Carrefour Fokou Akwa", Town = douala, TownID = douala.TownID };
        private static Quarter mvolye_yde = new Quarter() { QuarterCode = "MVL", QuarterLabel = "Mvolyé", Town = yaounde, TownID = yaounde.TownID };
        private static Quarter qrt1_ebw = new Quarter() { QuarterCode = "QT1", QuarterLabel = "Quartier 1", Town = ebolowa, TownID = ebolowa.TownID };
        private static Quarter kolbisson_bta = new Quarter() { QuarterCode = "KOLB", QuarterLabel = "Kolbisson", Town = bertoua, TownID = bertoua.TownID };
        private static Quarter mil_mra = new Quarter() { QuarterCode = "MILL", QuarterLabel = "Quartier Du Mil", Town = maroua, TownID = maroua.TownID };
        private static Quarter ydeAvenueKennedy = new Quarter() { QuarterCode = "Avenue Kennedy", QuarterLabel = "Avenue Kennedy", Town = yaounde, TownID = yaounde.TownID };
        public static List<Quarter> Quarters
        {
            get
            {
                return new List<Quarter>()
                {
                    marche_A_bfs,dlaAkwa,mvolye_yde,qrt1_ebw,kolbisson_bta,mil_mra, ydeAvenueKennedy
                };
            }
        }
        /*========================================================================================*/
        /*=========================== Company,Job and Branch initialization =================================*/
        private static Adress adressCompany = new Adress()
        {
            AdressEmail = "contact@dboydistribution.com",
            AdressPhoneNumber = "+237 337 095 38",
            AdressPOBox = "",
            AdressFax = "",
            AdressCellNumber = "",
            AdressFullName = "",
            Quarter = dlaAkwa,
            QuarterID = dlaAkwa.QuarterID
        };
        private static Adress adressAdmin = new Adress()
        {
            AdressEmail = "admin@dboydistribution.com",
            AdressPhoneNumber = "674 86 99 44",
            AdressPOBox = "",
            AdressFax = "",
            AdressCellNumber = "",
            AdressFullName = "",
            Quarter = mvolye_yde,
            QuarterID = mvolye_yde.QuarterID
        };
        private static Adress adressSuperAdmin = new Adress()
        {
            AdressEmail = "super-admin@dboydistribution.com",
            AdressPhoneNumber = "77777777",
            AdressPOBox = "",
            AdressFax = "",
            AdressCellNumber = "",
            AdressFullName = "",
            Quarter = mvolye_yde,
            QuarterID = mvolye_yde.QuarterID
        };
        private static Adress dlaHeadBranchAdress = new Adress()
        {
            AdressEmail = "dboydistribution@yahoo.com",
            AdressPhoneNumber = "243 709 538",
            AdressPOBox = "",
            AdressFax = "",
            AdressCellNumber = "652 520 627",
            AdressFullName = "Situé à Douala à-côté de FOKOU AKWA",
            Quarter = dlaAkwa,
            QuarterID = dlaAkwa.QuarterID
        };
        private static Adress ydeHeadBranchAdress = new Adress()
        {
            AdressEmail = "dboydistribution@yahoo.com",
            AdressPhoneNumber = "242 101 572",
            AdressPOBox = "11259",
            AdressFax = "",
            AdressCellNumber = "650 002 142",
            AdressFullName = "Situé à Yaounde à l'immeuble Les Galeries Avenus Kennedy 2ème etage",
            Quarter = ydeAvenueKennedy,
            QuarterID = ydeAvenueKennedy.QuarterID
        };

        private static Adress adressSecBranch = new Adress()
        {
            AdressEmail = "secondary-branch@dboydistribution.com",
            AdressPhoneNumber = "+288 4586 45",
            AdressPOBox = "20",
            AdressFax = "7458",
            AdressCellNumber = "6",
            AdressFullName = "",
            Quarter = marche_A_bfs,
            QuarterID = marche_A_bfs.QuarterID
        };
        private static Company defaultCompany = new Company()
        {
            CNI = "PO18211344463L",
            CompanyCapital = 100000000,
            CompanyIsDeletable = false,
            CompanySigle = "D-BOY",
            CompanySlogan = "Perfect Lens",
            CompanyTradeRegister = "RC/DLA/2014/A/1891 DU 25/04/2014",
            Description = "Lens distribution ...",
            Name = "D-BOY DISTRIBUTION",
            Adress = adressCompany,
            AdressID = adressCompany.AdressID
        };
        private static Branch YaoundeHeadBranch = new Branch()
        {
            BranchName = CodeValue.Parameter.Branch.YdeBranchName,
            Company = defaultCompany,
            CompanyID = defaultCompany.GlobalPersonID,
            BranchDescription = "Head Branch",
            BranchCode = "DBOY YDE 01",
            Adress = ydeHeadBranchAdress,
            AdressID = ydeHeadBranchAdress.AdressID,

        };
        private static Branch DoualaHeadBranch = new Branch()
        {
            BranchName = CodeValue.Parameter.Branch.DlaBranchName,
            Company = defaultCompany,
            CompanyID = defaultCompany.GlobalPersonID,
            BranchDescription = "Head Branch",
            BranchCode = "DBOY DLA 01",
            Adress = dlaHeadBranchAdress,
            AdressID = dlaHeadBranchAdress.AdressID
        };
        public static List<Branch> Branchs
        {
            get
            {
                return new List<Branch>() { YaoundeHeadBranch, DoualaHeadBranch };
            }
        }

        /*========================= BusinessDay initialization===================================*/

        public static BusinessDay headBranchBusDay = new BusinessDay
        {
            BDCode = "BusDay_" + YaoundeHeadBranch.BranchCode,
            BDLabel = "BusDay_" + YaoundeHeadBranch.BranchCode,
            BDDescription = "this is for " + YaoundeHeadBranch.BranchCode + " branch",
            BDStatut = false,//closed
            ClosingDayStarted = false,
            BDDateOperation = DateTime.Now,//aaa-mmm-jjj
            BranchID = YaoundeHeadBranch.BranchID,
            Branch = YaoundeHeadBranch
        };

        //public static BusinessDay scondaryBranchBusDay = new BusinessDay
        //{
        //    BDCode = "BusDay_" + scondaryBranch.BranchCode,
        //    BDLabel = "BusDay_" + scondaryBranch.BranchCode,
        //    BDDescription = "this is for " + scondaryBranch.BranchCode + " branch",
        //    BDStatut = false,//closed
        //    ClosingDayStarted = false,
        //    BDDateOperation = DateTime.Now,
        //    BranchID = scondaryBranch.BranchID,
        //    Branch = scondaryBranch
        //};

        public static List<BusinessDay> BusinessDays
        {
            get
            {
                return new List<BusinessDay>() { headBranchBusDay/*, scondaryBranchBusDay*/ };
            }
        }

        /*========================= Closing Day Task initialization===================================*/

        public static ClosingDayTask cdtBeginBackup = new ClosingDayTask
        {
            ClosingDayTaskCode = "Beging Backup",
            ClosingDayTaskLabel = "Backup",
            ClosingDayTaskDescription = "Avec cette tâche, nous faisons une sauvegarde de l'état de la bd avant de commencer les tâches de fermeture ",
        };

        public static ClosingDayTask cdtTillClosing = new ClosingDayTask
        {
            ClosingDayTaskCode = "TillClosing",
            ClosingDayTaskLabel = "Till Closing",
            ClosingDayTaskDescription = "Here, we check if the till was normaly closed",
        };

        public static ClosingDayTask cdtAccountingEntryChecking = new ClosingDayTask
        {
            ClosingDayTaskCode = "AccountingEntryChecking",
            ClosingDayTaskLabel = "Accounting Entry Checking",
            ClosingDayTaskDescription = "ici, nous nous assurons que la partie double générale est respectée",
        };

        public static ClosingDayTask cdtEndBackup = new ClosingDayTask
        {
            ClosingDayTaskCode = "End Backup",
            ClosingDayTaskLabel = "Backup",
            ClosingDayTaskDescription = "Avec cette tâche, nous faisons une sauvegarde de l'état de la bd après l'éxécution des tâches de fermeture",
        };

        public static List<ClosingDayTask> ClosingDayTasks
        {
            get
            {
                return new List<ClosingDayTask>() { cdtBeginBackup, cdtTillClosing, cdtAccountingEntryChecking, cdtEndBackup };
            }
        }

        /*========================= Closing Day Task initialization===================================*/

        public static List<BranchClosingDayTask> BranchClosingDayTasks
        {
            get
            {
                List<BranchClosingDayTask> allbcdt = new List<BranchClosingDayTask>();
                foreach (Branch branch in Branchs)
                {
                    foreach (ClosingDayTask cdt in ClosingDayTasks)
                    {
                        BranchClosingDayTask bcdt = new BranchClosingDayTask
                        {
                            Branch = branch,
                            BranchID = branch.BranchID,
                            ClosingDayTask = cdt,
                            ClosingDayTaskID = cdt.ClosingDayTaskID,
                            Statut = true
                        };
                        allbcdt.Add(bcdt);
                    }
                }
                return allbcdt;
            }
        }


        /*=====================================================================================*/
        /*=========================== Profiles initialization =================================*/
        private static Profile administrator = new Profile()
        {
            ProfileCode = "admin-Dental",
            ProfileLabel = "Administrateur",
            ProfileDescription = "Détient le contrôle sur toute l'application",
            ProfileState = true
        };

        private static Profile superAdministrator = new Profile()
        {
            ProfileCode = "Super-Admin-FSInventory",
            ProfileLabel = "Super Administrator",
            ProfileDescription = "Détient le contrôle sur toute l'application",
            ProfileState = true,
        };

        private static Profile employee = new Profile()
        {
            ProfileCode = CodeValue.Security.Profile.ClASS_CODE,
            ProfileLabel = "Employé",
            ProfileDescription = "Un employé de la compagnie qui ne peut se connecter à l'application",
            ProfileState = false
        };
        public static List<Profile> Profiles
        {
            get
            {
                return new List<Profile>() { superAdministrator, administrator, employee };
            }
        }
        /*======================== company and Jobs initialization ============================*/

        private static Job computeristJob = new Job()
        {
            JobLabel = "Computer Engineer",
            JobDescription = "Manage Information system",
            JobCode = "Info",
            Company = defaultCompany,
            CompanyID = defaultCompany.GlobalPersonID
        };
        private static Job tellerJob = new Job()
        {
            JobLabel = "Teller",
            JobDescription = "Manage Teller",
            JobCode = "Teller",
            Company = defaultCompany,
            CompanyID = defaultCompany.GlobalPersonID
        };
        private static Job accountantJob = new Job()
        {
            JobLabel = "Accountant",
            JobDescription = "Manage Account",
            JobCode = "Accountant",
            Company = defaultCompany,
            CompanyID = defaultCompany.GlobalPersonID
        };
        private static Job warehouseJob = new Job()
        {
            JobLabel = "Warehouse",
            JobDescription = "Manage Warehouse",
            JobCode = "Warehouse",
            Company = defaultCompany,
            CompanyID = defaultCompany.GlobalPersonID
        };
        public static List<Job> Jobs
        {
            get
            {
                return new List<Job>() { computeristJob, tellerJob, accountantJob, warehouseJob };
            }
        }
        /*=====================================================================================*/
        /*========================== ProfilesMenus initialization =============================*/
        static ActionMenuProfile adminMenu_User = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Security_User,
            MenuID = _Security_User.MenuID
        };
        static ActionMenuProfile adminMenu_DisconectedUSer = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _SuspendedUser,
            MenuID = _SuspendedUser.MenuID
        };
        static ActionMenuProfile adminMenu_Profile = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Security_Profile,
            MenuID = _Security_Profile.MenuID
        };
        //Moduel Sale attribution menu to admin user
        static ActionMenuProfile adminMenu_Sale_Customer = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Sale_Customer,
            MenuID = _Sale_Customer.MenuID
        };
        static ActionMenuProfile adminMenu_Sale_Sale = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Sale_Sale,
            MenuID = _Sale_Sale.MenuID
        };
        static ActionMenuProfile adminMenu_Sale_Report = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Sale_Report,
            MenuID = _Sale_Report.MenuID
        };
        //Module Accouting attribution menu to admin user

        static ActionMenuProfile adminMenu_Accounting_AccountOperation = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Accounting_AccountOperation,
            MenuID = _Accounting_AccountOperation.MenuID
        };
        static ActionMenuProfile adminMenu_Accounting_OperationType = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Accounting_OperationType,
            MenuID = _Accounting_OperationType.MenuID
        };
        static ActionMenuProfile adminMenu_Accounting_AccountingTask = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Accounting_AccountingTask,
            MenuID = _Accounting_AccountingTask.MenuID
        };
        static ActionMenuProfile adminMenu_Accounting_CollectifAccount = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Accounting_CollectifAccount,
            MenuID = _Accounting_CollectifAccount.MenuID
        };
        static ActionMenuProfile adminMenu_Accounting_Account = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Accounting_Account,
            MenuID = _Accounting_Account.MenuID
        };
        static ActionMenuProfile adminMenu_Accounting_AccountingSection = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Accounting_AccountingSection,
            MenuID = _Accounting_AccountingSection.MenuID
        };
        static ActionMenuProfile adminMenu_Authorize_Bud_Consumtion = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _AuhoriseBudgetConsumtion,
            MenuID = _AuhoriseBudgetConsumtion.MenuID
        };
        static ActionMenuProfile adminMenu_TillAdjust = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _TillAjustment,
            MenuID = _TillAjustment.MenuID
        };
        static ActionMenuProfile adminMenu_Accounting_State = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Accounting_State,
            MenuID = _Accounting_State.MenuID
        };
        /// <summary>
        /// Action menu for localization
        /// </summary>
        static ActionMenuProfile adminMenu_Localization = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = localization,
            MenuID = localization.MenuID
        };
        static ActionMenuProfile adminCompany = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = Parameter_Company,
            MenuID = Parameter_Company.MenuID
        };



        //************ Action menu profile of moneyManagerParameter
        static ActionMenuProfile adminMenu_moneyManagerParameter = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = moneyManagerParameter,
            MenuID = moneyManagerParameter.MenuID
        };
        /// <summary>
        /// début de la création des ActionMenuProfile en vue de préparer l'affectation des menus à l'administrateur
        /// </summary>

        //creation de adminMenu_Category pour attribution du menu catégorie à l'admin
        static ActionMenuProfile adminMenu_Category = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Supply_Category,
            MenuID = _Supply_Category.MenuID
        };

        //creation de adminMenu_Category pour attribution du menu produit à l'admin
        static ActionMenuProfile adminMenu_Product = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Supply_Product,
            MenuID = _Supply_Product.MenuID
        };

        //creation de adminMenu_Stock pour attribution du menu Stock à l'admin
        static ActionMenuProfile adminMenu_Stock = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Supply_Stock,
            MenuID = _Supply_Stock.MenuID
        };

        //creation de adminMenu_BusDay pour attribution du menu BusDay à l'admin
        static ActionMenuProfile adminMenu_BusDay = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Parameters_BusinessDay,
            MenuID = _Parameters_BusinessDay.MenuID
        };

        //creation de adminMenu_BusDay pour attribution du menu BusDay à l'admin
        static ActionMenuProfile adminMenu_Supply_Supplier = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Supply_Supplier,
            MenuID = _Supply_Supplier.MenuID
        };

        //creation de adminMenu_BusDay pour attribution du menu BusDay à l'admin
        static ActionMenuProfile adminMenu_Supply_Supply = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Supply_Supply,
            MenuID = _Supply_Supply.MenuID
        };
        //creation de adminMenu_BusDay pour attribution du menu BusDay à l'admin
        static ActionMenuProfile adminMenu_CashRegister_Open = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _CashRegister_Open,
            MenuID = _CashRegister_Open.MenuID
        };

        //creation de adminMenu_SupplierOrder pour attribution du menu _Supply_SupplierOrder à l'admin
        static ActionMenuProfile adminMenu_SupplierOrder = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Supply_SupplierOrder,
            MenuID = _Supply_SupplierOrder.MenuID
        };

        //creation de adminMenu_SupplierReturn pour attribution du menu _Supply_SupplierReturn à l'admin
        static ActionMenuProfile adminMenu_SupplierReturn = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Supply_SupplierReturn,
            MenuID = _Supply_SupplierReturn.MenuID
        };

        //creation de adminMenu_LensCriteria pour attribution du menu _Supply_LensCriteria à l'admin
        static ActionMenuProfile adminMenu_LensCriteria = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Supply_LensCriteria,
            MenuID = _Supply_LensCriteria.MenuID
        };

        static ActionMenuProfile adminMenu_CashRegister_Close = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _CashRegister_Close,
            MenuID = _CashRegister_Close.MenuID
        };
        static ActionMenuProfile adminMenu_CashRegister_Validate_Sale = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _CashRegister_Validate_Sale,
            MenuID = _CashRegister_Validate_Sale.MenuID
        };
        static ActionMenuProfile adminMenu_CashRegister_Validate_Purchase = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _CashRegister_Validate_Purchase,
            MenuID = _CashRegister_Validate_Purchase.MenuID
        };

        //affectation du menu dépôt à l'administrateur
        static ActionMenuProfile adminMenu_CashRegister_Deposit = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _CashRegister_Deposit,
            MenuID = _CashRegister_Deposit.MenuID
        };
        //state of cash register
        static ActionMenuProfile adminMenu_CashRegister_State = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _CashRegister_State,
            MenuID = _CashRegister_State.MenuID
        };

        //attribution du menu budget a l'admin
        static ActionMenuProfile adminMenu_Budget = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Parameters_Budget,
            MenuID = _Parameters_Budget.MenuID
        };
        //attribution du menu budget a l'admin
        static ActionMenuProfile adminMenu_CashRegister_Budget = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _CashRegister_Budget,
            MenuID = _CashRegister_Budget.MenuID
        };
        static ActionMenuProfile adminMenu_CashRegister_Treasury = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _CashRegister_Treasury,
            MenuID = _CashRegister_Treasury.MenuID
        };

        static ActionMenuProfile adminMenu_Supply_ProductTransfert = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Supply_ProductTransfert,
            MenuID = _Supply_ProductTransfert.MenuID
        };

        static ActionMenuProfile adminMenu_CashRegister_SpecialOrder = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Sale_SpecialOrder,
            MenuID = _Sale_SpecialOrder.MenuID
        };

        static ActionMenuProfile adminMenu_Sale_SpecialOrder = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _CashRegister_CashierSpecialOrder,
            MenuID = _CashRegister_CashierSpecialOrder.MenuID
        };

        static ActionMenuProfile adminMenu_Sale_Pricing = new ActionMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            Menu = _Sale_Pricing,
            MenuID = _Sale_Pricing.MenuID
        };

        /// <summary>
        /// fin de la création des ActionMenuProfile en vue de préparer l'affectation des menus à l'administrateur
        /// </summary>


        public static List<ActionMenuProfile> ActionMenuProfiles
        {
            get
            {
                return new List<ActionMenuProfile>() { adminMenu_User,adminMenu_DisconectedUSer, adminMenu_Profile, adminMenu_Sale_Customer, adminMenu_Sale_Sale,adminMenu_Localization, adminCompany, adminMenu_Accounting_CollectifAccount,adminMenu_Accounting_Account, adminMenu_Accounting_AccountingSection, adminMenu_Accounting_AccountingTask, adminMenu_Accounting_AccountOperation, adminMenu_Accounting_OperationType,
                                                       adminMenu_Category, adminMenu_Product, adminMenu_Stock, adminMenu_BusDay,adminMenu_CashRegister_Open, adminMenu_CashRegister_Close,adminMenu_moneyManagerParameter, adminMenu_Supply_Supply, adminMenu_Supply_Supplier,
                                                       adminMenu_SupplierOrder, adminMenu_SupplierReturn,adminMenu_CashRegister_Validate_Sale,adminMenu_CashRegister_Validate_Purchase,adminMenu_CashRegister_Deposit,adminMenu_CashRegister_State,adminMenu_Accounting_State, adminMenu_LensCriteria,
                                                       adminMenu_Budget,adminMenu_CashRegister_Budget,adminMenu_CashRegister_Treasury, adminMenu_Supply_ProductTransfert,adminMenu_Sale_Report,adminMenu_Authorize_Bud_Consumtion,adminMenu_TillAdjust, adminMenu_Sale_SpecialOrder, adminMenu_CashRegister_SpecialOrder, adminMenu_Sale_Pricing
                                                      };

            }
        }

        public static List<ActionMenuProfile> SuperAdminActionMenuProfiles
        {
            get
            {
                List<ActionMenuProfile> saMenus = new List<ActionMenuProfile>();

                foreach (ActionMenuProfile asmp in ActionMenuProfiles)
                {
                    asmp.Profile = superAdministrator;
                    asmp.ProfileID = superAdministrator.ProfileID;
                    saMenus.Add(asmp);
                }

                return saMenus;
            }
        }

        /*=====================================================================================*/
        /*========================== ProfilesSubMenus initialization ==========================*/
        //, , , , , calculate2
        static ActionSubMenuProfile adminSubMenu_User_User = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Security_Profile_Profile,
            SubMenuID = _Security_Profile_Profile.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_ProfileAvanced = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Security_Profile_Advanced,
            SubMenuID = _Security_Profile_Advanced.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Sale_Sale_NewCommand = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Sale_Command,
            SubMenuID = _Sale_Sale_Command.SubMenuID
        };

        static ActionSubMenuProfile adminSubMenu_Sale_OrderLensOrder = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Sale_OrderLensOrder,
            SubMenuID = _Sale_Sale_OrderLensOrder.SubMenuID
        };

        static ActionSubMenuProfile adminSubMenu_Sale_Sale_NewSale = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Sale_NewSale,
            SubMenuID = _Sale_Sale_NewSale.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Sale_Sale_SaleReturn = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Sale_SaleReturn,
            SubMenuID = _Sale_Sale_SaleReturn.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Localization_Country = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = country,
            SubMenuID = country.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Localization_Region = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = region,
            SubMenuID = region.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Localization_Town = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = town,
            SubMenuID = town.SubMenuID
        };
        static ActionSubMenuProfile adminSub_Localization_Quarter = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = quarter,
            SubMenuID = quarter.SubMenuID
        };
        //**************Sub menu Bank, Till and OtherPaymentMethod
        static ActionSubMenuProfile adminSubMenu_moneyManagerParameter_Till = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = moneyManagerParameter_Till,
            SubMenuID = moneyManagerParameter_Till.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_moneyManagerParameter_bank = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = moneyManagerParameter_bank,
            SubMenuID = moneyManagerParameter_bank.SubMenuID
        };
        //static ActionSubMenuProfile adminSub_moneyManagerParameter_OtherPaymentMethod = new ActionSubMenuProfile()
        //{
        //    Add = true,
        //    Delete = true,
        //    Update = true,
        //    Profile = administrator,
        //    ProfileID = administrator.ProfileID,
        //    SubMenu = moneyManagerParameter_OtherPaymentMethod,
        //    SubMenuID = moneyManagerParameter_OtherPaymentMethod.SubMenuID
        //};
        /// <summary>
        /// début de la création des ActionSubMenuProfile en vue de préparer l'affectation des sous menus à l'administrateur
        /// </summary>

        //creation de adminSubMenu_Lens pour attribution du sous menu Lens à l'admin
        static ActionSubMenuProfile adminSubMenu_Lens = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Product_Lens,
            SubMenuID = _Supply_Product_Lens.SubMenuID
        };

        //creation de adminSubMenu_GenericProduct pour attribution du sous menu produit générique à l'admin
        static ActionSubMenuProfile adminSubMenu_GenericProduct = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Product_GenericProduct,
            SubMenuID = _Supply_Product_GenericProduct.SubMenuID
        };

        //creation de adminSubMenu_OpenBD pour attribution du sous menu OpenBD à l'admin
        static ActionSubMenuProfile adminSubMenu_OpenBD = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Parameters_BusinessDay_OpenBD,
            SubMenuID = _Parameters_BusinessDay_OpenBD.SubMenuID
        };

        //creation de adminSubMenu_CloseBD pour attribution du sous menu CloseBD à l'admin
        static ActionSubMenuProfile adminSubMenu_CloseBD = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Parameters_BusinessDay_CloseBD,
            SubMenuID = _Parameters_BusinessDay_CloseBD.SubMenuID
        };

        //creation de adminSubMenu_ClosingDayTask pour attribution du sous menu ClosingDayTask à l'admin
        static ActionSubMenuProfile adminSubMenu_ClosingDayTask = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Parameters_BusinessDay_ClosingDayTask,
            SubMenuID = _Parameters_BusinessDay_ClosingDayTask.SubMenuID
        };

        //creation de adminSubMenu_Location pour attribution du sous menu Location à l'admin
        static ActionSubMenuProfile adminSubMenu_Location = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Stock_Location,
            SubMenuID = _Supply_Stock_Location.SubMenuID
        };

        //creation de adminSubMenu_Inventory pour attribution du sous menu Location à l'admin
        static ActionSubMenuProfile adminSubMenu_Inventory = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Stock_Inventory,
            SubMenuID = _Supply_Stock_Inventory.SubMenuID
        };

        //creation de _Supply_Stock_InventoryDirectory pour attribution du sous menu Location à l'admin
        static ActionSubMenuProfile adminSubMenu_InventoryDirectory = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Stock_InventoryDirectory,
            SubMenuID = _Supply_Stock_InventoryDirectory.SubMenuID
        };


        //creation de adminSubMenu_InventoryEntry pour attribution du sous menu Location à l'admin
        static ActionSubMenuProfile adminSubMenu_InventoryEntry = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Stock_InventoryEntry,
            SubMenuID = _Supply_Stock_InventoryEntry.SubMenuID
        };

        //creation de adminSubMenu_ProductTransmission pour attribution du sous menu ProductTransfert à l'admin
        static ActionSubMenuProfile adminSubMenu_ProductTransmission = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Transfert_ProductTransmission,
            SubMenuID = _Supply_Transfert_ProductTransmission.SubMenuID
        };

        //creation de adminSubMenu_ProductReception pour attribution du sous menu ProductReception à l'admin
        static ActionSubMenuProfile adminSubMenu_ProductReception = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Transfert_ProductReception_SM,
            SubMenuID = _Supply_Transfert_ProductReception_SM.SubMenuID
        };

        //creation de adminSubMenu_ProductReception pour attribution du sous menu ProductReception à l'admin
        static ActionSubMenuProfile adminSubMenu_ProductReception_BR = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Transfert_ProductReception_BR,
            SubMenuID = _Supply_Transfert_ProductReception_BR.SubMenuID
        };
        //creation de adminSubMenu_LensCoating pour attribution du sous menu LensCoating à l'admin
        static ActionSubMenuProfile adminSubMenu_LensCoating = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_LensCriteria_LensCoating,
            SubMenuID = _Supply_LensCriteria_LensCoating.SubMenuID
        };

        //creation de adminSubMenu_LensColour pour attribution du sous menu LensColour à l'admin
        static ActionSubMenuProfile adminSubMenu_LensColour = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_LensCriteria_LensColour,
            SubMenuID = _Supply_LensCriteria_LensColour.SubMenuID
        };

        //creation de adminSubMenu_LensMaterial pour attribution du sous menu LensMaterial à l'admin
        static ActionSubMenuProfile adminSubMenu_LensMaterial = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_LensCriteria_LensMaterial,
            SubMenuID = _Supply_LensCriteria_LensMaterial.SubMenuID
        };

        //creation de adminSubMenu_LensNumber pour attribution du sous menu LensNumber à l'admin
        static ActionSubMenuProfile adminSubMenu_LensNumber = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_LensCriteria_LensNumber,
            SubMenuID = _Supply_LensCriteria_LensNumber.SubMenuID
        };



        //sub menu pr acct entries
        static ActionSubMenuProfile adminSubMenu_AccountOperation_SingleEntry = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _AccountOperation_SingleEntry,
            SubMenuID = _AccountOperation_SingleEntry.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_AccountOperation_MultiEntries = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _AccountOperation_MultiEntries,
            SubMenuID = _AccountOperation_MultiEntries.SubMenuID
        };
        //rpt acct sub menu
        static ActionSubMenuProfile adminSubMenu_Accounting_State_AcctEntry = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Accounting_State_AcctEntry,
            SubMenuID = _Accounting_State_AcctEntry.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Accounting_State_AcctHistory = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Accounting_State_AcctHistory,
            SubMenuID = _Accounting_State_AcctHistory.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Accounting_State_TrialBalance = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Accounting_State_TrialBalance,
            SubMenuID = _Accounting_State_TrialBalance.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Accounting_State_IncomeExpense = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Accounting_State_IncomeExpense,
            SubMenuID = _Accounting_State_IncomeExpense.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Parameter_Company_Company = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Parameter_Company_AboutCompany,
            SubMenuID = _Parameter_Company_AboutCompany.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Parameter_Company_Job = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Parameter_Company_Job,
            SubMenuID = _Parameter_Company_Job.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Parameter_Company_Branch = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Parameter_Company_Branch,
            SubMenuID = _Parameter_Company_Branch.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_CashRegister_State_Hisotric = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _CashRegister_State_Hisotric,
            SubMenuID = _CashRegister_State_Hisotric.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_CashRegister_State_Bill = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _CashRegister_State_Bill,
            SubMenuID = _CashRegister_State_Bill.SubMenuID
        };
        //budget
        static ActionSubMenuProfile adminSubMenu_Parameter_Budget_FiscalYear = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Parameter_Budget_FiscalYear,
            SubMenuID = _Parameter_Budget_FiscalYear.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Parameter_Budget_BudgetLine = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Parameter_Budget_BudgetLine,
            SubMenuID = _Parameter_Budget_BudgetLine.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Parameter_Budget_BudgetAllocated = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Parameter_Budget_BudgetAllocated,
            SubMenuID = _Parameter_Budget_BudgetAllocated.SubMenuID
        };
        static ActionSubMenuProfile adminSubMenu_Parameter_Budget_BudgetAllocatedUpdate = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Parameter_Budget_BudgetAllocatedUpdate,
            SubMenuID = _Parameter_Budget_BudgetAllocatedUpdate.SubMenuID
        };
        static ActionSubMenuProfile adminMenu_Sale_Report_ListClt = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Report_ListClt,
            SubMenuID = _Sale_Report_ListClt.SubMenuID
        };
        static ActionSubMenuProfile adminMenu_Sale_Report_HistoClt = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Report_HistoClt,
            SubMenuID = _Sale_Report_HistoClt.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_Sale_Report_ReturnSale = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Report_ReturnSale,
            SubMenuID = _Sale_Report_ReturnSale.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_CashRegister_CashTransmission = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _CashRegister_CashTransmission,
            SubMenuID = _CashRegister_CashTransmission.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_CashRegister_CashReception = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _CashRegister_CashReception,
            SubMenuID = _CashRegister_CashReception.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_CashRegister_BankTransmission = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _CashRegister_BankTransmission,
            SubMenuID = _CashRegister_BankTransmission.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_CashRegister_BankReception = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _CashRegister_BankReception,
            SubMenuID = _CashRegister_BankReception.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_Sale_ReceiveSpecialOrder = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Sale_ReceiveSpecialOrder,
            SubMenuID = _Sale_Sale_ReceiveSpecialOrder.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_Sale_PostedSpecialOrderToSupplier = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Sale_PostedSpecialOrderToSupplier,
            SubMenuID = _Sale_Sale_PostedSpecialOrderToSupplier.SubMenuID
        };


        static ActionSubMenuProfile adminMenu_CashRegister_ValidateSpecialOrder = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _CashRegister_ValidateSpecialOrder,
            SubMenuID = _CashRegister_ValidateSpecialOrder.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_CashRegister_DeliverSpecialOrder = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _CashRegister_DeliverSpecialOrder,
            SubMenuID = _CashRegister_DeliverSpecialOrder.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_Supply_Category_GenericCategory = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Category_GenericCategory,
            SubMenuID = _Supply_Category_GenericCategory.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_Supply_Category_LensCategory = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Supply_Category_LensCategory,
            SubMenuID = _Supply_Category_LensCategory.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_Sale_Pricing_Range = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Pricing_Range,
            SubMenuID = _Sale_Pricing_Range.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_Sale_Pricing_LensPrice = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _Sale_Pricing_LensPrice,
            SubMenuID = _Sale_Pricing_LensPrice.SubMenuID
        };

        static ActionSubMenuProfile adminMenu_CashRegister_State_SaleReceipt = new ActionSubMenuProfile()
        {
            Add = true,
            Delete = true,
            Update = true,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            SubMenu = _CashRegister_State_SaleReceipt,
            SubMenuID = _CashRegister_State_SaleReceipt.SubMenuID
        };

        /// <summary>
        /// fin de la création des ActionSubMenuProfile en vue de préparer l'affectation des sous menus à l'administrateur
        /// </summary>

        public static List<ActionSubMenuProfile> ActionSubMenuProfiles
        {
            get
            {
                return new List<ActionSubMenuProfile>() { adminMenu_Supply_Category_GenericCategory, adminMenu_Supply_Category_LensCategory, adminSubMenu_User_User, adminSubMenu_ProfileAvanced, adminSubMenu_Sale_Sale_NewSale, adminSubMenu_Sale_Sale_SaleReturn,
                                                          adminSubMenu_Localization_Country, adminSubMenu_Localization_Region, adminSubMenu_Localization_Town,
                                                          adminSub_Localization_Quarter, adminSubMenu_Lens, adminSubMenu_GenericProduct, adminSubMenu_OpenBD, adminSubMenu_CloseBD,
                                                          adminSubMenu_ClosingDayTask, adminSubMenu_Location, adminSubMenu_Inventory, adminSubMenu_InventoryEntry, adminSubMenu_InventoryDirectory, adminSubMenu_Parameter_Company_Company,adminSubMenu_Parameter_Company_Job,
                                                          adminSubMenu_Parameter_Company_Branch, adminSubMenu_ProductTransmission, adminSubMenu_ProductReception,adminSubMenu_ProductReception_BR, adminSubMenu_Sale_Sale_NewCommand, adminSubMenu_Sale_OrderLensOrder,
                                                          adminSubMenu_moneyManagerParameter_Till,adminSubMenu_moneyManagerParameter_bank,//adminSub_moneyManagerParameter_OtherPaymentMethod,
                                                          adminSubMenu_ProductTransmission, adminSubMenu_Sale_Sale_NewCommand,adminSubMenu_AccountOperation_SingleEntry,adminSubMenu_AccountOperation_MultiEntries,
                                                          adminSubMenu_Accounting_State_AcctEntry,adminSubMenu_CashRegister_State_Bill,adminSubMenu_CashRegister_State_Hisotric,
                                                          adminSubMenu_LensCoating, adminSubMenu_LensColour, adminSubMenu_LensMaterial, adminSubMenu_LensNumber,adminSubMenu_Accounting_State_AcctHistory,
                                                          adminSubMenu_Accounting_State_TrialBalance,adminSubMenu_Accounting_State_IncomeExpense,adminSubMenu_Parameter_Budget_FiscalYear,adminSubMenu_Parameter_Budget_BudgetLine,
                                                          adminSubMenu_Parameter_Budget_BudgetAllocated,adminSubMenu_Parameter_Budget_BudgetAllocatedUpdate,adminMenu_Sale_Report_ListClt,adminMenu_Sale_Report_HistoClt,
                                                          adminMenu_CashRegister_CashTransmission, adminMenu_CashRegister_CashReception, adminMenu_CashRegister_BankTransmission,
                                                          adminMenu_CashRegister_BankReception, adminMenu_Sale_ReceiveSpecialOrder, adminMenu_CashRegister_ValidateSpecialOrder, adminMenu_CashRegister_DeliverSpecialOrder,
                                                          adminMenu_Sale_PostedSpecialOrderToSupplier, adminMenu_Sale_Pricing_Range, adminMenu_Sale_Pricing_LensPrice,
                                                          adminMenu_CashRegister_State_SaleReceipt,adminMenu_Sale_Report_ReturnSale
                                                         };
            }
        }

        public static List<ActionSubMenuProfile> SuperAdminActionSubMenuProfiles
        {
            get
            {
                List<ActionSubMenuProfile> saSubMenus = new List<ActionSubMenuProfile>();

                foreach (ActionSubMenuProfile asmp in ActionSubMenuProfiles)
                {
                    asmp.Profile = superAdministrator;
                    asmp.ProfileID = superAdministrator.ProfileID;
                    saSubMenus.Add(asmp);
                }

                return saSubMenus;
            }
        }



        /*============================== Users initialization =================================*/
        private static Sex masculin = new Sex() { SexCode = "M", SexLabel = "Masculin" };
        private static Sex feminin = new Sex() { SexCode = "F", SexLabel = "Feminin" };
        //private static Quarter marcheA = new Quarter() { QuarterCode = "A", QuarterLabel = "Marché A", Town = bafoussam, TownID = bafoussam.TownID };

        /*----------------------Default User UserConfiguration to avoid more cliks*/

        public static UserConfiguration YdeUserConfig = new UserConfiguration()
        {
            DefaultBranch = YaoundeHeadBranch,
            DefaultBranchID = YaoundeHeadBranch.BranchID,
            DefaultDeviseID = xafDevise.DeviseID,
            DefaultCulture = "en",

        };

        public static UserConfiguration DlaUserConfig = new UserConfiguration()
        {
            DefaultBranch = DoualaHeadBranch,
            DefaultBranchID = DoualaHeadBranch.BranchID,
            DefaultDeviseID = xafDevise.DeviseID,
            DefaultCulture = "en",
        };

        public static List<UserConfiguration> UserConfigurations
        {
            get
            {
                return new List<UserConfiguration>() { YdeUserConfig, DlaUserConfig };
            }
        }

        private static User superAdminAccount = new User()
        {
            CNI = "2244MRDF87955",
            Name = "Super",
            Description = "FATSOG GROUP SARL",
            Sex = masculin,
            SexID = masculin.SexID,
            IsConnected = true,
            AdressID = adressAdmin.AdressID,
            Adress = adressAdmin,
            UserAccountState = true,
            UserLogin = "fatsod15",
            UserPassword = "fatsod_inv",
            UserAccessLevel = 10,
            Profile = superAdministrator,
            ProfileID = superAdministrator.ProfileID,
            JobID = computeristJob.JobID,
            Job = computeristJob,
            Code = "SuperAdminCode15",
            //UserConfigurationID = YdeUserConfig.UserConfigurationID,
            //UserConfiguration = YdeUserConfig,

        };
        private static User adminAccount = new User()
        {
            CNI = "12457DLA5",
            Name = "Admin",
            Description = "Admin Surname",
            Sex = masculin,
            SexID = masculin.SexID,
            IsConnected = true,
            AdressID = adressAdmin.AdressID,
            Adress = adressAdmin,
            UserAccountState = true,
            UserLogin = "dental",
            UserPassword = "access",
            UserAccessLevel = 5,
            Profile = administrator,
            ProfileID = administrator.ProfileID,
            JobID = computeristJob.JobID,
            Job = computeristJob,
            Code = "AdminCode",
            //UserConfigurationID = YdeUserConfig.UserConfigurationID,
            //UserConfiguration = YdeUserConfig,

        };


        public static List<User> Users
        {
            get
            {
                return new List<User>() { superAdminAccount, adminAccount };
            }
        }
        public static List<Sex> Sexes
        {
            get
            {
                return new List<Sex>() { feminin };
            }
        }
        /*======================================================================================*/
        /*=========================== User Branch initialization =================================*/
        private static UserBranch userBranch = new UserBranch()
        {
            Branch = YaoundeHeadBranch,
            BranchID = YaoundeHeadBranch.BranchID,
            User = adminAccount,
            UserID = adminAccount.GlobalPersonID
        };
        private static UserBranch super_user_Branch = new UserBranch()
        {
            Branch = YaoundeHeadBranch,
            BranchID = YaoundeHeadBranch.BranchID,
            User = superAdminAccount,
            UserID = superAdminAccount.GlobalPersonID
        };
        //private static UserBranch userBranchT = new UserBranch()
        //{
        //    Branch = scondaryBranch,
        //    BranchID = scondaryBranch.BranchID,
        //    User = adminAccount,
        //    UserID = adminAccount.GlobalPersonID
        //};
        public static List<UserBranch> UserBranches
        {
            get
            {
                return new List<UserBranch>() { super_user_Branch, userBranch };
            }
        }
        /*=====================================================================================*/
    }
}

