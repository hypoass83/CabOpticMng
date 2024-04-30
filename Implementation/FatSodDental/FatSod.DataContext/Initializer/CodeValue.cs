namespace FatSod.DataContext.Initializer
{
    /// <summary>
    /// CodeValue
    /// </summary>
    public static class CodeValue
    {
        /// <summary>
        /// CRM
        /// </summary>
        /// 
        public static class CRM
        {
            public static class DeliverDesk
            {
                public const string Code = "DeliverDesk";
            }
            public static class OrderRXLenses
            {
                public const string Code = "OrderRXLenses";
            }
            public static class OrderStockOrderLenses
            {
                public const string Code = "OrderStockOrderLenses";
            }
            public static class SpecialOrderReception
            {
                public const string Code = "SpecialOrderReception";
            }
            public static class StockOrderReception
            {
                public const string Code = "StockOrderReception";
            }

            public static class RXLensesRpt
            {
                public const string Code = "RXLensesRpt";
            }

            public static class StockOrderRpt
            {
                public const string Code = "StockOrderRpt";
            }
            public static class LensMounting
            {
                public const string Code = "LensMounting";
            }
            public static class LensMountingDamage
            {
                public const string Code = "LensMountingDamage";
            }
            public static class Prescriptionharmonisation
            {
                public const string Code = "PrescriptionHarmonisation";
            }
            public static class RendezVous
            {
                public const string CODE = "RDV";
                public const string ConsultRDV = "ConsultRDV";
            }
            public static class AlertSMS
            {
                public const string EvenementSMS = "EvenementSMS";
                public const string RappelRdvSMS = "RappelRdvSMS";
                public const string RappelGeneralSMS = "RappelGeneralSMS";
            }
            public static class CustomerJourney
            {
                public const string CODE = "CustomerJourney";
            }
            public static class SpecialOrder
            {
                public const string PostSpecialOrder = "PostSpecialOrder";
                public const string ReceiveSpecialOrder = "ReceiveSpecialOrder";
                public const string DeliverSpecialOrder = "DeliverSpecialOrder";
            }

            public static class PatientReminder
            {
                public const string CODE = "PatientReminder";
            }
            public const string Code = "CRM";
            public const string NoPurchase = "NoPurchase";
            public const string CustomerSatisfaction = "CustomerSatisfaction";
            public const string DeliveryNotification = "DeliveryNotification";
            public const string InsuranceDeliveryNotification = "InsuranceDeliveryNotification";
            public const string NotificationSetting = "NotificationSetting";
            public const string PurchaseNotification = "PurchaseNotification";

        }
        
        public static class CCM
        {
            public const string CODE = "CCM";
            public static class ComplaintFeedBack
            {
                public const string CODE = "CCM_FB";
            }
            public static class FeedBackRpt
            {
                public const string CODE = "CCM_RPT_FB";
            }
            public static class ComplaintRpt
            {
                public const string CODE = "CCM_RPT_C";
            }

            public static class ComplaintRegistration
            {
                public const string CODE = "CCM_REG";
            }

            public static class ComplaintResolution
            {
                public const string CODE = "CCM_RES";
            }

            public static class ComplaintControlled
            {
                public const string CODE = "CCM_CTRL";
            }
        }
        
        /// <summary>
        /// Security
        /// </summary>
        public static class Security
        {
            /// <summary>
            /// Profile
            /// </summary>
            public static class Profile
            {
                /// <summary>
                /// Profile CODE
                /// </summary>
                public const string CODE = "PRC1";
                public const string ClASS_CODE = "EplC";
                public const string SUPER_ADMIN_PROFILE = "Super-Admin-FSInventory";
            }
            /// <summary>
            /// User
            /// </summary>
            public static class User
            {
                /// <summary>
                /// User CODE
                /// </summary>
                public const string CODE = "SUSR";
                public const string ProAvancedCODE = "APRF1";
                public const string SuspendedUser = "SuspendedUser";
                //public const string MenuCODE = "USR1";
            }

        }
        public static class Parameter
        {
            public const string CompanyJobCode = "CompJobCode";
            public const string CompanyCode = "CompCode";
            public const string JobCode = "JobCode";
            public const string COUNTRYCODE = "COUNTRYCODE";
            public const string REGIONCode = "REGIONCode";
            public const string TOWNCode = "TOWNCode";
            public const string QTER = "QTER";
            public static class Localization
            {
                public const string MenuCODE = "LOC";
            }
            public static class Branch
            {
                public const string MenuCODE = "ZAGC";
                public const string DlaBranchName = "DBOY Douala";
                public const string YdeBranchName = "DBOY Yaoundé";
            }
            public static class MoneyManagement
            {
                public const string MENUCODE = "MENUMONEYCODE";
                public const string BankCODE = "BKCODE";
                public const string TillCODE = "TLCODE";
                public const string OTCODE = "OTCODE";
            }
            public static class Budget
            {
                public const string CODE = "mnuBudget";
                public const string LABEL = "Budget";
                public const string DESCRIPTION = "Parametrage du Budget";
                public const string ICON_NAME = "budget.png";
                public const string CONTROLLER = "Null";
                public const string PATH = "Null";
                public const bool FLAT = true;
                public const bool STATE = true;
                //sub menu
                public const string SubMenuCODEFY = "submnu1FiscalYear";
                public const string SubMenuCODEBL = "submnu2BudgetLine";
                public const string SubMenuCODEBA = "submnu3BudgetAllocated";
                public const string SubMenuCODEBAU = "submnu4BudgetAllocatedUpdates";
                
                public static class Sale{
                    public const string SaleReceipt = "SaleReceipt";
                }
            }

        }
        //** Sale Code Values
        public static class Sale
        {
            public static class Consultation
            {
                public const string CODE = "Consultation";
            }
            public static class Customer
            {
                public const string CODE = "CLT";
                public const string CODECLT = "NEWCLT";
                public const string GENERALPUBLICNUMBER = "OPT000022";
            }
            public static class CustomerUpdate
            {
                public const string CODE = "MenuCustomerUpdate";
            }
            public static class Assurance
            {
                public const string CODE = "MenuAssurance";
                public const string LieuxDepotBill = "LieuxDepotBill";
                public const string InsuredCompany = "InsuredCompany";
            }
            public static class NewSale
            {
                public const string CODE = "SAL";
                public const string C_CODE = "NEWCOMMAND";
                public const string S_CODE = "NEWSAL";
                public const string OtherSale = "OtherSale";
                public const string Proforma = "MenuProforma";
                public const string SaleToCustomer = "SaleToCustomer";
                public const string ValideProforma = "MenuValideProforma";
                public const string R_CODE = "RTN";
                public const string OrderLensOrder = "OrderLensOrder";
                public const string Menu_SaleSpecialOrder = "SaleSpecialOrder";
                public const string ReceiveSpecialOrder = "ReceiveSpecialOrder";
                public const string PostedSpecialOrderToSupplier = "PostedToSupplier";
                public const string Pricing = "Pricing";
                public const string RangeNumber = "a_NumberRange";
                public const string Price = "b_Price";
                public const string DIRECTBILL = "MnuVDIRECTBILL";
                
            }
            public static class SalePaymentMode
            {
                public const string CODE = "PYMODE";
            }
            /// <summary>
            /// 
            /// </summary>
            public static class Report
            {
                public const string CODE = "SMnuSalRpt";
                public const string SMCODECLT = "SubMnu1ListClient";
                public const string SMCODECUSTHIST = "SubMnu2HistCusto";
                public const string SRCODERETSALE = "SubMnu3RetSale";
                public const string RptValideBill = "RptValideBill";
                public const string RptRegularizeBill = "RptRegularizeBill";
                public const string RptDeletedBill = "RptDeletedBill";
                public const string RptValidatedBDF = "RptValidatedBDF";
                public const string RptProformaLst = "RptProformaLst";
            }
        }
        public static class CashRegister
        {
            public const string MODULE_CODE = "MODULE_3";
            public const string MENU_OPEN_CODE = "A_OPEN_CODE_CASH";
            public const string MENU_VALIDATE_PURCHASE = "B_VAL_PURCHASE_COD_CASH";
            public const string MENU_VALIDATE_SALE = "C_VAL_SALE_COD_CASH";
            public const string MENU_SALE_RECEIPT = "SaleReceipt";
            public const string MENU_DEPOSIT_RECEIPT = "PrintDeposit";
            public const string MENU_CLOSE_CODE = "Y_CLOSE_COD_CASH";
            public const string MENU_STATE_CASH = "Z_VAL_STATE_CASH";
            public const string SUBMENU_DEPOT_ASSURE = "DepotStockLenses";
            public const string SUBMENU_DEPOT_NON_ASSURE = "DepotSpecialOrder";
            public const string SUBMENU_STATE_CASH_BILL = "A_STATE_CASH_BILL";
            public const string SUBMENU_CASHTRANSMISSION = "A_CashTransmission";
            public const string SUBMENU_CASHRECEPTION = "B_CASHRECEPTION";
            public const string SUBMENU_STATE_CASH_HISOTRIC = "B_STATE_CASH_HISTORIC";
            public const string SUBMENU_BANKTRANSMISSION = "C_BankTransmission";
            public const string SUBMENU_BANKRECEPTION = "D_BankReception";
            public const string CODEBudgetExpense = "E_BudgetExpense";
            public const string CODETreasury = "E_CODETreasurye";
            public const string Menu_CashierSpecialOrder = "D_CashierSpecialOrder";
            public const string ValidateSpecialOrder = "A_ValidateSpecialOrder";
            public const string DeliverSpecialOrder = "B_DeliverSpecialOrder";

            public const string CODETILLADJUST = "E_TILLADJUST";
            public const string DisplayFacture = "DisplayFacture";
            public const string DepotInsured = "DepotInsured";

            public const string RptBankHistMenu = "RptBankHistMenu";

            public const string AuthorizeSale = "AuthorizeSale";
            public const string AuthorizePrescriptionSale = "AuthorizePrescriptionSale";
            public const string ValidatedSale = "ValidatedSale";

            public const string CommandOtherSale = "CommandOtherSale";
            public const string ValideOtherSale = "ValideOtherSale";

            public const string ValideBorderoDepotFacture = "ValideBorderoDepotFacture";
            public const string BorderoDepotFacture = "BorderoDepotFacture";

            public const string CommandDilatation = "CommandDilatation";
            public const string ForceOpenTeller = "ForceOpenTeller";
        }

        //Accounting code menu
        /// <summary>
        /// classe de definition des code menu pour le module Accounting
        /// </summary>
        public static class Accounting
        {
            public const string ACCOUNTNAME = "Account Name";
            public const string ACCOUNTCOLLECTIF = "Collectif Account";
            public static class ParamInitAcct
            {
                public const double VATRATE = 0d;
                public const double DISCOUNTRATE = 0d;
                public const string Recapproduct = "RECAP";
            }

            public static class AccountingSection
            {
                public const string CODE = "mnuAcct1";
            }
            public static class CollectifAccount
            {
                public const string CODE = "mnuAcct11";
            }
            public static class Account
            {
                public const string CODE = "mnuAcct2";
            }
            public static class Operation
            {
                public const string CODE = "mnuAcct3";
            }
            public static class AccountingTask
            {
                public const string CODE = "mnuAcct4";
            }
            
            public static class Journal
            {
                public const string CODE = "mnuAcct5";
            }
            public static class AccountOperation
            {
                public const string CODE = "mnuAcct6";
            }
            public static class BudgetConsume
            {
                public const string CODEAUTHBUDCONSUME = "mnuAcct66";
            }
            public static class TreasuryOperation
            {
                public const string TranferToTeller = "TranferToTeller";
                public const string TransfertToBank = "TransfertToBank";
                public const string ReceiveFromTeller = "ReceiveFromTeller";
                public const string ReceiveFromBank = "ReceiveFromBank";
            }
            public static class Report
            {
                public const string CODE = "mnuAcct7";
                public const string CODESUBMENU1 = "submnuAcct7rpt1";
                public const string CODESUBMENU2 = "submnuAcct7rpt2";
                public const string CODESUBMENU3 = "submnuAcct7rpt3";
                public const string CODESUBMENU4 = "submnuAcct7rpt4";
                
            }

            public static class AccountOperation_singleEntry
            {
                public const string CODE = "submnuAcct1";
            }
            public static class AccountOperation_multiEntries
            {
                public const string CODE = "submnuAcct2";
            }
            public static class DefaultCodeAccountingSection
            {
                //Mode de Paiement lors de la vente
                public const string Credit = "Credit";
                //Produit ou Marchandise
                public const string CODEPROD = "PROD";
                //Fournisseur
                public const string CODEFOURN = "SUPP";
                //Client
                public const string CODECLIENT = "CUST";
                //ssurance
                public const string CODEASSUR = "ASSU";
                //Banque
                public const string CODEBANK = "BANK";
                public const string DIGITAL_PAYMENT = "DIGITAL PAYMENT";
                //Caisse
                public const string CODECAIS = "TILL";
                //postal check
                public const string CODEPOSTCHECK = "PCHK";
                //TVACOLLECTE
                public const string CODETVACOLECT = "CVAT";
                //TVA DEDUC
                public const string CODETVADEDUC = "DVAT";
                //Transport
                public const string CODESALETRANSP = "STRA";
                public const string CODEPURCTRANSP = "PTRA";
                //Transport Retour vente
                public const string CODERETSALETRANSP = "SRTR";
                //RemiseAchat
                public const string CODEESCPTEACHAT = "PESC";
                //RemiseVente
                public const string CODEESCPTEVENTE = "SESC";
                //ACHAT M'SE
                public const string CODEACHATMSE = "PFOO";
                //VTE M'SE
                public const string CODEVENTEMSE = "SFOO";
                //STOCK VAR
                public const string CODESTOCKVAR = "SVAR";
                //avance sur achat
                public const string CODEAVANCEACHAT = "PADV";
                //avance sur vente
                public const string CODEAVANCEVENTE = "SADV";
            }
            public static class InitOperationType
            {
                //VENTE
                public const string CODESALE = "SALE";
                //ACHAT
                public const string CODEPURCHASE = "PURCHASE";
                //RETOUR VENTE
                public const string CODESALERETURN = "RETURNSALE";
                //RETOUR ACHAT
                public const string CODEPURCHASERETURN = "RETURNPURCHASE";
                //MANUAL OPERATION
                public const string CODEMANUAL = "MANUAL";
                //entre en stock
                public const string CODESTOCKINPUT = "STOCKINPUT";
                //sortie en stock
                public const string CODESTOCKOUTPUT = "STOCKOUTPUT";
                //depreciation en stock
                public const string CODESTOCKDEPRECIATE = "STOCKDEPRECIATE";
                //BUDGET
                public const string CODEBUDGET = "BUDGET";
                //SHORTAGE
                public const string CODETELLERADJUST = "TELLERADJUST";
                //OVERAGE
                public const string CODEOVERAGE = "OVERAGE";
                //TREASURY OP
                public const string CODETREASURY = "TREASURYOPERATION";
            }
            //INIT MACRO OPERATION
            public static class InitMacroOperation
            {
                public const string CODEDELIVERY = "DELIVERY";
                public const string CODEBILLING = "BILLING";
                public const string CODEPAYMENT = "PAYMENT";
                public const string CODEMANUAL = "MANUAL";
                public const string CODECERTIFY = "CERTIFY";
                public const string CODEADVANCED = "ADVANCED";
            }
            //INIT TYPE REGLEMENT
            public static class InitReglementType
            {
                public const string CODEAUCUN = "NONE";
                public const string CODEBANK = "BANK";
                public const string CODECASH = "CASH";
            }

            /// <summary>
            /// INIT OPERATION
            /// </summary>
            public static class InitOperation
            {
                //MANUAL
                public const string CODEMANUALOP = InitMacroOperation.CODEMANUAL;
                //VENTE
                public const string CODESALEDELIVERY = InitMacroOperation.CODEDELIVERY + "-" + InitOperationType.CODESALE;
                public const string CODESALEBILLING = InitMacroOperation.CODEBILLING + "-" + InitOperationType.CODESALE;
                //avance avec livraison
                public const string CODECASHADVANCEDSALEDELIVERY = InitMacroOperation.CODEADVANCED + "-" + InitReglementType.CODECASH+ "-" + InitMacroOperation.CODEDELIVERY + "-" + InitOperationType.CODESALE;
                public const string CODEBANKADVANCEDSALEDELIVERY = InitMacroOperation.CODEADVANCED + "-" + InitReglementType.CODEBANK + "-" + InitMacroOperation.CODEDELIVERY + "-" + InitOperationType.CODESALE;

                //ADVANCED SALE
                public const string CODECASHSALEADVANCEDPAYMENT = InitMacroOperation.CODEADVANCED + "-" + InitReglementType.CODECASH + "-" + InitOperationType.CODESALE;
                public const string CODEBANKSALEADVANCEDPAYMENT = InitMacroOperation.CODEADVANCED + "-" + InitReglementType.CODEBANK + "-" + InitOperationType.CODESALE;
                //CANCEL ADVANCED SALE
                public const string CODECANCELSALEADVANCEDPAYMENT = "CANCEL" + InitMacroOperation.CODEADVANCED + "-" + InitOperationType.CODESALE;
                //PAYMENT SALE
                public const string CODECASHSALEPAYMENT = InitMacroOperation.CODEPAYMENT + "-" + InitReglementType.CODECASH + "-" + InitOperationType.CODESALE;
                public const string CODEBANKSALEPAYMENT = InitMacroOperation.CODEPAYMENT + "-" + InitReglementType.CODEBANK + "-" + InitOperationType.CODESALE;
                //RETOUR VENTE
                public const string CODECERTIFYRETURNSALE = InitMacroOperation.CODECERTIFY + "-" + InitOperationType.CODESALERETURN;

                //SORTIE EN STOCK
                public const string CODESTOCKOUTPUT = InitMacroOperation.CODECERTIFY + "-" + InitOperationType.CODESTOCKOUTPUT;
                //ENTREE EN STOCK
                public const string CODESTOCKINPUT = InitMacroOperation.CODECERTIFY + "-" + InitOperationType.CODESTOCKINPUT;
                //DEPRECIATION DU STOCK
                public const string CODESTOCKDEPRECIATE = InitMacroOperation.CODECERTIFY + "-" + InitOperationType.CODESTOCKDEPRECIATE;
                
                //ACHAT
                public const string CODEPURCHASEDELIVERY = InitMacroOperation.CODEDELIVERY + "-" + InitOperationType.CODEPURCHASE;
                public const string CODEPURCHASEBILLING = InitMacroOperation.CODEBILLING + "-" + InitOperationType.CODEPURCHASE;
                //ADVANCED PURCHASE
                public const string CODECASHPURCHASEADVANCEDPAYMENT = InitMacroOperation.CODEADVANCED + "-" + InitReglementType.CODECASH + "-" + InitOperationType.CODEPURCHASE;
                public const string CODEBANKPURCHASEADVANCEDPAYMENT = InitMacroOperation.CODEADVANCED + "-" + InitReglementType.CODEBANK + "-" + InitOperationType.CODEPURCHASE;
                //CANCEL ADVANCED PURCHASE
                public const string CODECANCELPURCHASEADVANCEDPAYMENT = "CANCEL" + InitMacroOperation.CODEADVANCED + "-" + InitOperationType.CODEPURCHASE;
                //PAYMENT PURCHASE
                public const string CODECASHPAYMENTPURCHASE = InitMacroOperation.CODEPAYMENT + "-" + InitReglementType.CODECASH + "-" + InitOperationType.CODEPURCHASE;
                public const string CODEBANKPAYMENTPURCHASE = InitMacroOperation.CODEPAYMENT + "-" + InitReglementType.CODEBANK + "-" + InitOperationType.CODEPURCHASE;
                //RETOUR ACHAT
                public const string CODECERTIFYRETURNPURCHASE = InitMacroOperation.CODECERTIFY + "-" + InitOperationType.CODEPURCHASERETURN;
                //BUDGET
                public const string CODEBUDGET = InitMacroOperation.CODECERTIFY + "-" + InitOperationType.CODEBUDGET;
                //SHORTAGE
                public const string CODETELLERADJUST = InitMacroOperation.CODECERTIFY + "-" + InitOperationType.CODETELLERADJUST;
                //OVERAGE
                public const string CODEOVERAGE = InitMacroOperation.CODECERTIFY + "-" + InitOperationType.CODEOVERAGE;
            }

        }

        public static class Supply
        {

            public static string StockNonInsureReserve = "StockNonInsureReserve";

            public static string lensCategoryCode = "Lenses";
            public static string DefaultLensCoating = "DefaultLensCoating";
            public static string DefaultLensColour = "DefaultLensColour";
            public static string DefaultLensOtherCriteria = "DefaultLensOtherCriteria";
            public static string DefaultLensMaterial = "DefaultLensMaterial";
            public static string dlaOrderLensWaitingLocationCode = "dlaOrderLensWaitingLocationCode";
            public static string ydeOrderLensWaitingLocationCode = "ydeOrderLensWaitingLocationCode";
            public static string StockOutput = "StockOutput";
            public static string StockReplacement = "StockReplacement";

            public static class ListOfStockInput
            {
                public const string CODE = "ListOfStockInput";
            }
            public static class RegProductNumber
            {
                public const string CODE = "RegProductNumber";
            }
            public static class StockDamage
            {
                public const string CODE = "StockDamage";
            }
            public static class ProductGift
            {
                public const string CODE = "ProductGift";
            }
            public static class PrintProductTransfert
            {
                public const string CODE = "PrintProductTransfert";
            }
            public static class PrintProductReception
            {
                public const string CODE = "PrintProductReception";
            }
            public static class ProductSaleHistory
            {
                public const string CODE = "ProductSaleHistory";
            }
            public static class GenerateCodeBare
            {
                public const string CODE = "GenerateCodeBare";
            }
            public static class SupplyModule
            {
                public const string CODE = "MODULE_2";
                public const string LABEL = "Supply";
                public const string DESCRIPTION = "Gérer les approvisionnements";
                public const string AREA = "Supply";
                public const string MODULE_PRESSED_IMAGE_PATH = "MOD_PRESSED_IMG_SUPPLY";
                public const string MODULE_IMAGE_PATH = "MOD_IMG_SUPPLY";
                public const string MODULE_DISABLED_IMAGE_PATH = "MOD_DISABLED_IMG_SUPPLY";
                public const bool MODULE_STATE = true;
                public const int MODULE_IMAGE_HEIGHT = 28;
                public const int MODULE_IMAGE_WEIGHT = 60;
            }
            public static class CategoryMenu
            {
                public const string CODE = "a_Category";
                public const string LABEL = "Category";
                public const string DESCRIPTION = "Permet de créer, modifier, consulter et supprimer les catégories de produit de l'entreprise";
                public const string ICON_NAME = "category.png";
                public const string CONTROLLER = "Category";
                public const string PATH = "Categorie";
                public const bool FLAT = true;
                public const bool STATE = true;
            }

            public static class ProductBrandMenu
            {
                public const string CODE = "ProductBrand";
            }

            public static class LensCategorySM
            {
                public const string CODE = "b_LensCategory";
                public const string LABEL = "Lens Category";
                public const string DESCRIPTION = "Permet de créer, modifier, consulter et supprimer les catégories de verre de produit de l'entreprise";
                public const string CONTROLLER = "LensCategory";
                public const string PATH = "LensCategory";
                public const bool FLAT = true;
                public const bool STATE = true;
            }

            public static class ProductMenu
            {
                public const string CODE = "b_Product";
                public const string LABEL = "Product";
                public const string DESCRIPTION = "Permet de créer, modifier, consulter et supprimer les produits de l'entreprise";
                public const string ICON_NAME = "product.png";
                public const string CONTROLLER = "Null";
                public const string PATH = "Null";
                public const bool FLAT = true;
                public const bool STATE = true;
            }
            public static class StocktMenu
            {
                public const string CODE = "d_Stock";
                public const string LABEL = "Stock";
                public const string DESCRIPTION = "contient les sous menu de gestion des dépots(lieux de stockage), d'entrée en stock et de sortie en stock";
                public const string ICON_NAME = "stock.png";
                public const string CONTROLLER = "Null";
                public const string PATH = "Null";
                public const bool FLAT = true;
                public const bool STATE = true;
            }

            public static class SupplyMenu
            {
                public const string CODE = "e_Purchase";
                public const string LABEL = "Purchase";
                public const string DESCRIPTION = "contient la gestion des achats ";
                public const string ICON_NAME = "supply.png";
                public const string CONTROLLER = "Purchase";
                public const string PATH = "Purchase";
                public const bool FLAT = true;
                public const bool STATE = true;
            }

            public static class SupplierReturnMenu
            {
                public const string CODE = "f_SupplierReturn";
                public const string LABEL = "Supplier Return";
                public const string DESCRIPTION = "contient la gestion des retours de marchandises au fournisseur ";
                public const string ICON_NAME = "supplierReturn.png";
                public const string CONTROLLER = "SupplierReturn";
                public const string PATH = "SupplierReturn";
                public const bool FLAT = true;
                public const bool STATE = true;
            }

            public static class SupplierOrderMenu
            {
                public const string CODE = "g_SupplierOrder";
                public const string LABEL = "Supplier Order";
                public const string DESCRIPTION = "contient la gestion des commandes de marchandises au fournisseur ";
                public const string ICON_NAME = "supplierOrder.png";
                public const string CONTROLLER = "SupplierOrder";
                public const string PATH = "SupplierOrder";
                public const bool FLAT = true;
                public const bool STATE = true;
            }

            public static class SupplierMenu
            {
                public const string CODE = "c_Supplier";
                public const string LABEL = "Supplier";
                public const string DESCRIPTION = "contient la gestion des fournisseurs ";
                public const string ICON_NAME = "supplier.png";
                public const string CONTROLLER = "Supply";
                public const string PATH = "Supply";
                public const bool FLAT = true;
                public const bool STATE = true;
            }

            //menu contenant les critères d'un verre
            public static class LensCriteriaMenu
            {
                public const string CODE = "c_LensCriteria";
                public const string LABEL = "LensCriteria";
                public const string DESCRIPTION = "C'est un menu avec des sous menus. il contient la liste des menus permettant de créer les critères d'un verre ";
                public const string ICON_NAME = "lens.png";
                public const string CONTROLLER = "Null";
                public const string PATH = "Null";
                public const bool FLAT = true;
                public const bool STATE = true;
            }

            //menu contenant les sous menus d'envoi et de reception d'un transfert
            public static class ProductTransfertMenu
            {
                public const string CODE = "e_ProductTransfert";
                public const string LABEL = "ProductTransfert";
                public const string DESCRIPTION = "C'est un menu avec des sous menus. il contient la liste des menus permettant d'envoyer et de receptionner les produits par transfert";
                public const string ICON_NAME = "lens.png";
                public const string CONTROLLER = "Null";
                public const string PATH = "Null";
                public const bool FLAT = true;
                public const bool STATE = true;
            }


            public static class BusinessDayMenu
            {
                public const string CODE = "a_BusinessDay";
                public const string LABEL = "Business Day";
                public const string DESCRIPTION = "Permet de créer, ouvrir, fermer, modifier, consulter et supprimer les Business Day de l'entreprise";
                public const string ICON_NAME = "business.png";
                public const string CONTROLLER = "Null";
                public const string PATH = "Null";
                public const bool FLAT = true;
                public const bool STATE = true;
            }

            public static class DepositMenu
            {
                public const string CODE = "d_Deposit";
                public const string LABEL = "Deposit";
                public const string DESCRIPTION = "Permet de Gérer les dépôts d'argent sur le compte des clients";
                public const string ICON_NAME = "deposit.png";
                public const string CONTROLLER = "Deposit";
                public const string PATH = "Deposit";
                public const bool FLAT = true;
                public const bool STATE = true;
            }

            public static class DepositReason
            {
                public const string SalePayment = "SalePayment";
                public const string SavingDeposit = "SavingDeposit";
                public const string SavingAccount = "SavingAccount";
                public const string SpecialOrderPayment = "SpecialOrderDeposit";
                public const string Insurance = "Insurance";
            }


            public static class LensProduct_SM
            {
                public const string CODE = "a_LensProduct";
                public const string LABEL = "Lens";
                public const string DESCRIPTION = "Permet de créer, modifier, consulter et supprimer les produits lens(verre) ";
                public const string CONTROLLER = "Lens";
                public const string PATH = "Lens";
            }

            public static class GenericProduct_SM
            {
                public const string CODE = "b_GenericProduct";
                public const string LABEL = "Generic Product";
                public const string DESCRIPTION = "Permet de créer, modifier, consulter et supprimer les produits génériques de l'entreprise";
                public const string CONTROLLER = "GenericProduct";
                public const string PATH = "GenericProduct";
            }

            public static class OpenBD_SM
            {
                public const string CODE = "a_OpenBD";
                public const string LABEL = "Open Bussiness Day";
                public const string DESCRIPTION = "Permet d'ouvrir une journée de travail ";
                public const string CONTROLLER = "OpenBD";
                public const string PATH = "OpenBD";
            }

            public static class CloseBD_SM
            {
                public const string CODE = "b_CloseBD";
                public const string LABEL = "Close a Bussiness Day";
                public const string DESCRIPTION = "Permet de fermer une journée de travail ";
                public const string CONTROLLER = "CloseBD";
                public const string PATH = "CloseBD";
            }

            public static class ClosingDayTask_SM
            {
                public const string CODE = "c_ClosingDayTask";
                public const string LABEL = "Choose Closing Day Tasks";
                public const string DESCRIPTION = "il faut choisir les tâches de fermeture d'une journée ";
                public const string CONTROLLER = "ClosingDayTask";
                public const string PATH = "ClosingDayTask";
            }

            public static class Location_SM
            {
                public const string CODE = "a_Location";
                public const string LABEL = "Location";
                public const string DESCRIPTION = "Permet créer les lieux de stockage des produits";
                public const string CONTROLLER = "Location";
                public const string PATH = "Location";
            }

            public static class InventoryDirectory_SM
            {
                public const string CODE = "b_InventoryDirectory";
                public const string LABEL = "Inventory Directory";
                public const string DESCRIPTION = "Permet la gestion des dossiers d'inventaire";
                public const string CONTROLLER = "InventoryDirectory";
                public const string PATH = "InventoryDirectory";
            }

            public const string PRODUCT_TRANSFERT = "ProductTransfert";
            public const string PRODUCT_TRANSFERT_SENDING = "ProductSending";
            public const string PRODUCT_TRANSFERT_RECEIVING = "ProductReceiving";

            public const string NonDepositedBordero = "NonDepositedBordero";
            public const string DepositedBordero = "DepositedBordero";


            public static class InventoryEntry_SM
            {
                public const string CODE = "c_InventoryEntry";
                public const string LABEL = "Inventory Entry";
                public const string DESCRIPTION = "Permet faire la saisie d'inventaire";
                public const string CONTROLLER = "InventoryEntry";
                public const string PATH = "InventoryEntry";
            }

            public static class Inventory_SM
            {
                public const string CODE = "d_Inventory";
                public const string LABEL = "Inventory";
                public const string DESCRIPTION = "Permet faire la correspondance entre les chiffres physiques et ceux du système";
                public const string CONTROLLER = "Inventory";
                public const string PATH = "Inventory";
            }

            public static class LensMaterial_SM
            {
                public const string CODE = "a_LensMaterial";
                public const string LABEL = "LensMaterial";
                public const string DESCRIPTION = "Permet créer les matières; critère d'un verre optique";
                public const string CONTROLLER = "LensMaterial";
                public const string PATH = "LensMaterial";
            }

            public static class LensCoating_SM
            {
                public const string CODE = "b_LensCoating";
                public const string LABEL = "LensCoating";
                public const string DESCRIPTION = "Permet créer les traitements(AR) qu'on peut éffectuer sur un verre";
                public const string CONTROLLER = "LensCoating";
                public const string PATH = "LensCoating";
            }

            public static class LensColour_SM
            {
                public const string CODE = "c_LensColour";
                public const string LABEL = "LensColour";
                public const string DESCRIPTION = "Permet créer les couleurs(White, Blanc) d'un verre";
                public const string CONTROLLER = "LensColour";
                public const string PATH = "LensColour";
            }

            public static class LensNumber_SM
            {
                public const string CODE = "d_LensNumber";
                public const string LABEL = "LensNumber";
                public const string DESCRIPTION = "Permet créer les numéro(cylindrique(+0.5), sphérique(+4.5), asymétrique(+0.5, +4.5) ) d'un verre";
                public const string CONTROLLER = "LensNumber";
                public const string PATH = "LensNumber";
            }

            public static class ProductTransmission_SM
            {
                public const string CODE = "a_ProductTransmission";
                public const string LABEL = "ProductTransmission";
                public const string DESCRIPTION = "Permet, dans le processus de transfert, d'envoyer les produits d'un lieux de stockage vers l'autre";
                public const string CONTROLLER = "ProductTransmission";
                public const string PATH = "ProductTransmission";
            }

            public static class ProductReception_SM
            {
                public const string CODE = "b_ProductReception";
                public const string LABEL = "ProductReception";
                public const string DESCRIPTION = "Permet, dans le processus de transfert, de receptionner les produits envoyés d'un lieux de stockage vers l'autre";
                public const string CONTROLLER = "ProductReception";
                public const string PATH = "ProductReception";
            }
            public static class ProductReception_BR
            {
                public const string CODE = "b_ProductReception_BR";
                public const string LABEL = "ProductReceptionBR";
                public const string DESCRIPTION = "Permet, dans le processus de transfert, de receptionner les produits envoyés d'un lieux de stockage vers l'autre";
                public const string CONTROLLER = "ProductReceptionBR";
                public const string PATH = "ProductReceptionBR";
            }

        }
    }

}

