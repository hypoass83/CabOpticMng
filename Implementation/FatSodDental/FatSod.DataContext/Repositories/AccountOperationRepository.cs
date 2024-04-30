using FastSod.Utilities.Util;
using FatSod.Budget.Entities;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
    /// <summary>
    /// 
    /// </summary>
	public partial class AccountOperationRepository : RepositorySupply<AccountOperation>, IAccountOperation
	{
        ITransactNumber _trnRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
		public AccountOperationRepository(EFDbContext context)
		{
			this.context = context;
		}
        /// <summary>
        /// 
        /// </summary>
		public AccountOperationRepository()
			: base()
		{

		}
        
        private bool EcritHistoGrandLivreRegelementAvecAvance(EcritureGLHist eGL)
        {
            bool res = true;
            double Amount = 0d;
            int i = 0;
            try
            {
                //recup du type operation
                Operation operationType = context.Operations.Where(a => a.OperationID == eGL.OperationID).SingleOrDefault();
                int operationTypeID = (operationType == null) ? 0 : operationType.OperationTypeID;

                //recuperation des taches des operation deja configure
                foreach (AccountingTask lt in context.AccountingTasks.Where(a => a.OperationID == eGL.OperationID).ToList().OrderByDescending(o => o.AccountingTaskSens))
                {
                    //fabrication de l'objet accountingoperation
                    AccountOperation accountoperation = new AccountOperation();
                    accountoperation.BranchID = eGL.BranchID;
                    accountoperation.DeviseID = eGL.DeviseID;
                    accountoperation.CodeTransaction = eGL.CodeTransaction;
                    accountoperation.DateOperation = eGL.DateOperation;
                    accountoperation.Description = lt.AccountingTaskDescription + ": " + eGL.Description;
                    accountoperation.OperationID = eGL.OperationID;
                    accountoperation.Reference = eGL.Reference;

                    //recup du ACCTSECTIONCODE
                    string acctSectionCode = context.AccountingSections.SingleOrDefault(a => a.AccountingSectionID == lt.AccountingSectionID).AccountingSectionCode;

                    //recuperation du compte 
                    if (lt.AccountID != null && lt.AccountID > 0)
                    {
                        accountoperation.AccountID = (int)lt.AccountID;
                        if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT)
                        {
                            accountoperation.AccountTierID = eGL.AccountIDTierCusto;
                        }
                        else
                        {
                            accountoperation.AccountTierID = null;
                        }
                    }
                    else
                    {
                        if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT)
                        {
                            //cpte product
                            accountoperation.AccountID = eGL.AccountIDTierCusto;
                            accountoperation.AccountTierID = eGL.AccountIDTierCusto;
                        }
                        else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD)
                        {
                            //cpte product
                            accountoperation.AccountID = eGL.AccountIDTierProduct;
                            accountoperation.AccountTierID = null;
                        }
                        else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK)
                        {
                            //cpte treso
                            accountoperation.AccountID = eGL.AccountIDTresor;
                            accountoperation.AccountTierID = null;
                        }
                        else
                        {
                            res = false;
                            throw new Exception("Error while running Accounting Operation: Bad configuration Account number for Account Section" + acctSectionCode + ". Contact Your Provider");
                        }
                    }

                    if (i == 0) Amount = eGL.MontantClientDeposit;
                    if (i == 1) Amount = eGL.MontantTotalClientAdvance;
                    if (i == 2) Amount = eGL.MontantTotalClientAdvance+ eGL.MontantClientDeposit;
                    //ecriture ds le gl
                    res = ecritureLigneGLRegelementAvecAvance(accountoperation, operationTypeID, lt.AccountingTaskSens.ToUpper(), eGL.idSalePurchage, Amount);
                    if (!res)
                    {
                        res = false;
                        throw new Exception("Error while running Accounting Operation. Contact Your Provider");
                    }
                    i = i + 1;
                }

                return res;
            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }
        }

        private bool ecritureLigneGLRegelementAvecAvance(AccountOperation accountOperation,  int operationTypeID, string sensop, int idSalePurchage, double Amount)
        {
            bool res = true;
            //recuperation de la table fille
            OperationType opType = context.OperationTypes.SingleOrDefault(op => op.operationTypeID == operationTypeID);
            string operationtypecode = opType != null ? opType.operationTypeCode : "";

            if (sensop.ToUpper().Trim() == "DB")
            {
                if (Amount > 0) //mtant principal
                {
                    SaleAccountOperation glOperation = new SaleAccountOperation();
                    //glOperation.AccountOperationID = glOperation.AccountOperationID;
                    glOperation.BranchID = accountOperation.BranchID;
                    glOperation.DeviseID = accountOperation.DeviseID;
                    glOperation.OperationID = accountOperation.OperationID;
                    glOperation.DateOperation = accountOperation.DateOperation;
                    glOperation.CodeTransaction = accountOperation.CodeTransaction;
                    glOperation.Description = accountOperation.Description;
                    glOperation.Reference = accountOperation.Reference;
                    glOperation.SaleID = idSalePurchage;
                    glOperation.AccountID = accountOperation.AccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
                    glOperation.Credit = 0;
                    glOperation.Debit = Amount;
                    context.AccountOperations.Add(glOperation);
                    context.SaveChanges();
                    res = true;
                }

            }
            else //credit account
            {
                if (Amount > 0) //mtant principal
                {
                    SaleAccountOperation glOperation = new SaleAccountOperation();
                    glOperation.BranchID = accountOperation.BranchID;
                    glOperation.DeviseID = accountOperation.DeviseID;
                    glOperation.OperationID = accountOperation.OperationID;
                    glOperation.DateOperation = accountOperation.DateOperation;
                    glOperation.CodeTransaction = accountOperation.CodeTransaction;
                    glOperation.Description = accountOperation.Description;
                    glOperation.Reference = accountOperation.Reference;
                    glOperation.SaleID = idSalePurchage;
                    glOperation.AccountID = accountOperation.AccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
                    glOperation.Credit = Amount;
                    glOperation.Debit = 0;
                    context.AccountOperations.Add(glOperation);
                    context.SaveChanges();
                    res = true;
                }
               
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eGL"></param>
        /// <param name="isDeposit"></param>
        /// <returns></returns>
        private bool EcritHistoGrandLivreReglementRetourProduct (EcritureGLHist eGL, bool isDeposit = false)
		{
			bool res = true;
			
			try
			{
				//recup du type operation
				Operation operationType = context.Operations.Where(a => a.OperationID == eGL.OperationID).SingleOrDefault();
				int operationTypeID = (operationType == null) ? 0 : operationType.OperationTypeID;

				//recuperation des taches des operation deja configure
				foreach (AccountingTask lt in context.AccountingTasks.Where(a => a.OperationID == eGL.OperationID).ToList().OrderByDescending(o => o.AccountingTaskSens))
				{
					//fabrication de l'objet accountingoperation
					AccountOperation accountoperation = new AccountOperation();
					accountoperation.BranchID = eGL.BranchID;
					accountoperation.DeviseID = eGL.DeviseID;
					accountoperation.CodeTransaction = eGL.CodeTransaction;
					accountoperation.DateOperation = eGL.DateOperation;
					accountoperation.Description = lt.AccountingTaskDescription + ": " + eGL.Description;
					accountoperation.OperationID = eGL.OperationID;
					accountoperation.Reference = eGL.Reference;

                    //recup du ACCTSECTIONCODE
                    string acctSectionCode = context.AccountingSections.SingleOrDefault(a => a.AccountingSectionID == lt.AccountingSectionID).AccountingSectionCode;
						
					//recuperation du compte 
					if (lt.AccountID != null || lt.AccountID > 0)
					{
						accountoperation.AccountID = (int)lt.AccountID;
                        if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT)
                        {
                            accountoperation.AccountTierID = eGL.AccountIDTierCusto;
                        }
                        else 
                        {
                            accountoperation.AccountTierID = null;
                        }
					}
					else
					{
                        if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT)
						{
							//cpte product
							accountoperation.AccountID = eGL.AccountIDTierCusto;
                            accountoperation.AccountTierID = eGL.AccountIDTierCusto;
						}
						else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD)
						{
							//cpte product
							accountoperation.AccountID = eGL.AccountIDTierProduct;
                            accountoperation.AccountTierID = null;
						}
						else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK)
						{
							//cpte treso
							accountoperation.AccountID = eGL.AccountIDTresor;
                            accountoperation.AccountTierID = null;
						}
						else
						{
							res = false;
							throw new Exception("Error while running Accounting Operation: Bad configuration Account number for Account Section" + acctSectionCode + ". Contact Your Provider");
						}
					}
					
					//ecriture ds le gl
					res = ecritureLigneGL(accountoperation, eGL.MontantPrincDB, eGL.MontantPrincCR, operationTypeID, (lt.AccountingTaskSens.ToUpper()=="DB") ? "CR":"DB", eGL.idSalePurchage, eGL.TVAAmount, 0,
							eGL.Discount, 0, eGL.Transport, 0, isDeposit);
					if (!res)
					{
						res = false;
						throw new Exception("Error while running Accounting Operation. Contact Your Provider");
					}
				}

				return res;
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}

		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eGL"></param>
        /// <param name="isDeposit"></param>
        /// <returns></returns>
        public bool EcritHistoGrandLivreLivraisonAvecAvance(EcritureGLHist eGL, bool isDeposit = false)
        {
            bool res = true;
            int vatAccountID = 0;
            int DiscountAccountID = 0;
            int TransportAccountID = 0;
            double MontantADebiterouCrediter = 0d;

            try
            {
                //recup du type operation
                Operation operationType = context.Operations.Where(a => a.OperationID == eGL.OperationID).SingleOrDefault();
                int operationTypeID = (operationType == null) ? 0 : operationType.OperationTypeID;

                //recuperation des taches des operation deja configure
                foreach (AccountingTask lt in context.AccountingTasks.Where(a => a.OperationID == eGL.OperationID).ToList().OrderByDescending(o => o.AccountingTaskSens))
                {
                    MontantADebiterouCrediter = 0;
                    //fabrication de l'objet accountingoperation
                    AccountOperation accountoperation = new AccountOperation();
                    accountoperation.BranchID = eGL.BranchID;
                    accountoperation.DeviseID = eGL.DeviseID;
                    accountoperation.CodeTransaction = eGL.CodeTransaction;
                    accountoperation.DateOperation = eGL.DateOperation;
                    accountoperation.Description = lt.AccountingTaskDescription + ": " + eGL.Description;
                    accountoperation.OperationID = eGL.OperationID;
                    accountoperation.Reference = eGL.Reference;

                    //recup du ACCTSECTIONCODE
                    string acctSectionCode = context.AccountingSections.SingleOrDefault(a => a.AccountingSectionID == lt.AccountingSectionID).AccountingSectionCode;

                    //recuperation du compte 
                    if (lt.AccountID != null && lt.AccountID > 0)
                    {
                        MontantADebiterouCrediter = eGL.MontantTotalClientAdvance;
                        accountoperation.AccountID = (int)lt.AccountID;
                        if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT)
                        {
                            accountoperation.AccountTierID = eGL.AccountIDTierCusto;
                        }
                        else
                        {
                            accountoperation.AccountTierID = null;
                        }
                    }
                    else
                    {
                        if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT)
                        {
                            //cpte product
                            accountoperation.AccountID = eGL.AccountIDTierCusto;
                            accountoperation.AccountTierID = eGL.AccountIDTierCusto;
                            MontantADebiterouCrediter = eGL.MontantPrincCR;
                        }
                        else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD)
                        {
                            //cpte product
                            accountoperation.AccountID = eGL.AccountIDTierProduct;
                            accountoperation.AccountTierID = null;
                            MontantADebiterouCrediter = eGL.MontantPrincCR;
                        }
                        else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK)
                        {
                            //cpte treso
                            accountoperation.AccountID = eGL.AccountIDTresor;
                            accountoperation.AccountTierID = null;
                            MontantADebiterouCrediter = eGL.MontantClientDeposit;
                        }
                        else
                        {
                            res = false;
                            throw new Exception("Error while running Accounting Operation: Bad configuration Account number for Account Section" + acctSectionCode + ". Contact Your Provider");
                        }
                    }
                    //cpte tva
                    if (lt.VatAccountID != null || lt.VatAccountID > 0)
                    {
                        vatAccountID = (int)lt.VatAccountID;
                    }
                    else
                    {
                        vatAccountID = 0;
                    }
                    //cpte Discount
                    if (lt.DiscountAccountID != null || lt.DiscountAccountID > 0)
                    {
                        DiscountAccountID = (int)lt.DiscountAccountID;
                    }
                    else
                    {
                        DiscountAccountID = 0;
                    }
                    //cpte Transport
                    if (lt.TransportAccountID != null || lt.TransportAccountID > 0)
                    {
                        TransportAccountID = (int)lt.TransportAccountID;
                    }
                    else
                    {
                        TransportAccountID = 0;
                    }

                    //ecriture ds le gl
                    res = ecritureLigneGL(accountoperation, MontantADebiterouCrediter, eGL.MontantPrincCR, operationTypeID, lt.AccountingTaskSens, eGL.idSalePurchage, eGL.TVAAmount, vatAccountID,
                            eGL.Discount, DiscountAccountID, eGL.Transport, TransportAccountID, isDeposit);
                    if (!res)
                    {
                        res = false;
                        throw new Exception("Error while running Accounting Operation. Contact Your Provider");
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eGL"></param>
        /// <param name="isDeposit"></param>
        /// <returns></returns>
        private bool EcritHistoGrandLivre(EcritureGLHist eGL, bool isDeposit = false)
		{
			bool res = true;
			int vatAccountID = 0;
			int DiscountAccountID = 0;
			int TransportAccountID = 0;

			try
			{
				//recup du type operation
				Operation operationType = context.Operations.Where(a => a.OperationID == eGL.OperationID).SingleOrDefault();
				int operationTypeID = (operationType == null) ? 0 : operationType.OperationTypeID;

				//recuperation des taches des operation deja configure
				foreach (AccountingTask lt in context.AccountingTasks.Where(a => a.OperationID == eGL.OperationID).ToList().OrderByDescending(o => o.AccountingTaskSens))
				{
					//fabrication de l'objet accountingoperation
					AccountOperation accountoperation = new AccountOperation();
					accountoperation.BranchID = eGL.BranchID;
					accountoperation.DeviseID = eGL.DeviseID;
					accountoperation.CodeTransaction = eGL.CodeTransaction;
					accountoperation.DateOperation = eGL.DateOperation;
					accountoperation.Description = lt.AccountingTaskDescription + ": " + eGL.Description;
					accountoperation.OperationID = eGL.OperationID;
					accountoperation.Reference = eGL.Reference;

                    //recup du ACCTSECTIONCODE
                    string acctSectionCode = context.AccountingSections.SingleOrDefault(a => a.AccountingSectionID == lt.AccountingSectionID).AccountingSectionCode;
						
					//recuperation du compte 
					if (lt.AccountID != null && lt.AccountID > 0)
					{
						accountoperation.AccountID = (int)lt.AccountID;
                        if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT)
                        {
                            accountoperation.AccountTierID = eGL.AccountIDTierCusto;
                        }
                        else 
                        {
                            accountoperation.AccountTierID = null;
                        }
					}
					else
					{
                        if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT)
						{
							//cpte product
							accountoperation.AccountID = eGL.AccountIDTierCusto;
                            accountoperation.AccountTierID = eGL.AccountIDTierCusto;
						}
						else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD)
						{
							//cpte product
							accountoperation.AccountID = eGL.AccountIDTierProduct;
                            accountoperation.AccountTierID = null;
						}
						else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK)
						{
							//cpte treso
							accountoperation.AccountID = eGL.AccountIDTresor;
                            accountoperation.AccountTierID = null;
						}
						else
						{
							res = false;
							throw new Exception("Error while running Accounting Operation: Bad configuration Account number for Account Section" + acctSectionCode + ". Contact Your Provider");
						}
					}
					//cpte tva
					if (lt.VatAccountID != null && lt.VatAccountID > 0)
					{
						vatAccountID = (int)lt.VatAccountID;
					}
					else
					{
						vatAccountID = 0;
					}
					//cpte Discount
					if (lt.DiscountAccountID != null && lt.DiscountAccountID > 0)
					{
						DiscountAccountID = (int)lt.DiscountAccountID;
					}
					else
					{
						DiscountAccountID = 0;
					}
					//cpte Transport
					if (lt.TransportAccountID != null && lt.TransportAccountID > 0)
					{
						TransportAccountID = (int)lt.TransportAccountID;
					}
					else
					{
						TransportAccountID = 0;
					}
					
					//ecriture ds le gl
					res = ecritureLigneGL(accountoperation, eGL.MontantPrincDB, eGL.MontantPrincCR, operationTypeID, lt.AccountingTaskSens, eGL.idSalePurchage, eGL.TVAAmount, vatAccountID,
							eGL.Discount, DiscountAccountID, eGL.Transport, TransportAccountID, isDeposit);
					if (!res)
					{
						res = false;
						throw new Exception("Error while running Accounting Operation. Contact Your Provider");
					}
				}

				return res;
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}

		}

		private bool EcritHistoGrandLivreAdvSpecialOrder(EcritureGLHist eGL)
		{
			bool res = true;

			try
			{
				//recup du type operation
				Operation operationType = context.Operations.Where(a => a.OperationID == eGL.OperationID).SingleOrDefault();
				int operationTypeID = (operationType == null) ? 0 : operationType.OperationTypeID;

				//recuperation des taches des operation deja configure
				foreach (AccountingTask lt in context.AccountingTasks.Where(a => a.OperationID == eGL.OperationID).ToList().OrderByDescending(o => o.AccountingTaskSens))
				{
					//fabrication de l'objet accountingoperation
					AccountOperation accountoperation = new AccountOperation();
					accountoperation.BranchID = eGL.BranchID;
					accountoperation.DeviseID = eGL.DeviseID;
					accountoperation.CodeTransaction = eGL.CodeTransaction;
					accountoperation.DateOperation = eGL.DateOperation;
					accountoperation.Description = lt.AccountingTaskDescription + ": " + eGL.Description;
					accountoperation.OperationID = eGL.OperationID;
					accountoperation.Reference = eGL.Reference;
					//recuperation du compte 
					/*
                    if (lt.AccountID != null || lt.AccountID > 0)
					{
						accountoperation.AccountID = (int)lt.AccountID;
					}
					else
					{
						//recup du ACCTSECTIONCODE
						string acctSectionCode = context.AccountingSections.SingleOrDefault(a => a.AccountingSectionID == lt.AccountingSectionID).AccountingSectionCode;
						if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN)
						{
							//cpte product
							accountoperation.AccountID = eGL.AccountIDTierCusto;
						}
						else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD)
						{
							//cpte product
							accountoperation.AccountID = eGL.AccountIDTierProduct;
						}
						else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK)
						{
							//cpte treso
							accountoperation.AccountID = eGL.AccountIDTresor;
						}
						else
						{
							res = false;
							throw new Exception("Error while running Accounting Operation: Bad configuration Account number for Account Section" + acctSectionCode + ". Contact Your Provider");
						}
					}
					accountoperation.AccountTierID = eGL.AccountIDTierCusto;
                    */
                    //recup du ACCTSECTIONCODE
                    string acctSectionCode = context.AccountingSections.SingleOrDefault(a => a.AccountingSectionID == lt.AccountingSectionID).AccountingSectionCode;
					
                    //recuperation du compte 
                    if (lt.AccountID != null && lt.AccountID > 0)
                    {
                        accountoperation.AccountID = (int)lt.AccountID;
                        if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT)
                        {
                            accountoperation.AccountTierID = eGL.AccountIDTierCusto;
                        }
                        else
                        {
                            accountoperation.AccountTierID = null;
                        }
                    }
                    else
                    {
                        if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEFOURN || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEVENTE || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEAVANCEACHAT)
                        {
                            //cpte product
                            accountoperation.AccountID = eGL.AccountIDTierCusto;
                            accountoperation.AccountTierID = eGL.AccountIDTierCusto;
                        }
                        else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD)
                        {
                            //cpte product
                            accountoperation.AccountID = eGL.AccountIDTierProduct;
                            accountoperation.AccountTierID = null;
                        }
                        else if (acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS || acctSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK)
                        {
                            //cpte treso
                            accountoperation.AccountID = eGL.AccountIDTresor;
                            accountoperation.AccountTierID = null;
                        }
                        else
                        {
                            res = false;
                            throw new Exception("Error while running Accounting Operation: Bad configuration Account number for Account Section" + acctSectionCode + ". Contact Your Provider");
                        }
                    }
					if (lt.AccountingTaskSens.ToUpper().Trim() == "DB")
					{
						if (eGL.MontantPrincDB > 0) //mtant principal
						{
							accountoperation.Debit = eGL.MontantPrincDB;
							accountoperation.Credit = 0;
						}
					}

					if (lt.AccountingTaskSens.ToUpper().Trim() == "CR")
					{
						if (eGL.MontantPrincCR > 0) //mtant principal
						{
							accountoperation.Debit = 0;
							accountoperation.Credit = eGL.MontantPrincCR;
						}
					}

					if (eGL.MontantPrincCR == 0 && eGL.MontantPrincDB==0)
					{
						res = false;
						throw new Exception("Error while running Accounting Operation: Debit and Credit Amount is equal to Zero. Contact Your Provider");
					}

					context.AccountOperations.Add(accountoperation);
					context.SaveChanges();
				}
				
				return res;
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
		}

		private bool EcritHistoGrandLivreBudget(EcritureGLHist eGL)
		{
			bool res = true;

			try
			{
				//recup de l'id de l'operation
				int operationID = 0;
				OperationType operationType = context.OperationTypes.Where(a => a.operationTypeCode == CodeValue.Accounting.InitOperationType.CODEBUDGET).SingleOrDefault();
				if (operationType != null)
				{
					Operation operation = context.Operations.Where(o => o.OperationTypeID == operationType.operationTypeID).FirstOrDefault();
					operationID = (operation != null) ? operation.OperationID : 0;
				}
				if (operationID > 0)
				{
					//debit du cpte de budget
					if (eGL.MontantPrincDB > 0)
					{
						BudgetConsumptionAccountOperation glDBOperation = new BudgetConsumptionAccountOperation();
						glDBOperation.BranchID = eGL.BranchID;
						glDBOperation.DeviseID = eGL.DeviseID;
						glDBOperation.OperationID = operationID;
						glDBOperation.DateOperation = eGL.DateOperation;
						glDBOperation.CodeTransaction = eGL.CodeTransaction;
						glDBOperation.Description = eGL.Description;
						glDBOperation.Reference = eGL.Reference;
						glDBOperation.BudgetConsumptionID = eGL.idSalePurchage;
						glDBOperation.AccountID = eGL.AccountIDTierCusto;
						glDBOperation.Credit = 0;
						glDBOperation.Debit = eGL.MontantPrincDB;
						context.AccountOperations.Add(glDBOperation);
						context.SaveChanges();
					}

					//credit du cpte de tresorerie
					if (eGL.MontantPrincCR > 0)
					{
						BudgetConsumptionAccountOperation glCROperation = new BudgetConsumptionAccountOperation();
						glCROperation.BranchID = eGL.BranchID;
						glCROperation.DeviseID = eGL.DeviseID;
						glCROperation.OperationID = operationID;
						glCROperation.DateOperation = eGL.DateOperation;
						glCROperation.CodeTransaction = eGL.CodeTransaction;
						glCROperation.Description = eGL.Description;
						glCROperation.Reference = eGL.Reference;
						glCROperation.BudgetConsumptionID = eGL.idSalePurchage;
						glCROperation.AccountID = eGL.AccountIDTresor;
						glCROperation.Credit = eGL.MontantPrincCR;
						glCROperation.Debit = 0;
						context.AccountOperations.Add(glCROperation);
						context.SaveChanges();
					}
					res = true;
				}
				else
				{
					res = false;
					throw new Exception("Error while running Accounting Operation. Bad configuration of Operation " + CodeValue.Accounting.InitOperationType.CODEBUDGET);
				}

				return res;
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}

		}

        private bool EcritHistoGrandLivreTreasury(EcritureGLHist eGL)
        {
            bool res = true;

            try
            {
                //recup de l'id de l'operation
                int operationID = 0;
                OperationType operationType = context.OperationTypes.Where(a => a.operationTypeCode == CodeValue.Accounting.InitOperationType.CODETREASURY).SingleOrDefault();
                if (operationType != null)
                {
                    Operation operation = context.Operations.Where(o => o.OperationTypeID == operationType.operationTypeID).FirstOrDefault();
                    operationID = (operation != null) ? operation.OperationID : 0;
                }
                if (operationID > 0)
                {
                    //debit du cpte
                    if (eGL.MontantPrincDB > 0)
                    {
                        TreasuryOperationAccountOperation glDBOperation = new TreasuryOperationAccountOperation();
                        glDBOperation.BranchID = eGL.BranchID;
                        glDBOperation.DeviseID = eGL.DeviseID;
                        glDBOperation.OperationID = operationID;
                        glDBOperation.DateOperation = eGL.DateOperation;
                        glDBOperation.CodeTransaction = eGL.CodeTransaction;
                        glDBOperation.Description = eGL.Description;
                        glDBOperation.Reference = eGL.Reference;
                        glDBOperation.TreasuryOperationID = eGL.idSalePurchage;
                        glDBOperation.AccountID = eGL.AccountIDTierCusto;
                        glDBOperation.Credit = 0;
                        glDBOperation.Debit = eGL.MontantPrincDB;
                        context.AccountOperations.Add(glDBOperation);
                        context.SaveChanges();
                    }

                    //credit du cpte
                    if (eGL.MontantPrincCR > 0)
                    {
                        TreasuryOperationAccountOperation glCROperation = new TreasuryOperationAccountOperation();
                        glCROperation.BranchID = eGL.BranchID;
                        glCROperation.DeviseID = eGL.DeviseID;
                        glCROperation.OperationID = operationID;
                        glCROperation.DateOperation = eGL.DateOperation;
                        glCROperation.CodeTransaction = eGL.CodeTransaction;
                        glCROperation.Description = eGL.Description;
                        glCROperation.Reference = eGL.Reference;
                        glCROperation.TreasuryOperationID = eGL.idSalePurchage;
                        glCROperation.AccountID = eGL.AccountIDTresor;
                        glCROperation.Credit = eGL.MontantPrincCR;
                        glCROperation.Debit = 0;
                        context.AccountOperations.Add(glCROperation);
                        context.SaveChanges();
                    }
                    res = true;
                }
				else
				{
					res = false;
					throw new Exception("Error while running Accounting Operation. Bad configuration of Operation " + CodeValue.Accounting.InitOperationType.CODETREASURY);
				}

				return res;
            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CodeTransaction"></param>
        /// <returns></returns>
		public bool EcritureManuelleHistoGrandLivre(string CodeTransaction)
		{
			bool res = false;
			string accountingTaskSens;
			double montant = 0;
           
				try
				{
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //recup du type operation
                        int operationTypeID = context.Operations.Where(a => a.OperationCode == CodeValue.Accounting.InitOperation.CODEMANUALOP).SingleOrDefault().OperationTypeID;

                        //recuperation des taches des operation deja configure
                        foreach (Piece eGL in context.Pieces.Where(p => !p.isAcctOperation && p.CodeTransaction== CodeTransaction).ToList())
                        {
                            //fabrication de l'objet accountingoperation
                            AccountOperation accountoperation = new AccountOperation();
                            accountoperation.BranchID = eGL.BranchID;
                            accountoperation.DeviseID = eGL.DeviseID;
                            accountoperation.CodeTransaction = eGL.CodeTransaction;
                            accountoperation.DateOperation = eGL.DateOperation;
                            accountoperation.Description = eGL.Description;
                            accountoperation.OperationID = eGL.OperationID;
                            accountoperation.Reference = eGL.Reference;
                            accountoperation.AccountID = eGL.AccountID;
                            accountoperation.Debit = eGL.Debit;
                            accountoperation.Credit = eGL.Credit;
                            accountoperation.OperationID = eGL.OperationID;
                            //context.AccountOperations.Add(accountoperation);

                            //update du champ isAcctOperation
                            Piece pieceToUpdate = context.Pieces.Find(eGL.PieceID);
                            pieceToUpdate.isAcctOperation = true;

                            //ecriture du id ds la table de manualaccountoperation
                            montant = 0;
                            accountingTaskSens = "";

                            if (eGL.Debit > 0)
                            {
                                accountingTaskSens = "DB";
                                montant = eGL.Debit;
                            }
                            else
                            {
                                accountingTaskSens = "CR";
                                montant = eGL.Credit;
                            }
                            res = ecritureManualLigneGL(accountoperation, montant, operationTypeID, accountingTaskSens, eGL.PieceID);
                            if (!res)
                            {
                                //transact.Rollback();
                                throw new Exception("Error while running Accounting Operation. Contact Your Provider");
                            }

                            context.SaveChanges();
                        }

                        res = true;
                        //transact.Commit();
                        ts.Complete();
                    }
					
				}
				catch (Exception ex)
				{
					res = false;
                    //transact.Rollback();
					throw new Exception(ex.Message);
				}
                return res;
		}
		/// <summary>
		/// comptabilise les receptions des marchandises provenant du fournisseur
		/// </summary>
		/// <returns></returns>
		private bool comptabiliseReceptionMarchandise(Purchase pu, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			try
			{
                
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				string codetrn = "REGO";
				string desc = Resources.descREGO;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = pu.BranchID,
					DeviseID = pu.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = pu.Supplier.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = 0,
					DateOperation = bsday.BDDateOperation.Date,
					Description = desc + pu.Supplier.Name,
					Reference = pu.PurchaseReference,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = pu.NetCommercial,
					MontantPrincCR = pu.TotalPriceTTC,
					Discount = pu.DiscountAmount,
					Transport = pu.Transport,
					TVAAmount = pu.TVAAmount,
					idSalePurchage = pu.PurchaseID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
		/// <summary>
		/// comptabilise la sortie en stock
		/// </summary>
		/// <returns></returns>
		public bool comptabiliseEntreStockPurchase(Purchase pu, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				string codetrn = "STIN";
				string desc = Resources.descSTIN;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				//RECUPERATION DE TTTES LES LIGNE DE L'Achat ki n'on pas enc ete comptabilise
				foreach (PurchaseLine puline in context.PurchaseLines.Where(p => p.PurchaseID == pu.PurchaseID && !p.isPost).ToList())
				{
					double mountOfCurrentSale = (puline.LineQuantity * puline.LineUnitPrice);
					puline.Product = context.Products.SingleOrDefault(p => p.ProductID == puline.ProductID);
                    if (!(puline.Product is OrderLens))
                    {
                        //recuperation du productLocalization
                        int ProductLocalisationID = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == puline.ProductID && pl.LocalizationID == puline.LocalizationID).ProductLocalizationID;
                        if (ProductLocalisationID <= 0)
                        {
                            res = false;
                            throw new Exception("Error while running Operation. Wrong Product Localization Value ");
                        }
                        EcritureGLHist eGLConstatVente = new EcritureGLHist()
                        {
                            BranchID = pu.BranchID,
                            DeviseID = pu.DeviseID,
                            OperationID = operationID,
                            AccountIDTierCusto = pu.Supplier.AccountID,
                            AccountIDTierProduct = puline.Product.AccountID,
                            AccountIDTresor = 0,
                            DateOperation = bsday.BDDateOperation.Date,
                            Description = desc + puline.Product.ProductCode,
                            Reference = pu.PurchaseReference,
                            CodeTransaction = CodeTransaction,
                            MontantPrincDB = mountOfCurrentSale,
                            MontantPrincCR = mountOfCurrentSale,
                            Discount = pu.DiscountAmount,
                            Transport = pu.Transport,
                            TVAAmount = pu.TVAAmount,
                            idSalePurchage = ProductLocalisationID
                        };
                        //comptabilisation
                        res = EcritHistoGrandLivre(eGLConstatVente);
                        if (!res)
                        {
                            res = false;
                            throw new Exception("Error while running Operation. Please contact your Provider ");
                        }
                    }
                    
					//mise a jr de la ligne
					puline.isPost = true;

				} //fin boucle sur les ligne de vente
				res = checkPartieDouble(CodeTransaction);
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}

        /// <summary>
        /// comptabilise generale des entrees en stock
        /// </summary>
        /// <returns></returns>
        private bool comptabiliseStockInput(ProductLocalization pl, Branch br, BusinessDay bsday, string OperationCode)
        {
            bool res = true;

            try
            {
                //recup de l'id de l'operation
                Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
                int operationID = operation != null ? operation.OperationID : 0;
                
                string codetrn = "STIN";
                string desc = Resources.descSTIN;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
                EcritureGLHist eGLConstatVente = new EcritureGLHist()
                {
                    BranchID = pl.BranchID,
                    DeviseID = pl.DeviseID,
                    OperationID = operationID,
                    AccountIDTierCusto = 0,
                    AccountIDTierProduct =pl.Product.AccountID,
                    AccountIDTresor = 0,
                    DateOperation = bsday.BDDateOperation.Date,
                    Description = desc + pl.Product.ProductCode,
                    Reference = (pl.SellingReference == null || pl.SellingReference == "") ? CodeTransaction : pl.SellingReference,
                    CodeTransaction = CodeTransaction,
                    MontantPrincDB = pl.AveragePurchasePrice*pl.ProductLocalizationStockQuantity,
                    MontantPrincCR = pl.AveragePurchasePrice * pl.ProductLocalizationStockQuantity,
                    Discount = 0,
                    Transport = 0,
                    TVAAmount = 0,
                    idSalePurchage = pl.ProductLocalizationID
                };
                //comptabilisation
                res = EcritHistoGrandLivre(eGLConstatVente);
                if (!res)
                {
                    res = false;
                    throw new Exception("Error while running Operation. Please contact your Provider ");
                }

                res = checkPartieDouble(CodeTransaction);
            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }
            return res;
        }

        /// <summary>
        /// comptabilise generale des sorties en stock
        /// </summary>
        /// <returns></returns>
        private bool comptabiliseStockOutput(ProductLocalization pl, Branch br, BusinessDay bsday, string OperationCode)
        {
            bool res = true;

            try
            {
                //recup de l'id de l'operation
                Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
                int operationID = operation != null ? operation.OperationID : 0;

                string codetrn = "STOU";
                string desc = Resources.descOUST;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
                EcritureGLHist eGLConstatVente = new EcritureGLHist()
                {
                    BranchID = pl.BranchID,
                    DeviseID = pl.DeviseID,
                    OperationID = operationID,
                    AccountIDTierCusto = 0,
                    AccountIDTierProduct = pl.Product.AccountID,
                    AccountIDTresor = 0,
                    DateOperation = bsday.BDDateOperation.Date,
                    Description = desc + pl.ProductCode,
                    Reference = (pl.SellingReference==null || pl.SellingReference=="") ? CodeTransaction : pl.SellingReference,
                    CodeTransaction = CodeTransaction,
                    MontantPrincDB = pl.AveragePurchasePrice * pl.ProductLocalizationStockQuantity,
                    MontantPrincCR = pl.AveragePurchasePrice * pl.ProductLocalizationStockQuantity,
                    Discount = 0,
                    Transport = 0,
                    TVAAmount = 0,
                    idSalePurchage = pl.ProductLocalizationID
                };
                //comptabilisation
                res = EcritHistoGrandLivre(eGLConstatVente);
                if (!res)
                {
                    res = false;
                    throw new Exception("Error while running Operation. Please contact your Provider ");
                }

                res = checkPartieDouble(CodeTransaction);
            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }
            return res;
        }
		/// <summary>
		/// comptabilise les receptions des facture des marchandises provenant du fournisseur
		/// </summary>
		/// <returns></returns>
		private bool comptabiliseReceptionFacture(Purchase pu, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				string codetrn = "BIRE";
				string desc = Resources.descBIRE;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = pu.BranchID,
					DeviseID = pu.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = pu.Supplier.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = 0,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + pu.Supplier.Name,
					Reference = pu.PurchaseReference,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = pu.TotalPriceTTC,
					MontantPrincCR = pu.TotalPriceTTC,
					Discount = pu.DiscountAmount,
					Transport = pu.Transport,
					TVAAmount = pu.TVAAmount,
					idSalePurchage = pu.PurchaseID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
		/// <summary>
		/// comptabilise les avances d'argent chez le fournisseur
		/// </summary>
		/// <returns></returns>
		private bool comptabiliseAvanceMontantFournisseur(Purchase pu, Branch br, BusinessDay bsday, string OperationCode, PaymentMethod paymentMethod)
		{
			bool res = true;
			int AccountIDTresor = 0;
			double mountOfCurrentSale = 0;
			string codetrn = "";
			string desc = "";
			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				if (paymentMethod != null)
				{
					//recuperation du montant de la tranche
					mountOfCurrentSale = pu.PurchasePriceAdvance;
					//We determine type of this sale
					if (paymentMethod is Till)
					{
						codetrn = "TILL";
						desc = Resources.descTILL;
						AccountIDTresor = context.Tills.SingleOrDefault(a => a.ID == pu.PaymentMethodID).AccountID;
					}
					else if (paymentMethod is Bank)
					{
						codetrn = "BANK";
						desc = Resources.descBANK;
						AccountIDTresor = context.Banks.SingleOrDefault(a => a.ID == pu.PaymentMethodID).AccountID;
					}
                    else //aucne methode de paymenet def
                    {
                        res = false;
                        throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                    }
				}
				else //aucne methode de paymenet def
				{
					res = false;
					throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
				}
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = pu.BranchID,
					DeviseID = pu.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = pu.Supplier.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = AccountIDTresor,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + pu.Supplier.Name,
					Reference = pu.PurchaseReference,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = mountOfCurrentSale,
					MontantPrincCR = mountOfCurrentSale,
					Discount = pu.DiscountAmount,
					Transport = pu.Transport,
					TVAAmount = pu.TVAAmount,
					idSalePurchage = pu.PurchaseID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
		/// <summary>
		/// regularise les avances d'argent chez le fournisseur
		/// </summary>
		/// <returns></returns>
		private bool comptabiliseRegularisationAvanceFournisseur(Purchase pu, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				string codetrn = "READ";
				string desc = Resources.descREAD;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = pu.BranchID,
					DeviseID = pu.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = pu.Supplier.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = 0,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + pu.Supplier.Name,
					Reference = pu.PurchaseReference,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = pu.TotalPriceTTC,
					MontantPrincCR = pu.TotalPriceTTC,
					Discount = pu.DiscountAmount,
					Transport = pu.Transport,
					TVAAmount = pu.TVAAmount,
					idSalePurchage = pu.PurchaseID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
		/// <summary>
		/// regularise les reglements d'argent chez le fournisseur
		/// </summary>
		/// <returns></returns>
		private bool comptabiliseRegelementFournisseur(Purchase pu, Branch br, BusinessDay bsday, string OperationCode, PaymentMethod paymentMethod)
		{
			bool res = true;
			int AccountIDTresor = 0;
			double mountOfCurrentSale = 0;
			string codetrn = "";
			string desc = "";
			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				if (paymentMethod != null)
				{
					//recuperation du montant de la tranche
					mountOfCurrentSale = pu.TotalPriceTTC;
					//We determine type of this sale
					if (paymentMethod is Till)
					{
						codetrn = "TILL";
						desc = Resources.descTILL;
						AccountIDTresor = context.Tills.SingleOrDefault(a => a.ID == pu.PaymentMethodID).AccountID;
					}
					else if (paymentMethod is Bank)
					{
						codetrn = "BANK";
						desc = Resources.descBANK;
						AccountIDTresor = context.Banks.SingleOrDefault(a => a.ID == pu.PaymentMethodID).AccountID;
					}
                    else //aucne methode de paymenet def
                    {
                        res = false;
                        throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                    }
				}
				else //aucne methode de paymenet def
				{
					res = false;
					throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
				}
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = pu.BranchID,
					DeviseID = pu.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = pu.Supplier.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = AccountIDTresor,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + pu.Supplier.Name,
					Reference = pu.PurchaseReference,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = mountOfCurrentSale,
					MontantPrincCR = mountOfCurrentSale,
					Discount = pu.DiscountAmount,
					Transport = pu.Transport,
					TVAAmount = pu.TVAAmount,
					idSalePurchage = pu.PurchaseID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
		/// <summary>
		/// comptabilise les retour chez le fournisseur
		/// </summary>
		/// <returns></returns>
		private bool comptabiliseRetourFournisseur(Purchase pu, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			return res;
		}
		/// <summary>
		/// comptabilise les depenses budgetaires
		/// </summary>
		/// <returns></returns>
		private bool comptabiliseDepenseBudgetaire(BudgetConsumption buconsume, Branch br, BusinessDay bsday, PaymentMethod paymentMethod)
		{
			bool res = true;
			int AccountIDTresor = 0;
			double mountOfCurrentSale = 0;
			string codetrn = "";
			string desc = "";
			try
			{
				if (paymentMethod != null)
				{
					//recuperation du montant de la tranche
					mountOfCurrentSale = buconsume.VoucherAmount;
					//We determine type of this sale
					if (paymentMethod is Till)
					{
						codetrn = "TILL";
						desc = Resources.descTILLBUD;
						AccountIDTresor = context.Tills.SingleOrDefault(a => a.ID == buconsume.PaymentMethodID).AccountID;
					}
					else if (paymentMethod is Bank)
					{
						codetrn = "BANK";
						desc = Resources.descBANKBUD;
						AccountIDTresor = context.Banks.SingleOrDefault(a => a.ID == buconsume.PaymentMethodID).AccountID;
					}
                    else //aucne methode de paymenet def
                    {
                        res = false;
                        throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                    }
				}
				else //aucne methode de paymenet def
				{
					res = false;
					throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
				}
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLBudget = new EcritureGLHist()
				{
					BranchID = buconsume.BudgetAllocated.BranchID,
					DeviseID = buconsume.DeviseID.Value,
					AccountIDTierCusto = buconsume.BudgetAllocated.BudgetLine.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = AccountIDTresor,
					DateOperation = bsday.BDDateOperation.Date,
					Description = desc + buconsume.BeneficiaryName + " fv " + buconsume.Justification,
					Reference = buconsume.Reference,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = mountOfCurrentSale,
					MontantPrincCR = mountOfCurrentSale,
					idSalePurchage = buconsume.BudgetConsumptionID,
				};
				//comptabilisation
				res = EcritHistoGrandLivreBudget(eGLBudget);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}

        /*public bool ecritureComptableFinalValidateSpecialOrder(object o)
		{
			bool res = false;
            context = new EFDbContext();
            if (o is Sale) //traitement objet vente
			{
				Sale sa = (Sale)o;
				Branch br = context.Branches.SingleOrDefault(b => b.BranchID == sa.BranchID);
				BusinessDay bsday = context.BusinessDays.Where(bd => bd.BranchID == br.BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();
				sa.Customer = context.People.OfType<Customer>().SingleOrDefault(s => s.GlobalPersonID == sa.CustomerID);

				//si livraison marchadise
				if (sa.StatutSale == SalePurchaseStatut.Delivered)
				{
					res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
					//res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
				}
				//si generation facture
				if (sa.StatutSale == SalePurchaseStatut.Invoiced)
				{
					if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
					{
						res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
						//res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
						res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
					}
					if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
					{
						res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
					}
				}
				//si Reglement en avance
				if (sa.StatutSale == SalePurchaseStatut.Advanced)
				{
					PaymentMethod paymentMethod = context.PaymentMethods.Find(sa.PaymentMethodID);
					if (paymentMethod is Bank)
					{
						if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
						{
							res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
							//res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
							res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
							res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT, paymentMethod);
						}
						if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
						{
							res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
							res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT, paymentMethod);
						}
						if (sa.OldStatutSale == SalePurchaseStatut.Invoiced || sa.OldStatutSale == SalePurchaseStatut.Advanced)
						{
							res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT, paymentMethod);
						}
					}
					if (paymentMethod is Till)
					{
						if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
						{
							res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
							//res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
							res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
							res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT, paymentMethod);
						}
						if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
						{
							res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
							res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT, paymentMethod);
						}
						if (sa.OldStatutSale == SalePurchaseStatut.Invoiced || sa.OldStatutSale == SalePurchaseStatut.Advanced)
						{
							res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT, paymentMethod);
						}
					}
                    if (paymentMethod is SavingAccount)
                    {
                        if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
                        {
                            res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
                            res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                        }
                        if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
                        {
                            res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                        }
                    }
				}
				//si reglement total
				if (sa.StatutSale == SalePurchaseStatut.Paid)
				{
					PaymentMethod paymentMethod = context.PaymentMethods.Find(sa.PaymentMethodID);

					//verifions si cette vente a deja subit des avances de versement
					//recuperation de la somme total deje verser pour cette vente
					List<CustomerSlice> slice = context.CustomerSlices.Where(r => r.SaleID == sa.SaleID).ToList();
					if (slice != null) //il existe deja des tranches
					{
						//si c'est la seule tranche
						if (slice.Count == 1) //reglement total
						{
							if (paymentMethod is Bank)
							{
								if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
								{
									//comptabilise livraison
									res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
									//res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
									//comptabilise la facturation
									res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
									//comptabilise reglement
									res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT, paymentMethod);
								}
								if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
								{
									//comptabilise la facturation
									res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
									//comptabilise reglement
									res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT, paymentMethod);
								}
								if (sa.OldStatutSale == SalePurchaseStatut.Invoiced)
								{
									//comptabilise reglement
									res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT, paymentMethod);
								}
							}
							if (paymentMethod is Till)
							{
								if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
								{
									//comptabilise livraison
									res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
									//res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
									//comptabilise la facturation
									res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
									//comptabilise reglement
									res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT, paymentMethod);
								}
								if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
								{
									//comptabilise la facturation
									res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
									//comptabilise reglement
									res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT, paymentMethod);
								}
								if (sa.OldStatutSale == SalePurchaseStatut.Invoiced)
								{
									//comptabilise reglement
									res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT, paymentMethod);
								}
							}
                            if (paymentMethod is SavingAccount)
                            {
                                if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
                                {
                                    //comptabilise livraison
                                    res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
                                    //comptabilise la facturation
                                    res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                                }
                                if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
                                {
                                    //comptabilise la facturation
                                    res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                                }
                            }
						}
						else //si cette vente a o moins un slice
						{
							double AmountPaid = slice.Select(s => s.SliceAmount).Sum();

							if (AmountPaid >= sa.TotalPriceTTC)
							{
								//reglement de la derniere tranche
								if (paymentMethod is Bank)
								{
									//comptabilise reglement avance
									res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT, paymentMethod);
								}
								if (paymentMethod is Till)
								{
									//comptabilise reglement avance
									res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT, paymentMethod);
								}
								//ecriture d'annulation des tranches deja verser
								res = comptabiliseRegularisationAvanceClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECANCELSALEADVANCEDPAYMENT);
							}
							else
							{
								res = false;
								throw new Exception("Je Wanda Comment est ce Possible ?????");
							}
						}
					}


				}
			}
            return res;
        }*/
		/// <summary>
		/// methode generale de comptabilisation des ecritures ds le GL
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public bool ecritureComptableFinal(object o)
		{
			bool res = false;
            context = new EFDbContext();
			if (o is Purchase) //traitement entite achat
			{
				Purchase pu = (Purchase)o;

				Branch br = context.Branches.SingleOrDefault(b => b.BranchID == pu.BranchID);
				BusinessDay bsday = context.BusinessDays.Where(bd => bd.BranchID == br.BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();

				pu.Supplier = context.People.OfType<Supplier>().SingleOrDefault(s => s.GlobalPersonID == pu.SupplierID);
				//c'est ici ke mon stress commence

				//si reception marchadise
				if (pu.StatutPurchase == SalePurchaseStatut.Received)
				{
					res = comptabiliseReceptionMarchandise(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEDELIVERY);
					res = comptabiliseEntreStockPurchase(pu, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKINPUT);
				}
				//si Reception facture
				if (pu.StatutPurchase == SalePurchaseStatut.Invoiced)
				{
					if (pu.OldStatutPurchase == SalePurchaseStatut.Ordered)
					{
						res = comptabiliseReceptionMarchandise(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEDELIVERY);
						res = comptabiliseEntreStockPurchase(pu, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKINPUT);
						res = comptabiliseReceptionFacture(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING);
					}
					if (pu.OldStatutPurchase == SalePurchaseStatut.Received)
					{
						res = comptabiliseReceptionFacture(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING);
					}
				}
				//si Reglement en avance
				if (pu.StatutPurchase == SalePurchaseStatut.Advanced)
				{
					PaymentMethod paymentMethod = context.PaymentMethods.Find(pu.PaymentMethodID);
					if (paymentMethod is Bank)
					{
						if (pu.OldStatutPurchase == SalePurchaseStatut.Ordered)
						{
							res = comptabiliseReceptionMarchandise(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEDELIVERY);
							res = comptabiliseEntreStockPurchase(pu, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKINPUT);
							res = comptabiliseReceptionFacture(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING);
							res = comptabiliseAvanceMontantFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKPURCHASEADVANCEDPAYMENT, paymentMethod);
						}
						if (pu.OldStatutPurchase == SalePurchaseStatut.Received)
						{
							res = comptabiliseReceptionFacture(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING);
							res = comptabiliseAvanceMontantFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKPURCHASEADVANCEDPAYMENT, paymentMethod);
						}
						if (pu.OldStatutPurchase == SalePurchaseStatut.Invoiced || pu.OldStatutPurchase == SalePurchaseStatut.Advanced)
						{
							res = comptabiliseAvanceMontantFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKPURCHASEADVANCEDPAYMENT, paymentMethod);
						}
					}
					if (paymentMethod is Till)
					{
						if (pu.OldStatutPurchase == SalePurchaseStatut.Ordered)
						{
							res = comptabiliseReceptionMarchandise(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEDELIVERY);
							res = comptabiliseEntreStockPurchase(pu, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKINPUT);
							res = comptabiliseReceptionFacture(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING);
							res = comptabiliseAvanceMontantFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODECASHPURCHASEADVANCEDPAYMENT, paymentMethod);
						}
						if (pu.OldStatutPurchase == SalePurchaseStatut.Received)
						{
							res = comptabiliseReceptionFacture(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING);
							res = comptabiliseAvanceMontantFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODECASHPURCHASEADVANCEDPAYMENT, paymentMethod);
						}
						if (pu.OldStatutPurchase == SalePurchaseStatut.Invoiced || pu.OldStatutPurchase == SalePurchaseStatut.Advanced)
						{
							res = comptabiliseAvanceMontantFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODECASHPURCHASEADVANCEDPAYMENT, paymentMethod);
						}
					}
				}
				//si reglement total
				if (pu.StatutPurchase == SalePurchaseStatut.Paid)
				{
					PaymentMethod paymentMethod = context.PaymentMethods.Find(pu.PaymentMethodID);

					//verifions si cette achat a deja subit des avances de versement
					//recuperation de la somme total deje verser pour cette vente
					List<SupplierSlice> slice = context.SupplierSlices.Where(r => r.PurchaseID == pu.PurchaseID).ToList();
					if (slice != null) //il existe deja des tranches
					{
						//si c'est la seule tranche
						if (slice.Count == 1) //reglement total
						{
							if (paymentMethod is Bank)
							{
								if (pu.OldStatutPurchase == SalePurchaseStatut.Ordered)
								{
									res = comptabiliseReceptionMarchandise(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEDELIVERY);
									res = comptabiliseEntreStockPurchase(pu, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKINPUT);
									res = comptabiliseReceptionFacture(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING);
									res = comptabiliseRegelementFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKPAYMENTPURCHASE, paymentMethod);
								}
								if (pu.OldStatutPurchase == SalePurchaseStatut.Received)
								{
									res = comptabiliseReceptionFacture(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING);
									res = comptabiliseRegelementFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKPAYMENTPURCHASE, paymentMethod);
								}
								if (pu.OldStatutPurchase == SalePurchaseStatut.Invoiced)
								{
									//comptabilise reglement
									res = comptabiliseRegelementFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKPAYMENTPURCHASE, paymentMethod);
								}
							}
							if (paymentMethod is Till)
							{
								if (pu.OldStatutPurchase == SalePurchaseStatut.Ordered)
								{
									res = comptabiliseReceptionMarchandise(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEDELIVERY);
									res = comptabiliseEntreStockPurchase(pu, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKINPUT);
									res = comptabiliseReceptionFacture(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING);
									res = comptabiliseRegelementFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODECASHPAYMENTPURCHASE, paymentMethod);
								}
								if (pu.OldStatutPurchase == SalePurchaseStatut.Received)
								{
									res = comptabiliseReceptionFacture(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING);
									res = comptabiliseRegelementFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODECASHPAYMENTPURCHASE, paymentMethod);
								}
								if (pu.OldStatutPurchase == SalePurchaseStatut.Invoiced)
								{
									//comptabilise reglement
									res = comptabiliseRegelementFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODECASHPAYMENTPURCHASE, paymentMethod);
								}
							}
						}
						else //si cette vente a o moins un slice
						{
							double AmountPaid = slice.Select(s => s.SliceAmount).Sum();

							if (AmountPaid >= pu.TotalPriceTTC)
							{
								//reglement de la derniere tranche
								if (paymentMethod is Bank)
								{
									//comptabilise reglement avance
									res = comptabiliseAvanceMontantFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKPURCHASEADVANCEDPAYMENT, paymentMethod);
								}
								if (paymentMethod is Till)
								{
									//comptabilise reglement avance
									res = comptabiliseAvanceMontantFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODECASHPURCHASEADVANCEDPAYMENT, paymentMethod);
								}
								//ecriture d'annulation des tranches deja verser
								res = comptabiliseRegularisationAvanceFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODECANCELPURCHASEADVANCEDPAYMENT);
							}
							else
							{
								res = false;
								throw new Exception("Je Wanda Comment est ce Possible ?????");
							}
						}
					}


				}
				//si retour apres reception
				if (pu.StatutPurchase == SalePurchaseStatut.Returned)
				{
					res = comptabiliseRetourFournisseur(pu, br, bsday, CodeValue.Accounting.InitOperation.CODECERTIFYRETURNPURCHASE);
				}
			}
			else if (o is SupplierReturn) //traitement retour fournisseur
			{
				SupplierReturn sr = (SupplierReturn)o;
			}
            else if (o is Deposit) //traitement depot eparge
            {
                Deposit dep = (Deposit)o;
                Branch br = context.Branches.SingleOrDefault(b => b.BranchID == dep.BranchID);
                BusinessDay bsday = context.BusinessDays.Where(bd => bd.BranchID == br.BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();
                
                //dep.Customer = context.People.OfType<Customer>().SingleOrDefault(s => s.GlobalPersonID == dep.CustomerID);
                //si depot d'epargne
                if (dep.StatutSale==SalePurchaseStatut.Advanced)
                {
                    PaymentMethod paymentMethod = context.PaymentMethods.Find(dep.PaymentMethodID);
                    if (paymentMethod is Bank)
                    {
                        //comptabilise depot d'epargne
                        res = comptabiliseDepotClient(dep, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT, paymentMethod);
                    }
                    if (paymentMethod is Till)
                    {
                        //comptabilise depot d'epargne
                        res = comptabiliseDepotClient(dep, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT, paymentMethod);
                    }
                }
                //si dernier versement
                if (dep.StatutSale==SalePurchaseStatut.Paid)
                {
                    res = true;
                }
            }
			else if (o is Sale) //traitement objet vente
			{
				Sale sa = (Sale)o;
				Branch br = context.Branches.SingleOrDefault(b => b.BranchID == sa.BranchID);
				BusinessDay bsday = context.BusinessDays.Where(bd => bd.BranchID == br.BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();
				sa.Customer = context.People.OfType<Customer>().SingleOrDefault(s => s.GlobalPersonID == sa.CustomerID);

				//si livraison marchadise
				/*if (sa.StatutSale == SalePurchaseStatut.Delivered)
				{
					res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
					res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
				}
				//si generation facture
				if (sa.StatutSale == SalePurchaseStatut.Invoiced)
				{
					if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
					{
						res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
						res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
						res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
					}
					if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
					{
						res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
					}
				}*/
				//si Reglement en avance
				if (sa.StatutSale == SalePurchaseStatut.Advanced)
				{
                   
                    PaymentMethod paymentMethod = context.PaymentMethods.Find(sa.PaymentMethodID);
					if (paymentMethod is Bank)
					{
                        res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT, paymentMethod);
                        //if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
                        //{
                        //	res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
                        //	res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
                        //	res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                        //	res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT, paymentMethod);
                        //}
                        //if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
                        //{
                        //	res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                        //	res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT, paymentMethod);
                        //}
                        //if (sa.OldStatutSale == SalePurchaseStatut.Invoiced || sa.OldStatutSale == SalePurchaseStatut.Advanced)
                        //{
                        //	res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT, paymentMethod);
                        //}
                    }
					else if (paymentMethod is Till)
					{
                        res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT, paymentMethod);
                        //if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
                        //{
                        //	res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
                        //	res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
                        //	res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                        //	res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT, paymentMethod);
                        //}
                        //if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
                        //{
                        //	res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                        //	res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT, paymentMethod);
                        //}
                        //if (sa.OldStatutSale == SalePurchaseStatut.Invoiced || sa.OldStatutSale == SalePurchaseStatut.Advanced)
                        //{
                        //	res = comptabiliseAvanceMontantClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT, paymentMethod);
                        //}
                    }
                    //si par cpte epargne
                    /*else if (paymentMethod is AssureurPM)
                    {
                        if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
                        {
                            res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
                            res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
                            res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                        }
                        if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
                        {
                            res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                        }
                    }*/
                    else { res = true; }
				}
				//si reglement total
				if (sa.StatutSale == SalePurchaseStatut.Paid)
				{
					PaymentMethod paymentMethod = context.PaymentMethods.Find(sa.PaymentMethodID);
                    if (sa.OldStatutSale == SalePurchaseStatut.Advanced)
                    {
                        //si le client a deja eu a avancé un mtant deja cptabiliser
                        //alors on cptabilise la livraison, la sortie du produit et l'encaissement
                        //comptabilise livraison
                        //res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
                        //res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
                        ////comptabilise la facturation
                        //res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                        //reglement
                        if (paymentMethod is Bank)
                        {
                            //comptabilise reglement
                            res = comptabiliseReglementClientAvecAvance(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKADVANCEDSALEDELIVERY, paymentMethod);
                        }
                        if (paymentMethod is Till)
                        {
                            //comptabilise reglement
                            res = comptabiliseReglementClientAvecAvance(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHADVANCEDSALEDELIVERY, paymentMethod);
                        }
                    }
                    else 
                    {
                        //desormais je ne comptabilise que le depot d'argent
                        if (paymentMethod is Bank)
                        {
                            //comptabilise reglement
                            res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT, paymentMethod);
                        }
                        if (paymentMethod is Till)
                        {
                            //comptabilise reglement
                            res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT, paymentMethod);
                        }

                        //si aucun versement au par-avant alors on cptabilise
                        //la livraison, sortie et encaissement

                        /*if (sa.OldStatutSale == SalePurchaseStatut.Ordered)
                        {
                            //comptabilise livraison
                            res = comptabiliseLivraisonMarchandise(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY);
                            res = comptabiliseSortieStockSale(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
                            //comptabilise la facturation
                            res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                            //reglement
                            if (paymentMethod is Bank)
                            {
                                //comptabilise reglement
                                res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT, paymentMethod);
                            }
                            if (paymentMethod is Till)
                            {
                                //comptabilise reglement
                                res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT, paymentMethod);
                            }
                        }
                        if (sa.OldStatutSale == SalePurchaseStatut.Delivered)
                        {
                            //comptabilise la facturation
                            res = comptabiliseGenererFacture(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEBILLING);
                            //reglement
                            if (paymentMethod is Bank)
                            {
                                //comptabilise reglement
                                res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT, paymentMethod);
                            }
                            if (paymentMethod is Till)
                            {
                                //comptabilise reglement
                                res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT, paymentMethod);
                            }
                        }
                        if (sa.OldStatutSale == SalePurchaseStatut.Invoiced)
                        {
                            //reglement
                            if (paymentMethod is Bank)
                            {
                                //comptabilise reglement
                                res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT, paymentMethod);
                            }
                            if (paymentMethod is Till)
                            {
                                //comptabilise reglement
                                res = comptabiliseReglementClient(sa, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT, paymentMethod);
                            }
                        }*/


                    }

                    
				}

			}
			else if (o is CustomerReturn)
			{
				CustomerReturn cr = (CustomerReturn)o;
				Branch br = context.Branches.SingleOrDefault(b => b.BranchID == cr.Sale.BranchID);
				BusinessDay bsday = context.BusinessDays.Where(bd => bd.BranchID == br.BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();
				cr.Sale.Customer = context.People.OfType<Customer>().SingleOrDefault(s => s.GlobalPersonID == cr.Sale.CustomerID);
				//si retour apres Livraison
				if (cr.Sale.isReturn)
				{
					//recuperation du montant des slice retourne
					List<CustomerSlice> slice = context.CustomerSlices.Where(r => r.SaleID == cr.Sale.SaleID).ToList();
					if (slice == null || slice.Count == 0) //il existe deja des tranches dc la vente etait a Credit 
					{
						res = comptabiliseRetourVenteACredit(cr, br, bsday, CodeValue.Accounting.InitOperation.CODECERTIFYRETURNSALE);
					}
					else
					{
						//recup du montant total de la vente
						double AmountSale = slice.Select(s => s.SliceAmount).Sum();
						ExtraPrice ep = new ExtraPrice();
						double grossAmount = context.SaleLines.Where(sl => sl.SaleID == cr.Sale.SaleID).ToList().Select(sl2 => (sl2.LineQuantity * sl2.LineUnitPrice)).Sum();
						ep = Util.ExtraPrices(grossAmount, cr.Sale.RateReduction, cr.Sale.RateDiscount, cr.Sale.Transport, cr.Sale.VatRate);

						if (AmountSale < ep.TotalTTC) //si la vente avait subi une avance ie slice<mnt total
						{
							res = comptabiliseRetourClientAvance(cr, br, bsday, CodeValue.Accounting.InitOperation.CODECERTIFYRETURNSALE);
						}
						if (AmountSale == ep.TotalTTC) //si la vente etait cash totale ie slice=mnt total
						{
							res = comptabiliseRetourClientTotale(cr, br, bsday, CodeValue.Accounting.InitOperation.CODECERTIFYRETURNSALE);
						}
					}
				}
				/******************************************/

				/*******************************************/
			}
			else if (o is ProductTransfert)
			{
				ProductTransfert pt = (ProductTransfert)o;
                //si envoi

                //si reception
			}
            else if (o is ProductLocalization)
            {
                ProductLocalization pl = (ProductLocalization)o;
                Branch br = context.Branches.SingleOrDefault(b => b.BranchID == pl.BranchID);
                BusinessDay bsday = context.BusinessDays.Where(bd => bd.BranchID == br.BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();
                if (bsday == null)
                {
                    res = false;
                    throw new Exception("The Business Day is not open. Please contact your administrator ");
                }
                //si entree en stock
                if (pl.isStockInput)
                {
                    res = comptabiliseStockInput(pl, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKINPUT);
                }
                //si sortie en stock
                if (!pl.isStockInput)
                {
                    res = comptabiliseStockOutput(pl, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT);
                }
            }
			else if (o is BudgetConsumption)
			{
				BudgetConsumption buconsume = (BudgetConsumption)o;
				Branch br = context.Branches.SingleOrDefault(b => b.BranchID == buconsume.BudgetAllocated.BranchID);
				BusinessDay bsday = context.BusinessDays.Where(bd => bd.BranchID == br.BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();

				//si Reglement en avance
				if (buconsume.isValidated == true)
				{
					PaymentMethod paymentMethod = context.PaymentMethods.Find(buconsume.PaymentMethodID);

					if (paymentMethod != null)
					{
						res = comptabiliseDepenseBudgetaire(buconsume, br, bsday, paymentMethod);
					}
					else
					{
						res = false;
						throw new Exception("Tresory Account Not yet define for this transaction. Please contact your Administrator ");
					}
				}

			}
			else if (o is TillAdjust)
			{
				TillAdjust ta = (TillAdjust)o;
				//double shortage = 0d;
				//double overage = 0d;

				Branch br = context.Branches.SingleOrDefault(b => b.BranchID == ta.Till.BranchID);
				BusinessDay bsday = context.BusinessDays.Where(bd => bd.BranchID == br.BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();

				PaymentMethod paymentMethod = context.PaymentMethods.Find(ta.TillID);

				// recuperation de l'ecart

				if (ta.ecart < 0) //shortage
				{
					res = comptabiliseOverageShortage(ta, br, bsday, paymentMethod, CodeValue.Accounting.InitOperation.CODETELLERADJUST);
				}
				if (ta.ecart > 0) //overage
				{
					res = comptabiliseOverageShortage(ta, br, bsday, paymentMethod, CodeValue.Accounting.InitOperation.CODEOVERAGE);
				}
			}
            else if (o is TreasuryOperation)
            {
                TreasuryOperation to = (TreasuryOperation)o;
                Branch br = context.Branches.SingleOrDefault(b => b.BranchID == to.BranchID);
                BusinessDay bsday = context.BusinessDays.Where(bd => bd.BranchID == br.BranchID && bd.BDStatut == true && bd.ClosingDayStarted == false).SingleOrDefault();
                res = comptabiliseTreasuryOperation(to, br, bsday);

                
            }
			//context.SaveChanges();
			return res;
		}



        private bool comptabiliseTreasuryOperation(TreasuryOperation treasuryOp, Branch br, BusinessDay bsday)
        {
            bool res = true;
            string codetrn = "";
            string desc = "";
            int cpteDb = 0;
            int cpteCr = 0;
            try
			{
                Till till = context.PaymentMethods.OfType<Till>().FirstOrDefault(t => t.ID == treasuryOp.TillID);
                Bank bank = context.PaymentMethods.OfType<Bank>().FirstOrDefault(t => t.ID == treasuryOp.BankID);
                if (treasuryOp.OperationType== CodeValue.Accounting.TreasuryOperation.TranferToTeller)
                {
                    codetrn = "TRTE";
                    desc = Resources.descTRTE;
                    //credit de la caisse de depart pour le debit de la caisse receptrice
                    Till tillDest = context.PaymentMethods.OfType<Till>().FirstOrDefault(t => t.ID == treasuryOp.TillDestID);
                    cpteDb = tillDest.AccountID;
                    cpteCr = till.AccountID;
                }
                if (treasuryOp.OperationType == CodeValue.Accounting.TreasuryOperation.TransfertToBank)
                {
                    codetrn = "TRBA";
                    desc = Resources.descTRBA;
                    //credit de la caisse de depart pour le debit de la banque receptrice
                    cpteDb = bank.AccountID;
                    cpteCr = till.AccountID;
                }
                if (treasuryOp.OperationType == CodeValue.Accounting.TreasuryOperation.ReceiveFromTeller)
                {
                    codetrn = "RETE";
                    desc = Resources.descRETE;
                    //debit de la caisse de depart pour le credit de la caisse receptrice
                    Till tillDest = context.PaymentMethods.OfType<Till>().FirstOrDefault(t => t.ID == treasuryOp.TillDestID);
                    cpteDb = till.AccountID;
                    cpteCr = tillDest.AccountID;
                }
                if (treasuryOp.OperationType == CodeValue.Accounting.TreasuryOperation.ReceiveFromBank)
                {
                    codetrn = "REBA";
                    desc = Resources.descREBA;
                    //debit de la caisse de depart pour le credit de la banque receptrice
                    cpteDb = till.AccountID;
                    cpteCr = bank.AccountID;
                }
                 
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
                EcritureGLHist eGLTreasury = new EcritureGLHist();
                eGLTreasury.BranchID = treasuryOp.BranchID;
                eGLTreasury.DeviseID = treasuryOp.DeviseID;
                eGLTreasury.AccountIDTierCusto = cpteDb;
                eGLTreasury.AccountIDTresor = cpteCr;
                eGLTreasury.AccountIDTierProduct = 0;
                eGLTreasury.DateOperation = bsday.BDDateOperation.Date;
                eGLTreasury.Description = desc + treasuryOp.Justification;
                eGLTreasury.Reference = treasuryOp.OperationRef;
                eGLTreasury.CodeTransaction = CodeTransaction;
                eGLTreasury.MontantPrincDB = treasuryOp.OperationAmount;
                eGLTreasury.MontantPrincCR = treasuryOp.OperationAmount;
                eGLTreasury.idSalePurchage = treasuryOp.TreasuryOperationID;
                
                //comptabilisation
                res = EcritHistoGrandLivreTreasury(eGLTreasury);
                if (!res)
                {
                    res = false;
                    throw new Exception("Error while running Operation. Please contact your Provider ");
                }


                res = checkPartieDouble(CodeTransaction);

            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }
            return res;
        }

        //compta des vente
        /// <summary>
        /// comptabilise les livraison des marchandises au client
        /// </summary>
        /// <returns></returns>
        private bool comptabiliseLivraisonMarchandiseSansCheckPartieDouble(Sale sa, Branch br, BusinessDay bsday, string OperationCode, string CodeTransaction)
        {
            bool res = true;

            try
            {
                //recup de l'id de l'operation
                Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
                int operationID = operation != null ? operation.OperationID : 0;

                string codetrn = "DESA";
                string desc = Resources.descDESA;
                

                EcritureGLHist eGLConstatVente = new EcritureGLHist()
                {
                    BranchID = sa.BranchID,
                    DeviseID = sa.DeviseID,
                    OperationID = operationID,
                    AccountIDTierCusto = sa.Customer.AccountID,
                    AccountIDTierProduct = 0,
                    AccountIDTresor = 0,
                    DateOperation = bsday.BDDateOperation.Date,
                    Description = desc + sa.Customer.Name,
                    Reference = sa.SaleReceiptNumber,
                    CodeTransaction = CodeTransaction,
                    MontantPrincDB = sa.TotalPriceTTC,
                    MontantPrincCR = sa.TotalPriceTTC,
                    Discount = sa.DiscountAmount,
                    Transport = sa.Transport,
                    TVAAmount = sa.TVAAmount,
                    idSalePurchage = sa.SaleID
                };
                //comptabilisation
                res = EcritHistoGrandLivre(eGLConstatVente);
                if (!res)
                {
                    res = false;
                    throw new Exception("Error while running Operation. Please contact your Provider ");
                }

            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }
            return res;
        }
        //compta des vente
        /// <summary>
        /// comptabilise les livraison des marchandises au client
        /// </summary>
        /// <returns></returns>
        private bool comptabiliseLivraisonMarchandise(Sale sa, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				string codetrn = "DESA";
				string desc = Resources.descDESA;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = sa.BranchID,
					DeviseID = sa.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = sa.Customer.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = 0,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + sa.Customer.Name,
					Reference = sa.SaleReceiptNumber,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = sa.TotalPriceTTC,
					MontantPrincCR = sa.NetCommercial,
					Discount = sa.DiscountAmount,
					Transport = sa.Transport,
					TVAAmount = sa.TVAAmount,
					idSalePurchage = sa.SaleID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);

			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
        /// <summary>
        /// methode de verification de la partie double
        /// </summary>
        /// <param name="CodeTransaction"></param>
        /// <returns></returns>
        private bool checkPartieDouble(string CodeTransaction)
		{
			bool res = false;
			//vefication de la partie double et validation de la transaction
			List<AccountOperation> listOp = (from d in context.AccountOperations where d.CodeTransaction == CodeTransaction
                                                 select d).ToList();
            //context.AccountOperations.Where(d => d.CodeTransaction == CodeTransaction).ToList();
			double DebitOp = 0d;
			double CreditOp = 0d;
			if (listOp.Count() > 0 && listOp != null)
			{
				DebitOp = listOp.Select(d => d.Debit).Sum();
				CreditOp = listOp.Select(c => c.Credit).Sum();
			}

			if (/*DebitOp > 0 &&*/ DebitOp != CreditOp)
			{
				res = false;
				throw new Exception(CodeTransaction + " Error while running Accounting Operation: Double-Entry bookkeeping non Respect !!! Contact Your Provider");
			}
			else
			{
				res = true;
			}
			return res;
		}

        ///
        /// 
        /// <summary>
        /// comptabilise les sorties de stock
        /// </summary>
        /// <returns></returns>
        private bool comptabiliseSortieStockSale(Sale sa, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				string codetrn = "OUST";
				string desc = Resources.descOUST;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				//RECUPERATION DE TTTES LES LIGNE DE L'Sale ki n'on pas enc ete comptabilise
				foreach (SaleLine saleline in context.SaleLines.Where(s => s.SaleID == sa.SaleID && !s.isPost).ToList())
				{
					double mountOfCurrentSale = (saleline.LineQuantity * saleline.LineUnitPrice);
					saleline.Product = context.Products.SingleOrDefault(p => p.ProductID == saleline.ProductID);
                    if (!(saleline.Product is OrderLens))
                    {
                        int ProductLocalisationID = 0;
                        //recuperation du productLocalization
                        if (!(saleline.Product.Category.isSerialNumberNull))
                        {
                            ProductLocalisationID = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == saleline.ProductID && pl.LocalizationID == saleline.LocalizationID).ProductLocalizationID;
                        }
                        else
                        {
                            ProductLocalisationID = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == saleline.ProductID && pl.LocalizationID == saleline.LocalizationID && pl.NumeroSerie == saleline.NumeroSerie && pl.Marque==saleline.marque).ProductLocalizationID;
                        }
                        
                        if (ProductLocalisationID == null || ProductLocalisationID <= 0)
                        {
                            res = false;
                            throw new Exception("Error while running Operation. Wrong Product Localization Value ");
                        }
                        EcritureGLHist eGLConstatVente = new EcritureGLHist()
                        {
                            BranchID = sa.BranchID,
                            DeviseID = sa.DeviseID,
                            OperationID = operationID,
                            AccountIDTierCusto = sa.Customer.AccountID,
                            AccountIDTierProduct = saleline.Product.AccountID,
                            AccountIDTresor = 0,
                            DateOperation = bsday.BDDateOperation.Date,
                            Description = desc + saleline.Product.ProductCode,
                            Reference = sa.SaleReceiptNumber,
                            CodeTransaction = CodeTransaction,
                            MontantPrincDB = mountOfCurrentSale,
                            MontantPrincCR = mountOfCurrentSale,
                            Discount = sa.DiscountAmount,
                            Transport = sa.Transport,
                            TVAAmount = sa.TVAAmount,
                            idSalePurchage = ProductLocalisationID
                        };
                        //comptabilisation
                        res = EcritHistoGrandLivre(eGLConstatVente);
                        if (!res)
                        {
                            res = false;
                            throw new Exception("Error while running Operation. Please contact your Provider ");
                        }
                    }
					//mise a jr de la ligne
					saleline.isPost = true;

				} //fin boucle sur les ligne de vente
				res = checkPartieDouble(CodeTransaction);
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
        /// <summary>
        /// comptabilise les generation des facture des marchandises 
        /// </summary>
        /// <returns></returns>
        private bool comptabiliseGenererFacture(Sale sa, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				string codetrn = "SABI";
				string desc = Resources.descSABI;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = sa.BranchID,
					DeviseID = sa.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = sa.Customer.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = 0,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + sa.Customer.Name,
					Reference = sa.SaleReceiptNumber,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = sa.TotalPriceTTC,
					MontantPrincCR = sa.TotalPriceTTC,
					Discount = sa.DiscountAmount,
					Transport = sa.Transport,
					TVAAmount = sa.TVAAmount,
					idSalePurchage = sa.SaleID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);

			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}

        /// <summary>
        /// comptabilisation des surplus et perte observe a la caisse
        /// </summary>
        /// <param name="ta"></param>
        /// <param name="br"></param>
        /// <param name="bsday"></param>
        /// <param name="paymentMethod"></param>
        /// <param name="OperationCode"></param>
        /// <returns></returns>
        private bool comptabiliseOverageShortage(TillAdjust ta, Branch br, BusinessDay bsday, PaymentMethod paymentMethod, string OperationCode)
		{
			bool res = true;
			int AccountIDTresor = 0;
			double mountOfCurrentSale = 0;
			string codetrn = "";
			string desc = "";
			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				if (paymentMethod != null)
				{
					//recuperation du montant de la tranche

					mountOfCurrentSale = (ta.ecart < 0) ? ta.ecart * -1 : ta.ecart;

					if (paymentMethod is Till)
					{
						if (ta.ecart < 0)
						{
							codetrn = "SHOR";
							desc = Resources.descSHOR;
						}
						else
						{
							codetrn = "OVER";
							desc = Resources.descOVER;
						}
						AccountIDTresor = context.PaymentMethods.OfType<Till>().SingleOrDefault(a => a.ID == ta.TillID).AccountID;
					}
					else
					{
						res = false;
						throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
					}
				}
				else //aucne methode de paymenet def
				{
					res = false;
					throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
				}
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = br.BranchID,
					DeviseID = ta.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = 0,
					AccountIDTierProduct = 0,
					AccountIDTresor = AccountIDTresor,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + ta.Justification,
					Reference = CodeTransaction,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = mountOfCurrentSale,
					MontantPrincCR = mountOfCurrentSale,
					Discount = 0,
					Transport = 0,
					TVAAmount = 0,
					idSalePurchage = ta.TillAdjustID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);

			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}

        /// <summary>
        /// comptabilise les avances d'argent du client
        /// </summary>
        /// <returns></returns>
        private bool comptabiliseAvanceMontantClient(Sale sa, Branch br, BusinessDay bsday, string OperationCode, PaymentMethod paymentMethod)
		{
			bool res = true;
			int AccountIDTresor = 0;
			double mountOfCurrentSale = 0;
			string codetrn = "";
			string desc = "";
			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				if (paymentMethod != null)
				{
					//recuperation du montant de la tranche
					mountOfCurrentSale = sa.SaleTotalPriceAdvance;
					//We determine type of this sale
					if (paymentMethod is Till)
					{
						codetrn = "TILL";
						desc = Resources.descTILLAVCLT;
						AccountIDTresor = context.Tills.SingleOrDefault(a => a.ID == sa.PaymentMethodID).AccountID;
					}
					else if (paymentMethod is Bank)
					{
						codetrn = "BANK";
						desc = Resources.descBANKAVCLT;
						AccountIDTresor = context.Banks.SingleOrDefault(a => a.ID == sa.PaymentMethodID).AccountID;
					}
                    else //aucne methode de paymenet def
                    {
                        res = false;
                        throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                    }
				}
				else //aucne methode de paymenet def
				{
					res = false;
					throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
				}
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
                
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = sa.BranchID,
					DeviseID = sa.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = sa.Customer.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = AccountIDTresor,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + sa.Customer.Name,
					Reference = sa.SaleReceiptNumber,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = mountOfCurrentSale,
					MontantPrincCR = mountOfCurrentSale,
					Discount = sa.DiscountAmount,
					Transport = sa.Transport,
					TVAAmount = sa.TVAAmount,
					idSalePurchage = sa.SaleID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);

			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
        /// <summary>
        /// regularise les avances d'argent du client
        /// </summary>
        /// <returns></returns>
        private bool comptabiliseRegularisationAvanceClient(Sale sa, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				string codetrn = "READ";
				string desc = Resources.descREAD;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = sa.BranchID,
					DeviseID = sa.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = sa.Customer.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = 0,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + sa.Customer.Name,
					Reference = sa.SaleReceiptNumber,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = sa.TotalPriceTTC,
					MontantPrincCR = sa.TotalPriceTTC,
					Discount = sa.DiscountAmount,
					Transport = sa.Transport,
					TVAAmount = sa.TVAAmount,
					idSalePurchage = sa.SaleID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);

			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
        private bool comptabiliseDepotClient(Deposit dep, Branch br, BusinessDay bsday, string OperationCode, PaymentMethod paymentMethod)
        {
            bool res = true;
            int AccountIDTresor = 0;
            double mountOfCurrentSale = 0;
            string codetrn = "";
            string desc = "";

            try
            {
                //recup de l'id de l'operation
                Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
                int operationID = operation != null ? operation.OperationID : 0;

                if (paymentMethod != null)
                {
                    //recuperation du montant de la tranche
                    mountOfCurrentSale = dep.Amount;
                    //We determine type of this sale
                    if (paymentMethod is Till)
                    {
                        codetrn = "DETI";
                        desc = Resources.descTILLDEPCLT;
                        AccountIDTresor = context.Tills.SingleOrDefault(a => a.ID == dep.PaymentMethodID).AccountID;
                    }
                    else if (paymentMethod is Bank)
                    {
                        codetrn = "DEBA";
                        desc = Resources.descBANKDEPCLT;
                        AccountIDTresor = context.Banks.SingleOrDefault(a => a.ID == dep.PaymentMethodID).AccountID;
                    }
                    else //aucne methode de paymenet def
                    {
                        res = false;
                        throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                    }
                }
                else //aucne methode de paymenet def
                {
                    res = false;
                    throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                }
                Customer cust = this.context.Customers.Find(dep.CustomerID);
                _trnRepository = new TransactNumberRepository(context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
                EcritureGLHist eGLConstatVente = new EcritureGLHist()
                {
                    BranchID = dep.BranchID,
                    DeviseID = dep.DeviseID,
                    OperationID = operationID,
                    AccountIDTierCusto = cust.AccountID,
                    AccountIDTierProduct = 0,
                    AccountIDTresor = AccountIDTresor,
                    DateOperation = bsday.BDDateOperation.Date,
                    Description = desc + cust.Name,
                    Reference = dep.DepositReference,
                    CodeTransaction = CodeTransaction,
                    MontantPrincDB = mountOfCurrentSale,
                    MontantPrincCR = mountOfCurrentSale,
                    Discount = 0,
                    Transport = 0,
                    TVAAmount = 0,
                    idSalePurchage = dep.DepositID
                };
                //comptabilisation
                res = EcritHistoGrandLivre(eGLConstatVente,true);
                if (!res)
                {
                    res = false;
                    throw new Exception("Error while running Operation. Please contact your Provider ");
                }

                res = checkPartieDouble(CodeTransaction);

            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }
            return res;
        }
        /// <summary>
        /// comptabilise les reglements d'argent du client
        /// </summary>
        /// <returns></returns>
        private bool comptabiliseReglementClient(Sale sa, Branch br, BusinessDay bsday, string OperationCode, PaymentMethod paymentMethod)
        {
            bool res = true;
            int AccountIDTresor = 0;
            double mountOfCurrentSale = 0;
            string codetrn = "";
            string desc = "";

            try
            {
                //recup de l'id de l'operation
                Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
                int operationID = operation != null ? operation.OperationID : 0;

                if (paymentMethod != null)
                {
                    //recuperation du montant de la tranche
                    mountOfCurrentSale = sa.TotalPriceTTC;
                    //We determine type of this sale
                    if (paymentMethod is Till)
                    {
                        codetrn = "TILL";
                        desc = Resources.descTILLREGCLT;
                        AccountIDTresor = context.Tills.SingleOrDefault(a => a.ID == sa.PaymentMethodID).AccountID;
                    }
                    else if (paymentMethod is Bank)
                    {
                        codetrn = "BANK";
                        desc = Resources.descBANKREGCLT;
                        AccountIDTresor = context.Banks.SingleOrDefault(a => a.ID == sa.PaymentMethodID).AccountID;
                    }
                    else //aucne methode de paymenet def
                    {
                        res = false;
                        throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                    }
                }
                else //aucne methode de paymenet def
                {
                    res = false;
                    throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                }
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
                EcritureGLHist eGLConstatVente = new EcritureGLHist()
                {
                    BranchID = sa.BranchID,
                    DeviseID = sa.DeviseID,
                    OperationID = operationID,
                    AccountIDTierCusto = sa.Customer.AccountID,
                    AccountIDTierProduct = 0,
                    AccountIDTresor = AccountIDTresor,
                    DateOperation = bsday.BDDateOperation.Date,
                    Description = desc + sa.Customer.Name,
                    Reference = sa.SaleReceiptNumber,
                    CodeTransaction = CodeTransaction,
                    MontantPrincDB = mountOfCurrentSale,
                    MontantPrincCR = mountOfCurrentSale,
                    Discount = sa.DiscountAmount,
                    Transport = sa.Transport,
                    TVAAmount = sa.TVAAmount,
                    idSalePurchage = sa.SaleID
                };
                //comptabilisation
                res = EcritHistoGrandLivre(eGLConstatVente);
                if (!res)
                {
                    res = false;
                    throw new Exception("Error while running Operation. Please contact your Provider ");
                }

                res = checkPartieDouble(CodeTransaction);

            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }
            return res;
        }

        private bool comptabiliseReglementClientAvecAvance(Sale sa, Branch br, BusinessDay bsday, string OperationCode, PaymentMethod paymentMethod)
        {
            bool res = true;
            int AccountIDTresor = 0;
            double mountOfCurrentSale = 0;
            
            string codetrn = "";
            string desc = "";

            try
            {
                //recup de l'id de l'operation
                Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
                int operationID = operation != null ? operation.OperationID : 0;

                if (paymentMethod != null)
                {
                    //recuperation du montant de la tranche
                    mountOfCurrentSale = sa.TotalPriceTTC;
                    //We determine type of this sale
                    if (paymentMethod is Till)
                    {
                        codetrn = "TILL";
                        desc = Resources.descTILLREGCLT;
                        AccountIDTresor = context.Tills.SingleOrDefault(a => a.ID == sa.PaymentMethodID).AccountID;
                    }
                    else if (paymentMethod is Bank)
                    {
                        codetrn = "BANK";
                        desc = Resources.descBANKREGCLT;
                        AccountIDTresor = context.Banks.SingleOrDefault(a => a.ID == sa.PaymentMethodID).AccountID;
                    }
                    else //aucne methode de paymenet def
                    {
                        res = false;
                        throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                    }
                }
                else //aucne methode de paymenet def
                {
                    res = false;
                    throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                }
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
                EcritureGLHist eGLConstatVente = new EcritureGLHist()
                {
                    BranchID = sa.BranchID,
                    DeviseID = sa.DeviseID,
                    OperationID = operationID,
                    AccountIDTierCusto = sa.Customer.AccountID,
                    AccountIDTierProduct = 0,
                    AccountIDTresor = AccountIDTresor,
                    DateOperation = bsday.BDDateOperation.Date,
                    Description = desc + sa.Customer.Name,
                    Reference = sa.SaleReceiptNumber,
                    CodeTransaction = CodeTransaction,
                    MontantPrincDB = mountOfCurrentSale,
                    MontantPrincCR = mountOfCurrentSale,
                    Discount = sa.DiscountAmount,
                    Transport = sa.Transport,
                    TVAAmount = sa.TVAAmount,
                    idSalePurchage = sa.SaleID,
                    MontantClientDeposit=sa.MontantClientDeposit,
                    MontantTotalClientAdvance= sa.MontantTotalClientAdvance
                };
                //comptabilisation
                res = EcritHistoGrandLivreRegelementAvecAvance(eGLConstatVente);
                if (!res)
                {
                    res = false;
                    throw new Exception("Error while running Operation. Please contact your Provider ");
                }
                //comptabilisation des livrasions
                res = comptabiliseLivraisonMarchandiseSansCheckPartieDouble(sa, br, bsday, CodeValue.Accounting.InitOperation.CODESALEDELIVERY, CodeTransaction);
                if (!res)
                {
                    res = false;
                    throw new Exception("Error while running Operation. Please contact your Provider ");
                }
                res = checkPartieDouble(CodeTransaction);
                if (!res)
                {
                    res = false;
                    throw new Exception("Error while checking partie double. Please contact your Provider ");
                }
            }
            catch (Exception ex)
            {
                res = false;
                throw new Exception(ex.Message);
            }
            return res;
        }
        /// <summary>
        /// comptabilise les reglements retour d'argent du client
        /// </summary>
        /// <returns></returns>
        private bool comptabiliseReglementRetourClient(CustomerReturn cr, Branch br, BusinessDay bsday, string OperationCode, PaymentMethod paymentMethod, double paidAmount)
		{
			bool res = true;
			int AccountIDTresor = 0;
			double mountOfCurrentSale = 0;
			string codetrn = "";
			string desc = "";

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				if (paymentMethod != null)
				{
					//recuperation du montant de la tranche
                    mountOfCurrentSale = paidAmount;
					//We determine type of this sale
					if (paymentMethod is Till)
					{
						codetrn = "TILL";
						desc = Resources.descTILLREGCLT;
                        AccountIDTresor = context.Tills.SingleOrDefault(a => a.ID == paymentMethod.ID).AccountID;
					}
					else if (paymentMethod is Bank)
					{
						codetrn = "BANK";
                        desc = Resources.descBANKREGCLT;
                        AccountIDTresor = context.Banks.SingleOrDefault(a => a.ID == paymentMethod.ID).AccountID;
					}
                    else //aucne methode de paymenet def
                    {
                        res = false;
                        throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                    }
				}
				else //aucne methode de paymenet def
				{
					res = false;
					throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
				}
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = cr.Sale.BranchID,
                    DeviseID = cr.Sale.DeviseID,
					OperationID = operationID,
                    AccountIDTierCusto = cr.Sale.Customer.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = AccountIDTresor,
                    DateOperation = bsday.BDDateOperation.Date,
                    Description = desc + cr.Sale.Customer.Name,
                    Reference = cr.Sale.SaleReceiptNumber,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = mountOfCurrentSale,
					MontantPrincCR = mountOfCurrentSale,
                    Discount = cr.Sale.DiscountAmount,
                    Transport = cr.Sale.Transport,
                    TVAAmount = cr.Sale.TVAAmount,
                    idSalePurchage = cr.Sale.SaleID
				};
				//comptabilisation
				res = EcritHistoGrandLivreReglementRetourProduct(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);

			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
		/// <summary>
		/// comptabilise les retour client
		/// </summary>
		/// <returns></returns>

		private bool comptabiliseConstatRetourVente(CustomerReturn cr, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				string codetrn = "RTSA";
				string desc = Resources.descRTSA;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = cr.Sale.BranchID,
					DeviseID = cr.Sale.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = cr.Sale.Customer.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = 0,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + cr.Sale.Customer.Name,
					Reference = cr.Sale.SaleReceiptNumber,
					CodeTransaction = CodeTransaction,
                    MontantPrincDB = cr.TotalPriceReturn,//TotalPriceMarchandise,
                    MontantPrincCR = cr.TotalPriceReturn,
					Discount = cr.DiscountAmount,
					Transport = cr.Transport,
					TVAAmount = cr.TVAAmount,
					idSalePurchage = cr.CustomerReturnID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);

			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
		private bool comptabiliseEntreStockRetourVente(CustomerReturn cr, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				string codetrn = "RTSI";
				string desc = Resources.descRTSI;
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				//RECUPERATION DE TTTES LES LIGNE Du retour de vente ki n'on pas enc ete comptabilise
				foreach (CustomerReturnLine crline in context.CustomerReturnLines.Where(p => p.CustomerReturnID == cr.CustomerReturnID).ToList())
				{
					double mountOfCurrentSale = (crline.LineQuantity * crline.SaleLine.LineUnitPrice);
					crline.SaleLine.Product = context.Products.SingleOrDefault(p => p.ProductID == crline.SaleLine.ProductID);
                    if (!(crline.SaleLine.Product is OrderLens))
                    {
                        int ProductLocalisationID = 0;
                        //recuperation du productLocalization
                        if (!(crline.SaleLine.Product.Category.isSerialNumberNull))
                        {
                            ProductLocalisationID = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == crline.SaleLine.ProductID && pl.LocalizationID == crline.SaleLine.LocalizationID).ProductLocalizationID;
                        }
                        else
                        {
                            ProductLocalisationID = context.ProductLocalizations.FirstOrDefault(pl => pl.ProductID == crline.SaleLine.ProductID && pl.LocalizationID == crline.SaleLine.LocalizationID && pl.NumeroSerie == crline.SaleLine.NumeroSerie && pl.Marque == crline.SaleLine.marque).ProductLocalizationID;
                        }
                            
                        if (ProductLocalisationID == null || ProductLocalisationID <= 0)
                        {
                            res = false;
                            throw new Exception("Error while running Operation. Wrong Product Localization Value ");
                        }
                        EcritureGLHist eGLConstatVente = new EcritureGLHist()
                        {
                            BranchID = cr.Sale.BranchID,
                            DeviseID = cr.Sale.DeviseID,
                            OperationID = operationID,
                            AccountIDTierCusto = cr.Sale.Customer.AccountID,
                            AccountIDTierProduct = crline.SaleLine.Product.AccountID,
                            AccountIDTresor = 0,
                            DateOperation = bsday.BDDateOperation.Date,
                            Description = desc + crline.SaleLine.Product.ProductCode,
                            Reference = crline.CustomerReturn.Sale.SaleReceiptNumber,
                            CodeTransaction = CodeTransaction,
                            MontantPrincDB = mountOfCurrentSale,
                            MontantPrincCR = mountOfCurrentSale,
                            Discount = crline.CustomerReturn.DiscountAmount,
                            Transport = crline.CustomerReturn.Transport,
                            TVAAmount = crline.CustomerReturn.TVAAmount,
                            idSalePurchage = ProductLocalisationID
                        };
                        //comptabilisation
                        res = EcritHistoGrandLivre(eGLConstatVente, true);
                        if (!res)
                        {
                            res = false;
                            throw new Exception("Error while running Operation. Please contact your Provider ");
                        }
                    }
                    
					//mise a jr de la ligne
					//crline.isPost = true;

				} //fin boucle sur les ligne de vente
				res = checkPartieDouble(CodeTransaction);
			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
		private bool comptabiliseRetourVenteReglement(CustomerReturn cr, Branch br, BusinessDay bsday, string OperationCode, PaymentMethod paymentMethod)
		{
			bool res = true;
			int AccountIDTresor = 0;
			double mountOfCurrentSale = 0;
			string codetrn = "";
			string desc = "";

			try
			{
				//recup de l'id de l'operation
				Operation operation = context.Operations.Where(a => a.OperationCode == OperationCode).SingleOrDefault();
				int operationID = operation != null ? operation.OperationID : 0;

				if (paymentMethod != null)
				{
					//recuperation du montant de la tranche
					mountOfCurrentSale = cr.TotalPriceReturn;
					//We determine type of this sale
					if (paymentMethod is Till)
					{
						codetrn = "TILL";
						desc = Resources.descTILLSARET;
						AccountIDTresor = context.Tills.SingleOrDefault(a => a.ID == cr.Sale.PaymentMethodID).AccountID;
					}
					else if (paymentMethod is Bank)
					{
						codetrn = "BANK";
						desc = Resources.descBANKSARET;
						AccountIDTresor = context.Banks.SingleOrDefault(a => a.ID == cr.Sale.PaymentMethodID).AccountID;
					}
                    else //aucne methode de paymenet def
                    {
                        res = false;
                        throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
                    }
				}
				else //aucne methode de paymenet def
				{
					res = false;
					throw new Exception("Error while running Operation.Bad Payment Method: Please contact your Provider ");
				}
                _trnRepository = new TransactNumberRepository(this.context);
                string CodeTransaction = _trnRepository.returnTransactNumber(codetrn, bsday);
                if (CodeTransaction == "")
                {
                    res = false;
                    throw new Exception("Error while running Operation.Transaction Number is Null: Please contact your Provider ");
                }
				EcritureGLHist eGLConstatVente = new EcritureGLHist()
				{
					BranchID = cr.Sale.BranchID,
					DeviseID = cr.Sale.DeviseID,
					OperationID = operationID,
					AccountIDTierCusto = cr.Sale.Customer.AccountID,
					AccountIDTierProduct = 0,
					AccountIDTresor = AccountIDTresor,
                    DateOperation = bsday.BDDateOperation.Date,
					Description = desc + cr.Sale.Customer.Name,
					Reference = cr.Sale.SaleReceiptNumber,
					CodeTransaction = CodeTransaction,
					MontantPrincDB = mountOfCurrentSale,
					MontantPrincCR = mountOfCurrentSale,
					Discount = cr.DiscountAmount,
					Transport = cr.Transport,
					TVAAmount = cr.TVAAmount,
					idSalePurchage = cr.CustomerReturnID
				};
				//comptabilisation
				res = EcritHistoGrandLivre(eGLConstatVente);
				if (!res)
				{
					res = false;
					throw new Exception("Error while running Operation. Please contact your Provider ");
				}

				res = checkPartieDouble(CodeTransaction);

			}
			catch (Exception ex)
			{
				res = false;
				throw new Exception(ex.Message);
			}
			return res;
		}
		private bool comptabiliseRetourVenteACredit(CustomerReturn cr, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;
			//constat retour marchandise     ssi client noncash
            res = comptabiliseConstatRetourVente(cr, br, bsday, OperationCode);
            if (!res)
            {
                res = false;
                throw new Exception("Error while running Operation comptabiliseRetourVente. Please contact your Provider ");
            }
            List<CustomerSlice> slice = context.CustomerSlices.Where(r => r.SaleID == cr.SaleID).ToList();
            if (slice.Count >0) //il existe deja des tranches
            {
                
                double paidAmount = slice.Select(l => l.SliceAmount).Sum();
                //le mnt a retourne doit etre inf a la somme des montant de
                double retAmnt = 0d;
                if (paidAmount <= cr.TotalPriceReturn) retAmnt = paidAmount;
                else retAmnt = cr.TotalPriceReturn;
                //retour a l'expediteur
                //mecuperation de la caisse ki a valider l'operation
                Sale tellerSale = context.Sales.Find(cr.SaleID);
                UserTill currenteller = context.UserTills.Where(c => c.UserID == tellerSale.OperatorID).FirstOrDefault();
                //comptabilisation du reglement si le client a  deja verser l'argent
                ////recuperation de la premiere caisse ouverte
                //TillDay tillDay = context.TillDays.FirstOrDefault(td => td.IsOpen);
                //if (tillDay == null)
                //{
                //    throw new Exception("At Least One Cash Register Must Open Before Proceed ");
                //}
                int PaymentMethodID = currenteller.TillID;// context.Tills.FirstOrDefault(t => t.BranchID == cr.Sale.BranchID).ID;
                PaymentMethod paymentMethod = context.PaymentMethods.Find(PaymentMethodID);
                res = comptabiliseReglementRetourClient(cr, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT, paymentMethod, retAmnt);
                if (!res)
                {
                    res = false;
                    throw new Exception("Error while running Operation comptabiliseRetourVente. Please contact your Provider ");
                }
            }
            
			//retour m/se en stock
			res = comptabiliseEntreStockRetourVente(cr, br, bsday, CodeValue.Accounting.InitOperation.CODESTOCKINPUT);
			if (!res)
			{
				res = false;
				throw new Exception("Error while running Operation comptabiliseEntreStockRetourVente. Please contact your Provider ");
			}
			return res;
		}
		private bool comptabiliseRetourClientAvance(CustomerReturn cr, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;
			//pour l'instant on fait un retour en stock coe pr vente a credit
			res = comptabiliseRetourVenteACredit(cr, br, bsday, OperationCode);
			if (!res)
			{
				res = false;
				throw new Exception("Error while running Operation comptabiliseRetourClientCredit. Please contact your Provider ");
			}
			return res;
		}
		private bool comptabiliseRetourClientTotale(CustomerReturn cr, Branch br, BusinessDay bsday, string OperationCode)
		{
			bool res = true;
			res = comptabiliseRetourVenteACredit(cr, br, bsday, OperationCode);
			if (!res)
			{
				res = false;
				throw new Exception("Error while running Operation comptabiliseEntreStockRetourVente. Please contact your Provider ");
			}
			//pour l'instant on ne touche pas l'argent a la caisse ou a la banque
			//retour reglement
			//PaymentMethod paymentMethod = context.PaymentMethods.Find(cr.Sale.PaymentMethodID);
			//if (paymentMethod is Bank)
			//{
			//    res = comptabiliseRetourVenteReglement(cr, br, bsday, CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT, paymentMethod);
			//    if (!res)
			//    {
			//        res = false;
			//        throw new Exception("Error while running Operation comptabiliseEntreStockRetourVente. Please contact your Provider ");
			//    }
			//}
			//if (paymentMethod is Till)
			//{
			//    res = comptabiliseRetourVenteReglement(cr, br, bsday, CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT, paymentMethod);
			//    if (!res)
			//    {
			//        res = false;
			//        throw new Exception("Error while running Operation comptabiliseEntreStockRetourVente. Please contact your Provider ");
			//    }
			//}
			return res;
		}
		/// <summary>
		/// ecriture d'une ligne d'achat ds le grand livre
		/// </summary>
		/// <param name="accountOperation"></param>
		/// <param name="MontantDBP"></param>
		/// <param name="MontantCRP"></param>
		/// <param name="operationTypeID"></param>
		/// <param name="sensop"></param>
		/// <param name="idSalePurchage"></param>
		/// <param name="MontantTVA"></param>
		/// <param name="VatAccountID"></param>
		/// <param name="Discount"></param>
		/// <param name="DiscountAccountID"></param>
		/// <param name="Transport"></param>
		/// <param name="TransportAccountID"></param>
		/// <returns></returns>
		private bool ecritureLignePurchaseGL(AccountOperation accountOperation, double MontantDBP, double MontantCRP, int operationTypeID, string sensop, int idSalePurchage, double MontantTVA, int VatAccountID,
				double Discount, int DiscountAccountID, double Transport, int TransportAccountID)
		{
			bool res = false;

			if (sensop.ToUpper().Trim() == "DB")
			{
				if (MontantDBP > 0) //mtant principal
				{
					PurchaseAccountOperation glOperation = new PurchaseAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.PurchaseID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantDBP;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					PurchaseAccountOperation glOperation = new PurchaseAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.PurchaseID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantTVA;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					PurchaseAccountOperation glOperation = new PurchaseAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.PurchaseID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Discount;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					PurchaseAccountOperation glOperation = new PurchaseAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.PurchaseID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Transport;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			else //credit account
			{
				if (MontantCRP > 0) //mtant principal
				{
					PurchaseAccountOperation glOperation = new PurchaseAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.PurchaseID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantCRP;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					PurchaseAccountOperation glOperation = new PurchaseAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.PurchaseID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantTVA;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					PurchaseAccountOperation glOperation = new PurchaseAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.PurchaseID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Discount;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					PurchaseAccountOperation glOperation = new PurchaseAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.PurchaseID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Transport;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			//
			return res;
		}

		private bool ecritureLignePurchaseReturnGL(AccountOperation accountOperation, double MontantDBP, double MontantCRP, int operationTypeID, string sensop, int idSalePurchage, double MontantTVA, int VatAccountID,
				double Discount, int DiscountAccountID, double Transport, int TransportAccountID)
		{
			bool res = false;

			if (sensop.ToUpper().Trim() == "DB")
			{
				if (MontantDBP > 0) //mtant principal
				{
					PurchaseReturnAccountOperation glOperation = new PurchaseReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SupplierReturnID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantDBP;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					PurchaseReturnAccountOperation glOperation = new PurchaseReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SupplierReturnID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantTVA;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					PurchaseReturnAccountOperation glOperation = new PurchaseReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SupplierReturnID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Discount;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					PurchaseReturnAccountOperation glOperation = new PurchaseReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SupplierReturnID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Transport;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			else //credit account
			{
				if (MontantCRP > 0) //mtant principal
				{
					PurchaseReturnAccountOperation glOperation = new PurchaseReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SupplierReturnID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantCRP;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					PurchaseReturnAccountOperation glOperation = new PurchaseReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SupplierReturnID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantTVA;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					PurchaseReturnAccountOperation glOperation = new PurchaseReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SupplierReturnID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Discount;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					PurchaseReturnAccountOperation glOperation = new PurchaseReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SupplierReturnID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Transport;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			return res;
		}

		private bool ecritureLigneSaleReturnGL(AccountOperation accountOperation, double MontantDBP, double MontantCRP, int operationTypeID, string sensop, int idSalePurchage, double MontantTVA, int VatAccountID,
				double Discount, int DiscountAccountID, double Transport, int TransportAccountID)
		{
			bool res = false;

			if (sensop.ToUpper().Trim() == "DB")
			{
				if (MontantDBP > 0) //mtant principal
				{
					SaleReturnAccountOperation glOperation = new SaleReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.CustomerReturnID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantDBP;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					SaleReturnAccountOperation glOperation = new SaleReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.CustomerReturnID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantTVA;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					SaleReturnAccountOperation glOperation = new SaleReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.CustomerReturnID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Discount;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					SaleReturnAccountOperation glOperation = new SaleReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.CustomerReturnID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Transport;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			else //credit account
			{
				if (MontantCRP > 0) //mtant principal
				{
					SaleReturnAccountOperation glOperation = new SaleReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.CustomerReturnID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantCRP;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					SaleReturnAccountOperation glOperation = new SaleReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.CustomerReturnID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantTVA;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					SaleReturnAccountOperation glOperation = new SaleReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.CustomerReturnID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Discount;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					SaleReturnAccountOperation glOperation = new SaleReturnAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.CustomerReturnID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Transport;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			return res;
		}

		private bool ecritureLigneProductTransferGL(AccountOperation accountOperation, double MontantDBP, double MontantCRP, int operationTypeID, string sensop, int idSalePurchage, double MontantTVA, int VatAccountID,
				double Discount, int DiscountAccountID, double Transport, int TransportAccountID)
		{
			bool res = false;

			if (sensop.ToUpper().Trim() == "DB")
			{
				if (MontantDBP > 0) //mtant principal
				{
					ProductTransferAccountOperation glOperation = new ProductTransferAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductTransfertID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantDBP;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					ProductTransferAccountOperation glOperation = new ProductTransferAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductTransfertID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantTVA;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					ProductTransferAccountOperation glOperation = new ProductTransferAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductTransfertID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Discount;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					ProductTransferAccountOperation glOperation = new ProductTransferAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductTransfertID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Transport;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			else //credit account
			{
				if (MontantCRP > 0) //mtant principal
				{
					ProductTransferAccountOperation glOperation = new ProductTransferAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductTransfertID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantCRP;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					ProductTransferAccountOperation glOperation = new ProductTransferAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductTransfertID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantTVA;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					ProductTransferAccountOperation glOperation = new ProductTransferAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductTransfertID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Discount;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					ProductTransferAccountOperation glOperation = new ProductTransferAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductTransfertID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Transport;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			return res;
		}
        private bool ecritureLigneDepositGL(AccountOperation accountOperation, double MontantDBP, double MontantCRP, int operationTypeID, string sensop, int idSalePurchage, double MontantTVA, int VatAccountID,
                double Discount, int DiscountAccountID, double Transport, int TransportAccountID)
        {
            bool res = false;

            if (sensop.ToUpper().Trim() == "DB")
            {
                if (MontantDBP > 0) //mtant principal
                {
                    DepositAccountOperation glOperation = new DepositAccountOperation();
                    glOperation.BranchID = accountOperation.BranchID;
                    glOperation.DeviseID = accountOperation.DeviseID;
                    glOperation.OperationID = accountOperation.OperationID;
                    glOperation.DateOperation = accountOperation.DateOperation;
                    glOperation.CodeTransaction = accountOperation.CodeTransaction;
                    glOperation.Description = accountOperation.Description;
                    glOperation.Reference = accountOperation.Reference;
                    glOperation.DepositID = idSalePurchage;
                    glOperation.AccountID = accountOperation.AccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
                    glOperation.Credit = 0;
                    glOperation.Debit = MontantDBP;
                    context.AccountOperations.Add(glOperation);
                    context.SaveChanges();
                    res = true;
                }
                if (MontantTVA > 0 && VatAccountID > 0) //tva
                {
                    //debit cpte tva
                    DepositAccountOperation glOperation = new DepositAccountOperation();
                    glOperation.BranchID = accountOperation.BranchID;
                    glOperation.DeviseID = accountOperation.DeviseID;
                    glOperation.OperationID = accountOperation.OperationID;
                    glOperation.DateOperation = accountOperation.DateOperation;
                    glOperation.CodeTransaction = accountOperation.CodeTransaction;
                    glOperation.Description = accountOperation.Description;
                    glOperation.Reference = accountOperation.Reference;
                    glOperation.DepositID = idSalePurchage;
                    glOperation.AccountID = VatAccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
                    glOperation.Credit = 0;
                    glOperation.Debit = MontantTVA;
                    context.AccountOperations.Add(glOperation);
                    context.SaveChanges();
                    res = true;
                }

            }
            else //credit account
            {
                if (MontantCRP > 0) //mtant principal
                {
                    DepositAccountOperation glOperation = new DepositAccountOperation();
                    glOperation.BranchID = accountOperation.BranchID;
                    glOperation.DeviseID = accountOperation.DeviseID;
                    glOperation.OperationID = accountOperation.OperationID;
                    glOperation.DateOperation = accountOperation.DateOperation;
                    glOperation.CodeTransaction = accountOperation.CodeTransaction;
                    glOperation.Description = accountOperation.Description;
                    glOperation.Reference = accountOperation.Reference;
                    glOperation.DepositID = idSalePurchage;
                    glOperation.AccountID = accountOperation.AccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
                    glOperation.Credit = MontantCRP;
                    glOperation.Debit = 0;
                    context.AccountOperations.Add(glOperation);
                    context.SaveChanges();
                    res = true;
                }
                if (MontantTVA > 0 && VatAccountID > 0) //tva
                {
                    //debit cpte tva
                    DepositAccountOperation glOperation = new DepositAccountOperation();
                    glOperation.BranchID = accountOperation.BranchID;
                    glOperation.DeviseID = accountOperation.DeviseID;
                    glOperation.OperationID = accountOperation.OperationID;
                    glOperation.DateOperation = accountOperation.DateOperation;
                    glOperation.CodeTransaction = accountOperation.CodeTransaction;
                    glOperation.Description = accountOperation.Description;
                    glOperation.Reference = accountOperation.Reference;
                    glOperation.DepositID = idSalePurchage;
                    glOperation.AccountID = VatAccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
                    glOperation.Credit = MontantTVA;
                    glOperation.Debit = 0;
                    context.AccountOperations.Add(glOperation);
                    context.SaveChanges();
                    res = true;
                }

                
            }
            //
            return res;
        }
		private bool ecritureLigneSaleGL(AccountOperation accountOperation, double MontantDBP, double MontantCRP, int operationTypeID, string sensop, int idSalePurchage, double MontantTVA, int VatAccountID,
				double Discount, int DiscountAccountID, double Transport, int TransportAccountID)
		{
			bool res = false;

			if (sensop.ToUpper().Trim() == "DB")
			{
				if (MontantDBP > 0) //mtant principal
				{
					SaleAccountOperation glOperation = new SaleAccountOperation();
					//glOperation.AccountOperationID = glOperation.AccountOperationID;
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SaleID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantDBP;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					SaleAccountOperation glOperation = new SaleAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SaleID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantTVA;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					SaleAccountOperation glOperation = new SaleAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SaleID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Discount;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					SaleAccountOperation glOperation = new SaleAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SaleID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Transport;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			else //credit account
			{
				if (MontantCRP > 0) //mtant principal
				{
					SaleAccountOperation glOperation = new SaleAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SaleID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantCRP;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					SaleAccountOperation glOperation = new SaleAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SaleID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantTVA;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					SaleAccountOperation glOperation = new SaleAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SaleID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Discount;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					SaleAccountOperation glOperation = new SaleAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.SaleID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Transport;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			//
			return res;
		}
		/// <summary>
		/// methode d'ecriture ds le GL en fonction du type d'operation
		/// </summary>
		/// <param name="accountOperation"></param>
		/// <param name="MontantPrincDB"></param>
		/// <param name="MontantPrincCR"></param>
		/// <param name="operationTypeID"></param>
		/// <param name="sensop"></param>
		/// <param name="idSalePurchage"></param>
		/// <param name="MontantTVA"></param>
		/// <param name="VatAccountID"></param>
		/// <param name="Discount"></param>
		/// <param name="DiscountAccountID"></param>
		/// <param name="Transport"></param>
		/// <param name="TransportAccountID"></param>
		/// <param name="isDeposit"></param>
		/// <returns></returns>
		public bool ecritureLigneGL(AccountOperation accountOperation, double MontantPrincDB, double MontantPrincCR, int operationTypeID, string sensop,
				int idSalePurchage, double MontantTVA, int VatAccountID, double Discount, int DiscountAccountID, double Transport, int TransportAccountID, bool isDeposit = false)
		{
			bool res = true;
			//recuperation de la table fille
			OperationType opType = context.OperationTypes.SingleOrDefault(op => op.operationTypeID == operationTypeID);
			string operationtypecode = opType != null ? opType.operationTypeCode : "";

			//if (isReturn)
			//{
			//	if (operationtypecode == CodeValue.Accounting.InitOperationType.CODESTOCKINPUT) // entree en stock retour vente
			//	{
			//		res = ecritureLigneSaleReturnGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
			//	}
			//	if (operationtypecode == CodeValue.Accounting.InitOperationType.CODESTOCKOUTPUT) //sortie en stock retour achat
			//	{
			//		res = ecritureLignePurchaseReturnGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
			//	}
			//}
			//else
			//{
			if (operationtypecode == CodeValue.Accounting.InitOperationType.CODESTOCKINPUT) // entree en stock achat
			{
				res = ecritureLigneStockInputOutputGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
			}
			if (operationtypecode == CodeValue.Accounting.InitOperationType.CODESTOCKOUTPUT) //sortie en stock vente
			{
				res = ecritureLigneStockInputOutputGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
			}
			//}
			//ecriture des opererations ds le GL
			if (operationtypecode == CodeValue.Accounting.InitOperationType.CODEPURCHASE) //achat
			{
				res = ecritureLignePurchaseGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
			}
			if (operationtypecode == CodeValue.Accounting.InitOperationType.CODEPURCHASERETURN) //retour achat
			{
				res = ecritureLignePurchaseReturnGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
			}
			if (operationtypecode == CodeValue.Accounting.InitOperationType.CODESALERETURN) //retour vente
			{
				res = ecritureLigneSaleReturnGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
			}
			if (operationtypecode == CodeValue.Accounting.InitOperationType.CODESALE) //vente
			{
                if (isDeposit)
                {
                    res = ecritureLigneDepositGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
                }
                else
                {
                    res = ecritureLigneSaleGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
                }
			}
			if (operationtypecode == CodeValue.Accounting.InitOperationType.CODESTOCKDEPRECIATE) //stock depreciate
			{
				res = ecritureLigneStockInputOutputGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
			}
			if (operationtypecode == CodeValue.Accounting.InitOperationType.CODETELLERADJUST || operationtypecode == CodeValue.Accounting.InitOperationType.CODEOVERAGE) //shortage
			{
				res = ecritureLigneShortageOverageGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
			}

            ////livraison avec avance
            //if (operationtypecode == CodeValue.Accounting.InitOperationType.CODESALERETURN) //retour vente
            //{
            //    res = ecritureLigneSaleReturnGL(accountOperation, MontantPrincDB, MontantPrincCR, operationTypeID, sensop, idSalePurchage, MontantTVA, VatAccountID, Discount, DiscountAccountID, Transport, TransportAccountID);
            //}

            //else
            //{
            //	//methode pas encore implemente
            //	res = true;
            //}
            return res;
		}
		private bool ecritureLigneShortageOverageGL(AccountOperation accountOperation, double MontantDBP, double MontantCRP, int operationTypeID, string sensop, int idSalePurchage, double MontantTVA, int VatAccountID,
				double Discount, int DiscountAccountID, double Transport, int TransportAccountID)
		{
			bool res = false;

			if (sensop.ToUpper().Trim() == "DB")
			{
				if (MontantDBP > 0) //mtant principal
				{
					TillAdjustAccountOperation glOperation = new TillAdjustAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.TillAdjustID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantDBP;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					TillAdjustAccountOperation glOperation = new TillAdjustAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.TillAdjustID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantTVA;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					TillAdjustAccountOperation glOperation = new TillAdjustAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.TillAdjustID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Discount;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					TillAdjustAccountOperation glOperation = new TillAdjustAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.TillAdjustID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Transport;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			else //credit account
			{
				if (MontantCRP > 0) //mtant principal
				{
					TillAdjustAccountOperation glOperation = new TillAdjustAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.TillAdjustID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantCRP;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					TillAdjustAccountOperation glOperation = new TillAdjustAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.TillAdjustID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantTVA;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					TillAdjustAccountOperation glOperation = new TillAdjustAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.TillAdjustID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Discount;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					TillAdjustAccountOperation glOperation = new TillAdjustAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.TillAdjustID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
					glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Transport;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			return res;
		}
		/// <summary>
		/// stock input/output/depreciate
		/// </summary>
		/// <param name="accountOperation"></param>
		/// <param name="MontantDBP"></param>
		/// <param name="MontantCRP"></param>
		/// <param name="operationTypeID"></param>
		/// <param name="sensop"></param>
		/// <param name="idSalePurchage"></param>
		/// <param name="MontantTVA"></param>
		/// <param name="VatAccountID"></param>
		/// <param name="Discount"></param>
		/// <param name="DiscountAccountID"></param>
		/// <param name="Transport"></param>
		/// <param name="TransportAccountID"></param>
		/// <returns></returns>
		private bool ecritureLigneStockInputOutputGL(AccountOperation accountOperation, double MontantDBP, double MontantCRP, int operationTypeID, string sensop, int idSalePurchage, double MontantTVA, int VatAccountID,
				double Discount, int DiscountAccountID, double Transport, int TransportAccountID)
		{
			bool res = false;

			if (sensop.ToUpper().Trim() == "DB")
			{
				if (MontantDBP == 0)
				{
					// pas normal mais on sort de la methode sans rien faire pour l'instant
					res = true;
				}
				if (MontantDBP > 0) //mtant principal
				{
					ProductLocalizationAccountOperation glOperation = new ProductLocalizationAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductLocalizationID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantDBP;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					ProductLocalizationAccountOperation glOperation = new ProductLocalizationAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductLocalizationID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = MontantTVA;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}

				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					ProductLocalizationAccountOperation glOperation = new ProductLocalizationAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductLocalizationID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Discount;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					ProductLocalizationAccountOperation glOperation = new ProductLocalizationAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductLocalizationID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = 0;
					glOperation.Debit = Transport;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			else //credit account
			{
				if (MontantCRP == 0)
				{
					// pas normal mais on sort de la methode sans rien faire pour l'instant
					res = true;
				}
				if (MontantCRP > 0) //mtant principal
				{
					ProductLocalizationAccountOperation glOperation = new ProductLocalizationAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductLocalizationID = idSalePurchage;
					glOperation.AccountID = accountOperation.AccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantCRP;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (MontantTVA > 0 && VatAccountID > 0) //tva
				{
					//debit cpte tva
					ProductLocalizationAccountOperation glOperation = new ProductLocalizationAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductLocalizationID = idSalePurchage;
					glOperation.AccountID = VatAccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = MontantTVA;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Discount > 0 && DiscountAccountID > 0) //escompte
				{
					ProductLocalizationAccountOperation glOperation = new ProductLocalizationAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductLocalizationID = idSalePurchage;
					glOperation.AccountID = DiscountAccountID;
                    glOperation.AccountTierID =accountOperation.AccountTierID;
					glOperation.Credit = Discount;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
				if (Transport > 0 && TransportAccountID > 0) //transport
				{
					ProductLocalizationAccountOperation glOperation = new ProductLocalizationAccountOperation();
					glOperation.BranchID = accountOperation.BranchID;
					glOperation.DeviseID = accountOperation.DeviseID;
					glOperation.OperationID = accountOperation.OperationID;
					glOperation.DateOperation = accountOperation.DateOperation;
					glOperation.CodeTransaction = accountOperation.CodeTransaction;
					glOperation.Description = accountOperation.Description;
					glOperation.Reference = accountOperation.Reference;
					glOperation.ProductLocalizationID = idSalePurchage;
					glOperation.AccountID = TransportAccountID;
                    glOperation.AccountTierID = accountOperation.AccountTierID;
					glOperation.Credit = Transport;
					glOperation.Debit = 0;
					context.AccountOperations.Add(glOperation);
					context.SaveChanges();
					res = true;
				}
			}
			return res;
		}
		public bool ecritureManualLigneGL(AccountOperation accountOperation, double Montant, int operationTypeID, string sensop, long PieceID)
		{
			bool res = false;
			//recuperation de la table fille
			string operationtypecode = context.OperationTypes.SingleOrDefault(op => op.operationTypeID == operationTypeID).operationTypeCode;
			if (sensop.ToUpper().Trim() == "DB")
			{
				//ecriture des opererations en debit ds le GL
				if (operationtypecode == CodeValue.Accounting.InitOperationType.CODEMANUAL) //OPERATION MANUEL
				{
					ManualAccountOperation AccountOperationSave = new ManualAccountOperation();
					AccountOperationSave.AccountID = accountOperation.AccountID;
					AccountOperationSave.BranchID = accountOperation.BranchID;
					AccountOperationSave.DeviseID = accountOperation.DeviseID;
					AccountOperationSave.OperationID = accountOperation.OperationID;
					AccountOperationSave.DateOperation = accountOperation.DateOperation;
					AccountOperationSave.CodeTransaction = accountOperation.CodeTransaction;
					AccountOperationSave.Credit = 0;
					AccountOperationSave.Debit = Montant;
					AccountOperationSave.Description = accountOperation.Description;
					AccountOperationSave.Reference = accountOperation.Reference;
					AccountOperationSave.PieceID = PieceID;
					context.AccountOperations.Add(AccountOperationSave);
					context.SaveChanges();
					res = true;
				}
			}
			else if (sensop.ToUpper().Trim() == "CR")
			{
				//ecriture des opererations en debit ds le GL
				if (operationtypecode == CodeValue.Accounting.InitOperationType.CODEMANUAL) //OPERATION MANUEL
				{
					ManualAccountOperation AccountOperationSave = new ManualAccountOperation();
					AccountOperationSave.AccountID = accountOperation.AccountID;
					AccountOperationSave.BranchID = accountOperation.BranchID;
					AccountOperationSave.DeviseID = accountOperation.DeviseID;
					AccountOperationSave.OperationID = accountOperation.OperationID;
					AccountOperationSave.DateOperation = accountOperation.DateOperation;
					AccountOperationSave.CodeTransaction = accountOperation.CodeTransaction;
					AccountOperationSave.Credit = Montant;
					AccountOperationSave.Debit = 0;
					AccountOperationSave.Description = accountOperation.Description;
					AccountOperationSave.Reference = accountOperation.Reference;
					AccountOperationSave.PieceID = PieceID;
					context.AccountOperations.Add(AccountOperationSave);
					context.SaveChanges();
					res = true;
				}
			}
			else //si pas choix sens operation
			{
				res = false;
				throw new Exception("Error while running Operation. Please contact your Provider ");
			}
			return res;
		}

		
	}
}
