using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using Ext.Net;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using FatSod.DataContext.Repositories;

namespace FatSodDental.UI.Tools
{
    public static partial class LoadComponent
    {
        public static List<Category> GetAllGenericCategories()
        {
            context = new EFDbContext();
            List<ListItem> categories = new List<ListItem>();
            List<Category> listCategory = context.Categories.Where(cat => !(cat is LensCategory)).ToList();

            return listCategory;
        }
        public static List<ListItem> GetAllLensType()
        {
            List<ListItem> LensType = new List<ListItem>();
            LensType.Add(new ListItem(FatSod.Ressources.Resources.Progressif, "PROG"));
            LensType.Add(new ListItem(FatSod.Ressources.Resources.SingleVision, "SV"));
            LensType.Add(new ListItem(FatSod.Ressources.Resources.Bifocal, "BIFOCAL"));
            //LensType.Add(new ListItem(FatSod.Ressources.Resources.BifocalRTop, "R-TOP"));
            return LensType;
        }
        public static List<ListItem> GetAllGenericCategoryItems()
        {
            context = new EFDbContext();
            List<ListItem> categories = new List<ListItem>();
            List<Category> listCategory = GetAllGenericCategories();

            foreach (Category category in listCategory)
            {
                categories.Add(new ListItem(category.CategoryLabel, category.CategoryID));
            }

            return categories;
        }
        public static List<ListItem> getAllCategories()
        {
            context = new EFDbContext();
            List<ListItem> categories = new List<ListItem>();
            ListItem choice = new ListItem(FatSod.Ressources.Resources.Select, 0);
            choice.Index = 0;
            //categories.Add(choice);
            foreach (Category category in context.Categories)
            {
                categories.Add(new ListItem(category.CategoryLabel, category.CategoryID));
            }

            return categories;
        }
        public static List<ListItem> GetAllLensNumber()
        {
            context = new EFDbContext();
            List<ListItem> ListLensNumber = new List<ListItem>();
            foreach (LensNumber lensnum in context.LensNumbers.ToList())
            {
                ListLensNumber.Add(new ListItem(lensnum.LensNumberDescription, lensnum.LensNumberID));
            }
            return ListLensNumber;
        }
        public static List<ListItem> GetProductCategory()
        {
            context = new EFDbContext();
            List<ListItem> ProductCategory = new List<ListItem>();

            //var lstProductCat = context.Categories.Join(context.LensCategories, cat => cat.CategoryID, ln => ln.CategoryID,
            //    (cat, ln) => new { cat, ln })
            //    .Where(lsp => !lsp.ln.IsSpecialCategory)
            //    .Select(s => new
            //    {
            //        CategoryID = s.cat.CategoryID,
            //        CategoryCode = s.cat.CategoryCode,
            //    })
            //    .ToList();
            //foreach (var productcat in lstProductCat)
            foreach (Category productcat in context.Categories.ToList())
            {
                ProductCategory.Add(new ListItem(productcat.CategoryCode, productcat.CategoryID));
            }

            return ProductCategory;
        }

        public static List<ListItem> getAllGenericProducts()
        {
            context = new EFDbContext();
            List<ListItem> GenericProducts = new List<ListItem>();
            foreach (GenericProduct genericProducts in context.GenericProducts)
            {
                GenericProducts.Add(new ListItem(genericProducts.ProductCode, genericProducts.ProductID));
            }

            return GenericProducts;
        }
        public static List<ListItem> getEquipmentProducts()
        {
            context = new EFDbContext();
            List<ListItem> GenericProducts = new List<ListItem>();
            foreach (GenericProduct genericProducts in context.GenericProducts.Where(g => g.CategoryID == 2))
            {
                GenericProducts.Add(new ListItem(genericProducts.ProductCode, genericProducts.ProductID));
            }

            return GenericProducts;
        }
        public static List<ListItem> getAllFramesProducts()
        {
            context = new EFDbContext();
            List<ListItem> GenericProducts = new List<ListItem>();
            foreach (GenericProduct genericProducts in context.GenericProducts.Where(g => g.CategoryID == 1))
            {
                GenericProducts.Add(new ListItem(genericProducts.ProductCode, genericProducts.ProductID));
            }

            return GenericProducts;
        }
        public static List<ListItem> getAllSalesProductsType()
        {
            context = new EFDbContext();
            List<ListItem> SalesProductsType = new List<ListItem>();

            SalesProductsType.Add(new ListItem(FatSod.Ressources.Resources.VerreEtMonture, 1));
            //SalesProductsType.Add(new ListItem(FatSod.Ressources.Resources.Autre, 2));
            //SalesProductsType.Add(new ListItem(FatSod.Ressources.Resources.Autre, 3));

            return SalesProductsType;
        }
        public static List<ListItem> AllEyeSidesPossibilities()
        {
            List<ListItem> eyeSides = new List<ListItem>();

            eyeSides.Add(new ListItem(Resources.RightSide, EyeSide.OD));
            eyeSides.Add(new ListItem(Resources.LeftSide, EyeSide.OG));
            return eyeSides;
        }

