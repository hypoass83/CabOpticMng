using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSod.Budget.Entities;

namespace FatSod.DataContext.Initializer
{
    internal static partial class Parameters
    {
        //********** Accounting Module
        private static Module _Accounting = new Module()
        {
            ModuleCode = "MODULE_4",
            ModuleLabel = "Comptabilite",
            ModuleDescription = "Module de comptabilite",
            ModuleArea = "Accounting",
            ModuleState = true,
            ModuleImageHeight = 28,
            ModuleImageWeight = 60,
            ModulePressedImagePath = "MOD_PRESSED_IMG_ACC",
            ModuleImagePath = "MOD_IMG_ACC",
            ModuleDisabledImagePath = "MOD_DISABLED_IMG_ACC"
        };

        //********** Accounting Menu
        //AccountingSection
        private static Menu _Accounting_AccountingSection = new Menu()
        {
            MenuLabel = "Accounting Section",
            MenuCode = CodeValue.Accounting.AccountingSection.CODE,
            MenuDescription = "Chapitre Comptable",
            MenuFlat = true,
            MenuIconName = "accountingSection.png",
            MenuController = "CollectifAccount",
            MenuPath = "AccountingSection",
            MenuState = true,
            Module = _Accounting,
            ModuleID = _Accounting.ModuleID
        };
        private static Menu _Accounting_CollectifAccount = new Menu()
        {
            MenuLabel = "CollectifAccount",
            MenuCode = CodeValue.Accounting.CollectifAccount.CODE,
            MenuDescription = "Creation des Comptes Collectifs",
            MenuFlat = true,
            MenuIconName = "account.png",
            MenuController = "CollectifAccount",
            MenuPath = "CollectifAccount",
            MenuState = true,
            Module = _Accounting,
            ModuleID = _Accounting.ModuleID
        };
        private static Menu _Accounting_Account = new Menu()
        {
            MenuLabel = "Account",
            MenuCode = CodeValue.Accounting.Account.CODE,
            MenuDescription = "Creation des Comptes du GL",
            MenuFlat = true,
            MenuIconName = "account.png",
            MenuController = "Account",
            MenuPath = "Account",
            MenuState = true,
            Module = _Accounting,
            ModuleID = _Accounting.ModuleID
        };
        private static Menu _Accounting_AccountingTask = new Menu()
        {
            MenuLabel = "Accounting Task",
            MenuCode = CodeValue.Accounting.AccountingTask.CODE,
            MenuDescription = "Définir les taches d'une operation comptable",
            MenuFlat = true,
            MenuIconName = "operation.png",
            MenuController = "AccountingTask",
            MenuPath = "Index",
            MenuState = true,
            Module = _Accounting,
            ModuleID = _Accounting.ModuleID
        };

        private static Menu _Accounting_OperationType = new Menu()
        {
            MenuLabel = "Define Operation",
            MenuCode = CodeValue.Accounting.Operation.CODE,
            MenuDescription = "Définir les operations comptable",
            MenuFlat = true,
            MenuIconName = "operationType.png",
            MenuController = "Operation",
            MenuPath = "Index",
            MenuState = true,
            Module = _Accounting,
            ModuleID = _Accounting.ModuleID
        };

        private static Menu _Accounting_AccountOperation = new Menu()
        {
            MenuLabel = "Accounting Enties",
            MenuCode = CodeValue.Accounting.AccountOperation.CODE,
            MenuDescription = "Ecriture des operations comptables",
            MenuFlat = true,
            MenuIconName = "accountEntries.png",
            MenuController = "AccountOperation",
            MenuPath = "Index",
            MenuState = true,
            Module = _Accounting,
            ModuleID = _Accounting.ModuleID
        };
        private static Menu _AuhoriseBudgetConsumtion = new Menu()
        {
            MenuLabel = "Authorise Budget Consumtion",
            MenuCode = CodeValue.Accounting.BudgetConsume.CODEAUTHBUDCONSUME,
            MenuDescription = "Authorise Budget Consumtion",
            MenuFlat = true,
            MenuIconName = "budget.png",
            MenuController = "AuthBudgetConsumtion",
            MenuPath = "Index",
            MenuState = true,
            Module = _Accounting,
            ModuleID = _Accounting.ModuleID
        };
        
