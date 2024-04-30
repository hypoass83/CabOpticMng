using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using FastSod.Utilities.Util;

namespace CABOPMANAGEMENT.Areas.CashRegister.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class CashRegisterController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/CashRegister";
        private const string VIEW_NAME_OPEN = "Open";
        private const string VIEW_NAME_CLOSE = "Close";

        private ITillDay _tillDayRepository;

        private IDeposit _depositRepository;
        IMouchar _opSneak;

        public CashRegisterController(
            ITillDay tillDayRepository,
            IDeposit depositRepository,
            IMouchar opSneak
            )
        {
            this._tillDayRepository = tillDayRepository;
            this._depositRepository = depositRepository;
            this._opSneak = opSneak;
        }
        //this enable to open till to allow sales operations
        
        
        public ActionResult Open()
        {
            ViewBag.DisplayForm = 1;
            try
            {
                //vefifion si le user est un caissier
                UserTill userTill = (from td in db.UserTills
                                     where td.UserID == SessionGlobalPersonID && td.HasAccess
                                     select td).SingleOrDefault();

                //if userTill then he not has access for cash register
                if (userTill == null)
                {
                    TempData["Message"] = "Access denied - You don't have access or cash register don't exist. Please contact our administrator";
                    ViewBag.DisplayForm = 0;
                    return this.View(GetTillDayList());
                }
                
                List<BusinessDay> businessDay = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = businessDay.FirstOrDefault().BDDateOperation;
                //we test if business is opened
                if (businessDay == null || businessDay.Count<=0) 
                {
                    TempData["Message"] = "Error Business - Business day is closed";
                    ViewBag.DisplayForm = 0;
                    return this.View(GetTillDayList());
                }
                ViewBag.BusnessDayDate = currentDateOp.ToString("yyyy-MM-dd");
                
                ViewBag.TillID = userTill.TillID;

                TillDayStatus tState = _tillDayRepository.TillDayStatus(userTill.TillID);
                if (tState == null)
                {
                    TempData["Message"] = "Error- Please call the administrator";
                    ViewBag.DisplayForm = 0;
                    return this.View(GetTillDayList());
                }
                if (tState.IsOpen)
                {
                    TempData["Message"] = "Error- This Cash Register is Still Open!!! Please Close It Before Proceed";
                    ViewBag.DisplayForm = 0;
                    return this.View(GetTillDayList());
                }
                TillDay yesterDayTillDay = _tillDayRepository.FindAll.SingleOrDefault(td => td.TillID == userTill.TillID && td.TillDayDate.Date == tState.TillDayLastClosingDate.Date); 
                if (yesterDayTillDay != null)
                {
                    ViewBag.CashInitialization = 0;
                    ViewBag.YesterdayClosingPrice = yesterDayTillDay.TillDayCashHand;//.TillDayClosingPrice;
                }
                else
                {
                    ViewBag.YesterdayClosingPrice = 0;
                    ViewBag.CashInitialization = 1;
                }
                Session["TillName"] = userTill.Till.Name;
                Session["TillID"] = userTill.Till.ID;

                return View(GetTillDayList());
            }
            catch (Exception e) 
            {
                TempData["Message"] = "Error - An error occure when we try to give response : " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View(GetTillDayList());
            }

            
        }
        //this enable to Close till to allow sales operations
        
        public ActionResult Close()
        {
            ViewBag.DisplayForm = 1;
            try
            {
                ViewBag.YesterdayTillDayClosingPrice = 0;
                TillSatut tillStatus = new TillSatut();
                ViewBag.TillSatut = tillStatus;
                TillDay currentTillDay = new TillDay();
                ViewBag.tillDay = currentTillDay;

                //vefifion si le user est un caissier
                UserTill userTill = (from td in db.UserTills
                                     where td.UserID == SessionGlobalPersonID && td.HasAccess
                                     select td).FirstOrDefault();
            
                if (userTill == null)
                {
                    TempData["Message"] = "Access denied - You don't have access or cash register don't exist. Please contact our administrator";
                    ViewBag.DisplayForm = 0;
                    return this.View();
                }
                Session["UserTill"] = userTill.TillID;

                List<BusinessDay> businessDay = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = businessDay.FirstOrDefault().BDDateOperation;

                ViewBag.BusnessDayDate = currentDateOp.ToString("yyyy-MM-dd");
                Session["BusnessDayDate"] = currentDateOp;
                currentTillDay = _tillDayRepository.FindAll.SingleOrDefault(t => t.TillID == userTill.TillID && (t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year) && t.IsOpen);
              
                if (currentTillDay == null)
                {
                    TillDay tillDay = new TillDay()
                    {
                        IsOpen = true,
                        TillDayClosingPrice=0,
                        TillDayDate= currentDateOp,
                        TillDayOpenPrice=0,
                        TillID= userTill.TillID
                    };
                    //pour une raison kelkonque je open encore
                    bool res = _tillDayRepository.OpenDay(tillDay, 0, 0, SessionGlobalPersonID, SessionBusinessDay(null).BDDateOperation, SessionBusinessDay(null).BranchID);
                    if (!res)
                    {
                        TempData["Message"] = "Error - You don't have beforehand opened cash register";
                        ViewBag.DisplayForm = 0;
                        return this.View();
                    }
                    currentTillDay = _tillDayRepository.FindAll.SingleOrDefault(t => t.TillID == userTill.TillID && (t.TillDayDate.Day == currentDateOp.Day && t.TillDayDate.Month == currentDateOp.Month && t.TillDayDate.Year == currentDateOp.Year) && t.IsOpen);

                    if (currentTillDay == null)
                    {
                        TempData["Message"] = "Error - You don't have beforehand opened cash register";
                        ViewBag.DisplayForm = 0;
                        return this.View();
                    }
                }
            
                ViewBag.YesterdayTillDayClosingPrice = currentTillDay.TillDayOpenPrice;

                //======================================== test
                tillStatus = _tillDayRepository.TillStatus(currentTillDay.TillID);
                //======================================= end test
                ViewBag.TillSatut = tillStatus;
            
                ViewBag.tillDay = currentTillDay;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Close teller:"+ ex.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }
        //** This actions allow to persiste information of initialize till at one date
        //[HttpPost]
        public JsonResult CloseDay(TillDay tillDay, double InputCash, double OutputCash, double YesterdayTillDayClosingPrice)
        {
            bool status = false;
            string redirectUrl = "";
            try
            {

                Till till = db.Tills.Find(tillDay.TillID);
                tillDay.Till = till;
                bool res = _tillDayRepository.CloseDay(tillDay, InputCash, OutputCash, YesterdayTillDayClosingPrice, SessionGlobalPersonID, SessionBusinessDay(null).BDDateOperation, SessionBusinessDay(null).BranchID);
                if (!res)
                {
                    status = false;
                    statusOperation = "Error - Contact Administrator ";
                    return new JsonResult { Data = new { status = status, Message = statusOperation, redirectUrl = redirectUrl } };
                }
                status = true;
                statusOperation = Resources.MsgCloseTeller;
                Session["UserBusDays"] = null;
                redirectUrl = Url.Action("Login", "Session", new { area = "" });
            }
            catch (Exception e) {
                status = false;
                statusOperation = e.Message + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation, redirectUrl = redirectUrl } };
        }
        //[HttpPost]
        public JsonResult OpenDay(TillDay tillDay, double YesterdayClosingPrice = 0, double CashInitialization = 0)
        {
            bool status = false;
            string redirectUrl = "";
            try
            {
                Till till = db.Tills.Find(tillDay.TillID);
                tillDay.Till = till;
                bool res = _tillDayRepository.OpenDay(tillDay, YesterdayClosingPrice, CashInitialization, SessionGlobalPersonID, SessionBusinessDay(null).BDDateOperation, SessionBusinessDay(null).BranchID);
                if (!res)
                {
                    status = false;
                    statusOperation = "Error - Contact Administrator " ;
                    return new JsonResult { Data = new { status = status, Message = statusOperation, redirectUrl = redirectUrl } };
                }
                status = true;
                statusOperation = Resources.MsgOpenTeller;
                redirectUrl = Url.Action("Index", "Home", new { area = "" });
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = e.Message + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation, redirectUrl = redirectUrl } };
        }

        ////Return list of tillDay
        public List<TillDay> GetTillDayList()
        {
            List<TillDay> model = new List<TillDay>();

            int? tillID = (Session["TillID"]==null) ?0 : (int)Session["TillID"];
            if (tillID.Value>0 || tillID!=null)
            {
                List<TillDay> lstTillDay = (from td in db.TillDays
                                            where td.TillID == tillID
                                            select td)
                                        .OrderByDescending(t => t.TillDayDate)
                                        .Take(8)
                                        .ToList();
                lstTillDay.ForEach(td =>
                {
                    model.Add(
                            new TillDay
                            {
                                TillDayID = td.TillDayID,
                                Till = td.Till,
                                TillDayOpenPrice = td.TillDayOpenPrice,
                                TillDayDate = td.TillDayDate,
                                TillDayClosingPrice = td.TillDayCashHand,
                                IsOpen = td.IsOpen
                            }
                        );
                });
            }
            
            return model;
        }
       

        //This method return a list of tills of one branch
        public JsonResult GetTillOfBanch(int BranchID)
        {
            int i = 0;
            List<Till> model = new List<Till>();
            List<UserTill> userTill = db.UserTills.Where(td => td.HasAccess && td.USer.IsConnected && td.USer.UserAccountState).ToList();
            foreach (UserTill ustill in userTill)
            {
                db.Tills.Where(t => t.BranchID == BranchID && t.ID== ustill.TillID).ToList().ForEach(t =>
                {
                    i++;
                    model.Add(new Till { Name = t.Name, ID = t.ID });
                }
              );
            }
            //else
            //{
            //    i++;
            //    model.Add(new Till { Name = userTill.Till.Name, ID = userTill.Till.ID });
            //}

            /*
            if (i == 0)
            {
                this.GetCmp<ComboBox>("TillID").Disabled = true;
                return this.Direct();
            }
            this.GetCmp<ComboBox>("TillID").Disabled = false;
            return this.Store(model);*/
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}