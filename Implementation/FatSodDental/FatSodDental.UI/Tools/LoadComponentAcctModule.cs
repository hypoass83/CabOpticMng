using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using Ext.Net;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using FatSod.DataContext.Repositories;
using FatSod.Security.Abstracts;

namespace FatSodDental.UI.Tools
{
    public static partial class LoadComponent
    {

        /********************* Accounting combobox*************************/
        public static List<ListItem> ClassAccounts
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> classAccountList = new List<ListItem>();
                foreach (ClassAccount classAccount in context.ClassAccounts.ToArray().OrderBy(m => m.ClassAccountNumber))
                {
                    classAccountList.Add(new ListItem(classAccount.ClassAccountCode, classAccount.ClassAccountID));
                }
                return classAccountList;
            }
        }
        /*======= Accounting Section list */
        public static List<ListItem> AccountingSections
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> accountingSectionList = new List<ListItem>();
                foreach (AccountingSection accountingSection in context.AccountingSections.ToArray().OrderBy(m => m.AccountingSectionNumber))
                {
                    accountingSectionList.Add(new ListItem(accountingSection.AccountingSectionNumber.ToString()+"-"+accountingSection.AccountingSectionLabel, accountingSection.AccountingSectionID));
                }
                return accountingSectionList;
            }
        }
        public static List<ListItem> GetAllAccounts
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> accountingList = new List<ListItem>();
                foreach (Account accounting in context.Accounts.ToArray().OrderBy(m => m.AccountNumber))
                {
                    accountingList.Add(new ListItem(accounting.AccountNumber.ToString()+"-"+accounting.AccountLabel, accounting.AccountID));
                }
                return accountingList;
            }
        }
        public static List<ListItem> GetAllInsurance
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> InsuranceList = new List<ListItem>();
                foreach (Assureur assurance in context.Assureurs.ToArray().OrderBy(m => m.Name))
                {
                    InsuranceList.Add(new ListItem(assurance.Name, assurance.GlobalPersonID));
                }
                return InsuranceList;
            }
        }
        public static List<ListItem> GetAllAccountNames
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> accountingList = new List<ListItem>();
                foreach (Account accounting in context.Accounts.ToArray().OrderBy(m => m.AccountNumber))
                {
                    accountingList.Add(new ListItem(accounting.AccountLabel + "-" + accounting.AccountNumber.ToString(), accounting.AccountID));
                }
                return accountingList;
            }
        }
        public static List<ListItem> GetCustomerAccountNames
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> accountingList = new List<ListItem>();
                
                var customerAct = context.Accounts.Join(context.Customers, ac => ac.AccountID, cu => cu.AccountID, (ac, cu) => new { ac, cu })
                    .Select(s => new
                    {
                        AccountLabel = s.ac.AccountLabel,
                        AccountNumber=s.ac.AccountNumber,
                        AccountID = s.ac.AccountID
                    }).ToList();

                foreach (var accounting in customerAct.OrderBy(m => m.AccountNumber))
                {
                    accountingList.Add(new ListItem(accounting.AccountLabel + "-" + accounting.AccountNumber.ToString(), accounting.AccountID));
                }
                return accountingList;
            }
        }

        public static List<ListItem> GetCustomerFront
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> accountingList = new List<ListItem>();

                var customerAct = context.Accounts.Join(context.Customers, ac => ac.AccountID, cu => cu.AccountID, (ac, cu) => new { ac, cu })
                    .Select(s => new
                    {
                        AccountLabel = s.cu.Name,
                        AccountID = s.cu.GlobalPersonID
                    }).ToList();

                foreach (var accounting in customerAct.OrderBy(m => m.AccountLabel))
                {
                    accountingList.Add(new ListItem(accounting.AccountLabel, accounting.AccountID));
                }
                return accountingList;
            }
                
        }
        public static List<ListItem> GetCollectifAccounts
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> colAcctList = new List<ListItem>();
                foreach (CollectifAccount collectiveaccount in context.CollectifAccounts.ToArray().OrderBy(m => m.CollectifAccountNumber))
                {
                    colAcctList.Add(new ListItem(collectiveaccount.CollectifAccountNumber.ToString() + "-" + collectiveaccount.CollectifAccountLabel, collectiveaccount.CollectifAccountID));
                }
                return colAcctList;
            }
        }
        public static List<ListItem> GetTransportAccount
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> accountingList = new List<ListItem>();
                foreach (Account accounting in context.Accounts.Where(a => a.CollectifAccount.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODESALETRANSP).ToArray().OrderBy(m => m.AccountNumber))
                {
                    accountingList.Add(new ListItem(accounting.AccountNumber.ToString() + "-" + accounting.AccountLabel, accounting.AccountID));
                }
                return accountingList;
            }
        }
        
        public static List<ListItem> GetManualPostingAccountNames
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> accountingList = new List<ListItem>();
                foreach (Account accounting in context.Accounts.Where(m => m.isManualPosting).ToArray().OrderBy(m => m.AccountNumber))
                {
                    accountingList.Add(new ListItem(accounting.AccountNumber.ToString() + "-" + accounting.AccountLabel, accounting.AccountID));
                }
                return accountingList;
            }
        }
        //list of operation
        public static List<ListItem> Operation
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> operationList = new List<ListItem>();
                foreach (Operation operation in context.Operations.ToArray().OrderBy(m => m.OperationCode))
                {
                    operationList.Add(new ListItem(operation.OperationLabel, operation.OperationID));
                }
                return operationList;
            }
        }
        //list of operation type
        public static List<ListItem> OperationType
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> operationTypeList = new List<ListItem>();
                foreach (OperationType operationType in context.OperationTypes.ToArray().OrderBy(m => m.operationTypeCode))
                {
                    operationTypeList.Add(new ListItem(operationType.operationTypeCode, operationType.operationTypeID));
                }
                return operationTypeList;
            }
        }

        //list of Macro Operation
        public static List<ListItem> MacroOperation
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> macroOperationList = new List<ListItem>();
                foreach (MacroOperation macroOperation in context.MacroOperations.ToArray().OrderBy(m => m.MacroOperationCode))
                {
                    macroOperationList.Add(new ListItem(macroOperation.MacroOperationCode, macroOperation.MacroOperationID));
                }
                return macroOperationList;
            }
        }

        //list of Reglement Type
        public static List<ListItem> ReglementType
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> ReglementTypeList = new List<ListItem>();
                foreach (ReglementType ReglementType in context.ReglementTypes.ToArray().OrderBy(m => m.ReglementTypeCode))
                {
                    ReglementTypeList.Add(new ListItem(ReglementType.ReglementTypeCode, ReglementType.ReglementTypeID));
                }
                return ReglementTypeList;
            }
        }
        public static List<ListItem> GetDevise
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> DeviseList = new List<ListItem>();
                foreach (Devise Devise in context.Devises.ToArray().OrderByDescending(m => m.DefaultDevise))
                {
                    DeviseList.Add(new ListItem(Devise.DeviseCode, Devise.DeviseID));

                }
                return DeviseList;
            }
        }

        public static List<ListItem> GetOpenedBranches
        {
            get
            {

                IBusinessDay busDayRepo = new BusinessDayRepository();
                List<ListItem> openedBranchesList = new List<ListItem>();
                List<Branch> openedBranches = busDayRepo.GetOpenedBranches();
                foreach (Branch branch in openedBranches)
                {
                    openedBranchesList.Add(new ListItem(branch.BranchName, branch.BranchID));

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
        public static List<ListItem> BankPaymentMethod
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> model = new List<ListItem>();
                string BuyTypeCode = CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK;
                context.PaymentMethods.OfType<Bank>().Where(p => p.Account.CollectifAccount.AccountingSection.AccountingSectionCode == BuyTypeCode).ToList().ForEach(p =>
                {
                    model.Add(new ListItem(p.Name, p.ID));
                });
                return model;
            }
        }
    }
}