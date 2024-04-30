using FatSod.Ressources;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.CashRegister
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class ForceOpenTellerController : BaseController
    {
        private ITillDay _tillDayRepository;

        public ForceOpenTellerController(
            ITillDay tillDayRepository
            )
        {
            this._tillDayRepository = tillDayRepository;
        }
        // GET: CashRegister/ForceCloseTeller
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return View(ModelTeller);
        }

        /// <summary>
        /// Return a Model that use in view
        /// </summary>
        private List<TillDayStatus> ModelTeller
        {
            get
            {
                List<TillDayStatus> model = new List<TillDayStatus>();
                db.TillDayStatus.Where(t => !t.IsOpen).ToList().ForEach(u =>
                {
                    model.Add(
                            new TillDayStatus
                            {
                                TillID = u.TillID,
                                Till = u.Till,
                                TillDayLastClosingDate = u.TillDayLastClosingDate,
                                TillDayLastOpenDate = u.TillDayLastOpenDate,
                                TillDayStatusID = u.TillDayStatusID,
                            }
                        );
                });
                return model;
            }
        }

        public JsonResult Edit(int id)
        {
            TillDay tillDayEntity = db.TillDays.ToList().OrderBy(t => t.TillDayDate).LastOrDefault(t => t.TillID == id);
            //returns the Json result of _BeneficiariesList
            List<object> _TillList = new List<object>();
            _TillList.Add(new
            {
                TillID = tillDayEntity.TillID,
                TellerCode = tillDayEntity.Till.Code,
                TellerName = tillDayEntity.Till.Description,
                OpeningAmount = tillDayEntity.TillDayOpenPrice,
                ClosingAmount = tillDayEntity.TillDayCashHand
            });
            return Json(_TillList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OpenCashreg(int TillID)
        {
            bool status = false;
            string Message = "";
            try
            {
                TillDay tillDayEntity = db.TillDays.ToList().OrderBy(t => t.TillDayDate).LastOrDefault(t => t.TillID == TillID);
                BusinessDay oldBusDay = SessionBusinessDay(null);
                _tillDayRepository.ForceOpenDay(tillDayEntity, tillDayEntity.TillDayCashHand, tillDayEntity.TillDayOpenPrice, SessionGlobalPersonID, oldBusDay.BDDateOperation, oldBusDay.BranchID);

                status = true;
                Message = Resources.ForceOpenTeller + " OK";
            }
            catch (Exception e)
            {
                status = false;

                Message = e.Message;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            return new JsonResult { Data = new { status = status, Message = Message } }; //,  redirectUrl = Url.Action("Index", "Home", new { area = "" }) } };
        }
    }
}