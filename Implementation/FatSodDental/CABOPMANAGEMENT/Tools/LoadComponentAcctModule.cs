using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using FatSod.DataContext.Repositories;
using FatSod.Security.Abstracts;

namespace CABOPMANAGEMENT.Tools
{
    public static partial class LoadComponent
    {

        /********************* Accounting combobox*************************/
        public static List<ClassAccount> ClassAccounts
        {
            get
            {
                context = new EFDbContext();
                List<ClassAccount> classAccountList = new List<ClassAccount>();
                foreach (ClassAccount classAccount in context.ClassAccounts.ToArray().OrderBy(m => m.ClassAccountNumber))
                {
                    classAccountList.Add(new ClassAccount { ClassAccountCode=classAccount.ClassAccountCode, ClassAccountID = classAccount.ClassAccountID });
                }
                return classAccountList;
            }
        }
        /*======= Accounting Section list */
        public static List<AccountingSection> AccountingSections
        {
            get
            {
                context = new EFDbContext();
                List<AccountingSection> accountingSectionList = new List<AccountingSection>();
                foreach (AccountingSection accountingSection in context.AccountingSections.ToArray().OrderBy(m => m.AccountingSectionNumber))
                {
                    accountingSectionList.Add(new AccountingSection { AccountingSectionLabel=accountingSection.AccountingSectionNumber.ToString() + "-" + accountingSection.AccountingSectionLabel, AccountingSectionID = accountingSection.AccountingSectionID });
                }
                return accountingSectionList;
            }
        }
        public static List<Account> GetAllAccounts
        {
            get
            {
                context = new EFDbContext();
                List<Account> accountingList = new List<Account>();
                foreach (Account accounting in context.Accounts.ToArray().OrderBy(m => m.AccountNumber))
                {
                    accountingList.Add(new Account { AccountLabel=accounting.AccountNumber.ToString() + "-" + accounting.AccountLabel, AccountID = accounting.AccountID });
                }
                return accountingList;
            }
        }
        public static List<Account> GetAllAccountNames
        {
            get
            {
                context = new EFDbContext();
                List<Account> accountingList = new List<Account>();
                foreach (Account accounting in context.Accounts.ToArray().OrderBy(m => m.AccountNumber))
                {
                    accountingList.Add(new Account { AccountLabel = accounting.AccountLabel + "-" + accounting.AccountNumber.ToString(), AccountID=accounting.AccountID });
                }
                return accountingList;
            }
        }
        public static List<Customer> GetCustomerNames
        {
            get 
            {
                context = new EFDbContext();
                List<Customer> customerList = new List<Customer>();
                var customerAct = context.Customers
                .Select(s => new
                {
                    CustomerFullName = s.Name.Trim(),
                    CNI = s.CNI.Trim(),
                    GlobalPersonID = s.GlobalPersonID
                }).ToList();
                foreach (var accounting in customerAct.OrderBy(m => m.CustomerFullName))
                {
                    customerList.Add(new Customer { Name = accounting.CustomerFullName.Trim() + "-" + accounting.CNI.Trim().ToString(), GlobalPersonID=accounting.GlobalPersonID });
                }
                return customerList;
            }
        }
        public static List<Account> GetCustomerAccountNames
        {
            get
            {
                context = new EFDbContext();
                List<Account> accountingList = new List<Account>();
                
                var customerAct = context.Accounts.Join(context.Customers, ac => ac.AccountID, cu => cu.AccountID, (ac, cu) => new { ac, cu })
                    .Select(s => new
                    {
                        AccountLabel = s.ac.AccountLabel,
                        AccountNumber=s.ac.AccountNumber,
                        AccountID = s.ac.AccountID
                    }).ToList();

                foreach (var accounting in customerAct.OrderBy(m => m.AccountNumber))
                {
                    accountingList.Add(new Account {AccountLabel= accounting.AccountLabel + "-" + accounting.AccountNumber.ToString(), AccountID=accounting.AccountID });
                }
                return accountingList;
            }
        }

        public static List<Account> GetCustomerFront
        {
            get
            {
                context = new EFDbContext();
                List<Account> accountingList = new List<Account>();

                var customerAct = context.Accounts.Join(context.Customers, ac => ac.AccountID, cu => cu.AccountID, (ac, cu) => new { ac, cu })
                    .Select(s => new
                    {
                        AccountLabel = s.cu.Name,
                        AccountID = s.cu.GlobalPersonID
                    }).ToList();

                foreach (var accounting in customerAct.OrderBy(m => m.AccountLabel))
                {
                    accountingList.Add(new Account { AccountLabel=accounting.AccountLabel, AccountID = accounting.AccountID });
                }
                return accountingList;
            }
                
        }
        public static List<CollectifAccount> GetCollectifAccounts
        {
            get
            {
                context = new EFDbContext();
                List<CollectifAccount> colAcctList = new List<CollectifAccount>();
                foreach (CollectifAccount collectiveaccount in context.CollectifAccounts.ToArray().OrderBy(m => m.CollectifAccountNumber))
                {
                    colAcctList.Add(new CollectifAccount {CollectifAccountLabel= collectiveaccount.CollectifAccountNumber.ToString() + "-" + collectiveaccount.CollectifAccountLabel, CollectifAccountID=collectiveaccount.CollectifAccountID });
                }
                return colAcctList;
            }
        }
        public static List<Account> GetTransportAccount
        {
            get
            {
                context = new EFDbContext();
                List<Account> accountingList = new List<Account>();
                foreach (Account accounting in context.Accounts.Where(a => a.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODESALETRANSP).ToArray().OrderBy(m => m.AccountNumber))
                {
                    accountingList.Add(new Account { AccountLabel=accounting.AccountNumber.ToString() + "-" + accounting.AccountLabel, AccountID = accounting.AccountID });
                }
                return accountingList;
            }
        }

