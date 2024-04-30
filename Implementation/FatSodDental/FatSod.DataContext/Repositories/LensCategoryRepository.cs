using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;


namespace FatSod.DataContext.Repositories
{
    /// <summary>
    /// LensCategoryRepository
    /// </summary>
    public class LensCategoryRepository : RepositorySupply<LensCategory>, ILensCategory
    {
        /// <summary>
        /// 
        /// </summary>
        public LensCategoryRepository() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public LensCategoryRepository(EFDbContext ctx)
            : base(ctx)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lensCategory"></param>
        /// <returns></returns>
        public LensCategory CreateLensCategory(LensCategory lensCategory)
        {
            LensCategory res = new LensCategory();

            //Begin of transaction
                try
            {
                    using (TransactionScope ts = new TransactionScope())
                {
                    ICollectifAccount colAccRepo = new CollectifAccountRepository(context);
                    string collectifAccountLabel = lensCategory.CategoryCode + " " + CodeValue.Accounting.ACCOUNTCOLLECTIF;
                    CollectifAccount cacc = colAccRepo.GetCollectifAccount(CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD, collectifAccountLabel);

                    LensCoating defaultLC = context.LensCoatings.AsNoTracking().SingleOrDefault(lc => lc.LensCoatingCode == CodeValue.Supply.DefaultLensCoating);
                    LensColour defaultLCol = context.LensColours.AsNoTracking().SingleOrDefault(lc => lc.LensColourCode == CodeValue.Supply.DefaultLensColour);
                    LensMaterial defaultLM = context.LensMaterials.AsNoTracking().SingleOrDefault(lc => lc.LensMaterialCode == CodeValue.Supply.DefaultLensMaterial);

                    lensCategory.CollectifAccountID = cacc.CollectifAccountID;
                    lensCategory.LensMaterialID = defaultLM.LensMaterialID;
                    lensCategory.LensCoatingID = defaultLC.LensCoatingID;
                    lensCategory.LensColourID = defaultLCol.LensColourID;

                    res = this.Create(lensCategory);

                        //transaction.Commit();
                        ts.Complete();
                    }
                    

                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    throw new Exception(e.Message + ": Check " + e.InnerException);
                }
                return res;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lensCategory"></param>
        /// <returns></returns>
        public LensCategory UpdateLensCategory(LensCategory lensCategory)
        {
            LensCategory res = new LensCategory();

            LensCategory oldLensCategory = context.LensCategories.AsNoTracking().SingleOrDefault(lc => lc.CategoryID == lensCategory.CategoryID);
            lensCategory.CollectifAccountID = oldLensCategory.CollectifAccountID;
            lensCategory.LensMaterialID = oldLensCategory.LensMaterialID;
            lensCategory.LensCoatingID = oldLensCategory.LensCoatingID;
            lensCategory.LensColourID = oldLensCategory.LensColourID;
            //lensCategory.TypeLens = oldLensCategory.TypeLens;

            res = this.Update(lensCategory, lensCategory.CategoryID);
            if (oldLensCategory.CategoryCode != lensCategory.CategoryCode)
            {
                //Mise à jour des codes de tous les verres de cette catégorie
                List<Lens> updatedLenses = context.Lenses.Where(l => l.LensCategoryID == lensCategory.CategoryID).ToList();
                if (updatedLenses != null && updatedLenses.Count > 0)
                {
                    foreach (Lens lens in updatedLenses)
                    {
                        lens.ProductCode = res.CategoryCode + " " + lens.LensNumberFullCode;
                    }
                    context.SaveChanges();
                }
            }
           
            return res;
        }
    }
}
