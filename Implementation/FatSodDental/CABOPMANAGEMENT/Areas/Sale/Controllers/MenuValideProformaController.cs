using FastSod.Utilities.Util;
using FatSod.DataContext.Initializer;
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
using System.Web.Configuration;

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class MenuValideProformaController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Sale/MenuValideProforma";
        private const string VIEW_NAME = "Index";
             
        private IBusinessDay _busDayRepo;

        private ICustomerOrder _customerOrderRepository;
       
        //private List<BusinessDay> lstBusDay;
        
        //private BusinessDay businessDay;
        
        public MenuValideProformaController(
            IBusinessDay busDayRepo, 
            ICustomerOrder customerOrderRepository
        )
        {
            this._busDayRepo = busDayRepo;
            this._customerOrderRepository = customerOrderRepository;
        }
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
            ViewBag.DisplayForm = 1;
            try
            {
                
           
                List<BusinessDay> bdDay = (List<BusinessDay>)Session["UserBusDays"];
                if (bdDay == null)
                {
                    bdDay = _busDayRepo.GetOpenedBusinessDay(CurrentUser);
                }
                if (bdDay.Count() > 1)
                {
                    TempData["Message"] = "Wrong Business day.<br/>contact our administrator for this purpose<code/>.";
                    ViewBag.Disabled = false;
                    ViewBag.DisplayForm = 0;
                }
                Session["businessDay"] = bdDay.FirstOrDefault();
                DateTime currentDateOp = bdDay.FirstOrDefault().BDDateOperation;
                ViewBag.CurrentBranch = bdDay.FirstOrDefault().BranchID;
                ViewBag.BusnessDayDate = bdDay.FirstOrDefault().BDDateOperation.ToString("yyyy-MM-dd");// businessDay.BDDateOperation;

                ViewBag.currentcompany = WebConfigurationManager.AppSettings["AppNameP"];

                Session["BusnessDayDate"] =currentDateOp;
                int deviseID = (Session["DefaultDeviseID"] == null) ? 0 : (int)Session["DefaultDeviseID"];
                if (deviseID <= 0)
                {
                    InjectUserConfigInSession();
                }

                return View(/*PendingCustomerSale(currentDateOp, currentDateOp)*/);
            }

            catch (Exception e)
            {
                TempData["Message"] = "Error " + e.Message;
                ViewBag.DisplayForm = 0;
                return this.View();
            }
        }

        //[HttpPost]
        public JsonResult GenerateBill(int Assureur)
        {
            List<object> list = new List<object>();
            int numerofacture = 0;
            Assureur assure = db.Assureurs.Find(Assureur);
            if (assure!=null)
            {
                numerofacture = assure.CompteurFacture;
            }
            string Numerofacture =_customerOrderRepository.generateBill(numerofacture);
            list.Add(
            new {
                NumeroFacture = Numerofacture
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //retourne les ventes non delivre
        private List<CustomerOrder> ModelReturnAbleCommands()
        {

            //double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            List<CustomerOrder> model = new List<CustomerOrder>();
            
            //il faut mainteant vérifier si la vente à encore au moins une ligne de vente pouvant faire l'objet d'un retour
            foreach (CustomerOrder command in db.CustomerOrders.Where(sa => !sa.IsDelivered).ToList())
            {
                SaleTotalPrice = Util.ExtraPrices(command.CustomerOrderLines.Select(sl => sl.LineAmount).Sum(), command.RateReduction, command.RateDiscount, command.Transport, command.VatRate).TotalTTC;

                Remainder = SaleTotalPrice;// - Advanced;
                model.Add(
                    new CustomerOrder
                    {
                        CustomerOrderID = command.CustomerOrderID,
                        CustomerOrderDate = command.CustomerOrderDate,
                        CustomerName = command.CustomerName,
                        CustomerOrderNumber = command.CustomerOrderNumber,
                        SaleTotalPrice = SaleTotalPrice,
                        Remainder=Remainder,
                        Remarque = command.Remarque,
                        MedecinTraitant = command.MedecinTraitant,
                        InsurreName=command.InsurreName
                    }
                    );
            }


            return model;
        }
        

        
        public JsonResult Delete(int CustomerOrderID)
        {
            bool status = false;
            string Message = "";
            try
            {
                db.CustomerOrderLines.Where(ol => ol.CustomerOrderID == CustomerOrderID).ToList().ForEach(ol =>
                {
                    db.CustomerOrderLines.Remove(ol);
                    db.SaveChanges();
                });
                //_customerOrderRepository.Delete(ID);
                db.CustomerOrders.Remove(db.CustomerOrders.Find(CustomerOrderID));
                db.SaveChanges();
                status = true;
                Message = Resources.Success+ " Proforma has been deleted";
            }
            catch (Exception e)
            {
                Message = "Error " + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };
        }


        public JsonResult getListofDepositLocations()
        {
            List<LieuxdeDepotBordero> lieuxDeptBord = db.LieuxdeDepotBorderos.ToList();
            List<object> lieuxDeptBordList = new List<object>();
            foreach (LieuxdeDepotBordero ldb in lieuxDeptBord)
            {
                lieuxDeptBordList.Add(new { Name = ldb.LieuxdeDepotBorderoName, ID = ldb.LieuxdeDepotBorderoID });
            }
            return Json(lieuxDeptBordList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InitializeFields(int CustomerOrderID)
        {
            
            double Advanced = 0d;
            double Remainder = 0d;
            double SaleTotalPrice = 0d;
            List<object> list = new List<object>();
            if (CustomerOrderID>0)
            {
                //we take sale and her salelines
                CustomerOrder customerOrder = db.CustomerOrders.Find(CustomerOrderID);
                

                if (customerOrder.CustomerOrderLines == null || customerOrder.CustomerOrderLines.Count <= 0)
                { SaleTotalPrice = 0; }
                else { SaleTotalPrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Select(sl => sl.LineAmount).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC; }

                Remainder = SaleTotalPrice - Advanced;

                double LensPrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Where(l => l.Product.Category.CategoryID != 1 && l.Product.Category.CategoryID != 2).Select(sl => sl.LineAmount).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC;
                double MonturePrice = Util.ExtraPrices(customerOrder.CustomerOrderLines.Where(l => l.Product.Category.CategoryID == 1).Select(sl => sl.LineAmount).Sum(), customerOrder.RateReduction, customerOrder.RateDiscount, customerOrder.Transport, customerOrder.VatRate).TotalTTC;

                double partAssuVerre = (customerOrder.VerreAssurance == null) ? 0 : customerOrder.VerreAssurance;
                double partAssMonture = (customerOrder.MontureAssurance == null) ? 0 : customerOrder.MontureAssurance;
                double plafond = (customerOrder.Plafond == null) ? 0 : customerOrder.Plafond; //; partAssMonture + partAssuVerre;

                list.Add(
                    new {
                        CustomerOrderID = customerOrder.CustomerOrderID,
                        BranchID = customerOrder.BranchID,
                        CustomerName = customerOrder.CustomerName,
                        InsurreName=(customerOrder.InsurreName!=null) ? customerOrder.InsurreName : "",
                        CompanyName = customerOrder.CompanyName,
                        AssureurID = customerOrder.AssureurID,
                        PoliceAssurance =(customerOrder.PoliceAssurance!=null) ? customerOrder.PoliceAssurance : "",
                        CompteurFacture = customerOrder.CompteurFacture,
                        PhoneNumber =(customerOrder.PhoneNumber!=null) ? customerOrder.PhoneNumber : "",
                        Remarque = customerOrder.Remarque,
                        MedecinTraitant = customerOrder.MedecinTraitant,
                        NumeroBonPriseEnCharge =(customerOrder.NumeroBonPriseEnCharge!=null) ? customerOrder.NumeroBonPriseEnCharge : "",
                        CustomerOrderNumber = customerOrder.CustomerOrderNumber,
                        IsDetail = customerOrder.DatailBill,
                        Debt = Remainder,
                        DeviseID = customerOrder.DeviseID,
                        Verres = LensPrice,
                        Monture = MonturePrice,
                        VerreAssurance = partAssuVerre,
                        MontureAssurance = partAssMonture,
                        Plafond = plafond,
                        LieuxdeDepotBorderoID=(customerOrder.LieuxdeDepotBorderoID.HasValue && customerOrder.LieuxdeDepotBorderoID!=null) ? customerOrder.LieuxdeDepotBorderoID.Value : 0,
                        GestionnaireID = customerOrder.GestionnaireID,
                        SellerID=customerOrder.SellerID,
                        InsuredCompanyID=customerOrder.InsuredCompanyID,
                        CustomerID = customerOrder.CustomerID
                    });
            }
            
            Session["Receipt_CustomerOrderID"] = CustomerOrderID;
            Session["Receipt_CommandID"] = CustomerOrderID;

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadMatricule(int AssureurID)
        {
            List<object> _InfoList = new List<object>();
            Assureur assure = (from ass in db.Assureurs
                                where ass.GlobalPersonID == AssureurID
                                select ass).SingleOrDefault();
            if (assure != null)
            {
                _InfoList.Add(new
                {
                    Matricule = assure.Matricule
                });
            }
            return Json(_InfoList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApplyInsurancePrices(double Verres, double Monture, double Autre,double lensInsPartprice,double frameInsPartprice,double otherInsPartPrice)
        {
            double  valueLens = Verres;
            double valueFrame = Monture;
            double valueOther = Autre;

        
            double lensCustPartprice = 0;
            double frameCustPartprice = 0;
            double otherCustPartPrice = 0;

            List<object> list = new List<object>();
            //distribution des parts           

            lensCustPartprice =valueLens-lensInsPartprice;
            frameCustPartprice=valueFrame-frameInsPartprice;
            otherCustPartPrice=valueOther-otherInsPartPrice;     
        
            list.Add(new {
                VerreAssurance= lensInsPartprice,
                MontureAssurance = frameInsPartprice,
                AutreAssurance = otherInsPartPrice,
                VerreMalade= lensCustPartprice,
                MontureMalade = frameCustPartprice,
                AutreMalade = otherCustPartPrice
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        

        public JsonResult ValidateProforma(CustomerOrder currentOrder,string heureVente, int spray = 0, int boitier = 0)
        {
            bool status = false;
            string Message = "";
            try
            {
                currentOrder.VatRate = CodeValue.Accounting.ParamInitAcct.VATRATE;
                currentOrder.IsDelivered = true;
                currentOrder.BillState = StatutFacture.Validated;


                //Nom de l'employé de DBOY qui à saisie la commande
                //currentOrder.Operator = this.CurrentUser;
                currentOrder.OperatorID = this.SessionGlobalPersonID;
                currentOrder.ValidateBillDate = SessionBusinessDay(null).BDDateOperation;

                // je calcule le plafond avec la remise
                var PlafondWithRemiseAmount = currentOrder.Plafond; // - (currentOrder.RemiseAssurance * currentOrder.Plafond) / 100;
                currentOrder.Plafond = PlafondWithRemiseAmount;
                if (currentOrder.CustomerOrderID > 0)
                {
                    List<CustomerOrderLine> newsCustomerOrderLine = db.CustomerOrderLines.Where(col => col.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                    _customerOrderRepository.UpdateCustomerOrder(currentOrder,heureVente, newsCustomerOrderLine, SessionGlobalPersonID,spray,boitier);
                    statusOperation = Resources.CmdUpdated;
                }
                else
                {
                    status = false;
                    Message = "Error  - Select Customer before validate";
                    return new JsonResult { Data = new { status = status, Message = Message } };
                }
                status = true;
                Message = Resources.Success + " " + statusOperation;
               
            }
            catch (Exception e) {
                Message = "Error " + e.Message;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = Message } };


        }
        
        
        public JsonResult PendingCustomerSale(DateTime BeginDate, DateTime EndDate)
        {
            var model = new
            {
                data = from command in db.CustomerOrders.Where(sa => (sa.CustomerOrderDate>= BeginDate && sa.CustomerOrderDate <= EndDate) && sa.BillState == StatutFacture.Proforma).ToList() // this.ModelReturnAbleCommands()
                select new
                {
                    CustomerOrderID = command.CustomerOrderID,
                    CustomerOrderDate = command.CustomerOrderDate.ToString("yyyy-MM-dd"),
                    InsurreName=command.InsurreName,
                    CustomerFullName = command.CustomerName,
                    CustomerOrderNumber = command.CustomerOrderNumber,
                    SaleTotalPrice = Util.ExtraPrices(command.CustomerOrderLines.Select(sl => sl.LineAmount).Sum(), command.RateReduction, command.RateDiscount, command.Transport, command.VatRate).TotalTTC , //command.SaleTotalPrice,
                    //Remainder = command.Remainder,
                    Remarque = command.Remarque,
                    MedecinTraitant = command.MedecinTraitant,
                    Insurance = command.Assureur.Name
                }
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult populateAssurance()
        {
            List<Assureur> assureur = db.Assureurs.Where(ass => ass.Name.ToLower() != "default").OrderBy(ass =>ass.Name.Trim()).ToList();
            List<object> assureurList = new List<object>();
            foreach (Assureur ass in assureur)
            {
                assureurList.Add(new { Name=ass.Name, ID=ass.GlobalPersonID });
            }
            return Json(assureurList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRemiseOfCurrentAssurence( int AssureurID)
        {
            Assureur assureur = db.Assureurs.FirstOrDefault(ass => ass.GlobalPersonID ==AssureurID );
        
            return Json(new { Remise = assureur.Remise}, JsonRequestBehavior.AllowGet);
        }

        /*
        public void GenerateBorderoDepotLunette()
        {
            List<object> model = new List<object>();
            List<object> modelsubRpt = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
                int customerOrderID = (Session["Receipt_CustomerOrderID"] == null) ? 0 : (int)Session["Receipt_CustomerOrderID"];

                string repName = "";
                bool isValid = false;
              

                string path = "";
                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

                string TitleDeposit = "";
                string RptTitle = "";

                Company cmpny = db.Companies.FirstOrDefault();
                if (customerOrderID > 0)//depot pour une vente
                {

                    CustomerOrder currentOrder = db.CustomerOrders.Find(customerOrderID);

                    string Prescription = "";
                    //recuperation des versements
                    List<CustomerOrderLine> lstOrderLine = db.CustomerOrderLines.Where(sl => sl.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                   
                    foreach (CustomerOrderLine custOrderLine in lstOrderLine)
                    {
                        string labelFrame = (custOrderLine.marque != null && custOrderLine.reference != null) ? Resources.Marque.ToUpper() + " " + custOrderLine.marque + " " + Resources.Reference.ToUpper() + " " + custOrderLine.reference : "";
                        int i = (custOrderLine.marque != null && custOrderLine.reference != null) ? 2 : 1;
                        if (labelFrame.Trim().Length > 0)
                        {
                            Prescription = labelFrame;
                        }
                        else
                        {
                            if (custOrderLine.Product.Prescription != null)
                            {
                                Prescription = custOrderLine.Product.Prescription;
                            }
                            else
                            {
                                Prescription = (custOrderLine.Product.ProductCode.Contains(" HD ")) ? custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode + " HD", "") : custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode, "");
                            }
                        }
                        model.Add(
                        new //RptReceipt
                        {
                            RptReceiptID = 1,
                            ReceiveAmount = (labelFrame.Trim().Length > 0) ? currentOrder.MontureAssurance : Math.Round(currentOrder.VerreAssurance / 2), //montant du verre TotalReceiveAmount, //part assureur
                            TotalAmount = 0, //montant restant de la facture
                            LineUnitPrice = (labelFrame.Trim().Length > 0) ? currentOrder.MontureAssurance : Math.Round(currentOrder.VerreAssurance / 2), //montant du verre
                            CompanyName = cmpny.Adress.AdressFullName,
                            CompanyAdress = "B.P. " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email:" + cmpny.Adress.AdressEmail,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                            CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + ", Fax: " + cmpny.Adress.AdressFax + ", Cell: " + cmpny.Adress.AdressCellNumber,
                            CustomerAdress = currentOrder.NumeroBonPriseEnCharge,// "No.ONOC:87 / ONOC",
                            BranchName = currentOrder.Branch.BranchName,
                            BranchAdress = currentOrder.Branch.Adress.Quarter.QuarterLabel + " - " + currentOrder.Branch.Adress.Quarter.Town.TownLabel,
                            BranchTel = "Tel: " + currentOrder.Branch.Adress.AdressPhoneNumber,
                            Reference = currentOrder.NumeroFacture,
                            CompanyCNI = "NO CONT : " + cmpny.CNI,
                            Operator = CurrentUser.Name + " " + CurrentUser.Description,
                            CustomerName = currentOrder.CustomerName,// customerRpt.Name + " " + customerRpt.Description,
                            ProductLabel = Prescription, //(labelFrame.Trim().Length > 0) ? labelFrame : custOrderLine.Product.Prescription,
                            SaleDate = currentOrder.CustomerOrderDate,
                            Title = cmpny.Adress.Quarter.Town.TownLabel,
                            MontantLettre = "",//MONTANT FACTURE,
                            RateTVA = currentOrder.VatRate,
                            RateReduction = currentOrder.RateReduction,
                            RateDiscount = currentOrder.RateDiscount,
                            Transport = currentOrder.Transport,
                            RptReceiptPaymentDetailID = i,
                            LineQuantity = (labelFrame.Trim().Length > 0) ? custOrderLine.LineQuantity : custOrderLine.LineQuantity * 2,
                            CustomerAccount = currentOrder.Assureur.Name, //nom de la societe assureur
                            BranchAbbreviation = currentOrder.CompanyName, //NOM DE LA SOCIETE DU CLIENT
                            DeviseLabel = currentOrder.PoliceAssurance,//police d'assurance
                            ProductRef = (labelFrame.Trim().Length > 0) ? "MONTURE MATIERE :" + custOrderLine.FrameCategory : custOrderLine.Product.Category.CategoryDescription,
                            CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                        }
                    );
                        //
                    }

                    path = Server.MapPath("~/Reports/Sales/RptBorderoDepotLunette.rpt");
                    repName = "RptBorderoDepotLunette";
                    
                    isValid = true;
                }


                if (isValid)
                {
                    rptH.Load(path);
                    rptH.SetDataSource(model);

                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, repName);
                }
                else
                {
                    Response.Write("Nothing Found; No Report name found");
                }

            }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }
        //This method print a receipt of customer
        public void GenerateFacture()
        {
            List<object> model = new List<object>();
            List<object> modelsubRpt = new List<object>();
            ReportDocument rptH = new ReportDocument();
            try
            {
            int customerOrderID = (Session["Receipt_CustomerOrderID"] == null) ? 0 : (int)Session["Receipt_CustomerOrderID"];
           
            string repName = "";
            bool isValid = false;
            double totalAmount = 0d;
            double totalRemaining = 0d;

           
            string path = "";
            string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
            byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);

            string TitleDeposit = "";
            string RptTitle = "";

            Company cmpny = db.Companies.FirstOrDefault();
            if (customerOrderID > 0)//depot pour une vente
            {

                double saleAmount = 0d;
                CustomerOrder currentOrder = db.CustomerOrders.Find(customerOrderID);

                string Prescription = "";
                //recuperation des versements
                List<CustomerOrderLine> lstOrderLine = db.CustomerOrderLines.Where(sl => sl.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                //totalAmount = (lstOrderLine.Count > 0) ? Util.ExtraPrices(lstOrderLine.Select(c => c.LineUnitPrice).Sum(), currentOrder.RateReduction, currentOrder.RateDiscount, currentOrder.Transport, currentOrder.VatRate).TotalTTC : 0; //montant du verre
                //totalRemaining = totalAmount - TotalReceiveAmount;
                totalAmount = currentOrder.Plafond; // currentOrder.MontureAssurance + currentOrder.VerreAssurance;
                string montantLettre = NumberConverter.Spell((ulong)totalAmount).ToUpper();
                foreach (CustomerOrderLine custOrderLine in lstOrderLine)
                {
                    string labelFrame = (custOrderLine.marque != null && custOrderLine.reference != null) ? Resources.Marque.ToUpper() + " " + custOrderLine.marque + " " + Resources.Reference.ToUpper() + " " + custOrderLine.reference : "";
                    int i = (custOrderLine.marque != null && custOrderLine.reference != null) ? 2 : 1;
                    if (labelFrame.Trim().Length > 0)
                    {
                        Prescription = labelFrame;
                    }
                    else
                    {
                        if (custOrderLine.Product.Prescription!=null)
                        {
                            Prescription = custOrderLine.Product.Prescription;
                        }
                        else
                        {
                            Prescription = (custOrderLine.Product.ProductCode.Contains(" HD ")) ? custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode + " HD", "") : custOrderLine.Product.ProductCode.Replace(custOrderLine.Product.Category.CategoryCode, "");
                        }
                    }
                    model.Add(
                    new //RptReceipt
                    {
                        RptReceiptID = 1,
                        ReceiveAmount = (labelFrame.Trim().Length > 0) ? currentOrder.MontureAssurance : Math.Round(currentOrder.VerreAssurance / 2), //montant du verre TotalReceiveAmount, //part assureur
                        TotalAmount = totalAmount, //montant restant de la facture
                        LineUnitPrice = (labelFrame.Trim().Length > 0) ? currentOrder.MontureAssurance : Math.Round(currentOrder.VerreAssurance / 2), //montant du verre
                        CompanyName = cmpny.Adress.AdressFullName,
                        CompanyAdress = "B.P. " + cmpny.Adress.AdressPOBox + ", " + cmpny.Adress.Quarter.Town.TownLabel + ", Email:" + cmpny.Adress.AdressEmail,// "Head Quater : " + cmpny.Adress.Quarter.Town.Region.RegionLabel + " - " + cmpny.Adress.Quarter.Town.TownLabel,
                        CompanyTel = "Tel: " + cmpny.Adress.AdressPhoneNumber + ", Fax: " + cmpny.Adress.AdressFax + ", Cell: " + cmpny.Adress.AdressCellNumber,
                        CustomerAdress = currentOrder.NumeroBonPriseEnCharge,// "No.ONOC:87 / ONOC",
                        BranchName = currentOrder.Branch.BranchName,
                        BranchAdress = currentOrder.Branch.Adress.Quarter.QuarterLabel + " - " + currentOrder.Branch.Adress.Quarter.Town.TownLabel,
                        BranchTel = "Tel: " + currentOrder.Branch.Adress.AdressPhoneNumber,
                        Reference = currentOrder.NumeroFacture,
                        CompanyCNI = "NO CONT : " + cmpny.CNI,
                        Operator = CurrentUser.Name + " " + CurrentUser.Description,
                        CustomerName = currentOrder.CustomerName,// customerRpt.Name + " " + customerRpt.Description,
                        ProductLabel = Prescription, //(labelFrame.Trim().Length > 0) ? labelFrame : custOrderLine.Product.Prescription,
                        SaleDate = currentOrder.CustomerOrderDate,
                        Title = cmpny.Adress.Quarter.Town.TownLabel,
                        MontantLettre = montantLettre,//MONTANT FACTURE,
                        RateTVA = currentOrder.VatRate,
                        RateReduction = currentOrder.RateReduction,
                        RateDiscount = currentOrder.RateDiscount,
                        Transport = currentOrder.Transport,
                        RptReceiptPaymentDetailID = i,
                        LineQuantity = (labelFrame.Trim().Length > 0) ? custOrderLine.LineQuantity : custOrderLine.LineQuantity * 2,
                        CustomerAccount = currentOrder.Assureur.Name, //nom de la societe assureur
                        BranchAbbreviation = currentOrder.CompanyName, //NOM DE LA SOCIETE DU CLIENT
                        DeviseLabel = currentOrder.PoliceAssurance,//police d'assurance
                        ProductRef = (labelFrame.Trim().Length > 0) ? "MONTURE MATIERE :" + custOrderLine.FrameCategory : custOrderLine.Product.Category.CategoryDescription,
                        CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == cmpny.GlobalPersonID).Content : COMPANY_LOGO
                    }
                );
                    //
                }

                if (currentOrder.DatailBill==0)
                {
                    path = Server.MapPath("~/Reports/CashRegister/RptFactureAssurance.rpt");
                    repName = "RptFactureAssurance";
                }
                else
                {
                    path = Server.MapPath("~/Reports/CashRegister/RptFactureAssuranceDetail.rpt");
                    repName = "RptFactureAssuranceDetail";
                }
                

                isValid = true;
            }


            if (isValid)
            {
                FatSod.Ressources.Resources.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("fr");
                rptH.Load(path);
                rptH.SetDataSource(model);
               
                //rptH.SetParameterValue("RptTitle", RptTitle);
                bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, isDownloadRpt, repName);
                //return File(stream, "application/pdf");
            }
            else
            {
                Response.Write("Nothing Found; No Report name found");
            }
        
        }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
    }*/

    }
}