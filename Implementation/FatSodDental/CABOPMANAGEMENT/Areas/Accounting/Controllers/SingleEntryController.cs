using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Globalization;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
	public class SingleEntryController : BaseController
		{
		//Current Controller and current page
		private const string CONTROLLER_NAME = "Accounting/SingleEntry";
		private const string VIEW_NAME = "Index";
		//person repository

		private IAccountOperation _accountOperationRepository;
		private IPiece _pieceRepository;
		private ITransactNumber _transactNumbeRepository;
		private IBusinessDay _businessDayRepository;

        
        private List<BusinessDay> listBDUser;

        public SingleEntryController(IAccountOperation accountOperationRepository, IPiece pieceRepository,
			ITransactNumber transactNumbeRepository, IBusinessDay businessDayRepository)
			{
			this._accountOperationRepository = accountOperationRepository;
			this._pieceRepository = pieceRepository;
			this._transactNumbeRepository = transactNumbeRepository;
			this._businessDayRepository = businessDayRepository;
            
			}
		//
		// GET: /Accounting/AccountOperation/
        [OutputCache(Duration = 3600)] 
		public ActionResult Index()
			{

            ViewBag.Disabled = true;
            listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _businessDayRepository.GetOpenedBusinessDay(CurrentUser);
            }
            if (listBDUser.Count() > 1)
            {
                ViewBag.Disabled = false;
            }
            BusinessDay busDays = listBDUser.FirstOrDefault();
            ViewBag.BusnessDayDate = busDays.BDDateOperation.ToString("yyyy-MM-dd");
            ViewBag.CurrentBranch = busDays.BranchID;
            Session["BusnessDayDate"] = busDays.BDDateOperation;

            int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
            if (deviseID <= 0)
            {
                InjectUserConfigInSession();
            }
            deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
            ViewBag.DefaultDeviseID = deviseID;
            ViewBag.DefaultDevise = (deviseID <= 0) ? "" : db.Devises.Find(deviseID).DeviseCode;

            return View(/*ModelAcctOp()*/);
			}

        //public List<Piece> ModelAcctOp()
        //{
        //    List<Piece> list = new List<Piece>();
        //    db.Pieces.Where(p => !p.isAcctOperation).ToList().ForEach(c =>
        //    {
        //        list.Add(
        //                new Piece
        //                {
        //                    PieceID = c.PieceID,
        //                    Branch=c.Branch,
        //                    Devise=c.Devise,
        //                    Operation=c.Operation,
        //                    Account=c.Account,
        //                    //UIBranchCode = c.UIBranchCode,
        //                    //UIDeviseCode = c.UIDeviseCode,
        //                    //UIOperationCode = c.UIOperationCode,
        //                    //UIAccountNumber = c.UIAccountNumber,
        //                    DateOperation = c.DateOperation,
        //                    Description = c.Description,
        //                    Reference = c.Reference,
        //                    CodeTransaction = c.CodeTransaction,
        //                    Debit = c.Debit,
        //                    Credit = c.Credit
        //                }
        //        );
        //    });
        //    return list;
        //}

        public JsonResult GetManualPostingAccountNames()
        {
            List<object> accountingList = new List<object>();

            foreach (Account accounting in db.Accounts.Where(m => m.isManualPosting).ToArray().OrderBy(m => m.AccountNumber))
            {
                accountingList.Add(
                    new  {
                        AccountNumber = accounting.AccountNumber.ToString() + "-" + accounting.AccountLabel,
                        AccountID = accounting.AccountID
                    } );
            }

            return Json(accountingList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetList(int BranchID, int DeviseID, string CodeTransaction)
        {
            List<Piece> list = new List<Piece>();
            if (BranchID > 0 && DeviseID > 0 && CodeTransaction.Length > 0)
            {
                db.Pieces.Where(pi => pi.BranchID == BranchID && pi.DeviseID == DeviseID && !pi.isAcctOperation && pi.CodeTransaction == CodeTransaction).ToList().ForEach(c =>
                {
                    list.Add(
                            new Piece
                            {
                                PieceID = c.PieceID,
                                //UIBranchCode = c.UIBranchCode,
                                //UIDeviseCode = c.UIDeviseCode,
                                //UIOperationCode = c.UIOperationCode,
                                CodeTransaction = c.CodeTransaction,
                                Account = c.Account,
                                //DateOperation = c.DateOperation,
                                //Description = c.Description,
                                //Reference = c.Reference,
                                Debit = c.Debit,
                                Credit = c.Credit
                            }
                        );
                });
            }

            var model = new
            {
                data = from c in list
                       select new
                       {
                           PieceID = c.PieceID,
                           CodeTransaction = c.CodeTransaction,
                           AccountNumber = c.Account.AccountNumber.ToString(),
                           Debit = c.Debit,
                           Credit = c.Credit
                       }
            };

            return Json(model, JsonRequestBehavior.AllowGet);
            //return Json(list, JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        public ActionResult AddVoucher(Piece piece, string AccountDebitID, string AccountCreditID, double Amount)
			{
            bool status = false;
            try
				{

                int OperationTypeId = LoadComponent.GetOperationTypeID(piece.Journal);
                piece.OperationID = db.Operations.FirstOrDefault(p => p.OperationTypeID == OperationTypeId).OperationID;

				if (piece.PieceID > 0)
					{
					//update de la table accounting task
					statusOperation = Resources.UIAddVoucher +":"+ piece.Description + " already exist : Please Contact your Administrator "; // + Resources.AlertUpdateAction;
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = statusOperation } };
                }
				else
					{

					SingleEntry singleEntry = new SingleEntry();
					singleEntry.AccountCreditID = Convert.ToInt32(AccountCreditID);
					singleEntry.AccountDebitID = Convert.ToInt32(AccountDebitID);
					singleEntry.Amount = Amount;
					singleEntry.BranchID = piece.BranchID;
                    /*
                    Branch currentBranch = _branchRepository.Find(piece.BranchID);
                    businessDay = _businessDayRepository.GetOpenedBusinessDay(currentBranch);
                    */
                    singleEntry.CodeTransaction = piece.CodeTransaction;// Session["trnNum"].ToString(); //_transactNumbeRepository.returnTransactNumber("MANU", businessDay);
					singleEntry.DateOperation = piece.DateOperation;
					singleEntry.Description = piece.Description;
					singleEntry.DeviseID = piece.DeviseID;
					singleEntry.OperationID = piece.OperationID;
					singleEntry.Reference = piece.Reference;

					_pieceRepository.EcriturePieceSingleEntry(singleEntry);
					statusOperation = piece.Description + " : " + Resources.AlertAddAction;
					}
			
				//this.chargeSolde();

                status = true;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }
			catch (Exception e)
				{
				statusOperation = Resources.er_alert_danger + e.Message;
                status = false;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }

			}

		
		public ActionResult AddAccountOperation(string CodeTransaction)
		{
            bool status = false;
            try
			{
                if (CodeTransaction.Length>0)
                {
                    _accountOperationRepository.EcritureManuelleHistoGrandLivre(CodeTransaction);
                    statusOperation = Resources.Success + ":" + Resources.AlertAddAction;
                }
                else
                {
                    statusOperation = Resources.AlertAddAction + ": Please add value to voucher before proceed" ;
                }
				
                status = true;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };

			}
			catch (Exception e)
			{
			    statusOperation = Resources.UIAddVoucher +":"+ Resources.er_alert_danger + e.Message;
                status = false;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }

		}
		//[HttpPost]
		public ActionResult RemoveVoucher(int ID)
		{
            bool status = false;
            try
			{
                Piece pieceToDelete = db.Pieces.Find(ID);
                if (pieceToDelete!=null)
                {
                    List<Piece> lstpieceToDelete = db.Pieces.Where(c => c.CodeTransaction == pieceToDelete.CodeTransaction).ToList();
                    db.Pieces.RemoveRange(lstpieceToDelete);
                    db.SaveChanges();
                    status = true;
                    statusOperation = Resources.Success + ":" + pieceToDelete.Reference + " : " + Resources.AlertDeleteAction;
                }
                else
                {
                    status = false;
                    statusOperation = Resources.UIAccount + ": we cannot delete !!!! Data not found";
                }
                
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }
			catch (Exception e)
			{
				statusOperation = Resources.UIAccount+":"+ Resources.er_alert_danger + e.Message;
                status = false;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }
		}

        

        public JsonResult LoadServerdate(int? BranchID)
        {
            List<object> _InfoList = new List<object>();
            string trnnum = "";
            if (BranchID != null && BranchID.HasValue && BranchID.Value > 0)
            {
                List<BusinessDay> listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser == null)
                {
                    listBDUser = _businessDayRepository.GetOpenedBusinessDay(CurrentUser);
                }
                BusinessDay bdDay = listBDUser.FirstOrDefault(b => b.BranchID == BranchID);

                trnnum = _transactNumbeRepository.returnTransactNumber("MANU", bdDay);
                _InfoList.Add(new
                {
                    DateOperation = bdDay.BDDateOperation.ToString("yyyy-MM-dd"),
                    CodeTransaction = trnnum,
                    Journal="MANUAL"
                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult chargeSolde(int BranchID, int DeviseID,string CodeTransaction)
        {
            List<object> _InfoList = new List<object>();
            try
            {

           
                double Totaldebit = 0d;
                double TotalCredit = 0d;
                double Solde = 0d;
                if (CodeTransaction.Length>0)
                {
                    var pieceCpta = db.Pieces.Where(pi => pi.BranchID== BranchID && pi.DeviseID== DeviseID && !pi.isAcctOperation && pi.CodeTransaction == CodeTransaction).ToList();
                    if (pieceCpta!=null)
                    {
                        Totaldebit = pieceCpta.Sum(s => s.Debit);
                        TotalCredit = pieceCpta.Sum(s => s.Credit);

                        Solde = TotalCredit - Totaldebit;
                    }
                
                }
                string culture = (Resources.Culture==null)? "en-US" : Resources.Culture.ToString();

                _InfoList.Add(new
                {
                    TotalDebit = Totaldebit.ToString("0"),
                    TotalCredit = TotalCredit.ToString("0"),
                    Solde = Solde.ToString("0")
                });
            }
            
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                return new JsonResult { Data = new { status = false, Message = statusOperation } };
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);

        }
		
		public JsonResult InitializeFields(string ID)
		{
            List<object> list = new List<object>();
            double Amount = 0d;
            int AccountDebitID = 0;
            int AccountCreditID = 0;

            long id = long.Parse(ID);
            Piece pieceToUpdate =db.Pieces.SingleOrDefault(c => c.PieceID == id);
            if (pieceToUpdate!=null)
            {
			    //this.chargeSolde();
			    if (pieceToUpdate.Debit > 0)
			    {
                    AccountCreditID = 0;
                    AccountDebitID = pieceToUpdate.AccountID;
				    Amount = pieceToUpdate.Debit;
			    }
			    else
			    {
			        AccountCreditID = pieceToUpdate.AccountID;
                    AccountDebitID = 0;
                    Amount = pieceToUpdate.Credit;
			    }

                list.Add(
                   new
                   {
                       PieceID= pieceToUpdate.PieceID,
                       BranchID= pieceToUpdate.BranchID,
                       DeviseID= pieceToUpdate.DeviseID,
                       JournalID= pieceToUpdate.Operation.OperationCode,
                       DateOperation= pieceToUpdate.DateOperation.ToString("yyyy-MM-dd"),
                       Reference= pieceToUpdate.Reference,
                       Description= pieceToUpdate.Description,
                       AccountCreditID= AccountCreditID,
                       AccountDebitID= AccountDebitID,
                       Amount= Amount
                   });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /*** methodes lors du chargement du formulaire*********************/
        public JsonResult GetOpenedBranches()
        {

            List<object> openedBranchesList = new List<object>();
            List<Branch> openedBranches = _businessDayRepository.GetOpenedBranches();
            foreach (Branch branch in openedBranches)
            {
                openedBranchesList.Add(new
                {
                    BranchID = branch.BranchID,
                    BranchName = branch.BranchName
                });
            }

            return Json(openedBranchesList, JsonRequestBehavior.AllowGet);
        }
    }
}