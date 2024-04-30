using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using FatSod.DataContext.Repositories;

namespace CABOPMANAGEMENT.Tools
{
    public static partial class LoadComponent
    {
        public static List<Category> GetAllGenericCategories()
        {
            context = new EFDbContext();
            List<Category> listCategory = context.Categories.Where(cat => !(cat is LensCategory) && cat.CategoryCode!= "RECAPPRODUCT").ToList();
            return listCategory;
        }

        public static List<Category> GetAllDILATIONCategories()
        {
            context = new EFDbContext();
            List<Category> listCategory = context.Categories.Where(cat => !(cat is LensCategory) && !(cat.isSerialNumberNull) && cat.CategoryCode.ToUpper() == "DILATATION").ToList();
            return listCategory;
        }
        public static List<Category> GetAllFrameGenericCategories()
        {
            context = new EFDbContext();
            List<Category> listCategory = context.Categories.Where(cat => !(cat is LensCategory) && !(cat.isSerialNumberNull) && (cat.CategoryCode.ToUpper() != "RECAPPRODUCT" && cat.CategoryCode.ToUpper() != "DILATATION")).ToList();
            return listCategory;
        }

        public static List<ProductBrand> GetAllGenericProductBrands()
        {
            context = new EFDbContext();
            List<ProductBrand> listProductBrand = context.ProductBrands.ToList();

            return listProductBrand;
        }
        public static List<LieuxdeDepotBordero> GetAllGenericLieuxdeDepotBorderos()
        {
            context = new EFDbContext();
            List<LieuxdeDepotBordero> listLieuxdeDepotBordero = context.LieuxdeDepotBorderos.ToList();

            return listLieuxdeDepotBordero;
        }
        public static List<InsuredCompany> GetAllGenericInsuredCompanies()
        {
            context = new EFDbContext();
            List<InsuredCompany> listInsuredCompanies = context.InsuredCompanies.ToList();

            return listInsuredCompanies;
        }
        public static List<Category> GetAllGenericCategoryItems()
        {
            context = new EFDbContext();

            //holds list of Category
            List<Category> _CategoryList = new List<Category>();
            //queries all the Beneficiariess for its ID and Name property.
            var CategoryList = (from s in context.Categories where s.CategoryCode != "RECAPPRODUCT"
                                select new { s.CategoryID, s.CategoryLabel }).ToList();

            //save list of Beneficiariess to the _BeneficiariesList
            foreach (var item in CategoryList)
            {
                _CategoryList.Add(new Category
                {
                    CategoryID = item.CategoryID,
                    CategoryLabel = item.CategoryLabel
                });
            }

            return _CategoryList;
        }

        public static List<Category> GetAllCategoryItems()
        {
            context = new EFDbContext();

            //holds list of Category
            List<Category> _CategoryList = new List<Category>();
            //queries all the Beneficiariess for its ID and Name property.
            var CategoryList = (from s in context.Categories
                                select new { s.CategoryID, s.CategoryLabel }).ToList();

            //save list of Beneficiariess to the _BeneficiariesList
            foreach (var item in CategoryList)
            {
                _CategoryList.Add(new Category
                {
                    CategoryID = item.CategoryID,
                    CategoryLabel = item.CategoryLabel
                });
            }

            return _CategoryList;
        }
        public static List<LensNumber> GetAllLensNumber()
        {
            context = new EFDbContext();
            List<LensNumber> ListLensNumber = new List<LensNumber>();
            foreach (LensNumber lensnum in context.LensNumbers.ToList())
            {
                ListLensNumber.Add(new LensNumber { LensNumberDescription = lensnum.LensNumberDescription, LensNumberID=lensnum.LensNumberID });
            }
            return ListLensNumber;
        }
        public static List<Category> GetProductCategory()
        {
            context = new EFDbContext();
            List<Category> ProductCategory = new List<Category>();

            foreach (Category productcat in context.Categories.ToList())
            {
                ProductCategory.Add(new Category { CategoryCode=productcat.CategoryCode, CategoryID = productcat.CategoryID });
            }

            return ProductCategory;
        }

