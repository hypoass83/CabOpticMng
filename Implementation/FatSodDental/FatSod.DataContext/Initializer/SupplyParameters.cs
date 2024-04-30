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

        //création du module Supply
        private static Module supply = new Module()
        {
            ModuleCode = CodeValue.Supply.SupplyModule.CODE,
            ModuleLabel = CodeValue.Supply.SupplyModule.LABEL,
            ModuleDescription = CodeValue.Supply.SupplyModule.DESCRIPTION,
            ModuleArea = CodeValue.Supply.SupplyModule.AREA,
            ModuleState = CodeValue.Supply.SupplyModule.MODULE_STATE,
            ModuleImageHeight = CodeValue.Supply.SupplyModule.MODULE_IMAGE_HEIGHT,
            ModuleImageWeight = CodeValue.Supply.SupplyModule.MODULE_IMAGE_WEIGHT,
            ModulePressedImagePath = CodeValue.Supply.SupplyModule.MODULE_PRESSED_IMAGE_PATH,
            ModuleImagePath = CodeValue.Supply.SupplyModule.MODULE_IMAGE_PATH,
            ModuleDisabledImagePath = CodeValue.Supply.SupplyModule.MODULE_DISABLED_IMAGE_PATH
        };

        /// <summary>
        /// début de la création des menus du module Supply
        /// </summary>

        //menu category du module supply
        private static Menu _Supply_Category = new Menu()
        {
            MenuLabel = CodeValue.Supply.CategoryMenu.LABEL,
            MenuCode = CodeValue.Supply.CategoryMenu.CODE,
            MenuDescription = CodeValue.Supply.CategoryMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.CategoryMenu.FLAT,
            MenuIconName = CodeValue.Supply.CategoryMenu.ICON_NAME,
            MenuController = null/*CodeValue.Supply.CategoryMenu.CONTROLLER*/,
            MenuPath = null/*CodeValue.Supply.CategoryMenu.PATH*/,
            MenuState = CodeValue.Supply.CategoryMenu.STATE,
            Module = supply,
            ModuleID = supply.ModuleID
        };

        //menu product du module supply
        private static Menu _Supply_Product = new Menu()
        {
            MenuLabel = CodeValue.Supply.ProductMenu.LABEL,
            MenuCode = CodeValue.Supply.ProductMenu.CODE,
            MenuDescription = CodeValue.Supply.ProductMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.ProductMenu.FLAT,
            MenuIconName = CodeValue.Supply.ProductMenu.ICON_NAME,
            MenuController = CodeValue.Supply.ProductMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.ProductMenu.PATH,
            MenuState = CodeValue.Supply.ProductMenu.STATE,
            Module = supply,
            ModuleID = supply.ModuleID
        };

        //menu Stock du module supply
        private static Menu _Supply_Stock = new Menu()
        {
            MenuLabel = CodeValue.Supply.StocktMenu.LABEL,
            MenuCode = CodeValue.Supply.StocktMenu.CODE,
            MenuDescription = CodeValue.Supply.StocktMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.StocktMenu.FLAT,
            MenuIconName = CodeValue.Supply.StocktMenu.ICON_NAME,
            MenuController = CodeValue.Supply.StocktMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.StocktMenu.PATH,
            MenuState = CodeValue.Supply.StocktMenu.STATE,
            Module = supply,
            ModuleID = supply.ModuleID
        };

        //menu achat du module supply
        private static Menu _Supply_Supply = new Menu()
        {
            MenuLabel = CodeValue.Supply.SupplyMenu.LABEL,
            MenuCode = CodeValue.Supply.SupplyMenu.CODE,
            MenuDescription = CodeValue.Supply.SupplyMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.SupplyMenu.FLAT,
            MenuIconName = CodeValue.Supply.SupplyMenu.ICON_NAME,
            MenuController = CodeValue.Supply.SupplyMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.SupplyMenu.PATH,
            MenuState = CodeValue.Supply.SupplyMenu.STATE,
            Module = supply,
            ModuleID = supply.ModuleID
        };

        //menu retour de marchandises au fournisseur du module supply
        private static Menu _Supply_SupplierReturn = new Menu()
        {
            MenuLabel = CodeValue.Supply.SupplierReturnMenu.LABEL,
            MenuCode = CodeValue.Supply.SupplierReturnMenu.CODE,
            MenuDescription = CodeValue.Supply.SupplierReturnMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.SupplierReturnMenu.FLAT,
            MenuIconName = CodeValue.Supply.SupplierReturnMenu.ICON_NAME,
            MenuController = CodeValue.Supply.SupplierReturnMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.SupplierReturnMenu.PATH,
            MenuState = CodeValue.Supply.SupplierReturnMenu.STATE,
            Module = supply,
            ModuleID = supply.ModuleID
        };

        //menu retour de marchandises au fournisseur du module supply
        private static Menu _Supply_SupplierOrder = new Menu()
        {
            MenuLabel = CodeValue.Supply.SupplierOrderMenu.LABEL,
            MenuCode = CodeValue.Supply.SupplierOrderMenu.CODE,
            MenuDescription = CodeValue.Supply.SupplierOrderMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.SupplierOrderMenu.FLAT,
            MenuIconName = CodeValue.Supply.SupplierOrderMenu.ICON_NAME,
            MenuController = CodeValue.Supply.SupplierOrderMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.SupplierOrderMenu.PATH,
            MenuState = CodeValue.Supply.SupplierOrderMenu.STATE,
            Module = supply,
            ModuleID = supply.ModuleID
        };

        //menu Fournisseur du module supply
        private static Menu _Supply_Supplier = new Menu()
        {
            MenuLabel = CodeValue.Supply.SupplierMenu.LABEL,
            MenuCode = CodeValue.Supply.SupplierMenu.CODE,
            MenuDescription = CodeValue.Supply.SupplierMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.SupplierMenu.FLAT,
            MenuIconName = CodeValue.Supply.SupplierMenu.ICON_NAME,
            MenuController = CodeValue.Supply.SupplierMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.SupplierMenu.PATH,
            MenuState = CodeValue.Supply.SupplierMenu.STATE,
            Module = supply,
            ModuleID = supply.ModuleID
        };

        //menu LensCriteria du module supply
        private static Menu _Supply_LensCriteria = new Menu()
        {
            MenuLabel = CodeValue.Supply.LensCriteriaMenu.LABEL,
            MenuCode = CodeValue.Supply.LensCriteriaMenu.CODE,
            MenuDescription = CodeValue.Supply.LensCriteriaMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.LensCriteriaMenu.FLAT,
            MenuIconName = CodeValue.Supply.LensCriteriaMenu.ICON_NAME,
            MenuController = CodeValue.Supply.LensCriteriaMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.LensCriteriaMenu.PATH,
            MenuState = CodeValue.Supply.LensCriteriaMenu.STATE,
            Module = supply,
            ModuleID = supply.ModuleID
        };

        //menu ProductTransfert du module supply
        static Menu _Supply_ProductTransfert = new Menu()
        {
            MenuLabel = CodeValue.Supply.ProductTransfertMenu.LABEL,
            MenuCode = CodeValue.Supply.ProductTransfertMenu.CODE,
            MenuDescription = CodeValue.Supply.ProductTransfertMenu.DESCRIPTION,
            MenuFlat = CodeValue.Supply.ProductTransfertMenu.FLAT,
            MenuIconName = CodeValue.Supply.ProductTransfertMenu.ICON_NAME,
            MenuController = CodeValue.Supply.ProductTransfertMenu.CONTROLLER,
            MenuPath = CodeValue.Supply.ProductTransfertMenu.PATH,
            MenuState = CodeValue.Supply.ProductTransfertMenu.STATE,
            Module = supply,
            ModuleID = supply.ModuleID,
        };

        /// <summary>
        /// fin de la création des menus du module Supply
        /// </summary>
        /// 

        /// <summary>
        /// début de la création des sous menus du module Supply
        /// </summary>
        /// 

        private static SubMenu _Supply_Category_GenericCategory = new SubMenu()
        {
            SubMenuLabel = CodeValue.Supply.CategoryMenu.LABEL,
            SubMenuCode = CodeValue.Supply.CategoryMenu.CODE,
            SubMenuDescription = CodeValue.Supply.CategoryMenu.DESCRIPTION,
            SubMenuController = CodeValue.Supply.CategoryMenu.CONTROLLER,
            SubMenuPath = CodeValue.Supply.CategoryMenu.PATH,
            Menu = _Supply_Category,
            MenuID = _Supply_Category.MenuID,
        };

        private static SubMenu _Supply_Category_LensCategory = new SubMenu()
        {
            SubMenuLabel = CodeValue.Supply.LensCategorySM.LABEL,
            SubMenuCode = CodeValue.Supply.LensCategorySM.CODE,
            SubMenuDescription = CodeValue.Supply.LensCategorySM.DESCRIPTION,
            SubMenuController = CodeValue.Supply.LensCategorySM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.LensCategorySM.PATH,
            Menu = _Supply_Category,
            MenuID = _Supply_Category.MenuID,
        };

        //sous menu Lens(verre) du menu product du module supply
        static SubMenu _Supply_Product_Lens = new SubMenu()
        {
            Menu = _Supply_Product,
            MenuID = _Supply_Product.MenuID,
            SubMenuCode = CodeValue.Supply.LensProduct_SM.CODE,
            SubMenuLabel = CodeValue.Supply.LensProduct_SM.LABEL,
            SubMenuDescription = CodeValue.Supply.LensProduct_SM.DESCRIPTION,
            SubMenuController = CodeValue.Supply.LensProduct_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.LensProduct_SM.PATH
        };

        //sous menu produit générique du menu product du module supply
        static SubMenu _Supply_Product_GenericProduct = new SubMenu()
        {
            Menu = _Supply_Product,
            MenuID = _Supply_Product.MenuID,
            SubMenuCode = CodeValue.Supply.GenericProduct_SM.CODE,
            SubMenuDescription = CodeValue.Supply.GenericProduct_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.GenericProduct_SM.LABEL,
            SubMenuController = CodeValue.Supply.GenericProduct_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.GenericProduct_SM.PATH
        };

        //sous menu location du menu Stock du module supply
        static SubMenu _Supply_Stock_Location = new SubMenu()
        {
            Menu = _Supply_Stock,
            MenuID = _Supply_Stock.MenuID,
            SubMenuCode = CodeValue.Supply.Location_SM.CODE,
            SubMenuDescription = CodeValue.Supply.Location_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.Location_SM.LABEL,
            SubMenuController = CodeValue.Supply.Location_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.Location_SM.PATH
        };

        //sous menu InventoryDirectory du menu Stock du module supply
        static SubMenu _Supply_Stock_InventoryDirectory = new SubMenu()
        {
            Menu = _Supply_Stock,
            MenuID = _Supply_Stock.MenuID,
            SubMenuCode = CodeValue.Supply.InventoryDirectory_SM.CODE,
            SubMenuDescription = CodeValue.Supply.InventoryDirectory_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.InventoryDirectory_SM.LABEL,
            SubMenuController = CodeValue.Supply.InventoryDirectory_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.InventoryDirectory_SM.PATH
        };

        //sous menu InventoryEntry du menu Stock du module supply
        static SubMenu _Supply_Stock_InventoryEntry = new SubMenu()
        {
            Menu = _Supply_Stock,
            MenuID = _Supply_Stock.MenuID,
            SubMenuCode = CodeValue.Supply.InventoryEntry_SM.CODE,
            SubMenuDescription = CodeValue.Supply.InventoryEntry_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.InventoryEntry_SM.LABEL,
            SubMenuController = CodeValue.Supply.InventoryEntry_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.InventoryEntry_SM.PATH
        };

        //sous menu Inventory du menu Stock du module supply
        static SubMenu _Supply_Stock_Inventory = new SubMenu()
        {
            Menu = _Supply_Stock,
            MenuID = _Supply_Stock.MenuID,
            SubMenuCode = CodeValue.Supply.Inventory_SM.CODE,
            SubMenuDescription = CodeValue.Supply.Inventory_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.Inventory_SM.LABEL,
            SubMenuController = CodeValue.Supply.Inventory_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.Inventory_SM.PATH
        };

        //sous menu LensMaterial du menu LensCriteria du module supply
        static SubMenu _Supply_LensCriteria_LensMaterial = new SubMenu()
        {
            Menu = _Supply_LensCriteria,
            MenuID = _Supply_LensCriteria.MenuID,
            SubMenuCode = CodeValue.Supply.LensMaterial_SM.CODE,
            SubMenuDescription = CodeValue.Supply.LensMaterial_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.LensMaterial_SM.LABEL,
            SubMenuController = CodeValue.Supply.LensMaterial_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.LensMaterial_SM.PATH
        };

        //sous menu LensCoating du menu LensCriteria du module supply
        static SubMenu _Supply_LensCriteria_LensCoating = new SubMenu()
        {
            Menu = _Supply_LensCriteria,
            MenuID = _Supply_LensCriteria.MenuID,
            SubMenuCode = CodeValue.Supply.LensCoating_SM.CODE,
            SubMenuDescription = CodeValue.Supply.LensCoating_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.LensCoating_SM.LABEL,
            SubMenuController = CodeValue.Supply.LensCoating_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.LensCoating_SM.PATH
        };

        //sous menu LensColour du menu LensCriteria du module supply
        static SubMenu _Supply_LensCriteria_LensColour = new SubMenu()
        {
            Menu = _Supply_LensCriteria,
            MenuID = _Supply_LensCriteria.MenuID,
            SubMenuCode = CodeValue.Supply.LensColour_SM.CODE,
            SubMenuDescription = CodeValue.Supply.LensColour_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.LensColour_SM.LABEL,
            SubMenuController = CodeValue.Supply.LensColour_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.LensColour_SM.PATH
        };

        //sous menu LensNumber du menu LensCriteria du module supply
        static SubMenu _Supply_LensCriteria_LensNumber = new SubMenu()
        {
            Menu = _Supply_LensCriteria,
            MenuID = _Supply_LensCriteria.MenuID,
            SubMenuCode = CodeValue.Supply.LensNumber_SM.CODE,
            SubMenuDescription = CodeValue.Supply.LensNumber_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.LensNumber_SM.LABEL,
            SubMenuController = CodeValue.Supply.LensNumber_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.LensNumber_SM.PATH
        };

        /*
                //sous menu StockEntry du menu Stock du module supply
                static SubMenu _Supply_Stock_StockEntry = new SubMenu()
                {
                    Menu = _Supply_Stock,
                    MenuID = _Supply_Stock.MenuID,
                    SubMenuCode = CodeValue.Supply.StockEntry_SM.CODE,
                    SubMenuDescription = CodeValue.Supply.StockEntry_SM.DESCRIPTION,
                    SubMenuLabel = CodeValue.Supply.StockEntry_SM.LABEL,
                    SubMenuController = CodeValue.Supply.StockEntry_SM.CONTROLLER,
                    SubMenuPath = CodeValue.Supply.StockEntry_SM.PATH
                };

                //sous menu StockOutput du menu Stock du module supply
                static SubMenu _Supply_Stock_StockOutput = new SubMenu()
                {
                    Menu = _Supply_Stock,
                    MenuID = _Supply_Stock.MenuID,
                    SubMenuCode = CodeValue.Supply.StockOutput_SM.CODE,
                    SubMenuDescription = CodeValue.Supply.StockOutput_SM.DESCRIPTION,
                    SubMenuLabel = CodeValue.Supply.StockOutput_SM.LABEL,
                    SubMenuController = CodeValue.Supply.StockOutput_SM.CONTROLLER,
                    SubMenuPath = CodeValue.Supply.StockOutput_SM.PATH
                };
        */
        //sous menu ProductTransmission du menu ProductTransfert du module supply 
        static SubMenu _Supply_Transfert_ProductTransmission = new SubMenu()
        {
            Menu = _Supply_ProductTransfert,
            MenuID = _Supply_ProductTransfert.MenuID,
            SubMenuCode = CodeValue.Supply.ProductTransmission_SM.CODE,
            SubMenuDescription = CodeValue.Supply.ProductTransmission_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.ProductTransmission_SM.LABEL,
            SubMenuController = CodeValue.Supply.ProductTransmission_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.ProductTransmission_SM.PATH
        };

        //sous menu ProductTransmission du menu ProductTransfert du module supply 
        static SubMenu _Supply_Transfert_ProductReception_SM = new SubMenu()
        {
            Menu = _Supply_ProductTransfert,
            MenuID = _Supply_ProductTransfert.MenuID,
            SubMenuCode = CodeValue.Supply.ProductReception_SM.CODE,
            SubMenuDescription = CodeValue.Supply.ProductReception_SM.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.ProductReception_SM.LABEL,
            SubMenuController = CodeValue.Supply.ProductReception_SM.CONTROLLER,
            SubMenuPath = CodeValue.Supply.ProductReception_SM.PATH
        };

        //sous menu ProductTransmission du menu ProductTransfert du module supply 
        static SubMenu _Supply_Transfert_ProductReception_BR = new SubMenu()
        {
            Menu = _Supply_ProductTransfert,
            MenuID = _Supply_ProductTransfert.MenuID,
            SubMenuCode = CodeValue.Supply.ProductReception_BR.CODE,
            SubMenuDescription = CodeValue.Supply.ProductReception_BR.DESCRIPTION,
            SubMenuLabel = CodeValue.Supply.ProductReception_BR.LABEL,
            SubMenuController = CodeValue.Supply.ProductReception_BR.CONTROLLER,
            SubMenuPath = CodeValue.Supply.ProductReception_BR.PATH
        };

        /// <summary>
        /// fin de la création des sous menus du module Supply
        /// </summary>
        /// 

        //private static Category lens = new Category()
        //{
        //    CategoryCode = CodeValue.Supply.lensCategoryCode,
        //    CategoryLabel = CodeValue.Supply.lensCategoryCode,
        //    CategoryDescription = "Category devant contenir tous les produits de type verre de DBOY"
        //};
        //public static List<Category> Categories
        //{
        //    get
        //    {
        //        return new List<Category>() { lens };
        //    }
        //}

        private static LensMaterial lensMaterialMin = new LensMaterial()
        {
            LensMaterialCode = "MIN",
            LensMaterialLabel = "Mineral",
            LensMaterialDescription = "Mineral"
        };

        private static LensMaterial lensMaterialCR39 = new LensMaterial()
        {
            LensMaterialCode = "CR39",
            LensMaterialLabel = "Organic",
            LensMaterialDescription = "Organic"
        };

        private static LensMaterial trivex = new LensMaterial()
        {
            LensMaterialCode = "TRIV",
            LensMaterialLabel = "Trivex",
            LensMaterialDescription = "Trive"
        };

        private static LensMaterial polycarbonate = new LensMaterial()
        {
            LensMaterialCode = "POLY",
            LensMaterialLabel = "POLY",
            LensMaterialDescription = "Polycarbonate"
        };

        private static LensMaterial DefaultLensMaterial = new LensMaterial()
        {
            LensMaterialCode = CodeValue.Supply.DefaultLensMaterial,
            LensMaterialLabel = CodeValue.Supply.DefaultLensMaterial,
            LensMaterialDescription = CodeValue.Supply.DefaultLensMaterial
        };

        public static List<LensMaterial> LensMaterials
        {
            get
            {
                return new List<LensMaterial>() { lensMaterialMin, lensMaterialCR39, trivex, polycarbonate, DefaultLensMaterial };
            }
        }


        private static LensCoating lensCoatingDefault = new LensCoating()
        {
            LensCoatingCode = CodeValue.Supply.DefaultLensCoating,
            LensCoatingLabel = CodeValue.Supply.DefaultLensCoating,
            LensCoatingDescription = CodeValue.Supply.DefaultLensCoating
        };
        private static LensCoating lensCoatingAR = new LensCoating()
        {
            LensCoatingCode = "AR",
            LensCoatingLabel = "Anti-Reflet",
            LensCoatingDescription = "Anti-Reflet"
        };
        public static List<LensCoating> LensCoatings
        {
            get
            {
                return new List<LensCoating>() { lensCoatingDefault, lensCoatingAR };
            }
        }


        private static LensColour lensColourDefault = new LensColour()
        {
            LensColourCode = CodeValue.Supply.DefaultLensColour,
            LensColourLabel = CodeValue.Supply.DefaultLensColour,
            LensColourDescription = CodeValue.Supply.DefaultLensColour
        };

        private static LensColour lensColourBlanc = new LensColour()
        {
            LensColourCode = "WHITE",
            LensColourLabel = "WHITE",
            LensColourDescription = "WHITE"
        };

        private static LensColour lensColourPhoto = new LensColour()
        {
            LensColourCode = "PHOTO",
            LensColourLabel = "Photochromic",
            LensColourDescription = "Photochromic"
        };

        public static List<LensColour> LensColours
        {
            get
            {
                return new List<LensColour>() { lensColourDefault, lensColourBlanc, lensColourPhoto };
            }
        }
        //initialise lens category
        //CR39 WHITE
        private static LensCategory CR39WHITE = new LensCategory()
        {
            CategoryCode = "SV CR39 WHITE",
            CategoryLabel = "Single Vision Organic White",
            CategoryDescription = "Single Vision Organic White",
            BifocalCode = "",
            IsProgressive = false,
            LensMaterial = lensMaterialCR39,
            LensMaterialID = lensMaterialCR39.LensMaterialID,
            LensCoating = lensCoatingDefault,
            LensCoatingID = lensCoatingDefault.LensCoatingID,
            LensColour = lensColourBlanc,
            LensColourID = lensColourBlanc.LensColourID,
            CollectifAccount = CR39WHITECollectifAccount,
            CollectifAccountID = CR39WHITECollectifAccount.CollectifAccountID
        };
        //CR39 WHITE HMC (AR)
        private static LensCategory CR39WHITEHMC = new LensCategory()
        {
            CategoryCode = "SV CR39 WHITE HMC (AR)",
            CategoryLabel = "Single Vision Organic White HMC (AR)",
            CategoryDescription = "Single Vision Organic White HMC (AR)",
            BifocalCode = "",
            IsProgressive = false,
            LensMaterial = lensMaterialCR39,
            LensMaterialID = lensMaterialCR39.LensMaterialID,
            LensCoating = lensCoatingAR,
            LensCoatingID = lensCoatingAR.LensCoatingID,
            LensColour = lensColourBlanc,
            LensColourID = lensColourBlanc.LensColourID,
            CollectifAccount = CR39WHITEHMCCollectifAccount,
            CollectifAccountID = CR39WHITEHMCCollectifAccount.CollectifAccountID
        };
        //BF PGX SD
        private static LensCategory BFPGXDT = new LensCategory()
        {
            CategoryCode = "BF PGX D-TOP",
            CategoryLabel = "Bifocal PGX D-TOP",
            CategoryDescription = "Bifocal PGX D-TOP",
            BifocalCode = "D-TOP",
            IsProgressive = false,
            LensMaterial = lensMaterialMin,
            LensMaterialID = lensMaterialMin.LensMaterialID,
            LensCoating = lensCoatingDefault,
            LensCoatingID = lensCoatingDefault.LensCoatingID,
            LensColour = lensColourPhoto,
            LensColourID = lensColourPhoto.LensColourID,
            CollectifAccount = BFPGXDTCollectifAccount,
            CollectifAccountID = BFPGXDTCollectifAccount.CollectifAccountID
        };
        private static LensCategory BFPGXRT = new LensCategory()
        {
            CategoryCode = "BF PGX R-TOP",
            CategoryLabel = "Bifocal PGX Round Top",
            CategoryDescription = "Bifocal PGX Round Top",
            BifocalCode = "R-TOP",
            IsProgressive = false,
            LensMaterial = lensMaterialMin,
            LensMaterialID = lensMaterialMin.LensMaterialID,
            LensCoating = lensCoatingDefault,
            LensCoatingID = lensCoatingDefault.LensCoatingID,
            LensColour = lensColourPhoto,
            LensColourID = lensColourPhoto.LensColourID,
            CollectifAccount = BFPGXRTCollectifAccount,
            CollectifAccountID = BFPGXRTCollectifAccount.CollectifAccountID
        };
        private static LensCategory PGX = new LensCategory()
        {
            CategoryCode = "SV PGX",
            CategoryLabel = "Single Vision PGX",
            CategoryDescription = "Single Vision PGX",
            BifocalCode = "",
            IsProgressive = false,
            LensMaterial = lensMaterialMin,
            LensMaterialID = lensMaterialMin.LensMaterialID,
            LensCoating = lensCoatingDefault,
            LensCoatingID = lensCoatingDefault.LensCoatingID,
            LensColour = lensColourPhoto,
            LensColourID = lensColourPhoto.LensColourID,
            CollectifAccount = PGXCollectifAccount,
            CollectifAccountID = PGXCollectifAccount.CollectifAccountID
        };
        private static LensCategory PGXHMC = new LensCategory()
        {
            CategoryCode = "SV PGX HMC (AR)",
            CategoryLabel = "Single Vision PGX HMC (AR)",
            CategoryDescription = "Single Vision PGX HMC (AR)",
            BifocalCode = "",
            IsProgressive = false,
            LensMaterial = lensMaterialMin,
            LensMaterialID = lensMaterialMin.LensMaterialID,
            LensCoating = lensCoatingAR,
            LensCoatingID = lensCoatingAR.LensCoatingID,
            LensColour = lensColourPhoto,
            LensColourID = lensColourPhoto.LensColourID,
            CollectifAccount = PGXHMCCollectifAccount,
            CollectifAccountID = PGXHMCCollectifAccount.CollectifAccountID
        };
        private static LensCategory PROCR39WHITEHMC = new LensCategory()
        {
            CategoryCode = "PRO CR39 WHITE HMC (AR)",
            CategoryLabel = "Progressive Organic White HMC (AR)",
            CategoryDescription = "Progressive Organic White HMC (AR)",
            BifocalCode = "",
            IsProgressive = true,
            LensMaterial = lensMaterialCR39,
            LensMaterialID = lensMaterialCR39.LensMaterialID,
            LensCoating = lensCoatingAR,
            LensCoatingID = lensCoatingAR.LensCoatingID,
            LensColour = lensColourBlanc,
            LensColourID = lensColourBlanc.LensColourID,
            CollectifAccount = PROCR39WHITEHMCCollectifAccount,
            CollectifAccountID = PROCR39WHITEHMCCollectifAccount.CollectifAccountID
        };
        private static LensCategory PROPGXHMC = new LensCategory()
        {
            CategoryCode = "PRO PGX HMC (AR)",
            CategoryLabel = "Progressive PGX HMC (AR)",
            CategoryDescription = "Progressive PGX HMC (AR)",
            BifocalCode = "",
            IsProgressive = true,
            LensMaterial = lensMaterialMin,
            LensMaterialID = lensMaterialMin.LensMaterialID,
            LensCoating = lensCoatingAR,
            LensCoatingID = lensCoatingAR.LensCoatingID,
            LensColour = lensColourPhoto,
            LensColourID = lensColourPhoto.LensColourID,
            CollectifAccount = PROPGXHMCCollectifAccount,
            CollectifAccountID = PROPGXHMCCollectifAccount.CollectifAccountID
        };
        private static LensCategory PROPGX = new LensCategory()
        {
            CategoryCode = "PRO PGX",
            CategoryLabel = "Progressive PGX",
            CategoryDescription = "Progressive PGX",
            BifocalCode = "",
            IsProgressive = true,
            LensMaterial = lensMaterialMin,
            LensMaterialID = lensMaterialMin.LensMaterialID,
            LensCoating = lensCoatingDefault,
            LensCoatingID = lensCoatingDefault.LensCoatingID,
            LensColour = lensColourPhoto,
            LensColourID = lensColourPhoto.LensColourID,
            CollectifAccount = PROPGXCollectifAccount,
            CollectifAccountID = PROPGXCollectifAccount.CollectifAccountID
        };
        private static LensCategory PROTRANSITIONHMC = new LensCategory()
        {
            CategoryCode = "PRO TRANSITION HMC (AR)",
            CategoryLabel = "Progressive TRANSITION HMC (AR)",
            CategoryDescription = "Progressive TRANSITION HMC (AR)",
            BifocalCode = "",
            IsProgressive = true,
            LensMaterial = lensMaterialCR39,
            LensMaterialID = lensMaterialCR39.LensMaterialID,
            LensCoating = lensCoatingAR,
            LensCoatingID = lensCoatingAR.LensCoatingID,
            LensColour = lensColourPhoto,
            LensColourID = lensColourPhoto.LensColourID,
            CollectifAccount = PROTRANSITIONHMCCollectifAccount,
            CollectifAccountID = PROTRANSITIONHMCCollectifAccount.CollectifAccountID
        };
        private static LensCategory TRANSITIONHMC = new LensCategory()
        {
            CategoryCode = "SV TRANSITION HMC (AR)",
            CategoryLabel = "Single Vision TRANSITION HMC (AR)",
            CategoryDescription = "Single Vision TRANSITION HMC (AR)",
            BifocalCode = "",
            IsProgressive = false,
            LensMaterial = lensMaterialCR39,
            LensMaterialID = lensMaterialCR39.LensMaterialID,
            LensCoating = lensCoatingAR,
            LensCoatingID = lensCoatingAR.LensCoatingID,
            LensColour = lensColourPhoto,
            LensColourID = lensColourPhoto.LensColourID,
            CollectifAccount = TRANSITIONHMCCollectifAccount,
            CollectifAccountID = TRANSITIONHMCCollectifAccount.CollectifAccountID
        };
        public static List<LensCategory> LensCategories
        {
            get
            {
                return new List<LensCategory>() { CR39WHITE, CR39WHITEHMC, BFPGXDT, BFPGXRT, PGX, PGXHMC, 
                    PROCR39WHITEHMC,PROPGXHMC,PROPGX,PROTRANSITIONHMC,TRANSITIONHMC };
            }
        }

        private static Category Frame = new Category()
        {
            CategoryCode = "Frames",
            CategoryLabel = "Frame",
            CategoryDescription = "Frames",
        };
        private static Category Equipment = new Category()
        {
            CategoryCode = "Equipment",
            CategoryLabel = "Optical equipment",
            CategoryDescription = "Optical equipment",
        };
        public static List<Category> Categories
        {
            get
            {
                return new List<Category>() { Frame, Equipment };
            }
        }
        //// dla
        //private static Localization WarehouseDlaBranch = new Localization()
        //{
        //    LocalizationCode = "Warehouse Stocks",
        //    LocalizationLabel = "Warehouse Stocks",
        //    LocalizationDescription = "Warehouse Stocks",
        //    Quarter = dlaAkwa,
        //    QuarterID = dlaAkwa.QuarterID,
        //    Branch = DoualaHeadBranch,
        //    BranchID = DoualaHeadBranch.BranchID,
        //};
        //// yde
        //private static Localization WarehouseYdeBranch = new Localization()
        //{
        //    LocalizationCode = "Warehouse Stocks",
        //    LocalizationLabel = "Warehouse Stocks",
        //    LocalizationDescription = "Warehouse Stocks",
        //    Quarter = ydeAvenueKennedy,
        //    QuarterID = ydeAvenueKennedy.QuarterID,
        //    Branch = YaoundeHeadBranch,
        //    BranchID = YaoundeHeadBranch.BranchID,
        //};
        //// dla
        //private static Localization DboyDlaBranch = new Localization()
        //{
        //    LocalizationCode = "D-Boy Distribution",
        //    LocalizationLabel = "D-Boy Distribution",
        //    LocalizationDescription = "D-Boy Distribution",
        //    Quarter = dlaAkwa,
        //    QuarterID = dlaAkwa.QuarterID,
        //    Branch = DoualaHeadBranch,
        //    BranchID = DoualaHeadBranch.BranchID,
        //};
        //// yde
        //private static Localization DboyYdeBranch = new Localization()
        //{
        //    LocalizationCode = "D-Boy Distribution",
        //    LocalizationLabel = "D-Boy Distribution",
        //    LocalizationDescription = "D-Boy Distribution",
        //    Quarter = ydeAvenueKennedy,
        //    QuarterID = ydeAvenueKennedy.QuarterID,
        //    Branch = YaoundeHeadBranch,
        //    BranchID = YaoundeHeadBranch.BranchID,
        //};
        //// yde
        //private static Localization ValdozYdeBranch = new Localization()
        //{
        //    LocalizationCode = "Valdoz Optique",
        //    LocalizationLabel = "Valdoz Optique",
        //    LocalizationDescription = "Valdoz Optique",
        //    Quarter = ydeAvenueKennedy,
        //    QuarterID = ydeAvenueKennedy.QuarterID,
        //    Branch = YaoundeHeadBranch,
        //    BranchID = YaoundeHeadBranch.BranchID,
        //};
        ////ecriture des magasins
        //public static List<Localization> Localizations
        //{
        //    get
        //    {
        //        return new List<Localization>() { WarehouseDlaBranch, WarehouseYdeBranch, DboyDlaBranch, DboyYdeBranch, ValdozYdeBranch };
        //    }
        //}
        //fabrication des numero
        public static List<LensNumber> SphericalVal //sph = -20.00 to +20.00 et cyl = "" | 0.00
        {
            get
            {
                List<LensNumber> sph = new List<LensNumber>();
                string sphval = "";
                for (decimal i = -20.00m; i <= 20.00m; i += 0.25m)
                {
                    if (i > 0) sphval = "+" + i.ToString().Replace(",", ".");
                    else sphval = i.ToString().Replace(",", ".");
                    sph.Add
                    (
                        new LensNumber
                        {
                            LensNumberSphericalValue = sphval,
                            LensNumberCylindricalValue = "",
                            LensNumberDescription = sphval,
                            LensNumberAdditionValue = "",
                        }
                    );
                }
                return sph;
            }

        }
        public static List<LensNumber> CylindricalVal //sph = -20.00 to +20.00 et cyl = -6.00 to +6.00(exclu "" | 0.00)
        {
            get
            {
                List<LensNumber> cyl = new List<LensNumber>();
                string cylval = "";
                string sphval = "";
                for (decimal i = -6.00m; i <= 6.00m; i += 0.25m)
                {
                    if (i > 0) cylval = "+" + i.ToString().Replace(",", ".");
                    else cylval = i.ToString().Replace(",", ".");

                    if (cylval == "0.00") continue; //cylval = "";//pour éviter les numéros du genre +1.00 0.00 = +1.00 créés dans SphericalVal

                    for (decimal j = -20.00m; j <= 20.00m; j += 0.25m)
                    {
                        if (j > 0) sphval = "+" + j.ToString().Replace(",", ".");
                        else sphval = j.ToString().Replace(",", ".");
                        cyl.Add
                        (
                            new LensNumber
                            {
                                LensNumberSphericalValue = sphval,
                                LensNumberCylindricalValue = cylval,
                                LensNumberDescription = sphval + " " + cylval,
                                LensNumberAdditionValue = "",
                            }
                        );
                    }
                }
                return cyl;
            }
        }
        public static List<LensNumber> SphericalValAddCylindricalVal //sph = -4.00 to +4.00 et /*cyl = -6.00 to +6.00*/ add = -4.00 to +4.00 (exclusion de 0.00)
        {
            get
            {
                List<LensNumber> spheaddcyl = new List<LensNumber>();
                string addVal = "";
                string sphval = "";
                for (decimal i = 0.00m; i <= 4.00m; i += 0.25m)
                {
                    if (i == 0) continue;//ceci a déjà été créée dans SphericalVal et CylindricalVal
                    if (i > 0) addVal = "+" + i.ToString().Replace(",", ".");
                    else addVal = i.ToString().Replace(",", ".");
                    for (decimal j = -4.00m; j <= 4.00m; j += 0.25m)
                    {
                        if (j > 0) sphval = "+" + j.ToString().Replace(",", ".");
                        else sphval = j.ToString().Replace(",", ".");
                        spheaddcyl.Add
                        (
                            //Ceci n'intègre pas les numéros du genre 
                            //a - +1.00 -1.00 add +2.00
                            //b - +1.00 -1.00@60 add +2.00
                            new LensNumber
                            {
                                LensNumberSphericalValue = sphval,
                                LensNumberAdditionValue = addVal,
                                LensNumberDescription = sphval + " add " + addVal,
                                LensNumberCylindricalValue = "",
                            }
                        );
                    }
                }
                return spheaddcyl;
            }
        }

        public static List<LensNumberRange> LensNumberRanges //-4.00 to +4.00 ADD 0.00 TO 4.00
        {
            get
            {
                List<LensNumberRange> lensNumberRanges = new List<LensNumberRange>();

                decimal init = -20.00m;

                //Génération de toutes les intervalles sphériques qui contiennent les intervalles cylindriques
                while (init < 20.00m)
                {
                    decimal j = (init + 1.75m);
                    LensNumberRange lnr = new LensNumberRange() // ;
                    {
                        Minimum = (init > 0) ? ("+" + init.ToString().Replace(",", ".")) : ("" + init.ToString().Replace(",", ".")),
                        Maximum = (j > 0) ? ("+" + j.ToString().Replace(",", ".")) : ("" + j.ToString().Replace(",", ".")),
                    };

                    lensNumberRanges.Add(lnr);

                    init += 2.00m;
                }

                 LensNumberRange lnr1 = new LensNumberRange()
                {
                    Minimum = "+" + 20.00m.ToString().Replace(",", "."),
                    Maximum = "+" + 20.00m.ToString().Replace(",", "."),
                    //IsAdditionRange = false, 
                };

                 LensNumberRange lnr2 = new LensNumberRange()
                {
                    Minimum = "+" + 1.00m.ToString().Replace(",", "."),
                    Maximum = "+" + 3.00m.ToString().Replace(",", "."),
                    //IsAdditionRange = true, 
                };

                lensNumberRanges.Add(lnr1);
                lensNumberRanges.Add(lnr2);

                return lensNumberRanges;
            }
        }

    }
}
