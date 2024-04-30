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

namespace FatSodDental.UI.Areas.CashRegister.Controllers
{
	[Authorize(Order = 1)]
	[TakeBusinessDay(Order = 2)]
	public class TillAdjustController : BaseController
	{
		//Current Controller and current page
		private const string CONTROLLER_NAME = "CashRegister/TillAdjust";
		private const string VIEW_NAME_OPEN = "Index";

		private ITillDay _tillDayRepository;
		private IBusinessDay _busDayRepo;
		private ITillAdjust _TillAdjust;
        private IRepositorySupply<UserTill> _userTillRepository;

        private List<BusinessDay> lstBusDay;

		public TillAdjustController(
				 ITillDay tillDayRepository,
				 IBusinessDay busDayRepo,
				ITillAdjust TillAdjust,
            IRepositorySupply<UserTill> userTillRepository
				)
		{
			this._tillDayRepository = tillDayRepository;
			this._busDayRepo = busDayRepo;
			this._TillAdjust = TillAdjust;
            this._userTillRepository = userTillRepository;
		}
		//
		// GET: /CashRegister/TillAdjust/
        [OutputCache(Duration = 3600)] 
		public ActionResult Index()
		{
            try
            {

            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    WrapByScriptTag = false
            //};
			//We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.CashRegister.CODETILLADJUST, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            return View();
            }

            catch (Exception e)
            {
                X.Msg.Alert("Error ", e.Message).Show();
                return this.Direct();
            }
		}
		public ActionResult LoadTillAdjustInfo(int TillID, int DeviseID)
		{
			try
			{
                UserTill userTill = _userTillRepository.FindAll.LastOrDefault(td => td.TillID == TillID && td.HasAccess);
				//if userTill then he not has access for cash register
				if (userTill == null)
				{
					X.Msg.Alert("Access denied", "Sorry Please choose a Teller").Show();
					return this.Direct();
				}
				Till till = db.Tills.FirstOrDefault(t => t.ID == userTill.TillID);

                lstBusDay = (List<BusinessDay>)Session["UserBusDays"];
                if (lstBusDay == null)
                {
                    lstBusDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                };
                BusinessDay businessDay = lstBusDay.FirstOrDefault();

				this.GetCmp<DateField>("TillAdjustDate").Value = businessDay.BDDateOperation;

				double tillBalance = _tillDayRepository.TillStatus(till).Ballance;
				this.GetCmp<NumberField>("ComputerPrice").Value = tillBalance;

				return this.Direct();
			}
			catch (Exception e)
			{
				X.Msg.Alert("Error", "An error occure when we try to give response : " + e.Message).Show();
				return this.Direct();
			}

		}
		public ActionResult AddTillAdjust(TillAdjust tillAdjust)
		{
			try
			{
				bool res=_TillAdjust.SaveTillAdjust(tillAdjust,SessionGlobalPersonID,SessionBusinessDay(null).BranchID);
				if (res)
				{
					statusOperation = tillAdjust.Justification + "-" + Resources.AlertUpdateTillAdjust;
					this.AlertSucces(Resources.Success, statusOperation);
				}
				else
				{
					statusOperation = tillAdjust.Justification + "-" + Resources.AlertErrorTillAdjust;
					this.AlertError(Resources.er_alert_danger, statusOperation);
				}
				this.GetCmp<FormPanel>("TillAdjust").Reset();
				return this.Direct();
			}
			catch (Exception e)
			{
				statusOperation = Resources.er_alert_danger + e.Message;
				X.Msg.Alert(Resources.TillAdjust, statusOperation).Show();
				return this.Direct();
			}
		}
	}
}