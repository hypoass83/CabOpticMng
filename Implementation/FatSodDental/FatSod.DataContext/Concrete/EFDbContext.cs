using System.Data.Entity;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSod.DataContext.Initializer;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
//using FatSod.Report.WrapReports;
using FatSod.Budget.Entities;

namespace FatSod.DataContext.Concrete
{
    /// <summary>
    /// EFDbContext
    /// </summary>
    public class EFDbContext : DbContext
    {
        //public EFDbContext() { this.Database.Initialize(true); }
        /*===================== Security Tables  ========================*/
        public DbSet<UserConfiguration> UserConfigurations { get; set; }
        public DbSet<LensNumberRange> LensNumberRanges { get; set; }
        public DbSet<LensNumberRangePrice> LensNumberRangePrices { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<SubMenu> SubMenus { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<OrderLens> OrderLenses { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ActionMenuProfile> ActionMenuProfiles { get; set; }
        public DbSet<ActionSubMenuProfile> ActionSubMenuProfiles { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<UserBranch> UserBranches { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Quarter> Quarters { get; set; }
        public DbSet<Sex> Sexes { get; set; }
        public DbSet<Adress> Adresses { get; set; }
        public DbSet<ProductTransfertLine> ProductTransfertLines { get; set; }
        public DbSet<ProductDamageLine> ProductDamageLines { get; set; }
        public DbSet<StockReplacementLine> StockReplacementLines { get; set; }
        public DbSet<ProductGiftLine> ProductGiftLines { get; set; }
        public DbSet<BusinessDay> BusinessDays { get; set; }
        public DbSet<ClosingDayTask> ClosingDayTasks { get; set; }
        public DbSet<BranchClosingDayTask> BranchClosingDayTasks { get; set; }
        public DbSet<Sneak> Sneaks { get; set; }
        public DbSet<Mouchar> Mouchars { get; set; }
        public DbSet<SneakDay> SneakDays { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<GlobalPerson> GlobalPeople { get; set; }
        public DbSet <ProductBrand> ProductBrands { get; set; }
        /*=================================================================*/
        /*========================== Sale Tables  =========================*/
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<UserTill> UserTills { get; set; }
        public DbSet<Till> Tills { get; set; }
        public DbSet<DigitalPaymentMethod> DigitalPaymentMethods { get; set; }
        
        public DbSet<Bank> Banks { get; set; }
        public DbSet<AssureurPM> AssureurPMs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<AuthoriseSale> AuthoriseSales { get; set; }
        public DbSet<BankSale> BankSales { get; set; }
        public DbSet<AssureurSale> AssureurSales { get; set; }
        public DbSet<TillSale> TillSales { get; set; }
        public DbSet<DigitalPaymentSale> DigitalPaymentSales { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<SaleLine> SaleLines { get; set; }
        public DbSet<AuthoriseSaleLine> AuthoriseSaleLines { get; set; }
        public DbSet<TillDay> TillDays { get; set; }
        public DbSet<TillDayStatus> TillDayStatus { get; set; }
        public DbSet<TillAdjust> TillAdjusts { get; set; }
        public DbSet<CustomerSlice> CustomerSlices { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Assureur> Assureurs { get; set; }
        public DbSet<Slice> Slices { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<CustomerOrderLine> CustomerOrderLines { get; set; }
        public DbSet<CustomerReturnLine> CustomerReturnLines { get; set; }
        public DbSet<CustomerReturn> CustomerReturns { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<AllDeposit> AllDeposits {get;set;}
        public DbSet<SavingAccount> SavingAccounts { get; set; }
        public DbSet<SavingAccountSale> SavingAccountSales { get; set; }
        public DbSet<CustomerReturnSlice> CustomerReturnSlices { get; set; }
        public DbSet<TreasuryOperation> TreasuryOperations { get; set; }

        //table cumul pou sauvegarder les ventes et les factures pour des raisons de stockage de special order
        public DbSet<CumulSaleAndBill> CumulSaleAndBills { get; set; }
        public DbSet<CumulSaleAndBillLine> CumulSaleAndBillLines { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<PrescriptionLStep> PrescriptionLSteps { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<ConsultOldPrescr> ConsultOldPrescrs { get; set; }
        public DbSet<ConsultPersonalMedHisto> ConsultPersonalMedHistos { get; set; }

        public DbSet<ConsultDilPresc> ConsultDilPrescs { get; set; }
        public DbSet<ConsultDilatation> ConsultDilatations { get; set; }
        public DbSet<ConsultLensPrescription> ConsultLensPrescriptions { get; set; }
        public DbSet<ConsultPrescrLastStep> ConsultPrescrLastSteps { get; set; }

        public DbSet<AcuiteVisuelL> AcuiteVisuelLs { get; set; }
        public DbSet<AVLTS> AVLTSs { get; set; }
        public DbSet<AcuiteVisuelP> AcuiteVisuelPs { get; set; }

        public DbSet<BorderoDepot> BorderoDepots { get; set; }

        public DbSet<LieuxdeDepotBordero> LieuxdeDepotBorderos { get; set; }
        /*=================================================================*/
        /*========================= Purchase Tables  ==========================*/

        public DbSet<Category> Categories { get; set; }
        public DbSet<GenericProduct> GenericProducts { get; set; }
        public DbSet<BarCodeGenerator> BarCodeGenerators { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<ProductLocalization> ProductLocalizations { get; set; }
        public DbSet<ProductTransfert> ProductTransferts { get; set; }
        public DbSet<ProductDamage> ProductDamages { get; set; }
        public DbSet<StockReplacement> StockReplacements { get; set; }
        
        public DbSet<ProductGift> ProductGifts { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseLine> PurchaseLines { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierSlice> SupplierSlices { get; set; }
        public DbSet<SupplierOrder> SupplierOrders { get; set; }
        public DbSet<SupplierOrderLine> SupplierOrderLines { get; set; }
        public DbSet<SupplierReturnLine> SupplierReturnLines { get; set; }
        public DbSet<SupplierReturn> SupplierReturns { get; set; }
        public DbSet<TillPurchase> TillPurchases { get; set; }
        public DbSet<BankPurchase> BankPurchases { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<LensCategory> LensCategories { get; set; }
        public DbSet<Lens> Lenses { get; set; }
        public DbSet<LensCoating> LensCoatings { get; set; }
        public DbSet<LensColour> LensColours { get; set; }
        public DbSet<LensMaterial> LensMaterials { get; set; }
        public DbSet<LensNumber> LensNumbers { get; set; }
        public DbSet<EmployeeStock> EmployeeStocks { get; set; }
        public DbSet<EmployeeStockHistoric> EmployeeStockHistorics { get; set; }
        public DbSet<InventoryHistoric> InventoryHistorics { get; set; }
        public DbSet<InventoryDirectory> InventoryDirectories { get; set; }
        public DbSet<InventoryDirectoryLine> InventoryDirectoryLines { get; set; }

        public DbSet<RegProductNumber> RegProductNumbers { get; set; }
        public DbSet<RegProductNumberLine> RegProductNumberLines { get; set; }

        /*=================================================================*/
        /*========================= Accounting Tables  ==========================*/
        public DbSet<ClassAccount> ClassAccounts { get; set; }
        public DbSet<AccountingSection> AccountingSections { get; set; }
        public DbSet<CollectifAccount> CollectifAccounts { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Journal> Journals { get; set; }
        
        public DbSet<AccountingTask> AccountingTasks { get; set; }
        public DbSet<AccountOperation> AccountOperations { get; set; }
        //public DbSet<ReglementType> ReglementTypes { get; set; }
        //public DbSet<MacroOperation> MacroOperations { get; set; }
        public DbSet<Devise> Devises { get; set; }
        public DbSet<Piece> Pieces { get; set; }
        public DbSet<TransactNumber> TransactNumbers { get; set; }

        public DbSet<CompteurBorderoDepot> CompteurBorderoDepots { get; set; }
        //budget tables
        public DbSet<FiscalYear> FiscalYears { get; set; }
        public DbSet<BudgetLine> BudgetLines { get; set; }
        public DbSet<BudgetAllocated> BudgetAllocateds { get; set; }
        public DbSet<BudgetAllocatedUpdate> BudgetAllocatedUpdates { get; set; }
        public DbSet<BudgetConsumption> BudgetConsumptions { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }

        //rendez vous
        public DbSet<RendezVous> RendezVous { get; set; }
        public DbSet<HistoRendezVous> HistoRendezVous { get; set; }
        public DbSet<HistoSMS> HistoSMSs { get; set; }
        public DbSet<ExtractSMS> ExtractSMSs { get; set; }

        //consultation
        public DbSet<ATCD> ATCDs { get; set; }
        public DbSet<ATCDPersonnel> ATCDPersonnels { get; set; }
        public DbSet<ATCDFamilial> ATCDFamiliaux { get; set; }

        public DbSet<InsuredCompany> InsuredCompanies { get; set; }
        /*=================================================================*/
        /*========================= Reports Tables  ==========================*/
        /*public DbSet<RptBorderoDepotFacture> RptBorderoDepotFactures { get; set; }
        public DbSet<RptReceipt> RptReceipts { get; set; }
        public DbSet<RptPaymentDetail> RptPaymentDetails { get; set; }
        public DbSet<RptTransfertForm> RptTransfertForms { get; set; }
        public DbSet<RptCashDayOperation> RptCashDayOperations { get; set; }
        public DbSet<RptCustomerPayment> RptCustomerPayments { get; set; }
        public DbSet<RptHeader> RptHeaders { get; set; }
        public DbSet<RptInventory> RptInventories { get; set; }
        public DbSet<RptBill> RptBills { get; set; }
        public DbSet<RptCashOpHist> RptCashOpHists { get; set; }
        public DbSet<RptReturnSale> RptReturnSales { get; set; }
        public DbSet<RptSpecialOrder> RptSpecialOrders { get; set; }
        public DbSet<RptInventoryEntry> RptInventoryEntrys { get; set; }
        /*****rpt acct ************/
        /*public DbSet<RptEtatsJournal> RptEtatsJournaux { get; set; }
        public DbSet<RptAcctingPlan> RptAcctingPlans { get; set; }
        public DbSet<RptPrintStmt> RptPrintStmts { get; set; }
        public DbSet<RptBalanceGenerale> RptBalanceGenerales { get; set; }
        public DbSet<RptIncomeExpense> RptIncomeExpenses { get; set; }
        public DbSet<RptbudgetExpense> RptbudgetExpenses { get; set; }
        public DbSet<RptPrintStockMvt> RptPrintStockMvts { get; set; }
        public DbSet<RptBareCode> RptBareCodes { get; set; }
        */
        /*=================================================================*/

        //views

        public DbSet<vcumulRealSales> vcumulRealSales { get; set; }
        public DbSet<vCumulSaleWithoutreturn> vCumulSaleWithoutreturn { get; set; }
        public DbSet<vCumulPartialreturn> vCumulPartialreturn { get; set; }

        public DbSet<viewRealSales> viewRealSales { get; set; }
        public DbSet<viewSaleWithoutreturn> viewSaleWithoutreturn { get; set; }
        public DbSet<viewPartialreturn> viewPartialreturn { get; set; }

        public DbSet<viewcustomerSlice> viewcustomerSlice { get; set; }
        public DbSet<saletotalprice> saletotalprice { get; set; }
        public DbSet<CutomertotalpricePerDay> CutomertotalpricePerDay { get; set; }
        public DbSet<viewcumulreturnsalepersale> viewcumulreturnsalepersale { get; set; }
        public DbSet<PendingSale> PendingSale { get; set; }

        public DbSet<viewCustomers> viewCustomers { get; set; }

        public DbSet<V_CustomerStatus> V_CustomerStatus { get; set; }
        public DbSet<V_InsureStatus> V_InsureStatus { get; set; }
        public DbSet<V_Detail_Sales> V_Detail_Sales { get; set; }
        public DbSet<V_Summary_Sales> V_Summary_Sales { get; set; }
        public DbSet<V_Detail_Insured_Bill> V_Detail_Insured_Bill { get; set; }
        public DbSet<V_Summary_Insured_Bill> V_Summary_Insured_Bill { get; set; }
        public DbSet<NoPurchase> NoPurchases { get; set; }
        public DbSet<NotificationSetting> NotificationSettings { get; set; }

        public DbSet<CustomerSatisfaction> CustomerSatisfactions { get; set; }
        public DbSet<CustomerComplaint> CustomerComplaints { get; set; }
        public DbSet<ComplaintFeedBack> ComplaintFeedBacks { get; set; }
        public DbSet<BannedNumber> BannedNumbers { get; set; }
        public DbSet<InventoryCounting> InventoryCountings { get; set; }
        public DbSet<InventoryCountingLine> InventoryCountingLines { get; set; }

        public DbSet<InventoryReconciliation> InventoryReconciliations { get; set; }
        public DbSet<InventoryReconciliationLine> InventoryReconciliationLines { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //============= Security tables
            modelBuilder.Entity<GlobalPerson>().ToTable("GlobalPeople");
            modelBuilder.Entity<Person>().ToTable("People");
            modelBuilder.Entity<User>().ToTable("Users");
            //============ Sales tables
            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Assureur>().ToTable("Assureurs");
            modelBuilder.Entity<Company>().ToTable("Companies");

            modelBuilder.Entity<InsuredCompany>().ToTable("InsuredCompanies");
            
            modelBuilder.Entity<TillSale>().ToTable("TillSales");
            modelBuilder.Entity<BankSale>().ToTable("BankSales");
            modelBuilder.Entity<AssureurPM>().ToTable("AssureurPMs");
            modelBuilder.Entity<AssureurSale>().ToTable("AssureurSales");
            modelBuilder.Entity<DigitalPaymentSale>().ToTable("DigitalPaymentSales");
            modelBuilder.Entity<CustomerOrderLine>().ToTable("CustomerOrderLines");
            modelBuilder.Entity<SaleLine>().ToTable("SaleLines");
            modelBuilder.Entity<AuthoriseSaleLine>().ToTable("AuthoriseSaleLines");
            modelBuilder.Entity<TillPurchase>().ToTable("TillPurchases");
            modelBuilder.Entity<BankPurchase>().ToTable("BankPurchases");
            modelBuilder.Entity<SavingAccountSale>().ToTable("SavingAccountSales");
            modelBuilder.Entity<SavingAccount>().ToTable("SavingAccounts");
            modelBuilder.Entity<Bank>().ToTable("Banks");
            modelBuilder.Entity<Till>().ToTable("Tills");
            modelBuilder.Entity<DigitalPaymentMethod>().ToTable("DigitalPaymentMethods");
            modelBuilder.Entity<CustomerSlice>().ToTable("CustomerSlices");
            modelBuilder.Entity<CustomerReturnSlice>().ToTable("CustomerReturnSlices");

            modelBuilder.Entity<CumulSaleAndBillLine>().ToTable("CumulSaleAndBillLines");

            //========= Supply tables
            modelBuilder.Entity<Supplier>().ToTable("Suppliers");
            //modelBuilder.Entity<Line>().ToTable("Lines");
            modelBuilder.Entity<PurchaseLine>().ToTable("PurchaseLines");
            modelBuilder.Entity<SupplierOrderLine>().ToTable("SupplierOrderLines");
            modelBuilder.Entity<SupplierReturnLine>().ToTable("SupplierReturnLines");
            modelBuilder.Entity<Slice>().ToTable("Slices");
            modelBuilder.Entity<SupplierSlice>().ToTable("SupplierSlices");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<GenericProduct>().ToTable("GenericProducts");
            modelBuilder.Entity<Lens>().ToTable("Lenses");
            modelBuilder.Entity<LensCategory>().ToTable("LensCategories");
            modelBuilder.Entity<OrderLens>().ToTable("OrderLenses");

            //=========== Accounting tables
            modelBuilder.Entity<AccountOperation>().ToTable("AccountOperations");
            modelBuilder.Entity<ManualAccountOperation>().ToTable("ManualAccountOperations");
            modelBuilder.Entity<SaleAccountOperation>().ToTable("SaleAccountOperations");
            modelBuilder.Entity<PurchaseAccountOperation>().ToTable("PurchaseAccountOperations");
            modelBuilder.Entity<ProductTransferAccountOperation>().ToTable("ProductTransferAccountOperations");
            modelBuilder.Entity<SaleReturnAccountOperation>().ToTable("SaleReturnAccountOperations");
            modelBuilder.Entity<PurchaseReturnAccountOperation>().ToTable("PurchaseReturnAccountOperations");
            modelBuilder.Entity<ProductLocalizationAccountOperation>().ToTable("ProductLocalizationAccountOperations");
            
            modelBuilder.Entity<BudgetConsumptionAccountOperation>().ToTable("BudgetConsumptionAccountOperations");
            modelBuilder.Entity<TillAdjustAccountOperation>().ToTable("TillAdjustAccountOperations");
            modelBuilder.Entity<TreasuryOperationAccountOperation>().ToTable("TreasuryOperationAccountOperations");
            modelBuilder.Entity<DepositAccountOperation>().ToTable("DepositAccountOperations");

            modelBuilder.Entity<ATCD>().ToTable("ATCDs");
            modelBuilder.Entity<ATCDPersonnel>().ToTable("ATCDPersonnels");
            modelBuilder.Entity<ATCDFamilial>().ToTable("ATCDFamiliaux");

            modelBuilder.Entity<ProductLocalization>().MapToStoredProcedures();
            modelBuilder.Entity<GlobalPerson>().MapToStoredProcedures();
            modelBuilder.Entity<CustomerOrder>().MapToStoredProcedures();
            modelBuilder.Entity<Line>().MapToStoredProcedures();
            modelBuilder.Entity<Sale>().MapToStoredProcedures();
            modelBuilder.Entity<AccountOperation>().MapToStoredProcedures();
            modelBuilder.Entity<InventoryHistoric>().MapToStoredProcedures();
            modelBuilder.Entity<Deposit>().MapToStoredProcedures();
            modelBuilder.Entity<AllDeposit>().MapToStoredProcedures();
            modelBuilder.Entity<Consultation>().MapToStoredProcedures();

            modelBuilder.Entity<ConsultOldPrescr>().MapToStoredProcedures();
            modelBuilder.Entity<ConsultPrescrLastStep>().MapToStoredProcedures();

            modelBuilder.Entity<AcuiteVisuelP>().MapToStoredProcedures();
            modelBuilder.Entity<AcuiteVisuelL>().MapToStoredProcedures();
            modelBuilder.Entity<AVLTS>().MapToStoredProcedures();

            modelBuilder.Entity<ConsultDilPresc>().MapToStoredProcedures();
            modelBuilder.Entity<ConsultDilatation>().MapToStoredProcedures();
            modelBuilder.Entity<ConsultLensPrescription>().MapToStoredProcedures();
            modelBuilder.Entity<PrescriptionLStep>().MapToStoredProcedures();

            //================= Report Tables
            // modelBuilder.Entity<RptInventory>().ToTable("RptInventories");
            //creation des tables AccountingTask et AccountingSpecificTask
            /*modelBuilder.Entity<AccountingTask>()
                .Map(map =>
                {
                    map.Properties(p => new
                        {
                            p.AccountingTaskID,
                            p.AccountingSectionID,
                            p.AccountingTaskDescription,
                            p.AccountingTaskSens,
                            p.OperationID,
                            p.ApplyVat,
                        });
                    map.ToTable("AccountingTask");
                })
                .Map(map =>
                {
                    map.Properties(p => new
                    {
                        p.AccountingTaskID,
                        p.AccountID,
                        p.VatAccountID,
                        p.DiscountAccountID,
                        p.TransportAccountID
                    });
                    map.ToTable("AccountingSpecificTask");
                });*/
            //========== Data base initialization
            Database.SetInitializer<EFDbContext>(new InventoryInitializer());
            base.OnModelCreating(modelBuilder);
        }
        public ObjectContext ObjectContext
        {
            get
            {
                return (this as IObjectContextAdapter).ObjectContext;
            }
        }
    }
}

