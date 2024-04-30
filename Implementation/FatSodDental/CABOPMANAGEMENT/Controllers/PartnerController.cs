using FatSod.Ressources;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class PartnerController : BaseController
    {
        private IPartner partnerRepository;

        private const string CONTROLLER_NAME = "Administration/Partner";
        private const string VIEW_NAME = "Index";


        public PartnerController(IPartner partnerRepository)
        {
            this.partnerRepository = partnerRepository;

        }

        /// <summary>
        /// the view of this action method is stronglyTyped with FatSod.Supply.Entities.Partner
        /// </summary>
        /// <returns>ActionResult</returns>
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return View(PartnerModel());
        }


        //[HttpPost]
        public JsonResult AddManager()
        {
            Partner partner = new Partner();
            TryUpdateModel(partner);

            //en cas de mise à jour
            if (partner.PartnerId > 0)
            {
                return this.UpdatePartner(partner);
            }
            else
            {
                return this.AddPartner(partner);
            }
        }

        public JsonResult AddPartner(Partner partner)
        {
            bool status = false;
            string Message = "";
            try
            {

                if (((db.Partners.FirstOrDefault(c => c.PartnerCode == partner.FullName || c.FullName == partner.FullName) == null)))
                {
                    partnerRepository.Create(partner);
                    statusOperation = "The Partner " + partner.FullName + " has been successfully created";
                    Message = Resources.Success + "-" + statusOperation;
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    String statusOperation1 = @"Un partenaire ayant le code " + partner.PartnerCode + " et / ou le label " + partner.FullName + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le label";

                    statusOperation = Resources.er_alert_danger + statusOperation1;
                    Message = Resources.Partner + "-" + statusOperation;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                Message = Resources.Partner + "-" + statusOperation;
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }


        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public JsonResult UpdatePartner(Partner partner)
        {
            bool status = false;
            string Message = "";
            try
            {
                Partner existingPartner = db.Partners.Find(partner.PartnerId);
                List<Partner> Partners = db.Partners.ToList();
                Partners.Remove(existingPartner);

                if (((Partners.FirstOrDefault(c => c.PartnerCode == partner.PartnerCode || c.FullName == partner.FullName) == null)))
                {
                    partnerRepository.Update(partner, partner.PartnerId);
                    statusOperation = "The Partner " + partner.PartnerCode + " has been successfully updated";
                    status = true;
                    Message = Resources.Success + "-" + statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    statusOperation = @"Un partenaire ayant le code " + partner.PartnerCode + " et / ou le Nom " + partner.FullName + " existe déjà!<br/>"
                                        + "veuillez changer de code et / ou le Nom";
                    Message = Resources.Partner + "-" + statusOperation;
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
        public JsonResult Delete(int PartnerId)
        {
            bool status = false;
            string Message = "";
            try
            {
                partnerRepository.Delete(PartnerId);
                Message = "Partner - The Partner " + " has been successfully deleted";
                status = true;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
            catch (Exception e)
            {
                Message = "Partner - L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

        public JsonResult Details(int ID)
        {
            List<object> list = new List<object>();

            if (ID > 0)
            {
                Partner partner = db.Partners.Find(ID);

                list.Add(new
                {
                    PartnerId = partner.PartnerId,
                    PartnerCode = partner.PartnerCode,
                    FullName = partner.FullName,
                    PhoneNumber = partner.PhoneNumber,
                    Email = partner.Email,
                    Company = partner.Company,
                    Function = partner.Function,
                    ProductsAndServices = partner.ProductsAndServices
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private ActionResult NotAuthorized()
        {
            return View();
        }

        public List<Partner> PartnerModel()
        {
            List<Partner> partners = partnerRepository.FindAll
                .Select(c =>
                    new Partner
                    {
                        PartnerId = c.PartnerId,
                        PartnerCode = c.PartnerCode,
                        FullName = c.FullName,
                        ProductsAndServices = c.ProductsAndServices,
                        Company = c.Company,
                        Email = c.Email,
                        Function = c.Function,
                        PhoneNumber = c.PhoneNumber
                    }
                ).ToList();

            return partners;
        }

        //[HttpPost]
        public JsonResult GetAllPartners()
        {
            var model = new
            {
                data = PartnerModel()
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }

}