        private static Menu _Accounting_State = new Menu()
        {
            MenuLabel = "Accounting Report",
            MenuCode = CodeValue.Accounting.Report.CODE,
            MenuDescription = "Accounting Report",
            MenuFlat = true,
            MenuIconName = "report.png",
            MenuController = "Report",
            MenuPath = "Index",
            IsChortcut = true,
            MenuState = true,
            Module = _Accounting,
            ModuleID = _Accounting.ModuleID
        };
        /*=========================== CLASS ACCOUNT initialization =================================*/
        private static ClassAccount classe1ClassAccount = new ClassAccount()
        {
            ClassAccountNumber = 1,
            ClassAccountCode = "Classe 1",
            ClassAccountLabel = "Comptes de ressources durables"
        };
        private static ClassAccount classe2ClassAccount = new ClassAccount()
        {
            ClassAccountNumber = 2,
            ClassAccountCode = "Classe 2",
            ClassAccountLabel = "Comptes de l’actif immobilisé"
        };
        private static ClassAccount classe3ClassAccount = new ClassAccount()
        {
            ClassAccountNumber = 3,
            ClassAccountCode = "Classe 3",
            ClassAccountLabel = "Comptes de stocks"
        };
        private static ClassAccount classe4ClassAccount = new ClassAccount()
        {
            ClassAccountNumber = 4,
            ClassAccountCode = "Classe 4",
            ClassAccountLabel = "Comptes de tiers"
        };
        private static ClassAccount classe5ClassAccount = new ClassAccount()
        {
            ClassAccountNumber = 5,
            ClassAccountCode = "Classe 5",
            ClassAccountLabel = "Comptes de trésorerie "
        };
        private static ClassAccount classe6ClassAccount = new ClassAccount()
        {
            ClassAccountNumber = 6,
            ClassAccountCode = "Classe 6",
            ClassAccountLabel = "Comptes de charges des activités ordinaires"
        };
        private static ClassAccount classe7ClassAccount = new ClassAccount()
        {
            ClassAccountNumber = 7,
            ClassAccountCode = "Classe 7",
            ClassAccountLabel = "Comptes de produits des activités ordinaires "
        };
        private static ClassAccount classe8ClassAccount = new ClassAccount()
        {
            ClassAccountNumber = 8,
            ClassAccountCode = "Classe 8",
            ClassAccountLabel = "Comptes des autres charges et des autres produits"
        };
        private static ClassAccount classe9ClassAccount = new ClassAccount()
        {
            ClassAccountNumber = 9,
            ClassAccountCode = "Classe 9",
            ClassAccountLabel = "Comptabilité analytique de gestion"
        };
        public static List<ClassAccount> ClassAccount
        {
            get
            {
                return new List<ClassAccount>() { classe1ClassAccount, classe2ClassAccount, classe3ClassAccount, classe4ClassAccount, classe5ClassAccount, classe6ClassAccount, classe7ClassAccount, classe8ClassAccount, classe9ClassAccount };
            }
        }
        /************************Devise init*************************************/
        private static Devise xafDevise = new Devise()
        {
            DeviseCode = "XAF",
            DeviseLabel = "FRANC CFA",
            DeviseDescription = "FRANC DE LA COMMUNAUTE FRANCAISE D'AFRIQUE",
            DefaultDevise = true
        };
        private static Devise euroDevise = new Devise()
        {
            DeviseCode = "EUR",
            DeviseLabel = "DEVISE EURO",
            DeviseDescription = "DEVISE EUROPEENE",
            DefaultDevise = false
        };
        private static Devise dollardDevise = new Devise()
        {
            DeviseCode = "USD",
            DeviseLabel = "US DOLLARD",
            DeviseDescription = "US DOLLARD",
            DefaultDevise = false
        };
        public static List<Devise> Devise
        {
            get
            {
                return new List<Devise>() { xafDevise, euroDevise, dollardDevise };
            }
        }


        /*=========================== Accounting section initialization =================================*/