        public static List<ListItem> GetFrameMaterial()
        {
            List<ListItem> FrameMaterial = new List<ListItem>();

            FrameMaterial.Add(new ListItem(Resources.PLASTIQUE, "PLASTIQUE"));
            FrameMaterial.Add(new ListItem(Resources.METALIQUE, "METALLIQUE"));
            FrameMaterial.Add(new ListItem(Resources.MIX, "MIXTE"));
            return FrameMaterial;
        }
        public static List<ListItem> GetProductCategories()
        {
            context = new EFDbContext();
            List<ListItem> ProductCategories = new List<ListItem>();

            List<Product> lstProductCat = new List<Product>();

            IRepositorySupply<Product> prod = new RepositorySupply<Product>(context);

            IEqualityComparer<Product> productComparer = new GenericComparer<Product>("ProductDescription");

            lstProductCat = prod.FindAll.Where(p => p is Lens).Distinct(productComparer).ToList();

            lstProductCat.ForEach(productcat =>
            {
                ProductCategories.Add(new ListItem(productcat.ProductDescription, productcat.ProductDescription));
            });

            return ProductCategories;
        }

        public static List<ListItem> GetLensCategories()
        {
            context = new EFDbContext();
            List<ListItem> LensCategorList = new List<ListItem>();

            List<LensCategory> LensCategories = new List<LensCategory>();

            IRepositorySupply<LensCategory> prod = new RepositorySupply<LensCategory>(context);

            //IEqualityComparer<Product> productComparer = new GenericComparer<Product>("ProductDescription");

            LensCategories = prod.FindAll.ToList();

            LensCategories.ForEach(productcat =>
            {
                LensCategorList.Add(new ListItem(productcat.CategoryCode, productcat.CategoryID));
            });

            return LensCategorList;
        }

        public static List<ListItem> GetAllLensCategories()
        {
            context = new EFDbContext();
            List<ListItem> LensCategorList = new List<ListItem>();

            List<LensCategory> LensCategories = new List<LensCategory>();

            IRepositorySupply<LensCategory> prod = new RepositorySupply<LensCategory>(context);

            //IEqualityComparer<Product> productComparer = new GenericComparer<Product>("ProductDescription");

            LensCategories = prod.FindAll.ToList();

            LensCategories.ForEach(productcat =>
            {
                LensCategorList.Add(new ListItem(productcat.CategoryCode, productcat.CategoryCode));
            });

            return LensCategorList;
        }

        public static List<ListItem> GetAllLensCategoriesID()
        {
            context = new EFDbContext();
            List<ListItem> LensCategorList = new List<ListItem>();

            List<LensCategory> LensCategories = new List<LensCategory>();

            IRepositorySupply<LensCategory> prod = new RepositorySupply<LensCategory>(context);

            //IEqualityComparer<Product> productComparer = new GenericComparer<Product>("ProductDescription");

            LensCategories = prod.FindAll.ToList();

            LensCategories.ForEach(productcat =>
            {
                LensCategorList.Add(new ListItem(productcat.CategoryCode, productcat.CategoryID));
            });

            return LensCategorList;
        }
        public static List<ListItem> getAllBranches()
        {
            context = new EFDbContext();
            List<ListItem> branches = new List<ListItem>();
            foreach (Branch branch in context.Branches)
            {
                branches.Add(new ListItem(branch.BranchCode, branch.BranchID));
            }

            return branches;
        }

        public static List<ListItem> getAllCountries()
        {
            context = new EFDbContext();
            List<ListItem> countries = new List<ListItem>();
            ListItem choice = new ListItem(FatSod.Ressources.Resources.Select, 0);
            choice.Index = 0;
            countries.Add(choice);
            foreach (Country country in context.Countries)
            {
                countries.Add(new ListItem(country.CountryCode, country.CountryID));
            }

            return countries;
        }


    }
}