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
    public class LieuxDepotBillController : BaseController
    {
        private IRepositorySupply<LieuxdeDepotBordero> _LieuxdeDepotBorderoRepository;

        public LieuxDepotBillController(
                 IRepositorySupply<LieuxdeDepotBordero> lieuxdeDepotBorderoRepository
                )
        {
            this._LieuxdeDepotBorderoRepository = lieuxdeDepotBorderoRepository;

        }

        // GET: Sale/LieuxDepotBill
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return View(LieuxdeDepotBorderoModel());
        }


        //[HttpPost]
        public JsonResult AddManager()
        {
            LieuxdeDepotBordero LieuxdeDepotBordero = new LieuxdeDepotBordero();
            TryUpdateModel(LieuxdeDepotBordero);

            //en cas de mise à jour
            if (LieuxdeDepotBordero.LieuxdeDepotBorderoID > 0)
            {
                return this.UpdateLieuxdeDepotBordero(LieuxdeDepotBordero);
            }
            else
            {
                return this.AddLieuxdeDepotBordero(LieuxdeDepotBordero);
            }
        }

        public JsonResult AddLieuxdeDepotBordero(LieuxdeDepotBordero LieuxdeDepotBordero)
        {
            bool status = false;
            string Message = "";
            try
            {

                if (((db.LieuxdeDepotBorderos.FirstOrDefault(c => c.LieuxdeDepotBorderoName == LieuxdeDepotBordero.LieuxdeDepotBorderoName) == null)))
                {
                    _LieuxdeDepotBorderoRepository.Create(LieuxdeDepotBordero);
                    statusOperation =  LieuxdeDepotBordero.LieuxdeDepotBorderoName + " has been successfully created";
                    Message = Resources.Success + "-" + statusOperation;
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    String statusOperation1 = @" Le " + LieuxdeDepotBordero.LieuxdeDepotBorderoName + " existe déjà!<br/>"
                                        + "veuillez changer le Nom";

                    statusOperation = Resources.er_alert_danger + statusOperation1;
                    Message = Resources.LieuxdeDepotBordero + "-" + statusOperation;
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }

            }
            catch (Exception e)
            {
                statusOperation = Resources.er_alert_danger + e.Message;
                Message = Resources.LieuxdeDepotBordero + "-" + statusOperation;
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }

        /// <summary>
        /// cette méthode est appelée lorqu' après avoir clicquer sur l'icone modifier du tableau on modifie le formulaire et clique sur enregistrer
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        public JsonResult UpdateLieuxdeDepotBordero(LieuxdeDepotBordero LieuxdeDepotBordero)
        {
            bool status = false;
            string Message = "";
            try
            {
                LieuxdeDepotBordero existingLieuxdeDepotBordero = db.LieuxdeDepotBorderos.Find(LieuxdeDepotBordero.LieuxdeDepotBorderoID);
                List<LieuxdeDepotBordero> LieuxdeDepotBorderos = db.LieuxdeDepotBorderos.ToList();
                LieuxdeDepotBorderos.Remove(existingLieuxdeDepotBordero);

                if (((LieuxdeDepotBorderos.FirstOrDefault(c => c.LieuxdeDepotBorderoName == LieuxdeDepotBordero.LieuxdeDepotBorderoName) == null)))
                {
                    _LieuxdeDepotBorderoRepository.Update(LieuxdeDepotBordero, LieuxdeDepotBordero.LieuxdeDepotBorderoID);
                    statusOperation =  LieuxdeDepotBordero.LieuxdeDepotBorderoName + " has been successfully updated";
                    status = true;
                    Message = Resources.Success + "-" + statusOperation;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {
                    statusOperation = @"Le " + LieuxdeDepotBordero.LieuxdeDepotBorderoName + " existe déjà!<br/>"
                                        + "veuillez changer le Nom";
                    Message = Resources.LieuxdeDepotBordero + "-" + statusOperation;
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
        public JsonResult DeleteLieuxdeDepotBordero(int LieuxdeDepotBorderoID)
        {
            bool status = false;
            string Message = "";
            try
            {
                LieuxdeDepotBordero deletedLieuxdeDepotBordero = db.LieuxdeDepotBorderos.Find(LieuxdeDepotBorderoID);
                List<CustomerOrder> LieuxdeDepotBorderoProducts = db.CustomerOrders.Where(p => p.LieuxdeDepotBorderoID == deletedLieuxdeDepotBordero.LieuxdeDepotBorderoID).ToList();

                if ((LieuxdeDepotBorderoProducts == null) || (LieuxdeDepotBorderoProducts.Count == 0))
                {
                    db.LieuxdeDepotBorderos.Remove(deletedLieuxdeDepotBordero);
                    db.SaveChanges();
                    Message = "LieuxdeDepotBordero " + deletedLieuxdeDepotBordero.LieuxdeDepotBorderoName + " has been successfully deleted";
                    status = true;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                else
                {

                    Message = "LieuxdeDepotBordero - Désolé vous ne pouvez pas supprimer " + deletedLieuxdeDepotBordero.LieuxdeDepotBorderoName + " parcequ'elle contient déjà été attribué";
                    status = false;
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
            }
            catch (Exception e)
            {
                Message = "LieuxdeDepotBordero - L'erreur " + e.Message + " s'est produite lors de la suppression! veuillez contactez l'administrateur et/ou essayez à nouveau";
                status = false;
                return new JsonResult { Data = new { status = status, Message = Message } };
            }
        }



        public JsonResult InitializePurchaseFields(int ID)
        {

            List<object> list = new List<object>();

            if (ID > 0)
            {
                LieuxdeDepotBordero LieuxdeDepotBordero = db.LieuxdeDepotBorderos.Find(ID);

                list.Add(new
                {
                    LieuxdeDepotBorderoID = LieuxdeDepotBordero.LieuxdeDepotBorderoID,
                    LieuxdeDepotBorderoName = LieuxdeDepotBordero.LieuxdeDepotBorderoName,
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        

        public List<LieuxdeDepotBordero> LieuxdeDepotBorderoModel()
        {
            List<LieuxdeDepotBordero> dataTmp = LoadComponent.GetAllGenericLieuxdeDepotBorderos().ToList();
            List<LieuxdeDepotBordero> list = new List<LieuxdeDepotBordero>();

            foreach (LieuxdeDepotBordero c in dataTmp)
            {
                list.Add(
                    new LieuxdeDepotBordero
                    {
                        LieuxdeDepotBorderoID = c.LieuxdeDepotBorderoID,
                        LieuxdeDepotBorderoName = c.LieuxdeDepotBorderoName
                    }
                   );
            }

            return list;
        }

        //[HttpPost]
        public JsonResult GetAlllistLieuxdeDepotBorderos()
        {
            var model = new
            {
                data = from c in LieuxdeDepotBorderoModel()
                       select new
                       {
                           LieuxdeDepotBorderoID = c.LieuxdeDepotBorderoID,
                           LieuxdeDepotBorderoName = c.LieuxdeDepotBorderoName,
                       }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}