        private static AccountingSection prodAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 311,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD,
            AccountingSectionLabel = "Product Account Section",
            ClassAccount = classe3ClassAccount,
            ClassAccountID = classe3ClassAccount.ClassAccountID
        };
        private static AccountingSection supplyAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 401,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN,
            AccountingSectionLabel = "Supply Account Section",
            ClassAccount = classe4ClassAccount,
            ClassAccountID = classe4ClassAccount.ClassAccountID
        };
        private static AccountingSection clientAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 411,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT,
            AccountingSectionLabel = "Customer Account Section",
            ClassAccount = classe4ClassAccount,
            ClassAccountID = classe4ClassAccount.ClassAccountID
        };
        private static AccountingSection assureAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 412,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEASSUR,
            AccountingSectionLabel = "Insurance Account Section",
            ClassAccount = classe4ClassAccount,
            ClassAccountID = classe4ClassAccount.ClassAccountID
        };

        private static AccountingSection tvacollecteAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 443,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODETVACOLECT,
            AccountingSectionLabel = "VAT COLLECTED ",
            ClassAccount = classe4ClassAccount,
            ClassAccountID = classe4ClassAccount.ClassAccountID
        };

        private static AccountingSection tvadeductibleAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 445,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODETVADEDUC,
            AccountingSectionLabel = "DEDUCTED VAT",
            ClassAccount = classe4ClassAccount,
            ClassAccountID = classe4ClassAccount.ClassAccountID
        };

        private static AccountingSection bankAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 521,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK,
            AccountingSectionLabel = "Bank Account Section",
            ClassAccount = classe5ClassAccount,
            ClassAccountID = classe5ClassAccount.ClassAccountID
        };
        private static AccountingSection caisseAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 571,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS,
            AccountingSectionLabel = "Teller Account Section",
            ClassAccount = classe5ClassAccount,
            ClassAccountID = classe5ClassAccount.ClassAccountID
        };
        private static AccountingSection postalcheckAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 531,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEPOSTCHECK,
            AccountingSectionLabel = "Postal Check Account Section",
            ClassAccount = classe5ClassAccount,
            ClassAccountID = classe5ClassAccount.ClassAccountID
        };
        private static AccountingSection saleTransportAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 707,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODESALETRANSP,
            AccountingSectionLabel = "Sale Transport Account Section",
            ClassAccount = classe6ClassAccount,
            ClassAccountID = classe6ClassAccount.ClassAccountID
        };
        private static AccountingSection purchaseTransportAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 611,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEPURCTRANSP,
            AccountingSectionLabel = "Purchase Transport Account Section",
            ClassAccount = classe6ClassAccount,
            ClassAccountID = classe6ClassAccount.ClassAccountID
        };
        private static AccountingSection returnsaleTransportAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 612,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODERETSALETRANSP,
            AccountingSectionLabel = "Transport Return Sale Product Account Section",
            ClassAccount = classe6ClassAccount,
            ClassAccountID = classe6ClassAccount.ClassAccountID
        };
        
        private static AccountingSection escpteAchatAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 773,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEESCPTEACHAT,
            AccountingSectionLabel = "Purchase Discount Account Section",
            ClassAccount = classe7ClassAccount,
            ClassAccountID = classe7ClassAccount.ClassAccountID
        };
        private static AccountingSection escpteVenteAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 673,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEESCPTEVENTE,
            AccountingSectionLabel = "Sale Discount Account Section",
            ClassAccount = classe6ClassAccount,
            ClassAccountID = classe6ClassAccount.ClassAccountID
        };

        private static AccountingSection achatMseAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 601,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEACHATMSE,
            AccountingSectionLabel = "Purcharse goods Account Section",
            ClassAccount = classe6ClassAccount,
            ClassAccountID = classe6ClassAccount.ClassAccountID
        };
        private static AccountingSection venteMseAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 701,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEVENTEMSE,
            AccountingSectionLabel = "Sale goods Account Section",
            ClassAccount = classe7ClassAccount,
            ClassAccountID = classe7ClassAccount.ClassAccountID
        };
        private static AccountingSection stockVarAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 603,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODESTOCKVAR,
            AccountingSectionLabel = "Stock Variation Account Section",
            ClassAccount = classe6ClassAccount,
            ClassAccountID = classe6ClassAccount.ClassAccountID
        };
        private static AccountingSection avanceAchatAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 409,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT,
            AccountingSectionLabel = "Supplier Command Avanced Account Section",
            ClassAccount = classe4ClassAccount,
            ClassAccountID = classe4ClassAccount.ClassAccountID
        };
        private static AccountingSection avanceVenteAccountingSection = new AccountingSection()
        {
            AccountingSectionNumber = 419,
            AccountingSectionCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE,
            AccountingSectionLabel = "Customer Command Avanced Account Section",
            ClassAccount = classe4ClassAccount,
            ClassAccountID = classe4ClassAccount.ClassAccountID
        };
        public static List<AccountingSection> AccountingSections
        {
            get
            {
                return new List<AccountingSection>() { prodAccountingSection, supplyAccountingSection, clientAccountingSection, bankAccountingSection, caisseAccountingSection, postalcheckAccountingSection, tvacollecteAccountingSection, tvadeductibleAccountingSection, saleTransportAccountingSection,
                purchaseTransportAccountingSection,returnsaleTransportAccountingSection,escpteAchatAccountingSection,escpteVenteAccountingSection,achatMseAccountingSection,venteMseAccountingSection,stockVarAccountingSection,avanceAchatAccountingSection,avanceVenteAccountingSection};
            }
        }

        /*=========================== Collective Account for product initialization =================================*/

        private static CollectifAccount CR39WHITECollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31111,
            CollectifAccountLabel = "SV CR39 WHITE Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        private static CollectifAccount CR39WHITEHMCCollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31112,
            CollectifAccountLabel = "SV CR39 WHITE HMC (AR) Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        private static CollectifAccount BFPGXDTCollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31113,
            CollectifAccountLabel = "BF PGX D-TOP Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        private static CollectifAccount BFPGXRTCollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31114,
            CollectifAccountLabel = "BF PGX R-TOP Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        private static CollectifAccount PGXCollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31115,
            CollectifAccountLabel = "SV PGX Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        private static CollectifAccount PGXHMCCollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31116,
            CollectifAccountLabel = "SV PGX HMC (AR) Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        private static CollectifAccount PROCR39WHITEHMCCollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31117,
            CollectifAccountLabel = "PRO CR39 WHITE HMC (AR) Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        private static CollectifAccount PROPGXHMCCollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31118,
            CollectifAccountLabel = "PRO PGX HMC (AR) Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        private static CollectifAccount PROPGXCollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31119,
            CollectifAccountLabel = "PRO PGX Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        private static CollectifAccount PROTRANSITIONHMCCollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31120,
            CollectifAccountLabel = "PRO TRANSITION HMC (AR) Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        private static CollectifAccount TRANSITIONHMCCollectifAccount = new CollectifAccount()
        {
            CollectifAccountNumber = 31121,
            CollectifAccountLabel = "SV TRANSITION HMC (AR) Collectif Account",
            AccountingSection = prodAccountingSection,
            AccountingSectionID = prodAccountingSection.AccountingSectionID
        };
        public static List<CollectifAccount> CollectifAccounts
        {
            get
            {
                return new List<CollectifAccount>() { CR39WHITECollectifAccount, CR39WHITEHMCCollectifAccount, BFPGXDTCollectifAccount,
                BFPGXRTCollectifAccount,PGXCollectifAccount,PGXHMCCollectifAccount,PROCR39WHITEHMCCollectifAccount,PROPGXHMCCollectifAccount,
                PROPGXCollectifAccount,PROTRANSITIONHMCCollectifAccount,TRANSITIONHMCCollectifAccount};
            }
        }

        /*=========================== Operation type initialization =================================*/
        private static OperationType saleOperationType = new OperationType()
        {
            operationTypeCode = CodeValue.Accounting.InitOperationType.CODESALE,
            operationTypeLabel = "SALE OPERATION",
            operationTypeDescription = "SALE OPERATION JOURNAL"
        };
        private static OperationType purchaseOperationType = new OperationType()
        {
            operationTypeCode = CodeValue.Accounting.InitOperationType.CODEPURCHASE,
            operationTypeLabel = "PURCHASE OPERATION",
            operationTypeDescription = "PURCHASE OPERATION JOURNAL"
        };
        private static OperationType saleReturnOperationType = new OperationType()
        {
            operationTypeCode = CodeValue.Accounting.InitOperationType.CODESALERETURN,
            operationTypeLabel = "SALE RETURN OPERATION ",
            operationTypeDescription = "SALE RETURN OPERATION JOURNAL"
        };
        private static OperationType purchaseReturnOperationType = new OperationType()
        {
            operationTypeCode = CodeValue.Accounting.InitOperationType.CODEPURCHASERETURN,
            operationTypeLabel = "PURCHASE RETURN OPERATION",
            operationTypeDescription = "PURCHASE RETURN OPERATION JOURNAL"
        };
      
        private static OperationType manualOperationType = new OperationType()
        {
            operationTypeCode = CodeValue.Accounting.InitOperationType.CODEMANUAL,
            operationTypeLabel = "MANUAL OPERATION",
            operationTypeDescription = "MANUAL OPERATION JOURNAL"
        };
         private static OperationType stockInputOperationType = new OperationType()
        {
            operationTypeCode = CodeValue.Accounting.InitOperationType.CODESTOCKINPUT,
            operationTypeLabel = "STOCK INPUT OPERATION",
            operationTypeDescription = "STOCK INPUT OPERATION JOURNAL"
        };
         private static OperationType stockOutputOperationType = new OperationType()
         {
             operationTypeCode = CodeValue.Accounting.InitOperationType.CODESTOCKOUTPUT,
             operationTypeLabel = "STOCK OUTPUT OPERATION",
             operationTypeDescription = "STOCK OUTPUT OPERATION JOURNAL"
         };
        private static OperationType stockDepreciateOperationType = new OperationType()
         {
             operationTypeCode = CodeValue.Accounting.InitOperationType.CODESTOCKDEPRECIATE,
             operationTypeLabel = "STOCK DEPRECIATE OPERATION",
             operationTypeDescription = "STOCK DEPRECIATE OPERATION JOURNAL"
         };
        private static OperationType budgetOperationType = new OperationType()
        {
            operationTypeCode = CodeValue.Accounting.InitOperationType.CODEBUDGET,
            operationTypeLabel = "BUDGET OPERATION",
            operationTypeDescription = "BUDGET OPERATION JOURNAL"
        };
        private static OperationType tellerAdjustOperationType = new OperationType()
        {
            operationTypeCode = CodeValue.Accounting.InitOperationType.CODETELLERADJUST,
            operationTypeLabel = "TELLER ADJUST OPERATION",
            operationTypeDescription = "TELLER ADJUST OPERATION JOURNAL"
        };
        private static OperationType overageOperationType = new OperationType()
        {
            operationTypeCode = CodeValue.Accounting.InitOperationType.CODEOVERAGE,
            operationTypeLabel = "OVERAGE OPERATION",
            operationTypeDescription = "OVERAGE OPERATION JOURNAL"
        };
        public static List<OperationType> OperationType
        {
            get
            {
                return new List<OperationType>() { saleOperationType, purchaseOperationType, saleReturnOperationType, purchaseReturnOperationType, 
                     manualOperationType,stockInputOperationType,stockOutputOperationType,stockDepreciateOperationType,budgetOperationType,
                    tellerAdjustOperationType,overageOperationType};
            } 
        }
        /*=========================== Macro Operation Initialization =================================*/
        //MANUAL
        private static MacroOperation manualMacroOperation = new MacroOperation()
        {
            MacroOperationCode = CodeValue.Accounting.InitMacroOperation.CODEMANUAL,
            MacroOperationLabel = "MANUAL OPERATION",
            MacroOperationDescription = "MANUAL OPERATION"
        };
        //CERTIFY
        private static MacroOperation certifyMacroOperation = new MacroOperation()
        {
            MacroOperationCode = CodeValue.Accounting.InitMacroOperation.CODECERTIFY,
            MacroOperationLabel = "CERTIFY OPERATION",
            MacroOperationDescription = "CERTIFY OPERATION"
        };
        //LIVRAISON
        private static MacroOperation deliveryMacroOperation = new MacroOperation()
        {
            MacroOperationCode = CodeValue.Accounting.InitMacroOperation.CODEDELIVERY,
            MacroOperationLabel = "DELIVERY OPERATION",
            MacroOperationDescription = "DELIVERY OPERATION"
        };
        //FACTURATION
        private static MacroOperation billingMacroOperation = new MacroOperation()
        {
            MacroOperationCode = CodeValue.Accounting.InitMacroOperation.CODEBILLING,
            MacroOperationLabel = "BILLING OPERATION",
            MacroOperationDescription = "BILLING OPERATION"
        };
        //advanced
        private static MacroOperation advancedMacroOperation = new MacroOperation()
        {
            MacroOperationCode = CodeValue.Accounting.InitMacroOperation.CODEADVANCED,
            MacroOperationLabel = "ADVANCED OPERATION",
            MacroOperationDescription = "ADVANCED OPERATION"
        };
        //REGLEMENT
        private static MacroOperation paymentMacroOperation = new MacroOperation()
        {
            MacroOperationCode = CodeValue.Accounting.InitMacroOperation.CODEPAYMENT,
            MacroOperationLabel = "PAYMENT OPERATION",
            MacroOperationDescription = "PAYMENT OPERATION"
        };
        public static List<MacroOperation> MacroOperation
        {
            get
            {
                return new List<MacroOperation>() { certifyMacroOperation,deliveryMacroOperation, billingMacroOperation,advancedMacroOperation, paymentMacroOperation, manualMacroOperation };
            }
        }
        /*=========================== Reglement Type initialization =================================*/
        private static ReglementType noneReglementType = new ReglementType()
        {
            ReglementTypeCode = CodeValue.Accounting.InitReglementType.CODEAUCUN,
            ReglementTypeLabel = "NONE",
            ReglementTypeDescription = "NONE"
        };
        private static ReglementType bankReglementType = new ReglementType()
        {
            ReglementTypeCode = CodeValue.Accounting.InitReglementType.CODEBANK,
            ReglementTypeLabel = "BANK",
            ReglementTypeDescription = "BANK"
        };
        private static ReglementType cashReglementType = new ReglementType()
        {
            ReglementTypeCode = CodeValue.Accounting.InitReglementType.CODECASH,
            ReglementTypeLabel = "CASH",
            ReglementTypeDescription = "CASH"
        };
        
        public static List<ReglementType> ReglementType
        {
            get
            {
                return new List<ReglementType>() { noneReglementType, bankReglementType, cashReglementType };
            }
        }

        /*=========================== Operation initialization =================================*/
        //MANUAL
        private static Operation manualOperation = new Operation()
        {

            OperationCode = CodeValue.Accounting.InitOperation.CODEMANUALOP,
            OperationLabel = "MANUAL OPERATION",
            OperationDescription = "MANUAL OPERATION",
            OperationType = manualOperationType,
            OperationTypeID = manualOperationType.operationTypeID,
            //MacroOperation = manualMacroOperation,
            //MacroOperationID = manualMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        //VENTE
        private static Operation saleDeliveryOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODESALEDELIVERY,
            OperationLabel = "DELIVERY SALE OPERATION",
            OperationDescription = "DELIVERY SALE OPERATION",
            OperationType = saleOperationType,
            OperationTypeID = saleOperationType.operationTypeID,
            //MacroOperation = deliveryMacroOperation,
            //MacroOperationID = deliveryMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        private static Operation saleBillingOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODESALEBILLING,
            OperationLabel = "BILLING SALE OPERATION",
            OperationDescription = "BILLING SALE OPERATION",
            OperationType = saleOperationType,
            OperationTypeID = saleOperationType.operationTypeID,
            //MacroOperation = billingMacroOperation,
            //MacroOperationID = billingMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        private static Operation cashSaleAdvancedOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT,
            OperationLabel = "CASH ADVANCED SALE OPERATION",
            OperationDescription = "CASH ADVANCED SALE OPERATION",
            OperationType = saleOperationType,
            OperationTypeID = saleOperationType.operationTypeID,
            //MacroOperation = advancedMacroOperation,
            //MacroOperationID = advancedMacroOperation.MacroOperationID,
            //ReglementType = cashReglementType,
            //ReglementTypeID = cashReglementType.ReglementTypeID
        };
        private static Operation bankSaleAdvancedOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT,
            OperationLabel = "BANK ADVANCED SALE OPERATION",
            OperationDescription = "BANK ADVANCED SALE OPERATION",
            OperationType = saleOperationType,
            OperationTypeID = saleOperationType.operationTypeID,
            //MacroOperation = advancedMacroOperation,
            //MacroOperationID = advancedMacroOperation.MacroOperationID,
            //ReglementType = bankReglementType,
            //ReglementTypeID = bankReglementType.ReglementTypeID
        };
        private static Operation cancelSaleAdvancedOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODECANCELSALEADVANCEDPAYMENT,
            OperationLabel = "CANCEL ADVANCED SALE OPERATION",
            OperationDescription = "CANCEL ADVANCED SALE OPERATION",
            OperationType = saleOperationType,
            OperationTypeID = saleOperationType.operationTypeID,
            //MacroOperation = certifyMacroOperation,
            //MacroOperationID = certifyMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        private static Operation cashSaleOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT,
            OperationLabel = "CASH SALE OPERATION",
            OperationDescription = "CASH SALE OPERATION",
            OperationType = saleOperationType,
            OperationTypeID = saleOperationType.operationTypeID,
            //MacroOperation = paymentMacroOperation,
            //MacroOperationID = paymentMacroOperation.MacroOperationID,
            //ReglementType = cashReglementType,
            //ReglementTypeID = cashReglementType.ReglementTypeID
        };
        private static Operation bankSaleOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT,
            OperationLabel = "BANK SALE OPERATION",
            OperationDescription = "BANK SALE OPERATION",
            OperationType = saleOperationType,
            OperationTypeID = saleOperationType.operationTypeID,
            //MacroOperation = paymentMacroOperation,
            //MacroOperationID = paymentMacroOperation.MacroOperationID,
            //ReglementType = bankReglementType,
            //ReglementTypeID = bankReglementType.ReglementTypeID
        };
        //RETOUR VENTE
        private static Operation certifyReturnSaleOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODECERTIFYRETURNSALE,
            OperationLabel = "CERTIFY RETURN SALE OPERATION",
            OperationDescription = "CERTIFY RETURN SALE OPERATION",
            OperationType = saleReturnOperationType,
            OperationTypeID = saleReturnOperationType.operationTypeID,
            //MacroOperation = certifyMacroOperation,
            //MacroOperationID = certifyMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        //SORTIE EN STOCK
        private static Operation stockOutputConstatOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT,
            OperationLabel = "CERTIFY STOCK OUTPUT OPERATION",
            OperationDescription = "CERTIFY STOCK OUTPUT OPERATION",
            OperationType = stockOutputOperationType,
            OperationTypeID = stockOutputOperationType.operationTypeID,
            //MacroOperation = certifyMacroOperation,
            //MacroOperationID = certifyMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        //DEPRECIATE OPERATION
        private static Operation stockDepreciateConstatOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODESTOCKDEPRECIATE,
            OperationLabel = "CERTIFY STOCK DEPRECIATE OPERATION",
            OperationDescription = "CERTIFY STOCK DEPRECIATE OPERATION",
            OperationType = stockDepreciateOperationType,
            OperationTypeID = stockDepreciateOperationType.operationTypeID,
            //MacroOperation = certifyMacroOperation,
            //MacroOperationID = certifyMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        
        //ACHAT
        private static Operation purchaseDeliveryOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODEPURCHASEDELIVERY,
            OperationLabel = "RECEIVE DELIVERY ORDER OPERATION",
            OperationDescription = "RECEIVE DELIVERY ORDER OPERATION",
            OperationType = purchaseOperationType,
            OperationTypeID = purchaseOperationType.operationTypeID,
            //MacroOperation = deliveryMacroOperation,
            //MacroOperationID = deliveryMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        private static Operation purchaseBillingOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING,
            OperationLabel = "RECEIVE BILLING PURCHASE OPERATION",
            OperationDescription = "RECEIVE BILLING PURCHASE OPERATION",
            OperationType = purchaseOperationType,
            OperationTypeID = purchaseOperationType.operationTypeID,
            //MacroOperation = billingMacroOperation,
            //MacroOperationID = billingMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        private static Operation cashPurchaseAdvancedOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODECASHPURCHASEADVANCEDPAYMENT,
            OperationLabel = "CASH PURCHASE ADVANCED OPERATION",
            OperationDescription = "CASH PURCHASE ADVANCED OPERATION",
            OperationType = purchaseOperationType,
            OperationTypeID = purchaseOperationType.operationTypeID,
            //MacroOperation = paymentMacroOperation,
            //MacroOperationID = paymentMacroOperation.MacroOperationID,
            //ReglementType = cashReglementType,
            //ReglementTypeID = cashReglementType.ReglementTypeID
        };
        private static Operation bankPurchaseAdvancedOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODEBANKPURCHASEADVANCEDPAYMENT,
            OperationLabel = "BANK PURCHASE ADVANCED OPERATION",
            OperationDescription = "BANK PURCHASE ADVANCED OPERATION",
            OperationType = purchaseOperationType,
            OperationTypeID = purchaseOperationType.operationTypeID,
            //MacroOperation = paymentMacroOperation,
            //MacroOperationID = paymentMacroOperation.MacroOperationID,
            //ReglementType = bankReglementType,
            //ReglementTypeID = bankReglementType.ReglementTypeID
        };
        private static Operation cancelPurchaseAdvancedOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODECANCELPURCHASEADVANCEDPAYMENT,
            OperationLabel = "CANCEL ADVANCED PURCHASE OPERATION",
            OperationDescription = "CANCEL ADVANCED PURCHASE OPERATION",
            OperationType = purchaseOperationType,
            OperationTypeID = purchaseOperationType.operationTypeID,
            //MacroOperation = certifyMacroOperation,
            //MacroOperationID = certifyMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        //entree EN STOCK
        private static Operation stockInputConstatOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODESTOCKINPUT,
            OperationLabel = "CERTIFY STOCK INPUT OPERATION",
            OperationDescription = "CERTIFY STOCK INPUT OPERATION",
            OperationType = stockInputOperationType,
            OperationTypeID = stockInputOperationType.operationTypeID,
            //MacroOperation = deliveryMacroOperation,
            //MacroOperationID = deliveryMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        private static Operation cashPurchaseOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODECASHPAYMENTPURCHASE,
            OperationLabel = "CASH PURCHASE OPERATION",
            OperationDescription = "CASH PURCHASE OPERATION",
            OperationType = purchaseOperationType,
            OperationTypeID = purchaseOperationType.operationTypeID,
            //MacroOperation = paymentMacroOperation,
            //MacroOperationID = paymentMacroOperation.MacroOperationID,
            //ReglementType = cashReglementType,
            //ReglementTypeID = cashReglementType.ReglementTypeID
        };
        private static Operation bankPurchaseOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODEBANKPAYMENTPURCHASE,
            OperationLabel = "BANK PURCHASE OPERATION",
            OperationDescription = "BANK PURCHASE OPERATION",
            OperationType = purchaseOperationType,
            OperationTypeID = purchaseOperationType.operationTypeID,
            //MacroOperation = paymentMacroOperation,
            //MacroOperationID = paymentMacroOperation.MacroOperationID,
            //ReglementType = bankReglementType,
            //ReglementTypeID = bankReglementType.ReglementTypeID
        };
        
        //RETOUR ACHAT
        private static Operation certifyReturnPurchaseOperation = new Operation()
        {
            OperationCode = CodeValue.Accounting.InitOperation.CODECERTIFYRETURNPURCHASE,
            OperationLabel = "CERTIFY RETURN PURCHASE OPERATION",
            OperationDescription = "CERTIFY RETURN PURCHASE OPERATION",
            OperationType = purchaseReturnOperationType,
            OperationTypeID = purchaseReturnOperationType.operationTypeID,
            //MacroOperation = certifyMacroOperation,
            //MacroOperationID = certifyMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        //Budget
        private static Operation budgetOperation = new Operation()
        {

            OperationCode = CodeValue.Accounting.InitOperation.CODEBUDGET,
            OperationLabel = "BUDGET OPERATION",
            OperationDescription = "BUDGET OPERATION",
            OperationType = budgetOperationType,
            OperationTypeID = budgetOperationType.operationTypeID,
            //MacroOperation = certifyMacroOperation,
            //MacroOperationID = certifyMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };

        //Shortage
        private static Operation tellerAdjustOperation = new Operation()
        {

            OperationCode = CodeValue.Accounting.InitOperation.CODETELLERADJUST,
            OperationLabel = "TELLER ADJUST OPERATION",
            OperationDescription = "TELLER ADJUST OPERATION",
            OperationType = tellerAdjustOperationType,
            OperationTypeID = tellerAdjustOperationType.operationTypeID,
            //MacroOperation = certifyMacroOperation,
            //MacroOperationID = certifyMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        //Overage
        private static Operation overageOperation = new Operation()
        {

            OperationCode = CodeValue.Accounting.InitOperation.CODEOVERAGE,
            OperationLabel = "OVERAGE OPERATION",
            OperationDescription = "OVERAGE OPERATION",
            OperationType = overageOperationType,
            OperationTypeID = overageOperationType.operationTypeID,
            //MacroOperation = certifyMacroOperation,
            //MacroOperationID = certifyMacroOperation.MacroOperationID,
            //ReglementType = noneReglementType,
            //ReglementTypeID = noneReglementType.ReglementTypeID
        };
        public static List<Operation> Operation
        {
            get
            {
                return new List<Operation>() { saleDeliveryOperation,saleBillingOperation,cashSaleAdvancedOperation,bankSaleAdvancedOperation, 
                    purchaseDeliveryOperation,purchaseBillingOperation,cashPurchaseAdvancedOperation,bankPurchaseAdvancedOperation, cashPurchaseOperation, 
                    bankPurchaseOperation, certifyReturnSaleOperation,certifyReturnPurchaseOperation,manualOperation,cashSaleOperation, bankSaleOperation,
                    stockOutputConstatOperation,stockInputConstatOperation,stockDepreciateConstatOperation,cancelSaleAdvancedOperation,cancelPurchaseAdvancedOperation,
                    budgetOperation,tellerAdjustOperation,overageOperation};
            }
        }
        //Initialisation de l'exercice fiscal
        private static FiscalYear fiscalYear2015 = new FiscalYear()
        {
            FiscalYearNumber = 2016,
            FiscalYearStatus = true,
            FiscalYearLabel = "Fiscal Year 2016",
            StartFrom=new DateTime(2016,1,1),
            EndFrom = new DateTime(2016,12,31)
        };
        public static List<FiscalYear> FiscalYear
        {
            get
            {
                return new List<FiscalYear>() { fiscalYear2015 };
            }
        }

    }

}
