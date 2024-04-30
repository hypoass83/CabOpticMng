using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using System.Web.UI;
using FatSod.DataContext.Concrete;
using ExtPartialViewResult = Ext.Net.MVC.PartialViewResult;
using System.Collections.Generic;

namespace FatSodDental.UI.Areas.Sale.Controllers
{
    [Authorize]
    public class PaymentModeController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/PaymentMode";
        private const string VIEW_NAME = "Index";
        //person repository
        private FatSod.Supply.Abstracts.IRepositorySupply<SavingAccount> _paymentModeRepository;
        
        //Construcitor
        public PaymentModeController(FatSod.Supply.Abstracts.IRepositorySupply<SavingAccount> paymentModeRepository)
        {
            this._paymentModeRepository = paymentModeRepository;
            
        }
        
        // GET: Sale/PaymentMode
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;
            //Session["UserProfile"] = CurrentUser().ProfileID;
            //Session["UserID"] = CurrentUser().PersonID;
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Sale.SalePaymentMode.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            //this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            //{
            //    Url = Url.Action("Index"),
            //    DisableCaching = false,
            //    Mode = LoadMode.Frame
            //});

            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelPayment
            //};
            //return rPVResult;
            return View(ModelPayment);
        }
        private List<object> ModelPayment
        {
            get
            {
                List<object> model = new List<object>();

                foreach (SavingAccount c in db.SavingAccounts.ToList())
                {
                    model.Add(
                        new
                        {
                            BranchID = c.BranchID,
                            PaymentModeCode = c.Code,
                            CustomerID = c.CustomerID,
                            PaymentModeDescription = c.Description,
                            PaymentModeID = c.ID,
                            PaymentModeLabel = c.Name
                        }
                    );
                }
                //);
                return model;
            }
        }
        //Add Method
        [HttpPost]
        public ActionResult Add(SavingAccount paymentMode)
        {
            if (paymentMode.ID > 0)
            {
                SavingAccount paymentModeToUpdate = db.SavingAccounts.FirstOrDefault(p => p.ID == paymentMode.ID);
                paymentModeToUpdate.Name = paymentMode.Name;
                paymentModeToUpdate.Code = paymentMode.Code;
                paymentModeToUpdate.Description = paymentMode.Description;
                _paymentModeRepository.Update(paymentModeToUpdate);
                TempData["status"] = "Les modifications ont été bien prises en compte";
            }
            else
            {
                _paymentModeRepository.Create(paymentMode);
                TempData["status"] = "Nouveau mode de payment  " + paymentMode.Name + " crée avec succès";
            }
            TempData["alertType"] = "alert alert-success";
            return RedirectToAction("Index");
        }
        //Delete Action
        [HttpPost]
        public ActionResult Delete(int ID)
        {
            SavingAccount paymentModeToDelete = _paymentModeRepository.FindAll.FirstOrDefault(p => p.ID == ID);
            _paymentModeRepository.Delete(paymentModeToDelete);
            TempData["alertType"] = "alert alert-warning";
            TempData["status"] = "Opération de suppression du mode de paiement" + paymentModeToDelete.Name + " éffectuée avec succès";
            return RedirectToAction("Index");
        }
        //Initialize field for update
        public ActionResult InitializeFields(int ID)
        {
            SavingAccount paymentModeToUpdate = db.SavingAccounts.FirstOrDefault(p => p.ID == ID);
            this.GetCmp<FormPanel>("PaymentModeForm").Reset(true);
            this.GetCmp<TextField>("ID").Value = paymentModeToUpdate.ID;
            this.GetCmp<TextField>("Code").Value = paymentModeToUpdate.Code;
            this.GetCmp<TextField>("Name").Value = paymentModeToUpdate.Name;
            this.GetCmp<TextField>("Description").Value = paymentModeToUpdate.Description;
            return this.Direct();
        }
    }
}