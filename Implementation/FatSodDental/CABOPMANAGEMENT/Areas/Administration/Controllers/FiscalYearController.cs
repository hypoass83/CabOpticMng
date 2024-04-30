using FatSod.Security.Abstracts;
using CABOPMANAGEMENT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;
using FatSod.Budget.Entities;

namespace CABOPMANAGEMENT.Areas.Administration.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class FiscalYearController : BaseController
    {
        private IRepository<FiscalYear> _fiscalYearRepo;
        
        public FiscalYearController(IRepository<FiscalYear> fiscalYearRepo)
        {
            this._fiscalYearRepo = fiscalYearRepo;
            
        }
        // GET: Administration/FiscalYear
        
        public ActionResult Index()
        {
           
            return View(ModelFiscalYear);
        }

        //[HttpPost]
        public JsonResult AddFiscalYear(FiscalYear fiscalYear, int isOpen)
        {
            bool status = false;
            try
            {
                fiscalYear.FiscalYearStatus = Convert.ToBoolean(isOpen);
                if (fiscalYear.FiscalYearID > 0)
                {
                    statusOperation = Resources.AlertUpdateAction;
                    _fiscalYearRepo.Update(fiscalYear, fiscalYear.FiscalYearID);
                }
                else
                {
                    statusOperation = Resources.AlertAddAction;
                    _fiscalYearRepo.Create(fiscalYear);
                }
                status = true;
            }
            catch (Exception e)
            {
                status = false;
                statusOperation = e.Message + " " + e.InnerException;
            }
            
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        
        //[HttpPost]
        public JsonResult DeleteFiscalYear(int ID)
        {
            bool status = false;
            try
            {
                db.FiscalYears.Remove(db.FiscalYears.Find(ID));
                db.SaveChanges();
                status = true;
                statusOperation = Resources.AlertDeleteAction;
                //_fiscalYearRepo.Delete(db.FiscalYears.Find(ID));
            }
            catch (Exception e)
            {
                status = false;
                statusOperation ="Can't Delete It "+e.Message + " " + e.InnerException;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }
        //[HttpPost]
        public JsonResult InitializeFieldsFiscalYear(int ID)
        {
            FiscalYear fiscal =  db.FiscalYears.Find(ID);
            List<object> _fiscalList = new List<object>();
            _fiscalList.Add(new
            {
                FiscalYearID = fiscal.FiscalYearID,
                FiscalYearNumber = fiscal.FiscalYearNumber,
                FiscalYearStatus = Convert.ToInt16(fiscal.FiscalYearStatus),
                FiscalYearLabel = fiscal.FiscalYearLabel,
                StartFrom = fiscal.StartFrom.ToString("yyyy-MM-dd"),
                EndFrom = fiscal.EndFrom.ToString("yyyy-MM-dd")
            });
            return Json(_fiscalList, JsonRequestBehavior.AllowGet);
        }

        private List<FiscalYear> ModelFiscalYear
        {
            get
            {
                
                List<FiscalYear> model = new List<FiscalYear>();
                
                 db.FiscalYears.ToList().ForEach(f =>
                {
                    model.Add(
                            new FiscalYear
                            {
                                FiscalYearID = f.FiscalYearID,
                                FiscalYearNumber = f.FiscalYearNumber,
                                FiscalYearStatus = f.FiscalYearStatus,
                                FiscalYearLabel = f.FiscalYearLabel,
                                StartFrom = f.StartFrom,
                                EndFrom = f.EndFrom
                            }                       
                        );
                });
                return model;
            }
        }
    }
}