        public static List<Account> GetManualPostingAccountNames
        {
            get
            {
                context = new EFDbContext();
                List<Account> accountingList = new List<Account>();
                foreach (Account accounting in context.Accounts.Where(m => m.isManualPosting).ToArray().OrderBy(m => m.AccountNumber))
                {
                    accountingList.Add(new Account { AccountLabel = accounting.AccountNumber.ToString() + "-" + accounting.AccountLabel, AccountID=accounting.AccountID });
                }
                return accountingList;
            }
        }
        //list of operation
        public static List<Operation> Operation
        {
            get
            {
                context = new EFDbContext();
                List<Operation> operationList = new List<Operation>();
                foreach (Operation operation in context.Operations.ToArray().OrderBy(m => m.OperationCode))
                {
                    operationList.Add(new Operation { OperationLabel=operation.OperationLabel, OperationID = operation.OperationID });
                }
                return operationList;
            }
        }
        //list of operation type
        public static List<OperationType> OperationType
        {
            get
            {
                context = new EFDbContext();
                List<OperationType> operationTypeList = new List<OperationType>();
                foreach (OperationType operationType in context.OperationTypes.ToArray().OrderBy(m => m.operationTypeCode))
                {
                    operationTypeList.Add(new OperationType { operationTypeCode=operationType.operationTypeCode, operationTypeID = operationType.operationTypeID });
                }
                return operationTypeList;
            }
        }

        //list of Macro Operation
        //public static List<MacroOperation> MacroOperation
        //{
        //    get
        //    {
        //        context = new EFDbContext();
        //        List<MacroOperation> macroOperationList = new List<MacroOperation>();
        //        foreach (MacroOperation macroOperation in context.MacroOperations.ToArray().OrderBy(m => m.MacroOperationCode))
        //        {
        //            macroOperationList.Add(new MacroOperation {MacroOperationCode= macroOperation.MacroOperationCode, MacroOperationID=macroOperation.MacroOperationID });
        //        }
        //        return macroOperationList;
        //    }
        //}

        //list of Reglement Type
        //public static List<ReglementType> ReglementType
        //{
        //    get
        //    {
        //        context = new EFDbContext();
        //        List<ReglementType> ReglementTypeList = new List<ReglementType>();
        //        foreach (ReglementType ReglementType in context.ReglementTypes.ToArray().OrderBy(m => m.ReglementTypeCode))
        //        {
        //            ReglementTypeList.Add(new ReglementType { ReglementTypeCode=ReglementType.ReglementTypeCode, ReglementTypeID = ReglementType.ReglementTypeID });
        //        }
        //        return ReglementTypeList;
        //    }
        //}
        public static List<Devise> GetDevise
        {
            get
            {
                context = new EFDbContext();
                List<Devise> DeviseList = new List<Devise>();
                foreach (Devise Devise in context.Devises.ToArray().OrderByDescending(m => m.DefaultDevise))
                {
                    DeviseList.Add(new Devise { DeviseCode=Devise.DeviseCode, DeviseID = Devise.DeviseID });

                }
                return DeviseList;
            }
        }

        public static List<Branch> GetOpenedBranches
        {
            get
            {

                IBusinessDay busDayRepo = new BusinessDayRepository();
                List<Branch> openedBranchesList = new List<Branch>();
                List<Branch> openedBranches = busDayRepo.GetOpenedBranches();
                foreach (Branch branch in openedBranches)
                {
                    openedBranchesList.Add(new Branch { BranchName=branch.BranchName, BranchID = branch.BranchID });

                }
                return openedBranchesList;
            }
        }

        public static int GetOperationTypeID(string CodeOp)
        {
            context = new EFDbContext();
            return context.OperationTypes.FirstOrDefault(o => o.operationTypeCode == CodeOp).operationTypeID;
        }
        //This method return list of payment method 
        public static List<object> SpecificTillPaymentMethod(string BuyTypeCode)
        {
            context = new EFDbContext();
            List<object> model = new List<object>();
            context.PaymentMethods.OfType<Till>().Where(p => p.Account.CollectifAccount.AccountingSection.AccountingSectionCode == BuyTypeCode).ToList().ForEach(p =>
            {
                model.Add(
                        new
                        {
                            ID = p.ID,
                            Name = p.Name
                        }
                    );
            });
            return model;
        }
        public static List<object> SpecificBankPaymentMethod(string BuyTypeCode)
        {
            context = new EFDbContext();
            List<object> model = new List<object>();
            context.PaymentMethods.OfType<Bank>().Where(p => p.Account.CollectifAccount.AccountingSection.AccountingSectionCode == BuyTypeCode).ToList().ForEach(p =>
            {
                model.Add(
                        new
                        {
                            ID = p.ID,
                            Name = p.Name
                        }
                    );
            });
            return model;
        }
        public static List<Bank> BankPaymentMethod
        {
            get
            {
                context = new EFDbContext();
                List<Bank> model = new List<Bank>();
                string BuyTypeCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK;
                context.PaymentMethods.OfType<Bank>().Where(p => p.Account.CollectifAccount.AccountingSection.AccountingSectionCode == BuyTypeCode).ToList().ForEach(p =>
                {
                    model.Add(new Bank { Name=p.Name, ID = p.ID });
                });
                return model;
            }
        }
    }
}