        public static List<EyeSide> AllEyeSidesPossibilities()
        {
            List<EyeSide> eyeSides = new List<EyeSide>();

            eyeSides.Add(EyeSide.OD);
            eyeSides.Add(EyeSide.OG);
            return eyeSides;
        }
       
        public static List<Product> GetProductCategories()
        {
            context = new EFDbContext();
            List<Product> ProductCategories = new List<Product>();

            List<Product> lstProductCat = new List<Product>();

            IRepositorySupply<Product> prod = new RepositorySupply<Product>(context);

            IEqualityComparer<Product> productComparer = new GenericComparer<Product>("ProductDescription");

            lstProductCat = prod.FindAll.Where(p => p is Lens).Distinct(productComparer).ToList();

            lstProductCat.ForEach(productcat =>
            {
                ProductCategories.Add(new Product { ProductDescription = productcat.ProductDescription, ProductCode = productcat.ProductCode });
            });

            return ProductCategories;
        }

        public static List<GenericProduct> GetGenericProductCategories()
        {
            context = new EFDbContext();
            List<GenericProduct> GenericProductCategories = new List<GenericProduct>();

            List<GenericProduct> lstProductCat = new List<GenericProduct>();

            lstProductCat = context.GenericProducts.ToList();

            lstProductCat.ForEach(productcat =>
            {
                GenericProductCategories.Add(new GenericProduct { ProductLabel=productcat.ProductLabel, ProductID = productcat.ProductID });
            });

            return GenericProductCategories;
        }
        public static List<LensCategory> GetLensCategories()
        {
            context = new EFDbContext();
            List<LensCategory> LensCategorList = new List<LensCategory>();

            List<LensCategory> LensCategories = new List<LensCategory>();

            IRepositorySupply<LensCategory> prod = new RepositorySupply<LensCategory>(context);

            //IEqualityComparer<Product> productComparer = new GenericComparer<Product>("ProductDescription");

            LensCategories = prod.FindAll.ToList();

            LensCategories.ForEach(productcat =>
            {
                LensCategorList.Add(new LensCategory { CategoryCode=productcat.CategoryCode, CategoryID = productcat.CategoryID });
            });

            return LensCategorList;
        }

        public static List<LensCategory> GetAllLensCategories()
        {
            context = new EFDbContext();
            List<LensCategory> LensCategorList = new List<LensCategory>();

            List<LensCategory> LensCategories = new List<LensCategory>();

            IRepositorySupply<LensCategory> prod = new RepositorySupply<LensCategory>(context);


            LensCategories = prod.FindAll.ToList();

            LensCategories.ForEach(productcat =>
            {
                LensCategorList.Add(new LensCategory { CategoryDescription=productcat.CategoryDescription, CategoryCode = productcat.CategoryCode });
            });

            return LensCategorList;
        }

        public static List<LensCategory> GetAllLensCategoriesID()
        {
            context = new EFDbContext();
            List<LensCategory> LensCategorList = new List<LensCategory>();

            List<LensCategory> LensCategories = new List<LensCategory>();

            IRepositorySupply<LensCategory> prod = new RepositorySupply<LensCategory>(context);

            //IEqualityComparer<Product> productComparer = new GenericComparer<Product>("ProductDescription");

            LensCategories = prod.FindAll.ToList();

            LensCategories.ForEach(productcat =>
            {
                LensCategorList.Add(new LensCategory { CategoryCode=productcat.CategoryCode,CategoryID= productcat.CategoryID });
            });

            return LensCategorList;
        }
        public static List<Branch> getAllBranches()
        {
            context = new EFDbContext();
            List<Branch> branches = new List<Branch>();
            foreach (Branch branch in context.Branches)
            {
                branches.Add(new Branch { BranchCode = branch.BranchCode, BranchID=branch.BranchID });
            }

            return branches;
        }

        public static List<Country> getAllCountries()
        {
            context = new EFDbContext();
            List<Country> countries = new List<Country>();
            
            foreach (Country country in context.Countries)
            {
                countries.Add(new Country {CountryCode= country.CountryCode, CountryID=country.CountryID });
            }

            return countries;
        }


    }
}