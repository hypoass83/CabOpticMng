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
using SaleE = FatSod.Supply.Entities.Sale;
using FatSod.Supply.Abstracts;
using FatSod.Ressources;
using System.Threading.Tasks;
using FatSodDental.UI.Filters;
using FastSod.Utilities.Util;
using FatSod.DataContext.Concrete;
using System.Data.Entity;
using System.Web.UI;
using ExtPartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace FatSodDental.UI.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class SaleReturnController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/SaleReturn";
        private const string VIEW_NAME = "Index";
        
        private ICustomerReturn _customerReturnRepository;

        private IBusinessDay _busDayRepo;
        private IDeposit _depositRepository;

        private List<BusinessDay> listBDUser;

        public SaleReturnController(
            IDeposit depositRepo,
            ICustomerReturn customerReturnRepository,
            IBusinessDay busDayRepo
            )
        {
            this._customerReturnRepository = customerReturnRepository;
            this._busDayRepo = busDayRepo;
            this._depositRepository = depositRepo;
        }
        // GET: Sale/SaleReturn
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            //We test if this user has an autorization to get this sub menu
            //if (!LoadAction.isAutoriseToGetSubMenu(SessionProfileID, CodeValue.Sale.NewSale.R_CODE, db))
            //{
            //    RedirectToAction("NotAuthorized", "User");
            //}

            Session["Curent_Page"] = VIEW_NAME;
            Session["Curent_Controller"] = CONTROLLER_NAME;
            

            listBDUser = (List<BusinessDay>)Session["UserBusDays"];

            if (listBDUser == null)
            {
                listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
            }
            DateTime BDDateOperation = listBDUser.FirstOrDefault().BDDateOperation;
            
            Session["CustomerReturnLines"] = new List<CustomerReturnLine>();
            Session["SaleLines"] = new List<SaleLine>();

            //ExtPartialViewResult rPVResult = new ExtPartialViewResult
            //{
            //    ClearContainer = true,
            //    //RenderMode = RenderMode.AddTo,
            //    ContainerId = "Body",
            //    //WrapByScriptTag = false,
            //    Model = ModelReturnAbleSales(BDDateOperation)
            //};
            this.GetCmp<DateField>("SoldDate").Value = BDDateOperation;
            //rPVResult.ViewBag.BusnessDayDate = BDDateOperation;
            return View(ModelReturnAbleSales(BDDateOperation));
        }

        /// <summary>
        /// This method allow to initialize grid panel for to updated one sale line
        /// </summary>
        /// <param name="ID">ID of SaleLine</param>
        /// <returns></returns>
        [HttpPost]
        public DirectResult InitializeCRLineFieldsBySL(int SaleLineID)
        {

            //we take sale and her salelines
            SaleLine saleLineToReduce = db.SaleLines.Find(SaleLineID);
            this.GetCmp<NumberField>("SaleLineID").Value = saleLineToReduce.LineID;
            this.GetCmp<TextField>("TMPID").SetValue(0);

            this.GetCmp<TextField>("Product").SetValue(saleLineToReduce.Product.GetProductCode());
            this.GetCmp<TextField>("Localization").SetValue(saleLineToReduce.Localization.LocalizationLabel);
            this.GetCmp<NumberField>("LineQuantity").SetValue(0);
            //Nous nous assurons que l'utilisateur ne saississe pas une quantité supérieur à celle qui peut être retournée.. 
            this.GetCmp<NumberField>("LineQuantity").MaxValue = this.SaleLineReturnAbleQuantity(saleLineToReduce);

            this.ManageCady();

            return this.Direct();
        }


        /// <summary>
        /// This method allow to initialize grid panel for to updated one sale line
        /// </summary>
        /// <param name="ID">ID of SaleLine</param>
        /// <returns></returns>
        [HttpPost]
        public DirectResult ReturnAllSaleLine(int SaleLineID)
        {

            //we take sale and her salelines
            SaleLine saleLineToReduce = db.SaleLines.Find(SaleLineID);
            this.GetCmp<NumberField>("SaleLineID").Value = saleLineToReduce.LineID;
            this.GetCmp<TextField>("TMPID").SetValue(0);

            this.GetCmp<TextField>("Product").SetValue(saleLineToReduce.Product.GetProductCode());
            this.GetCmp<TextField>("Localization").SetValue(saleLineToReduce.Localization.LocalizationLabel);

            double max = this.SaleLineReturnAbleQuantity(saleLineToReduce);

            this.GetCmp<NumberField>("LineQuantity").SetValue(max);
            //Nous nous assurons que l'utilisateur ne saississe pas une quantité supérieur à celle qui peut être retournée.. 
            this.GetCmp<NumberField>("LineQuantity").MaxValue = max;

            this.ManageCady();

            return this.Direct();
        }


        /// <summary>
        /// Cette méthode est appelée quand une vente est sélectionnée et permet de renseigner les champs de formulaire liés à la vente sélectionnée. il s'agit de :
        /// 1-Le formulaire de vente
        /// 2-le cady de la vente. 
        /// NB : Il reste à l'utilisateur de remplir le cady de retour
        /// </summary>
        /// <param name="ID"> ID de la vente sélectionnée par l'utilisateur</param>
        /// <returns></returns>
        [HttpPost]
        public DirectResult InitializeFields(int SaleID)
        {
            ResetReturn();

            //we take sale and her salelines
            SaleE selectedSale = db.Sales.Find(SaleID);
            
            List<SaleLine> allSaleSLines = db.SaleLines.Where(sl => sl.SaleID == SaleID).ToList();
            List<SaleLine> returnableSaleSLines = new List<SaleLine>();

            foreach (SaleLine sl in allSaleSLines)
            {
                if (_customerReturnRepository.IsAllLineReturn(sl) == false)
                {
                    returnableSaleSLines.Add(sl);
                }
            }

            Session["SaleLines"] = returnableSaleSLines;

            BusinessDay currentBD = _busDayRepo.GetOpenedBusinessDay(CurrentUser).FirstOrDefault();
            
            this.GetCmp<DateField>("CustomerReturnDate").Value = currentBD.BDDateOperation;
            this.GetCmp<TextField>("SaleID").Value = selectedSale.SaleID;
            this.GetCmp<ComboBox>("CustomerID").SetValue(selectedSale.CustomerID);
            this.GetCmp<DateField>("SaleDate").Value = selectedSale.SaleDate.Date;
            this.GetCmp<TextField>("SaleReceiptNumber").Value = selectedSale.SaleReceiptNumber;
            this.GetCmp<DateField>("SaleDeliveryDate").Value = selectedSale.SaleDeliveryDate;

            this.SimpleReset2();

            this.ApplyExtraPrices(null, selectedSale.RateReduction, selectedSale.RateDiscount, 0, selectedSale.Transport, selectedSale.VatRate);

            return this.Direct();
        }

        public ActionResult ReloadSalesListStore()
        {
            this.GetCmp<Store>("SalesListStore").Reload();
            return this.Direct();
        }


        /// <summary>
        /// Cette méthode est appelée quand une vente est sélectionnée et permet de renseigner les champs de formulaire liés à la vente sélectionnée. il s'agit de :
        /// 1-Le formulaire de vente
        /// 2-le cady de la vente. 
        /// NB : Il reste à l'utilisateur de remplir le cady de retour
        /// </summary>
        /// <param name="ID"> ID de la vente sélectionnée par l'utilisateur</param>
        /// <returns></returns>
        [HttpPost]
        public DirectResult ReturnAllSale(int SaleID, string CustomerReturnCauses="")
        {
            try
            {
                _customerReturnRepository.ReturnAllSale(SaleID, CustomerReturnCauses,SessionGlobalPersonID,SessionBusinessDay(null).BranchID);

                this.AlertSucces(Resources.Success, "Return was done successfuly");
                return this.ResetReturn();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Erreur", e.Message).Show();
                return this.Direct();
            }

        }

        /// <summary>
        /// Quantité de la ligne qui peut encore être retournée
        /// </summary>
        /// <param name="selectedSaleLine"></param>
        /// <returns></returns>
        public double SaleLineReturnAbleQuantity(SaleLine selectedSaleLine)
        {
            double returnedQuantity = this._customerReturnRepository.SaleLineReturnedQuantity(selectedSaleLine);
            return selectedSaleLine.LineQuantity - returnedQuantity;
        }
        private List<object> ModelSaleLines
        {
            get
            {
                List<object> model = new List<object>();
                List<SaleLine> SaleLines = (List<SaleLine>)Session["SaleLines"];
                if (SaleLines != null && SaleLines.Count > 0)
                {
                    foreach (SaleLine sl in SaleLines)
                    {
                        //Si toutes les lignes de la ligne de vente ont déjà été retournées, on ne l'ajoute pas dans le tableau des salelines
                        if (_customerReturnRepository.IsAllLineReturn(sl) == true) continue;

                        SaleE sale = db.Sales.Find(sl.SaleID);
                        model.Add(
                                new
                                {
                                    SaleLineID = sl.LineID,
                                    ProductLabel = sl.Product.GetProductCode(),
                                    LineUnitPrice = sl.LineUnitPrice,
                                    LineQuantity = SaleLineReturnAbleQuantity(sl),
                                    LineAmount = sl.LineQuantity * sl.LineUnitPrice,
                                    ReturnPrice = Util.ExtraPrices(sl.LineQuantity * sl.LineUnitPrice, sale.RateReduction, sale.RateDiscount, sale.Transport, sale.VatRate)
                                }
                              );
                    }
                }
                return model;
            }
        }
        [HttpPost]
        public StoreResult SaleLines()
        {
            return this.Store(ModelSaleLines);
        }
        private List<object> ModelCustomerReturnLines
        {
            get
            {
                List<object> model = new List<object>();
                List<CustomerReturnLine> CustomerReturnLines = (List<CustomerReturnLine>)Session["CustomerReturnLines"];
                if (CustomerReturnLines != null && CustomerReturnLines.Count > 0)
                {
                    CustomerReturnLines.ToList().ForEach(crl =>
                    {
                        SaleLine line = db.SaleLines.Find(crl.SaleLineID);
                        model.Add(
                                new
                                {
                                    TMPID = crl.TMPID,
                                    SaleLineID = crl.SaleLineID,
                                    Product = line.Product.GetProductCode(),
                                    LineQuantity = crl.LineQuantity,
                                    LineUnitPrice = line.LineUnitPrice,
                                    CustomerReturnCauses = crl.CustomerReturnCauses,
                                    LineAmount = crl.LineQuantity * line.LineUnitPrice
                                }
                              );
                    });
                }
                return model;
            }
        }
        [HttpPost]
        public StoreResult CustomerReturnLines()
        {
            //we take sale and her salelines
            return this.Store(ModelCustomerReturnLines);
        }

        /// <summary>
        /// 
        /// This method that called when user save changes
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReturnSale(CustomerReturn customerReturn)
        {
            List<CustomerReturnLine> CustomerReturnLines = (List<CustomerReturnLine>)Session["CustomerReturnLines"];
            customerReturn.CustomerReturnLines = CustomerReturnLines;
            try
            {
                _customerReturnRepository.ReturnSale(customerReturn, SessionGlobalPersonID, SessionBusinessDay(null).BranchID);

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

            Session["CustomerReturnLines"] = new List<CustomerReturnLine>();
            Session["SaleLines"] = new List<SaleLine>();
            this.GetCmp<FormPanel>("SaleGeneralInformation").Reset(true);
            this.GetCmp<Store>("SalesListStore").Reload();
            SimpleReset2();
            return this.Direct();
        }

        /// <summary>
        /// Liste des ventes donc la garantie court encore et dont tous les éléments de la liste n'ont pas encore été retournés
        /// </summary>
        private List<object> ModelReturnAbleSales(DateTime SoldDate)
            {
        
                
                List<object> model = new List<object>();
                listBDUser = (List<BusinessDay>)Session["UserBusDays"];
                if (listBDUser == null)
                {
                    listBDUser = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                DateTime BDDateOperation = listBDUser.FirstOrDefault().BDDateOperation;
                int currentBD = listBDUser.FirstOrDefault().BranchID;
                
                //retourne la liste des ventes qui ont deja ete valide
                List<SaleE> allSales = (from sal in db.Sales
                                        where (sal.BranchID == currentBD/*.BranchID && DbFunctions.DiffHours(currentBD.BDDateOperation,sal.SaleDate) <= 30*/)
                                        select sal).ToList();
                    
                //il faut mainteant vérifier si la vente à encore au moins une ligne de vente pouvant faire l'objet d'un retour
                foreach (SaleE s in allSales)
                {
                    if ((s.SaleDate.Date.Equals(SoldDate.Date)) && _customerReturnRepository.IsSaleCanBeReturn(s) == true)
                    {
                        model.Add(
                                    new
                                    {
                                        SaleID = s.SaleID,
                                        SaleDate = s.SaleDate,
                                        SaleDeliveryDate = s.SaleDeliveryDate,
                                        AdressEmail = s.AdressEmail,
                                        SaleReceiptNumber = s.SaleReceiptNumber,
                                        SaleTotalPrice = _depositRepository.SaleBill(s),//Util.ExtraPrices(s.SaleLines.Select(sl => sl.LineAmount).Sum(), s.RateReduction, s.RateDiscount, s.Transport, s.VatRate).TotalTTC,
                                        AdressPhoneNumber = s.AdressPhoneNumber,
                                        PersonName = s.Customer.CustomerFullName
                                    }
                                  );
                    }
                }
                return model;
        }
        [HttpPost]
        public StoreResult ReturnAbleSales(DateTime SoldDate)
        {
            return this.Store(ModelReturnAbleSales(SoldDate));
        }

        [HttpPost]
        public ActionResult AddCustomerReturnLine(CustomerReturnLine crLine, double Transport)
        {

            SaleLine selectedSaleLine = db.SaleLines.Find(crLine.SaleLineID);
            crLine.ProductID = selectedSaleLine.ProductID;
            crLine.LocalizationID = selectedSaleLine.LocalizationID;
            List<CustomerReturnLine> customerReturnLines = (List<CustomerReturnLine>)Session["CustomerReturnLines"];


            //il s'agit d'une modification alors on fait un drop and create
            if (crLine.TMPID > 0)
            {
                this.RemoveCustRetLine(crLine.TMPID, Transport);
            }
            //A la sortie du if, le contenu de la session pourra être modifiée
            customerReturnLines = (List<CustomerReturnLine>)Session["CustomerReturnLines"];

            //alors la variable de session n'était pas vide
            if (customerReturnLines != null && customerReturnLines.Count > 0)
            {
                //c'est un nouvel ajout dans le panier
                if (crLine.TMPID == 0)
                {
                    //existe t-il déjà une ligne de vente ayant le meme produit et le même magasin que celui en création?
                    CustomerReturnLine existing = customerReturnLines.SingleOrDefault(pl => pl.ProductID == crLine.ProductID && pl.LocalizationID == crLine.LocalizationID);

                    if (existing != null && existing.TMPID > 0)
                    {
                        //la quantité est la somme des deux quantité
                        crLine.LineQuantity += existing.LineQuantity;
                        //le prix c'est le prix de la nouvelle ligne
                        //l'id c'est l'id de la ligne existante
                        crLine.TMPID = existing.TMPID;
                        crLine.CustomerReturnLineID = existing.CustomerReturnLineID;
                        //on retire l'ancien pour ajouter le nouveau
                        customerReturnLines.Remove(existing);
                    }

                    if (existing == null || existing.TMPID == 0)
                    {
                        crLine.TMPID = customerReturnLines.Select(pl => pl.TMPID).Max() + 1;
                    }
                }
                customerReturnLines.Add(crLine);
            }

            //alors la variable de session était vide
            if (customerReturnLines == null || customerReturnLines.Count == 0)
            {
                //c'est bon pour la création mais pas pour les modifications
                customerReturnLines = new List<CustomerReturnLine>();
                if (crLine.TMPID == 0)
                {
                    crLine.TMPID = 1;
                }
                customerReturnLines.Add(crLine);
            }



            Session["CustomerReturnLines"] = customerReturnLines;
            this.ManageCady();
            SaleE selectedSale = db.Sales.Find(selectedSaleLine.SaleID);

            ApplyExtraPrices(customerReturnLines, selectedSale.RateReduction, selectedSale.RateDiscount, Transport, selectedSale.Transport, selectedSale.VatRate);

            //il faut aussi reduire la quantité de la ligne de vente qui a été affectée
            List<SaleLine> SaleLines = (List<SaleLine>)Session["SaleLines"];
            if (SaleLines != null && SaleLines.Count > 0)
            {
                SaleLine reducedSL = SaleLines.SingleOrDefault(sl => sl.LineID == crLine.SaleLineID);
                SaleLines.Remove(reducedSL);
                //C'est la quantité originale de la ligne car la quantité dans le cady pourrais etre poluée
                reducedSL.LineQuantity = selectedSaleLine.LineQuantity - crLine.LineQuantity;

                if (reducedSL.LineQuantity > 0)
                {
                    SaleLines.Add(reducedSL);
                }
                Session["SaleLines"] = SaleLines;
            }


            return this.Reset2();

        }

        public void ApplyExtraPrices(List<CustomerReturnLine> CustomerReturnLines, double reduction, double discount, double transport, double maxTransport, double vatRate )
        {

            double valueOperation = 0;

            if ((CustomerReturnLines != null && CustomerReturnLines.Count > 0))
            {
                foreach (CustomerReturnLine crl in CustomerReturnLines)
                {
                    SaleLine sl = db.SaleLines.Find(crl.SaleLineID);
                    valueOperation += crl.LineQuantity * sl.LineUnitPrice;
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
            this.GetCmp<FormPanel>("FormAddCustomerReturnLine").Reset(true);

            this.GetCmp<Store>("CustomerReturnLinesStore").Reload();

            this.GetCmp<Store>("SaleLinesStore").Reload();

            ManageCady();
        }

        public void RemoveCustRetLine(int TMPID, double Transport)
        {
            //lors de la création
            List<CustomerReturnLine> CustomerReturnLines = (List<CustomerReturnLine>)Session["CustomerReturnLines"];

            if (CustomerReturnLines != null && CustomerReturnLines.Count > 0)
            {
                CustomerReturnLine toRemove = CustomerReturnLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                CustomerReturnLines.Remove(toRemove);
                Session["CustomerReturnLines"] = CustomerReturnLines;


                //il faut aussi augmenter la quantité de la ligne de vente qui a été affectée
                List<SaleLine> SaleLines = (List<SaleLine>)Session["SaleLines"];
                SaleLine reducedSL = SaleLines.SingleOrDefault(sl => sl.LineID == toRemove.SaleLineID);

                //Début de l'augmentation de la quantité de la ligne de vente
                if (reducedSL == null || reducedSL.LineID == 0) //la ligne de vente n'existait plus dans la session
                {
                    reducedSL = db.SaleLines.Find(toRemove.SaleLineID);
                    if (_customerReturnRepository.IsAllLineReturn(reducedSL) == false)
                    {
                        reducedSL.LineQuantity = this.SaleLineReturnAbleQuantity(reducedSL);
                    }
                }
                else //la ligne de vente existe encore dans la session
                {
                    SaleLines.Remove(reducedSL);
                    reducedSL.LineQuantity += toRemove.LineQuantity;
                }

                //on recalcul le montant à rembourser au client
                SaleE selectedSale = db.Sales.Find(reducedSL.SaleID);
                ApplyExtraPrices(CustomerReturnLines, selectedSale.RateReduction, selectedSale.RateDiscount, Transport, selectedSale.Transport, selectedSale.VatRate);

                if (reducedSL.LineQuantity > 0)
                {
                    SaleLines.Add(reducedSL);
                    Session["SaleLines"] = SaleLines;
                }

            }
        }

        [HttpPost]
        public ActionResult RemoveCRLine(int TMPID, double Transport = 0)
        {

            this.RemoveCustRetLine(TMPID, Transport);
            return this.Reset2();
        }

        [HttpPost]
        public ActionResult UpdateCRLine(int TMPID)
        {
            this.InitializeCRLineFieldsByCRL(TMPID);

            return this.Direct();
        }
        public void InitializeCRLineFieldsByCRL(int TMPID)
        {

            this.GetCmp<FormPanel>("FormAddCustomerReturnLine").Reset(true);
            this.GetCmp<Store>("CustomerReturnLinesStore").Reload();

            List<CustomerReturnLine> CustomerReturnLines = (List<CustomerReturnLine>)Session["CustomerReturnLines"];

            if (TMPID > 0)
            {
                CustomerReturnLine crLine = CustomerReturnLines.SingleOrDefault(pl => pl.TMPID == TMPID);
                SaleLine saleLineToReduce = db.SaleLines.Find(crLine.SaleLineID);

                this.GetCmp<TextField>("SaleLineID").SetValue(crLine.SaleLineID);
                this.GetCmp<TextField>("TMPID").SetValue(crLine.TMPID);

                this.GetCmp<TextField>("Product").Value = saleLineToReduce.Product.GetProductCode();
                this.GetCmp<TextField>("Localization").SetValue(saleLineToReduce.Localization.LocalizationLabel);
                this.GetCmp<NumberField>("LineQuantity").SetValue(crLine.LineQuantity);
                this.GetCmp<NumberField>("LineQuantity").MaxValue = this.SaleLineReturnAbleQuantity(saleLineToReduce);
                this.GetCmp<NumberField>("CustomerReturnCauses").SetValue(crLine.CustomerReturnCauses);

            }

            ManageCady();

        }
        public void ManageCady()
        {
            List<CustomerReturnLine> CustomerReturnLines = (List<CustomerReturnLine>)Session["CustomerReturnLines"];

            if (CustomerReturnLines != null && CustomerReturnLines.Count > 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(0);//faux
                this.GetCmp<Button>("SaveReturn").Disabled = false;
            }
            if (CustomerReturnLines == null || CustomerReturnLines.Count == 0)
            {
                this.GetCmp<TextField>("IsCadyEmpty").SetValue(1);//vrai
                this.GetCmp<Button>("SaveReturn").Disabled = true;

            }
        }
    }

}