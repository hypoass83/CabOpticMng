using FatSod.DataContext.Initializer;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using System.Threading.Tasks;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using FatSod.DataContext.Concrete;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class SupplierReturnController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/" + CodeValue.Supply.SupplierReturnMenu.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.SupplierReturnMenu.PATH;

        private IInventoryDirectory _invDirRepo;
        private ISupplierReturn _supplierReturnRepository;
        private IBusinessDay _busDayRepo;
        
        public SupplierReturnController(
            ISupplierReturn srRepo, 
            IBusinessDay busDayRepo, IInventoryDirectory idRepo
            )
        {
            this._supplierReturnRepository = srRepo;
            this._busDayRepo = busDayRepo;
            this._invDirRepo = idRepo;
            
        }
        public ActionResult SupplierReturn()
        {
            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, VIEW_NAME, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}
            //Session["Curent_Page"] = VIEW_NAME;
            //Session["Curent_Controller"] = CONTROLLER_NAME;
            //Session["SupplierReturnLines"] = new List<SupplierReturnLine>();
            //Session["PurchaseLines"] = new List<PurchaseLine>();
            /*
            this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            {
                Url = Url.Action(VIEW_NAME),
                DisableCaching = false,
                Mode = LoadMode.Frame
            });*/
            //Ext.Net.MVC.PartialViewResult rPVResult = new Ext.Net.MVC.PartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelReturnAblePurchases
            //};
            //return rPVResult;
            return View(ModelReturnAblePurchases);
        }

        /// <summary>
        /// This method allow to initialize grid panel for to updated one Purchase line
        /// </summary>
        /// <param name="ID">ID of PurchaseLine</param>
        /// <returns></returns>
        [HttpPost]
        public DirectResult InitializeSRLineFieldsBySL(int PurchaseLineID)
        {

            //we take Purchase and her PurchaseLines
            PurchaseLine selectedPurchaseLine = db.PurchaseLines.Find(PurchaseLineID);
            this.GetCmp<NumberField>("PurchaseLineID").Value = selectedPurchaseLine.LineID;
            this.GetCmp<TextField>("TMPID").SetValue(0);

            this.GetCmp<TextField>("Product").SetValue(selectedPurchaseLine.Product.GetProductCode());
            this.GetCmp<TextField>("Localization").SetValue(selectedPurchaseLine.Localization.LocalizationLabel);
            this.GetCmp<NumberField>("LineQuantity").SetValue(0);
            //Nous nous assurons que l'utilisateur ne saississe pas une quantité supérieur à celle qui peut être retournée.. 
            this.GetCmp<NumberField>("LineQuantity").MaxValue = this.PurchaseLineReturnAbleQuantity(selectedPurchaseLine);

            this.ManageCady();

            return this.Direct();
        }

        /// <summary>
        /// Cette méthode est appelée quand un achat est sélectionné et permet de renseigner les champs de formulaire liés à l'achat sélectionné. il s'agit de :
        /// 1-Le formulaire d'achat
        /// 2-le cady d'achat. 
        /// NB : Il reste à l'utilisateur de remplir le cady de retour
        /// </summary>
        /// <param name="ID"> ID de l'achat sélectionné par l'utilisateur</param>
        /// <returns></returns>
        [HttpPost]
        public DirectResult InitializeFields(int PurchaseID)
        {
            ResetReturn();
            
            //we take Purchase and her PurchaseLines
            Purchase selectedPurchase = db.Purchases.Find(PurchaseID);
            
            List<PurchaseLine> allPurchaseLines = db.PurchaseLines.Where(sl => sl.PurchaseID == PurchaseID).ToList();
            List<PurchaseLine> returnablePurchaseLines = new List<PurchaseLine>();

            foreach (PurchaseLine pl in allPurchaseLines)
            {
                if (_supplierReturnRepository.IsAllLineReturn(pl) == false)
                {
                    returnablePurchaseLines.Add(pl);
                }
            }

            Session["PurchaseLines"] = returnablePurchaseLines;

            BusinessDay currentBD = _busDayRepo.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();
            this.GetCmp<DateField>("SupplierReturnDate").Value = currentBD.BDDateOperation;
            this.GetCmp<TextField>("PurchaseID").Value = selectedPurchase.PurchaseID;
            this.GetCmp<ComboBox>("SupplierID").SetValue(selectedPurchase.SupplierID);
            this.GetCmp<DateField>("PurchaseDate").Value = selectedPurchase.PurchaseDate;
            this.GetCmp<TextField>("PurchaseReference").Value = selectedPurchase.PurchaseReference;
            this.GetCmp<DateField>("PurchaseDeliveryDate").Value = selectedPurchase.PurchaseDeliveryDate;

            this.SimpleReset2();

            this.ApplyExtraPrices(null, selectedPurchase.RateReduction, selectedPurchase.RateDiscount, 0, selectedPurchase.Transport, selectedPurchase.VatRate);

            return this.Direct();
        }
        
        /// <summary>
        /// Quantité de la ligne qui peut encore être retournée
        /// </summary>
        /// <param name="selectedPurchaseLine"></param>
        /// <returns></returns>
        public double PurchaseLineReturnAbleQuantity(PurchaseLine selectedPurchaseLine)
        {
            double returnedQuantity = this._supplierReturnRepository.PurchaseLineReturnedQuantity(selectedPurchaseLine);
            return selectedPurchaseLine.LineQuantity - returnedQuantity;
        }
        private List<object> ModelPurchaseLines
        {
            get
            {
                List<object> model = new List<object>();
                List<PurchaseLine> PurchaseLines = (List<PurchaseLine>)Session["PurchaseLines"];
                if (PurchaseLines != null && PurchaseLines.Count > 0)
                {
                    foreach (PurchaseLine pl in PurchaseLines)
                    {
                        //Si toutes les lignes de la ligne d'achat ont déjà été retournées, on ne l'ajoute pas dans le tableau des PurchaseLines
                        if (_supplierReturnRepository.IsAllLineReturn(pl) == true) continue;

                        Purchase purchase = db.Purchases.Find(pl.PurchaseID);
                        model.Add(
                                new
                                {
                                    PurchaseLineID = pl.LineID,
                                    ProductLabel = pl.Product.GetProductCode(),
                                    LineUnitPrice = pl.LineUnitPrice,
                                    LineQuantity = PurchaseLineReturnAbleQuantity(pl),
                                    LineAmount = pl.LineQuantity * pl.LineUnitPrice,
                                    ReturnPrice = Util.ExtraPrices(pl.LineQuantity * pl.LineUnitPrice, purchase.RateReduction, purchase.RateDiscount, purchase.Transport, purchase.VatRate)
                                }
                              );
                    }
                }
                return model;
            }
        }
        [HttpPost]
        public StoreResult PurchaseLines()
        {
            return this.Store(ModelPurchaseLines);
        }
        private List<object> ModelSupplierReturnLines
        {
            get
            {
                List<object> model = new List<object>();
                List<SupplierReturnLine> SupplierReturnLines = (List<SupplierReturnLine>)Session["SupplierReturnLines"];
                if (SupplierReturnLines != null && SupplierReturnLines.Count > 0)
                {
                    SupplierReturnLines.ToList().ForEach(srl =>
                    {
                        PurchaseLine line = db.PurchaseLines.Find(srl.PurchaseLineID);
                        model.Add(
                                new
                                {
                                    TMPID = srl.TMPID,
                                    PurchaseLineID = srl.PurchaseLineID,
                                    Product = line.Product.GetProductCode(),
                                    LineQuantity = srl.LineQuantity,
                                    LineUnitPrice = line.LineUnitPrice,
                                    SupplierReturnCauses = srl.SupplierReturnCauses,
                                    LineAmount = srl.LineQuantity * line.LineUnitPrice
                                }
                              );
                    });
                }
                return model;
            }
        }
        [HttpPost]
        public StoreResult SupplierReturnLines()
        {
            //we take Purchase and her PurchaseLines
            return this.Store(ModelSupplierReturnLines);
        }

        /// <summary>
        /// 
        /// This method that called when user save changes
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReturnPurchase(SupplierReturn supplierReturn)
        {
            List<SupplierReturnLine> SupplierReturnLines = (List<SupplierReturnLine>)Session["SupplierReturnLines"];
            supplierReturn.SupplierReturnLines = SupplierReturnLines;
            try
            {
                _supplierReturnRepository.ReturnPurchase(supplierReturn);

                this.AlertSucces(Resources.Success, "Return was done successfuly");
                return this.ResetReturn();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Erreur", e.Message).Show();
                return this.Direct();
            }
        }

        //This method reset All forms and reloads all gridpanels
        public DirectResult ResetReturn()
        {

            Session["SupplierReturnLines"] = new List<SupplierReturnLine>();
            Session["PurchaseLines"] = new List<PurchaseLine>();
            this.GetCmp<FormPanel>("PurchaseGeneralInformation").Reset(true);
            this.GetCmp<Store>("PurchaseListStore").Reload();
            SimpleReset2();
            return this.Direct();
        }
        /// <summary>
        /// Liste des ventes donc la garantie court encore et dont tous les éléments de la liste n'ont pas encore été retournés
        /// </summary>
        private List<object> ModelReturnAblePurchases
        {
            get
            {
                List<object> model = new List<object>();
                BusinessDay currentBD = _busDayRepo.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();
                List<int> userBranchIds = new List<int>();
                //Nous mettons tous les ids des branchs appartenant à l'utilisateur courant dans la liste précédament crée 
                db.UserBranches.Where(ub => ub.UserID == SessionGlobalPersonID).ToList().ForEach(ub2 => userBranchIds.Add(ub2.BranchID));
                //listes des achats passés dans les agences de l'utilisateur courant et dont la garantie court toujours
               
                //peu sublir un retour les ventes ayant au moins 30 jous
                List<Purchase> allPurchases = db.Purchases.Where(s => userBranchIds.Contains(s.BranchID) && (currentBD.BDDateOperation - s.PurchaseDate).TotalDays <= 30).ToList();
                
                ////Il faut enlever tous les achats contenant au moins un produit qui est dans un dossier d'inventaire ouvers ou en cours
                //List<Product> lockedProducts = _invDirRepo.LockedProducts();
                //allPurchases.RemoveAll(s => s.PurchaseLines.Select(sl => sl.Product).Any(p => lockedProducts.Contains(p)));

                //IL faut exclure tous les produits qui sont dans un dossier d'inventaire ayant le statut ouvers ou en cours
                List<InventoryDirectoryLine> lockedProducts = _invDirRepo.LockedInventoryDirectoryLines();
                List<int> productIds = new List<int>();
                List<int> locationIds = new List<int>();

                foreach (InventoryDirectoryLine idl in lockedProducts)
                {
                    productIds.Add(idl.ProductID);
                    locationIds.Add(idl.LocalizationID);
                }

                allPurchases.RemoveAll(s => s.PurchaseLines.Any(pl => productIds.Contains(pl.ProductID) && locationIds.Contains(pl.LocalizationID)));

                //il faut mainteant vérifier si l'achat a encore au moins une ligne d'achat pouvant faire l'objet d'un retour
                foreach (Purchase p in allPurchases)
                {
                    if (_supplierReturnRepository.IsPurchaseCanBeReturn(p) == true)
                    {
                        model.Add(
                                    new
                                    {
                                        PurchaseID = p.PurchaseID,
                                        PurchaseDate = p.PurchaseDate,
                                        PurchaseDeliveryDate = p.PurchaseDeliveryDate,
                                        SupplierEmail = p.SupplierEmail,
                                        PurchaseReference = p.PurchaseReference,
                                        TotalPriceTTC = Util.ExtraPrices(p.PurchaseLines.Select(sl => sl.LineAmount).Sum(), p.RateReduction, p.RateDiscount, p.Transport, p.VatRate).TotalTTC,
                                        AdressPhoneNumber = p.SupplierPhoneNumber,
                                        PersonName = p.Supplier.SupplierFullName
                                    }
                                  );
                    }
                }
                return model;
            }
        }
        [HttpPost]
        public StoreResult ReturnAblePurchases()
        {
            return this.Store(ModelReturnAblePurchases);
        }

        [HttpPost]
        public ActionResult AddSupplierReturnLine(SupplierReturnLine srLine, double Transport)
        {

            PurchaseLine selectedPurchaseLine = db.PurchaseLines.Find(srLine.PurchaseLineID);
            srLine.ProductID = selectedPurchaseLine.ProductID;
            srLine.LocalizationID = selectedPurchaseLine.LocalizationID;
            List<SupplierReturnLine> SupplierReturnLines = (List<SupplierReturnLine>)Session["SupplierReturnLines"];


            //il s'agit d'une modification alors on fait un drop and create
            if (srLine.TMPID > 0)
            {
                this.RemoveSupRetLine(srLine.TMPID, Transport);
            }
            //A la sortie du if, le contenu de la session pourra être modifiée
            SupplierReturnLines = (List<SupplierReturnLine>)Session["SupplierReturnLines"];

            //alors la variable de session n'était pas vide
            if (SupplierReturnLines != null && SupplierReturnLines.Count > 0)
            {
                //c'est un nouvel ajout dans le panier
                if (srLine.TMPID == 0)
                {
                    //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                    SupplierReturnLine existing = SupplierReturnLines.SingleOrDefault(pl => pl.ProductID == srLine.ProductID && pl.LocalizationID == srLine.LocalizationID);

                    if (existing != null && existing.TMPID > 0)
                    {
                        //la quantité est la somme des deux quantité
                        srLine.LineQuantity += existing.LineQuantity;
                        //le prix c'est le prix de la nouvelle ligne
                        //l'id c'est l'id de la ligne existante
                        srLine.TMPID = existing.TMPID;
                        srLine.SupplierReturnLineID = existing.SupplierReturnLineID;
                        //on retire l'ancien pour ajouter le nouveau
                        SupplierReturnLines.Remove(existing);
                    }

                    if (existing == null || existing.TMPID == 0)
                    {
                        srLine.TMPID = SupplierReturnLines.Select(pl => pl.TMPID).Max() + 1;
                    }
                }
                SupplierReturnLines.Add(srLine);
            }

            //alors la variable de session était vide
            if (SupplierReturnLines == null || SupplierReturnLines.Count == 0)
            {
                //c'est bon pour la création mais pas pour les modifications
                SupplierReturnLines = new List<SupplierReturnLine>();
                if (srLine.TMPID == 0)
                {
                    srLine.TMPID = 1;
                }
                SupplierReturnLines.Add(srLine);
            }



            Session["SupplierReturnLines"] = SupplierReturnLines;
            this.ManageCady();
            Purchase selectedPurchase = db.Purchases.Find(selectedPurchaseLine.PurchaseID);

            ApplyExtraPrices(SupplierReturnLines, selectedPurchase.RateReduction, selectedPurchase.RateDiscount, Transport, selectedPurchase.Transport, selectedPurchase.VatRate);

            //il faut aussi reduire la quantité de la ligne de vente qui a été affectée
            List<PurchaseLine> PurchaseLines = (List<PurchaseLine>)Session["PurchaseLines"];
            if (PurchaseLines != null && PurchaseLines.Count > 0)
            {
                PurchaseLine reducedSL = PurchaseLines.SingleOrDefault(sl => sl.LineID == srLine.PurchaseLineID);
                PurchaseLines.Remove(reducedSL);
                //C'est la quantité originale de la ligne car la quantité dans le cady pourrais etre poluée
                reducedSL.LineQuantity = selectedPurchaseLine.LineQuantity - srLine.LineQuantity;

                if (reducedSL.LineQuantity > 0)
                {
                    PurchaseLines.Add(reducedSL);
                }
                Session["PurchaseLines"] = PurchaseLines;
            }


            return this.Reset2();

        }

        public void ApplyExtraPrices(List<SupplierReturnLine> SupplierReturnLines, double reduction, double discount, double transport, double maxTransport, double vatRate)
        {

            double valueOperation = 0;

            if ((SupplierReturnLines != null && SupplierReturnLines.Count > 0))
            {
                foreach (SupplierReturnLine srl in SupplierReturnLines)
                {
                    PurchaseLine pl = db.PurchaseLines.Find(srl.PurchaseLineID);
                    valueOperation += srl.LineQuantity * pl.LineUnitPrice;
                }
            }

            ExtraPrice extra = Util.ExtraPrices(valueOperation, reduction, discount, transport, vatRate);

            this.GetCmp<NumberField>("InitialHT").Value = valueOperation;
            this.GetCmp<NumberField>("DiscountAmount").Value = extra.DiscountAmount;
            this.GetCmp<NumberField>("Discount").Value = discount;
            this.GetCmp<NumberField>("NetCom").Value = extra.NetCom;
            this.GetCmp<NumberField>("ReductionAmount").Value = extra.ReductionAmount;
            this.GetCmp<NumberField>("Reduction").Value = reduction;
            this.GetCmp<NumberField>("TotalPriceHT").Value = extra.NetFinan;
            this.GetCmp<NumberField>("TVAAmount").Value = extra.TVAAmount;
            this.GetCmp<NumberField>("VatRate").Value = vatRate;
            this.GetCmp<NumberField>("TotalPriceTTC").Value = extra.TotalTTC;
            this.GetCmp<NumberField>("InitialTTC").Value = extra.TotalTTC;
            this.GetCmp<NumberField>("Transport").Value = transport;
            this.GetCmp<NumberField>("Transport").MaxValue = maxTransport;



        }

        [HttpPost]
        public ActionResult Reset2()
        {
            SimpleReset2();
            return this.Direct();
        }

        public void SimpleReset2()
        {
            this.GetCmp<FormPanel>("FormAddSupplierReturnLine").Reset(true);

            this.GetCmp<Store>("PurchaseLineStore").Reload();

            this.GetCmp<Store>("SupplierReturnLinesStore").Reload();

            ManageCady();
        }

        public void RemoveSupRetLine(int TMPID, double Transport)
        {
            //lors de la création
            List<SupplierReturnLine> SupplierReturnLines = (List<SupplierReturnLine>)Session["SupplierReturnLines"];

            if (SupplierReturnLines != null && SupplierReturnLines.Count > 0)
            {
                SupplierReturnLine toRemove = SupplierReturnLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                SupplierReturnLines.Remove(toRemove);
                Session["SupplierReturnLines"] = SupplierReturnLines;


                //il faut aussi augmenter la quantité de la ligne d'achat qui a été affectée
                List<PurchaseLine> PurchaseLines = (List<PurchaseLine>)Session["PurchaseLines"];
                PurchaseLine reducedSL = PurchaseLines.SingleOrDefault(sl => sl.LineID == toRemove.PurchaseLineID);

                //Début de l'augmentation de la quantité de la ligne d'achat
                if (reducedSL == null || reducedSL.LineID == 0) //la ligne d'achat n'existait plus dans la session
                {
                    reducedSL = db.PurchaseLines.Find(toRemove.PurchaseLineID);
                    if (_supplierReturnRepository.IsAllLineReturn(reducedSL) == false)
                    {
                        reducedSL.LineQuantity = this.PurchaseLineReturnAbleQuantity(reducedSL);
                    }
                }
                else //la ligne de vente existe encore dans la session
                {
                    PurchaseLines.Remove(reducedSL);
                    reducedSL.LineQuantity += toRemove.LineQuantity;
                }

                //on recalcul le montant à rembourser par le fournisseur
                Purchase selectedPurchase = db.Purchases.Find(reducedSL.PurchaseID);
                ApplyExtraPrices(SupplierReturnLines, selectedPurchase.RateReduction, selectedPurchase.RateDiscount, Transport, selectedPurchase.Transport, selectedPurchase.VatRate);

                if (reducedSL.LineQuantity > 0)
                {
                    PurchaseLines.Add(reducedSL);
                    Session["PurchaseLines"] = PurchaseLines;
                }

            }
        }

        [HttpPost]
        public ActionResult RemoveSRLine(int TMPID, double Transport = 0)
        {

            this.RemoveSupRetLine(TMPID, Transport);
            return this.Reset2();
        }

        [HttpPost]
        public ActionResult UpdateSRLine(int TMPID)
        {
            this.InitializeSRLineFieldsBySRL(TMPID);

            return this.Direct();
        }
        public void InitializeSRLineFieldsBySRL(int TMPID)
        {

            this.GetCmp<FormPanel>("FormAddSupplierReturnLine").Reset(true);
            this.GetCmp<Store>("SupplierReturnLinesStore").Reload();

            List<SupplierReturnLine> SupplierReturnLines = (List<SupplierReturnLine>)Session["SupplierReturnLines"];

            if (TMPID > 0)
            {
                SupplierReturnLine srLine = SupplierReturnLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                PurchaseLine selectedPurLine = db.PurchaseLines.Find(srLine.PurchaseLineID);

                this.GetCmp<TextField>("PurchaseLineID").SetValue(srLine.PurchaseLineID);
                this.GetCmp<TextField>("TMPID").SetValue(srLine.TMPID);

                this.GetCmp<TextField>("Product").Value = selectedPurLine.Product.GetProductCode();
                this.GetCmp<TextField>("Localization").SetValue(selectedPurLine.Localization.LocalizationLabel);
                this.GetCmp<NumberField>("LineQuantity").SetValue(srLine.LineQuantity);
                this.GetCmp<NumberField>("LineQuantity").MaxValue = this.PurchaseLineReturnAbleQuantity(selectedPurLine);
                this.GetCmp<NumberField>("SupplierReturnCauses").SetValue(srLine.SupplierReturnCauses);

            }

            ManageCady();

        }
        public void ManageCady()
        {
            List<SupplierReturnLine> SupplierReturnLines = (List<SupplierReturnLine>)Session["SupplierReturnLines"];

            if (SupplierReturnLines != null && SupplierReturnLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);//faux
                this.GetCmp<Button>("SaveReturn").Disabled = false;
            }
            if (SupplierReturnLines == null || SupplierReturnLines.Count == 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(1);//vrai
                this.GetCmp<Button>("SaveReturn").Disabled = true;

            }
        }
    }

}