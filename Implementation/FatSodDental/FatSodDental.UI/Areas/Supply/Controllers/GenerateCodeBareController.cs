using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Ext.Net;
using Ext.Net.MVC;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSod.Report.WrapReports;
using FatSodDental.UI.Controllers;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using BarcodeLib;
using System.Drawing;
//using Neodynamic.Web.MVC.Barcode;

namespace FatSodDental.UI.Areas.Supply.Controllers
{
    public class GenerateCodeBareController : BaseController
    {
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Supply/GenerateCodeBare";
        private const string VIEW_NAME = "Index";
        //person repository

        private IBarCodeGenerator _barCodeGeneratorRepository;
        private Company cmpny;

        public GenerateCodeBareController(
            IBarCodeGenerator barCodeGeneratorRepository
            )
        {
            this._barCodeGeneratorRepository = barCodeGeneratorRepository;
        }
        //
        // GET: /Sale/GenerateCodeBare/
        
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {

            BarCodeGenerator BarCodeGen = new BarCodeGenerator();
            
            return View(ModelAcctOp(BarCodeGen));
        }
        
        public ActionResult DisplayEntries(BarCodeGenerator BarCodeGen)
        {
            try
            {
                BarCodeGen.GeneratedByID = this.SessionGlobalPersonID;
                BarCodeGen.GenerateDate = SessionBusinessDay(null).BDDateOperation;

                _barCodeGeneratorRepository.valideBarCodeGenerator(BarCodeGen);

                Session["BareCode"] = BarCodeGen;
                this.PartialReset();
                return this.Direct();
            }
            catch (Exception e) 
            { 
                X.Msg.Alert("Error ", e.Message + " " + e.StackTrace + " " + e.InnerException).Show(); 
                return this.Direct(); 
            }
        
        }
        public DirectResult RemoveGenerateBarCode(int ID)
        {
            try
            {
                _barCodeGeneratorRepository.supprimeBarCodeGenerator(ID);
                this.GetCmp<Store>("Store").Reload();
                this.AlertSucces(Resources.Success, "Command has been deleted");
                return this.Direct();
            }
            catch (Exception e)
            {
                X.Msg.Alert("Error", e.Message).Show();
                return this.Direct();
            }
        }
        [HttpPost]
        public ActionResult UpdateLine(int ID)
        {


            try
            {
                int BranchID = SessionBusinessDay(null).BranchID;
                BarCodeGenerator upBarCodeGenerator = db.BarCodeGenerators.Find(ID);
                this.GetCmp<ComboBox>("BranchID").SetValue(BranchID);
                this.GetCmp<ComboBox>("ProductID").SetValue(upBarCodeGenerator.ProductID);
                this.GetCmp<NumberField>("QtyGenerate").SetValue(upBarCodeGenerator.QtyGenerate);

                Session["BareCode"] = upBarCodeGenerator;
                
                return this.Direct();
            }
            catch (Exception e) { X.Msg.Alert("Error ", e.Message).Show(); return this.Direct(); }
        }
        public void Reset()
        {
            this.GetCmp<FormPanel>("RptGenerateCodeBare").Reset(true);
            this.PartialReset();
        }
        public void PartialReset()
        {
            this.GetCmp<Store>("Store").Reload();
        }
        [HttpPost]
        public StoreResult GetList()
        {
            return this.Store(ModelAcctOp((BarCodeGenerator)Session["BareCode"]));
        }

