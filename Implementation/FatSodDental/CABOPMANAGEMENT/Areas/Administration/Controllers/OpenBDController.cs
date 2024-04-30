using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using CABOPMANAGEMENT.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;
using System;

namespace CABOPMANAGEMENT.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
   // [TakeBusinessDay(Order = 2)]
    public class OpenBDController : BaseController
    {
       
        private IBusinessDay _busDayRepo;

        //Construcitor
        public OpenBDController(IBusinessDay busDayRepo)
        {
            this._busDayRepo = busDayRepo; 
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        public ActionResult OpenBD()
        {
            if (_busDayRepo.GetClosedBusinessDay() == null || _busDayRepo.GetClosedBusinessDay().Count == 0)
            {
                TempData["Message"] = "Good News - All Business Days are Opened";
                return View(GetAllCloseBusinessDay());
            }
            
            return View(GetAllCloseBusinessDay());
        }

       // [HttpPost]
        public JsonResult OpenBusinessDay(BusinessDay OpenBD)
        {

            bool status = false;
            
            string Message = "";
            if (OpenBD.BusinessDayID > 0)
            {
                _busDayRepo.OpenBusinessDay(db.Branches.Find(OpenBD.BranchID), OpenBD.BDDateOperation,SessionGlobalPersonID);

                if (_busDayRepo.GetClosedBusinessDay() == null || _busDayRepo.GetClosedBusinessDay().Count == 0)
                {
                    //return RedirectToAction("Index", "Main", new { aria = "" });
                    //this.Reset();
                    List<BusinessDay> userBDList = _busDayRepo.GetOpenedBusinessDay(this.CurrentUser);
                    Session["UserBusDays"] = userBDList;
                    status = true;
                    Message = "";
                    return new JsonResult { Data = new { status = status, Message = Message,  redirectUrl = Url.Action("Index", "Home", new { area = "" }) } };
                }

                //au moins un business day de l'utilisateur est ouverte et il peut commencer à travailler
                /*if (_busDayRepo.GetOpenedBusinessDay(CurrentUser) != null && _busDayRepo.GetOpenedBusinessDay(CurrentUser).Count > 0)
                {*/
                    status = true;
                    Message = "Open business day Ok";
                    
               // }
               
            }

            return new JsonResult { Data = new { status = status,  Message = Message } };
        }

       

        public JsonResult InitializeOpenBDFields(int ID)
        {
            List<object> list = new List<object>();
            if (ID > 0)
            {

                BusinessDay busDay = new BusinessDay();
                busDay = db.BusinessDays.Find(ID);
                if (busDay == null)
                {
                    int branchid = (Session["DefaultBranchID"] == null) ? 0 : (int)Session["DefaultBranchID"];
                    if (branchid <= 0)
                    {
                        InjectUserConfigInSession();
                        branchid = (Session["DefaultBranchID"] == null) ? 0 : (int)Session["DefaultBranchID"];
                    }

                    Branch newBr = db.Branches.Find(branchid);
                    list.Add(
                    new
                    {
                        BusinessDayID = 1,
                        BranchID = newBr.BranchID,
                        BranchName = newBr.BranchName,
                        BDDateOperation = DateTime.Today.ToString("yyyy-MM-dd")
                    });
                }
                else
                {
                    list.Add(
                    new
                    {
                        BusinessDayID = busDay.BusinessDayID,
                        BranchID = busDay.BranchID,
                        BranchName = busDay.BranchName,
                        BDDateOperation = busDay.BDDateOperation.ToString("yyyy-MM-dd")
                    });
                }


            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OpenedBusday()
        {
            List<object> list = new List<object>();

            foreach (BusinessDay busDay in _busDayRepo.GetClosedBusinessDay())
            {
                list.Add(
                    new
                    {
                        ID = busDay.BranchID,
                        Name = busDay.BranchName
                    }
                    );
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }

       
        public List<BusinessDay> GetAllCloseBusinessDay()
        {

            List<BusinessDay> list = new List<BusinessDay>();
            List<BusinessDay> closedList = _busDayRepo.GetClosedBusinessDay();
            if (closedList == null || closedList.Count == 0)
            {
                int branchid = (Session["DefaultBranchID"] == null) ? 0 : (int)Session["DefaultBranchID"];
                if (branchid <= 0)
                {
                    InjectUserConfigInSession();
                    branchid = (Session["DefaultBranchID"] == null) ? 0 : (int)Session["DefaultBranchID"];
                }

                Branch newBr = db.Branches.Find(branchid);
                list.Add(
                        new BusinessDay
                        {
                            BranchID = newBr.BranchID,
                            BusinessDayID = 1,
                            BDDateOperation = DateTime.Today,
                            ClosingDayStarted = false,
                            BDStatut = false,
                            Branch = newBr
                        });
            }
            else
            {
                foreach (BusinessDay busDay in closedList)
                {
                    list.Add(
                        new BusinessDay
                        {
                            BranchID = busDay.BranchID,
                            BusinessDayID = busDay.BusinessDayID,
                            BDDateOperation = busDay.BDDateOperation,
                            ClosingDayStarted = busDay.ClosingDayStarted,
                            BDStatut = busDay.BDStatut,
                            Branch = busDay.Branch
                        }
                        );
                }
            }


            return list;

        }

        public ActionResult ChangeBusDay(int? BranchID)
        {
            /*this.GetCmp<DateField>("OpenBDDate").Reset();
            if (BranchID > 0)
            {
                BusinessDay businessDay = _busDayRepo.GetOpenedBusinessDay(db.Branches.Find(BranchID.Value));
                this.GetCmp<DateField>("OpenBDDate").SetValue(businessDay.BDDateOperation);
            }*/
            return this.View();
        }

    }
}