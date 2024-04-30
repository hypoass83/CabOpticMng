using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSod.DataContext.Repositories;
using FatSod.DataContext.Services.BarCode;
using FatSod.Supply.Abstracts;
using FatSod.Budget.Entities;
using FatSod.Budget.Abstracts;
using FatSod.Supply.Abstracts.BarCode;

namespace CABOPMANAGEMENT.Tools
{
    public class NinjectDependencyResolver : IDependencyResolver, IDisposable
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            this.kernel = kernel;
            AddBindingDatas();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindingDatas()
        {
            //Définition de repositories qui pourront être injectées
            /*===================== Security Repositories  ========================*/
            kernel.Bind<IModule>().To<ModuleRepository>();
            kernel.Bind<IUser>().To<UserRepository>();
            kernel.Bind<IPerson>().To<PersonRepository>();
            kernel.Bind<IGlobalPerson>().To<GlobalPersonRepository>();
            kernel.Bind<IRepository<Person>>().To<Repository<Person>>();
            kernel.Bind<IRepository<Company>>().To<Repository<Company>>();
            kernel.Bind<IProfile>().To<ProfileRepository>();
            kernel.Bind<IRepository<User>>().To<Repository<User>>();
            kernel.Bind<IRepository<Module>>().To<Repository<Module>>();
            kernel.Bind<IRepository<Profile>>().To<Repository<Profile>>();
            kernel.Bind<IRepository<Menu>>().To<Repository<Menu>>();
            kernel.Bind<IRepository<SubMenu>>().To<Repository<SubMenu>>();
            kernel.Bind<IRepository<Town>>().To<Repository<Town>>();
            kernel.Bind<IRepository<Quarter>>().To<Repository<Quarter>>();
            kernel.Bind<IRepository<Country>>().To<Repository<Country>>();
            kernel.Bind<IRepository<Region>>().To<Repository<Region>>();
            kernel.Bind<IRepository<ActionSubMenuProfile>>().To<Repository<ActionSubMenuProfile>>();
            

            kernel.Bind<IRepository<File>>().To<Repository<File>>();
            kernel.Bind<IActionSubMenuProfile>().To<ActionSubMenuProfileRepository>();
            kernel.Bind<IRepository<ActionMenuProfile>>().To<Repository<ActionMenuProfile>>();
            kernel.Bind<IActionMenuProfile>().To<ActionMenuProfileRepository>();
            kernel.Bind<IRepository<Adress>>().To<Repository<Adress>>();
            kernel.Bind<IRepository<Sex>>().To<Repository<Sex>>();
            kernel.Bind<IRepository<Branch>>().To<Repository<Branch>>();
            kernel.Bind<IRepository<UserBranch>>().To<Repository<UserBranch>>();
            kernel.Bind<IRepository<Job>>().To<Repository<Job>>();
            kernel.Bind<IBusinessDay>().To<BusinessDayRepository>();
            kernel.Bind<IOpenAndCloseBusDay>().To<OpenAndCloseBusDay>();
            kernel.Bind<IRepository<ClosingDayTask>>().To<Repository<ClosingDayTask>>();
            kernel.Bind<IRepository<BranchClosingDayTask>>().To<Repository<BranchClosingDayTask>>();

            //binding de supply
            kernel.Bind<IRepositorySupply<Category>>().To<RepositorySupply<Category>>();
            kernel.Bind<IRepositorySupply<Product>>().To<RepositorySupply<Product>>();
            kernel.Bind<IRepositorySupply<GenericProduct>>().To<RepositorySupply<GenericProduct>>();
            kernel.Bind<IRepositorySupply<ProductLocalization>>().To<RepositorySupply<ProductLocalization>>();
            kernel.Bind<IRepositorySupply<SupplierOrderLine>>().To<RepositorySupply<SupplierOrderLine>>();
            kernel.Bind<IRepositorySupply<SupplierOrder>>().To<RepositorySupply<SupplierOrder>>();
            kernel.Bind<IRepositorySupply<PurchaseLine>>().To<RepositorySupply<PurchaseLine>>();
            kernel.Bind<IRepositorySupply<Line>>().To<RepositorySupply<Line>>();
            
            kernel.Bind<ISupplierReturn>().To<SupplierReturnRepository>();
            kernel.Bind<ILens>().To<LensRepository>();
            kernel.Bind<ILensNumberRangePrice>().To<PriceRepository>();
            kernel.Bind<IRepositorySupply<SupplierReturnLine>>().To<RepositorySupply<SupplierReturnLine>>();
            kernel.Bind<IRepositorySupply<Purchase>>().To<RepositorySupply<Purchase>>();
            kernel.Bind<IRepositorySupply<Localization>>().To<RepositorySupply<Localization>>();
            kernel.Bind<IRepositorySupply<Supplier>>().To<RepositorySupply<Supplier>>();
            kernel.Bind<IRepositorySupply<ProductTransfertLine>>().To<RepositorySupply<ProductTransfertLine>>();
            kernel.Bind<IProductLocalization>().To<PLRepository>();
            kernel.Bind<IPurchase>().To<PurchaseRepository>();
            kernel.Bind<IPaymentMethod>().To<PMRepository>();
            kernel.Bind<ISupplierOrder>().To<SupplierOrderRepository>();
            kernel.Bind<IRepositorySupply<Slice>>().To<RepositorySupply<Slice>>();
            kernel.Bind<IRepositorySupply<CustomerReturnSlice>>().To<RepositorySupply<CustomerReturnSlice>>();
            kernel.Bind<IRepositorySupply<SupplierSlice>>().To<RepositorySupply<SupplierSlice>>();
            kernel.Bind<IDeposit>().To<DepositRepository>();
            kernel.Bind<IRepositorySupply<EmployeeStock>>().To<RepositorySupply<EmployeeStock>>();
            kernel.Bind<IRepositorySupply<EmployeeStockHistoric>>().To<RepositorySupply<EmployeeStockHistoric>>();

            kernel.Bind<IRepositorySupply<Lens>>().To<RepositorySupply<Lens>>();
            kernel.Bind<IRepositorySupply<LensCoating>>().To<RepositorySupply<LensCoating>>();
            kernel.Bind<IRepositorySupply<LensColour>>().To<RepositorySupply<LensColour>>();
            kernel.Bind<IRepositorySupply<LensMaterial>>().To<RepositorySupply<LensMaterial>>();
            kernel.Bind<IRepositorySupply<LensNumber>>().To<RepositorySupply<LensNumber>>();
            kernel.Bind<IRepositorySupply<InventoryHistoric>>().To<RepositorySupply<InventoryHistoric>>();
            kernel.Bind<IRepositorySupply<LensNumberRange>>().To<RepositorySupply<LensNumberRange>>();
            kernel.Bind<ISavingAccount>().To<SavingAccountRepository>();
			kernel.Bind<ITillAdjust>().To<TillAdjustRepository>();
            kernel.Bind<ITreasuryOperation>().To<TreasuryOperationRepository>();
            kernel.Bind<ITransfert>().To<TransfertRepository>();
            kernel.Bind<IRepositorySupply<InventoryDirectoryLine>>().To<RepositorySupply<InventoryDirectoryLine>>();
            kernel.Bind<IInventoryDirectory>().To<InventoryDirectoryRepository>();
            kernel.Bind<IRepositorySupply<LensCategory>>().To<RepositorySupply<LensCategory>>();
            kernel.Bind<ILensCategory>().To<LensCategoryRepository>();

            kernel.Bind<IRepositorySupply<ATCD>>().To<RepositorySupply<ATCD>>();

            /*=======================================================================*/
            /*===================== Account repositories  ============================*/
            kernel.Bind<IRepositorySupply<AccountingSection>>().To<RepositorySupply<AccountingSection>>();
            //kernel.Bind<IRepositorySupply<CollectifAccount>>().To<IRepositorySupply<CollectifAccount>>();
            kernel.Bind<IRepositorySupply<CollectifAccount>>().To<RepositorySupply<CollectifAccount>>();
            kernel.Bind<IRepositorySupply<Account>>().To<RepositorySupply<Account>>();
            kernel.Bind<IRepositorySupply<Operation>>().To<RepositorySupply<Operation>>();
            kernel.Bind<IRepositorySupply<OperationType>>().To<RepositorySupply<OperationType>>();
            kernel.Bind<IRepositorySupply<AccountingTask>>().To<RepositorySupply<AccountingTask>>();
            kernel.Bind<IRepositorySupply<ClassAccount>>().To<RepositorySupply<ClassAccount>>();
            kernel.Bind<IRepositorySupply<MacroOperation>>().To<RepositorySupply<MacroOperation>>();
            
            kernel.Bind<IAccountOperation>().To<AccountOperationRepository>();
            kernel.Bind<IPiece>().To<PieceRepository>();
            kernel.Bind<ITransactNumber>().To<TransactNumberRepository>();
            kernel.Bind<ICompteurBorderoDepot>().To<CompteurBorderoDepotRepository>();
            kernel.Bind<IAccount>().To<AccountRepository>();
            kernel.Bind<IBudgetConsumption>().To<BudgetConsumptionRepository>();
            kernel.Bind<IRepositorySupply<BudgetAllocated>>().To<RepositorySupply<BudgetAllocated>>();

            /*=======================================================================*/
            /*===================== Sales repositories  ============================*/
            kernel.Bind<ISale>().To<SaleRepository>();
            kernel.Bind<ICustomerOrder>().To<CustomerOrderRepository>();
            kernel.Bind<IRepositorySupply<PaymentMethod>>().To<RepositorySupply<PaymentMethod>>();
            kernel.Bind<IRepositorySupply<TillDay>>().To<RepositorySupply<TillDay>>();
            kernel.Bind<IRepositorySupply<Devise>>().To<RepositorySupply<Devise>>();
            kernel.Bind<IRepositorySupply<Till>>().To<RepositorySupply<Till>>();
            kernel.Bind<IRepositorySupply<UserTill>>().To<RepositorySupply<UserTill>>();
            kernel.Bind<IRepositorySupply<SavingAccount>>().To<RepositorySupply<SavingAccount>>();
            kernel.Bind<IRepositorySupply<SaleLine>>().To<RepositorySupply<SaleLine>>();
            kernel.Bind<IRepositorySupply<CustomerOrderLine>>().To<RepositorySupply<CustomerOrderLine>>();
            kernel.Bind<IRepositorySupply<CustomerOrder>>().To<RepositorySupply<CustomerOrder>>();
            kernel.Bind<IRepositorySupply<CustomerReturnLine>>().To<RepositorySupply<CustomerReturnLine>>();
            kernel.Bind<ICustomerReturn>().To<CustomerReturnRepository>();
            kernel.Bind<IRepositorySupply<CustomerSlice>>().To<RepositorySupply<CustomerSlice>>();
            kernel.Bind<IRepositorySupply<Customer>>().To<RepositorySupply<Customer>>();
            kernel.Bind<IRepository<OrderLens>>().To<Repository<OrderLens>>();
            kernel.Bind<IRepositorySupply<Bank>>().To<RepositorySupply<Bank>>();
            kernel.Bind<ITillDay>().To<TillDayRepository>();

            kernel.Bind<IRepositorySupply<ProductBrand>>().To<RepositorySupply<ProductBrand>>();
            kernel.Bind<IRepositorySupply<LieuxdeDepotBordero>>().To<RepositorySupply<LieuxdeDepotBordero>>();
            kernel.Bind<IRepositorySupply<InsuredCompany>>().To<RepositorySupply<InsuredCompany>>();

            //Binding Budget
            kernel.Bind<IRepository<BudgetAllocated>>().To<Repository<BudgetAllocated>>();
            kernel.Bind<IRepository<BudgetAllocatedUpdate>>().To<Repository<BudgetAllocatedUpdate>>();
            kernel.Bind<IRepository<BudgetConsumption>>().To<Repository<BudgetConsumption>>();
            kernel.Bind<IRepository<BudgetConsumptionAccountOperation>>().To<Repository<BudgetConsumptionAccountOperation>>();
            kernel.Bind<IRepository<BudgetLine>>().To<Repository<BudgetLine>>();
            kernel.Bind<IRepository<FiscalYear>>().To<Repository<FiscalYear>>();
            kernel.Bind<IRepository<UserConfiguration>>().To<Repository<UserConfiguration>>();
            kernel.Bind<IRepository<PaymentMethod>>().To<Repository<PaymentMethod>>();

            kernel.Bind<IMouchar>().To<MoucharRepository>();
            kernel.Bind<IProductDamage>().To<ProductDamageRepository>();
            kernel.Bind<IStockReplacement>().To<StockReplacementRepository>();
            
            kernel.Bind<IRegProductNumber>().To<RegProductNumberRepository>();
            kernel.Bind<IProductGift>().To<ProductGiftRepository>();
            /*=======================================================================*/
            kernel.Bind<IBill>().To<BillRepository>();

            kernel.Bind<IRendezVous>().To<RendezVousRepository>();
            kernel.Bind<IHistoSMS>().To<HistoSMSRepository>();
            kernel.Bind<IExtractSMS>().To<ExtractSMSRepository>();
            kernel.Bind<IUserTill>().To<UserTillRepository>();
            kernel.Bind<IAuthoriseSale>().To<AuthoriseSaleRepository>();
            kernel.Bind<IPrescriptionLStep>().To<PrescriptionRepository>();
            kernel.Bind<INoPurchase>().To<NoPurchaseRepository>();
            kernel.Bind<INotificationSetting>().To<NotificationSettingRepository>();
            kernel.Bind<ICustomerSatisfaction>().To<CustomerSatisfactionRepository>();
            kernel.Bind<ICustomerComplaint>().To<CustomerComplaintRepository>();
            kernel.Bind<IComplaintFeedBack>().To<ComplaintFeedBackRepository>();
            kernel.Bind<IBarCodeService>().To<BarCodeService>();
            kernel.Bind<IDigitalPayment>().To<DigitalPaymentRepository>();
        }

        public void Dispose()
        {
            if (this.kernel != null)
                this.kernel.Dispose();
            this.kernel = null;
        }
    }
}