using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Ressources;
using System;

namespace CABOPMANAGEMENT.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    //[TakeBusinessDay(Order = 2)]
    public class CloseBDController : BaseController
    {
       

        private IBusinessDay _busDayRepo;

        //Construcitor
        public CloseBDController(
            IBusinessDay busDayRepo
            )
        {
            this._busDayRepo = busDayRepo;   
        }
        // GET: Sale/Sale Return all a sales historique add enable to create a new sale
        
        public ActionResult CloseBD()
        {
           
            if (_busDayRepo.GetOpenedBusinessDay() == null || _busDayRepo.GetOpenedBusinessDay().Count == 0)
            {
                TempData["Message"] = "Good News - All Business Days Have Been Closed";
                return View(GetAllBusDayOpen());
            }

            return View(GetAllBusDayOpen());
        }

       
        public JsonResult BtnCloseBusinessDay(int BusinessDayID)
        {
            bool status = false;
            string Message = "";
            try
            {
                if (BusinessDayID > 0)
            {
                BusinessDay oldBusDay = db.BusinessDays.Find(BusinessDayID);
                
                //fermeture du bs day
                _busDayRepo.CloseBusinessDay(oldBusDay.Branch,oldBusDay.BDDateOperation,SessionGlobalPersonID);
                
                List<BusinessDay> userBDList = _busDayRepo.GetOpenedBusinessDay(this.CurrentUser);
                Session["UserBusDays"] = userBDList;
                //au moins un business day de l'utilisateur est ouverte et il peut commencer à travailler
                if (_busDayRepo.GetOpenedBusinessDay(CurrentUser) != null && _busDayRepo.GetOpenedBusinessDay(CurrentUser).Count > 0)
                {
                    status = true;
                    Message = "Closed business day Ok";
                    
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

                status = true;
                Message = "";
            }
        }
            catch (Exception e)
            {
                status = false;

                Message = e.Message;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            return new JsonResult { Data = new { status = status, Message = Message,  redirectUrl = Url.Action("Index", "Home", new { area = "" }) } };
        }
        
       
        public JsonResult InitializeCloseBDFields(int ID)
        {
            List<object> list = new List<object>();
            if (ID > 0)
            {
                BusinessDay businDay = new BusinessDay();
                businDay = db.BusinessDays.Find(ID);
                list.Add(
                    new
                    {
                        BusinessDayID = businDay.BusinessDayID,
                        BranchID = businDay.BranchID,
                        BranchName = businDay.BranchName,
                        BDDateOperation = businDay.BDDateOperation.ToString("yyyy-MM-dd")
                    }
                    );
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult OpenedBusday()
        {
            List<object> list = new List<object>();

            foreach (BusinessDay busDay in _busDayRepo.GetOpenedBusinessDay())
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

        
        public List<BusinessDay> GetAllBusDayOpen()
        {
            List<BusinessDay> list = new List<BusinessDay>();
            List<BusinessDay> busDays = _busDayRepo.GetOpenedBusinessDay();

            foreach (BusinessDay busDay in busDays)
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
            if (busDays == null || busDays.Count==0)
            {
                Session["UserBusDays"] = null;
            }
            return list;
        }


    }
}