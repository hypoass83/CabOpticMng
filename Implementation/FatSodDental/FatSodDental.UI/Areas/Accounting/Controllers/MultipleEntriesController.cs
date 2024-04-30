using Ext.Net;
using Ext.Net.MVC;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Filters;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace FatSodDental.UI.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class MultipleEntriesController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/MultipleEntries";
        private const string VIEW_NAME = "Index";
        //person repository
        
        private IAccountOperation _accountOperationRepository;
        private IPiece _pieceRepository;
        private ITransactNumber _transactNumbeRepository;
        private IBusinessDay _businessDayRepository;
        
        

        public MultipleEntriesController(IAccountOperation accountOperationRepository, IPiece pieceRepository,
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
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = Model()
            //};
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;

            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Accounting.AccountOperation_multiEntries.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            this.chargeSolde();
            //TODO faudrait se souvenir de l'integration du champs agence dans les criteres de requette

            return View(Model());
        }
        public List<object> Model()
        {
            List<object> list = new List<object>();
            db.Pieces.Where(p => !p.isAcctOperation).ToList().ForEach(c =>
            {
                list.Add(
                                                new
                                                {
                                                    PieceID = c.PieceID,
                                                    UIBranchCode = c.UIBranchCode,
                                                    UIDeviseCode = c.UIDeviseCode,
                                                    UIOperationCode = c.UIOperationCode,
                                                    UIAccountNumber = c.UIAccountNumber,
                                                    DateOperation = c.DateOperation,
                                                    Description = c.Description,
                                                    Reference = c.Reference,
                                                    CodeTransaction = c.CodeTransaction,
                                                    Debit = c.Debit,
                                                    Credit = c.Credit
                                                }
                                );
            });
            return list;
        }
        //[HttpPost]
        public StoreResult GetList()
        {
            return this.Store(Model());
        }

        [HttpPost]
        public ActionResult AddVoucher(Piece piece, string AccountID, string AccountSens, string Amount)
        {
            try
            {
                int OperationTypeId = LoadComponent.GetOperationTypeID(piece.Journal);
                piece.OperationID = db.Operations.FirstOrDefault(p=>p.OperationTypeID==OperationTypeId).OperationID;

                if (piece.PieceID > 0)
                {
                    //update de la table accounting task
                    _pieceRepository.UpdatePiece(piece, piece.PieceID);
                    statusOperation = piece.Description + Resources.AlertUpdateAction;
                    this.AlertSucces(Resources.Success, statusOperation);
                }
                else
                {

                    MultipleEntries multipleEntries = new MultipleEntries();
                    multipleEntries.AccountID = Convert.ToInt32(AccountID);
                    multipleEntries.AccountSens = AccountSens;
                    multipleEntries.Amount = double.Parse(Amount);
                    multipleEntries.BranchID = piece.BranchID;

                    multipleEntries.CodeTransaction = Session["trnNum"].ToString(); 
                    multipleEntries.DateOperation = piece.DateOperation;
                    multipleEntries.Description = piece.Description;
                    multipleEntries.DeviseID = piece.DeviseID;
                    multipleEntries.OperationID = piece.OperationID;
                    multipleEntries.Reference = piece.Reference;

                    _pieceRepository.EcriturePieceMultipleEntry(multipleEntries);
                    statusOperation = piece.Description + " : " + Resources.AlertAddAction;
                }
                this.PartialReset();
                this.chargeSolde();

                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAddVoucher, statusOperation).Show();
                return this.Direct();
            }

        }

        [HttpPost]
        public ActionResult AddAccountOperation()
        {
            try
            {
                _accountOperationRepository.EcritureManuelleHistoGrandLivre();
                this.Reset();
                this.chargeSolde();
                Session["trnNum"] = null;
                statusOperation = Resources.AlertAddAction;
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAddVoucher, statusOperation).Show();
                return this.Direct();
            }

        }
        [HttpPost]
        public ActionResult RemoveVoucher(string ID)
        {
            try
            {
                Piece pieceToDelete = db.Pieces.Find(Convert.ToInt32(ID));
                db.Pieces.Remove(pieceToDelete);
                db.SaveChanges();
                //_pieceRepository.Delete(pieceToDelete);
                statusOperation = pieceToDelete.Reference + " : " + Resources.AlertDeleteAction;

                this.Reset();
                this.chargeSolde();

                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIAccount, statusOperation).Show();
                return this.Direct();
            }
        }
        [HttpPost]
        public ActionResult ResetAll()
        {
            this.Reset();
            return this.Direct();
        }
        [HttpPost]
        public ActionResult ResetAccount()
        {
            this.PartialReset();
            return this.Direct();
        }
        public ActionResult LoadServerdate(string BranchID)
        {

            List<BusinessDay> listBDUser = (List<BusinessDay>)Session["UserBusDays"];
            if (listBDUser == null)
            {
                listBDUser = _businessDayRepository.GetOpenedBusinessDay(CurrentUser);
            }
            BusinessDay bdDay = listBDUser.FirstOrDefault(b => b.BranchID == Convert.ToInt32(BranchID));

            Session["trnNum"] = _transactNumbeRepository.returnTransactNumber("MANU", bdDay);
            this.GetCmp<DateField>("DateOperation").Value = bdDay.BDDateOperation;
            return this.Direct();
        }
        private void chargeSolde()
        {
            var pieceCpta = db.Pieces.Where(pi => !pi.isAcctOperation).ToList();
            double Totaldebit = pieceCpta.Sum(s => s.Debit);
            double TotalCredit = pieceCpta.Sum(s => s.Credit);
            //double Totaldebit = _pieceRepository.FindAll.Where(pi => !pi.isAcctOperation).Select(s => s.Debit).Sum();
            //double TotalCredit = _pieceRepository.FindAll.Where(pi => !pi.isAcctOperation).Select(s => s.Credit).Sum();
            this.GetCmp<TextField>("TotalDebit").Value = Totaldebit;
            //ViewBag.TotalDebit = Totaldebit;
            this.GetCmp<TextField>("TotalCredit").Value = TotalCredit;
            //ViewBag.TotalCredit = TotalCredit;
            this.GetCmp<TextField>("Solde").Value = TotalCredit - Totaldebit;
            //ViewBag.Solde = TotalCredit - Totaldebit;
            if (TotalCredit - Totaldebit != 0) this.GetCmp<Button>("btnSave").Disabled = true;
        }
        public void Reset()
        {
            this.GetCmp<FormPanel>("FormOpIdentification").Reset(true);
            this.PartialReset();
        }
        public void PartialReset()
        {
            this.GetCmp<ComboBox>("AccountID").Reset();
            this.GetCmp<ComboBox>("AccountSens").Reset();
            this.GetCmp<NumberField>("Amount").Reset();
            this.GetCmp<Store>("Store").Reload();

            this.GetCmp<ComboBox>("AccountID").AllowBlank = true;
            this.GetCmp<ComboBox>("AccountSens").AllowBlank = true;
            this.GetCmp<NumberField>("Amount").AllowBlank = true;
            this.GetCmp<Button>("btnSave").Disabled = false;
        }
        public ActionResult InitializeFields(string ID)
        {
            long id = long.Parse(ID);
            Piece pieceToUpdate = db.Pieces.SingleOrDefault(c => c.PieceID == id);
            this.GetCmp<TextField>("PieceID").Value = pieceToUpdate.PieceID;
            this.GetCmp<ComboBox>("BranchID").Value = pieceToUpdate.BranchID;
            this.GetCmp<ComboBox>("DeviseID").Value = pieceToUpdate.DeviseID;
            this.GetCmp<TextField>("JournalID").Value = pieceToUpdate.Operation.OperationCode;
            this.GetCmp<DateField>("DateOperation").Value = pieceToUpdate.DateOperation;
            this.GetCmp<TextField>("Reference").Value = pieceToUpdate.Reference;
            this.GetCmp<TextField>("Description").Value = pieceToUpdate.Description;
            this.GetCmp<ComboBox>("AccountID").Value = pieceToUpdate.AccountID;

            if (pieceToUpdate.Debit > 0)
            {
                this.GetCmp<ComboBox>("AccountSens").Value = "DB";
                this.GetCmp<NumberField>("Amount").Value = pieceToUpdate.Debit;
            }
            else
            {
                this.GetCmp<ComboBox>("AccountSens").Value = "CR";
                this.GetCmp<NumberField>("Amount").Value = pieceToUpdate.Credit;
            }
            this.chargeSolde();
            return this.Direct();
        }
    }
}