        private List<object> ModelAcctOp(BarCodeGenerator BarCodeGen) //, string BeginDate, string EndDate)
        {
            List<object> list = new List<object>();
            //recuperation de ttes les operation du type op choisi

            List<BarCodeGenerator> listBarCodeGen = db.BarCodeGenerators.Where(ao => ao.GeneratedByID==SessionGlobalPersonID ).ToList(); //.ProductID == BarCodeGen.ProductID && ao.CodeBar == BarCodeGen.CodeBar).ToList();

            listBarCodeGen.ForEach(c =>
            {
                list.Add(
                            new
                            {
                                BarCodeGeneratorID = c.BarCodeGeneratorID,
                                CodeBar = c.CodeBar,
                                ProductCode = c.Product.ProductCode,
                                GeneratedByID = c.GeneratedByID,
                                GenerateDate = c.GenerateDate,
                                QtyGenerate = c.QtyGenerate
                            }
                        );
                
            });
            //}

            return list;

        }
        private List<object> ModelRepAcctOp(BarCodeGenerator barcodeGen)
        {
            Product prod = db.Products.Find(barcodeGen.ProductID);
            List<object> list = new List<object>();
            //BarcodeProfessional bcp = new BarcodeProfessional();
            ////Set the desired barcode type or symbology
            //bcp.Symbology = Neodynamic.Web.MVC.Barcode.Symbology.Code39;
            ////Set value to encode
            //bcp.Code = barcodeGen.CodeBar;
            ////Generate barcode image
            //byte[] imgBuffer = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png);

            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
            {
                IncludeLabel = true,
                Alignment = AlignmentPositions.CENTER,
                Width = 300,
                Height = 100,
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                BackColor = Color.White,
                ForeColor = Color.Black,
            };
            System.Drawing.Image img = barcode.Encode(TYPE.CODE128, barcodeGen.CodeBar);

            byte[] imgBuffer = CopyImageToByteArray(img);

            for (int i = 0; i < barcodeGen.QtyGenerate; i++)
            {
                
                list.Add(
                             new
                             {
                                 RptBareCodeID = i,
                                 BareCode = barcodeGen.CodeBar,
                                 ProductName = prod.ProductLabel,
                                 ProductDescription = prod.ProductDescription,
                                 Price = prod.SellingPrice,
                                 BarcodeImage = imgBuffer
                             }
                         );
            }
            
            return list;

        }

        public static byte[] CopyImageToByteArray(System.Drawing.Image theImage)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                theImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }
        public void ShowGenericRpt()
        {
            ReportDocument rptH = new ReportDocument();
            try
            {
                bool isValid = true;

                string strReportName = Session["ReportName"].ToString();    // Setting ReportName
                string strOperator1 = Session["Operator"].ToString();         // Setting Operator1
                
                var rptSource = Session["rptSource"];

                if (string.IsNullOrEmpty(strReportName) && rptSource==null)
                {
                    isValid = false;
                }

                if (isValid)
                {
                    
                    string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports//Supply//" + strReportName + ".rpt";
                    rptH.Load(strRptPath);
                    if (rptSource != null && rptSource.GetType().ToString() != "System.String") rptH.SetDataSource(rptSource);
                    if (!string.IsNullOrEmpty(strOperator1)) rptH.SetParameterValue("Operator", strOperator1);

                    bool isDownloadRpt = (bool)Session["isDownloadRpt"];
                    rptH.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, strReportName);

                    // Clear all sessions value
                    Session["ReportName"] = null;
                    Session["Operator"] = null;
                    Session["BareCode"] = null;
                    
                }
                else
                {
                    Response.Write("Nothing Found; No Report name found");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            finally
            {
                rptH.Close();
                rptH.Dispose();
            }
        }

        //This method load a method that print 
        public ActionResult PrintReport()
        {
            this.GetCmp<Panel>("PanelReport").LoadContent(new ComponentLoader
            {
                Url = Url.Action("ShowGeneric"),
                DisableCaching = false,
                Mode = LoadMode.Frame,
            });

            return this.Direct();
        }

        /// This is used for showing Generic Report(with data and report parameter) in a same window       
        public ActionResult ShowGeneric()
        {
            // Clear all sessions value
            Session["ReportName"] = null;
            
            Session["Operator"] = null;

            BarCodeGenerator BareCode = (BarCodeGenerator)Session["BareCode"];
            this.Session["ReportName"] = "RptBarCodeGenerator";
        
            this.Session["Operator"] = CurrentUser.Name;
            if (BareCode != null)
            {
                this.Session["rptSource"] = ModelRepAcctOp(BareCode);
            }
            else
            {
                this.Session["rptSource"] = null;
            }

            return RedirectToAction("ShowGenericRpt", "GenerateCodeBare");
        }
    }
}