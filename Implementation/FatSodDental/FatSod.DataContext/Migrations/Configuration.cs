namespace FatSod.DataContext.Migrations
{
    using FatSod.Security.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Collections.Generic;
    using FatSod.Supply.Entities;
    using FatSod.DataContext.Concrete;
    using FatSod.DataContext.Initializer;

    internal sealed class Configuration : DbMigrationsConfiguration<FatSod.DataContext.Concrete.EFDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "FatSod.DataContext.Concrete.EFDbContext";
        }

        protected override void Seed(FatSod.DataContext.Concrete.EFDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            #region OLD STUFFS

            //menu general sale
            Menu RptCashMenu = context.Menus.Where(m => m.MenuCode == "Z_VAL_STATE_CASH").FirstOrDefault();
            SubMenu existRptCashMenu = context.SubMenus.Where(s => s.SubMenuCode == "RptGeneSale").FirstOrDefault();
            if (existRptCashMenu == null)
            {
                SubMenu RptGeneSale = new SubMenu()
                {
                    Menu = RptCashMenu,
                    MenuID = RptCashMenu.MenuID,
                    SubMenuCode = "RptGeneSale",
                    SubMenuDescription = "RptGeneSale menu",
                    SubMenuLabel = "RptGeneSale",
                    SubMenuController = "RptGeneSale",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(RptGeneSale);
                context.SaveChanges();
            }

            SubMenu existRptBankHistMenu = context.SubMenus.Where(s => s.SubMenuCode == "RptBankHistMenu").FirstOrDefault();
            if (existRptBankHistMenu == null)
            {
                SubMenu RptBankHistMenu = new SubMenu()
                {
                    Menu = RptCashMenu,
                    MenuID = RptCashMenu.MenuID,
                    SubMenuCode = "RptBankHistMenu",
                    SubMenuDescription = "RptBankHistMenu menu",
                    SubMenuLabel = "RptBankHistMenu",
                    SubMenuController = "State",
                    SubMenuPath = "BankHistOperation"
                };
                context.SubMenus.AddOrUpdate(RptBankHistMenu);
                context.SaveChanges();
            }

            //suppression du menu BorderoDepotFacture ds caisse
            Menu oldBorderoDepotFacture = context.Menus.Where(s => s.MenuCode == "BorderoDepotFacture" && s.ModuleID != 2).FirstOrDefault();
            if (oldBorderoDepotFacture != null)
            {
                //supression du menu
                List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == oldBorderoDepotFacture.MenuID).ToList();
                context.ActionMenuProfiles.RemoveRange(lstActionMP);
                context.Menus.Remove(oldBorderoDepotFacture);
                context.SaveChanges();
            }



            //ajout des menus de proformat et de validation proformat
            Menu existMenuProforma = context.Menus.Where(s => s.MenuCode == "MenuProforma").FirstOrDefault();
            if (existMenuProforma == null)
            {
                Menu MenuProforma = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "MenuProforma",
                    MenuController = "MenuProforma",
                    MenuDescription = "Menu Proforma",
                    MenuFlat = false,
                    MenuIconName = "MenuProforma",
                    MenuLabel = "MenuProforma",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 2,
                    AppearanceOrder = 0
                };
                context.Menus.AddOrUpdate(MenuProforma);
                context.SaveChanges();
            }
            Menu existMenuValideProforma = context.Menus.Where(s => s.MenuCode == "MenuValideProforma").FirstOrDefault();
            if (existMenuValideProforma == null)
            {
                Menu MenuValideProforma = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "MenuValideProforma",
                    MenuController = "MenuValideProforma",
                    MenuDescription = "Menu Valide Proforma",
                    MenuFlat = false,
                    MenuIconName = "MenuValideProforma",
                    MenuLabel = "MenuValideProforma",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 2,
                    AppearanceOrder = 1
                };
                context.Menus.AddOrUpdate(MenuValideProforma);
                context.SaveChanges();
            }

            //CREATION DU MENU Saisi facture directe
            Menu existMnuDirectBill = context.Menus.Where(s => s.MenuCode == "MnuVDIRECTBILL").FirstOrDefault();
            if (existMnuDirectBill == null)
            {
                Menu MnuDIRECTBILL = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "MnuVDIRECTBILL",
                    MenuController = "DIRECTBILL",
                    MenuDescription = "Menu DIRECTBILL",
                    MenuFlat = false,
                    MenuIconName = "DIRECTBILL",
                    MenuLabel = "DIRECTBILL",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 2,
                    AppearanceOrder = 2
                };
                context.Menus.AddOrUpdate(MnuDIRECTBILL);
                context.SaveChanges();
            }

            //creation du menu BorderoDepotFacture ds assurance
            Menu existBorderoDepotFacture = context.Menus.Where(s => s.MenuCode == "BorderoDepotFacture").FirstOrDefault();
            if (existBorderoDepotFacture == null)
            {
                Menu BorderoDepotFacture = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "BorderoDepotFacture",
                    MenuController = "BorderoDepotFacture",
                    MenuDescription = "Menu Bordero Depot Facture",
                    MenuFlat = false,
                    MenuIconName = "BorderoDepotFacture",
                    MenuLabel = "BorderoDepotFacture",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 2,
                    AppearanceOrder = 3
                };
                context.Menus.AddOrUpdate(BorderoDepotFacture);
                context.SaveChanges();
            }

            //delete ValideBorderoDepotFacture ds caisse
            Menu oldValideBorderoDepotFacture = context.Menus.Where(s => s.MenuCode == "ValideBorderoDepotFacture" && s.ModuleID != 2).FirstOrDefault();
            if (oldValideBorderoDepotFacture != null)
            {
                //supression du menu
                List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == oldValideBorderoDepotFacture.MenuID).ToList();
                context.ActionMenuProfiles.RemoveRange(lstActionMP);
                context.Menus.Remove(oldValideBorderoDepotFacture);
                context.SaveChanges();
            }
            //ajout de ValideBorderoDepotFacture ds assurance
            Menu existValideBorderoDepotFacture = context.Menus.Where(s => s.MenuCode == "ValideBorderoDepotFacture").FirstOrDefault();
            if (existValideBorderoDepotFacture == null)
            {
                Menu ValideBorderoDepotFacture = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "ValideBorderoDepotFacture",
                    MenuController = "ValideBorderoDepotFacture",
                    MenuDescription = "Menu Valide Bordero Depot Facture",
                    MenuFlat = false,
                    MenuIconName = "ValideBorderoDepotFacture",
                    MenuLabel = "ValideBorderoDepotFacture",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 2,
                    AppearanceOrder = 4
                };
                context.Menus.AddOrUpdate(ValideBorderoDepotFacture);
                context.SaveChanges();
            }

            //ajout du menu de param sur le module assur
            Menu existParamInsurance = context.Menus.Where(s => s.MenuCode == "ParamInsurance").FirstOrDefault();
            if (existParamInsurance == null)
            {
                Menu ParamInsurance = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "ParamInsurance",
                    MenuController = null,
                    MenuDescription = "Menu Param Insurance",
                    MenuFlat = false,
                    MenuIconName = "ParamInsurance",
                    MenuLabel = "ParamInsurance",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 2,
                    AppearanceOrder = 5
                };
                context.Menus.AddOrUpdate(ParamInsurance);
                context.SaveChanges();
                existParamInsurance = ParamInsurance;
            }

            if (existParamInsurance == null)
            {
                existParamInsurance = context.Menus.Where(s => s.MenuCode == "ParamInsurance").FirstOrDefault();
            }

            if (existParamInsurance != null)
            {
                //sub menu Lieux de depot
                SubMenu existLieuxDepotBill = context.SubMenus.Where(s => s.SubMenuCode == "LieuxDepotBill").FirstOrDefault();
                if (existLieuxDepotBill == null)
                {
                    SubMenu LieuxDepotBill = new SubMenu()
                    {
                        Menu = existParamInsurance,
                        MenuID = existParamInsurance.MenuID,
                        SubMenuCode = "LieuxDepotBill",
                        SubMenuDescription = "Lieux Depot Bill",
                        SubMenuLabel = "Lieux Depot Bill",
                        SubMenuController = "LieuxDepotBill",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(LieuxDepotBill);
                    context.SaveChanges();
                }

                //sub menu Insured Company
                SubMenu existInsuredCompany = context.SubMenus.Where(s => s.SubMenuCode == "InsuredCompany").FirstOrDefault();
                if (existInsuredCompany == null)
                {
                    SubMenu InsuredCompany = new SubMenu()
                    {
                        Menu = existParamInsurance,
                        MenuID = existParamInsurance.MenuID,
                        SubMenuCode = "InsuredCompany",
                        SubMenuDescription = "Insured Company",
                        SubMenuLabel = "Insured Company",
                        SubMenuController = "InsuredCompany",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(InsuredCompany);
                    context.SaveChanges();
                }

                //MENU DE CREATION DES ASSURANCES DS LE MODULE SALE
                Menu existMenuAssurance = context.Menus.Where(s => s.MenuCode == "MenuAssurance").FirstOrDefault();
                if (existMenuAssurance != null)
                {
                    //supression du menu
                    List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == existMenuAssurance.MenuID).ToList();
                    context.ActionMenuProfiles.RemoveRange(lstActionMP);
                    context.Menus.Remove(existMenuAssurance);
                    context.SaveChanges();
                }

                //sub menu assurance
                SubMenu existSubMenuAssurance = context.SubMenus.Where(s => s.SubMenuCode == "MenuAssurance").FirstOrDefault();
                if (existSubMenuAssurance == null)
                {
                    SubMenu SubMenuAssurance = new SubMenu()
                    {
                        Menu = existParamInsurance,
                        MenuID = existParamInsurance.MenuID,
                        SubMenuCode = "MenuAssurance",
                        SubMenuController = "Assurance",
                        SubMenuDescription = "Menu Assurance",
                        SubMenuLabel = "MenuAssurance",
                        SubMenuPath = "Index",

                    };
                    context.SubMenus.AddOrUpdate(SubMenuAssurance);
                    context.SaveChanges();
                }
            }



            //menu customer debtors
            Menu RptSaleMenu = context.Menus.Where(m => m.MenuCode == "SMnuSalRpt").FirstOrDefault();

            if (RptSaleMenu != null)
            {
                //suppression du menu RptValidatedBDF ds accounting
                SubMenu oldRptValidatedBDF = context.SubMenus.Where(s => s.SubMenuCode == "RptValidatedBDF" && s.MenuID != RptSaleMenu.MenuID).FirstOrDefault();
                if (oldRptValidatedBDF != null)
                {
                    //supression du submenu
                    List<ActionSubMenuProfile> lstActionMP = context.ActionSubMenuProfiles.Where(am => am.SubMenuID == oldRptValidatedBDF.SubMenuID).ToList();
                    context.ActionSubMenuProfiles.RemoveRange(lstActionMP);
                    context.SubMenus.Remove(oldRptValidatedBDF);
                    context.SaveChanges();
                }

                //creation du menu RptValidatedBDF ds assurance
                //list validated bordero de depot facture
                SubMenu exitRptValidatedBDF = context.SubMenus.Where(s => s.SubMenuCode == "RptValidatedBDF").FirstOrDefault();
                if (exitRptValidatedBDF == null)
                {
                    SubMenu RptValidatedBDF = new SubMenu()
                    {
                        Menu = RptSaleMenu,
                        MenuID = RptSaleMenu.MenuID,
                        SubMenuCode = "RptValidatedBDF",
                        SubMenuDescription = "RptValidatedBDF menu",
                        SubMenuLabel = "RptValidatedBDF",
                        SubMenuController = "RptValidatedBDF",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(RptValidatedBDF);
                }

                SubMenu existRptRegularizeBill = context.SubMenus.Where(s => s.SubMenuCode == "RptRegularizeBill").FirstOrDefault();
                if (existRptRegularizeBill == null)
                {
                    SubMenu RptRegularizeBill = new SubMenu()
                    {
                        Menu = RptSaleMenu,
                        MenuID = RptSaleMenu.MenuID,
                        SubMenuCode = "RptRegularizeBill",
                        SubMenuDescription = "RptRegularizeBill menu",
                        SubMenuLabel = "RptRegularizeBill",
                        SubMenuController = "RptRegularizeBill",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(RptRegularizeBill);
                    context.SaveChanges();
                }

                SubMenu existRptValideBill = context.SubMenus.Where(s => s.SubMenuCode == "RptValideBill").FirstOrDefault();
                if (existRptValideBill == null)
                {
                    SubMenu RptValideBill = new SubMenu()
                    {
                        Menu = RptSaleMenu,
                        MenuID = RptSaleMenu.MenuID,
                        SubMenuCode = "RptValideBill",
                        SubMenuDescription = "RptValideBill menu",
                        SubMenuLabel = "RptValideBill",
                        SubMenuController = "RptValideBill",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(RptValideBill);
                    context.SaveChanges();
                }

                SubMenu existRptProformaLst = context.SubMenus.Where(s => s.SubMenuCode == "RptProformaLst").FirstOrDefault();
                if (existRptProformaLst == null)
                {
                    SubMenu RptProformaLst = new SubMenu()
                    {
                        Menu = RptSaleMenu,
                        MenuID = RptSaleMenu.MenuID,
                        SubMenuCode = "RptProformaLst",
                        SubMenuDescription = "RptProformaLst menu",
                        SubMenuLabel = "RptProformaLst",
                        SubMenuController = "RptProformaLst",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(RptProformaLst);
                    context.SaveChanges();
                }

                SubMenu existInsuredReport = context.SubMenus.Where(s => s.SubMenuCode == "InsuredReport").FirstOrDefault();
                if (existInsuredReport == null)
                {
                    SubMenu InsuredReport = new SubMenu()
                    {
                        Menu = RptSaleMenu,
                        MenuID = RptSaleMenu.MenuID,
                        SubMenuCode = "InsuredReport",
                        SubMenuDescription = "InsuredReport menu",
                        SubMenuLabel = "InsuredReport",
                        SubMenuController = "InsuredReport",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(InsuredReport);
                    context.SaveChanges();
                }

                SubMenu existInsuredReportSeller = context.SubMenus.Where(s => s.SubMenuCode == "InsuredReportSeller").FirstOrDefault();
                if (existInsuredReportSeller == null)
                {
                    SubMenu InsuredReportSeller = new SubMenu()
                    {
                        Menu = RptSaleMenu,
                        MenuID = RptSaleMenu.MenuID,
                        SubMenuCode = "InsuredReportSeller",
                        SubMenuDescription = "InsuredReportSeller menu",
                        SubMenuLabel = "InsuredReportSeller",
                        SubMenuController = "InsuredReportSeller",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(InsuredReportSeller);
                    context.SaveChanges();
                }
                SubMenu existRptDeletedBill = context.SubMenus.Where(s => s.SubMenuCode == "RptDeletedBill").FirstOrDefault();
                if (existRptDeletedBill == null)
                {
                    SubMenu RptDeletedBill = new SubMenu()
                    {
                        Menu = RptSaleMenu,
                        MenuID = RptSaleMenu.MenuID,
                        SubMenuCode = "RptDeletedBill",
                        SubMenuDescription = "RptDeletedBill menu",
                        SubMenuLabel = "RptDeletedBill",
                        SubMenuController = "RptDeletedBill",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(RptDeletedBill);
                    context.SaveChanges();
                }

            }

            //LIST OF BUDGET EXPENSES
            Menu RptAccountingMenu = context.Menus.Where(m => m.MenuCode == "mnuAcct7").FirstOrDefault();
            if (RptAccountingMenu != null)
            {
                SubMenu exitRptBudgetExpense = context.SubMenus.Where(s => s.SubMenuCode == "RptbudgetExpense").FirstOrDefault();
                if (exitRptBudgetExpense == null)
                {
                    SubMenu RptbudgetExpense = new SubMenu()
                    {
                        Menu = RptAccountingMenu,
                        MenuID = RptAccountingMenu.MenuID,
                        SubMenuCode = "RptbudgetExpense",
                        SubMenuDescription = "RptbudgetExpense menu",
                        SubMenuLabel = "RptbudgetExpense",
                        SubMenuController = "RptbudgetExpense",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(RptbudgetExpense);
                }

                //list of return sales
                SubMenu exitRptLstReturnSales = context.SubMenus.Where(s => s.SubMenuCode == "RptLstReturnSales").FirstOrDefault();
                if (exitRptLstReturnSales == null)
                {
                    SubMenu RptLstReturnSales = new SubMenu()
                    {
                        Menu = RptAccountingMenu,
                        MenuID = RptAccountingMenu.MenuID,
                        SubMenuCode = "RptLstReturnSales",
                        SubMenuDescription = "RptLstReturnSales menu",
                        SubMenuLabel = "RptLstReturnSales",
                        SubMenuController = "RptLstReturnSales",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(RptLstReturnSales);
                }
            }

            //DELETE CUSTOMER HISTORY FRONT
            SubMenu exitCustomerHistoryFront = context.SubMenus.Where(s => s.SubMenuCode == "CustomerHistoryFront").FirstOrDefault();
            if (exitCustomerHistoryFront != null)
            {
                //supression du menu
                List<ActionSubMenuProfile> lstActionMP = context.ActionSubMenuProfiles.Where(am => am.SubMenuID == exitCustomerHistoryFront.SubMenuID).ToList();
                context.ActionSubMenuProfiles.RemoveRange(lstActionMP);
                context.SubMenus.Remove(exitCustomerHistoryFront);
                context.SaveChanges();
            }



            //depot
            Menu RptDepot = context.Menus.Where(m => m.MenuCode == "d_Deposit").FirstOrDefault();
            SubMenu exitDepotStockLenses = context.SubMenus.Where(s => s.SubMenuCode == "DepotStockLenses").FirstOrDefault();
            if (exitDepotStockLenses == null)
            {
                SubMenu DepotStockLenses = new SubMenu()
                {
                    Menu = RptDepot,
                    MenuID = RptDepot.MenuID,
                    SubMenuCode = "DepotStockLenses", //sera use pr les depots des non assure
                    SubMenuDescription = "DepotStockLenses menu",
                    SubMenuLabel = "DepotStockLenses",
                    SubMenuController = "Deposit",
                    SubMenuPath = "Deposit"
                };
                context.SubMenus.AddOrUpdate(DepotStockLenses);
            }
            //Ajout du sous menu deposit to customer
            SubMenu exitDepotToCustomer = context.SubMenus.Where(s => s.SubMenuCode == "DepotToCustomer").FirstOrDefault();
            if (exitDepotToCustomer == null)
            {
                SubMenu DepotToCustomer = new SubMenu()
                {
                    Menu = RptDepot,
                    MenuID = RptDepot.MenuID,
                    SubMenuCode = "DepotToCustomer",
                    SubMenuDescription = "DepotToCustomer menu",
                    SubMenuLabel = "DepotToCustomer",
                    SubMenuController = "DepotToCustomer",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(DepotToCustomer);
            }
            SubMenu exitDepotInsured = context.SubMenus.Where(s => s.SubMenuCode == "DepotInsured").FirstOrDefault();
            if (exitDepotInsured == null)
            {
                SubMenu DepotInsured = new SubMenu()
                {
                    Menu = RptDepot,
                    MenuID = RptDepot.MenuID,
                    SubMenuCode = "DepotInsured", //sera use pr les depots des non assure
                    SubMenuDescription = "Depot Insured menu",
                    SubMenuLabel = "DepotInsured",
                    SubMenuController = "DepotInsured",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(DepotInsured);
            }



            //teller ForceOpenTeller
            Menu existForceOpenTeller = context.Menus.Where(s => s.MenuCode == "ForceOpenTeller").FirstOrDefault();
            if (existForceOpenTeller == null)
            {
                Menu ForceOpenTeller = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "ForceOpenTeller",
                    MenuController = "ForceOpenTeller",
                    MenuDescription = "Force Opening Teller",
                    MenuFlat = false,
                    MenuIconName = "ForceOpenTeller",
                    MenuLabel = "ForceOpenTeller",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(ForceOpenTeller);
                context.SaveChanges();
            }


            //ajout des menu stock damage et change product number
            Menu d_Stock = context.Menus.Where(m => m.MenuCode == "d_Stock").FirstOrDefault();
            SubMenu existStockDamage = context.SubMenus.Where(s => s.SubMenuCode == "StockDamage").FirstOrDefault();
            if (existStockDamage == null)
            {
                SubMenu StockDamage = new SubMenu()
                {
                    Menu = d_Stock,
                    MenuID = d_Stock.MenuID,
                    SubMenuCode = "StockDamage",
                    SubMenuDescription = "Stock Damage menu",
                    SubMenuLabel = "Stock Damage",
                    SubMenuController = "StockDamage",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(StockDamage);
            }
            //change product number
            SubMenu existRegProductNumber = context.SubMenus.Where(s => s.SubMenuCode == "RegProductNumber").FirstOrDefault();
            if (existRegProductNumber == null)
            {
                SubMenu RegProductNumber = new SubMenu()
                {
                    Menu = d_Stock,
                    MenuID = d_Stock.MenuID,
                    SubMenuCode = "RegProductNumber",
                    SubMenuDescription = "Reg. Product Number",
                    SubMenuLabel = "Reg. Product Number",
                    SubMenuController = "RegProductNumber",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(RegProductNumber);
            }
            SubMenu existProductGift = context.SubMenus.Where(s => s.SubMenuCode == "ProductGift").FirstOrDefault();
            if (existProductGift == null)
            {
                SubMenu ProductGift = new SubMenu()
                {
                    Menu = d_Stock,
                    MenuID = d_Stock.MenuID,
                    SubMenuCode = "ProductGift",
                    SubMenuDescription = "Product Gift menu",
                    SubMenuLabel = "Product Gift",
                    SubMenuController = "ProductGift",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(ProductGift);
            }
            //Print or remove deposit Amount
            SubMenu existPrintDeposit = context.SubMenus.Where(s => s.SubMenuCode == "PrintDeposit").FirstOrDefault();
            if (existPrintDeposit == null)
            {
                SubMenu PrintDeposit = new SubMenu()
                {
                    Menu = RptCashMenu,
                    MenuID = RptCashMenu.MenuID,
                    SubMenuCode = "PrintDeposit",
                    SubMenuDescription = "Print Deposit",
                    SubMenuLabel = "Print Deposit",
                    SubMenuController = "PrintDeposit",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(PrintDeposit);
            }
            //stock reserve
            SubMenu existStockNonInsureReserve = context.SubMenus.Where(s => s.SubMenuCode == "StockNonInsureReserve").FirstOrDefault();
            if (existStockNonInsureReserve == null)
            {
                SubMenu StockNonInsureReserve = new SubMenu()
                {
                    Menu = d_Stock,
                    MenuID = d_Stock.MenuID,
                    SubMenuCode = "StockNonInsureReserve",
                    SubMenuDescription = "Stock Stock Non Insured Reserve menu",
                    SubMenuLabel = "Stock Stock Non Insured Reserve",
                    SubMenuController = "StockNonInsureReserve",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(StockNonInsureReserve);
                context.SaveChanges();
            }
            //stock output
            SubMenu existStockOutput = context.SubMenus.Where(s => s.SubMenuCode == "StockOutput").FirstOrDefault();
            if (existStockOutput == null)
            {
                SubMenu StockOutput = new SubMenu()
                {
                    Menu = d_Stock,
                    MenuID = d_Stock.MenuID,
                    SubMenuCode = "StockOutput",
                    SubMenuDescription = "Stock Output menu",
                    SubMenuLabel = "Stock Output",
                    SubMenuController = "StockOutput",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(StockOutput);
                context.SaveChanges();
            }
            //Replacement
            SubMenu existStockReplacement = context.SubMenus.Where(s => s.SubMenuCode == "StockReplacement").FirstOrDefault();
            if (existStockReplacement == null)
            {
                SubMenu StockReplacement = new SubMenu()
                {
                    Menu = d_Stock,
                    MenuID = d_Stock.MenuID,
                    SubMenuCode = "StockReplacement",
                    SubMenuDescription = "Stock Replacement menu",
                    SubMenuLabel = "Stock Replacement",
                    SubMenuController = "StockReplacement",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(StockReplacement);
                context.SaveChanges();
            }
            //menu de mise a jour seuil de securite
            SubMenu existInventoryAddedQty = context.SubMenus.Where(s => s.SubMenuCode == "InventoryAddedQty").FirstOrDefault();
            if (existInventoryAddedQty == null)
            {
                SubMenu InventoryAddedQty = new SubMenu()
                {
                    Menu = d_Stock,
                    MenuID = d_Stock.MenuID,
                    SubMenuCode = "InventoryAddedQty",
                    SubMenuDescription = "Inventory Added Qty menu",
                    SubMenuLabel = "InventoryAddedQty",
                    SubMenuController = "InventoryAddedQty",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(InventoryAddedQty);
                context.SaveChanges();
            }



            SubMenu exitDisplayFacture = context.SubMenus.Where(s => s.SubMenuCode == "DisplayFacture").FirstOrDefault();
            if (exitDisplayFacture == null)
            {
                SubMenu DisplayFacture = new SubMenu()
                {
                    Menu = RptCashMenu,
                    MenuID = RptCashMenu.MenuID,
                    SubMenuCode = "DisplayFacture",
                    SubMenuDescription = "DisplayFacture menu",
                    SubMenuLabel = "DisplayFacture",
                    SubMenuController = "State",
                    SubMenuPath = "DisplayFacture"
                };
                context.SubMenus.AddOrUpdate(DisplayFacture);
            }

            //creation du menu report ds supply
            Menu existMenuSupplyRpt = context.Menus.Where(s => s.MenuCode == "MenuSupplyReport").FirstOrDefault();
            if (existMenuSupplyRpt == null)
            {
                Menu MenuSupplyRpt = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "MenuSupplyReport",
                    MenuController = null,
                    MenuDescription = "Menu Supply Report",
                    MenuFlat = false,
                    MenuIconName = "MenuSupplyReport",
                    MenuLabel = "MenuSupplyReport",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 5
                };
                context.Menus.AddOrUpdate(MenuSupplyRpt);
                context.SaveChanges();
                existMenuSupplyRpt = MenuSupplyRpt;
            }
            //Print Product Sale History
            SubMenu existProductSaleHistory = context.SubMenus.Where(s => s.SubMenuCode == "ProductSaleHistory").FirstOrDefault();
            if (existProductSaleHistory == null)
            {
                SubMenu ProductSaleHistory = new SubMenu()
                {
                    Menu = existMenuSupplyRpt,
                    MenuID = existMenuSupplyRpt.MenuID,
                    SubMenuCode = "ProductSaleHistory",
                    SubMenuDescription = "Print Product Sale History",
                    SubMenuLabel = "Print Product Sale History",
                    SubMenuController = "ProductSaleHistory",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(ProductSaleHistory);
            }
            //Print Stock Input
            SubMenu existStockInput = context.SubMenus.Where(s => s.SubMenuCode == "StockInput").FirstOrDefault();
            if (existStockInput == null)
            {
                SubMenu StockInput = new SubMenu()
                {
                    Menu = existMenuSupplyRpt,
                    MenuID = existMenuSupplyRpt.MenuID,
                    SubMenuCode = "StockInput",
                    SubMenuDescription = "Print Stock Input",
                    SubMenuLabel = "Print Stock Input",
                    SubMenuController = "StockInput",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(StockInput);
            }
            //Print Stock Mvt
            SubMenu existStockMouvement = context.SubMenus.Where(s => s.SubMenuCode == "StockMouvement").FirstOrDefault();
            if (existStockMouvement == null)
            {
                SubMenu StockMouvement = new SubMenu()
                {
                    Menu = existMenuSupplyRpt,
                    MenuID = existMenuSupplyRpt.MenuID,
                    SubMenuCode = "StockMouvement",
                    SubMenuDescription = "Print Stock Mouvement",
                    SubMenuLabel = "Print Stock Mouvement",
                    SubMenuController = "StockMouvement",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(StockMouvement);
                context.SaveChanges();
            }
            //Print List of gift
            SubMenu existListOfGift = context.SubMenus.Where(s => s.SubMenuCode == "ListOfGift").FirstOrDefault();
            if (existListOfGift == null)
            {
                SubMenu ListOfGift = new SubMenu()
                {
                    Menu = existMenuSupplyRpt,
                    MenuID = existMenuSupplyRpt.MenuID,
                    SubMenuCode = "ListOfGift",
                    SubMenuDescription = "Print List Of Gift",
                    SubMenuLabel = "Print List Of Gift",
                    SubMenuController = "ListOfGift",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(ListOfGift);
                context.SaveChanges();
            }
            //Print Lift of Damage
            SubMenu existListOfDammage = context.SubMenus.Where(s => s.SubMenuCode == "ListOfDammage").FirstOrDefault();
            if (existListOfDammage == null)
            {
                SubMenu ListOfDammage = new SubMenu()
                {
                    Menu = existMenuSupplyRpt,
                    MenuID = existMenuSupplyRpt.MenuID,
                    SubMenuCode = "ListOfDammage",
                    SubMenuDescription = "Print List Of Dammage",
                    SubMenuLabel = "Print List Of Dammage",
                    SubMenuController = "ListOfDammage",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(ListOfDammage);
                context.SaveChanges();
            }
            //Print List of Replacement
            SubMenu existListOfReplacement = context.SubMenus.Where(s => s.SubMenuCode == "ListOfReplacement").FirstOrDefault();
            if (existListOfReplacement == null)
            {
                SubMenu ListOfReplacement = new SubMenu()
                {
                    Menu = existMenuSupplyRpt,
                    MenuID = existMenuSupplyRpt.MenuID,
                    SubMenuCode = "ListOfReplacement",
                    SubMenuDescription = "Print List Of Replacement",
                    SubMenuLabel = "Print List Of Replacement",
                    SubMenuController = "ListOfReplacement",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(ListOfReplacement);
                context.SaveChanges();
            }
            //Print Lift of stock input
            SubMenu existListOfStockInput = context.SubMenus.Where(s => s.SubMenuCode == "ListOfStockInput").FirstOrDefault();
            if (existListOfStockInput == null)
            {
                SubMenu ListOfStockInput = new SubMenu()
                {
                    Menu = existMenuSupplyRpt,
                    MenuID = existMenuSupplyRpt.MenuID,
                    SubMenuCode = "ListOfStockInput",
                    SubMenuDescription = "Print List Of Stock Input",
                    SubMenuLabel = "Print List Of Stock Input",
                    SubMenuController = "ListOfStockInput",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(ListOfStockInput);
                context.SaveChanges();
            }
            //Generate Bare Code
            Menu RptProduct = context.Menus.Where(m => m.MenuCode == "b_Product").FirstOrDefault();
            SubMenu existProductBrand = context.SubMenus.Where(s => s.SubMenuCode == "ProductBrand").FirstOrDefault();
            if (existProductBrand == null)
            {
                SubMenu ProductBrand = new SubMenu()
                {
                    Menu = RptProduct,
                    MenuID = RptProduct.MenuID,
                    SubMenuCode = "ProductBrand",
                    SubMenuDescription = "Product Brand",
                    SubMenuLabel = "Product Brand",
                    SubMenuController = "ProductBrand",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(ProductBrand);
                context.SaveChanges();
            }

            //supression du menu de vente
            Menu M_SAL = context.Menus.Where(m => m.MenuCode == "SAL").FirstOrDefault();
            if (M_SAL != null)
            {
                List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == M_SAL.MenuID).ToList();
                context.ActionMenuProfiles.RemoveRange(lstActionMP);
                context.Menus.Remove(M_SAL);
                context.SaveChanges();
            }

            //supression du menu NEWSALASSUR
            Menu existNEWSALASSUR = context.Menus.Where(s => s.MenuCode == "NEWSALASSUR").FirstOrDefault();
            if (existNEWSALASSUR != null)
            {
                List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == existNEWSALASSUR.MenuID).ToList();
                context.ActionMenuProfiles.RemoveRange(lstActionMP);
                context.Menus.Remove(existNEWSALASSUR);
                context.SaveChanges();
            }
            //supression du sous menu DepotSpecialOrder
            SubMenu exitDepotSpecialOrder = context.SubMenus.Where(s => s.SubMenuCode == "DepotSpecialOrder").FirstOrDefault();
            if (exitDepotSpecialOrder != null)
            {
                List<ActionSubMenuProfile> lstActionSubMP = context.ActionSubMenuProfiles.Where(am => am.SubMenuID == exitDepotSpecialOrder.SubMenuID).ToList();
                context.ActionSubMenuProfiles.RemoveRange(lstActionSubMP);
                context.SubMenus.Remove(exitDepotSpecialOrder);
                context.SaveChanges();
            }

            //Ajout du menu sale to customer ds le module teller
            Menu existSaleToCustomer = context.Menus.Where(s => s.MenuCode == "SaleToCustomer").FirstOrDefault();
            if (existSaleToCustomer != null)
            {
                DeleteMenu(context, existSaleToCustomer);
            }
            //Ajout du menu Othersale ds le module teller
            Menu existOtherSale = context.Menus.Where(s => s.MenuCode == "OtherSale").FirstOrDefault();
            if (existOtherSale == null)
            {
                Menu OtherSale = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "OtherSale",
                    MenuController = "OtherSale",
                    MenuDescription = "Menu Other Sale",
                    MenuFlat = false,
                    MenuIconName = "OtherSale",
                    MenuLabel = "OtherSale",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(OtherSale);
                context.SaveChanges();
            }
            //suppression des menu open/close teller
            Menu existA_OPEN_CODE_CASH = context.Menus.Where(s => s.MenuCode == "A_OPEN_CODE_CASH").FirstOrDefault();
            if (existA_OPEN_CODE_CASH == null)
            {
                Menu A_OPEN_CODE_CASH = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "A_OPEN_CODE_CASH",
                    MenuController = "CashRegister",
                    MenuDescription = "Menu Open Cash",
                    MenuFlat = false,
                    MenuIconName = "A_OPEN_CODE_CASH",
                    MenuLabel = "A_OPEN_CODE_CASH",
                    MenuPath = "Open",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(A_OPEN_CODE_CASH);
                context.SaveChanges();
                ////suppression des actionsubmenuprofile
                //List<ActionMenuProfile> amnu = context.ActionMenuProfiles.Where(a => a.MenuID == existA_OPEN_CODE_CASH.MenuID).ToList();
                //context.ActionMenuProfiles.RemoveRange(amnu);
                //context.SaveChanges();

                //context.Menus.Remove(existA_OPEN_CODE_CASH);
                //context.SaveChanges();
            }
            //suppression des menu open/close teller
            Menu existY_CLOSE_COD_CASH = context.Menus.Where(s => s.MenuCode == "Y_CLOSE_COD_CASH").FirstOrDefault();
            if (existY_CLOSE_COD_CASH == null)
            {
                Menu Y_CLOSE_COD_CASH = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "Y_CLOSE_COD_CASH",
                    MenuController = "CashRegister",
                    MenuDescription = "Menu Close Cash",
                    MenuFlat = false,
                    MenuIconName = "Y_CLOSE_COD_CASH",
                    MenuLabel = "Y_CLOSE_COD_CASH",
                    MenuPath = "Close",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(Y_CLOSE_COD_CASH);
                context.SaveChanges();
                ////suppression des actionsubmenuprofile
                //List<ActionMenuProfile> amnu = context.ActionMenuProfiles.Where(a => a.MenuID == existY_CLOSE_COD_CASH.MenuID).ToList();
                //context.ActionMenuProfiles.RemoveRange(amnu);
                //context.SaveChanges();

                //context.Menus.Remove(existY_CLOSE_COD_CASH);
                //context.SaveChanges();
            }

            //ajout du module crm
            Module existCRM = context.Modules.Where(s => s.ModuleCode == "CRM").FirstOrDefault();
            if (existCRM == null)
            {
                Module ModuleCRM = new Module()
                {
                    ModuleArea = "CRM",
                    ModuleCode = "CRM",
                    ModuleDescription = "Module de CRM",
                    ModuleLabel = "Module de CRM",
                    ModuleState = true,
                };
                context.Modules.Add(ModuleCRM);
                context.SaveChanges();
            }
            //SUPRESSION DU MENU CLT
            Menu existCLT = context.Menus.Where(s => s.MenuCode == "CLT").FirstOrDefault();
            if (existCLT != null)
            {
                //suppression des actionsubmenuprofile
                List<ActionMenuProfile> amnu = context.ActionMenuProfiles.Where(a => a.MenuID == existCLT.MenuID).ToList();
                context.ActionMenuProfiles.RemoveRange(amnu);
                context.SaveChanges();

                context.Menus.Remove(existCLT);
                context.SaveChanges();
            }


            //CREATION DU MENU CLT DS LE NEW MODULE
            Menu existMnuCLT = context.Menus.Where(s => s.MenuCode == "NEWCLT").FirstOrDefault();
            if (existMnuCLT == null)
            {
                Menu MnuCLT = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "NEWCLT",
                    MenuController = "Customer",
                    MenuDescription = "Menu Customer",
                    MenuFlat = false,
                    MenuIconName = "Customer",
                    MenuLabel = "Customer",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 7
                };
                context.Menus.AddOrUpdate(MnuCLT);
                context.SaveChanges();
            }



            ////suppression des menus de delivery desk
            ////-delevery
            //Menu existDeliverDesk = context.Menus.Where(s => s.MenuCode == "DeliverDesk").FirstOrDefault();
            //if (existDeliverDesk != null)
            //{
            //    List<ActionMenuProfile> existActionMenuProfileDeliverDesk = context.ActionMenuProfiles.Where(s => s.MenuID == existDeliverDesk.MenuID).ToList();
            //    if (existActionMenuProfileDeliverDesk.Count > 0)
            //    {
            //        context.ActionMenuProfiles.RemoveRange(existActionMenuProfileDeliverDesk);
            //        context.SaveChanges();
            //    }

            //    context.Menus.Remove(existDeliverDesk);
            //    context.SaveChanges();
            //}



           

            //- Lens Mounting
            Menu existLensMounting = context.Menus.Where(s => s.MenuCode == "LensMounting").FirstOrDefault();
            if (existLensMounting != null)
            {
                List<ActionMenuProfile> existActionMenuProfileLensMounting = context.ActionMenuProfiles.Where(s => s.MenuID == existLensMounting.MenuID).ToList();
                if (existActionMenuProfileLensMounting.Count > 0)
                {
                    context.ActionMenuProfiles.RemoveRange(existActionMenuProfileLensMounting);
                    context.SaveChanges();
                }

                context.Menus.Remove(existLensMounting);
                context.SaveChanges();
            }


            //-Prescription Harmonisation
            Menu existPrescriptionHarmonisation = context.Menus.Where(s => s.MenuCode == "PrescriptionHarmonisation").FirstOrDefault();
            if (existPrescriptionHarmonisation != null)
            {
                List<ActionMenuProfile> existActionMenuProfilePrescriptionHarmonisation = context.ActionMenuProfiles.Where(s => s.MenuID == existPrescriptionHarmonisation.MenuID).ToList();
                if (existActionMenuProfilePrescriptionHarmonisation.Count > 0)
                {
                    context.ActionMenuProfiles.RemoveRange(existActionMenuProfilePrescriptionHarmonisation);
                    context.SaveChanges();
                }
                context.Menus.Remove(existPrescriptionHarmonisation);
                context.SaveChanges();
            }


            //CREATION DU MENU RDV DS LE NEW MODULE
            Menu existMnuRDV = context.Menus.Where(s => s.MenuCode == "RDV").FirstOrDefault();
            if (existMnuRDV == null)
            {
                Menu MnuRDV = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "RDV",
                    MenuController = "RendezVous",
                    MenuDescription = "Menu RendezVous",
                    MenuFlat = false,
                    MenuIconName = "RendezVous",
                    MenuLabel = "RendezVous",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 7
                };
                context.Menus.AddOrUpdate(MnuRDV);
                context.SaveChanges();
            }

            //CREATION DU MENU ALERTE DS LE NEW MODULE
            Menu existMnuALERTESMS = context.Menus.Where(s => s.MenuCode == "ALERTESMS").FirstOrDefault();
            if (existMnuALERTESMS == null)
            {
                Menu MnuALERTESMS = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "ALERTESMS",
                    MenuController = "ALERTESMS",
                    MenuDescription = "Menu ALERTESMS",
                    MenuFlat = false,
                    MenuIconName = "ALERTESMS",
                    MenuLabel = "ALERTESMS",
                    MenuPath = "",
                    MenuState = true,
                    ModuleID = 7
                };
                context.Menus.AddOrUpdate(MnuALERTESMS);
                context.SaveChanges();
            }

            //suppression DU MENU customer journey
            Menu existMnuCustomerJourney = context.Menus.Where(s => s.MenuCode == "CustomerJourney").FirstOrDefault();
            if (existMnuCustomerJourney != null)
            {
                DeleteMenu(context, existMnuCustomerJourney);
            }

            //CREATION DU MENU customer journey Back
            Menu existMnuCustomerJourneyBack = context.Menus.Where(s => s.MenuCode == "CustomerJourneyBack").FirstOrDefault();
            if (existMnuCustomerJourneyBack == null)
            {
                Menu MnuCustomerJourneyBack = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "CustomerJourneyBack",
                    MenuController = "CustomerJourneyBack",
                    MenuDescription = "Menu Customer Journey Back",
                    MenuFlat = false,
                    MenuIconName = "CustomerJourneyBack",
                    MenuLabel = "CustomerJourneyBack",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 7
                };
                context.Menus.AddOrUpdate(MnuCustomerJourneyBack);
                context.SaveChanges();
            }

            //CREATION DU MENU customer journey
            Menu existMnuCustomerJourneyFront = context.Menus.Where(s => s.MenuCode == "CustomerJourneyFront").FirstOrDefault();
            if (existMnuCustomerJourneyFront == null)
            {
                Menu MnuCustomerJourneyFront = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "CustomerJourneyFront",
                    MenuController = "CustomerJourneyFront",
                    MenuDescription = "Menu Customer Journey",
                    MenuFlat = false,
                    MenuIconName = "CustomerJourneyFront",
                    MenuLabel = "CustomerJourneyFront",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 7
                };
                context.Menus.AddOrUpdate(MnuCustomerJourneyFront);
                context.SaveChanges();
            }

            //creation du menu de consultation
            Menu MnuConsultation = context.Menus.Where(s => s.MenuCode == "MnuConsultation").FirstOrDefault();
            if (MnuConsultation == null)
            {
                MnuConsultation = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "MnuConsultation",
                    MenuController = "MnuConsultation",
                    MenuDescription = "Menu Consultation",
                    MenuFlat = false,
                    MenuIconName = "MnuConsultation",
                    MenuLabel = "MnuConsultation",
                    MenuPath = "",
                    MenuState = true,
                    ModuleID = 7,
                    AppearanceOrder = 5
                };
                context.Menus.AddOrUpdate(MnuConsultation);
                context.SaveChanges();
            }

            if (MnuConsultation != null)
            {
                //suppression de l'ancien menu consultation a la racine du module client non assuree
                Menu oldConsultation = context.Menus.Where(s => s.MenuCode == "Consultation").FirstOrDefault();
                if (oldConsultation != null)
                {
                    //supression du submenu
                    List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == oldConsultation.MenuID).ToList();
                    context.ActionMenuProfiles.RemoveRange(lstActionMP);
                    context.Menus.Remove(oldConsultation);
                    context.SaveChanges();
                }

                //creation du sous menu de consultation
                SubMenu existConsultation = context.SubMenus.Where(s => s.SubMenuCode == "Consultation").FirstOrDefault();
                if (existConsultation == null)
                {
                    SubMenu Consultation = new SubMenu()
                    {
                        IsChortcut = false,
                        SubMenuCode = "Consultation",
                        SubMenuController = "Consultation",
                        SubMenuDescription = "Menu Consultation",
                        SubMenuLabel = "Consultation",
                        SubMenuPath = "Index",
                        MenuID = MnuConsultation.MenuID,
                        AppearanceOrder = 0
                    };
                    context.SubMenus.AddOrUpdate(Consultation);
                    context.SaveChanges();
                }

                //suppression de l'ancien menu consultation a la racine du module client non assuree
                Menu oldConsultRDV = context.Menus.Where(s => s.MenuCode == "ConsultRDV").FirstOrDefault();
                if (oldConsultRDV != null)
                {
                    //supression du submenu
                    List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == oldConsultRDV.MenuID).ToList();
                    context.ActionMenuProfiles.RemoveRange(lstActionMP);
                    context.Menus.Remove(oldConsultRDV);
                    context.SaveChanges();
                }

                //creation du sous menu de ConsultRDV
                SubMenu existConsultRDV = context.SubMenus.Where(s => s.SubMenuCode == "ConsultRDV").FirstOrDefault();
                if (existConsultRDV == null)
                {
                    SubMenu MnuConsultRDV = new SubMenu()
                    {
                        IsChortcut = false,
                        SubMenuCode = "ConsultRDV",
                        SubMenuController = "ConsultRDV",
                        SubMenuDescription = "Menu ConsultRDV",
                        SubMenuLabel = "ConsultRDV",
                        SubMenuPath = "Index",
                        MenuID = MnuConsultation.MenuID,
                        AppearanceOrder = 1
                    };
                    context.SubMenus.AddOrUpdate(MnuConsultRDV);
                    context.SaveChanges();
                }

            }



            //ajout du menu report dans client
            Menu existCRMReport = context.Menus.Where(s => s.MenuCode == "CRMReport").FirstOrDefault();
            if (existCRMReport == null)
            {
                Menu CRMReport = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "CRMReport",
                    MenuController = "CRMReport",
                    MenuDescription = "Menu CRMReport",
                    MenuFlat = false,
                    MenuIconName = "CRMReport",
                    MenuLabel = "CRMReport",
                    MenuPath = "",
                    MenuState = true,
                    ModuleID = 7
                };
                context.Menus.AddOrUpdate(CRMReport);
                context.SaveChanges();
            }

            Menu NewCRMReport = context.Menus.Where(m => m.MenuCode == "CRMReport").FirstOrDefault();
            if (NewCRMReport != null)
            {

                

                SubMenu existSalesReport = context.SubMenus.Where(s => s.SubMenuCode == "SalesReport").FirstOrDefault();
                if (existSalesReport == null)
                {
                    SubMenu SalesReport = new SubMenu()
                    {
                        Menu = NewCRMReport,
                        MenuID = NewCRMReport.MenuID,
                        SubMenuCode = "SalesReport",
                        SubMenuDescription = "SalesReport",
                        SubMenuLabel = "SalesReport",
                        SubMenuController = "SalesReport",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(SalesReport);
                    context.SaveChanges();
                }


                SubMenu existSalesReportSeller = context.SubMenus.Where(s => s.SubMenuCode == "SalesReportSeller").FirstOrDefault();
                if (existSalesReportSeller == null)
                {
                    SubMenu SalesReportSeller = new SubMenu()
                    {
                        Menu = NewCRMReport,
                        MenuID = NewCRMReport.MenuID,
                        SubMenuCode = "SalesReportSeller",
                        SubMenuDescription = "SalesReportSeller",
                        SubMenuLabel = "SalesReportSeller",
                        SubMenuController = "SalesReportSeller",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(SalesReportSeller);
                    context.SaveChanges();
                }

                SubMenu existConsultationHistoryReport = context.SubMenus.Where(s => s.SubMenuCode == "RptConsultationHistory").FirstOrDefault();
                if (existConsultationHistoryReport == null)
                {
                    SubMenu consultationHistoryReport = new SubMenu()
                    {
                        Menu = NewCRMReport,
                        MenuID = NewCRMReport.MenuID,
                        SubMenuCode = "RptConsultationHistory",
                        SubMenuDescription = "RptConsultationHistory",
                        SubMenuLabel = "RptConsultationHistory",
                        SubMenuController = "RptConsultationHistory",
                        SubMenuPath = "Index"
                    };
                    context.SubMenus.AddOrUpdate(consultationHistoryReport);
                    context.SaveChanges();
                }

                
            }

            //remove RappelGeneralSMS
            SubMenu existRappelGeneralSMS = context.SubMenus.Where(s => s.SubMenuCode == "RappelGeneralSMS").FirstOrDefault();
            if (existRappelGeneralSMS != null)
            {
                DeleteSubMenu(context, existRappelGeneralSMS);
            }
            SubMenu existRappelRdvSMS = context.SubMenus.Where(s => s.SubMenuCode == "RappelRdvSMS").FirstOrDefault();
            if (existRappelRdvSMS != null)
            {
                DeleteSubMenu(context, existRappelRdvSMS);
            }
            SubMenu existEvenementSMS = context.SubMenus.Where(s => s.SubMenuCode == "EvenementSMS" && s.Menu.MenuCode != "NotificationSMS").FirstOrDefault();
            if (existEvenementSMS != null)
            {
                DeleteSubMenu(context, existEvenementSMS);
            }
            Menu ALERTESMS = context.Menus.Where(s => s.MenuCode == "ALERTESMS").FirstOrDefault();
            if (ALERTESMS != null)
            {
                DeleteMenu(context, ALERTESMS);
            }


            //CREATION DU MENU Saisi des retours sur vente
            Menu existSaleReturn = context.Menus.Where(s => s.MenuCode == "RTN").FirstOrDefault();
            if (existSaleReturn == null)
            {
                Menu SaleReturn = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "RTN",
                    MenuController = "SaleReturn",
                    MenuDescription = "Menu Return Sale",
                    MenuFlat = false,
                    MenuIconName = "SaleReturn",
                    MenuLabel = "SaleReturn",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 4
                };
                context.Menus.AddOrUpdate(SaleReturn);
                context.SaveChanges();
            }

            //Ajout du menu authorise sale ds le module teller
            Menu existAuthorizeSale = context.Menus.Where(s => s.MenuCode == "AuthorizeSale").FirstOrDefault();
            if (existAuthorizeSale == null)
            {
                Menu AuthorizeSale = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "AuthorizeSale",
                    MenuController = "AuthorizeSale",
                    MenuDescription = "Menu Authorize Sale",
                    MenuFlat = false,
                    MenuIconName = "AuthorizeSale",
                    MenuLabel = "AuthorizeSale",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(AuthorizeSale);
                context.SaveChanges();
            }

            //Ajout du menu authorise sale after prescription ds le module teller
            Menu existAuthorizePrescriptionSale = context.Menus.Where(s => s.MenuCode == "AuthorizePrescriptionSale").FirstOrDefault();
            if (existAuthorizePrescriptionSale == null)
            {
                Menu AuthorizePrescriptionSale = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "AuthorizePrescriptionSale",
                    MenuController = "AuthorizePrescriptionSale",
                    MenuDescription = "Menu Authorize Sale Prescription",
                    MenuFlat = false,
                    MenuIconName = "AuthorizePrescriptionSale",
                    MenuLabel = "AuthorizePrescriptionSale",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(AuthorizePrescriptionSale);
                context.SaveChanges();
            }

            //Ajout du menu authorise sale ds le module teller
            Menu existValidatedSale = context.Menus.Where(s => s.MenuCode == "ValidatedSale").FirstOrDefault();
            if (existValidatedSale == null)
            {
                Menu ValidatedSale = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "ValidatedSale",
                    MenuController = "ValidatedSale",
                    MenuDescription = "Menu Validated Sale",
                    MenuFlat = false,
                    MenuIconName = "ValidatedSale",
                    MenuLabel = "ValidatedSale",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(ValidatedSale);
                context.SaveChanges();
            }

            Menu existCommandDilatation = context.Menus.Where(s => s.MenuCode == "CommandDilatation").FirstOrDefault();
            if (existCommandDilatation == null)
            {
                Menu CommandDilatation = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "CommandDilatation",
                    MenuController = "CommandDilatation",
                    MenuDescription = "Menu Define Command Dilatation",
                    MenuFlat = false,
                    MenuIconName = "CommandDilatation",
                    MenuLabel = "CommandDilatation",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(CommandDilatation);
                context.SaveChanges();
            }

            Menu existValideDilatation = context.Menus.Where(s => s.MenuCode == "ValideDilatation").FirstOrDefault();
            if (existValideDilatation == null)
            {
                Menu ValideDilatation = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "ValideDilatation",
                    MenuController = "ValideDilatation",
                    MenuDescription = "Menu Valide Dilatation",
                    MenuFlat = false,
                    MenuIconName = "ValideDilatation",
                    MenuLabel = "ValideDilatation",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(ValideDilatation);
                context.SaveChanges();
            }


            Menu existCommandEquip = context.Menus.Where(s => s.MenuCode == "CommandOtherSale").FirstOrDefault();
            if (existCommandEquip == null)
            {
                Menu CommandEquip = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "CommandOtherSale",
                    MenuController = "CommandOtherSale",
                    MenuDescription = "Menu Define Command other sales",
                    MenuFlat = false,
                    MenuIconName = "CommandOtherSale",
                    MenuLabel = "CommandOtherSale",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(CommandEquip);
                context.SaveChanges();
            }

            Menu existValideOtherSale = context.Menus.Where(s => s.MenuCode == "ValideOtherSale").FirstOrDefault();
            if (existValideOtherSale == null)
            {
                Menu ValideOtherSale = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "ValideOtherSale",
                    MenuController = "ValideOtherSale",
                    MenuDescription = "Menu Valide other sales",
                    MenuFlat = false,
                    MenuIconName = "ValideOtherSale",
                    MenuLabel = "ValideOtherSale",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = 6
                };
                context.Menus.AddOrUpdate(ValideOtherSale);
                context.SaveChanges();
            }



            //ajout du sous menu commande verre speciaux
            SubMenu existPostSpecialOrder = context.SubMenus.Where(s => s.SubMenuCode == "PostSpecialOrder").FirstOrDefault();
            if (existPostSpecialOrder != null)
            {
                /*SubMenu PostSpecialOrder = new SubMenu()
                {
                    Menu = existSpecialOrder,
                    MenuID = existSpecialOrder.MenuID,
                    SubMenuCode = "PostSpecialOrder",
                    SubMenuDescription = "Post Special Order",
                    SubMenuLabel = "Post Special Order",
                    SubMenuController = "PostSpecialOrder",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(PostSpecialOrder);*/
                List<ActionSubMenuProfile> existSubActionMenuProfilePOstSO = context.ActionSubMenuProfiles.Where(s => s.SubMenuID == existPostSpecialOrder.SubMenuID).ToList();
                if (existSubActionMenuProfilePOstSO.Count > 0)
                {
                    context.ActionSubMenuProfiles.RemoveRange(existSubActionMenuProfilePOstSO);
                    context.SaveChanges();
                }
                context.SubMenus.Remove(existPostSpecialOrder);
                context.SaveChanges();
            }
            //ajout du sous menu reception verre speciaux
            SubMenu existReceiveSpecialOrder = context.SubMenus.Where(s => s.SubMenuCode == "ReceiveSpecialOrder").FirstOrDefault();
            if (existReceiveSpecialOrder != null)
            {

                List<ActionSubMenuProfile> existSubActionMenuProfileReceivSO = context.ActionSubMenuProfiles.Where(s => s.SubMenuID == existReceiveSpecialOrder.SubMenuID).ToList();
                if (existSubActionMenuProfileReceivSO.Count > 0)
                {
                    context.ActionSubMenuProfiles.RemoveRange(existSubActionMenuProfileReceivSO);
                    context.SaveChanges();
                }

                context.SubMenus.Remove(existReceiveSpecialOrder);
                context.SaveChanges();
            }
            //ajout du sous menu Livraison verre speciaux
            SubMenu existDeliverSpecialOrder = context.SubMenus.Where(s => s.SubMenuCode == "DeliverSpecialOrder").FirstOrDefault();
            if (existDeliverSpecialOrder != null)
            {

                List<ActionSubMenuProfile> existSubActionMenuProfileDelivSO = context.ActionSubMenuProfiles.Where(s => s.SubMenuID == existDeliverSpecialOrder.SubMenuID).ToList();
                if (existSubActionMenuProfileDelivSO.Count > 0)
                {
                    context.ActionSubMenuProfiles.RemoveRange(existSubActionMenuProfileDelivSO);
                    context.SaveChanges();
                }
                context.SubMenus.Remove(existDeliverSpecialOrder);
                context.SaveChanges();
            }
            //ajout du menu special order
            Menu existSpecialOrder = context.Menus.Where(s => s.MenuCode == "SpecialOrder").FirstOrDefault();
            if (existSpecialOrder != null)
            {


                List<ActionMenuProfile> existActionMenuProfile = context.ActionMenuProfiles.Where(s => s.MenuID == existSpecialOrder.MenuID).ToList();
                if (existActionMenuProfile.Count > 0)
                {
                    context.ActionMenuProfiles.RemoveRange(existActionMenuProfile);
                    context.SaveChanges();
                }
                context.Menus.Remove(existSpecialOrder);
                context.SaveChanges();
            }
            #endregion

            #region CCM Module, Menus And SubMenus
            //ajout du module crm
            Module complaintManagementModule = context.Modules.Where(s => s.ModuleCode == "CCM").FirstOrDefault();
            if (complaintManagementModule == null)
            {
                complaintManagementModule = new Module()
                {
                    ModuleArea = "CCM",
                    ModuleCode = "CCM",
                    ModuleDescription = "Module de CCM",
                    ModuleLabel = "Module de CCM",
                    ModuleState = true,
                };
                context.Modules.Add(complaintManagementModule);
                context.SaveChanges();
            }

            //suppression d deliver desk ds crm
            Menu oldDeliverProduct = context.Menus.Where(s => s.MenuCode == "DeliverProduct" && s.ModuleID==7).FirstOrDefault();
            if (oldDeliverProduct != null) {
                foreach (SubMenu subM in context.SubMenus.Where(c => c.MenuID == oldDeliverProduct.MenuID).ToList())
                {
                    DeleteSubMenu(context, subM);
                }
                DeleteMenu(context, oldDeliverProduct);
            }
                
            //creation du menu de deliver desk
            Menu NewDeliverProduct = context.Menus.Where(s => s.MenuCode == "DeliverProduct").FirstOrDefault();
            if (NewDeliverProduct == null)
            {
                Menu DeliverProduct = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "DeliverProduct",
                    MenuController = "DeliverProduct",
                    MenuDescription = "Menu Deliver Product",
                    MenuFlat = false,
                    MenuIconName = "DeliverProduct",
                    MenuLabel = "DeliverProduct",
                    MenuPath = "",
                    MenuState = true,
                    ModuleID = complaintManagementModule.ModuleID,
                    AppearanceOrder = 0
                };
                context.Menus.AddOrUpdate(DeliverProduct);
                context.SaveChanges();
                NewDeliverProduct = DeliverProduct;
            }

            
            if (NewDeliverProduct != null)
            {
                //creation du submenu de deliver desk
                //SubMenu oldSubDeliverDesk = context.SubMenus.Where(s => s.SubMenuCode == "DeliverDesk").FirstOrDefault();
                //if (oldSubDeliverDesk != null)
                //{
                //    DeleteSubMenu(context, oldSubDeliverDesk);
                //}
                
                SubMenu existSubDeliverDesk = context.SubMenus.Where(s => s.SubMenuCode == "DeliverDesk").FirstOrDefault();
                if (existSubDeliverDesk == null)
                {
                    SubMenu DeliverDesk = new SubMenu()
                    {
                        Menu = NewDeliverProduct,
                        MenuID = NewDeliverProduct.MenuID,
                        SubMenuCode = "DeliverDesk",
                        SubMenuDescription = "DeliverDesk",
                        SubMenuLabel = "DeliverDesk",
                        SubMenuController = "DeliverDesk",
                        SubMenuPath = "Index",
                        AppearanceOrder = 6
                    };
                    context.SubMenus.AddOrUpdate(DeliverDesk);
                    context.SaveChanges();
                }

                //creation du submenu de stock order reception
                //SubMenu oldSubOrderRXLenses = context.SubMenus.Where(s => s.SubMenuCode == "OrderRXLenses").FirstOrDefault();
                //if (oldSubOrderRXLenses != null)
                //{
                //    DeleteSubMenu(context, oldSubOrderRXLenses);
                //}
                SubMenu existSubOrderRXLenses = context.SubMenus.Where(s => s.SubMenuCode == "OrderRXLenses").FirstOrDefault();
                if (existSubOrderRXLenses == null)
                {
                    SubMenu OrderRXLenses = new SubMenu()
                    {
                        Menu = NewDeliverProduct,
                        MenuID = NewDeliverProduct.MenuID,
                        SubMenuCode = "OrderRXLenses",
                        SubMenuDescription = "OrderRXLenses",
                        SubMenuLabel = "OrderRXLenses",
                        SubMenuController = "OrderRXLenses",
                        SubMenuPath = "Index",
                        AppearanceOrder=0
                    };
                    context.SubMenus.AddOrUpdate(OrderRXLenses);
                    context.SaveChanges();
                }

                //creation du submenu de Order Stock Order Lenses
                //SubMenu oldSubOrderStockOrderLenses = context.SubMenus.Where(s => s.SubMenuCode == "OrderStockOrderLenses").FirstOrDefault();
                //if (oldSubOrderStockOrderLenses != null)
                //{
                //    DeleteSubMenu(context, oldSubOrderStockOrderLenses);
                //}

                SubMenu existSubOrderStockOrderLenses = context.SubMenus.Where(s => s.SubMenuCode == "OrderStockOrderLenses").FirstOrDefault();
                if (existSubOrderStockOrderLenses == null)
                {
                    SubMenu OrderStockOrderLenses = new SubMenu()
                    {
                        Menu = NewDeliverProduct,
                        MenuID = NewDeliverProduct.MenuID,
                        SubMenuCode = "OrderStockOrderLenses",
                        SubMenuDescription = "OrderStockOrderLenses",
                        SubMenuLabel = "OrderStockOrderLenses",
                        SubMenuController = "OrderRXLenses",
                        SubMenuPath = "Index",
                        AppearanceOrder = 2

                    };
                    context.SubMenus.AddOrUpdate(OrderStockOrderLenses);
                    context.SaveChanges();
                }

                //creation du submenu de special order reception
                //SubMenu oldSubSpecialOrderReception = context.SubMenus.Where(s => s.SubMenuCode == "SpecialOrderReception").FirstOrDefault();
                //if (oldSubSpecialOrderReception != null)
                //{
                //    DeleteSubMenu(context, oldSubSpecialOrderReception);
                //}

                SubMenu existSubSpecialOrderReception = context.SubMenus.Where(s => s.SubMenuCode == "SpecialOrderReception").FirstOrDefault();
                if (existSubSpecialOrderReception == null)
                {
                    SubMenu SpecialOrderReception = new SubMenu()
                    {
                        Menu = NewDeliverProduct,
                        MenuID = NewDeliverProduct.MenuID,
                        SubMenuCode = "SpecialOrderReception",
                        SubMenuDescription = "SpecialOrderReception",
                        SubMenuLabel = "SpecialOrderReception",
                        SubMenuController = "SpecialOrderReception",
                        SubMenuPath = "Index",
                        AppearanceOrder = 1
                    };
                    context.SubMenus.AddOrUpdate(SpecialOrderReception);
                    context.SaveChanges();
                }

                //creation du submenu de stock order reception
                //SubMenu oldSubStockOrderReception = context.SubMenus.Where(s => s.SubMenuCode == "StockOrderReception").FirstOrDefault();
                //if (oldSubStockOrderReception != null)
                //{
                //    DeleteSubMenu(context, oldSubStockOrderReception);
                //}

                SubMenu existSubStockOrderReception = context.SubMenus.Where(s => s.SubMenuCode == "StockOrderReception").FirstOrDefault();
                if (existSubStockOrderReception == null)
                {
                    SubMenu StockOrderReception = new SubMenu()
                    {
                        Menu = NewDeliverProduct,
                        MenuID = NewDeliverProduct.MenuID,
                        SubMenuCode = "StockOrderReception",
                        SubMenuDescription = "StockOrderReception",
                        SubMenuLabel = "StockOrderReception",
                        SubMenuController = "SpecialOrderReception",
                        SubMenuPath = "Index",
                        AppearanceOrder = 3
                    };
                    context.SubMenus.AddOrUpdate(StockOrderReception);
                    context.SaveChanges();
                }


                //creation du submenu de Lens Mounting
                //SubMenu oldSubLensMounting = context.SubMenus.Where(s => s.SubMenuCode == "LensMounting").FirstOrDefault();
                //if (oldSubLensMounting != null)
                //{
                //    DeleteSubMenu(context, oldSubLensMounting);
                //}

                SubMenu existSubLensMounting = context.SubMenus.Where(s => s.SubMenuCode == "LensMounting").FirstOrDefault();
                if (existSubLensMounting == null)
                {
                    SubMenu LensMounting = new SubMenu()
                    {
                        Menu = NewDeliverProduct,
                        MenuID = NewDeliverProduct.MenuID,
                        SubMenuCode = "LensMounting",
                        SubMenuDescription = "LensMounting",
                        SubMenuLabel = "LensMounting",
                        SubMenuController = "LensMounting",
                        SubMenuPath = "Index",
                        AppearanceOrder = 5
                    };
                    context.SubMenus.AddOrUpdate(LensMounting);
                    context.SaveChanges();
                }

                //creation du submenu de Lens Mounting Damage
                //SubMenu oldlensMountingDamage = context.SubMenus.Where(s => s.SubMenuCode == "LensMountingDamage").FirstOrDefault();
                //if (oldlensMountingDamage != null)
                //{
                //    DeleteSubMenu(context, oldlensMountingDamage);
                //}
                SubMenu lensMountingDamage = context.SubMenus.Where(s => s.SubMenuCode == "LensMountingDamage").FirstOrDefault();
                if (lensMountingDamage == null)
                {
                    lensMountingDamage = new SubMenu()
                    {
                        Menu = NewDeliverProduct,
                        MenuID = NewDeliverProduct.MenuID,
                        SubMenuCode = "LensMountingDamage",
                        SubMenuDescription = "LensMountingDamage",
                        SubMenuLabel = "LensMountingDamage",
                        SubMenuController = "LensMountingDamage",
                        SubMenuPath = "Index",
                        AppearanceOrder = 7
                    };
                    context.SubMenus.AddOrUpdate(lensMountingDamage);
                    context.SaveChanges();
                }

                //creation du submenu de Lens Mounting
                //SubMenu oldSubPrescriptionHarmonisation = context.SubMenus.Where(s => s.SubMenuCode == "PrescriptionHarmonisation").FirstOrDefault();
                //if (oldSubPrescriptionHarmonisation != null)
                //{
                //    DeleteSubMenu(context, oldSubPrescriptionHarmonisation);
                //}

                SubMenu existSubPrescriptionHarmonisation = context.SubMenus.Where(s => s.SubMenuCode == "PrescriptionHarmonisation").FirstOrDefault();
                if (existSubPrescriptionHarmonisation == null)
                {
                    SubMenu PrescriptionHarmonisation = new SubMenu()
                    {
                        Menu = NewDeliverProduct,
                        MenuID = NewDeliverProduct.MenuID,
                        SubMenuCode = "PrescriptionHarmonisation",
                        SubMenuDescription = "PrescriptionHarmonisation",
                        SubMenuLabel = "PrescriptionHarmonisation",
                        SubMenuController = "PrescriptionHarmonisation",
                        SubMenuPath = "Index",
                        AppearanceOrder = 4
                    };
                    context.SubMenus.AddOrUpdate(PrescriptionHarmonisation);
                    context.SaveChanges();
                }
            }

            //CREATION DU MENU GESTION PLAINTE
            Menu EXITGESTIONPLAINTE = context.Menus.Where(s => s.MenuCode == "GESTIONPLAINTE").FirstOrDefault();
            if (EXITGESTIONPLAINTE == null)
            {
                Menu GESTIONPLAINTE = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "GESTIONPLAINTE",
                    MenuController = "GESTIONPLAINTE",
                    MenuDescription = "GESTIONPLAINTE",
                    MenuFlat = false,
                    MenuIconName = "GESTIONPLAINTE",
                    MenuLabel = "GESTIONPLAINTE",
                    MenuPath = "",
                    MenuState = true,
                    ModuleID = complaintManagementModule.ModuleID,
                    AppearanceOrder=1
                };
                context.Menus.AddOrUpdate(GESTIONPLAINTE);
                context.SaveChanges();
                EXITGESTIONPLAINTE = GESTIONPLAINTE;
            }

            if (EXITGESTIONPLAINTE!=null)
            {
                Menu existcomplaintRegMenu = context.Menus.Where(s => s.MenuCode == "CCM_REG").FirstOrDefault();
                if (existcomplaintRegMenu != null)
                {
                    DeleteMenu(context, existcomplaintRegMenu);
                }

                SubMenu complaintRegMenu = context.SubMenus.Where(s => s.SubMenuCode == "CCM_REG").FirstOrDefault();
                if (complaintRegMenu == null)
                {
                    complaintRegMenu = new SubMenu()
                    {
                        MenuID = EXITGESTIONPLAINTE.MenuID,
                        SubMenuCode = "CCM_REG",
                        SubMenuDescription = "ComplaintRegistration",
                        SubMenuLabel = "Menu Complaint Registration",
                        SubMenuController = "ComplaintRegistration",
                        SubMenuPath = "Index",
                        AppearanceOrder=0
                    };
                    context.SubMenus.AddOrUpdate(complaintRegMenu);
                    context.SaveChanges();
                }


                Menu exitcomplaintResolutionMenu = context.Menus.Where(s => s.MenuCode == "CCM_RES").FirstOrDefault();
                if (exitcomplaintResolutionMenu != null)
                {
                    DeleteMenu(context, exitcomplaintResolutionMenu);
                }

                SubMenu complaintResolutionMenu = context.SubMenus.Where(s => s.SubMenuCode == "CCM_RES").FirstOrDefault();
                if (complaintResolutionMenu == null)
                {
                    complaintResolutionMenu = new SubMenu()
                    {
                        MenuID = EXITGESTIONPLAINTE.MenuID,
                        SubMenuCode = "CCM_RES",
                        SubMenuDescription = "ComplaintResolution",
                        SubMenuLabel = "Menu Complaint Resolution",
                        SubMenuController = "ComplaintResolution",
                        SubMenuPath = "Index",
                        AppearanceOrder = 1
                    };
                    context.SubMenus.AddOrUpdate(complaintResolutionMenu);
                    context.SaveChanges();
                }

                Menu exitcomplaintControlMenu = context.Menus.Where(s => s.MenuCode == "CCM_CTRL").FirstOrDefault();
                if (exitcomplaintControlMenu != null)
                {
                    DeleteMenu(context, exitcomplaintControlMenu);
                }
                SubMenu complaintControlMenu = context.SubMenus.Where(s => s.SubMenuCode == "CCM_CTRL").FirstOrDefault();
                if (complaintControlMenu == null)
                {
                    complaintControlMenu = new SubMenu()
                    {
                        MenuID = EXITGESTIONPLAINTE.MenuID,
                        SubMenuCode = "CCM_CTRL",
                        SubMenuDescription = "ComplaintControlled",
                        SubMenuLabel = "Menu ComplaintControlled",
                        SubMenuController = "ComplaintControlled",
                        SubMenuPath = "Index",
                        AppearanceOrder = 2
                    };
                    context.SubMenus.AddOrUpdate(complaintControlMenu);
                    context.SaveChanges();
                }

                Menu exitcomplaintFeedBackMenu = context.Menus.Where(s => s.MenuCode == "CCM_FB").FirstOrDefault();
                if (exitcomplaintFeedBackMenu != null)
                {
                    DeleteMenu(context, exitcomplaintFeedBackMenu);
                }

                SubMenu complaintFeedBackMenu = context.SubMenus.Where(s => s.SubMenuCode == "CCM_FB").FirstOrDefault();
                if (complaintFeedBackMenu == null)
                {
                    complaintFeedBackMenu = new SubMenu()
                    {
                        MenuID = EXITGESTIONPLAINTE.MenuID,
                        SubMenuCode = "CCM_FB",
                        SubMenuDescription = "ComplaintFeedBack",
                        SubMenuLabel = "Menu ComplaintFeedBack",
                        SubMenuController = "ComplaintFeedBack",
                        SubMenuPath = "Index",
                        AppearanceOrder = 3
                    };
                    context.SubMenus.AddOrUpdate(complaintFeedBackMenu);
                    context.SaveChanges();
                }
            }

            Menu oldCustomerSatisfaction = context.Menus.Where(s => s.MenuCode == "CustomerSatisfaction" && s.ModuleID==7).FirstOrDefault();
            if (oldCustomerSatisfaction != null)
            {
                DeleteMenu(context, oldCustomerSatisfaction);
            }
            
            Menu existCustomerSatisfaction = context.Menus.Where(s => s.MenuCode == "CustomerSatisfaction").FirstOrDefault();
            if (existCustomerSatisfaction == null)
            {
                Menu CustomerSatisfaction = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "CustomerSatisfaction",
                    MenuController = "CustomerSatisfaction",
                    MenuDescription = "Menu Customer Satisfaction",
                    MenuFlat = false,
                    MenuIconName = "CustomerSatisfaction",
                    MenuLabel = "CustomerSatisfaction",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = complaintManagementModule.ModuleID,
                    AppearanceOrder = 2
                };
                context.Menus.AddOrUpdate(CustomerSatisfaction);
                context.SaveChanges();
            }

            Menu oldNoPurchase = context.Menus.Where(s => s.MenuCode == "NoPurchase" && s.ModuleID == 7).FirstOrDefault();
            if (oldNoPurchase != null)
            {
                DeleteMenu(context, oldNoPurchase);
            }

            //Ajout du menu No Purchase 
            Menu existNoPurchase = context.Menus.Where(s => s.MenuCode == "NoPurchase").FirstOrDefault();
            if (existNoPurchase == null)
            {
                Menu NoPurchase = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "NoPurchase",
                    MenuController = "NoPurchase",
                    MenuDescription = "Menu No Purchase",
                    MenuFlat = false,
                    MenuIconName = "NoPurchase",
                    MenuLabel = "NoPurchase",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = complaintManagementModule.ModuleID,
                    AppearanceOrder = 3
                };
                context.Menus.AddOrUpdate(NoPurchase);
                context.SaveChanges();
            }

            Menu CCMReport = context.Menus.Where(s => s.MenuCode == "CCM_RPT").FirstOrDefault();
            if (CCMReport == null)
            {
                CCMReport = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "CCM_RPT",
                    MenuController = "ComplaintReport",
                    MenuDescription = "Menu CCM Report",
                    MenuFlat = false,
                    MenuIconName = "CCMReport",
                    MenuLabel = "CCMReport",
                    MenuPath = "",
                    MenuState = true,
                    ModuleID = complaintManagementModule.ModuleID,
                    AppearanceOrder = 5
                };
                context.Menus.AddOrUpdate(CCMReport);
                context.SaveChanges();
            }

            #region Report Sub Menus
            SubMenu complaintReportSM = context.SubMenus.Where(s => s.SubMenuCode == "CCM_RPT_C").FirstOrDefault();
            if (complaintReportSM == null)
            {
                complaintReportSM = new SubMenu()
                {
                    MenuID = CCMReport.MenuID,
                    SubMenuCode = "CCM_RPT_C",
                    SubMenuDescription = "ComplaintReport",
                    SubMenuLabel = "ComplaintReport",
                    SubMenuController = "ComplaintRpt",
                    SubMenuPath = "Index",
                    AppearanceOrder = 0
                };
                context.SubMenus.AddOrUpdate(complaintReportSM);
                context.SaveChanges();
            }

            SubMenu feedBackReportSM = context.SubMenus.Where(s => s.SubMenuCode == "CCM_RPT_FB").FirstOrDefault();
            if (feedBackReportSM == null)
            {
                feedBackReportSM = new SubMenu()
                {
                    MenuID = CCMReport.MenuID,
                    SubMenuCode = "CCM_RPT_FB",
                    SubMenuDescription = "FeedBackReport",
                    SubMenuLabel = "FeedBackReport",
                    SubMenuController = "FeedBackRpt",
                    SubMenuPath = "Index",
                    AppearanceOrder = 1
                };
                context.SubMenus.AddOrUpdate(feedBackReportSM);
                context.SaveChanges();
            }
            //suppression de l'ancien emplacement
            SubMenu oldStockOrderRpt = context.SubMenus.Where(s => s.SubMenuCode == "StockOrderRpt" && s.MenuID == NewCRMReport.MenuID).FirstOrDefault();
            if (oldStockOrderRpt!=null)
            {
                DeleteSubMenu(context, oldStockOrderRpt);
            }

            SubMenu existStockOrderRpt = context.SubMenus.Where(s => s.SubMenuCode == "StockOrderRpt").FirstOrDefault();
            if (existStockOrderRpt == null)
            {
                SubMenu StockOrderRpt = new SubMenu()
                {
                    Menu = CCMReport,
                    MenuID = CCMReport.MenuID,
                    SubMenuCode = "StockOrderRpt",
                    SubMenuDescription = "StockOrderRpt",
                    SubMenuLabel = "StockOrderRpt",
                    SubMenuController = "RXLensesRpt",
                    SubMenuPath = "Index",
                    AppearanceOrder = 3
                };
                context.SubMenus.AddOrUpdate(StockOrderRpt);
                context.SaveChanges();
            }

            //suppression de l'ancien emplacement
            SubMenu oldRXLensesRpt = context.SubMenus.Where(s => s.SubMenuCode == "RXLensesRpt" && s.MenuID == NewCRMReport.MenuID).FirstOrDefault();
            if (oldRXLensesRpt != null)
            {
                DeleteSubMenu(context, oldRXLensesRpt);
            }

            SubMenu existRXLensesRpt = context.SubMenus.Where(s => s.SubMenuCode == "RXLensesRpt").FirstOrDefault();
            if (existRXLensesRpt == null)
            {
                SubMenu RXLensesRpt = new SubMenu()
                {
                    Menu = CCMReport,
                    MenuID = CCMReport.MenuID,
                    SubMenuCode = "RXLensesRpt",
                    SubMenuDescription = "RXLensesRpt",
                    SubMenuLabel = "RXLensesRpt",
                    SubMenuController = "RXLensesRpt",
                    SubMenuPath = "Index",
                    AppearanceOrder = 2
                };
                context.SubMenus.AddOrUpdate(RXLensesRpt);
                context.SaveChanges();
            }

            //suppression de l'ancien emplacement
            SubMenu oldCustomerSatisfactionReport = context.SubMenus.Where(s => s.SubMenuCode == "CustomerSatisfactionReport" && s.MenuID == NewCRMReport.MenuID).FirstOrDefault();
            if (oldCustomerSatisfactionReport != null)
            {
                DeleteSubMenu(context, oldCustomerSatisfactionReport);
            }

            SubMenu existCustomerSatisfactionReport = context.SubMenus.Where(s => s.SubMenuCode == "CustomerSatisfactionReport").FirstOrDefault();
            if (existCustomerSatisfactionReport == null)
            {
                SubMenu CustomerSatisfactionReport = new SubMenu()
                {
                    Menu = CCMReport,
                    MenuID = CCMReport.MenuID,
                    SubMenuCode = "CustomerSatisfactionReport",
                    SubMenuDescription = "CustomerSatisfactionReport",
                    SubMenuLabel = "CustomerSatisfactionReport",
                    SubMenuController = "CustomerSatisfactionReport",
                    SubMenuPath = "Index",
                    AppearanceOrder = 4
                };
                context.SubMenus.AddOrUpdate(CustomerSatisfactionReport);
                context.SaveChanges();
            }

            //suppression de l'ancien emplacement
            SubMenu oldNoPurchaseReport = context.SubMenus.Where(s => s.SubMenuCode == "NoPurchaseReport" && s.MenuID == NewCRMReport.MenuID).FirstOrDefault();
            if (oldNoPurchaseReport != null)
            {
                DeleteSubMenu(context, oldNoPurchaseReport);
            }

            SubMenu existNoPurchaseReport = context.SubMenus.Where(s => s.SubMenuCode == "NoPurchaseReport").FirstOrDefault();
            if (existNoPurchaseReport == null)
            {
                SubMenu NoPurchaseReport = new SubMenu()
                {
                    Menu = CCMReport,
                    MenuID = CCMReport.MenuID,
                    SubMenuCode = "NoPurchaseReport",
                    SubMenuDescription = "NoPurchaseReport",
                    SubMenuLabel = "NoPurchaseReport",
                    SubMenuController = "NoPurchaseReport",
                    SubMenuPath = "Index",
                    AppearanceOrder = 5
                };
                context.SubMenus.AddOrUpdate(NoPurchaseReport);
                context.SaveChanges();
            }

            //suppression de l'ancien emplacement
            SubMenu oldRptLensMountingHistory = context.SubMenus.Where(s => s.SubMenuCode == "RptLensMountingHistory" && s.MenuID == NewCRMReport.MenuID).FirstOrDefault();
            if (oldRptLensMountingHistory != null)
            {
                DeleteSubMenu(context, oldRptLensMountingHistory);
            }

            SubMenu existLensMountingHistoryReport = context.SubMenus.Where(s => s.SubMenuCode == "RptLensMountingHistory").FirstOrDefault();
            if (existLensMountingHistoryReport == null)
            {
                SubMenu lensMountingHistoryReport = new SubMenu()
                {
                    Menu = CCMReport,
                    MenuID = CCMReport.MenuID,
                    SubMenuCode = "RptLensMountingHistory",
                    SubMenuDescription = "RptLensMountingHistory",
                    SubMenuLabel = "RptLensMountingHistory",
                    SubMenuController = "RptLensMountingHistory",
                    SubMenuPath = "Index",
                    AppearanceOrder = 7
                };
                context.SubMenus.AddOrUpdate(lensMountingHistoryReport);
                context.SaveChanges();
            }

            //suppression de l'ancien emplacement
            Menu oldPatientRecord = context.Menus.Where(s => s.MenuCode == "PatientRecord" && s.ModuleID == 7).FirstOrDefault();
            if (oldPatientRecord != null)
            {
                DeleteMenu(context, oldPatientRecord);
            }

            // Creation du SOUS menu Dossier Patient
            var existPatientRecord = context.SubMenus.Where(s => s.SubMenuCode == "PatientRecord").FirstOrDefault();
            if (existPatientRecord == null)
            {
                existPatientRecord = new SubMenu()
                {
                    Menu = CCMReport,
                    MenuID = CCMReport.MenuID,
                    SubMenuCode = "PatientRecord",
                    SubMenuDescription = "PatientRecord",
                    SubMenuLabel = "PatientRecord",
                    SubMenuController = "PatientRecord",
                    SubMenuPath = "Index",
                    AppearanceOrder = 6
                };
                context.SubMenus.AddOrUpdate(existPatientRecord);
                context.SaveChanges();

            }

            #endregion
            #endregion

            #region BareCode Module, Menus and SubMenus
            Module barCodeManagementModule = context.Modules.Where(s => s.ModuleCode == "BarCode").FirstOrDefault();
            if (barCodeManagementModule == null)
            {
                barCodeManagementModule = new Module()
                {
                    ModuleArea = "BarCode",
                    ModuleCode = "BarCode",
                    ModuleDescription = "Module de BarCode",
                    ModuleLabel = "Module de BarCode",
                    ModuleState = true,
                };
                context.Modules.Add(barCodeManagementModule);
                context.SaveChanges();
            }

            #region Barcode Generator Menu
            Menu barCodeGeneratorMenu = context.Menus.Where(s => s.MenuCode == "BarCodeGenerator").FirstOrDefault();
            if (barCodeGeneratorMenu == null)
            {
                barCodeGeneratorMenu = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "BarCodeGenerator",
                    MenuController = "BarCodeGenerator",
                    MenuDescription = "Menu BarCode Generator",
                    MenuFlat = false,
                    MenuIconName = "BarCodeGenerator",
                    MenuLabel = "BarCode Generator",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = barCodeManagementModule.ModuleID
                };
                context.Menus.AddOrUpdate(barCodeGeneratorMenu);
                context.SaveChanges();
            }
            #endregion

            #region Stock Input
            Menu barCodeStockInputMenu = context.Menus.Where(s => s.MenuCode == "BarCodeStockInput").FirstOrDefault();
            if (barCodeStockInputMenu == null)
            {
                barCodeStockInputMenu = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "BarCodeStockInput",
                    MenuController = "BarCodeStockInput",
                    MenuDescription = "Menu BarCode Stock Input ",
                    MenuFlat = false,
                    MenuIconName = "BarCodeStockInput",
                    MenuLabel = "BarCode Stock Input",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = barCodeManagementModule.ModuleID
                };
                context.Menus.AddOrUpdate(barCodeStockInputMenu);
                context.SaveChanges();
            }
            #endregion

            #region Inventory Menu
            CreateBarcodeInventoryMenuAndSubMenus(context, barCodeManagementModule);
            #endregion

            #region Inventory Menu
            CreateBarcodeTransfertMenuAndSubMenus(context, barCodeManagementModule);
            #endregion

            #endregion

            #region Product Transfert Menus and SubMenus
            var supplyModule = context.Modules.SingleOrDefault(m => m.ModuleCode == "MODULE_2");
            CreateProductTransfertMenuAndSubMenus(context, supplyModule);
            var cashRegisterModule = context.Modules.SingleOrDefault(m => m.ModuleCode == "MODULE_3");
            //var saleModule = context.Modules.SingleOrDefault(m => m.ModuleCode == "MODULE_1");
            CreateNonDepositedBordero(context, cashRegisterModule);
            #endregion

            #region MODULES ORDERING
            ModuleOrdering(context);
            #endregion

            #region Banned Numbers
            AddBannedNumbers(context);
            #endregion

            #region DIGITAL PAYMENT
            CreateDigitalPaymentSubMenus(context, RptCashMenu.MenuID);
            #endregion

            #region PARTNER SUB MENU
            var existPartner = context.SubMenus.Where(s => s.SubMenuCode == "Partner").FirstOrDefault();
            if (existPartner == null)
            {
                var company = context.Menus.SingleOrDefault(m => m.MenuCode == "CompJobCode");
                var partner = new SubMenu()
                {
                    IsChortcut = false,
                    SubMenuCode = "Partner",
                    SubMenuController = "Partner",
                    SubMenuDescription = "Partner",
                    SubMenuLabel = "Partner",
                    SubMenuPath = "Index",
                    MenuID = company.MenuID
                };
                context.SubMenus.AddOrUpdate(partner);
                context.SaveChanges();
            }
            #endregion
        }

        private void CreateNonDepositedBordero(EFDbContext context, Module cashRegisterModule)
        {
            //suppression du menu ds cash
            var OldNonDepositedBorderoMenu = context.Menus.Where(s => s.MenuCode == CodeValue.Supply.NonDepositedBordero && s.ModuleID==cashRegisterModule.ModuleID).FirstOrDefault();
            if (OldNonDepositedBorderoMenu!=null)
            {
                //supression du menu
                List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == OldNonDepositedBorderoMenu.MenuID).ToList();
                context.ActionMenuProfiles.RemoveRange(lstActionMP);
                context.Menus.Remove(OldNonDepositedBorderoMenu);
                context.SaveChanges();
            }

            Menu RptSaleMenu = context.Menus.Where(m => m.MenuCode == "SMnuSalRpt").FirstOrDefault();

            SubMenu NonDepositedBorderoMenu = context.SubMenus.Where(s => s.SubMenuCode == CodeValue.Supply.NonDepositedBordero).FirstOrDefault();
            if (NonDepositedBorderoMenu == null)
            {
                NonDepositedBorderoMenu = new SubMenu()
                {
                    Menu = RptSaleMenu,
                    MenuID = RptSaleMenu.MenuID,
                    SubMenuCode = CodeValue.Supply.NonDepositedBordero,
                    SubMenuDescription = CodeValue.Supply.NonDepositedBordero,
                    SubMenuLabel = CodeValue.Supply.NonDepositedBordero,
                    SubMenuController = CodeValue.Supply.NonDepositedBordero,
                    SubMenuPath = "Index",
                    AppearanceOrder = 7
                };
                context.SubMenus.AddOrUpdate(NonDepositedBorderoMenu);
                context.SaveChanges();
            }


            //suppression du menu ds cash
            var oldDepositedBorderoMenu = context.Menus.Where(s => s.MenuCode == CodeValue.Supply.DepositedBordero && s.ModuleID == cashRegisterModule.ModuleID).FirstOrDefault();
            if (oldDepositedBorderoMenu != null)
            {
                //supression du menu
                List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == oldDepositedBorderoMenu.MenuID).ToList();
                context.ActionMenuProfiles.RemoveRange(lstActionMP);
                context.Menus.Remove(oldDepositedBorderoMenu);
                context.SaveChanges();
            }

            SubMenu DepositedBorderoMenu = context.SubMenus.Where(s => s.SubMenuCode == CodeValue.Supply.DepositedBordero).FirstOrDefault();
            if (DepositedBorderoMenu == null)
            {
                DepositedBorderoMenu = new SubMenu()
                {
                    Menu = RptSaleMenu,
                    MenuID = RptSaleMenu.MenuID,
                    SubMenuCode = CodeValue.Supply.DepositedBordero,
                    SubMenuDescription = CodeValue.Supply.DepositedBordero,
                    SubMenuLabel = CodeValue.Supply.DepositedBordero,
                    SubMenuController = CodeValue.Supply.DepositedBordero,
                    SubMenuPath = "Index",
                    AppearanceOrder=8
                    
                };
                context.SubMenus.AddOrUpdate(DepositedBorderoMenu);
                context.SaveChanges();
            }

           
        }

        public void CreateDigitalPaymentSubMenus(EFDbContext context, int RptCashMenuId)
        {
            // 
            var existDigitalPayment = context.SubMenus.Where(s => s.SubMenuCode == "DigitalPayment").FirstOrDefault();
            if (existDigitalPayment == null)
            {
                var treasuryMenu = context.Menus.SingleOrDefault(m => m.MenuCode == "MENUMONEYCODE");
                var digitalPayment = new SubMenu()
                {
                    MenuID = treasuryMenu.MenuID,
                    SubMenuCode = "DigitalPayment",
                    SubMenuDescription = "Digital Payment menu",
                    SubMenuLabel = "DigitalPayment",
                    SubMenuController = "DigitalPayment",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(digitalPayment);
                context.SaveChanges();
            }

            var existDigitalOperationHistory = context.SubMenus.Where(s => s.SubMenuCode == "DigitalOperationHistory").FirstOrDefault();
            if (existDigitalOperationHistory == null)
            {
                var digitalOperationHistory = new SubMenu()
                {
                    //Menu = RptCashMenu,
                    MenuID = RptCashMenuId, //RptCashMenu.MenuID,
                    SubMenuCode = "DigitalOperationHistory",
                    SubMenuDescription = "Digital Operation History menu",
                    SubMenuLabel = "DigitalOperationHistory",
                    SubMenuController = "DigitalOperationHistory",
                    SubMenuPath = "Index"
                };
                context.SubMenus.AddOrUpdate(digitalOperationHistory);
                context.SaveChanges();
            }
        }


        private void CreateBarcodeInventoryMenuAndSubMenus(EFDbContext context, Module barCodeManagementModule)
        {
            var inventoryCountingMenu = context.Menus.Where(s => s.MenuCode == "InventoryCounting").FirstOrDefault();

            if (inventoryCountingMenu == null)
            {
                inventoryCountingMenu = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "InventoryCounting",
                    MenuController = "InventoryCounting",
                    MenuDescription = "Menu Barcode Inventory ",
                    MenuFlat = false,
                    MenuIconName = "InventoryCounting",
                    MenuLabel = "Barcode Inventory",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = barCodeManagementModule.ModuleID
                };
                context.Menus.AddOrUpdate(inventoryCountingMenu);
                context.SaveChanges();
            }

            #region Directory Sub Menu
            SubMenu directorySM = context.SubMenus.Where(s => s.SubMenuCode == "Directory").FirstOrDefault();
            if (directorySM == null)
            {
                directorySM = new SubMenu()
                {
                    MenuID = inventoryCountingMenu.MenuID,
                    SubMenuCode = "Directory",
                    SubMenuDescription = "Directory",
                    SubMenuLabel = "Directory",
                    SubMenuController = "Directory",
                    SubMenuPath = "Index",
                    AppearanceOrder = 1
                };
                context.SubMenus.AddOrUpdate(directorySM);
                context.SaveChanges();
            }
            #endregion

            #region Counting Sub Menu
            SubMenu countingSM = context.SubMenus.Where(s => s.SubMenuCode == "Counting").FirstOrDefault();
            if (countingSM == null)
            {
                countingSM = new SubMenu()
                {
                    MenuID = inventoryCountingMenu.MenuID,
                    SubMenuCode = "Counting",
                    SubMenuDescription = "Counting",
                    SubMenuLabel = "Counting",
                    SubMenuController = "Counting",
                    SubMenuPath = "Index",
                    AppearanceOrder = 2
                };
                context.SubMenus.AddOrUpdate(countingSM);
                context.SaveChanges();
            }
            #endregion

            #region Reconcilliation Sub Menu
            SubMenu reconciliationSM = context.SubMenus.Where(s => s.SubMenuCode == "Reconciliation").FirstOrDefault();
            if (reconciliationSM == null)
            {
                reconciliationSM = new SubMenu()
                {
                    MenuID = inventoryCountingMenu.MenuID,
                    SubMenuCode = "Reconciliation",
                    SubMenuDescription = "Reconciliation",
                    SubMenuLabel = "Reconciliation",
                    SubMenuController = "Reconciliation",
                    SubMenuPath = "Index",
                    AppearanceOrder = 3
                };
                context.SubMenus.AddOrUpdate(reconciliationSM);
                context.SaveChanges();
            }
            #endregion


        }

        private void CreateBarcodeTransfertMenuAndSubMenus(EFDbContext context, Module barcodeModule)
        {
            var transfertMenu = context.Menus.Where(m => m.MenuCode == "Transfert" && m.ModuleID == barcodeModule.ModuleID).FirstOrDefault();

            if (transfertMenu == null)
            {
                transfertMenu = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "Transfert",
                    MenuController = "Transfert",
                    MenuDescription = "Menu Transfert ",
                    MenuFlat = false,
                    MenuIconName = "Transfert",
                    MenuLabel = "Transfert",
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = barcodeModule.ModuleID
                };
                context.Menus.AddOrUpdate(transfertMenu);
                context.SaveChanges();
            }

            #region Stock Input Sub Menu
            SubMenu stockInputSM = context.SubMenus.Where(sm => sm.SubMenuCode == "StockInput" &&
                                                               sm.MenuID == transfertMenu.MenuID
                                                         ).FirstOrDefault();
            if (stockInputSM == null)
            {
                stockInputSM = new SubMenu()
                {
                    MenuID = transfertMenu.MenuID,
                    SubMenuCode = "StockInput",
                    SubMenuDescription = "StockInput",
                    SubMenuLabel = "StockInput",
                    SubMenuController = "StockInput",
                    SubMenuPath = "Index",
                    AppearanceOrder = 1
                };
                context.SubMenus.AddOrUpdate(stockInputSM);
                context.SaveChanges();
            }
            #endregion

            #region Print Barcode Sub Menu
            SubMenu printBarcodeSM = context.SubMenus.Where(sm => sm.SubMenuCode == "PrintBarcode").FirstOrDefault();
            if (printBarcodeSM == null)
            {
                printBarcodeSM = new SubMenu()
                {
                    MenuID = transfertMenu.MenuID,
                    SubMenuCode = "PrintBarcode",
                    SubMenuDescription = "PrintBarcode",
                    SubMenuLabel = "PrintBarcode",
                    SubMenuController = "PrintBarcode",
                    SubMenuPath = "Index",
                    AppearanceOrder = 2
                };
                context.SubMenus.AddOrUpdate(printBarcodeSM);
                context.SaveChanges();
            }
            #endregion

            #region SMS MANAGEMENT Module, Menus And SubMenus
            //ajout du module SMSMNG
            Module SMSMNGModule = context.Modules.Where(s => s.ModuleCode == "SMSMNG").FirstOrDefault();
            if (SMSMNGModule == null)
            {
                SMSMNGModule = new Module()
                {
                    ModuleArea = "SMSMNG",
                    ModuleCode = "SMSMNG",
                    ModuleDescription = "Module de SMSMNG",
                    ModuleLabel = "Module de SMSMNG",
                    ModuleState = true,
                    AppearanceOrder=6
                };
                context.Modules.Add(SMSMNGModule);
                context.SaveChanges();
            }

            /*******************notification***********************************/
            //menu sms notification
            Menu existNotificationSMS = context.Menus.Where(s => s.MenuCode == "NotificationSMS").FirstOrDefault();
            if (existNotificationSMS == null)
            {
                Menu NotificationSMS = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "NotificationSMS",
                    MenuController = null,
                    MenuDescription = "Menu Notification SMS",
                    MenuFlat = false,
                    MenuIconName = "NotificationSMS",
                    MenuLabel = "NotificationSMS",
                    MenuPath = "",
                    MenuState = true,
                    ModuleID = SMSMNGModule.ModuleID,
                    AppearanceOrder = 0
                };
                context.Menus.AddOrUpdate(NotificationSMS);
                context.SaveChanges();
                existNotificationSMS = NotificationSMS;
            }

            if (existNotificationSMS == null)
            {
                existNotificationSMS = context.Menus.Where(s => s.MenuCode == "NotificationSMS").FirstOrDefault();
            }

            if (existNotificationSMS != null)
            {
                //creation du menu de PurchaseNotification
                Menu existPurchaseNotification = context.Menus.Where(s => s.MenuCode == "PurchaseNotification").FirstOrDefault();
                if (existPurchaseNotification != null)
                {
                    //supression du menu
                    DeleteMenu(context, existPurchaseNotification);
                }

                //sub menu NotificationSetting
                SubMenu PurchaseNotification = context.SubMenus.Where(s => s.SubMenuCode == "PurchaseNotification").FirstOrDefault();
                if (PurchaseNotification == null)
                {
                    PurchaseNotification = new SubMenu()
                    {
                        Menu = existNotificationSMS,
                        MenuID = existNotificationSMS.MenuID,
                        SubMenuCode = "PurchaseNotification",
                        SubMenuDescription = "PurchaseNotification",
                        SubMenuLabel = "PurchaseNotification",
                        SubMenuController = "PurchaseNotification",
                        SubMenuPath = "Index",
                        AppearanceOrder = 0
                    };
                    context.SubMenus.AddOrUpdate(PurchaseNotification);
                    context.SaveChanges();
                }


                //creation du menu de DeliveryNotification
                Menu existDeliveryNotification = context.Menus.Where(s => s.MenuCode == "DeliveryNotification").FirstOrDefault();
                if (existDeliveryNotification != null)
                {
                    // supression du menu
                    DeleteMenu(context, existDeliveryNotification);
                }
                //sub menu NotificationSetting
                SubMenu DeliveryNotification = context.SubMenus.Where(s => s.SubMenuCode == "DeliveryNotification").FirstOrDefault();
                if (DeliveryNotification == null)
                {
                    DeliveryNotification = new SubMenu()
                    {
                        Menu = existNotificationSMS,
                        MenuID = existNotificationSMS.MenuID,
                        SubMenuCode = "DeliveryNotification",
                        SubMenuDescription = "DeliveryNotification",
                        SubMenuLabel = "DeliveryNotification",
                        SubMenuController = "DeliveryNotification",
                        SubMenuPath = "Index",
                        AppearanceOrder = 1
                    };
                    context.SubMenus.AddOrUpdate(DeliveryNotification);
                    context.SaveChanges();
                }

                //creation du menu de InsuranceDeliveryNotification
                Menu existInsuranceDeliveryNotification = context.Menus.Where(s => s.MenuCode == "InsuranceDeliveryNotification").FirstOrDefault();
                if (existInsuranceDeliveryNotification != null)
                {
                    // supression du menu
                    DeleteMenu(context, existInsuranceDeliveryNotification);
                }
                //sub menu InsuranceDeliveryNotification
                SubMenu InsuranceDeliveryNotification = context.SubMenus.Where(s => s.SubMenuCode == "InsuranceDeliveryNotification").FirstOrDefault();
                if (InsuranceDeliveryNotification == null)
                {
                    InsuranceDeliveryNotification = new SubMenu()
                    {
                        Menu = existNotificationSMS,
                        MenuID = existNotificationSMS.MenuID,
                        SubMenuCode = "InsuranceDeliveryNotification",
                        SubMenuDescription = "InsuranceDeliveryNotification",
                        SubMenuLabel = "InsuranceDeliveryNotification",
                        SubMenuController = "InsuranceDeliveryNotification",
                        SubMenuPath = "Index",
                        AppearanceOrder = 2
                    };
                    context.SubMenus.AddOrUpdate(InsuranceDeliveryNotification);
                    context.SaveChanges();
                }

                //creation du menu de CommandGlassNotification
                Menu existCommandGlassNotification = context.Menus.Where(s => s.MenuCode == "CommandGlassNotification").FirstOrDefault();
                if (existCommandGlassNotification != null)
                {
                    // supression du menu
                    DeleteMenu(context, existCommandGlassNotification);
                }
                //sub menu CommandGlassNotification
                SubMenu CommandGlassNotification = context.SubMenus.Where(s => s.SubMenuCode == "CommandGlassNotification").FirstOrDefault();
                if (CommandGlassNotification == null)
                {
                    CommandGlassNotification = new SubMenu()
                    {
                        Menu = existNotificationSMS,
                        MenuID = existNotificationSMS.MenuID,
                        SubMenuCode = "CommandGlassNotification",
                        SubMenuDescription = "CommandGlassNotification",
                        SubMenuLabel = "CommandGlassNotification",
                        SubMenuController = "CommandGlassNotification",
                        SubMenuPath = "Index",
                        AppearanceOrder = 3
                    };
                    context.SubMenus.AddOrUpdate(CommandGlassNotification);
                    context.SaveChanges();
                }

                //creation du menu de BirthdayNotification
                Menu existBirthdayNotification = context.Menus.Where(s => s.MenuCode == "BirthdayNotification").FirstOrDefault();
                if (existBirthdayNotification != null)
                {
                    // supression du menu
                    DeleteMenu(context, existBirthdayNotification);
                }
                //sub menu BirthdayNotification
                SubMenu BirthdayNotification = context.SubMenus.Where(s => s.SubMenuCode == "BirthdayNotification").FirstOrDefault();
                if (BirthdayNotification == null)
                {
                    BirthdayNotification = new SubMenu()
                    {
                        Menu = existNotificationSMS,
                        MenuID = existNotificationSMS.MenuID,
                        SubMenuCode = "BirthdayNotification",
                        SubMenuDescription = "BirthdayNotification",
                        SubMenuLabel = "BirthdayNotification",
                        SubMenuController = "BirthdayNotification",
                        SubMenuPath = "Index",
                        AppearanceOrder = 4
                    };
                    context.SubMenus.AddOrUpdate(BirthdayNotification);
                    context.SaveChanges();
                }


                //CREATION DU MENU PatientReminder DS LE NEW MODULE
                Menu existMnuPatientDelivery = context.Menus.Where(s => s.MenuCode == "PatientReminder").FirstOrDefault();
                if (existMnuPatientDelivery != null)
                {
                    // supression du menu
                    DeleteMenu(context, existMnuPatientDelivery);
                }

                //sub menu PatientReminder
                SubMenu PatientReminder = context.SubMenus.Where(s => s.SubMenuCode == "PatientReminder").FirstOrDefault();
                if (PatientReminder == null)
                {
                    PatientReminder = new SubMenu()
                    {
                        Menu = existNotificationSMS,
                        MenuID = existNotificationSMS.MenuID,
                        SubMenuCode = "PatientReminder",
                        SubMenuDescription = "PatientReminder",
                        SubMenuLabel = "PatientReminder",
                        SubMenuController = "PatientReminder",
                        SubMenuPath = "Index",
                        AppearanceOrder = 5
                    };
                    context.SubMenus.AddOrUpdate(PatientReminder);
                    context.SaveChanges();
                }


                SubMenu EvenementSMS = context.SubMenus.Where(s => s.SubMenuCode == "EvenementSMS").FirstOrDefault();
                if (EvenementSMS == null)
                {
                    EvenementSMS = new SubMenu()
                    {
                        Menu = existNotificationSMS,
                        MenuID = existNotificationSMS.MenuID,
                        SubMenuCode = "EvenementSMS",
                        SubMenuDescription = "EvenementSMS",
                        SubMenuLabel = "EvenementSMS",
                        SubMenuController = "EvenementSMS",
                        SubMenuPath = "Index",
                        AppearanceOrder = 6
                    };
                    context.SubMenus.AddOrUpdate(EvenementSMS);
                    context.SaveChanges();
                }

            }
            /******************end notification **************************/

            //menu sms parameter
            Menu existParamSMS = context.Menus.Where(s => s.MenuCode == "ParamSMS").FirstOrDefault();
            if (existParamSMS == null)
            {
                Menu ParamSMS = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = "ParamSMS",
                    MenuController = null,
                    MenuDescription = "Menu Param SMS",
                    MenuFlat = false,
                    MenuIconName = "ParamSMS",
                    MenuLabel = "ParamSMS",
                    MenuPath = "",
                    MenuState = true,
                    ModuleID = SMSMNGModule.ModuleID,
                    AppearanceOrder = 2
                };
                context.Menus.AddOrUpdate(ParamSMS);
                context.SaveChanges();
                existParamSMS = ParamSMS;
            }

            if (existParamSMS == null)
            {
                existParamSMS = context.Menus.Where(s => s.MenuCode == "ParamSMS").FirstOrDefault();
            }

            if (existParamSMS != null)
            {
                //suppression du menu de NotificationSetting
                Menu existNotificationSetting = context.Menus.Where(s => s.MenuCode == "NotificationSetting").FirstOrDefault();
                if (existNotificationSetting != null)
                {
                    //supression du menu
                    List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == existNotificationSetting.MenuID).ToList();
                    context.ActionMenuProfiles.RemoveRange(lstActionMP);
                    context.Menus.Remove(existNotificationSetting);
                    context.SaveChanges();
                }

                //sub menu NotificationSetting
                SubMenu NotificationSetting = context.SubMenus.Where(s => s.SubMenuCode == "NotificationSetting").FirstOrDefault();
                if (NotificationSetting == null)
                {
                    NotificationSetting = new SubMenu()
                    {
                        Menu = existParamSMS,
                        MenuID = existParamSMS.MenuID,
                        SubMenuCode = "NotificationSetting",
                        SubMenuDescription = "NotificationSetting",
                        SubMenuLabel = "NotificationSetting",
                        SubMenuController = "NotificationSetting",
                        SubMenuPath = "Index",
                        AppearanceOrder=0
                    };
                    context.SubMenus.AddOrUpdate(NotificationSetting);
                    context.SaveChanges();
                }

                
            }

            #endregion

        }

        private void DeleteMenu (EFDbContext context,Menu exitMenu)
        {
            //supression du menu
            List<ActionMenuProfile> lstActionMP = context.ActionMenuProfiles.Where(am => am.MenuID == exitMenu.MenuID).ToList();
            context.ActionMenuProfiles.RemoveRange(lstActionMP);
            context.Menus.Remove(exitMenu);
            context.SaveChanges();
        }

        private void DeleteSubMenu(EFDbContext context, SubMenu exitSubMenu)
        {
            //supression du menu
            List<ActionSubMenuProfile> lstActionMP = context.ActionSubMenuProfiles.Where(am => am.SubMenuID == exitSubMenu.SubMenuID).ToList();
            context.ActionSubMenuProfiles.RemoveRange(lstActionMP);
            context.SubMenus.Remove(exitSubMenu);
            context.SaveChanges();
        }

        private void CreateProductTransfertMenuAndSubMenus(EFDbContext context, Module supplyModule)
        {
            var productTransfertMenu = context.Menus.Where(s => s.MenuCode == CodeValue.Supply.PRODUCT_TRANSFERT).FirstOrDefault();

            if (productTransfertMenu == null)
            {
                productTransfertMenu = new Menu()
                {
                    IsChortcut = false,
                    MenuCode = CodeValue.Supply.PRODUCT_TRANSFERT,
                    MenuController = CodeValue.Supply.PRODUCT_TRANSFERT,
                    MenuDescription = CodeValue.Supply.PRODUCT_TRANSFERT,
                    MenuFlat = false,
                    MenuIconName = CodeValue.Supply.PRODUCT_TRANSFERT,
                    MenuLabel = CodeValue.Supply.PRODUCT_TRANSFERT,
                    MenuPath = "Index",
                    MenuState = true,
                    ModuleID = supplyModule.ModuleID
                };
                context.Menus.AddOrUpdate(productTransfertMenu);
                context.SaveChanges();
            }

            #region Product Sending Sub Menu
            SubMenu productSendingSM = context.SubMenus.Where(s => s.SubMenuCode == CodeValue.Supply.PRODUCT_TRANSFERT_SENDING).FirstOrDefault();
            if (productSendingSM == null)
            {
                productSendingSM = new SubMenu()
                {
                    MenuID = productTransfertMenu.MenuID,
                    SubMenuCode = CodeValue.Supply.PRODUCT_TRANSFERT_SENDING,
                    SubMenuDescription = CodeValue.Supply.PRODUCT_TRANSFERT_SENDING,
                    SubMenuLabel = CodeValue.Supply.PRODUCT_TRANSFERT_SENDING,
                    SubMenuController = CodeValue.Supply.PRODUCT_TRANSFERT_SENDING,
                    SubMenuPath = "Index",
                    AppearanceOrder = 1
                };
                context.SubMenus.AddOrUpdate(productSendingSM);
                context.SaveChanges();
            }
            #endregion

            #region Product Sending Sub Menu
            SubMenu productReceivingSM = context.SubMenus.Where(s => s.SubMenuCode == CodeValue.Supply.PRODUCT_TRANSFERT_RECEIVING).FirstOrDefault();
            if (productReceivingSM == null)
            {
                productReceivingSM = new SubMenu()
                {
                    MenuID = productTransfertMenu.MenuID,
                    SubMenuCode = CodeValue.Supply.PRODUCT_TRANSFERT_RECEIVING,
                    SubMenuDescription = CodeValue.Supply.PRODUCT_TRANSFERT_RECEIVING,
                    SubMenuLabel = CodeValue.Supply.PRODUCT_TRANSFERT_RECEIVING,
                    SubMenuController = CodeValue.Supply.PRODUCT_TRANSFERT_RECEIVING,
                    SubMenuPath = "Index",
                    AppearanceOrder = 2
                };
                context.SubMenus.AddOrUpdate(productReceivingSM);
                context.SaveChanges();
            }
            #endregion

            #region SECURITY USER 

            //SuspendedUser
            Menu existSuspendedUser = context.Menus.Where(s => s.MenuCode == "SuspendedUser").FirstOrDefault();
            if (existSuspendedUser == null)
            {
                Menu SuspendedUser = new Menu()
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
                context.Menus.AddOrUpdate(SuspendedUser);
                context.SaveChanges();
            }


            #endregion

            
        }

        void AddBannedNumbers(EFDbContext context)
        {
            #region Wamba Orange
            var wambaOrange = context.BannedNumbers.SingleOrDefault(n => n.Number == "691491729");
            if (wambaOrange == null)
            {
                wambaOrange = new BannedNumber()
                {
                    Number = "691491729"
                };
                context.BannedNumbers.Add(wambaOrange);
                context.SaveChanges();
            }
            #endregion

            #region Wamba MTN
            var wambaMTN = context.BannedNumbers.SingleOrDefault(n => n.Number == "675488938");
            if (wambaMTN == null)
            {
                wambaMTN = new BannedNumber()
                {
                    Number = "675488938"
                };
                context.BannedNumbers.Add(wambaMTN);
                context.SaveChanges();
            }
            #endregion

            #region Charly Orange
            var charlyOrange = context.BannedNumbers.SingleOrDefault(n => n.Number == "690289009");
            if (charlyOrange == null)
            {
                charlyOrange = new BannedNumber()
                {
                    Number = "690289009"
                };
                context.BannedNumbers.Add(charlyOrange);
                context.SaveChanges();
            }
            #endregion

            #region Yolande Orange
            var yolandeOrange = context.BannedNumbers.SingleOrDefault(n => n.Number == "694129783");
            if (yolandeOrange == null)
            {
                yolandeOrange = new BannedNumber()
                {
                    Number = "694129783"
                };
                context.BannedNumbers.Add(yolandeOrange);
                context.SaveChanges();
            }
            #endregion
            // YolandeOrange
        }

        #region Module Ordering
        public void ModuleOrdering(EFDbContext context)
        {
            // return;
            var modules = context.Modules;
            modules.SingleOrDefault(m => m.ModuleCode == CodeValue.CRM.Code).AppearanceOrder = 1;
            //modules.SingleOrDefault(m => m.ModuleCode == CodeValue.CRM.Practician).AppearanceOrder = 2;
            //modules.SingleOrDefault(m => m.ModuleCode == CodeValue.CashRegister.Sales).AppearanceOrder = 3;
            // Insured Management
            modules.SingleOrDefault(m => m.ModuleCode == "MODULE_1").AppearanceOrder = 4;
            modules.SingleOrDefault(m => m.ModuleCode == "MODULE_3").AppearanceOrder = 5;
            //modules.SingleOrDefault(m => m.ModuleCode == CodeValue.CRM.OrderingDesk).AppearanceOrder = 6;
            //modules.SingleOrDefault(m => m.ModuleCode == CodeValue.CRM.Workshop).AppearanceOrder = 7;
            //modules.SingleOrDefault(m => m.ModuleCode == CodeValue.CRM.DeliveryDesk).AppearanceOrder = 8;
            //modules.SingleOrDefault(m => m.ModuleCode == CodeValue.CRM.Notification).AppearanceOrder = 9;
            //modules.SingleOrDefault(m => m.ModuleCode == CodeValue.CRM.CustomerRelation).AppearanceOrder = 10;
            modules.SingleOrDefault(m => m.ModuleCode == CodeValue.CCM.CODE).AppearanceOrder = 11;
            modules.SingleOrDefault(m => m.ModuleCode == "MODULE_2").AppearanceOrder = 12;
            //modules.SingleOrDefault(m => m.ModuleCode == CodeValue.CRM.Marketing).AppearanceOrder = 13;
            //modules.SingleOrDefault(m => m.ModuleCode == "Budget").AppearanceOrder = 14;
            modules.SingleOrDefault(m => m.ModuleCode == "BarCode").AppearanceOrder = 15;
            modules.SingleOrDefault(m => m.ModuleCode == "MODULE_4").AppearanceOrder = 16;
            modules.SingleOrDefault(m => m.ModuleCode == "MODULE_5").AppearanceOrder = 17;
            modules.SingleOrDefault(m => m.ModuleCode == "MODULE_6").AppearanceOrder = 18;

            context.SaveChanges();

        }
        #endregion
    }
}
