using FatSod.DataContext.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.DataContext.Initializer;
using FatSod.Supply.Abstracts;
using FatSod.DataContext.Repositories;
using FatSod.Security.Abstracts;
namespace FatSod.Supply.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class LensConstruction
    {
        private static EFDbContext _context;
        /// <summary>
        /// 
        /// </summary>
        public LensConstruction()
        {
            _context = new EFDbContext();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public LensConstruction(EFDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CategoryCode"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static LensCategory PersistLensCategory(string CategoryCode, EFDbContext context)
        {
            LensCategory res = new LensCategory();

            //Création de la catégore de verre
            res = LensConstruction.GetLensCategoryByCategoryCode(CategoryCode, context);

            if (res.CategoryID > 0)
            {
                return res;
            }

            //1 - Numéro de compte du produit
            IAccount accountRepo = new AccountRepository(context);
            int maxColAccount = context.CollectifAccounts
                                       .Where(cac1 => cac1.AccountingSection.AccountingSectionCode == CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD)
                                       .Max(cac => cac.CollectifAccountNumber);
            CollectifAccount maxColAccount1 = context.CollectifAccounts.FirstOrDefault(cac => cac.CollectifAccountNumber == maxColAccount);
            CollectifAccount colAccount = new CollectifAccount()
            {
                AccountingSectionID = maxColAccount1.AccountingSectionID,
                CollectifAccountNumber = ++maxColAccount1.CollectifAccountNumber,
                CollectifAccountLabel = CategoryCode,
            };

            colAccount = context.CollectifAccounts.Add(colAccount);
            res.CollectifAccountID = colAccount.CollectifAccountID;

            res = context.LensCategories.Add(res);
            res.CollectifAccount = colAccount;
            return res;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CategoryCode"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static LensCategory GetLensCategoryByCategoryCode(string CategoryCode, EFDbContext context)
        {
            LensCategory res = new LensCategory();

            ILensCategory lensCategoryRepo = new LensCategoryRepository(context);
            _context = context;

            res = context.LensCategories.FirstOrDefault(lc => lc.CategoryCode == CategoryCode);

            if (res != null && res.CategoryID > 0)
            {
                return res;
            }

            res = new LensCategory
            {

                //Propriétes correspondant au verre
                //bifocalcode
                BifocalCode = GetBifocalCode(CategoryCode),
                //Progressive ?
                IsProgressive = IsLensProgressive(CategoryCode),
                //la matière
                LensMaterial = GetLensMaterialByCode(GetLensMaterialCodeByStockName(CategoryCode)),
                LensMaterialID = GetLensMaterialByCode(GetLensMaterialCodeByStockName(CategoryCode)).LensMaterialID,
                //Traitement
                LensCoating = GetLensCoatingByCode(GetLensCoatingCodeByStockName(CategoryCode)),
                LensCoatingID = GetLensCoatingByCode(GetLensCoatingCodeByStockName(CategoryCode)).LensCoatingID,
                //la colour
                LensColour = GetLensColourByCode(GetLensColourCodeByStockName(CategoryCode)),
                LensColourID = GetLensColourByCode(GetLensColourCodeByStockName(CategoryCode)).LensColourID,
                CategoryCode = CategoryCode,
            };
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetProductBySaleLine(SaleLine saleLine, EFDbContext context)
        {
            _context = context;
            Product res = new Product();
            //traitement pr verre
            if (saleLine.LensCategoryCode != null)
            {
                res = (Product)GetOrderLensByCustOrdLine(saleLine, _context);
            }
            else
            {
                res = GetFrameByCustOrdLine(saleLine, _context);
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetProductByAuthSaleLine(AuthoriseSaleLine saleLine, EFDbContext context)
        {
            _context = context;
            Product res = new Product();
            //traitement pr verre
            if (saleLine.LensCategoryCode != null)
            {
                res = (Product)GetOrderLensByCustOrdLine(saleLine, _context);
            }
            else
            {
                res = GetFrameByCustOrdLine(saleLine, _context);
            }
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetProductByCumulSaleAndBillLine(CumulSaleAndBillLine saleLine, EFDbContext context)
        {
            _context = context;
            Product res = new Product();

            //traitement pr verre
            if (saleLine.LensCategoryCode != null)
            {
                res = (Product)GetOrderLensByCustOrdLine(saleLine, _context);
            }
            else
            {
                res = GetFrameByCustOrdLine(saleLine, _context);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetProductByCumulSaleAndBillLineSOUT(CumulSaleAndBillLine saleLine, EFDbContext context)
        {
            _context = context;
            Product res = new Product();

            //traitement pr verre
            if (saleLine.LensCategoryCode != null)
            {
                res = (Product)GetOrderLensByCustOrdLineSOUT(saleLine, _context);
            }
            else
            {
                res = GetFrameByCustOrdLine(saleLine, _context);
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetFrameByCustOrdLine(SaleLine saleLine, EFDbContext context)
        {
            _context = context;
            Product res = _context.Products.Find(saleLine.ProductID);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetFrameByCustOrdLine(AuthoriseSaleLine saleLine, EFDbContext context)
        {
            _context = context;
            Product res = _context.Products.Find(saleLine.ProductID);
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetFrameByCustOrdLine(CumulSaleAndBillLine saleLine, EFDbContext context)
        {
            _context = context;
            Product res = _context.Products.Find(saleLine.ProductID);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetOrderLensByCustOrdLine(SaleLine saleLine, EFDbContext context)
        {
            _context = context;

            Product res = null;
            LensNumber lensNumber = GetLensNumber(saleLine, _context);

            //if (lensNumber != null)
            //{
            //    res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == saleLine.LensCategoryCode).FirstOrDefault();
            //}
            if (lensNumber != null && lensNumber.LensNumberID > 0)
            {
                res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == saleLine.LensCategoryCode).FirstOrDefault();
                if (res == null)
                {
                    if (saleLine.LensNumberSphericalValue == "0.00")
                    {
                        saleLine.LensNumberSphericalValue = "PLAN";
                        lensNumber = GetLensNumber(saleLine, _context);
                        if (lensNumber != null && lensNumber.LensNumberID > 0)
                        {
                            res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == saleLine.LensCategoryCode).FirstOrDefault();
                        }
                    }
                    //si c'est tjrs null checkons si c'est un orderlens
                    /*if (res == null)
                    {
                        res = _context.Products.OfType<OrderLens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == saleLine.LensCategoryCode).FirstOrDefault();
                    }*/
                }
            }
            if (lensNumber == null || lensNumber.LensNumberID <= 0 || res == null || res.ProductCode.Substring(1, 2) != saleLine.EyeSide.ToString())
            {
                lensNumber = new LensNumber()
                {
                    LensNumberAdditionValue = saleLine.Addition,
                    LensNumberCylindricalValue = saleLine.LensNumberCylindricalValue,
                    LensNumberSphericalValue = saleLine.LensNumberSphericalValue,
                };
                lensNumber.LensNumberDescription = lensNumber.LensNumberFullCode;
                // lensNumber.LensNumberDescription = lensNumber.LensNumberFullCode;
                LensCategory lensCategory = context.LensCategories.FirstOrDefault(col => col.CategoryCode == saleLine.LensCategoryCode);

                if (lensCategory == null || lensCategory.CategoryID <= 0)
                {
                    lensCategory = GetLensCategoryByCategoryCode(saleLine.LensCategoryCode, context);
                }

                res = new Lens
                {
                    LocalizationID = saleLine.LocalizationID,
                    //Propriétes correspondant au verre
                    //le numéro
                    LensNumber = lensNumber,
                    LensNumberID = lensNumber.LensNumberID,
                    /*
                    EyeSide = saleLine.EyeSide,
                    Index = saleLine.Index,
                    Axis = saleLine.Axis,
                    Addition = saleLine.Addition,
                    */
                    CategoryID = lensCategory.CategoryID,
                    LensCategoryID = lensCategory.CategoryID,
                    Category = lensCategory,
                    LensCategory = lensCategory,
                    //ProductCode = lensCategory.CategoryCode + " " + lensNumber.LensNumberFullCode,
                };
                res.ProductCode = GetLensCode((Lens)res);
                res.Prescription = GetLensCodePrescription((Lens)res, saleLine.EyeSide, saleLine.Axis);
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetOrderLensByCustOrdLine(AuthoriseSaleLine saleLine, EFDbContext context)
        {
            _context = context;
            Product res = null;
            LensNumber lensNumber = GetLensNumber(saleLine, _context);

            //if (lensNumber != null)
            //{
            //    res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == saleLine.LensCategoryCode).FirstOrDefault();
            //}

            if (lensNumber != null && lensNumber.LensNumberID > 0)
            {
                List<Lens> lenses = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == saleLine.LensCategoryCode).ToList();
                res = lenses.FirstOrDefault();
                if (res == null)
                {
                    if (saleLine.LensNumberSphericalValue == "0.00")
                    {
                        saleLine.LensNumberSphericalValue = "PLAN";
                        lensNumber = GetLensNumber(saleLine, _context);
                        if (lensNumber != null && lensNumber.LensNumberID > 0)
                        {
                            res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == saleLine.LensCategoryCode).FirstOrDefault();
                        }
                    }
                    //si c'est tjrs null checkons si c'est un orderlens
                    /*if (res == null)
                    {
                        res = _context.Products.OfType<OrderLens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == saleLine.LensCategoryCode).FirstOrDefault();

                    }*/
                }
            }

            if (lensNumber == null || lensNumber.LensNumberID <= 0 || res == null /*|| res.ProductCode.Substring(1, 2) != saleLine.EyeSide.ToString()*/)
            {
                lensNumber = new LensNumber()
                {
                    LensNumberAdditionValue = (saleLine.Addition == null) ? "" : saleLine.Addition,
                    LensNumberCylindricalValue = (saleLine.LensNumberCylindricalValue == null) ? "" : saleLine.LensNumberCylindricalValue,
                    LensNumberSphericalValue = (saleLine.LensNumberSphericalValue == null) ? "" : saleLine.LensNumberSphericalValue,
                };
                lensNumber.LensNumberDescription = lensNumber.LensNumberFullCode;

                LensCategory lensCategory = context.LensCategories.FirstOrDefault(col => col.CategoryCode == saleLine.LensCategoryCode);

                if (lensCategory == null || lensCategory.CategoryID <= 0)
                {
                    lensCategory = GetLensCategoryByCategoryCode(saleLine.LensCategoryCode, context);
                }

                res = new Lens
                {
                    LocalizationID = saleLine.LocalizationID,
                    //Propriétes correspondant au verre
                    //le numéro
                    LensNumber = lensNumber,
                    LensNumberID = lensNumber.LensNumberID,
                    /*
                    EyeSide = saleLine.EyeSide,
                    Index = (saleLine.Index == null) ? "" : saleLine.Index,
                    Axis = (saleLine.Axis == null) ? "" : saleLine.Axis,
                    Addition = (saleLine.Addition == null) ? "" : saleLine.Addition,
                    */
                    CategoryID = lensCategory.CategoryID,
                    LensCategoryID = lensCategory.CategoryID,
                    Category = lensCategory,
                    LensCategory = lensCategory,

                };
                res.ProductCode = GetLensCode((Lens)res);
                res.Prescription = GetLensCodePrescription((Lens)res, saleLine.EyeSide, saleLine.Axis);
            }

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetOrderLensByCustOrdLine(CumulSaleAndBillLine saleLine, EFDbContext context)
        {
            _context = context;
            Product res = null;
            LensNumber lensNumber = GetLensNumber(saleLine, _context);

            if (lensNumber != null)
            {
                res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.LensCategoryID == saleLine.ProductCategoryID).FirstOrDefault();
                if (res == null)
                {
                    if (saleLine.LensNumberSphericalValue == "0.00")
                    {
                        saleLine.LensNumberSphericalValue = "PLAN";
                        lensNumber = GetLensNumber(saleLine, _context);
                        if (lensNumber != null)
                        {
                            res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.LensCategoryID == saleLine.ProductCategoryID).FirstOrDefault();
                        }
                    }
                    //si c'est tjrs null checkons si c'est un orderlens
                    if (res == null)
                    {
                        res = _context.Products.OfType<OrderLens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.LensCategoryID == saleLine.ProductCategoryID).FirstOrDefault();
                    }
                }
            }

            if (lensNumber == null || lensNumber.LensNumberID <= 0 || res == null || res.ProductCode.Substring(1, 2) != saleLine.EyeSide.ToString())
            {
                lensNumber = new LensNumber()
                {
                    LensNumberAdditionValue = (saleLine.Addition == null) ? "" : saleLine.Addition,
                    LensNumberCylindricalValue = (saleLine.LensNumberCylindricalValue == null) ? "" : saleLine.LensNumberCylindricalValue,
                    LensNumberSphericalValue = (saleLine.LensNumberSphericalValue == null) ? "" : saleLine.LensNumberSphericalValue,
                };
                lensNumber.LensNumberDescription = lensNumber.LensNumberFullCode;

                //
                LensCategory lensCategory = context.LensCategories.FirstOrDefault(col => col.CategoryCode == saleLine.LensCategoryCode);

                if (lensCategory == null || lensCategory.CategoryID <= 0)
                {
                    lensCategory = GetLensCategoryByCategoryCode(saleLine.LensCategoryCode, context);
                }

                res = new Lens
                {
                    LocalizationID = saleLine.LocalizationID,
                    //Propriétes correspondant au verre
                    //le numéro
                    LensNumber = lensNumber,
                    LensNumberID = lensNumber.LensNumberID,

                    /*
                    EyeSide = saleLine.EyeSide,
                    Index = (saleLine.Index == null) ? "" : saleLine.Index,
                    Axis = (saleLine.Axis == null) ? "" : saleLine.Axis,
                    Addition = (saleLine.Addition == null) ? "" : saleLine.Addition,
                    */
                    CategoryID = lensCategory.CategoryID,
                    LensCategoryID = lensCategory.CategoryID,
                    Category = lensCategory,
                    LensCategory = lensCategory,
                    //ProductCode = lensCategory.CategoryCode + " " + lensNumber.LensNumberFullCode,
                };
                res.ProductCode = GetOrderLensCode((OrderLens)res);
                res.Prescription = GetOrderLensCodePrescription((OrderLens)res);

            }


            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetOrderLensByCustOrdLineSOUT(CumulSaleAndBillLine saleLine, EFDbContext context)
        {
            _context = context;
            Product res = null;
            LensNumber lensNumber = GetLensNumber(saleLine, _context);

            if (lensNumber != null)
            {
                //si progressif ou bifocal et c'est axer alors c'est un verre de commande
                //recuperation de la category du verre
                LensCategory lensCategory = context.LensCategories.FirstOrDefault(col => col.CategoryID == saleLine.ProductCategoryID);
                if (lensCategory == null || lensCategory.CategoryID <= 0)
                {
                    lensCategory = GetLensCategoryByCategoryCode(saleLine.LensCategoryCode, context);
                }
                //if (lensCategory.TypeLens.ToUpper()== "BIFOCAL" || lensCategory.TypeLens.ToUpper() == "PROG")
                if (saleLine.isCommandGlass)
                {
                    //if ((saleLine.Axis == null) || (saleLine.Axis == ""))
                    //{

                    //}
                    //else
                    //{
                    res = null;
                    goto orderlensproc;
                    //}
                }
                res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.LensCategoryID == saleLine.ProductCategoryID).FirstOrDefault();
                if (res == null)
                {
                    if (saleLine.LensNumberSphericalValue == "0.00")
                    {
                        saleLine.LensNumberSphericalValue = "PLAN";
                        lensNumber = GetLensNumber(saleLine, _context);
                        if (lensNumber != null)
                        {
                            res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.LensCategoryID == saleLine.ProductCategoryID).FirstOrDefault();
                        }
                    }
                    //si c'est tjrs null checkons si c'est un orderlens
                    if (res == null)
                    {
                        res = _context.Products.OfType<OrderLens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.LensCategoryID == saleLine.ProductCategoryID).FirstOrDefault();
                    }
                }
            }
        orderlensproc:
            if (lensNumber == null || lensNumber.LensNumberID <= 0 || res == null)
            {
                lensNumber = new LensNumber()
                {
                    LensNumberAdditionValue = (saleLine.Addition == null) ? "" : saleLine.Addition,
                    LensNumberCylindricalValue = (saleLine.LensNumberCylindricalValue == null) ? "" : saleLine.LensNumberCylindricalValue,
                    LensNumberSphericalValue = (saleLine.LensNumberSphericalValue == null) ? "" : saleLine.LensNumberSphericalValue,
                };
                lensNumber.LensNumberDescription = lensNumber.LensNumberFullCode;

                //
                LensCategory lensCategory = context.LensCategories.FirstOrDefault(col => col.CategoryCode == saleLine.LensCategoryCode);

                if (lensCategory == null || lensCategory.CategoryID <= 0)
                {
                    lensCategory = GetLensCategoryByCategoryCode(saleLine.LensCategoryCode, context);
                }

                res = new OrderLens
                {
                    LocalizationID = saleLine.LocalizationID,
                    //Propriétes correspondant au verre
                    //le numéro
                    LensNumber = lensNumber,
                    LensNumberID = lensNumber.LensNumberID,

                    EyeSide = saleLine.EyeSide,
                    Index = (saleLine.Index == null) ? "" : saleLine.Index,
                    Axis = (saleLine.Axis == null) ? "" : saleLine.Axis,
                    Addition = (saleLine.Addition == null) ? "" : saleLine.Addition,

                    CategoryID = lensCategory.CategoryID,
                    LensCategoryID = lensCategory.CategoryID,
                    Category = lensCategory,
                    LensCategory = lensCategory,
                    //ProductCode = lensCategory.CategoryCode + " " + lensNumber.LensNumberFullCode,
                };
                res.ProductCode = GetOrderLensCode((OrderLens)res);
                res.Prescription = GetOrderLensCodePrescription((OrderLens)res);

            }


            return res;
        }

        //---------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrderLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetProductByCustOrderLine(CustomerOrderLine customerOrderLine, EFDbContext context)
        {
            _context = context;
            Product res = new Product();
            //traitement pr verre
            if (customerOrderLine.LensCategoryCode != null)
            {
                res = (Product)GetOrderLensByCustOrderLine(customerOrderLine, _context);
            }
            else
            {
                res = GetFrameByCustOrderLine(customerOrderLine, _context);
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrderLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetFrameByCustOrderLine(CustomerOrderLine customerOrderLine, EFDbContext context)
        {
            _context = context;
            Product res = _context.Products.Find(customerOrderLine.ProductID);
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrderLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetOrderLensByCustOrderLine(CustomerOrderLine customerOrderLine, EFDbContext context)
        {
            _context = context;
            Product res = null;
            LensNumber lensNumber = GetLensNumber(customerOrderLine, _context);

            if (lensNumber != null)
            {
                res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == customerOrderLine.LensCategoryCode).FirstOrDefault();
            }

            if (lensNumber == null || lensNumber.LensNumberID <= 0 || res == null || res.ProductCode.Substring(1, 2) != customerOrderLine.EyeSide.ToString())
            {

                lensNumber = new LensNumber()
                {
                    LensNumberAdditionValue = (customerOrderLine.Addition == null) ? "" : customerOrderLine.Addition,
                    LensNumberCylindricalValue = (customerOrderLine.LensNumberCylindricalValue == null) ? "" : customerOrderLine.LensNumberCylindricalValue,
                    LensNumberSphericalValue = (customerOrderLine.LensNumberSphericalValue == null) ? "" : customerOrderLine.LensNumberSphericalValue,

                };
                lensNumber.LensNumberDescription = lensNumber.LensNumberFullCode;

                LensCategory lensCategory = context.LensCategories.FirstOrDefault(col => col.CategoryCode == customerOrderLine.LensCategoryCode);

                if (lensCategory == null || lensCategory.CategoryID <= 0)
                {
                    lensCategory = GetLensCategoryByCategoryCode(customerOrderLine.LensCategoryCode, context);
                }

                res = new Lens
                {

                    //Propriétes correspondant au verre
                    //le numéro
                    LensNumber = lensNumber,
                    LensNumberID = lensNumber.LensNumberID,
                    /*
                    EyeSide = customerOrderLine.EyeSide,
                    Index = (customerOrderLine.Index == null) ? "" : customerOrderLine.Index,
                    Axis = (customerOrderLine.Axis == null) ? "" : customerOrderLine.Axis,
                    Addition = (customerOrderLine.Addition == null) ? "" : customerOrderLine.Addition,
                    */
                    CategoryID = lensCategory.CategoryID,
                    LensCategoryID = lensCategory.CategoryID,
                    Category = lensCategory,
                    LensCategory = lensCategory,
                    //ProductCode = lensCategory.CategoryCode + " " + lensNumber.LensNumberFullCode,
                };
                res.ProductCode = GetLensCode((Lens)res);
                res.Prescription = GetLensCodePrescription((Lens)res, customerOrderLine.EyeSide, customerOrderLine.Axis);
            }
            return res;
        }

        //----------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="custOrdLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Product GetOrderLensByCustOrdLine(CustomerOrderLine custOrdLine, EFDbContext context)
        {
            _context = context;

            Product res = null;
            LensNumber lensNumber = GetLensNumber(custOrdLine, _context);

            //if (lensNumber != null)
            //{
            //    res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == custOrdLine.LensCategoryCode).FirstOrDefault();
            //}
            if (lensNumber != null && lensNumber.LensNumberID > 0)
            {
                res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == custOrdLine.LensCategoryCode).FirstOrDefault();
                if (res == null)
                {
                    if (custOrdLine.LensNumberSphericalValue == "0.00")
                    {
                        custOrdLine.LensNumberSphericalValue = "PLAN";
                        lensNumber = GetLensNumber(custOrdLine, _context);
                        if (lensNumber != null && lensNumber.LensNumberID > 0)
                        {
                            res = _context.Products.OfType<Lens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == custOrdLine.LensCategoryCode).FirstOrDefault();
                        }
                    }
                    //si c'est tjrs null checkons si c'est un orderlens
                    /*if (res == null)
                    {
                        res = _context.Products.OfType<OrderLens>().Where(p => p.LensNumberID == lensNumber.LensNumberID && p.Category.CategoryCode == custOrdLine.LensCategoryCode).FirstOrDefault();
                    }*/
                }
            }

            if (lensNumber == null || lensNumber.LensNumberID <= 0 || res == null || res.ProductCode.Substring(1, 2) != custOrdLine.EyeSide.ToString())
            {
                lensNumber = new LensNumber()
                {
                    LensNumberAdditionValue = (custOrdLine.Addition == null) ? "" : custOrdLine.Addition,
                    LensNumberCylindricalValue = (custOrdLine.LensNumberCylindricalValue == null) ? "" : custOrdLine.LensNumberCylindricalValue,
                    LensNumberSphericalValue = (custOrdLine.LensNumberSphericalValue == null) ? "" : custOrdLine.LensNumberSphericalValue,
                };
                lensNumber.LensNumberDescription = lensNumber.LensNumberFullCode;

                LensCategory lensCategory = context.LensCategories.FirstOrDefault(col => col.CategoryCode == custOrdLine.LensCategoryCode);

                if (lensCategory == null || lensCategory.CategoryID <= 0)
                {
                    lensCategory = GetLensCategoryByCategoryCode(custOrdLine.LensCategoryCode, context);
                }

                res = new Lens
                {

                    //Propriétes correspondant au verre
                    //le numéro
                    LensNumber = lensNumber,
                    LensNumberID = lensNumber.LensNumberID,
                    /*
                    EyeSide = custOrdLine.EyeSide,
                    Index = (custOrdLine.Index == null) ? "" : custOrdLine.Index,
                    Axis = (custOrdLine.Axis == null) ? "" : custOrdLine.Axis,
                    Addition = (custOrdLine.Addition == null) ? "" : custOrdLine.Addition,
                    */
                    CategoryID = lensCategory.CategoryID,
                    LensCategoryID = lensCategory.CategoryID,
                    Category = lensCategory,
                    LensCategory = lensCategory,
                    //ProductCode = lensCategory.CategoryCode + " " + lensNumber.LensNumberFullCode,
                };
                res.ProductCode = GetLensCode((Lens)res);
                res.Prescription = GetLensCodePrescription((Lens)res, custOrdLine.EyeSide, custOrdLine.Axis);
            }
            return res;
        }

        /*public static CustomerOrderLine GetCustOrdLineByOrderLens(CustomerOrderLine custOrdLine, OrderLens lens, EFDbContext context)
        {
            _context = context;

            custOrdLine.EyeSide = lens.EyeSide;
            custOrdLine.Addition = lens.Addition;

            custOrdLine.Axis = lens.Axis;
            custOrdLine.Index = lens.Index;
            custOrdLine.LensNumberCylindricalValue = lens.LensNumber.LensNumberCylindricalValue;
            custOrdLine.LensNumberSphericalValue = lens.LensNumber.LensNumberSphericalValue;
            custOrdLine.LensCategoryCode = lens.LensCategory.CategoryCode;
            custOrdLine.ProductCategoryID = lens.ProductDescription;

            return custOrdLine;
        }*/

        public static bool IsLensProgressive(string stockName)
        {
            bool res = false;

            stockName = stockName.ToLowerInvariant();
            string prog1 = "Progressive".ToLowerInvariant();
            string prog2 = "prog".ToLowerInvariant();

            if (stockName.Contains(prog1) || stockName.Contains(prog2))
            {
                res = true;
            }

            return res;
        }
        public static string GetBifocalCode(string stockName)
        {
            string res = "";
            stockName = stockName.ToLowerInvariant();
            string bifocal = "bifocal".ToLowerInvariant();

            if (stockName.Contains(bifocal))
            {
                string sr1 = "round top".ToLowerInvariant();
                string sr2 = "sr".ToLowerInvariant();
                string sr3 = "S.R".ToLowerInvariant();
                if (stockName.Contains(sr1) || stockName.Contains(sr2) || stockName.Contains(sr3))
                {
                    res = "SR";
                    return res;
                }

                string sd1 = "DTOP".ToLowerInvariant();
                string sd2 = "SD".ToLowerInvariant();
                string sd3 = "S.D".ToLowerInvariant();
                if (stockName.Contains(sd1) || stockName.Contains(sd2) || stockName.Contains(sd3))
                {
                    res = "SD";
                    return res;
                }
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lens"></param>
        /// <returns></returns>
        public static string GetOrderLensCode(OrderLens lens)
        {
            string res = "";

            res = res + (lens.EyeSide == EyeSide.OD ? " OD:" : " OG:");

            res = res + ' ' + lens.LensCategory.CategoryCode;

            if (lens.LensNumber.LensNumberSphericalValue != null && lens.LensNumber.LensNumberSphericalValue.Length > 0)
            {
                res = res + ' ' + lens.LensNumber.LensNumberSphericalValue;
            }

            if (lens.LensNumber.LensNumberCylindricalValue != null && lens.LensNumber.LensNumberCylindricalValue.Length > 0)
            {
                res = res + '(' + lens.LensNumber.LensNumberCylindricalValue + 'x' + lens.Axis + ')';
            }

            if (lens.LensNumber.LensNumberAdditionValue != null && lens.LensNumber.LensNumberAdditionValue.Length > 0)
            {
                res = res + ' ' + "ADD " + lens.LensNumber.LensNumberAdditionValue;
            }
            return res.TrimEnd().TrimStart();

        }

        public static string getPrescriptionSummary(ConsultLensPrescription prescription)
        {

            string summary = "";
            string labelProduct = "";
            string label1 = "";
            string label2 = "";
            string label3 = "";

            // OG
            summary = LensConstruction.GetLensCodeByPrescription(prescription, EyeSide.OG) + "<br>";

            // OD
            summary = summary + LensConstruction.GetLensCodeByPrescription(prescription, EyeSide.OD);

            return summary;
        }

        public static string GetLensCodeByPrescription(ConsultLensPrescription prescription, EyeSide EyeSide)
        {
            string res = "";

            res = res + "<strong>" + (EyeSide == EyeSide.OD ? " OD:" : " OG:") + "</strong>";

            res = res + ' ' + prescription.Category.CategoryCode;

            if (EyeSide == EyeSide.OG)
            {
                if (prescription.LSphValue != null && prescription.LSphValue.Length > 0)
                {
                    res = res + ' ' + prescription.LSphValue;
                }

                if (prescription.LCylValue != null && prescription.LCylValue.Length > 0)
                {
                    if (prescription.LAxis != null && prescription.LAxis.Length > 0)
                    {
                        res = res + '(' + prescription.LCylValue + 'x' + prescription.LAxis + ')';
                    } else
                    {
                        res = res + '(' + prescription.LCylValue + ')';
                    }
                }

                if (prescription.LAddition != null && prescription.LAddition.Length > 0)
                {
                    res = res + ' ' + "ADD " + prescription.LAddition;
                }
            }

            if (EyeSide == EyeSide.OD)
            {
                if (prescription.RSphValue != null && prescription.RSphValue.Length > 0)
                {
                    res = res + ' ' + prescription.RSphValue;
                }

                if (prescription.RCylValue != null && prescription.RCylValue.Length > 0)
                {
                    if (prescription.RAxis != null && prescription.RAxis.Length > 0)
                    {
                        res = res + '(' + prescription.RCylValue + 'x' + prescription.RAxis + ')';
                    }
                    else
                    {
                        res = res + '(' + prescription.RCylValue + ')';
                    }

                }

                if (prescription.RAddition != null && prescription.RAddition.Length > 0)
                {
                    res = res + ' ' + "ADD " + prescription.RAddition;
                }
            }


            return res.TrimEnd().TrimStart();

        }

        public static string GetLensCodeBySaleLine(SaleLine saleLine/*, EyeSide eyeSide*/)
        {
            string res = "";

            //res = res + "<strong>" + (saleLine.EyeSide == EyeSide.OD ? " OD:" : " OG:") + "</strong>";
            res = res + "<strong>" + (saleLine.OeilDroiteGauche == EyeSide.OD ? " OD:" : " OG:") + "</strong>";

            res = res + ' ' + saleLine.Product.Category.CategoryCode;

            if (saleLine.LensNumberSphericalValue != null && saleLine.LensNumberSphericalValue.Length > 0)
            {
                res = res + ' ' + saleLine.LensNumberSphericalValue;
            }

            if (saleLine.LensNumberCylindricalValue != null && saleLine.LensNumberCylindricalValue.Length > 0)
            {
                if (saleLine.Axis != null && saleLine.Axis.Length > 0)
                {
                    res = res + '(' + saleLine.LensNumberCylindricalValue + 'x' + saleLine.Axis + ')';
                }
                else
                {
                    res = res + '(' + saleLine.LensNumberCylindricalValue + ')';
                }
            }

            if (saleLine.Addition != null && saleLine.Addition.Length > 0)
            {
                res = res + ' ' + "ADD " + saleLine.Addition;
            }

            return res.TrimEnd().TrimStart();
        }

        public static string GetLensCodeByCustomerOrderLine(CustomerOrderLine customerOrderLine, EyeSide eyeSide)
        {
            string res = "";

            res = res + "<strong>" + (eyeSide == EyeSide.OD ? " OD:" : " OG:") + "</strong>";

            res = res + ' ' + customerOrderLine.Product.Category.CategoryCode;

            if (customerOrderLine.LensNumberSphericalValue != null && customerOrderLine.LensNumberSphericalValue.Length > 0)
            {
                res = res + ' ' + customerOrderLine.LensNumberSphericalValue;
            }

            if (customerOrderLine.LensNumberCylindricalValue != null && customerOrderLine.LensNumberCylindricalValue.Length > 0)
            {
                if (customerOrderLine.Axis != null && customerOrderLine.Axis.Length > 0)
                {
                    res = res + '(' + customerOrderLine.LensNumberCylindricalValue + 'x' + customerOrderLine.Axis + ')';
                }
                else
                {
                    res = res + '(' + customerOrderLine.LensNumberCylindricalValue + ')';
                }
            }

            if (customerOrderLine.Addition != null && customerOrderLine.Addition.Length > 0)
            {
                res = res + ' ' + "ADD " + customerOrderLine.Addition;
            }

            return res.TrimEnd().TrimStart();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lens"></param>
        /// <returns></returns>
        public static string GetOrderLensCodePrescription(OrderLens lens)
        {
            string res = "";

            res = res + (lens.EyeSide == EyeSide.OD ? " OD:" : " OG:");


            if (lens.LensNumber.LensNumberSphericalValue != null && lens.LensNumber.LensNumberSphericalValue.Length > 0)
            {
                res = res + ' ' + lens.LensNumber.LensNumberSphericalValue;
            }

            if (lens.LensNumber.LensNumberCylindricalValue != null && lens.LensNumber.LensNumberCylindricalValue.Length > 0)
            {
                if (lens.Axis.Length > 0)
                {
                    res = res + '(' + lens.LensNumber.LensNumberCylindricalValue + 'x' + lens.Axis + ')';
                }
                else
                {
                    res = res + '(' + lens.LensNumber.LensNumberCylindricalValue + ')';
                }
            }

            if (lens.LensNumber.LensNumberAdditionValue != null && lens.LensNumber.LensNumberAdditionValue.Length > 0)
            {
                res = res + ' ' + "ADD " + lens.LensNumber.LensNumberAdditionValue;
            }
            return res.TrimEnd().TrimStart();

        }

        public static string GetLensCodeByCumulSaleAndBillLine(CumulSaleAndBillLine cumulSaleAndBillLine, EyeSide EyeSide)
        {
            string res = "";

            res = res + "<strong>" + (EyeSide == EyeSide.OD ? " OD:" : " OG:") + "</strong>";

            res = res + ' ' + cumulSaleAndBillLine.LensCategoryCode;

            if (cumulSaleAndBillLine.LensNumberSphericalValue != null && cumulSaleAndBillLine.LensNumberSphericalValue.Length > 0)
            {
                res = res + ' ' + cumulSaleAndBillLine.LensNumberSphericalValue;
            }

            if (cumulSaleAndBillLine.LensNumberCylindricalValue != null && cumulSaleAndBillLine.LensNumberCylindricalValue.Length > 0)
            {
                if (cumulSaleAndBillLine.Axis != null && cumulSaleAndBillLine.Axis.Length > 0)
                {
                    res = res + '(' + cumulSaleAndBillLine.LensNumberCylindricalValue + 'x' + cumulSaleAndBillLine.Axis + ')';
                }
                else
                {
                    res = res + '(' + cumulSaleAndBillLine.LensNumberCylindricalValue + ')';
                }
            }

            if (cumulSaleAndBillLine.Addition != null && cumulSaleAndBillLine.Addition.Length > 0)
            {
                res = res + ' ' + "ADD " + cumulSaleAndBillLine.Addition;
            }

            return res.TrimEnd().TrimStart();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lens"></param>
        /// <param name="EyeSide"></param>
        /// <param name="Axis"></param>
        /// <returns></returns>
        public static string GetLensCodePrescription(Lens lens, EyeSide EyeSide, string Axis)
        {
            string res = "";

            res = res + (EyeSide == EyeSide.OD ? " OD:" : " OG:");


            if (lens.LensNumber.LensNumberSphericalValue != null && lens.LensNumber.LensNumberSphericalValue.Length > 0)
            {
                res = res + ' ' + lens.LensNumber.LensNumberSphericalValue;
            }

            if (lens.LensNumber.LensNumberCylindricalValue != null && lens.LensNumber.LensNumberCylindricalValue.Length > 0)
            {
                if (Axis.Length > 0)
                {
                    res = res + '(' + lens.LensNumber.LensNumberCylindricalValue + 'x' + Axis + ')';
                }
                else
                {
                    res = res + '(' + lens.LensNumber.LensNumberCylindricalValue + ')';
                }

            }

            if (lens.LensNumber.LensNumberAdditionValue != null && lens.LensNumber.LensNumberAdditionValue.Length > 0)
            {
                res = res + ' ' + "ADD " + lens.LensNumber.LensNumberAdditionValue;
            }
            return res.TrimEnd().TrimStart();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lens"></param>
        /// <param name="EyeSide"></param>
        /// <param name="Axis"></param>
        /// <returns></returns>
        public static string GetLensPrescriptionWitchDescription(Lens lens, EyeSide EyeSide, string Axis)
        {
            string res = "";

            res = res + (EyeSide == EyeSide.OD ? " OD:" : " OG:");

            res = res + ' ' + lens.LensCategoryName;

            if (lens.LensNumber.LensNumberSphericalValue != null && lens.LensNumber.LensNumberSphericalValue.Length > 0)
            {
                res = res + ' ' + lens.LensNumber.LensNumberSphericalValue;
            }

            if (lens.LensNumber.LensNumberCylindricalValue != null && lens.LensNumber.LensNumberCylindricalValue.Length > 0)
            {
                if (Axis.Length > 0)
                {
                    res = res + '(' + lens.LensNumber.LensNumberCylindricalValue + 'x' + Axis + ')';
                }
                else
                {
                    res = res + '(' + lens.LensNumber.LensNumberCylindricalValue + ')';
                }

            }

            if (lens.LensNumber.LensNumberAdditionValue != null && lens.LensNumber.LensNumberAdditionValue.Length > 0)
            {
                res = res + ' ' + "ADD " + lens.LensNumber.LensNumberAdditionValue;
            }
            return res.TrimEnd().TrimStart();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lens"></param>
        /// <returns></returns>
        public static string GetLensCode(Lens lens)
        {
            string res = "";
            if (lens.LensCategoryName != null && lens.LensCategoryName.Length > 0)
            {
                res = lens.LensCategoryName;
            }


            if (lens.LensNumber.LensNumberFullCode != null && lens.LensNumber.LensNumberFullCode.Length > 0)
            {
                res = res + ' ' + lens.LensNumber.LensNumberFullCode;
            }

            return res.TrimEnd().TrimStart();

        }
        /*
                public static string GetLensCategory(OrderLens lens)
                {
                    string res = "";


                    if (lens.BifocalCode != null && lens.BifocalCode.Length > 0)
                    {
                        res = "DF";
                    }

                    if (lens.IsProgressive == true)
                    {
                        res = res + ' ' + "PROG";
                    }
                    //le reste du monde

                    if (lens.LensMaterial.LensMaterialCode != null && lens.LensMaterial.LensMaterialCode.Length > 0)
                    {
                        res = res + ' ' + lens.LensMaterial.LensMaterialCode;
                    }

                    if (lens.LensColour.LensColourCode != null && lens.LensColour.LensColourCode.Length > 0 && lens.LensColour.LensColourCode != "DefaultLensColour")
                    {
                        res = res + ' ' + lens.LensColour.LensColourCode;
                    }

                    if (lens.LensCoating.LensCoatingCode != null && lens.LensCoating.LensCoatingCode.Length > 0 && lens.LensCoating.LensCoatingCode != "DefaultLensCoating")
                    {
                        res = res + ' ' +
                        lens.LensCoating.LensCoatingCode;
                    }

                    if (lens.BifocalCode != null && lens.BifocalCode.Length > 0)
                    {
                        res = res + ' ' + lens.BifocalCode;
                    }

                    if (lens.LensOtherCriterion != null && lens.LensOtherCriterion.Length > 0)
                    {
                        res = res + ' ' + '(' + lens.LensOtherCriterion + ')';
                    }

                    return res.TrimEnd().TrimStart();

                }
        */
        public static string GetLensMaterialCodeByStockName(string stockName)
        {
            string res = "";

            stockName = stockName.ToLowerInvariant();
            string mineral1 = "Mineral".ToLowerInvariant();
            string mineral2 = "MIN".ToLowerInvariant();
            string mineral3 = "MINI".ToLowerInvariant();

            if (stockName.Contains(mineral1) || stockName.Contains(mineral2) || stockName.Contains(mineral2))
            {
                res = "MIN";
                return res;
            }

            string organic1 = "Organic".ToLowerInvariant();
            string organic3 = "Organique".ToLowerInvariant();
            string organic2 = "CR39".ToLowerInvariant();

            if (stockName.Contains(organic1) || stockName.Contains(organic2) || stockName.Contains(organic2))
            {
                res = "CR39";
                return res;
            }

            return res;
        }
        public static LensMaterial GetLensMaterialByCode(string LensMaterialCode)
        {
            LensMaterial res = null;

            LensMaterialCode = LensMaterialCode.ToLowerInvariant();

            //res = _context.LensMaterials.SingleOrDefault(lm => lm.LensMaterialCode.ToLowerInvariant().Equals(LensMaterialCode));

            foreach (LensMaterial genProd in _context.LensMaterials)
            {
                string oldCode = genProd.LensMaterialCode.ToLowerInvariant();
                if (oldCode.Equals(LensMaterialCode))
                {
                    res = genProd;
                    break;
                }
            }
            return res;
        }
        public static string GetLensCoatingCodeByStockName(string stockName)
        {
            string res = "DefaultLensCoating";

            stockName = stockName.ToLowerInvariant();
            string ar = "ar".ToLowerInvariant();

            if (stockName.Contains(ar))
            {
                res = "AR";
                return res;
            }
            return res;
        }
        public static LensCoating GetLensCoatingByCode(string LensCoatingCode)
        {
            LensCoating res = null;

            LensCoatingCode = LensCoatingCode.ToLowerInvariant();

            foreach (LensCoating genProd in _context.LensCoatings)
            {
                string oldCode = genProd.LensCoatingCode.ToLowerInvariant();
                if (oldCode.Equals(LensCoatingCode))
                {
                    res = genProd;
                    break;
                }
            }
            return res;
        }
        public static string GetLensColourCodeByStockName(string stockName)
        {
            string res = "DefaultLensColour";

            stockName = stockName.ToLowerInvariant();
            string photo1 = "photo".ToLowerInvariant();
            string photo2 = "photochromic".ToLowerInvariant();

            if (stockName.Contains(photo1) || stockName.Contains(photo2))
            {
                res = "PHOTO";
                return res;
            }

            string blanc1 = "blanc".ToLowerInvariant();
            string blanc2 = "white".ToLowerInvariant();

            if (stockName.Contains(blanc1) || stockName.Contains(blanc2))
            {
                res = "BLANC";
                return res;
            }

            return res;


        }
        public static LensColour GetLensColourByCode(string LensColourCode)
        {
            LensColour res = null;

            LensColourCode = LensColourCode.ToLowerInvariant();

            foreach (LensColour genProd in _context.LensColours)
            {
                string oldCode = genProd.LensColourCode.ToLowerInvariant();
                if (oldCode.Equals(LensColourCode))
                {
                    res = genProd;
                    break;
                }
            }
            return res;
        }
        public static string GetLensOtherCriterion(string stockName)
        {
            string res = "";
            stockName = stockName.ToLowerInvariant();
            string transition = "transition".ToLowerInvariant();
            string pgx = "pgx".ToLowerInvariant();
            string pbx = "pbx".ToLowerInvariant();

            if (stockName.Contains(transition))
            {
                res = "TRANSITION";
                return res;
            }

            if (stockName.Contains(pgx))
            {
                res = "PGX";
                return res;
            }

            if (stockName.Contains(pbx))
            {
                res = "PBX";
                return res;
            }


            return res;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static LensNumber GetLensNumber(SaleLine saleLine, EFDbContext context)
        {
            LensNumber res = null;
            res = context.LensNumbers.Where(ln => (ln.LensNumberAdditionValue ?? "") == saleLine.Addition && (ln.LensNumberCylindricalValue ?? "") == saleLine.LensNumberCylindricalValue && (ln.LensNumberSphericalValue ?? "") == saleLine.LensNumberSphericalValue).FirstOrDefault();
            if (res == null)
            {
                res = new LensNumber
                {
                    LensNumberAdditionValue = saleLine.Addition,
                    LensNumberCylindricalValue = saleLine.LensNumberCylindricalValue,
                    LensNumberSphericalValue = saleLine.LensNumberSphericalValue,
                };
                res.LensNumberDescription = res.LensNumberFullCode;
            }
            return res;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static LensNumber GetLensNumber(AuthoriseSaleLine saleLine, EFDbContext context)
        {
            LensNumber res = null;
            //verifions si ce numero existe dans la liste des numero de verre
            List<LensNumber> numbers = context.LensNumbers.Where(ln => (ln.LensNumberAdditionValue ?? "") == saleLine.Addition && (ln.LensNumberCylindricalValue ?? "") == saleLine.LensNumberCylindricalValue && (ln.LensNumberSphericalValue ?? "") == saleLine.LensNumberSphericalValue).ToList();
            res = numbers.FirstOrDefault();
            if (res == null)
            {
                res = new LensNumber
                {
                    LensNumberAdditionValue = saleLine.Addition,
                    LensNumberCylindricalValue = saleLine.LensNumberCylindricalValue,
                    LensNumberSphericalValue = saleLine.LensNumberSphericalValue,
                };
            }
            return res;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static LensNumber GetLensNumber(CumulSaleAndBillLine saleLine, EFDbContext context)
        {
            LensNumber res = null;
            //verifions si ce numero existe dans la liste des numero de verre
            res = context.LensNumbers.Where(ln => (ln.LensNumberAdditionValue ?? "") == saleLine.Addition && (ln.LensNumberCylindricalValue ?? "") == saleLine.LensNumberCylindricalValue && (ln.LensNumberSphericalValue ?? "") == saleLine.LensNumberSphericalValue).FirstOrDefault();
            if (res == null)
            {
                res = new LensNumber
                {
                    LensNumberAdditionValue = saleLine.Addition,
                    LensNumberCylindricalValue = saleLine.LensNumberCylindricalValue,
                    LensNumberSphericalValue = saleLine.LensNumberSphericalValue,
                };
                res.LensNumberDescription = res.LensNumberFullCode;
            }

            return res;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="custOrdLine"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static LensNumber GetLensNumber(CustomerOrderLine custOrdLine, EFDbContext context)
        {
            LensNumber res = null;
            //verifions si ce numero existe dans la liste des numero de verre
            //res = context.LensNumbers.Where(ln => ln.LensNumberAdditionValue == custOrdLine.Addition && ln.LensNumberCylindricalValue == custOrdLine.LensNumberCylindricalValue && ln.LensNumberSphericalValue == custOrdLine.LensNumberSphericalValue).FirstOrDefault();
            res = context.LensNumbers.Where(ln => (ln.LensNumberAdditionValue ?? "") == custOrdLine.Addition && (ln.LensNumberCylindricalValue ?? "") == custOrdLine.LensNumberCylindricalValue && (ln.LensNumberSphericalValue ?? "") == custOrdLine.LensNumberSphericalValue).FirstOrDefault();
            if (res == null)
            {
                res = new LensNumber
                {
                    LensNumberAdditionValue = custOrdLine.Addition,
                    LensNumberCylindricalValue = custOrdLine.LensNumberCylindricalValue,
                    LensNumberSphericalValue = custOrdLine.LensNumberSphericalValue,
                };
                res.LensNumberDescription = res.LensNumberFullCode;
            }
            return res;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lensNumber"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static LensNumber GetLensNumber(LensNumber lensNumber, EFDbContext context)
        {
            _context = context;

            LensNumber res = _context.LensNumbers.FirstOrDefault(lm => (lm.LensNumberSphericalValue ?? "") == lensNumber.LensNumberSphericalValue &&
                                                                           (lm.LensNumberCylindricalValue ?? "") == lensNumber.LensNumberCylindricalValue &&
                                                                           (lm.LensNumberAdditionValue ?? "") == lensNumber.LensNumberAdditionValue);

            if (res == null || res.LensNumberID == 0)//le numéro n'existe pas ; il faut le créer
            {
                lensNumber.LensNumberDescription = lensNumber.LensNumberFullCode;
                res = _context.LensNumbers.Add(lensNumber);
                _context.SaveChanges();
            }

            return res;
        }

        /// <summary>
        /// Cette méthode répond à la question est ce que le côté gauche de l'oeil est valide?
        /// Le côté de l'oeil est valide si sphere ou le cylindre a une valeur.
        /// </summary>
        /// <param name="slm"></param>
        /// <returns></returns>
        public bool IsLEValid(SpecialLensModel slm)
        {
            bool res = false;

            res = (slm.LESphere != null && slm.LESphere.Length >= 4) || (slm.LECylinder != null && slm.LECylinder.Length >= 4);

            return res;
        }


        /// <summary>
        /// Cette méthode répond à la question est ce que le côté gauche de l'oeil est valide?
        /// Le côté de l'oeil est valide si sphere ou le cylindre a une valeur.
        /// </summary>
        /// <param name="slm"></param>
        /// <returns></returns>
        public bool IsREValid(SpecialLensModel slm)
        {
            bool res = false;

            res = (slm.RESphere != null && slm.RESphere.Length >= 4) || (slm.RECylinder != null && slm.RECylinder.Length >= 4);

            return res;
        }

        /// <summary>
        /// Cette méthode répond à la question est ce que le côté gauche de l'oeil est valide?
        /// Le côté de l'oeil est valide si sphere ou le cylindre a une valeur.
        /// </summary>
        /// <param name="slm"></param>
        /// <returns></returns>
        public bool IsFRValid(SpecialLensModel slm)
        {
            bool res = false;

            res = (slm.FrameProductID != null && slm.FrameProductID > 0);

            return res;
        }
        /// <summary>
        /// Cette méthode permet de transformer un SpecialLensModel en trois SaleLine.
        /// Elle est appelée après la soumission du formulaire de commande spéciale.
        /// </summary>
        /// <param name="slm">SpecialLensModel</param>
        /// <param name="boitier"></param>
        /// <param name="context"></param>
        /// <param name="spray"></param>
        /// <returns>Liste de CustomerOrderLine</returns>
        public List<SaleLine> Get_COL_From_SLM(SpecialLensModel slm, EFDbContext context, int spray, int boitier)
        {
            Int32 division, reste;
            double LensQty, LensPrice;

            _context = context;
            ITransactNumber tnRepo = new TransactNumberRepository();
            IBusinessDay busDayRepo = new BusinessDayRepository();
            List<SaleLine> cols = new List<SaleLine>();
            string specialOrderLineCode = (slm.LineID > 0) ? slm.SpecialOrderLineCode :
                                                              tnRepo.GenerateAndUpdateTransactNumber("SORD", busDayRepo.GetOpenedBusinessDay().FirstOrDefault()/*.BDDateOperation*/);
            //int lineID = (slm.LineID > 0) ? slm.LineID : 0;

            //Ajout des infos pour le côté droit de l'oeil
            if (IsREValid(slm))
            {
                LensQty = 0d;
                if (IsLEValid(slm))
                {
                    division = (Math.DivRem((int)slm.LensLineQuantity, 2, out reste));
                    if (reste == 0) //qte pair
                    {
                        LensQty = slm.LensLineQuantity / 2;
                        LensPrice = slm.LensPrice / 2;// slm.LensLineQuantity;
                    }
                    else throw new Exception("Wrong Lens Quantity configuration");
                }
                else
                {
                    LensQty = slm.LensLineQuantity;
                    LensPrice = slm.LensPrice; /// slm.LensLineQuantity;
                }
                cols.Add(
                    new SaleLine()
                    {
                        EyeSide = Entities.EyeSide.OD,
                        Addition = (slm.REAddition == null) ? "" : slm.REAddition,
                        Axis = (slm.REAxis == null) ? "" : slm.REAxis,
                        Index = (slm.REIndex == null) ? "" : slm.REIndex,
                        LensNumberCylindricalValue = (slm.RECylinder == null) ? "" : slm.RECylinder,
                        LensNumberSphericalValue = (slm.RESphere == null) ? "" : slm.RESphere,
                        LineQuantity = LensQty,//(IsLEValid(slm)) ? (slm.LensLineQuantity / 2) : slm.LensLineQuantity,
                        LineUnitPrice = LensPrice,//(IsLEValid(slm)) ? (slm.LensPrice / 2) : (slm.LensPrice) ,//LineUnitPrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.OD,
                        LensCategoryCode = slm.LensCategoryCode,
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LineID = slm.RELineID,
                        isGift = false,
                        isCommandGlass = slm.isRCommandGlass,
                        SupplyingName = slm.SupplyingName
                    }
                    );
            }
            //Ajout des infos pour le côté gauche de l'oeil

            if (IsLEValid(slm))
            {
                LensQty = 0d;
                if (IsREValid(slm))
                {
                    division = (Math.DivRem((int)slm.LensLineQuantity, 2, out reste));
                    if (reste == 0) //qte pair
                    {
                        LensQty = slm.LensLineQuantity / 2;
                        LensPrice = slm.LensPrice / 2;// slm.LensLineQuantity;
                    }
                    else throw new Exception("Wrong Lens Quantity configuration");
                }
                else
                {
                    LensQty = slm.LensLineQuantity;
                    LensPrice = slm.LensPrice; /// slm.LensLineQuantity;
                }
                cols.Add(
                    new SaleLine()
                    {
                        EyeSide = Entities.EyeSide.OG,
                        Addition = (slm.LEAddition == null) ? "" : slm.LEAddition,
                        Axis = (slm.LEAxis == null) ? "" : slm.LEAxis,
                        Index = (slm.LEIndex == null) ? "" : slm.LEIndex,
                        LensNumberCylindricalValue = (slm.LECylinder == null) ? "" : slm.LECylinder,
                        LensNumberSphericalValue = (slm.LESphere == null) ? "" : slm.LESphere,
                        LineQuantity = LensQty,//(IsREValid(slm)) ? (slm.LensLineQuantity / 2) : slm.LensLineQuantity,
                        LineUnitPrice = LensPrice,//(IsREValid(slm)) ? (slm.LensPrice/2) : slm.LensPrice,//LineUnitPrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.OG,
                        LensCategoryCode = slm.LensCategoryCode,
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LineID = slm.LELineID,
                        isGift = false,
                        isCommandGlass = slm.isLCommandGlass,
                        SupplyingName = slm.SupplyingName
                    }
                    );
            }

            //Ajout des infos pour le Cadre de l'oeil
            if (IsFRValid(slm))
            {
                cols.Add(
                    new SaleLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = slm.FrameLineQuantity,
                        LineUnitPrice = slm.FramePrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = slm.FrameProductID,
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice,
                        LineID = slm.FRLineID,
                        marque = slm.marque,
                        reference = slm.reference,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LensCategoryCode = null,
                        isGift = false,
                        NumeroSerie = slm.NumeroSerie,
                        isCommandGlass = false
                    }
                    );
            }
            //Ajout des infos pour le spray
            if (spray > 0)
            {
                Product lensSpray = _context.Products.Where(p => p.ProductCode.ToUpper().Trim() == "LENS SPRAY").FirstOrDefault();
                if (lensSpray == null) throw new Exception("Product LENS SPRAY Not yet create in the Database");
                cols.Add(
                    new SaleLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = 1,
                        LineUnitPrice = 0,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = lensSpray.ProductID,
                        PurchaseLineUnitPrice = 0,
                        LineID = 0,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LensCategoryCode = null,
                        isGift = true,
                        isCommandGlass = false
                    }
                    );
            }
            //Ajout des infos pour le boitier
            if (boitier > 0)
            {
                Product lensCases = _context.Products.Where(p => p.ProductCode.ToUpper().Trim() == "LENS CASES").FirstOrDefault();
                if (lensCases == null) throw new Exception("Product LENS CASES Not yet create in the Database");
                cols.Add(
                    new SaleLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = 1,
                        LineUnitPrice = 0,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = lensCases.ProductID,
                        PurchaseLineUnitPrice = 0,
                        LineID = 0,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LensCategoryCode = null,
                        isGift = true,
                        isCommandGlass = false
                    });
            }
            return cols;
        }


        /// <summary>
        /// Cette méthode permet de transformer un SpecialLensModel en trois SaleLine.
        /// Elle est appelée après la soumission du formulaire de commande spéciale.
        /// </summary>
        /// <param name="slm">SpecialLensModel</param>
        /// <param name="boitier"></param>
        /// <param name="context"></param>
        /// <param name="spray"></param>
        /// <returns>Liste de CustomerOrderLine</returns>
        public List<AuthoriseSaleLine> Get_AUTHCOL_From_SLM(SpecialLensModel slm, EFDbContext context, int spray, int boitier)
        {
            Int32 division, reste;
            double LensQty, LensPrice;

            _context = context;
            ITransactNumber tnRepo = new TransactNumberRepository();
            IBusinessDay busDayRepo = new BusinessDayRepository();
            List<AuthoriseSaleLine> cols = new List<AuthoriseSaleLine>();
            string specialOrderLineCode = (slm.LineID > 0) ? slm.SpecialOrderLineCode :
                                                              tnRepo.GenerateAndUpdateTransactNumber("SORD", busDayRepo.GetOpenedBusinessDay().FirstOrDefault()/*.BDDateOperation*/);
            //int lineID = (slm.LineID > 0) ? slm.LineID : 0;

            //Ajout des infos pour le côté droit de l'oeil
            if (IsREValid(slm))
            {
                LensQty = 0d;
                if (IsLEValid(slm))
                {
                    division = (Math.DivRem((int)slm.LensLineQuantity, 2, out reste));
                    if (reste == 0) //qte pair
                    {
                        LensQty = slm.LensLineQuantity / 2;
                        LensPrice = slm.LensPrice / 2;// slm.LensLineQuantity;
                    }
                    else throw new Exception("Wrong Lens Quantity configuration");
                }
                else
                {
                    LensQty = slm.LensLineQuantity;
                    LensPrice = slm.LensPrice / 2;// slm.LensLineQuantity;
                }
                cols.Add(
                    new AuthoriseSaleLine()
                    {
                        EyeSide = Entities.EyeSide.OD,
                        Addition = (slm.REAddition == null) ? "" : slm.REAddition,
                        Axis = (slm.REAxis == null) ? "" : slm.REAxis,
                        Index = (slm.REIndex == null) ? "" : slm.REIndex,
                        LensNumberCylindricalValue = (slm.RECylinder == null) ? "" : slm.RECylinder,
                        LensNumberSphericalValue = (slm.RESphere == null) ? "" : slm.RESphere,
                        LineQuantity = LensQty,//(IsLEValid(slm)) ? (slm.LensLineQuantity / 2) : slm.LensLineQuantity,
                        LineUnitPrice = LensPrice,//(IsLEValid(slm)) ? (slm.LensPrice / 2) : (slm.LensPrice) ,//LineUnitPrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.OD,
                        LensCategoryCode = slm.LensCategoryCode,
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LineID = slm.RELineID,
                        isGift = false,
                        isCommandGlass = slm.isRCommandGlass,
                        SupplyingName = slm.SupplyingName
                    }
                    );
            }
            //Ajout des infos pour le côté gauche de l'oeil

            if (IsLEValid(slm))
            {
                LensQty = 0d;
                if (IsREValid(slm))
                {
                    division = (Math.DivRem((int)slm.LensLineQuantity, 2, out reste));
                    if (reste == 0) //qte pair
                    {
                        LensQty = slm.LensLineQuantity / 2;
                        LensPrice = slm.LensPrice / 2;// slm.LensLineQuantity;
                    }
                    else throw new Exception("Wrong Lens Quantity configuration");
                }
                else
                {
                    LensQty = slm.LensLineQuantity;
                    LensPrice = slm.LensPrice / 2;// slm.LensLineQuantity;
                }
                cols.Add(
                    new AuthoriseSaleLine()
                    {
                        EyeSide = Entities.EyeSide.OG,
                        Addition = (slm.LEAddition == null) ? "" : slm.LEAddition,
                        Axis = (slm.LEAxis == null) ? "" : slm.LEAxis,
                        Index = (slm.LEIndex == null) ? "" : slm.LEIndex,
                        LensNumberCylindricalValue = (slm.LECylinder == null) ? "" : slm.LECylinder,
                        LensNumberSphericalValue = (slm.LESphere == null) ? "" : slm.LESphere,
                        LineQuantity = LensQty,//(IsREValid(slm)) ? (slm.LensLineQuantity / 2) : slm.LensLineQuantity,
                        LineUnitPrice = LensPrice,//(IsREValid(slm)) ? (slm.LensPrice/2) : slm.LensPrice,//LineUnitPrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.OG,
                        LensCategoryCode = slm.LensCategoryCode,
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LineID = slm.LELineID,
                        isGift = false,
                        isCommandGlass = slm.isLCommandGlass,
                        SupplyingName = slm.SupplyingName
                    }
                    );
            }

            //Ajout des infos pour le Cadre de l'oeil
            if (IsFRValid(slm))
            {
                cols.Add(
                    new AuthoriseSaleLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = slm.FrameLineQuantity,
                        LineUnitPrice = slm.FramePrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = slm.FrameProductID,
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice,
                        LineID = slm.FRLineID,
                        marque = slm.marque,
                        reference = slm.reference,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LensCategoryCode = null,
                        isGift = false,
                        NumeroSerie = slm.NumeroSerie,
                        isCommandGlass = false,
                        IsVIPRoom = slm.IsVIPRoom
                    }
                    );
            }
            //Ajout des infos pour le spray
            if (spray > 0)
            {
                Product lensSpray = _context.Products.Where(p => p.ProductCode.ToUpper().Trim() == "LENS SPRAY").FirstOrDefault();
                if (lensSpray == null) throw new Exception("Product LENS SPRAY Not yet create in the Database");
                cols.Add(
                    new AuthoriseSaleLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = 1,
                        LineUnitPrice = 0,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = lensSpray.ProductID,
                        PurchaseLineUnitPrice = 0,
                        LineID = 0,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LensCategoryCode = null,
                        isGift = true,
                        isCommandGlass = false
                    }
                    );
            }
            //Ajout des infos pour le boitier
            if (boitier > 0)
            {
                Product lensCases = _context.Products.Where(p => p.ProductCode.ToUpper().Trim() == "LENS CASES").FirstOrDefault();
                if (lensCases == null) throw new Exception("Product LENS CASES Not yet create in the Database");
                cols.Add(
                    new AuthoriseSaleLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = 1,
                        LineUnitPrice = 0,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = lensCases.ProductID,
                        PurchaseLineUnitPrice = 0,
                        LineID = 0,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LensCategoryCode = null,
                        isGift = true,
                        isCommandGlass = false
                    });
            }
            return cols;
        }

        /// <summary>
        /// Cette méthode permet de transformer un SpecialLensModel en trois CumulSaleAndBillLine.
        /// Elle est appelée après la soumission du formulaire de commande spéciale.
        /// </summary>
        /// <param name="boitier"></param>
        /// <param name="context"></param>
        /// <param name="LocalizationID"></param>
        /// <param name="spray"></param>
        /// <returns>Liste de CustomerOrderLine</returns>
        public List<CumulSaleAndBillLine> Get_CUMSALEBILLCOL_From_SLM(EFDbContext context, int spray, int boitier, int LocalizationID)
        {


            _context = context;

            List<CumulSaleAndBillLine> cols = new List<CumulSaleAndBillLine>();

            //Ajout des infos pour le spray
            if (spray > 0)
            {
                Product lensSpray = _context.Products.Where(p => p.ProductCode.ToUpper().Trim() == "LENS SPRAY").FirstOrDefault();
                if (lensSpray == null) throw new Exception("Product LENS SPRAY Not yet create in the Database");
                cols.Add(
                    new CumulSaleAndBillLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = 1,
                        LineUnitPrice = 0,
                        LocalizationID = LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = lensSpray.ProductID,
                        PurchaseLineUnitPrice = 0,
                        LineID = 0,
                        SpecialOrderLineCode = "",
                        LensCategoryCode = null,
                        isGift = true,
                        isCommandGlass = false
                    }
                    );
            }
            //Ajout des infos pour le boitier
            if (boitier > 0)
            {
                Product lensCases = _context.Products.Where(p => p.ProductCode.ToUpper().Trim() == "LENS CASES").FirstOrDefault();
                if (lensCases == null) throw new Exception("Product LENS CASES Not yet create in the Database");
                cols.Add(
                    new CumulSaleAndBillLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = 1,
                        LineUnitPrice = 0,
                        LocalizationID = LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = lensCases.ProductID,
                        PurchaseLineUnitPrice = 0,
                        LineID = 0,
                        SpecialOrderLineCode = "",
                        LensCategoryCode = null,
                        isGift = true,
                        isCommandGlass = false
                    });
            }
            return cols;
        }


        /// <summary>
        /// Cette méthode permet de transformer un SpecialLensModel en trois CumulSaleAndBillLine.
        /// Elle est appelée après la soumission du formulaire de commande spéciale.
        /// </summary>
        /// <param name="slm">SpecialLensModel</param>
        /// <param name="boitier"></param>
        /// <param name="context"></param>
        /// <param name="spray"></param>
        /// <returns>Liste de CustomerOrderLine</returns>
        public List<CumulSaleAndBillLine> Get_CUMSALEBILLCOL_From_SLM(SpecialLensModel slm, EFDbContext context, int spray, int boitier)
        {
            Int32 division, reste;
            double LensQty, LensPrice, PurchaseLineUnitPrice;

            _context = context;
            ITransactNumber tnRepo = new TransactNumberRepository();
            IBusinessDay busDayRepo = new BusinessDayRepository();
            List<CumulSaleAndBillLine> cols = new List<CumulSaleAndBillLine>();
            string specialOrderLineCode = (slm.LineID > 0) ? slm.SpecialOrderLineCode :
                                                              tnRepo.GenerateAndUpdateTransactNumber("SORD", busDayRepo.GetOpenedBusinessDay().FirstOrDefault()/*.BDDateOperation*/);
            //int lineID = (slm.LineID > 0) ? slm.LineID : 0;
            //si les 2 cote st valide


            //Ajout des infos pour le côté droit de l'oeil
            if (IsREValid(slm))
            {
                LensQty = 0d;
                PurchaseLineUnitPrice = 0d;
                if (IsLEValid(slm))
                {
                    division = (Math.DivRem((int)slm.LensLineQuantity, 2, out reste));
                    if (reste == 0) //qte pair
                    {
                        LensQty = slm.LensLineQuantity / 2;
                        LensPrice = slm.LensPrice / 2;// slm.LensLineQuantity;
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice / 2;
                    }
                    else throw new Exception("Wrong Lens Quantity configuration");
                }
                else
                {
                    LensQty = slm.LensLineQuantity;
                    LensPrice = slm.LensPrice / slm.LensLineQuantity;
                    PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice / slm.LensLineQuantity;
                }
                cols.Add(
                    new CumulSaleAndBillLine()
                    {
                        EyeSide = Entities.EyeSide.OD,
                        Addition = (slm.REAddition == null) ? "" : slm.REAddition,
                        Axis = (slm.REAxis == null) ? "" : slm.REAxis,
                        Index = (slm.REIndex == null) ? "" : slm.REIndex,
                        LensNumberCylindricalValue = (slm.RECylinder == null) ? "" : slm.RECylinder,
                        LensNumberSphericalValue = (slm.RESphere == null) ? "" : slm.RESphere,
                        LineQuantity = LensQty,//(IsLEValid(slm)) ? (slm.LensLineQuantity / 2) : slm.LensLineQuantity,
                        LineUnitPrice = LensPrice,//(IsLEValid(slm)) ? (slm.LensPrice / 2) : (slm.LensPrice) ,//LineUnitPrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.OD,
                        LensCategoryCode = slm.LensCategoryCode,
                        PurchaseLineUnitPrice = PurchaseLineUnitPrice,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LineID = slm.RELineID,
                        isGift = false,
                        ProductCategoryID = slm.ProductCategoryID,
                        isCommandGlass = slm.isRCommandGlass,
                        SupplyingName = slm.SupplyingName
                    }
                    );
            }
            //Ajout des infos pour le côté gauche de l'oeil

            if (IsLEValid(slm))
            {
                LensQty = 0d;
                if (IsREValid(slm))
                {
                    division = (Math.DivRem((int)slm.LensLineQuantity, 2, out reste));
                    if (reste == 0) //qte pair
                    {
                        LensQty = slm.LensLineQuantity / 2;
                        LensPrice = slm.LensPrice / 2;// slm.LensLineQuantity;
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice / 2;
                    }
                    else throw new Exception("Wrong Lens Quantity configuration");
                }
                else
                {
                    LensQty = slm.LensLineQuantity;
                    LensPrice = slm.LensPrice / slm.LensLineQuantity;
                    PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice / slm.LensLineQuantity;
                }
                cols.Add(
                    new CumulSaleAndBillLine()
                    {
                        EyeSide = Entities.EyeSide.OG,
                        Addition = (slm.LEAddition == null) ? "" : slm.LEAddition,
                        Axis = (slm.LEAxis == null) ? "" : slm.LEAxis,
                        Index = (slm.LEIndex == null) ? "" : slm.LEIndex,
                        LensNumberCylindricalValue = (slm.LECylinder == null) ? "" : slm.LECylinder,
                        LensNumberSphericalValue = (slm.LESphere == null) ? "" : slm.LESphere,
                        LineQuantity = LensQty,//(IsREValid(slm)) ? (slm.LensLineQuantity / 2) : slm.LensLineQuantity,
                        LineUnitPrice = LensPrice,//(IsREValid(slm)) ? (slm.LensPrice/2) : slm.LensPrice,//LineUnitPrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.OG,
                        LensCategoryCode = slm.LensCategoryCode,
                        PurchaseLineUnitPrice = PurchaseLineUnitPrice,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LineID = slm.LELineID,
                        isGift = false,
                        ProductCategoryID = slm.ProductCategoryID,
                        isCommandGlass = slm.isLCommandGlass,
                        SupplyingName = slm.SupplyingName
                    }
                    );
            }

            //Ajout des infos pour le Cadre de l'oeil
            if (IsFRValid(slm))
            {
                cols.Add(
                    new CumulSaleAndBillLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = slm.FrameLineQuantity,
                        LineUnitPrice = slm.FramePrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = slm.FrameProductID,
                        PurchaseLineUnitPrice = 0,
                        LineID = slm.FRLineID,
                        marque = slm.marque,
                        reference = slm.reference,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LensCategoryCode = null,
                        isGift = false,
                        NumeroSerie = slm.NumeroSerie,
                        isCommandGlass = false
                    }
                    );
            }
            //Ajout des infos pour le spray
            if (spray > 0)
            {
                Product lensSpray = _context.Products.Where(p => p.ProductCode.ToUpper().Trim() == "LENS SPRAY").FirstOrDefault();
                if (lensSpray == null) throw new Exception("Product LENS SPRAY Not yet create in the Database");
                cols.Add(
                    new CumulSaleAndBillLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = 1,
                        LineUnitPrice = 0,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = lensSpray.ProductID,
                        PurchaseLineUnitPrice = 0,
                        LineID = 0,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LensCategoryCode = null,
                        isGift = true,
                        isCommandGlass = false
                    }
                    );
            }
            //Ajout des infos pour le boitier
            if (boitier > 0)
            {
                Product lensCases = _context.Products.Where(p => p.ProductCode.ToUpper().Trim() == "LENS CASES").FirstOrDefault();
                if (lensCases == null) throw new Exception("Product LENS CASES Not yet create in the Database");
                cols.Add(
                    new CumulSaleAndBillLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = 1,
                        LineUnitPrice = 0,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = lensCases.ProductID,
                        PurchaseLineUnitPrice = 0,
                        LineID = 0,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LensCategoryCode = null,
                        isGift = true,
                        isCommandGlass = false
                    });
            }
            return cols;
        }

        public string getLensNumberValue(String value)
        {
            return (value.ToLower().Contains("plan") || value == "0.00") ? "" : value;
        }

        /// <summary>
        /// Cette méthode permet de transformer un SpecialLensModel en trois SaleLine.
        /// Elle est appelée après la soumission du formulaire de commande spéciale.
        /// </summary>
        /// <param name="slm">SpecialLensModel</param>
        /// <param name="context"></param>
        /// <returns>Liste de CustomerOrderLine</returns>
        public List<CustomerOrderLine> Get_COL_CUSTORDER_From_SLM(SpecialLensModel slm, EFDbContext context)
        {
            _context = context;
            ITransactNumber tnRepo = new TransactNumberRepository();
            IBusinessDay busDayRepo = new BusinessDayRepository();
            List<CustomerOrderLine> cols = new List<CustomerOrderLine>();
            string specialOrderLineCode = (slm.LineID > 0) ? slm.SpecialOrderLineCode :
                                                              tnRepo.GenerateAndUpdateTransactNumber("SORD", busDayRepo.GetOpenedBusinessDay().FirstOrDefault()/*.BDDateOperation*/);
            //int lineID = (slm.LineID > 0) ? slm.LineID : 0;

            //Ajout des infos pour le côté droit de l'oeil
            if (IsREValid(slm))
            {
                cols.Add(
                    new CustomerOrderLine()
                    {
                        EyeSide = Entities.EyeSide.OD,
                        Addition = (slm.REAddition == null) ? "" : slm.REAddition,
                        Axis = (slm.REAxis == null) ? "" : slm.REAxis,
                        Index = (slm.REIndex == null) ? "" : slm.REIndex,
                        LensNumberCylindricalValue = (slm.RECylinder == null) ? "" : slm.RECylinder,
                        LensNumberSphericalValue = (slm.RESphere == null) ? "" : slm.RESphere,
                        LineQuantity = (IsLEValid(slm)) ? (slm.LensLineQuantity / 2) : slm.LensLineQuantity,
                        LineUnitPrice = slm.REPrice,//LineUnitPrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.OD,
                        LensCategoryCode = slm.LensCategoryCode,
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LineID = slm.RELineID,
                        FrameCategory = null,
                        SupplyingName = slm.SupplyingName,
                        isCommandGlass = slm.isRCommandGlass
                    }
                    );
            }
            //Ajout des infos pour le côté gauche de l'oeil
            if (IsLEValid(slm))
            {
                cols.Add(
                    new CustomerOrderLine()
                    {
                        EyeSide = Entities.EyeSide.OG,
                        Addition = (slm.LEAddition == null) ? "" : slm.LEAddition,
                        Axis = (slm.LEAxis == null) ? "" : slm.LEAxis,
                        Index = (slm.LEIndex == null) ? "" : slm.LEIndex,
                        LensNumberCylindricalValue = (slm.LECylinder == null) ? "" : slm.LECylinder,
                        LensNumberSphericalValue = (slm.LESphere == null) ? "" : slm.LESphere,
                        LineQuantity = (IsREValid(slm)) ? (slm.LensLineQuantity / 2) : slm.LensLineQuantity,
                        LineUnitPrice = slm.LEPrice,//LineUnitPrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.OG,
                        LensCategoryCode = slm.LensCategoryCode,
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LineID = slm.LELineID,
                        FrameCategory = null,
                        SupplyingName = slm.SupplyingName,
                        isCommandGlass = slm.isLCommandGlass
                    }
                    );
            }

            //Ajout des infos pour le Cadre de l'oeil
            if (IsFRValid(slm))
            {
                cols.Add(
                    new CustomerOrderLine()
                    {
                        EyeSide = Entities.EyeSide.N,
                        LineQuantity = slm.FrameLineQuantity,
                        LineUnitPrice = slm.FramePrice,
                        LocalizationID = slm.LocalizationID,
                        OeilDroiteGauche = Entities.EyeSide.N,
                        ProductID = slm.FrameProductID,
                        PurchaseLineUnitPrice = slm.PurchaseLineUnitPrice,
                        LineID = slm.FRLineID,
                        marque = slm.marque,
                        reference = slm.reference,
                        SpecialOrderLineCode = specialOrderLineCode,
                        LensCategoryCode = null,
                        FrameCategory = slm.FrameCategory,
                        NumeroSerie = slm.NumeroSerie,
                        isCommandGlass = false,
                        IsVIPRoom = slm.IsVIPRoom
                    });
            }
            return cols;
        }

        public static Lens CreateLens(Lens currentProduct, EFDbContext context)
        {
            Lens product = context.Lenses.FirstOrDefault(pdt => pdt.ProductCode == currentProduct.ProductCode);

            if (product != null && product.ProductID > 0)
            {
                return product;
            }

            //1 - Création du numéro de Verre
            LensNumber currentLensNumber = LensConstruction.GetLensNumber(currentProduct.LensNumber, context);
            currentProduct.LensNumberID = currentLensNumber.LensNumberID;
            currentProduct.LensNumber = null;

            //2-Création de la catégorie du produit
            if (currentProduct.LensCategoryID <= 0)
            {
                LensCategory lensCategory = LensConstruction.PersistLensCategory(currentProduct.LensCategory.CategoryCode, context);
                currentProduct.LensCategoryID = lensCategory.CategoryID;
                currentProduct.CategoryID = lensCategory.CategoryID;
                currentProduct.LensCategory = lensCategory;
                currentProduct.Category = lensCategory;
            }

            //3 - Numéro de compte du produit
            CollectifAccount colAccount = currentProduct.LensCategory.CollectifAccount;
            //recuperation  du premier cpte de cette category
            Account Acct = (from a in context.Accounts
                            where a.CollectifAccountID == colAccount.CollectifAccountID
                            select a).FirstOrDefault();

            //currentProduct.AccountID = (from a in context.Accounts where a.CollectifAccountID==colAccount.CollectifAccountID
            //                                select a).FirstOrDefault().AccountID;
            if (Acct == null)
            {
                IAccount accountRepo = new AccountRepository(context);
                currentProduct.AccountID = accountRepo.GenerateAccountNumber(colAccount.CollectifAccountID, currentProduct.Category.CategoryCode, false).AccountID;
            }
            else
            {
                currentProduct.AccountID = Acct.AccountID;
            }

            currentProduct.Category = null;
            currentProduct.LensCategory = null;
            currentProduct = context.Lenses.Add(currentProduct);
            context.SaveChanges();

            return currentProduct;
        }

        /// <summary>
        ///Cette méthode permet d'obtenir un SpecialLensModel a partir d'une liste de CustomerOrderLine.
        ///Cette méthode est utilisé afin de remplir le formulaire lors de la modification d'une ligne
        /// </summary>
        /// <param name="cols">Liste de CustomerOrderLine<</param>
        /// <returns>SpecialLensModel</returns>
        public SpecialLensModel Get_SLM_From_COL(List<SaleLine> cols)
        {
            SpecialLensModel res = new SpecialLensModel();
            res.LensLineQuantity = 0;
            foreach (SaleLine col in cols)
            {
                //Remplissage des Paramètre de l'oeil Gauche
                if (col.OeilDroiteGauche == EyeSide.OG)
                {
                    res.LEAddition = col.Addition;
                    res.LEAxis = col.Axis;
                    res.LEIndex = col.Index;
                    res.LECylinder = col.LensNumberCylindricalValue;
                    res.LESphere = col.LensNumberSphericalValue;
                    res.LELineID = col.LineID;
                    res.LensPrice = col.LineUnitPrice;
                    res.LensLineQuantity = res.LensLineQuantity + col.LineQuantity;
                    res.isLCommandGlass = col.isCommandGlass;
                }

                //Remplissage des Paramètre de l'oeil Droit
                if (col.OeilDroiteGauche == EyeSide.OD)
                {
                    res.REAddition = col.Addition;
                    res.REAxis = col.Axis;
                    res.REIndex = col.Index;
                    res.RECylinder = col.LensNumberCylindricalValue;
                    res.RESphere = col.LensNumberSphericalValue;
                    res.RELineID = col.LineID;
                    res.LensPrice = col.LineUnitPrice;
                    res.LensLineQuantity = res.LensLineQuantity + col.LineQuantity;
                    res.isRCommandGlass = col.isCommandGlass;
                }
                //remplissage du cadre
                if (col.OeilDroiteGauche == EyeSide.N)
                {
                    res.marque = col.marque;
                    res.reference = col.reference;
                    res.FramePrice = col.LineAmount;
                    res.FrameProductID = col.ProductID;
                    res.FRLineID = col.LineID;
                    res.FrameLineQuantity = col.LineQuantity;
                    res.NumeroSerie = col.NumeroSerie;
                    res.isLCommandGlass = false;
                    res.isRCommandGlass = false;
                }
            }

            //Paramètre Générales aux deux yeux
            res.LineUnitPrice = cols.FirstOrDefault().LineUnitPrice;
            res.LocalizationID = cols.FirstOrDefault().LocalizationID;
            res.PurchaseLineUnitPrice = cols.FirstOrDefault().PurchaseLineUnitPrice;
            res.LineID = cols.FirstOrDefault().LineID;
            res.LensCategoryCode = cols.FirstOrDefault().Product.Category.CategoryCode;
            res.SpecialOrderLineCode = cols.FirstOrDefault().SpecialOrderLineCode;
            res.LineQuantity = cols.Select(cl => cl.LineQuantity).Sum();
            //res.SupplyingName = cols.FirstOrDefault().SupplyingName;

            return res;
        }

        /// <summary>
        ///Cette méthode permet d'obtenir un SpecialLensModel a partir d'une liste de CustomerOrderLine.
        ///Cette méthode est utilisé afin de remplir le formulaire lors de la modification d'une ligne
        /// </summary>
        /// <param name="cols">Liste de CustomerOrderLine<</param>
        /// <returns>SpecialLensModel</returns>
        public SpecialLensModel Get_SLM_From_COL(List<CustomerOrderLine> cols)
        {
            SpecialLensModel res = new SpecialLensModel();

            foreach (CustomerOrderLine col in cols)
            {
                res.LensPrice = 0;
                //Remplissage des Paramètre de l'oeil Gauche
                if (col.OeilDroiteGauche == EyeSide.OG)
                {
                    res.LEAddition = col.Addition;
                    res.LEAxis = col.Axis;
                    res.LEIndex = col.Index;
                    res.LECylinder = col.LensNumberCylindricalValue;
                    res.LESphere = col.LensNumberSphericalValue;
                    res.LELineID = col.LineID;
                    res.LEPrice = col.LineUnitPrice;
                    res.LensPrice = res.LensPrice = +col.LineUnitPrice;
                    res.isLCommandGlass = col.isCommandGlass;
                }

                //Remplissage des Paramètre de l'oeil Droit
                if (col.OeilDroiteGauche == EyeSide.OD)
                {
                    res.REAddition = col.Addition;
                    res.REAxis = col.Axis;
                    res.REIndex = col.Index;
                    res.RECylinder = col.LensNumberCylindricalValue;
                    res.RESphere = col.LensNumberSphericalValue;
                    res.RELineID = col.LineID;
                    res.REPrice = col.LineUnitPrice;
                    res.LensPrice = res.LensPrice = +col.LineUnitPrice;
                    res.isRCommandGlass = col.isCommandGlass;
                }
                //remplissage du cadre
                if (col.OeilDroiteGauche == EyeSide.N)
                {
                    res.marque = col.marque;
                    res.reference = col.reference;
                    res.FramePrice = col.LineAmount;
                    res.FrameProductID = col.ProductID;
                    res.FRLineID = col.LineID;
                    res.FrameCategory = col.FrameCategory;
                    res.NumeroSerie = col.NumeroSerie;
                    res.isLCommandGlass = false;
                    res.isRCommandGlass = false;
                }

            }

            //Paramètre Générales aux deux yeux
            res.LineUnitPrice = cols.FirstOrDefault().LineUnitPrice;
            res.LocalizationID = cols.FirstOrDefault().LocalizationID;
            res.PurchaseLineUnitPrice = cols.FirstOrDefault().PurchaseLineUnitPrice;
            res.LineID = cols.FirstOrDefault().LineID;
            res.LensCategoryCode = cols.FirstOrDefault().Product.Category.CategoryCode;
            res.SpecialOrderLineCode = cols.FirstOrDefault().SpecialOrderLineCode;
            res.LineQuantity = cols.Select(cl => cl.LineQuantity).Sum();
            //res.SupplyingName = cols.FirstOrDefault().SupplyingName;

            return res;
        }

        public static LensValidationError validateLens(string sph, string cyl)
        {

            LensValidationError error = new LensValidationError();
            error.code = LensValidationErrorCode.SUCCESS;
            error.value = "BOTH";

            /*if ((sph == null || sph == "") && (cyl == null || cyl == ""))
            {
                return error;
            }*/

            if (sph == null || sph == "")
            {
                error.code = LensValidationErrorCode.VALUE_REQUIRED;
                error.value = "SPH";
                return error;
            }

            string sphValidationCode = checkCommonValidation(sph);
            if (sphValidationCode != LensValidationErrorCode.SUCCESS)
            {
                error.code = sphValidationCode;
                error.value = "SPH";
                return error;
            }

            if ((cyl != null && cyl != "") && ( cyl.ToLower() == "plan" || cyl.ToLower() == "plano" || cyl == "0.00"))
            {
                error.code = LensValidationErrorCode.NO_VALUE_REQUIRED;
                error.value = "CYL";
                return error;
            }

            string cylValidationCode = checkCommonValidation(cyl);
            if (cylValidationCode != LensValidationErrorCode.SUCCESS)
            {
                error.code = cylValidationCode;
                error.value = "CYL";
                return error;
            }

            return error;
        }

        public static string checkCommonValidation(string lensNumber)
        {
            string res = LensValidationErrorCode.SUCCESS;
            if (lensNumber == null || lensNumber == "" || lensNumber == "0.00" || lensNumber.ToLower() == "plan" || lensNumber.ToLower() == "plano")
            {
                return res;
            }

            if(lensNumber.Length > 6 )
            {
                res = LensValidationErrorCode.NUMBER_MAX_DIGITS_VIOLATION;
                return res;
            }

            string unsignedNumber = lensNumber.Remove(0, 1);
            try
            {
                float value = float.Parse(unsignedNumber); // Test si le nombre saisi est un floattant valide; si non, leve une exeption
            
                if (lensNumber == "+0.00" || lensNumber == "-0.00")
                {
                    res = LensValidationErrorCode.NO_SIGN_PLANO;
                    return res;
                }

                string[] chunks = lensNumber.Split('.');
                string integerPart = chunks[0];
                if (integerPart[0] != '+' && integerPart[0] != '-')
                {
                    res = LensValidationErrorCode.MUST_BE_SIGNED;
                    return res;
                }

                if (chunks.Length != 2) // si 1, c'est un entier; si superieur a 2, c'est la sorcellerie
                {
                    res = LensValidationErrorCode.BAD_FORMAT;
                    return res;
                }

                string decimalPart = chunks[1];
                if (decimalPart != "00" && decimalPart != "25" && decimalPart != "50" && decimalPart != "75")
                {
                    res = LensValidationErrorCode.DECIMAL_PART_POSSIBILITIES_VIOLATION;
                    return res;
                }
                string unsignedIntegerPart = integerPart.Remove(0, 1);
                int integerPartInt = int.Parse(unsignedIntegerPart);
                if (integerPartInt > 20)
                {
                    res = LensValidationErrorCode.INTEGER_PART_MAX_VALUE_VIOLATION;
                    return res;
                }

                char firstIntegerDigit = integerPart[1]; // a la position 0 on a le signe
                if (unsignedIntegerPart != "0" && firstIntegerDigit == '0')
                {
                    res = LensValidationErrorCode.FIRST_UNSIGN_DIGIT_NOT_ZERO;
                    return res;
                }
            }
            catch (Exception ex)
            {
                res = LensValidationErrorCode.BAD_FORMAT;
                return res;
            }
            return res;
        }

        public static LensValidationError validateAddition(string addition, string side)
        {
            LensValidationError resRAddition = new LensValidationError();
            resRAddition.code = LensValidationErrorCode.SUCCESS;
            resRAddition.value = "ADDITION";

            if (addition.ToLower() == "plan" || addition.ToLower() == "plano" || addition == "0.00")
            {
                resRAddition.code = LensValidationErrorCode.NO_VALUE_REQUIRED;
                resRAddition = validateOneSide(resRAddition, side);
                return resRAddition;
            }

            if (addition[0] != '+')
            {
                resRAddition.code = LensValidationErrorCode.ADDITION_MUST_BE_POSITIVE;
                resRAddition = validateOneSide(resRAddition, side);
                return resRAddition;
            }

            string resRAdditionCode = LensConstruction.checkCommonValidation(addition);
            if (resRAdditionCode != LensValidationErrorCode.SUCCESS)
            {
                resRAddition.code = resRAdditionCode;
                resRAddition = validateOneSide(resRAddition, side);
                return resRAddition;
            }

            return resRAddition;
        }

        public static LensValidationError validateOneSide(LensValidationError error, string side)
        {
            if (error.code != LensValidationErrorCode.SUCCESS)
            {
                if (error.code == LensValidationErrorCode.BAD_FORMAT)
                {
                    error.errorMessage = side + " " + error.value + " Bad Value " + error.code;
                    return error;
                }

                if (error.code == LensValidationErrorCode.DECIMAL_PART_POSSIBILITIES_VIOLATION)
                {
                    error.errorMessage = side + " " + error.value + " Bad Value " + error.code;
                    return error;
                }

                if (error.code == LensValidationErrorCode.INTEGER_PART_MAX_VALUE_VIOLATION)
                {
                    error.errorMessage = side + " " + error.value + " Bad Value " + error.code;
                    return error;
                }

                if (error.code == LensValidationErrorCode.MUST_BE_SIGNED)
                {
                    error.errorMessage = side + " " + error.value + " Bad Value " + error.code;
                    return error;
                }

                if (error.code == LensValidationErrorCode.NO_SIGN_PLANO)
                {
                    error.errorMessage = side + " " + error.value + " Bad Value " + error.code;
                    return error;
                }

                if (error.code == LensValidationErrorCode.NO_VALUE_REQUIRED)
                {
                    error.errorMessage = side + " " + error.value + " Bad Value " + error.code;
                    return error;
                }

                if (error.code == LensValidationErrorCode.VALUE_REQUIRED)
                {
                    error.errorMessage = side + " " + error.value + " Bad Value " + error.code;
                    return error;
                }

                error.errorMessage = side + " " + error.value + " Bad Value " + error.code;
                return error;
            }

            return error;
        }

        public static LensValidationError validateLens(ConsultLensPrescription prescription)
        {
            LensValidationError resR = validateLens(prescription.RSphValue, prescription.RCylValue);
            resR = validateOneSide(resR, "Right Side");
            if (resR.code != LensValidationErrorCode.SUCCESS)
            {
                return resR;
            }

            LensValidationError resL = validateLens(prescription.LSphValue, prescription.LCylValue);
            resL = validateOneSide(resL, "Left Side");
            if (resL.code != LensValidationErrorCode.SUCCESS)
            {
                return resL;
            }


            if (prescription.RAddition != null && prescription.RAddition != "")
            {
                LensValidationError resRAddition = validateAddition(prescription.RAddition, "Right Side");
                if (resRAddition.code != LensValidationErrorCode.SUCCESS)
                {
                    return resRAddition;
                }
            }

            if (prescription.LAddition != null && prescription.LAddition != "")
            {
                LensValidationError resLAddition = validateAddition(prescription.LAddition, "Left Side");
                if (resLAddition.code != LensValidationErrorCode.SUCCESS)
                {
                    return resLAddition;
                }
            }

            return resR;
        }

        public static ConsultLensPrescription getConsultLensPrescriptionFromConsultOldPrescr(ConsultOldPrescr oldPrescription)
        {
            ConsultLensPrescription lensPrescription = new ConsultLensPrescription();

            lensPrescription.RAddition = oldPrescription.RAddition;
            lensPrescription.LAddition = oldPrescription.LAddition;

            lensPrescription.RCylValue = oldPrescription.RCylValue;
            lensPrescription.LCylValue = oldPrescription.LCylValue;

            lensPrescription.RSphValue = oldPrescription.RSphValue;
            lensPrescription.LSphValue = oldPrescription.LSphValue;

            return lensPrescription;
        }

        public static Lens GetLensFromOrderLens(int orderLensId, EFDbContext context, CumulSaleAndBillLine csbl)
        {
            OrderLens orderLens = context.OrderLenses.SingleOrDefault(ol => ol.ProductID == orderLensId);
            string productCode = orderLens.LensCategoryName + orderLens.LensNumberFullCode;

            Lens lens = context.Lenses.FirstOrDefault(l => l.LensCategoryID == orderLens.LensCategoryID &&
                                                           l.LensNumberID == orderLens.LensNumberID);

            if (lens == null)
            {
                lens = CreateLensFromOrderLens(orderLensId, context, csbl);
                // throw new Exception(productCode + " Not Yet Put on Stock, see Accountant for more Details ");
            }

            return lens;

        }

        public static Lens CreateLensFromOrderLens(int orderLensId, EFDbContext context, CumulSaleAndBillLine csbl)
        {
            OrderLens orderLens = context.OrderLenses.SingleOrDefault(ol => ol.ProductID == orderLensId);
            string productCode = orderLens.LensCategoryName + orderLens.LensNumberFullCode;

            Lens lens = context.Lenses.FirstOrDefault(l => l.LensCategoryID == orderLens.LensCategoryID &&
                                                           l.LensNumberID == orderLens.LensNumberID);

            if (lens == null)
            {
                lens = new Lens
                {
                    LocalizationID = csbl.LocalizationID,
                    //Propriétes correspondant au verre
                    //le numéro
                    LensNumber = orderLens.LensNumber,
                    LensNumberID = orderLens.LensNumber.LensNumberID,
                    /*
                    EyeSide = saleLine.EyeSide,
                    Index = saleLine.Index,
                    Axis = saleLine.Axis,
                    Addition = saleLine.Addition,
                    */
                    CategoryID = orderLens.LensCategory.CategoryID,
                    LensCategoryID = orderLens.LensCategory.CategoryID,
                    Category = orderLens.LensCategory,
                    LensCategory = orderLens.LensCategory,
                    //ProductCode = lensCategory.CategoryCode + " " + lensNumber.LensNumberFullCode,
                };
                lens.ProductCode = GetLensCode((Lens)lens);
                lens.Prescription = GetLensCodePrescription((Lens)lens, csbl.EyeSide, csbl.Axis);
                lens = CreateLens(lens, context);
            }

            return lens;

        }

    }

    public class LensValidationError
    {
        public string value; // SPH | CYL
        public string code; // Sucess ou un message detallant le 
        public string errorMessage; // Le message d'erreur qui sera afficher a l'utilisateur final
    }

    public static class LensValidationErrorCode
    {
        // 1- Les valeur 0.00 ou plan ou plano sont interdites pour CYL 
        public const string NO_VALUE_REQUIRED = "NO VALUE REQUIRED";

        // 2- SPH ne peut pas etre vide; il faudra remplacer le vide par 0.00 ou plan ou plano 
        public const string VALUE_REQUIRED = "VALUE REQUIRED";

        // 3-  Un nombre doit etre signe s'il n'est pas 0.00, plan ou plano
        public const string MUST_BE_SIGNED = "MUST BE SIGNED";

        // 4- La partie entiere d'un numero doit avoir au plus 2 caracteres
        public const string INTEGER_PART_MAX_VALUE_VIOLATION = "INTEGER PART MAX VALUE VIOLATION";

        // 5- Les valeurs possibles de la partie entierre sont 00; 25; 50; 75
        public const string DECIMAL_PART_POSSIBILITIES_VIOLATION = "DECIMAL PART POSSIBILITIES VIOLATION";

        // 6- la valeur 0.00 ne doit pas avoir de signe plus ou moins
        public const string NO_SIGN_PLANO = "NO SIGN PLANO";

        // 7- Le numero moins le signe doit etre un nombre decimal(en excluant plan et plano) 
        public const string BAD_FORMAT = "BAD FORMAT";

        // 8- Un numero doit avoir au plus 6 caracteres
        public const string NUMBER_MAX_DIGITS_VIOLATION = "NUMBER MAX DIGITS VIOLATION";

        // 9- Le premier caractere apres le signe ne peut pas etre 0
        public const string FIRST_UNSIGN_DIGIT_NOT_ZERO = "FIRST UNSIGN DIGIT NOT ZERO";

        // 10- L'addition doit toujour etre positif
        public const string ADDITION_MUST_BE_POSITIVE = "ADDITION MUST BE POSITIVE";


        public const string SUCCESS = "SUCCESS";
    }
}