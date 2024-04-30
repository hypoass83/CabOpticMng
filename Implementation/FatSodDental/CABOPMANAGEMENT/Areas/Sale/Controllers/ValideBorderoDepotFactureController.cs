using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.DataContext.Repositories;
using CABOPMANAGEMENT.Areas.Sale.Models;
using FatSod.Report.WrapReports;

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class ValideBorderoDepotFactureController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "CashRegister/ValideBorderoDepotFacture";
        private const string VIEW_NAME = "Index";


        private IBusinessDay _busDayRepo;
        //private ICustomerOrder _CORepository;
        private ISale _SaleRepository;

        public ValideBorderoDepotFactureController(
                 IBusinessDay busDayRepo,
                 //       ICustomerOrder CoRepository,
                 ISale saleRep
                )
        {
            this._busDayRepo = busDayRepo;
            //this._CORepository = CoRepository;
            this._SaleRepository = saleRep;
        }
        // GET: CashRegister/BorderoDepotFacture
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {

                List<BusinessDay> UserBusDays = (List<BusinessDay>)Session["UserBusDays"];
                DateTime currentDateOp = UserBusDays.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = UserBusDays.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = UserBusDays.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;

                Session["CustomerOrders"] = new List<CustomerOrder>();

                return View(ModelBorderoValidate());
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        public JsonResult DeleteCancelBordero(int BorderoDepotID)
        {
            bool status = false;
            string Message = "";
            try
            {
                BusinessDay secbusday = SessionBusinessDay(null);
                _SaleRepository.DeleteCancelBordero(secbusday.BranchID, BorderoDepotID, secbusday.BDDateOperation, SessionGlobalPersonID);
                status = true;
                Message = Resources.Success + " Data has been cancelled";
            }
            catch (Exception e)
            {
                Message = "Error " + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }

        public JsonResult ModelBorderoValidate()
        {
            var listDistAcc = db.BorderoDepots.Where(c => !c.ValideBorderoDepot)
                .Select(a => new
                {
                    BorderoDepotID = a.BorderoDepotID,
                    CodeBorderoDepot = a.CodeBorderoDepot,
                    AssureurName = a.Assureur.Name,
                    LieuxdeDepotBorderoID = a.LieuxdeDepotBorderoID,
                    Insurance = a.Assureur.Name,
                    DepositDate = a.BorderoDepotDate,
                    ValideBorderoDepot = a.ValideBorderoDepot,

                    ValidatedDate = a.ValidBorderoDepotDate,
                    GeneratedBy = (a.GenerateBy.Name + " " + a.GenerateBy.Description),
                    ValidatedBy = (a.ValidateBy != null) ? (a.ValidateBy.Name + " " + a.ValidateBy.Description) : 
                                  (!a.ValideBorderoDepot ? "PENDING" :  "UNKNOWN")
                }).ToList().OrderBy(c => c.CodeBorderoDepot);

            var model = new
            {
                data = from c in listDistAcc.ToList()
                select
                new
                {
                    BorderoDepotID=c.BorderoDepotID,
                    CodeBorderoDepot = c.CodeBorderoDepot,
                    AssureurName=c.AssureurName,

                    DepositDate = c.DepositDate.ToString("dd/MM/yyyy"),
                    ValidatedDate = c.ValidatedDate != null ? c.ValidatedDate.Value.ToString("dd/MM/yyyy") :
                                        (!c.ValideBorderoDepot ? "PENDING" : "UNKNOWN"),
                    GeneratedBy = c.GeneratedBy,
                    ValidatedBy = c.ValidatedBy,

                    LieuxdeDepotBordero = (c.LieuxdeDepotBorderoID >0 ) ? db.LieuxdeDepotBorderos.Find(c.LieuxdeDepotBorderoID).LieuxdeDepotBorderoName : ""
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModelBillInsure(int BranchID, DateTime Begindate, DateTime EndDate, int AssureurID = 0) //, string BeginDate, string EndDate)
        {
            List<object> list = new List<object>();
            List<CustomerOrder> listBillInsuredOp = new List<CustomerOrder>();

            if (AssureurID > 0)
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate) && co.AssureurID == AssureurID && co.BillState == StatutFacture.Validated).ToList();
            }
            else
            {
                listBillInsuredOp = db.CustomerOrders.Where(co => co.BranchID == BranchID && (co.CustomerOrderDate >= Begindate && co.CustomerOrderDate <= EndDate) && co.BillState == StatutFacture.Validated).ToList();
            }

            var model = new
            {
                data = from c in listBillInsuredOp
                       select
                        new
                        {
                        CustomerOrderID = c.CustomerOrderID,
                        BranchID = c.BranchID,
                        CustomerName = c.CustomerName,
                        CompanyName = c.CompanyName,
                        CustomerOrderDate = c.CustomerOrderDate.ToString("dd/MM/yyyy"),
                        UIBranchCode = c.Branch.BranchName,
                        CustomerOrderNumber = c.CustomerOrderNumber,
                        PoliceAssurance = c.PoliceAssurance,
                        NumeroFacture = c.NumeroFacture,
                        PhoneNumber = c.PhoneNumber,
                        MntAssureur = c.Plafond,
                                                //ReductionAmount = c.MntReduction,
                                                InsuranceCompany = c.Assureur.Name,
                        MntValidate = c.MntValidate,
                        Remainder = c.Remainder
                        }
            };

            return Json(model, JsonRequestBehavior.AllowGet);

        }

        //[HttpPost]
        public JsonResult CommandOderLines()
        {

            try
            {
                List<CustomerOrder> dataTmp = (List<CustomerOrder>)Session["CustomerOrders"];
                
                var model = new
                {
                    data = from authord in dataTmp
                    select new
                    {
                        CustomerOrderID =  authord.CustomerOrderID,
                        AssureurName = (authord.AssureurName == null) ? authord.CustomerName : authord.AssureurName,
                        CustomerName = (authord.CustomerName == null) ? "" : authord.CustomerName,
                        CompanyName = (authord.CompanyName == null) ? "" : authord.CompanyName,
                        CustomerOrderDate = (authord.CustomerOrderDate == null) ? "01/01/1900" : authord.CustomerOrderDate.ToString("dd/MM/yyyy"),
                        NumeroFacture = (authord.NumeroFacture == null) ? "" : authord.NumeroFacture,
                        ValidateAmount = authord.MntValideBordero,
                        Plafond = authord.Plafond
                    }
                };

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                List<object> list = new List<object>();
                // TempData["Message"] = "Error " + e.Message;
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ProcCalculeTaxAndPopulateTable(int BorderoDepotID, float TaxReduction)
        {
            List<object> _CommandList = new List<object>();
            try
            {
                double MntValideBordero = 0d;
                string CodeBorderoDepot = "";

                Session["CustomerOrders"] = new List<CustomerOrder>();
                double TotalBordero = 0d, TotMntValideBordero = 0d, TotDifference = 0d;
                List<CustomerOrder> customerOrders = (List<CustomerOrder>)Session["CustomerOrders"];

                var lstcustomerOrder = db.CustomerOrders.Join(db.BorderoDepots, c => c.BorderoDepotID, b => b.BorderoDepotID,
                    (c, b) => new { c, b }).Where(cb => cb.c.BorderoDepotID.HasValue && cb.c.BorderoDepotID.Value == BorderoDepotID)
                    .Select(s => new
                    {
                        Plafond = s.c.Plafond,
                        MntValideBordero = s.c.MntValideBordero,
                        LieuxdeDepotBordero = s.c.LieuxdeDepotBordero,
                        CodeBorderoDepot = s.b.CodeBorderoDepot,
                        CustomerOrderID = s.c.CustomerOrderID,
                        AssureurName = s.c.InsurreName,
                        CustomerName = s.c.CustomerName,
                        CompanyName = s.c.CompanyName,
                        CustomerOrderDate = s.c.CustomerOrderDate,
                        NumeroFacture = s.c.NumeroFacture,
                        NumeroBonPriseEnCharge = s.c.NumeroBonPriseEnCharge
                    }).ToList();

                if (lstcustomerOrder.Count == 0)
                {
                    TempData["Message"] = "Warning - No data to proceed";
                    return Json(_CommandList, JsonRequestBehavior.AllowGet);
                }

                foreach (var authord in lstcustomerOrder)
                {
                    CodeBorderoDepot = authord.CodeBorderoDepot;

                    TotalBordero = TotalBordero + authord.Plafond;
                    
                    MntValideBordero = authord.Plafond - Math.Floor((authord.Plafond * TaxReduction) / 100);
                    TotMntValideBordero = TotMntValideBordero + MntValideBordero;

                    CustomerOrder custOrder = new CustomerOrder()
                    {
                        CustomerOrderID = authord.CustomerOrderID,
                        MntValideBordero = MntValideBordero,
                        AssureurName = authord.AssureurName,
                        CustomerName = authord.CustomerName,
                        CompanyName = authord.CompanyName,
                        CustomerOrderDate = authord.CustomerOrderDate,
                        NumeroFacture = authord.NumeroFacture,
                        NumeroBonPriseEnCharge = authord.NumeroBonPriseEnCharge,
                        Plafond = authord.Plafond
                    };
                    customerOrders.Add(custOrder);
                    MntValideBordero = 0;
                }

                Session["CustomerOrders"] = customerOrders;

                TotDifference = TotalBordero - TotMntValideBordero;

                _CommandList.Add(new
                {
                    TotalBordero = TotalBordero,
                    TotMntValideBordero = TotMntValideBordero,
                    TotDifference = TotDifference,
                    CodeBorderoDepot = CodeBorderoDepot,
                    BorderoDepotID = BorderoDepotID
                });
                return Json(_CommandList, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                return Json(_CommandList, JsonRequestBehavior.AllowGet);
            }
        }

        //public JsonResult CalculeTaxAndPopulateTable(int BorderoDepotID, int TaxReduction)
        //{
        //    List<object> list = new List<object>();
        //    try
        //    {
        //        double MntValideBordero = 0d;
        //        Session["CustomerOrders"] = new List<CustomerOrder>();
                
        //        List<CustomerOrder> customerOrders = (List<CustomerOrder>)Session["CustomerOrders"];

        //        var lstcustomerOrder = db.CustomerOrders.Join(db.BorderoDepots, c => c.BorderoDepotID, b => b.BorderoDepotID,
        //            (c, b) => new { c, b }).Where(cb => cb.c.BorderoDepotID.HasValue && cb.c.BorderoDepotID.Value == BorderoDepotID)
        //            .Select(s => new
        //            {
        //                Plafond = s.c.Plafond,
        //                MntValideBordero = s.c.MntValideBordero,
        //                LieuxdeDepotBordero = s.c.LieuxdeDepotBordero,
        //                CodeBorderoDepot = s.b.CodeBorderoDepot,
        //                CustomerOrderID = s.c.CustomerOrderID,
        //                AssureurName = s.c.InsurreName,
        //                CustomerName = s.c.CustomerName,
        //                CompanyName = s.c.CompanyName,
        //                CustomerOrderDate = s.c.CustomerOrderDate,
        //                NumeroFacture = s.c.NumeroFacture,
        //                NumeroBonPriseEnCharge = s.c.NumeroBonPriseEnCharge
        //            }).ToList();

        //        if (lstcustomerOrder.Count == 0)
        //        {
        //            TempData["Message"] = "Warning - No data to proceed";
        //            return Json(list, JsonRequestBehavior.AllowGet);
        //        }

        //        foreach (var authord in lstcustomerOrder)
        //        {
        //            MntValideBordero= authord.Plafond - Math.Floor((authord.Plafond* TaxReduction)/100);
                    
        //            CustomerOrder custOrder = new CustomerOrder()
        //            {
        //                CustomerOrderID = authord.CustomerOrderID,
        //                MntValideBordero = MntValideBordero,
        //                AssureurName = authord.AssureurName,
        //                CustomerName = authord.CustomerName,
        //                CompanyName = authord.CompanyName,
        //                CustomerOrderDate = authord.CustomerOrderDate,
        //                NumeroFacture = authord.NumeroFacture,
        //                NumeroBonPriseEnCharge = authord.NumeroBonPriseEnCharge,
        //                Plafond = authord.Plafond
        //            };
        //            customerOrders.Add(custOrder);
        //            MntValideBordero = 0d;
        //        }

        //        Session["CustomerOrders"] = customerOrders;

        //        List<CustomerOrder> dataTmp = (List<CustomerOrder>)Session["CustomerOrders"];

        //        var model = new
        //        {
        //            data = from authord in dataTmp
        //                   select new
        //                   {
        //                       CustomerOrderID = authord.CustomerOrderID,
        //                       AssureurName = (authord.AssureurName == null) ? authord.CustomerName : authord.AssureurName,
        //                       CustomerName = (authord.CustomerName == null) ? "" : authord.CustomerName,
        //                       CompanyName = (authord.CompanyName == null) ? "" : authord.CompanyName,
        //                       CustomerOrderDate = (authord.CustomerOrderDate == null) ? "01/01/1900" : authord.CustomerOrderDate.ToString("dd/MM/yyyy"),
        //                       NumeroFacture = (authord.NumeroFacture == null) ? "" : authord.NumeroFacture,
        //                       ValidateAmount = authord.MntValideBordero,
        //                       Plafond = authord.Plafond
        //                   }
        //        };

        //        return Json(model, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
                
        //        // TempData["Message"] = "Error " + e.Message;
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public JsonResult InitializeCommandFields(int ID)
        {
            List<object> _CommandList = new List<object>();
            try
            {
                string CodeBorderoDepot = "";

                Session["CustomerOrders"] = new List<CustomerOrder>();
                double TotalBordero = 0d, TotMntValideBordero=0d,TotDifference=0d, TaxReduction=0d;
                List<CustomerOrder> customerOrders = (List<CustomerOrder>)Session["CustomerOrders"];

                var lstcustomerOrder = db.CustomerOrders.Join(db.BorderoDepots, c => c.BorderoDepotID, b => b.BorderoDepotID,
                    (c, b) => new { c, b }).Where(cb => cb.c.BorderoDepotID.HasValue && cb.c.BorderoDepotID.Value == ID)
                    .Select(s => new
                    {
                        Plafond = s.c.Plafond,
                        MntValideBordero = s.c.MntValideBordero,
                        LieuxdeDepotBordero = s.c.LieuxdeDepotBordero,
                        CodeBorderoDepot = s.b.CodeBorderoDepot,
                        CustomerOrderID=s.c.CustomerOrderID,
                        AssureurName=s.c.InsurreName,
                        CustomerName=s.c.CustomerName,
                        CompanyName=s.c.CompanyName,
                        CustomerOrderDate=s.c.CustomerOrderDate,
                        NumeroFacture=s.c.NumeroFacture,
                        NumeroBonPriseEnCharge=s.c.NumeroBonPriseEnCharge,
                        TaxReduction=s.c.RemiseAssurance
                    }).ToList();

                //List<CustomerOrder> lstcustomerOrder = (from c in db.CustomerOrders 
                //                               where   c.BorderoDepotID.HasValue && c.BorderoDepotID == ID
                //                                        select c).ToList();
                if (lstcustomerOrder.Count == 0)
                {
                    TempData["Message"] = "Warning - No data to proceed";
                    return Json(_CommandList, JsonRequestBehavior.AllowGet);
                }

                foreach (var authord in lstcustomerOrder)
                {
                    CodeBorderoDepot = authord.CodeBorderoDepot;
                    TaxReduction = authord.TaxReduction;

                    TotalBordero = TotalBordero + authord.Plafond;
                    TotMntValideBordero = TotMntValideBordero + authord.MntValideBordero;
                    CustomerOrder custOrder = new CustomerOrder()
                    {
                        CustomerOrderID = authord.CustomerOrderID,
                        MntValideBordero = authord.MntValideBordero,
                        AssureurName=authord.AssureurName,
                        CustomerName = authord.CustomerName,
                        CompanyName = authord.CompanyName,
                        CustomerOrderDate = authord.CustomerOrderDate,
                        NumeroFacture = authord.NumeroFacture,
                        NumeroBonPriseEnCharge = authord.NumeroBonPriseEnCharge,
                        Plafond = authord.Plafond
                    };
                    customerOrders.Add(custOrder);
                }

                Session["CustomerOrders"] = customerOrders;

                TotDifference = TotalBordero - TotMntValideBordero;

                _CommandList.Add(new
                {
                    TotalBordero = TotalBordero,
                    TotMntValideBordero= TotMntValideBordero,
                    TotDifference= TotDifference,
                    CodeBorderoDepot= CodeBorderoDepot,
                    BorderoDepotID= ID,
                    TaxReduction= TaxReduction
                });
                return Json(_CommandList, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                return Json(_CommandList, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult PaymentMethods()
        {
            List<object> model = new List<object>();
            db.PaymentMethods.OfType<Bank>().ToList().ForEach(p =>
            {
                model.Add(
                        new
                        {
                            ID = p.ID,
                            Name = p.Name
                        }
                    );
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValideBorderoDepot(List<BorderoItems> BorderoItems,int PaymentMethodID,string Reference,int BorderoDepotID,string HeureVente)
        {

            bool status = false;
            string Message = "";

            try
            {
                if (BorderoItems == null || BorderoItems.Count == 0)
                {
                    status = true;
                    Message = "No data to update!!! Field the accounting Amount before proceed";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                BusinessDay sessBusDay = SessionBusinessDay(null);
                _SaleRepository.ValidateBorderoDepotFacture(BorderoItems, BorderoDepotID, HeureVente, sessBusDay.BranchID, SessionGlobalPersonID, sessBusDay.BDDateOperation, PaymentMethodID, Reference);
                
                /*foreach (BorderoItems inLines in BorderoItems)
                {
                    CustomerOrder custOrder = db.CustomerOrders.Find(inLines.ID);
                    if (custOrder.CustomerOrderID > 0)
                    {
                        if (custOrder.AssureurID == null)
                        {
                            status = false;
                            Message ="The Bill Ref " + inLines.NumeroFacture + " not yet been validated. Please consult your Accountant";
                            return new JsonResult { Data = new { status = status, Message = Message } };
                        }
                        custOrder.MntValidate = Convert.ToDouble(inLines.MntValideBordero);

                        custOrder.PaymentMethodID = PaymentMethodID;
                        custOrder.PaymentReference = Reference;

                        if (custOrder.MntValidate >= 0)
                        {
                            _SaleRepository.SaveChanges(custOrder, inLines.heureVente, SessionGlobalPersonID, sessBusDay.BDDateOperation);
                        }

                    }
                }
                */

                status = true;
                Message = "Updated rows successfully!";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de l'opération, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + " car " + e.StackTrace + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }
    }
}