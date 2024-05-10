using Developpez.Dotnet;
using FastSod.Utilities.Util;
using FatSod.DataContext.Repositories;
using FatSod.Report.WrapReports;
using FatSod.Ressources;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using CABOPMANAGEMENT.Filters;
using CABOPMANAGEMENT.Tools;
using CABOPMANAGEMENT.ViewModel;
using CABOPMANAGEMENT.Models;

namespace CABOPMANAGEMENT.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class ReportsHomeController : BaseController
    {
        
        // GET: ReportsHome
        public ActionResult Index()
        {
            return View();
        }

        public List<MonthTotalVM> YearlySalesByMonth_forCharts(string year)
        {
            int _year = 0;
            int _toYear = 0;
            if (string.IsNullOrWhiteSpace(year))
            {
                _year = DateTime.Now.Year;
                _toYear = _year + 1;
            }
            else
            {
                _year = Convert.ToInt32(year);
                _toYear = _year + 1;
            }
            ICustomerReturn _customerReturnRepository = new CustomerReturnRepository(db);

            List<Sale> QuerySalelist = new List<Sale>();
            Sale getSalePeriode = new Sale();
            List<Sale> salelist = db.Sales.Where(s => (s.SaleDate.Year >= _year && s.SaleDate.Year < _toYear)).ToList();
            salelist.ForEach(allSales =>
            {
                getSalePeriode = _customerReturnRepository.GetRealSale(allSales);
                QuerySalelist.Add(getSalePeriode);
            });

            var query = QuerySalelist.Where(s => (s.SaleDate.Year >= _year && s.SaleDate.Year < _toYear));
            List<MonthTotalVM> _Model = new List<MonthTotalVM>();

            for (int i = 1; i < 13; i++)
            {
                double _grandTotal = 0;
                double temp = 0;
                temp = query.Where(x => x.SaleDate.Month == i).Sum(x => (double?)(x.SaleLines.Sum(p => (p.LineQuantity * p.LineUnitPrice)))) ?? 0;

                _grandTotal = temp;

                MonthTotalVM model = new MonthTotalVM()
                {
                    Year = _year,
                    Month = i,
                    GrandTotal = _grandTotal
                };
                _Model.Add(model);
            }
            return _Model.ToList();
        }

        public List<DayTotalVM> MonthlySalesByDate_forCharts(int yr, int mnt)
        {
            int year = yr;
            int month = mnt;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            var days = Enumerable.Range(1, daysInMonth);

            ICustomerReturn _customerReturnRepository = new CustomerReturnRepository(db);

            List<Sale> QuerySalelist = new List<Sale>();
            Sale getSalePeriode = new Sale();
            List<Sale> salelist = db.Sales.Where(x => x.SaleDate.Year == year && x.SaleDate.Month == month).ToList();
            salelist.ForEach(allSales =>
            {
                getSalePeriode = _customerReturnRepository.GetRealSale(allSales);
                QuerySalelist.Add(getSalePeriode);
            });

            var query = QuerySalelist.Where(x => x.SaleDate.Year == year && x.SaleDate.Month == month).Select(g => new
            {
                Day = g.SaleDate.Day,
                Total = g.SaleLines.Sum(s => s.LineQuantity * s.LineUnitPrice)
            });
            SalesVM model = new SalesVM
            {
                Date = new DateTime(year, month, 1),
                Days = days.GroupJoin(query, d => d, q => q.Day, (d, q) => new DayTotalVM
                {
                    Day = d,
                    Total = q.Sum(x => x.Total)
                }).ToList()
            };
            return model.Days.ToList();
        }

        public RptReceipt modelReportReceipt()
        {
            try
            {

                int i = 0;

                double TotalAdvancedAmount = 0d;

                double ReductionAmount = 0d;
                double DiscountAmount = 0d;
                double RealTVAAmount = 0d;
                double TotalAmountTTC = 0d;
                double balance = 0d;
                double TotalHT = 0d;

               
                int saleID = (Session["Receipt_SaleID"] == null) ? 0 : (int)Session["Receipt_SaleID"];
                string customerID = (Session["Receipt_CustomerID"] == null) ? "" : (string)Session["Receipt_CustomerID"];
                double receiveAmoung = (Session["ReceiveAmoung_Tot"] == null) ? 0 : (double)Session["ReceiveAmoung_Tot"];

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;

                List<ReceiptLine> receiptline = new List<ReceiptLine>();
                List<PaymentDetail> paymentline = new List<PaymentDetail>();

                RptReceipt model = new RptReceipt();

                string defaultImgPath = Server.MapPath("~/Content/Images/App/deboy-logo.png");
                byte[] COMPANY_LOGO = System.IO.File.ReadAllBytes(defaultImgPath);
                
                Sale currentSale = (from sal in db.Sales
                                    where sal.SaleID == saleID
                                    select sal).SingleOrDefault();
                //Customer customer = db.Customers.FirstOrDefault(p => p.GlobalPersonID == currentSale.CustomerID);
                double saleAmount = 0d;
                string labelProduct = "";
                //fabrication du detail
                db.SaleLines.Where(l => l.SaleID == saleID).ToList().ForEach(c =>
                {
                    i += 1;
                    saleAmount += c.LineAmount;
                    labelProduct = (c.marque != null && c.reference != null) ? "Frame/Monture " + c.marque + " - Ref " + c.reference : (c.Product is OrderLens) ?  c.ProductLabel : (c.Product is Lens) ? LensConstruction.GetLensPrescriptionWitchDescription((Lens)c.Product, c.OeilDroiteGauche, c.Axis): c.ProductLabel;
                    receiptline.Add(
                            new ReceiptLine
                            {
                                ReceiptLineID = i,
                                ReceiptLineQuantity = c.LineQuantity,
                                ReceiptLineUnitPrice = c.LineUnitPrice,
                                ReceiptLineAmount = c.LineAmount,
                                Designation = labelProduct,
                            }
                    );
                });

                //recuparation de lhistorique des depot pr la vente
                i = 0;
                List<PaymentDetail> modelAdvanced = new List<PaymentDetail>();
                List<CustomerSlice> slicelist = db.CustomerSlices.Where(s => s.SaleID == saleID).ToList();
                slicelist.ForEach(slice =>
                {
                    i++;
                    TotalAdvancedAmount += (slice.SliceAmount);
                    modelAdvanced.Add(
                    new PaymentDetail
                    {
                        PaymentDetailID = i,
                        PaymentDetailAmount = slice.SliceAmount,
                        PaymentDate = slice.SliceDate,
                        Reference = slice.Reference
                    });
                });

                //fabrication des avances clients

                ReductionAmount = saleAmount * (currentSale.RateReduction / 100);
                DiscountAmount = (saleAmount - currentSale.RateReduction) * (currentSale.RateDiscount / 100);
                TotalHT = saleAmount - ReductionAmount - DiscountAmount + currentSale.Transport;
                RealTVAAmount = (TotalHT * currentSale.VatRate) / 100;
                TotalAmountTTC = TotalHT + RealTVAAmount;
                balance = TotalAmountTTC - TotalAdvancedAmount;

                model.ReceiveAmount = receiveAmoung;
                model.CompanyName = Company.Name;
                model.CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel;
                model.CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber;
                model.BranchName = currentSale.Branch.BranchName;
                model.BranchAdress = currentSale.Branch.Adress.Quarter.QuarterLabel + " - " + currentSale.Branch.Adress.Quarter.Town.TownLabel;
                model.CompanyRC = Company.CompanyTradeRegister;
                model.Ref = String.Concat(currentSale.SaleReceiptNumber," ",(currentSale.DateRdv.HasValue) ? String.Concat(" - ",Resources.DateRdv, ":",currentSale.DateRdv.Value.ToString("dd/MM/yyyy")): "");
                model.CompanyCNI = "NO CONT : " + Company.CNI;
                model.Operator = GetOperator(currentSale.Operator); //currentSale.Operator.Name + " " + currentSale.Operator.Description; // CurrentUser.Name + " " + CurrentUser.Description;
                model.CustomerName = customerID;
                model.CustomerAccount = customerID;
                model.SaleDate = currentSale.SaleDateHours;
                model.Title = currentSale.AdressPhoneNumber;
                model.DeviseLabel = currentSale.Devise.DeviseLabel;
                model.RateTVA = currentSale.VatRate;
                model.RateReduction = currentSale.RateReduction;
                model.RateDiscount = currentSale.RateDiscount;
                model.Transport = currentSale.Transport;
                model.TotalAmount = saleAmount;
                //model.ReceiptLines = receiptline;
                model.Balance = balance;
                model.TotalAmountTTC = TotalAmountTTC;
                model.CompanyLogo = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID) != null ? db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID).Content : COMPANY_LOGO;
                model.CustomerAdress = "Email:" + currentSale.Branch.Adress.AdressEmail;
                model.BranchTel = "Tel: " + currentSale.Branch.Adress.AdressCellNumber + "/" + currentSale.Branch.Adress.AdressPhoneNumber;
                model.PaymentDetails = modelAdvanced;
                model.ReceiptLines = receiptline;
                model.SaleDeliveryDate = (currentSale.SaleDeliveryDate.HasValue) ? currentSale.SaleDeliveryDate.Value : currentSale.SaleDate;
                model.Deliver = (currentSale.SaleDeliver) ? "Livré/Delivered" : "Non Livré/Not Delivered";
                model.OperationDate = SessionBusinessDay(null).BDDateOperation;
                model.OperationHour = DateTime.Now.ToString("HH:mm:ss");
                return model;
            }
            catch (Exception ex)
            {
                RedirectToAction("Index", "Home");
                throw;
            }
        }

        public string GetOperator(User Operator)
        {
            return Operator != null ? (Operator.Name + " " + Operator.Description) :
                                      (CurrentUser.Name + " " + CurrentUser.Description);
        }
        ////This method print a receipt of customer
        public ActionResult RptReceipt()
        {
            var model = modelReportReceipt();

            return View(model);
        }
        ////This method print a receipt of customer
        public ActionResult RptReceiptDetail()
        {
            var model = modelReportReceipt();

            return View(model);
        }
        public ActionResult RptReceiptDetailOrther()
        {
            var model = modelReportReceipt();

            return View(model);
        }
        public RptProformaInvoice modelRptProformaInvoice()
        {
            try
            {

                int i = 0,j=0,k=0;
                double totalAmount = 0d;
                double TotalLens = 0d;
                double TotalFrame = 0d;

                int CommandID = (Session["Receipt_CommandID"] == null) ? 0 : (int)Session["Receipt_CommandID"];

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;

                List<DetailLens> DetailLensline = new List<DetailLens>();
                List<DetailFrame> DetailFrameline = new List<DetailFrame>();

                RptProformaInvoice model = new RptProformaInvoice();

               
                if (CommandID > 0)
                {
                    
                    CustomerOrder currentOrder = db.CustomerOrders.Find(CommandID);
                    //recuperation detail mnt
                    List <CustomerOrderLine> lstOrderLine = db.CustomerOrderLines.Where(sl => sl.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                    totalAmount = (lstOrderLine.Count > 0) ? Util.ExtraPrices(lstOrderLine.Select(c => c.LineAmount).Sum(), currentOrder.RateReduction, currentOrder.RateDiscount, currentOrder.Transport, currentOrder.VatRate).TotalTTC : 0; //montant du verre
                    //FatSod.Ressources.Resources.Culture = System.Globalization.CultureInfo.GetCultureInfo("fr-FR");
                    string montantLettre = LoadComponent.Int2Lettres((Int32)totalAmount).ToUpper();
                    string montantLettreEn= NumberConverter.Spell((long)totalAmount).ToUpper();
                    //LoadComponent.NumToWordBD((long)totalAmount).ToUpper();
                    
                    foreach (CustomerOrderLine c in lstOrderLine)
                    {
                        //OrderLens lensproduct = db.OrderLenses.Where(ol => ol.ProductID == c.ProductID).FirstOrDefault();
                        Product lensproduct = db.Products.Find(c.ProductID);
                        if (lensproduct is GenericProduct) //frame
                        {
                            i += 1;
                            TotalFrame = TotalFrame + c.LineAmount;
                            DetailFrameline.Add(
                                new DetailFrame
                                {
                                    DetailFrameID=i,
                                    FrameName="Marque : " + c.marque,
                                    FrameAmount="",
                                    FrameUnitPrice ="",
                                    Marque =c.marque,
                                    Reference=c.reference,
                                    Materiere=c.FrameCategory,
                                    FrameQuantity = "0" + c.LineQuantity.ToString()
                                });
                            i += 1;
                            DetailFrameline.Add(
                                new DetailFrame
                                {
                                    DetailFrameID = i,
                                    FrameName = "Reference : " + c.reference,
                                    FrameAmount = c.LineAmount.ToString("N0"),
                                    FrameUnitPrice = c.LineUnitPrice.ToString("N0"),
                                    Marque = c.marque,
                                    Reference = c.reference,
                                    Materiere = c.FrameCategory,
                                    FrameQuantity = ""
                                });
                            i += 1;
                            DetailFrameline.Add(
                                new DetailFrame
                                {
                                    DetailFrameID = i,
                                    FrameName = "Matiere : " + c.FrameCategory,
                                    FrameAmount = "",
                                    FrameUnitPrice = "",
                                    Marque = c.marque,
                                    Reference = c.reference,
                                    Materiere = c.FrameCategory,
                                    FrameQuantity = ""
                                });
                        }
                        if (lensproduct is OrderLens) // orderlens
                        {
                            OrderLens orderlensproduct = db.OrderLenses.Where(ol => ol.ProductID == c.ProductID).FirstOrDefault();
                            j += 1;
                            TotalLens = TotalLens + c.LineAmount;
                            if (j==1)
                            {
                                k += 1;
                                DetailLensline.Add(
                                new DetailLens
                                {
                                    DetailLensID = k,
                                    LensName = (c.SupplyingName==null) ? c.Product.Category.CategoryDescription: c.SupplyingName,
                                    LensDesignation = "",
                                    LensQty = "",
                                    LensUnitPrice = "",
                                    LensAmount = ""
                                });
                                k += 1;
                                DetailLensline.Add(
                                new DetailLens
                                {
                                    DetailLensID = k,
                                    LensName = "Sph Cyl Axe Add",
                                    LensDesignation = "",
                                    LensQty = "",
                                    LensUnitPrice = "",
                                    LensAmount = ""
                                });
                            }
                            k += 1;
                            DetailLensline.Add(
                            new DetailLens
                            {
                                DetailLensID = k,
                                LensName = (orderlensproduct.Addition == null || orderlensproduct.Addition == "") ? ((orderlensproduct.LensNumber.LensNumberSphericalValue==null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ?"//": orderlensproduct.LensNumber.LensNumberSphericalValue) + "\t " + ((orderlensproduct.LensNumber.LensNumberCylindricalValue==null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ?"//": orderlensproduct.LensNumber.LensNumberCylindricalValue) + "\t " + ((c.Axis==null || c.Axis == "") ?"//": c.Axis)+"\t"+"//" : ((orderlensproduct.LensNumber.LensNumberSphericalValue==null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ?"//": orderlensproduct.LensNumber.LensNumberSphericalValue) + "\t " + ((orderlensproduct.LensNumber.LensNumberCylindricalValue==null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ?"//": orderlensproduct.LensNumber.LensNumberCylindricalValue) + "\t " + ((c.Axis==null || c.Axis == "") ?"//": c.Axis) + " " +  "ADD" + orderlensproduct.Addition,
                                LensDesignation = c.OeilDroiteGauche.ToString(),
                                LensQty = c.LineQuantity.ToString(),
                                LensUnitPrice = c.LineUnitPrice.ToString("N0"),
                                LensAmount = c.LineAmount.ToString("N0")
                            });
                        }
                        if (lensproduct is Lens) // stock lens
                        {
                            Lens orderlensproduct = db.Lenses.Where(ol => ol.ProductID == c.ProductID).FirstOrDefault();
                            j += 1;
                            TotalLens = TotalLens + c.LineAmount;
                            if (j == 1)
                            {
                                k += 1;
                                DetailLensline.Add(
                                new DetailLens
                                {
                                    DetailLensID = k,
                                    LensName = (c.SupplyingName == null) ? c.Product.Category.CategoryDescription : c.SupplyingName,
                                    LensDesignation = "",
                                    LensQty = "",
                                    LensUnitPrice = "",
                                    LensAmount = ""
                                });
                                k += 1;
                                DetailLensline.Add(
                                new DetailLens
                                {
                                    DetailLensID = k,
                                    LensName = "Sph Cyl Axe Add",
                                    LensDesignation = "",
                                    LensQty = "",
                                    LensUnitPrice = "",
                                    LensAmount = ""
                                });
                            }
                            k += 1;
                            DetailLensline.Add(
                            new DetailLens
                            {
                                DetailLensID = k,
                                LensName =  (orderlensproduct.LensNumber.LensNumberAdditionValue == null || orderlensproduct.LensNumber.LensNumberAdditionValue == "") ? ((orderlensproduct.LensNumber.LensNumberSphericalValue == null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ? "//" : orderlensproduct.LensNumber.LensNumberSphericalValue) + "\t " + ((orderlensproduct.LensNumber.LensNumberCylindricalValue == null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ? "//" : orderlensproduct.LensNumber.LensNumberCylindricalValue) + "\t " + ((c.Axis == null || c.Axis == "") ? "//" : c.Axis) + "\t" + "//" : ((orderlensproduct.LensNumber.LensNumberSphericalValue == null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ? "//" : orderlensproduct.LensNumber.LensNumberSphericalValue) + "\t " + ((orderlensproduct.LensNumber.LensNumberCylindricalValue == null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ? "//" : orderlensproduct.LensNumber.LensNumberCylindricalValue) + "\t " + ((c.Axis == null || c.Axis == "") ? "//" : c.Axis) + " " + "ADD" + orderlensproduct.LensNumber.LensNumberAdditionValue,
                                LensDesignation = c.OeilDroiteGauche.ToString(),
                                LensQty = c.LineQuantity.ToString(),
                                LensUnitPrice = c.LineUnitPrice.ToString("N0"),
                                LensAmount = c.LineAmount.ToString("N0")
                            });
                        }
                    }
                    model.RptProformaInvoiceID = 1;
                    model.Reference = currentOrder.CustomerOrderNumber;
                    model.ProformaDate = currentOrder.CustomerOrderDate;
                    model.Title = "Proforma Invoice";
                    model.TitleFr = "Facture Proforma";

                    model.CompanyName = Company.Name;
                    model.CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel;
                    model.CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber;
                    model.CompanyCNI = "NO CONT : " + Company.CNI;
                    model.Operator = CurrentUser.Name + " " + CurrentUser.Description;

                    model.CustomerName = currentOrder.CustomerName;
                    model.InsurreName = (currentOrder.InsurreName==null || currentOrder.InsurreName=="") ? currentOrder.CustomerName : currentOrder.InsurreName;
                    model.CustomerPhone = currentOrder.PhoneNumber;
                    model.CustomerCompany = currentOrder.CompanyName;
                    model.CustomerDoctor = currentOrder.MedecinTraitant;
                    model.PrescriptionDate = currentOrder.CustomerOrderDate;

                    model.TotalAmount = totalAmount;

                    model.DeviseLabel = currentOrder.Devise.DeviseLabel;
                    model.MontantLettreFr = montantLettre;
                    model.MontantLettreEn = montantLettreEn;

                    model.TotalLens = TotalLens;
                    model.TotalFrame = TotalFrame;

                    model.DetailLenses = DetailLensline;
                    model.DetailFrames = DetailFrameline;

                    model.Agency = currentOrder.Branch.Adress.Quarter.Town.TownLabel;

                }

                return model;

            }
            catch (Exception ex)
            {
                RedirectToAction("Index", "Home");
                throw;
            }
        }

        public RptReceipt GenerateFacture(CultureInfo enUsCI)
        {
            try
            {

                int i = 0;
                int j = 1;
                int customerOrderID = (Session["Receipt_CustomerOrderID"] == null) ? 0 : (int)Session["Receipt_CustomerOrderID"];

                string InsurreName = (enUsCI.Name.ToString() == "en-US") ? " Insured " : " Assuré ";
                string PatientName = (enUsCI.Name.ToString() == "en-US") ? " Patient " : " Patient ";
                string FrameName = (enUsCI.Name.ToString() == "en-US") ? " FRAME " : " MONTURE ";

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;

                List<ReceiptLine> receiptline = new List<ReceiptLine>();

                RptReceipt model = new RptReceipt();

                if (customerOrderID > 0)//depot pour une vente
                {
                    CustomerOrder currentOrder = db.CustomerOrders.Find(customerOrderID);

                    //Customer customer = db.Customers.FirstOrDefault(p => p.GlobalPersonID == currentSale.CustomerID);
                    //double saleAmount = 0d;
                    string labelProduct = "";
                    string Prescription = "";
                    var InitialTotalAmount = currentOrder.Plafond; // / (1 - currentOrder.RemiseAssurance / 100);
                    double totType = db.CustomerOrderLines.Where(l => l.CustomerOrderID == customerOrderID).Count();
                    //fabrication du detail
                    db.CustomerOrderLines.Where(l => l.CustomerOrderID == customerOrderID).OrderBy(o => o.CustomerOrderID).ToList().ForEach(c =>
                    {
                        i += 1;

                        //OrderLens lensproduct = db.OrderLenses.Where(ol => ol.ProductID == c.ProductID).FirstOrDefault();
                        Product lensproduct = db.Products.Find(c.ProductID);
                        if (lensproduct is GenericProduct) //frame
                        {
                            Product frameproduct = db.Products.Find(c.ProductID);
                            if (frameproduct.Category.isSerialNumberNull) //frame
                            {
                                j += 1;
                                labelProduct = (c.marque != null && c.reference != null) ? FrameName + c.marque + " REF " + c.reference : "";
                                receiptline.Add(
                                        new ReceiptLine
                                        {
                                            ReceiptLineID = j,
                                            DetailQty = (currentOrder.DatailBill == 0) ? "" : currentOrder.MontureAssurance.ToString("N0"),//pr des besoin d'affichage ce champs sera use pour les montants
                                            Designation = "0" + c.LineQuantity + " " + labelProduct,
                                            ProducType = (totType == 1) ? InsurreName + ": " + currentOrder.InsurreName + " / " + PatientName + ": " + currentOrder.CustomerName : "" //sera use pr afficher le nom du client
                                        }
                                );
                            }
                        }
                        else // orderlens
                        {
                            if (lensproduct is OrderLens)
                            {
                                if (c.Product.Prescription != null)
                                {
                                    Prescription = c.Product.Prescription;
                                }
                                else
                                {
                                    Prescription = (c.Product.ProductCode.Contains(" HD ")) ? c.Product.ProductCode.Replace(c.Product.Category.CategoryCode + " HD", "") : c.Product.ProductCode.Replace(c.Product.Category.CategoryCode, "");
                                }
                            }
                            if (lensproduct is Lens)
                            {
                                //if (c.Product.Prescription != null)
                                //{
                                Prescription = LensConstruction.GetLensCodePrescription((Lens)lensproduct, c.OeilDroiteGauche, c.Axis);// c.EyeSide +" "+ c.Product.Prescription;
                                //}
                                //else
                                //{
                                //    Prescription =  (c.Product.ProductCode.Contains(" HD ")) ? c.EyeSide + " " + c.Product.ProductCode.Replace(c.Product.Category.CategoryCode + " HD", "") : c.EyeSide + " " + c.Product.ProductCode.Replace(c.Product.Category.CategoryCode, "");
                                //}
                            }



                            if (j == 1)
                            {
                                receiptline.Add(
                                    new ReceiptLine
                                    {
                                        ReceiptLineID = j,
                                        DetailQty = "",//pr des besoin d'affichage ce champs sera use pour les montants
                                        Designation = (c.SupplyingName == null) ? c.Product.Category.CategoryDescription : c.SupplyingName,
                                        ProducType = "" //sera use pr afficher le nom du client
                                    });
                                //
                                j += 1;
                                receiptline.Add(
                                    new ReceiptLine
                                    {
                                        ReceiptLineID = j,
                                        DetailQty = (currentOrder.DatailBill == 0) ? InitialTotalAmount.ToString("N0") : currentOrder.VerreAssurance.ToString("N0"),//pr des besoin d'affichage ce champs sera use pour les montants
                                        Designation = Prescription,
                                        ProducType = InsurreName + ": " + currentOrder.InsurreName  //sera use pr afficher le nom du client
                                    });
                            }
                            else
                            {
                                j += 1;
                                receiptline.Add(
                                        new ReceiptLine
                                        {
                                            ReceiptLineID = j,
                                            DetailQty = "",//pr des besoin d'affichage ce champs sera use pour les montants
                                            Designation = Prescription,
                                            ProducType = PatientName + ": " + currentOrder.CustomerName //sera use pr afficher le nom du client
                                        });
                            }


                        }

                    });

                    model.RptReceiptPaymentDetailID = currentOrder.DatailBill;
                    model.CompanyName = Company.Name;
                    model.CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel;
                    model.CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber;
                    model.BranchName = currentOrder.Branch.BranchName;
                    model.BranchAdress = currentOrder.Branch.Adress.Quarter.QuarterLabel + " - " + currentOrder.Branch.Adress.Quarter.Town.TownLabel;
                    model.CompanyRC = Company.CompanyTradeRegister;
                    model.Ref = currentOrder.NumeroFacture;
                    model.CompanyCNI = "NO CONT : " + Company.CNI;
                    model.Operator = CurrentUser.Name + " " + CurrentUser.Description;
                    model.CustomerName = currentOrder.CustomerName;
                    model.InsurreName = currentOrder.InsurreName;
                    model.CustomerAccount = currentOrder.Assureur.Name;
                    model.SaleDate = currentOrder.CustomerOrderDate;
                    model.SaleDateHours = currentOrder.CustomerDateHours.Value;
                    model.Title = Company.Adress.Quarter.Town.TownLabel;
                    model.DeviseLabel = currentOrder.PoliceAssurance;//police d'assurance;
                    
                    model.ReceiptLines = receiptline;
                    model.BranchAbbreviation = currentOrder.CompanyName; //NOM DE LA SOCIETE DU CLIENT
                    model.CustomerAdress = currentOrder.NumeroBonPriseEnCharge;
                    model.BranchTel = "Tel: " + currentOrder.Branch.Adress.AdressCellNumber + "/" + currentOrder.Branch.Adress.AdressPhoneNumber;
                    
                    
                    model.TotalAmount = InitialTotalAmount ;
                    model.RemiseAssurance = currentOrder.RemiseAssurance;

                    var AmountWithRemise = currentOrder.Plafond;
                    model.MontantLettre = LoadComponent.Int2Lettres((Int32)(InitialTotalAmount) ).ToUpper(); ;//MONTANT FACTURE,
                    model.MontantLettreEN = NumberConverter.Spell((long)InitialTotalAmount).ToUpper();
                    //LoadComponent.NumToWordBD((long)currentOrder.Plafond).ToUpper(); ;//MONTANT FACTURE,
                }
                return model;
            }
            catch (Exception ex)
            {
                RedirectToAction("Index", "Home");
                throw;
            }

        }

        public RptReceipt GenerateFacture()
        {
            try
            {

                int i = 0;
                int j = 1;
                int customerOrderID = (Session["Receipt_CustomerOrderID"] == null) ? 0 : (int)Session["Receipt_CustomerOrderID"];

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;

                List<ReceiptLine> receiptline = new List<ReceiptLine>();
               
                RptReceipt model = new RptReceipt();

                if (customerOrderID > 0)//depot pour une vente
                {
                    CustomerOrder currentOrder = db.CustomerOrders.Find(customerOrderID);
                    
                    //Customer customer = db.Customers.FirstOrDefault(p => p.GlobalPersonID == currentSale.CustomerID);
                    //double saleAmount = 0d;
                    string labelProduct = "";
                    string Prescription = "";
                    double totType = db.CustomerOrderLines.Where(l => l.CustomerOrderID == customerOrderID).Count();
                    var InitialTotalAmount = currentOrder.Plafond; // / (1 - currentOrder.RemiseAssurance / 100);
                    //fabrication du detail
                    db.CustomerOrderLines.Where(l => l.CustomerOrderID == customerOrderID).OrderBy(o => o.CustomerOrderID).ToList().ForEach(c =>
                    {
                        i += 1;

                        //OrderLens lensproduct = db.OrderLenses.Where(ol => ol.ProductID == c.ProductID).FirstOrDefault();
                        Product lensproduct = db.Products.Find(c.ProductID);
                        if (lensproduct is GenericProduct) //frame
                        {
                            Product frameproduct = db.Products.Find(c.ProductID);
                            if (frameproduct.Category.isSerialNumberNull) //frame
                            {
                                j += 1;
                                labelProduct = (c.marque != null && c.reference != null) ? "MONTURE/FRAME " + c.marque + " REF " + c.reference : "";
                                receiptline.Add(
                                        new ReceiptLine
                                        {
                                            ReceiptLineID = j,
                                            DetailQty = (currentOrder.DatailBill == 0) ? "" : currentOrder.MontureAssurance.ToString("N0"),//pr des besoin d'affichage ce champs sera use pour les montants
                                            Designation = "0" + c.LineQuantity + " " + labelProduct,
                                            ProducType = (totType == 1) ? Resources.InsurreName +": " +currentOrder.InsurreName : (totType == 2) ? Resources.PatientName + ": " + currentOrder.CustomerName : "" //sera use pr afficher le nom du client
                                    }
                                );
                            }
                        }
                        else // orderlens
                        {
                            if (lensproduct is OrderLens)
                            {
                                if (c.Product.Prescription != null)
                                {
                                    Prescription = c.Product.Prescription;
                                }
                                else
                                {
                                    Prescription = (c.Product.ProductCode.Contains(" HD ")) ? c.Product.ProductCode.Replace(c.Product.Category.CategoryCode + " HD", "") : c.Product.ProductCode.Replace(c.Product.Category.CategoryCode, "");
                                }
                            }
                            if (lensproduct is Lens)
                            {
                                //if (c.Product.Prescription != null)
                                //{
                                    Prescription = LensConstruction.GetLensCodePrescription((Lens)lensproduct, c.OeilDroiteGauche, c.Axis);// c.EyeSide +" "+ c.Product.Prescription;
                                //}
                                //else
                                //{
                                //    Prescription =  (c.Product.ProductCode.Contains(" HD ")) ? c.EyeSide + " " + c.Product.ProductCode.Replace(c.Product.Category.CategoryCode + " HD", "") : c.EyeSide + " " + c.Product.ProductCode.Replace(c.Product.Category.CategoryCode, "");
                                //}
                            }
                            


                            if (j == 1)
                            {
                                receiptline.Add(
                                    new ReceiptLine
                                    {
                                        ReceiptLineID = j,
                                        DetailQty = "",//pr des besoin d'affichage ce champs sera use pour les montants
                                        Designation = (c.SupplyingName==null) ? c.Product.Category.CategoryDescription: c.SupplyingName,
                                        ProducType = "" //sera use pr afficher le nom du client
                                    });
                                //
                                j += 1;
                                receiptline.Add(
                                    new ReceiptLine
                                    {
                                        ReceiptLineID = j,
                                        DetailQty = (currentOrder.DatailBill == 0) ? InitialTotalAmount.ToString("N0") : currentOrder.VerreAssurance.ToString("N0"),//pr des besoin d'affichage ce champs sera use pour les montants
                                        Designation = Prescription,
                                        ProducType = Resources.InsurreName + ": " + currentOrder.InsurreName  //sera use pr afficher le nom du client
                                    });
                            }
                            else
                            {
                                j += 1;
                                receiptline.Add(
                                        new ReceiptLine
                                        {
                                            ReceiptLineID = j,
                                            DetailQty = "",//pr des besoin d'affichage ce champs sera use pour les montants
                                            Designation = Prescription,
                                            ProducType = Resources.PatientName + ": " + currentOrder.CustomerName //sera use pr afficher le nom du client
                                        });
                            }
                           

                        }
                        
                    });

                    model.RptReceiptPaymentDetailID = currentOrder.DatailBill;
                    model.CompanyName = Company.Name;
                    model.CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel;
                    model.CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber;
                    model.BranchName = currentOrder.Branch.BranchName;
                    model.BranchAdress = currentOrder.Branch.Adress.Quarter.QuarterLabel + " - " + currentOrder.Branch.Adress.Quarter.Town.TownLabel;
                    model.CompanyRC = Company.CompanyTradeRegister;
                    model.Ref = currentOrder.NumeroFacture;
                    model.CompanyCNI = "NO CONT : " + Company.CNI;
                    model.Operator = CurrentUser.Name + " " + CurrentUser.Description;
                    model.CustomerName = currentOrder.CustomerName;
                    model.InsurreName = currentOrder.InsurreName;
                    model.CustomerAccount = currentOrder.Assureur.Name;
                    model.SaleDate = currentOrder.CustomerOrderDate;
                    model.SaleDateHours = currentOrder.CustomerDateHours.Value;
                    model.Title = Company.Adress.Quarter.Town.TownLabel;
                    model.DeviseLabel = currentOrder.PoliceAssurance;//police d'assurance;
                    
                    model.ReceiptLines = receiptline;
                    model.BranchAbbreviation = currentOrder.CompanyName; //NOM DE LA SOCIETE DU CLIENT
                    model.CustomerAdress = currentOrder.NumeroBonPriseEnCharge;
                    model.BranchTel = "Tel: " + currentOrder.Branch.Adress.AdressCellNumber + "/" + currentOrder.Branch.Adress.AdressPhoneNumber;

                    model.ReceiptLines = receiptline;
                   
                    model.TotalAmount = InitialTotalAmount ;
                    model.RemiseAssurance = currentOrder.RemiseAssurance;
                   
                    model.MontantLettre = LoadComponent.Int2Lettres((Int32)InitialTotalAmount).ToUpper(); ;//MONTANT FACTURE,
                    model.MontantLettreEN = NumberConverter.Spell((long)InitialTotalAmount).ToUpper();
                        //LoadComponent.NumToWordBD((long)currentOrder.Plafond).ToUpper(); ;//MONTANT FACTURE,
                }
                return model;
            }
            catch (Exception ex)
            {
                RedirectToAction("Index", "Home");
                throw;
            }

        }

        public ActionResult RptProforma(int ? CustomerOrderID)
        {
            if (CustomerOrderID.HasValue && CustomerOrderID != null && CustomerOrderID > 0)
            {
                Session["Receipt_CommandID"] = CustomerOrderID;
            }
            var model = modelRptProformaInvoice();

            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            return View(model);
        }

        #region RptProformaGolden

        public ActionResult RptProformaGolden(int? CustomerOrderID)
        {
            if (CustomerOrderID.HasValue && CustomerOrderID != null && CustomerOrderID > 0)
            {
                Session["Receipt_CommandID"] = CustomerOrderID;
            }
            var model = modelRptProformaGoldenInvoice();

            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            return View(model);
        }

        public ModelRptProformaInvoice modelRptProformaGoldenInvoice()
        {
            try
            {

                int i = 0, j = 0, k = 0;
                double totalAmount = 0d;
                double TotalLens = 0d;
                double TotalFrame = 0d;

                int CommandID = (Session["Receipt_CommandID"] == null) ? 0 : (int)Session["Receipt_CommandID"];

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;

                List<RxPrescription> LstRxPrescription = new List<RxPrescription>();
                List<DetailSales> LstDetailSales = new List<DetailSales>();

                ModelRptProformaInvoice model = new ModelRptProformaInvoice();


                if (CommandID > 0)
                {

                    CustomerOrder currentOrder = db.CustomerOrders.Find(CommandID);
                    //recuperation detail mnt
                    List<CustomerOrderLine> lstOrderLine = db.CustomerOrderLines.Where(sl => sl.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                    totalAmount = (lstOrderLine.Count > 0) ? Util.ExtraPrices(lstOrderLine.Select(c => c.LineAmount).Sum(), currentOrder.RateReduction, currentOrder.RateDiscount, currentOrder.Transport, currentOrder.VatRate).TotalTTC : 0; //montant du verre
                    //FatSod.Ressources.Resources.Culture = System.Globalization.CultureInfo.GetCultureInfo("fr-FR");
                    string montantLettre = LoadComponent.Int2Lettres((Int32)totalAmount).ToUpper();
                    string montantLettreEn = NumberConverter.Spell((long)totalAmount).ToUpper();
                    //LoadComponent.NumToWordBD((long)totalAmount).ToUpper();

                    foreach (CustomerOrderLine c in lstOrderLine)
                    {
                        string lenscate = (c.SupplyingName == null) ? c.Product.Category.CategoryDescription : c.SupplyingName;

                        Product lensproduct = db.Products.Find(c.ProductID);
                        if (lensproduct is GenericProduct) //frame
                        {
                            i += 1;
                            TotalFrame = TotalFrame + c.LineAmount;
                            LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 0,
                                    Designation = Resources.frame,
                                    Qte = "",
                                    PUHT = "",
                                    PercRed = "",
                                    MntHT = ""
                                });
                            i += 1;
                            LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 1,
                                    Designation = c.marque + " " + c.reference,
                                    Qte = c.LineQuantity.ToString("N0"),
                                    PUHT = c.LineUnitPrice.ToString("N0"),
                                    PercRed = "0",
                                    MntHT = c.LineAmount.ToString("N0")
                                });
                            
                        }
                        if (lensproduct is OrderLens) // orderlens
                        {
                            LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 3,
                                    Designation = "",
                                    Qte = "",
                                    PUHT = "",
                                    PercRed = "",
                                    MntHT = ""
                                });

                            OrderLens orderlensproduct = db.OrderLenses.Where(ol => ol.ProductID == c.ProductID).FirstOrDefault();
                            j += 1;
                            TotalLens = TotalLens + c.LineAmount;
                            

                            if (j == 1)
                            {
                                k += 1;
                                LstRxPrescription.Add(
                                new RxPrescription
                                {
                                    LineNumber = k,
                                    Field1 = "Rx Date",
                                    Field2 = currentOrder.CustomerOrderDate.ToString("dd/MM/yyyyy"),
                                    Oeil = c.OeilDroiteGauche.ToString(),
                                    Sphere = (orderlensproduct.LensNumber.LensNumberSphericalValue == null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberSphericalValue,
                                    Cylindre = (orderlensproduct.LensNumber.LensNumberCylindricalValue == null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberCylindricalValue,
                                    Axe = (c.Axis == null || c.Axis == "") ? "" : c.Axis,
                                    Add = (orderlensproduct.LensNumber.LensNumberAdditionValue == null || orderlensproduct.LensNumber.LensNumberAdditionValue == "") ? "" : orderlensproduct.LensNumber.LensNumberAdditionValue
                                });

                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 4,
                                    Designation = Resources.lenses,
                                    Qte = "",
                                    PUHT = "",
                                    PercRed = "",
                                    MntHT = ""
                                });
                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 5,
                                    Designation = c.OeilDroiteGauche.ToString() + " " + lenscate,
                                    Qte = c.LineQuantity.ToString("N0"),
                                    PUHT = c.LineUnitPrice.ToString("N0"),
                                    PercRed = "0",
                                    MntHT = c.LineAmount.ToString("N0")
                                });

                            }
                            else { 
                                k += 1;
                                LstRxPrescription.Add(
                                new RxPrescription
                                {
                                    LineNumber = k,
                                    Field1 =Resources.DateExpiration,
                                    Field2 = (currentOrder.DateExpiration.HasValue) ? currentOrder.DateExpiration.Value.ToString("dd/MM/yyyyy"):currentOrder.CustomerOrderDate.AddYears(1).ToString("dd/MM/yyyyy"),
                                    Oeil = c.OeilDroiteGauche.ToString(),
                                    Sphere = (orderlensproduct.LensNumber.LensNumberSphericalValue == null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberSphericalValue,
                                    Cylindre = (orderlensproduct.LensNumber.LensNumberCylindricalValue == null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberCylindricalValue,
                                    Axe = (c.Axis == null || c.Axis == "") ? "" : c.Axis,
                                    Add = (orderlensproduct.LensNumber.LensNumberAdditionValue == null || orderlensproduct.LensNumber.LensNumberAdditionValue == "") ? "" : orderlensproduct.LensNumber.LensNumberAdditionValue
                                });

                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 6,
                                    Designation = c.OeilDroiteGauche.ToString() + " " + lenscate,
                                    Qte = c.LineQuantity.ToString("N0"),
                                    PUHT = c.LineUnitPrice.ToString("N0"),
                                    PercRed = "0",
                                    MntHT = c.LineAmount.ToString("N0")
                                });
                            }
                        }
                        if (lensproduct is Lens) // stock lens
                        {
                            LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 3,
                                    Designation = "",
                                    Qte = "",
                                    PUHT = "",
                                    PercRed = "",
                                    MntHT = ""
                                });

                            Lens orderlensproduct = db.Lenses.Where(ol => ol.ProductID == c.ProductID).FirstOrDefault();
                            j += 1;
                            TotalLens = TotalLens + c.LineAmount;
                            if (j == 1)
                            {
                                k += 1;
                                LstRxPrescription.Add(
                                new RxPrescription
                                {
                                    LineNumber = k,
                                    Field1 = "Rx Date",
                                    Field2 = currentOrder.CustomerOrderDate.ToString("dd/MM/yyyyy"),
                                    Oeil = c.OeilDroiteGauche.ToString(),
                                    Sphere = (orderlensproduct.LensNumber.LensNumberSphericalValue == null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberSphericalValue,
                                    Cylindre = (orderlensproduct.LensNumber.LensNumberCylindricalValue == null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberCylindricalValue,
                                    Axe = (c.Axis == null || c.Axis == "") ? "" : c.Axis,
                                    Add = (orderlensproduct.LensNumber.LensNumberAdditionValue == null || orderlensproduct.LensNumber.LensNumberAdditionValue == "") ? "" : orderlensproduct.LensNumber.LensNumberAdditionValue
                                });

                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 4,
                                    Designation = Resources.lenses,
                                    Qte = "",
                                    PUHT = "",
                                    PercRed = "",
                                    MntHT = ""
                                });

                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 5,
                                    Designation = c.OeilDroiteGauche.ToString() + " " + lenscate,
                                    Qte = c.LineQuantity.ToString("N0"),
                                    PUHT = c.LineUnitPrice.ToString("N0"),
                                    PercRed = "0",
                                    MntHT = c.LineAmount.ToString("N0")
                                });
                            }
                            else
                            { 
                                k += 1;
                                LstRxPrescription.Add(
                                new RxPrescription
                                {
                                    LineNumber = k,
                                    Field1 = Resources.DateExpiration,
                                    Field2 = (currentOrder.DateExpiration.HasValue) ? currentOrder.DateExpiration.Value.ToString("dd/MM/yyyyy") : currentOrder.CustomerOrderDate.AddYears(1).ToString("dd/MM/yyyyy"),
                                    Oeil = c.OeilDroiteGauche.ToString(),
                                    Sphere = (orderlensproduct.LensNumber.LensNumberSphericalValue == null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberSphericalValue,
                                    Cylindre = (orderlensproduct.LensNumber.LensNumberCylindricalValue == null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberCylindricalValue,
                                    Axe = (c.Axis == null || c.Axis == "") ? "" : c.Axis,
                                    Add = (orderlensproduct.LensNumber.LensNumberAdditionValue == null || orderlensproduct.LensNumber.LensNumberAdditionValue == "") ? "" : orderlensproduct.LensNumber.LensNumberAdditionValue
                                });

                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 6,
                                    Designation = c.OeilDroiteGauche.ToString() + " " + lenscate,
                                    Qte = c.LineQuantity.ToString("N0"),
                                    PUHT = c.LineUnitPrice.ToString("N0"),
                                    PercRed = "0",
                                    MntHT = c.LineAmount.ToString("N0")
                                });
                            }
    
                        }
                    }
                    model.ModelRptProformaInvoiceID = 1;
                    model.Reference = currentOrder.CustomerOrderNumber;
                    model.ProformaDate = currentOrder.CustomerOrderDate;
                    model.Title = "Proforma Invoice";
                    model.TitleFr = "Facture Proforma";

                    model.CompanyName = Company.Name;
                    model.CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel;
                    model.CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber;
                    model.CompanyCNI = "NO CONT : " + Company.CNI;
                    model.Operator = CurrentUser.Name + " " + CurrentUser.Description;

                    model.CustomerName = currentOrder.CustomerName;
                    model.InsurreName = (currentOrder.InsurreName == null || currentOrder.InsurreName == "") ? currentOrder.CustomerName : currentOrder.InsurreName;
                    model.CustomerPhone = currentOrder.PhoneNumber;
                    model.CustomerCompany = currentOrder.CompanyName;
                    model.CustomerDoctor = currentOrder.MedecinTraitant;
                    model.PrescriptionDate = currentOrder.CustomerOrderDate;

                    model.TotalAmount = totalAmount;

                    model.DeviseLabel = currentOrder.Devise.DeviseLabel;
                    model.MontantLettreFr = montantLettre;
                    model.MontantLettreEn = montantLettreEn;

                    model.TotalLens = TotalLens;
                    model.TotalFrame = TotalFrame;

                    model.RxPrescription = LstRxPrescription;
                    model.DetailSales = LstDetailSales;

                    model.Agency = currentOrder.Branch.Adress.Quarter.Town.TownLabel;

                }

                return model;

            }
            catch (Exception ex)
            {
                RedirectToAction("Index", "Home");
                throw;
            }
        }

        public ActionResult RptFactureGolden(int? CustomerOrderID)
        {

            if (CustomerOrderID.HasValue && CustomerOrderID != null && CustomerOrderID > 0)
            {
                Session["Receipt_CustomerOrderID"] = CustomerOrderID;
            }

            var model = GenerateFactureGolden();

            return View(model);

        }

        public ModelRptFacture GenerateFactureGolden()
        {
            try
            {

                int i = 0, j = 0, k = 0;
                //double totalAmount = 0d;
                double TotalLens = 0d;
                double TotalFrame = 0d;

                int CommandID = (Session["Receipt_CustomerOrderID"] == null) ? 0 : (int)Session["Receipt_CustomerOrderID"];


                //int CommandID = (Session["Receipt_CommandID"] == null) ? 0 : (int)Session["Receipt_CommandID"];

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;

                List<RxPrescription> LstRxPrescription = new List<RxPrescription>();
                List<DetailSales> LstDetailSales = new List<DetailSales>();

                ModelRptFacture model = new ModelRptFacture();


                if (CommandID > 0)
                {

                    CustomerOrder currentOrder = db.CustomerOrders.Find(CommandID);

                    var InitialTotalAmount = currentOrder.Plafond;

                    //recuperation detail mnt
                    List<CustomerOrderLine> lstOrderLine = db.CustomerOrderLines.Where(sl => sl.CustomerOrderID == currentOrder.CustomerOrderID).ToList();
                    //totalAmount = (lstOrderLine.Count > 0) ? Util.ExtraPrices(lstOrderLine.Select(c => c.LineAmount).Sum(), currentOrder.RateReduction, currentOrder.RateDiscount, currentOrder.Transport, currentOrder.VatRate).TotalTTC : 0; //montant du verre
                   
                    foreach (CustomerOrderLine c in lstOrderLine)
                    {
                        string lenscate = (c.SupplyingName == null) ? c.Product.Category.CategoryDescription : c.SupplyingName;

                        Product lensproduct = db.Products.Find(c.ProductID);
                        if (lensproduct is GenericProduct) //frame
                        {
                            i += 1;
                            TotalFrame = TotalFrame + c.LineAmount;
                            LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 0,
                                    Designation = Resources.frame,
                                    Qte = "",
                                    PUHT = "",
                                    PercRed = "",
                                    MntHT = ""
                                });
                            i += 1;
                            LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 1,
                                    Designation = c.marque + " " + c.reference,
                                    Qte = c.LineQuantity.ToString("N0"),
                                    PUHT = c.LineUnitPrice.ToString("N0"),
                                    PercRed = "0",
                                    MntHT = c.LineAmount.ToString("N0")
                                });

                        }
                        if (lensproduct is OrderLens) // orderlens
                        {
                            LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 3,
                                    Designation = "",
                                    Qte = "",
                                    PUHT = "",
                                    PercRed = "",
                                    MntHT = ""
                                });

                            OrderLens orderlensproduct = db.OrderLenses.Where(ol => ol.ProductID == c.ProductID).FirstOrDefault();
                            j += 1;
                            TotalLens = TotalLens + c.LineAmount;


                            if (j == 1)
                            {
                                k += 1;
                                LstRxPrescription.Add(
                                new RxPrescription
                                {
                                    LineNumber = k,
                                    Field1 = "Rx Date",
                                    Field2 = currentOrder.CustomerOrderDate.ToString("dd/MM/yyyyy"),
                                    Oeil = c.OeilDroiteGauche.ToString(),
                                    Sphere = (orderlensproduct.LensNumber.LensNumberSphericalValue == null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberSphericalValue,
                                    Cylindre = (orderlensproduct.LensNumber.LensNumberCylindricalValue == null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberCylindricalValue,
                                    Axe = (c.Axis == null || c.Axis == "") ? "" : c.Axis,
                                    Add = (orderlensproduct.LensNumber.LensNumberAdditionValue == null || orderlensproduct.LensNumber.LensNumberAdditionValue == "") ? "" : orderlensproduct.LensNumber.LensNumberAdditionValue
                                });

                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 4,
                                    Designation = Resources.lenses,
                                    Qte = "",
                                    PUHT = "",
                                    PercRed = "",
                                    MntHT = ""
                                });
                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 5,
                                    Designation = c.OeilDroiteGauche.ToString() + " " + lenscate,
                                    Qte = c.LineQuantity.ToString("N0"),
                                    PUHT = c.LineUnitPrice.ToString("N0"),
                                    PercRed = "0",
                                    MntHT = c.LineAmount.ToString("N0")
                                });

                            }
                            else
                            {
                                k += 1;
                                LstRxPrescription.Add(
                                new RxPrescription
                                {
                                    LineNumber = k,
                                    Field1 = Resources.DateExpiration,
                                    Field2 = (currentOrder.DateExpiration.HasValue) ? currentOrder.DateExpiration.Value.ToString("dd/MM/yyyyy") : currentOrder.CustomerOrderDate.AddYears(1).ToString("dd/MM/yyyyy"),
                                    Oeil = c.OeilDroiteGauche.ToString(),
                                    Sphere = (orderlensproduct.LensNumber.LensNumberSphericalValue == null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberSphericalValue,
                                    Cylindre = (orderlensproduct.LensNumber.LensNumberCylindricalValue == null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberCylindricalValue,
                                    Axe = (c.Axis == null || c.Axis == "") ? "" : c.Axis,
                                    Add = (orderlensproduct.LensNumber.LensNumberAdditionValue == null || orderlensproduct.LensNumber.LensNumberAdditionValue == "") ? "" : orderlensproduct.LensNumber.LensNumberAdditionValue
                                });

                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 6,
                                    Designation = c.OeilDroiteGauche.ToString() + " " + lenscate,
                                    Qte = c.LineQuantity.ToString("N0"),
                                    PUHT = c.LineUnitPrice.ToString("N0"),
                                    PercRed = "0",
                                    MntHT = c.LineAmount.ToString("N0")
                                });
                            }
                        }
                        if (lensproduct is Lens) // stock lens
                        {
                            LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 3,
                                    Designation = "",
                                    Qte = "",
                                    PUHT = "",
                                    PercRed = "",
                                    MntHT = ""
                                });

                            Lens orderlensproduct = db.Lenses.Where(ol => ol.ProductID == c.ProductID).FirstOrDefault();
                            j += 1;
                            TotalLens = TotalLens + c.LineAmount;
                            if (j == 1)
                            {
                                k += 1;
                                LstRxPrescription.Add(
                                new RxPrescription
                                {
                                    LineNumber = k,
                                    Field1 = "Rx Date",
                                    Field2 = currentOrder.CustomerOrderDate.ToString("dd/MM/yyyyy"),
                                    Oeil = c.OeilDroiteGauche.ToString(),
                                    Sphere = (orderlensproduct.LensNumber.LensNumberSphericalValue == null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberSphericalValue,
                                    Cylindre = (orderlensproduct.LensNumber.LensNumberCylindricalValue == null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberCylindricalValue,
                                    Axe = (c.Axis == null || c.Axis == "") ? "" : c.Axis,
                                    Add = (orderlensproduct.LensNumber.LensNumberAdditionValue == null || orderlensproduct.LensNumber.LensNumberAdditionValue == "") ? "" : orderlensproduct.LensNumber.LensNumberAdditionValue
                                });

                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 4,
                                    Designation = Resources.lenses,
                                    Qte = "",
                                    PUHT = "",
                                    PercRed = "",
                                    MntHT = ""
                                });

                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 5,
                                    Designation = c.OeilDroiteGauche.ToString() + " " + lenscate,
                                    Qte = c.LineQuantity.ToString("N0"),
                                    PUHT = c.LineUnitPrice.ToString("N0"),
                                    PercRed = "0",
                                    MntHT = c.LineAmount.ToString("N0")
                                });
                            }
                            else
                            {
                                k += 1;
                                LstRxPrescription.Add(
                                new RxPrescription
                                {
                                    LineNumber = k,
                                    Field1 = Resources.DateExpiration,
                                    Field2 = (currentOrder.DateExpiration.HasValue) ? currentOrder.DateExpiration.Value.ToString("dd/MM/yyyyy") : currentOrder.CustomerOrderDate.AddYears(1).ToString("dd/MM/yyyyy"),
                                    Oeil = c.OeilDroiteGauche.ToString(),
                                    Sphere = (orderlensproduct.LensNumber.LensNumberSphericalValue == null || orderlensproduct.LensNumber.LensNumberSphericalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberSphericalValue,
                                    Cylindre = (orderlensproduct.LensNumber.LensNumberCylindricalValue == null || orderlensproduct.LensNumber.LensNumberCylindricalValue == "") ? "" : orderlensproduct.LensNumber.LensNumberCylindricalValue,
                                    Axe = (c.Axis == null || c.Axis == "") ? "" : c.Axis,
                                    Add = (orderlensproduct.LensNumber.LensNumberAdditionValue == null || orderlensproduct.LensNumber.LensNumberAdditionValue == "") ? "" : orderlensproduct.LensNumber.LensNumberAdditionValue
                                });

                                LstDetailSales.Add(
                                new DetailSales
                                {
                                    LineNumber = 6,
                                    Designation = c.OeilDroiteGauche.ToString() + " " + lenscate,
                                    Qte = c.LineQuantity.ToString("N0"),
                                    PUHT = c.LineUnitPrice.ToString("N0"),
                                    PercRed = "0",
                                    MntHT = c.LineAmount.ToString("N0")
                                });
                            }

                        }
                    }

                    model.RptReceiptPaymentDetailID = currentOrder.DatailBill;
                    model.CompanyName = Company.Name;
                    model.CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel;
                    model.CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber;
                    model.BranchName = currentOrder.Branch.BranchName;
                    model.BranchAdress = currentOrder.Branch.Adress.Quarter.QuarterLabel + " - " + currentOrder.Branch.Adress.Quarter.Town.TownLabel;
                    model.CompanyRC = Company.CompanyTradeRegister;
                    model.Ref = currentOrder.NumeroFacture;
                    model.CompanyCNI = "NO CONT : " + Company.CNI;
                    model.Operator = CurrentUser.Name + " " + CurrentUser.Description;
                    model.CustomerName = currentOrder.CustomerName;
                    model.InsurreName = currentOrder.InsurreName;
                    model.CustomerAccount = currentOrder.Assureur.Name;
                    model.SaleDate = currentOrder.CustomerOrderDate;
                    model.SaleDateHours = currentOrder.CustomerDateHours.Value;
                    model.Title = Company.Adress.Quarter.Town.TownLabel;
                    model.PoliceAssurance = currentOrder.PoliceAssurance;//police d'assurance;
                    model.MedecinTraitant = currentOrder.MedecinTraitant;
                    model.Relation = (currentOrder.Relation!=null) ? currentOrder.Relation:"None";

                    model.InsureCompanyName = currentOrder.CompanyName; //NOM DE LA SOCIETE DU CLIENT
                    model.NumBPCDate = currentOrder.NumeroBonPriseEnCharge;
                    model.BranchTel = "Tel: " + currentOrder.Branch.Adress.AdressCellNumber + "/" + currentOrder.Branch.Adress.AdressPhoneNumber;


                    model.TotalAmount = InitialTotalAmount;
                    model.RemiseAssurance = currentOrder.RemiseAssurance;

                    model.MontantLettre = LoadComponent.Int2Lettres((Int32)InitialTotalAmount).ToUpper(); ;//MONTANT FACTURE,
                    model.MontantLettreEN = NumberConverter.Spell((long)InitialTotalAmount).ToUpper();


                    model.LstRxPrescription = LstRxPrescription;
                    model.LstDetailSales = LstDetailSales;

                    

                }

                return model;

            }
            catch (Exception ex)
            {
                RedirectToAction("Index", "Home");
                throw;
            }
        }
        #endregion

        public ActionResult RptFacture()
        {
            //Resources.Culture = CultureInfo.GetCultureInfo("fr-FR");
            CultureInfo enUsCI = CultureInfo.GetCultureInfo("fr-FR");
            var model = GenerateFacture(enUsCI);

            return View(model);

        }
        public ActionResult RptFactureEN()
        {
            //CultureInfo enUsCI = CultureInfo.GetCultureInfo("en-US");
            CultureInfo enUsCI = CultureInfo.GetCultureInfo("en-US");
            var model = GenerateFacture(enUsCI);

            return View(model);

        }
        public ActionResult RptFactureValidate(int CustomerOrderID)
        {
            Session["Receipt_CustomerOrderID"] = CustomerOrderID;
            var model = GenerateFacture();
            return View(model);
        }
        public ActionResult RptBordero()
        {
            var model = GenerateFacture();

            return View(model);

        }
        
        //public ActionResult RptBorderoDepot()
        //{
        //    var model = GenerateBorderoDePot();

        //    return View(model);
        //}

        public ActionResult RptBorderoDepot(int ? BorderoDepotID)
        {
            if (BorderoDepotID.HasValue && BorderoDepotID!=null && BorderoDepotID>0)
            {
                Session["BorderoDepotID"] = BorderoDepotID;
            }
            var model = GenerateBorderoDePot();
            return View(model);
        }

        public RptReceipt GenerateBorderoDePot()
        {
            try
            {

                int i = 0;
                
                int BorderoDepotID = (Session["BorderoDepotID"] == null) ? 0 : (int)Session["BorderoDepotID"];

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;

                List<ReceiptLine> receiptline = new List<ReceiptLine>();

                

                RptReceipt model = new RptReceipt();

                if (BorderoDepotID > 0)//depot pour une vente
                {
                    
                    var currentOrder = db.CustomerOrders.Join(db.BorderoDepots, c => c.BorderoDepotID, b => b.BorderoDepotID,
                    (c, b) => new { c, b }).Where(cb => cb.c.BorderoDepotID.HasValue && cb.c.BorderoDepotID.Value == BorderoDepotID)
                    .Select(s => new
                    {
                        Branch = s.c.Branch,
                        Assureur = s.c.Assureur,
                        LieuxdeDepotBordero = s.c.LieuxdeDepotBordero,
                        NumeroFacture=s.b.CodeBorderoDepot,
                        RemiseAssurance=s.c.RemiseAssurance
                    }).FirstOrDefault();


                   var list = db.CustomerOrders.Join(db.BorderoDepots, c => c.BorderoDepotID, b => b.BorderoDepotID,
                    (c, b) => new { c, b })
                    .Join(db.CumulSaleAndBills, pr => pr.c.CustomerOrderID, pl => pl.CustomerOrderID, (pr, pl) => new { pr, pl })
                    .Where(cb => cb.pr.c.BorderoDepotID.HasValue && cb.pr.c.BorderoDepotID.Value== BorderoDepotID)
                    .Select(s => new
                    {
                        InsurreName = s.pr.c.InsurreName,
                        CustomerName = s.pr.c.CustomerName,
                        CompanyName = s.pr.c.CompanyName,
                        MontantFacture = s.pr.c.Plafond, // / (1 - s.pr.c.RemiseAssurance / 100),
                        NumeroFacture = s.pr.c.NumeroFacture,
                        DeliveryDate= (s.pl.ProductDeliverDate.HasValue || s.pl.ProductDeliverDate!=null) ? s.pl.ProductDeliverDate.Value : new DateTime(1900,1,1)
                    }).ToList();

                    if (list.Count==0)
                    {
                        
                        var newlist = db.CustomerOrders.Join(db.BorderoDepots, c => c.BorderoDepotID, b => b.BorderoDepotID,
                        (c, b) => new { c, b })
                        .Where(cb => cb.c.BorderoDepotID.HasValue && cb.c.BorderoDepotID.Value == BorderoDepotID)
                        .Select(s => new
                        {
                            InsurreName = s.c.InsurreName,
                            CustomerName = s.c.CustomerName,
                            CompanyName = s.c.CompanyName,
                            MontantFacture = s.c.Plafond, // / (1-s.c.RemiseAssurance/100), //s.c.Plafond,
                            NumeroFacture = s.c.NumeroFacture,
                            DeliveryDate = new DateTime(1900,1,1)
                        }).ToList();

                        list = newlist;
                    }
                   
                    double totAmountBordero = list.Sum(l => l.MontantFacture);
                    double totAmountWithReduction =totAmountBordero- (totAmountBordero* currentOrder.RemiseAssurance/100);
                    //fabrication du detail
                    list.ForEach(c =>
                    {
                        i += 1;


                        receiptline.Add(
                        new ReceiptLine
                        {
                            ReceiptLineID = i,
                            DetailQty = c.MontantFacture.ToString("N0"),//pr des besoin d'affichage ce champs sera use pour les montants
                            Designation = c.InsurreName,
                            ProducType = c.CustomerName,  //sera use pr afficher le nom du client
                            Reference = c.CompanyName,
                            NumeroFacture=c.NumeroFacture,
                            DeliveryDate= (c.DeliveryDate != null) ? c.DeliveryDate.ToString("dd/MM/yyyy") : ""
                        });
                            
                    });

                    model.CompanyName = Company.Name;
                    model.CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel;
                    model.CompanyTel = "Tel: " + Company.Adress.AdressPhoneNumber;
                    
                    
                    
                    model.Ref = currentOrder.NumeroFacture;
                    model.CompanyCNI = "NO CONT : " + Company.CNI;

                    model.Operator = (currentOrder.Assureur.Adress.AdressFax == null || currentOrder.Assureur.Adress.AdressFax == "") ? ""  : "Adresse: " + currentOrder.Assureur.Adress.AdressFax; // CurrentUser.Name + " " + CurrentUser.Description;

                    model.Title = "BILL NO "+currentOrder.NumeroFacture;
                    model.CustomerAccount = currentOrder.Assureur.Name;
                    model.BranchAdress = "BP:" + currentOrder.Assureur.Adress.AdressPOBox + " - " + currentOrder.Assureur.Adress.Quarter.Town.TownLabel;
                    model.BranchTel = "Tel: " + currentOrder.Assureur.Adress.AdressPhoneNumber;
                    model.CompanyRC = Company.CompanyTradeRegister;
                    model.BranchName = (currentOrder.Assureur.CompanyTradeRegister==null || currentOrder.Assureur.CompanyTradeRegister=="") ? "" : "No CONTRIBUABLE: " + currentOrder.Assureur.CompanyTradeRegister;

                    model.ReceiptLines = receiptline;
                    model.DeviseLabel = Company.Adress.Quarter.Town.TownLabel.ToUpper();
                    model.SaleDateHours = DateTime.Now;
                    
                    model.TotalAmount = totAmountBordero;
                    model.TotalAmountTTC = totAmountWithReduction;
                    model.RemiseAssurance = currentOrder.RemiseAssurance;

                    model.ReceiptLines = receiptline;
                    model.MontantLettre = LoadComponent.Int2Lettres((Int32)totAmountWithReduction).ToUpper(); 
                    model.MontantLettreEN = NumberConverter.Spell((long)totAmountWithReduction).ToUpper();
                }
                return model;
            }
            catch (Exception ex)
            {
                RedirectToAction("Index", "Home");
                throw;
            }

        }
        

        #region ASCOMA
        public ActionResult RptBorderoDepotAscoma(int? BorderoDepotID)
        {

            if (BorderoDepotID.HasValue && BorderoDepotID != null && BorderoDepotID > 0)
            {
                Session["BorderoDepotID"] = BorderoDepotID;
            }

            var model = GenerateBorderoDePotAscoma();

            return View(model);
        }

        public RptReceipt GenerateBorderoDePotAscoma()
        {
            try
            {

                int i = 0;

                int BorderoDepotID = (Session["BorderoDepotID"] == null) ? 0 : (int)Session["BorderoDepotID"];

                @ViewBag.CompanyLogoID = Company.GlobalPersonID;

                List<ReceiptLine> receiptLines = new List<ReceiptLine>();



                RptReceipt model = new RptReceipt();

                if (BorderoDepotID > 0)//depot pour une vente
                {

                    var currentOrder = db.CustomerOrders.Join(db.BorderoDepots, c => c.BorderoDepotID, b => b.BorderoDepotID,
                    (c, b) => new { c, b }).Where(cb => cb.c.BorderoDepotID.HasValue && cb.c.BorderoDepotID.Value == BorderoDepotID)
                    .Select(s => new
                    {
                        Branch = s.c.Branch,
                        Assureur = s.c.Assureur,
                        LieuxdeDepotBordero = s.c.LieuxdeDepotBordero,
                        NumeroFacture = s.b.CodeBorderoDepot,
                        TreatmentDate = s.c.TreatmentDate,
                        CustomerOrderDate = s.c.CustomerOrderDate
                    }).FirstOrDefault();


                    var list = db.CustomerOrders.Join(db.BorderoDepots, c => c.BorderoDepotID, b => b.BorderoDepotID,
                     (c, b) => new { c, b })
                     .Join(db.CumulSaleAndBills, pr => pr.c.CustomerOrderID, pl => pl.CustomerOrderID, (pr, pl) => new { pr, pl })
                     .Where(cb => cb.pr.c.BorderoDepotID.HasValue && cb.pr.c.BorderoDepotID.Value == BorderoDepotID)
                     .Select(s => new
                     {
                         InsurreName = s.pr.c.InsurreName,
                         TreatmentDate = s.pr.c.TreatmentDate,
                         CustomerOrderDate = s.pr.c.CustomerOrderDate,
                         CustomerName = s.pr.c.CustomerName,
                         CompanyName = s.pr.c.CompanyName,
                         Plafond = s.pr.c.Plafond,
                         TotalMalade = s.pr.c.TotalMalade,
                         MontantBrut = s.pr.c.Plafond + s.pr.c.TotalMalade,
                         NumeroFacture = s.pr.c.NumeroFacture,
                         MatriculePatient = s.pr.c.PoliceAssurance,
                         DeliveryDate = (s.pl.ProductDeliverDate.HasValue || s.pl.ProductDeliverDate != null) ? s.pl.ProductDeliverDate.Value : new DateTime(1900, 1, 1)
                     }).ToList();

                    if (list.Count == 0)
                    {
                        var newlist = db.CustomerOrders.Join(db.BorderoDepots, c => c.BorderoDepotID, b => b.BorderoDepotID,
                        (c, b) => new { c, b })
                        .Where(cb => cb.c.BorderoDepotID.HasValue && cb.c.BorderoDepotID.Value == BorderoDepotID)
                        .Select(s => new
                        {
                            InsurreName = s.c.InsurreName,
                            TreatmentDate = s.c.TreatmentDate,
                            CustomerOrderDate = s.c.CustomerOrderDate,
                            CustomerName = s.c.CustomerName,
                            CompanyName = s.c.CompanyName,
                            Plafond = s.c.Plafond,
                            TotalMalade = s.c.TotalMalade,
                            MontantBrut = s.c.Plafond + s.c.TotalMalade,
                            NumeroFacture = s.c.NumeroFacture,
                            MatriculePatient = s.c.PoliceAssurance,
                            DeliveryDate = new DateTime(1900, 1, 1)
                        }).ToList();

                        list = newlist;
                    }

                    double totalPlafond = list.Sum(l => l.Plafond);
                    double sommeTotalMalade = list.Sum(l => l.TotalMalade);
                    double sommeMontantBrut = list.Sum(l => l.MontantBrut);
                    //fabrication du detail
                    list.ForEach(c =>
                    {
                        i += 1;


                        receiptLines.Add(
                        new ReceiptLine
                        {
                            ReceiptLineID = i,
                            DetailQty = c.Plafond.ToString(),//pr des besoin d'affichage ce champs sera use pour les montants
                            TotalMalade = c.TotalMalade.ToString(),
                            MontantBrut = c.MontantBrut.ToString(),
                            Designation = c.InsurreName,
                            ProducType = c.CustomerName,  //sera use pr afficher le nom du client
                            Reference = c.CompanyName,
                            NumeroFacture = c.NumeroFacture,
                            MatriculePatient = c.MatriculePatient,
                            DeliveryDate = (c.DeliveryDate != null) ? c.DeliveryDate.ToString("dd/MM/yyyy") : "",
                            TreatmentDate = ((c.TreatmentDate != null) ? c.TreatmentDate?.ToString("dd/MM/yyyy") :
                                                                        c.CustomerOrderDate.ToString("dd/MM/yyyy")),

                        });

                    });

                    model.CompanyName = Company.Name;
                    model.CompanyAdress = "Head Quater : " + Company.Adress.Quarter.Town.Region.RegionLabel + " - " + Company.Adress.Quarter.Town.TownLabel;
                    model.CompanyTel = Company.Adress.AdressPhoneNumber;



                    model.Ref = currentOrder.NumeroFacture;
                    model.CompanyCNI = Company.CNI;

                    model.Operator = (currentOrder.Assureur.Adress.AdressFax == null || currentOrder.Assureur.Adress.AdressFax == "") ? "" : "Adresse: " + currentOrder.Assureur.Adress.AdressFax; // CurrentUser.Name + " " + CurrentUser.Description;

                    model.Title = currentOrder.NumeroFacture;
                    model.CustomerAccount = currentOrder.Assureur.Name;
                    model.BranchPOBox = currentOrder.Branch.Adress.AdressPOBox;
                    model.CompanyTown = Company.Adress.Quarter.Town.TownLabel;
                    model.BranchTel = currentOrder.Branch.Adress.AdressPhoneNumber;
                    model.CompanyRC = Company.CompanyTradeRegister;
                    model.BranchEmailAdress = currentOrder.Branch.Adress.AdressEmail;
                    model.AdressFullName = currentOrder.Branch.Adress.AdressFullName;
                    model.BranchPhoneNumber = currentOrder.Branch.Adress.AdressPhoneNumber;
                    model.CompanyTradeRegister = Company.CompanyTradeRegister;
                    model.ONOCNumber = Company.ONOCNumber;

                    var fileToRetrieve = db.Files.FirstOrDefault(f => f.GlobalPersonID == Company.GlobalPersonID);
                    model.CompanyLogoID = fileToRetrieve != null ? fileToRetrieve.FileID : 0;

                    model.ReceiptLines = receiptLines;
                    model.DeviseLabel = Company.Adress.Quarter.Town.TownLabel.ToUpper();
                    model.SaleDateHours = DateTime.Now;

                    model.SommePlafond = totalPlafond;
                    model.SommeTotalMalade = sommeTotalMalade;
                    model.SommeMontantBrut = sommeMontantBrut;


                    model.ReceiptLines = receiptLines;
                    model.MontantLettre = LoadComponent.Int2Lettres((Int32)totalPlafond).ToUpper();
                    model.MontantLettreEN = NumberConverter.Spell((long)totalPlafond).ToUpper();
                }
                return model;
            }
            catch (Exception ex)
            {
                RedirectToAction("Index", "Home");
                throw;
            }

        }

        #endregion

        private Company Company
        {
            get
            {
                return db.Companies.FirstOrDefault();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}