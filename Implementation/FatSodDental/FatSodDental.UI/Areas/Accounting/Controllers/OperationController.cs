using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using Ext.Net;
using Ext.Net.MVC;
using FatSodDental.UI.Tools;
using FatSodDental.UI.Controllers;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSodDental.UI.Filters;
using System.Web.UI;
using FatSod.DataContext.Concrete;

namespace FatSodDental.UI.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class OperationController : BaseController
    {
        private IRepositorySupply<Operation> _OperationRepository;
        //
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/Operation";
        private const string VIEW_NAME = "Index";
        

         public OperationController(IRepositorySupply<Operation> OperationRepository)
        {
            this._OperationRepository = OperationRepository;
            
            
        }
       
        //private void SessionParameters()
        //{
        //    //Session["UserProfile"] = SessionProfileID;
        //    //Session["UserID"] = SessionGlobalPersonID;
        //    //Session["Curent_Controller"] = CONTROLLER_NAME;
        //    //Session["Curent_Page"] = VIEW_NAME;
        //}

        //
        // GET: /Accounting/Operation/
        //[OutputCache(Duration = 3600)] 
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
            
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Accounting.Operation.CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            return View(Model());
        }
        public List<object> Model()
        {
            List<object> list = new List<object>();
            db.Operations.ToList().ForEach(c =>
            {
                list.Add(
                    new
                    {
                        OperationID = c.OperationID,
                        OperationCode = c.OperationCode,
                        OperationLabel = c.OperationLabel,
                        UIoperationTypeCode = c.UIoperationTypeCode,
                        UIMacroOperationCode = c.UIMacroOperationCode,
                        UIReglementTypeCode = c.UIReglementTypeCode,
                        OperationDescription = c.OperationDescription,
                        MacroOperationID = c.MacroOperationID,
                        OperationTypeID = c.OperationTypeID,
                        ReglementTypeID = c.ReglementTypeID
                    }
                );
            });
            return list;
        }
        //add Accounting Task
        [HttpPost]
        public ActionResult AddOperation(Operation operation)
        {
            try
            {
                if (operation.OperationID > 0)
                {

                    Operation OperationToUpdate = db.Operations.SingleOrDefault(c => c.OperationID == operation.OperationID);
                    operation.OperationCode = OperationToUpdate.OperationCode;
                    operation.ReglementTypeID = OperationToUpdate.ReglementTypeID;
                    operation.MacroOperationID = OperationToUpdate.MacroOperationID;
                    operation.OperationTypeID = OperationToUpdate.OperationTypeID;
                    _OperationRepository.Update(operation, operation.OperationID);
                    statusOperation = operation.OperationLabel + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    //if (!LoadAction.IsMenuActionAble(MenuAction.ADD, SessionProfileID, CodeValue.Accounting.AccountOperation.CODE))
                    if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Accounting.AccountOperation.CODE,db))
                    {
                        _OperationRepository.Create(operation);
                        statusOperation = operation.OperationLabel + " : " + Resources.AlertAddAction;
                    }
                    else
                    {
                        statusOperation = operation.OperationLabel + " : " +  Resources.msgCreateOperation;
                        X.Msg.Alert(Resources.UIOperation, statusOperation).Show();
                        return this.Direct();
                    }
                    
                }
                this.GetCmp<FormPanel>("Operation").Reset();
                this.GetCmp<Store>("Store").Reload();
                this.AlertSucces(Resources.Success, statusOperation);
                return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIOperation, statusOperation).Show();
                return this.Direct();
            }
        }
        [HttpPost]
        public StoreResult GetList()
        {
            List<object> list = new List<object>();
            db.Operations.ToList().ForEach(c =>
            {
                list.Add(
                    new
                    {
                        OperationID = c.OperationID,
                        OperationCode =  c.OperationCode,
                        OperationLabel = c.OperationLabel,
                        UIoperationTypeCode = c.UIoperationTypeCode,
                        UIMacroOperationCode = c.UIMacroOperationCode,
                        UIReglementTypeCode = c.UIReglementTypeCode,
                        OperationDescription = c.OperationDescription,
                        MacroOperationID = c.MacroOperationID,
                        OperationTypeID = c.OperationTypeID,
                        ReglementTypeID = c.ReglementTypeID
                    }
                );
            });
            return this.Store(list);
        }
        //Deletes actions
        [HttpPost]
        public ActionResult DeleteOperation(string ID)
        {
            try { 
            int id = Convert.ToInt32(ID);
            Operation OperationToDelete = db.Operations.Find(id);
            db.Operations.Remove(OperationToDelete);
            db.SaveChanges();
            //_OperationRepository.Delete(OperationToDelete);
            statusOperation = OperationToDelete.OperationLabel + " : " + Resources.AlertDeleteAction;
            this.GetCmp<FormPanel>("Operation").Reset();
            this.GetCmp<Store>("Store").Reload();
            this.AlertSucces(Resources.Success, statusOperation);
            return this.Direct();
            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                X.Msg.Alert(Resources.UIOperation, statusOperation).Show();
                return this.Direct();
            }
        }
        //click pr update
        public ActionResult ClickUpdateOperation(string ID)
        {
            int id = Convert.ToInt32(ID);
            Operation OperationToUpdate = db.Operations.SingleOrDefault(c => c.OperationID == id);
            this.GetCmp<FormPanel>("Operation").Reset(true);
            this.GetCmp<ComboBox>("OperationCode").Disable(true);
            this.GetCmp<ComboBox>("OperationTypeID").Disable(true);
            this.GetCmp<ComboBox>("MacroOperationID").Disable(true);
            this.GetCmp<ComboBox>("ReglementTypeID").Disable(true);
            this.GetCmp<TextField>("OperationID").Value = OperationToUpdate.OperationID;
            
            this.GetCmp<TextField>("OperationCode").Value = OperationToUpdate.OperationCode;
            this.GetCmp<TextField>("OperationLabel").Value = OperationToUpdate.OperationLabel;
            this.GetCmp<TextField>("OperationDescription").Value = OperationToUpdate.OperationDescription;
            this.GetCmp<ComboBox>("OperationTypeID").Value= OperationToUpdate.OperationTypeID;
            this.GetCmp<ComboBox>("MacroOperationID").Value = OperationToUpdate.MacroOperationID;
            this.GetCmp<ComboBox>("ReglementTypeID").Value = OperationToUpdate.ReglementTypeID;
            
            return this.Direct();
        }
	}
}