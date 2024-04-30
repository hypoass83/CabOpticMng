using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using CABOPMANAGEMENT.Controllers;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class InsuredCompanyController : BaseController
    {
        
        private IRepositorySupply<InsuredCompany> _InsuredCompanyRepository;

        public InsuredCompanyController(
                 IRepositorySupply<InsuredCompany> InsuredCompanyRepository
                )
        {
            this._InsuredCompanyRepository = InsuredCompanyRepository;

        }


        // GET: Sale/InsuredCompany
        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.InsuredCompany
        /// </summary>
        /// <returns>ActionResult</returns>
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return View(InsuredCompanyModel());
        }


        //[HttpPost]
        public JsonResult AddManager()
        {
            InsuredCompany InsuredCompany = new InsuredCompany();
            TryUpdateModel(InsuredCompany);

            //en cas de mise à jour
            if (InsuredCompany.InsuredCompanyID > 0)
            {
                return this.UpdateInsuredCompany(InsuredCompany);
            }
            else
            {
                return this.AddInsuredCompany(InsuredCompany);
            }
        }

        public JsonResult AddInsuredCompany(InsuredCompany InsuredCompany)
        {
            bool status = false;
            string Message = "";
            try
            {

                if (((db.InsuredCompanies.FirstOrDefault(c => c.InsuredCompanyCode == InsuredCompany.InsuredCompanyCode) == null)))
                {
                    _InsuredCompanyRepository.Create(InsuredCompany);
                    statusOperation = "The Insured Company " + InsuredCompany.InsuredCompanyCode + " has been successfully created";
                    Message = Resources.Success + "-" + statusOperation;
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    String statusOperation1 = @"Le code " + InsuredCompany.InsuredCompanyCode + " et / ou le label " + InsuredCompany.InsuredCompanyLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";

                    statusOperation = Resources.er_alert_danger + statusOperation1;
                    Message = Resources.InsuredCompany + "-" + statusOperation;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                Message = Resources.InsuredCompany + "-" + statusOperation;
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }


        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public JsonResult UpdateInsuredCompany(InsuredCompany InsuredCompany)
        {
            bool status = false;
            string Message = "";
            try
            {
                InsuredCompany existingInsuredCompany = db.InsuredCompanies.Find(InsuredCompany.InsuredCompanyID);
                List<InsuredCompany> InsuredCompanies = db.InsuredCompanies.ToList();
                InsuredCompanies.Remove(existingInsuredCompany);

                if (((InsuredCompanies.FirstOrDefault(c => c.InsuredCompanyCode == InsuredCompany.InsuredCompanyCode) == null)))
                {
                    _InsuredCompanyRepository.Update(InsuredCompany, InsuredCompany.InsuredCompanyID);
                    statusOperation = "The Insured Company " + InsuredCompany.InsuredCompanyCode + " has been successfully updated";
                    status = true;
                    Message = Resources.Success + "-" + statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    statusOperation = @"Le code " + InsuredCompany.InsuredCompanyCode + " et / ou le label " + InsuredCompany.InsuredCompanyLabel + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";
                    Message = Resources.InsuredCompany + "-" + statusOperation;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

            }
            catch (Exception e)
            {
                status = false;
                Message = @"Une erreur s'est produite lors de la mise à jour, veuillez contactez l'administrateur et/ou essayez à nouveau
                                    <br/>Code : <code>" + e.Message + "</code>";
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

        //[HttpPost]
        public JsonResult DeleteInsuredCompany(int InsuredCompanyID)
        {
            bool status = false;
            string Message = "";
            try
            {
                InsuredCompany deletedInsuredCompany = db.InsuredCompanies.Find(InsuredCompanyID);
                List<CustomerOrder> InsuredCompanyProducts = db.CustomerOrders.Where(p => p.InsuredCompanyID == deletedInsuredCompany.InsuredCompanyID).ToList();

                if ((InsuredCompanyProducts == null) || (InsuredCompanyProducts.Count == 0))
                {
                    db.InsuredCompanies.Remove(deletedInsuredCompany);
                    db.SaveChanges();
                    Message = "InsuredCompany - The Insured Company " + deletedInsuredCompany.InsuredCompanyCode + " has been successfully deleted";
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {

                    Message = "InsuredCompany - Désolé vous ne pouvez pas supprimer " + deletedInsuredCompany.InsuredCompanyCode + " parcequ'elle a déjà été attribué";
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
            }
            catch (Exception e)
            {
                Message = "InsuredCompany - L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }



        public JsonResult InitializePurchaseFields(int ID)
        {

            List<object> list = new List<object>();

            if (ID > 0)
            {
                InsuredCompany InsuredCompany = db.InsuredCompanies.Find(ID);

                list.Add(new
                {
                    InsuredCompanyID = InsuredCompany.InsuredCompanyID,
                    InsuredCompanyCode = InsuredCompany.InsuredCompanyCode,
                    InsuredCompanyLabel = InsuredCompany.InsuredCompanyLabel,
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }

        public List<InsuredCompany> InsuredCompanyModel()
        {
            List<InsuredCompany> dataTmp = LoadComponent.GetAllGenericInsuredCompanies().ToList();
            List<InsuredCompany> list = new List<InsuredCompany>();

            foreach (InsuredCompany c in dataTmp)
            {
                list.Add(
                    new InsuredCompany
                    {
                        InsuredCompanyID = c.InsuredCompanyID,
                        InsuredCompanyCode = c.InsuredCompanyCode,
                        InsuredCompanyLabel = c.InsuredCompanyLabel,
                    }
                   );
            }

            return list;
        }

        //[HttpPost]
        public JsonResult GetAlllistInsuredCompanies()
        {
            var model = new
            {
                data = from c in InsuredCompanyModel()
                       select new
                       {
                           InsuredCompanyID = c.InsuredCompanyID,
                           InsuredCompanyCode = c.InsuredCompanyCode,
                           InsuredCompanyLabel = c.InsuredCompanyLabel,
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}