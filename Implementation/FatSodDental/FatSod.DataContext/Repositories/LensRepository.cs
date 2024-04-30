using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using System.Data.Entity;
using System.Globalization;

namespace FatSod.DataContext.Repositories
{
    public class LensRepository : RepositorySupply<Lens>, ILens
    {
        public LensRepository(EFDbContext context1)
            : base(context1)
        {
            this.context = context1;

        }

        public LensRepository()
            : base()
        {


        }


        /// <summary>
        /// Permet de Générer les verre dont les numéros sont dans un intervalle de numéro : 
        /// </summary>
        /// <param name="lensCategoryCode">Code de la catégorie des verres à crééer</param>
        /// <param name="sphericalValueRange">intervalle des valeurs sphériques pour lesquelles on doit crééer les verres</param>
        /// <param name="cylindricalValueRange">intervalle des valeurs cylindrique pour lesquelles on doit crééer les verres</param>
        /// <param name="additionValueRange">intervalle des valeurs addition pour lesquelles on doit crééer les verres</param>
        /// <returns>Liste des verres créés</returns>
        /// 
        /// on peut avoir 8 cas de figure pour le couple (SphRange, CylRange, AddRange) qui sont : 
        /// 1 - (0 0 0)
        /// 2 - (0 0 1)
        /// 3 - (0 1 0)
        /// 4 - (0 1 1)
        /// 5 - (1 0 0)
        /// 6 - (1 0 1)
        /// 7 - (1 1 0)
        /// 8 - (1 1 1)
        ///
        /// 1 = l'intervalle contient au moins un élément(numéro)
        /// 0 = l'intervalle ne contient pas d'élément(numéro)
        /// 
        /// Il n'y a que les 6 dernières couples qui sont possibles
        public List<Lens> CreateLens(string lensCategoryCode, LensNumberRange sphericalValueRange, LensNumberRange cylindricalValueRange, LensNumberRange additionValueRange, int[] storeIds)
        {
            List<Lens> res = new List<Lens>();

            CultureInfo enUsCI = CultureInfo.GetCultureInfo("en-US");


            if (sphericalValueRange != null && sphericalValueRange.IsValid())//on a au moins uune valeur sphérique
            {
                if (cylindricalValueRange != null && cylindricalValueRange.IsValid())//on a au moins une valeur cylindrique
                {
                    if (additionValueRange != null && additionValueRange.IsValid())//on a au moins une valeur addition            = (1 1 1)
                    {
                        string sphVal = "";
                        string cylVal = "";
                        string addVal = "";
                        decimal sphMin = Convert.ToDecimal(sphericalValueRange.Minimum, enUsCI);
                        decimal sphMax = Convert.ToDecimal(sphericalValueRange.Maximum, enUsCI);

                        decimal cylMin = Convert.ToDecimal(cylindricalValueRange.Minimum, enUsCI);
                        decimal cylMax = Convert.ToDecimal(cylindricalValueRange.Maximum, enUsCI);

                        decimal addMin = Convert.ToDecimal(additionValueRange.Minimum, enUsCI);
                        decimal addMax = Convert.ToDecimal(additionValueRange.Maximum, enUsCI);

                        for (decimal sph = sphMin; sph <= sphMax; sph += 0.25m)
                        {

                            sphVal = (sph <= 0) ? (sph.ToString().Replace(",", ".")) : ("+" + sph.ToString().Replace(",", "."));
                            for (decimal cyl = cylMin; cyl <= cylMax; cyl += 0.25m)
                            {
                                //bool a = cyl < -6.00m || cyl > 6.00m;

                                cylVal = (cyl <= 0) ? (cyl.ToString().Replace(",", ".")) : ("+" + cyl.ToString().Replace(",", "."));
                                cylVal = (cyl == 0) ? "" : cylVal;//évite de créer les couple : +1.00 0.00 et favorise la création des couples +1.00 et +1.00 add +3.00 
                                for (decimal add = addMin; add <= addMax; add += 0.25m)
                                {

                                    addVal = (add <= 0) ? (add.ToString().Replace(",", ".")) : ("+" + add.ToString().Replace(",", "."));
                                    addVal = (add == 0) ? "" : addVal;//Eviter la création des couples : +1.00 -1.00 add 0.00 et +1.00 add 0.00
                                    this.CreateLens(lensCategoryCode, sphVal, cylVal, addVal, storeIds);
                                }
                            }
                        }

                        return res;
                    }

                    if (additionValueRange == null || !additionValueRange.IsValid())//on a aucune valeur addition                 = (1 1 0)
                    {
                        string sphVal = "";
                        string cylVal = "";
                        string addVal = "";


                        decimal sphMin = Convert.ToDecimal(sphericalValueRange.Minimum, enUsCI);
                        decimal sphMax = Convert.ToDecimal(sphericalValueRange.Maximum, enUsCI);

                        decimal cylMin = Convert.ToDecimal(cylindricalValueRange.Minimum, enUsCI);
                        decimal cylMax = Convert.ToDecimal(cylindricalValueRange.Maximum, enUsCI);

                        for (decimal sph = sphMin; sph <= sphMax; sph += 0.25m)
                        {

                            sphVal = (sph <= 0) ? (sph.ToString().Replace(",", ".")) : ("+" + sph.ToString().Replace(",", "."));
                            for (decimal cyl = cylMin; cyl <= cylMax; cyl += 0.25m)
                            {
                                cylVal = (cyl <= 0) ? (cyl.ToString().Replace(",", ".")) : ("+" + cyl.ToString().Replace(",", "."));
                                cylVal = (cyl == 0) ? "" : cylVal;//évite de créer les couple : +1.00 0.00 et favorise la création des couples +1.00 et +1.00 add +3.00 

                                this.CreateLens(lensCategoryCode, sphVal, cylVal, addVal, storeIds);
                            }
                        }

                        return res;
                    }
                    return res;
                }

                if (cylindricalValueRange == null || !cylindricalValueRange.IsValid())//on a aucune valeur cylindrique 
                {
                    if (additionValueRange != null && additionValueRange.IsValid())//on a au moins une valeur addition            = (1 0 1)
                    {

                        string sphVal = "";
                        string cylVal = "";
                        string addVal = "";

                        decimal sphMin = Convert.ToDecimal(sphericalValueRange.Minimum, enUsCI);
                        decimal sphMax = Convert.ToDecimal(sphericalValueRange.Maximum, enUsCI);

                        decimal addMin = Convert.ToDecimal(additionValueRange.Minimum, enUsCI);
                        decimal addMax = Convert.ToDecimal(additionValueRange.Maximum, enUsCI);

                        for (decimal sph = sphMin; sph <= sphMax; sph += 0.25m)
                        {
                            sphVal = (sph <= 0) ? (sph.ToString().Replace(",", ".")) : ("+" + sph.ToString().Replace(",", "."));
                            for (decimal add = addMin; add <= addMax; add += 0.25m)
                            {

                                addVal = (add <= 0) ? (add.ToString().Replace(",", ".")) : ("+" + add.ToString().Replace(",", "."));
                                addVal = (add == 0) ? "" : addVal;//Eviter la création des couples : +1.00 -1.00 add 0.00 et +1.00 add 0.00
                                this.CreateLens(lensCategoryCode, sphVal, cylVal, addVal, storeIds);
                            }
                        }

                        return res;
                    }

                    if (additionValueRange == null || !additionValueRange.IsValid())//on a aucune valeur addition                 = (1 0 0)
                    {
                        string sphVal = "";
                        string cylVal = "";
                        string addVal = "";

                        decimal sphMin = Convert.ToDecimal(sphericalValueRange.Minimum, enUsCI);
                        decimal sphMax = Convert.ToDecimal(sphericalValueRange.Maximum, enUsCI);

                        for (decimal sph = sphMin; sph <= sphMax; sph += 0.25m)
                        {
                            sphVal = (sph <= 0) ? (sph.ToString().Replace(",", ".")) : ("+" + sph.ToString().Replace(",", "."));
                            this.CreateLens(lensCategoryCode, sphVal, cylVal, addVal, storeIds);
                        }

                        return res;
                    }
                    return res;
                }

            }


            if (cylindricalValueRange != null && cylindricalValueRange.IsValid())//on a au moins une valeur cylindrique
            {

                if (sphericalValueRange == null || !sphericalValueRange.IsValid())//on a au moins une valeur sphérique
                {
                    if (additionValueRange != null && additionValueRange.IsValid())//on a au moins une valeur addition           = (0 1 1)
                    {
                        string sphVal = "0.00";
                        string cylVal = "";
                        string addVal = "";

                        decimal cylMin = Convert.ToDecimal(cylindricalValueRange.Minimum, enUsCI);
                        decimal cylMax = Convert.ToDecimal(cylindricalValueRange.Maximum, enUsCI);

                        decimal addMin = Convert.ToDecimal(additionValueRange.Minimum, enUsCI);
                        decimal addMax = Convert.ToDecimal(additionValueRange.Maximum, enUsCI);

                        for (decimal cyl = cylMin; cyl <= cylMax; cyl += 0.25m)
                        {
                            cylVal = (cyl <= 0) ? (cyl.ToString().Replace(",", ".")) : ("+" + cyl.ToString().Replace(",", "."));
                            cylVal = (cyl == 0) ? "" : cylVal;//évite de créer les couple : +1.00 0.00 et favorise la création des couples +1.00 et +1.00 add +3.00 
                            for (decimal add = addMin; add <= addMax; add += 0.25m)
                            {

                                addVal = (add <= 0) ? (add.ToString().Replace(",", ".")) : ("+" + add.ToString().Replace(",", "."));
                                addVal = (add == 0) ? "" : addVal;//Eviter la création des couples : +1.00 -1.00 add 0.00 et +1.00 add 0.00
                                this.CreateLens(lensCategoryCode, sphVal, cylVal, addVal, storeIds);
                            }
                        }

                        return res;
                    }

                    if (additionValueRange == null || !additionValueRange.IsValid())//on a aucune valeur addition                 = (0 1 0)
                    {

                        string sphVal = "0.00";
                        string cylVal = "";
                        string addVal = "";

                        decimal cylMin = Convert.ToDecimal(cylindricalValueRange.Minimum, enUsCI);
                        decimal cylMax = Convert.ToDecimal(cylindricalValueRange.Maximum, enUsCI);

                        for (decimal cyl = cylMin; cyl <= cylMax; cyl += 0.25m)
                        {
                            cylVal = (cyl <= 0) ? (cyl.ToString().Replace(",", ".")) : ("+" + cyl.ToString().Replace(",", "."));
                            if (cyl == 0) continue;//évite de créer les couple : 0.00 0.00 
                            this.CreateLens(lensCategoryCode, sphVal, cylVal, addVal, storeIds);
                        }

                        return res;
                    }
                    return res;
                }

            }

            return res;
        }
        /// <summary>
        /// Permet de crééer un verre si et seulement s'il n'existe pas encore
        /// </summary>
        /// <param name="lensCategoryCode">Code de la cat"gorie du verre </param>
        /// <param name="sphericalValue">valeur sphérique du verre</param>
        /// <param name="cylindricalValue">valeur cylindrique du verre</param>
        /// <param name="additionValue">valeur de l'addition du verre</param>
        /// <returns>Verre qui a été créé</returns>
        public Lens CreateLens(string lensCategoryCode, string sphericalValue, string cylindricalValue, string additionValue, int[] StoreIds)
        {
            Lens res = new Lens();
            try
            { 
            IAccount accountRepo = new AccountRepository(context);
            string accountLabel = lensCategoryCode + " " + CodeValue.Accounting.ACCOUNTNAME;
            //CollectifAccount colAcc = context.LensCategories.AsNoTracking().SingleOrDefault(cat => cat.CategoryCode == lensCategoryCode).CollectifAccount;
            string collectifAccountLabel = lensCategoryCode + " " + CodeValue.Accounting.ACCOUNTCOLLECTIF;
            Account acc = accountRepo.GenerateAccountNumber(CodeValue.Accounting.DefaultCodeAccountingSection.CODEPROD, collectifAccountLabel, accountLabel, false);

            LensCategory category = context.LensCategories.AsNoTracking().SingleOrDefault(cat => cat.CategoryCode == lensCategoryCode);

            LensNumber number = this.GetNumber(sphericalValue, cylindricalValue, additionValue);

            string productCode = lensCategoryCode + " " + number.LensNumberFullCode;
            res = context.Lenses.AsNoTracking().SingleOrDefault(l => l.ProductCode == productCode);

            //Création du verre parcequ'il n'existe pas encore
            if (res == null || res.ProductID <= 0)
            {
                res = new Lens()
                {
                    AccountID = acc.AccountID,
                    CategoryID = category.CategoryID,
                    LensCategoryID = category.CategoryID,
                    LensNumberID = number.LensNumberID,
                    ProductCode = productCode,
                    ProductDescription = lensCategoryCode,
                    ProductLabel = lensCategoryCode,
                    SellingPrice = 1
                };
                res = this.Create(res);
            }

            if (StoreIds != null && StoreIds.Count() > 0)
            {

                foreach (int id in StoreIds)
                {
                    ProductLocalization pl = context.ProductLocalizations.AsNoTracking().SingleOrDefault(pdl => pdl.LocalizationID == id &&
                                                                                                               pdl.ProductID == res.ProductID);

                    if (pl == null || pl.ProductLocalizationID == 0)
                    {
                        pl = new ProductLocalization()
                        {
                            LocalizationID = id,
                            ProductID = res.ProductID,
                            ProductLocalizationStockQuantity = 0,
                            ProductLocalizationSafetyStockQuantity = 0,
                            ProductLocalizationStockSellingPrice = 0,
                            AveragePurchasePrice = 0,
                            ProductLocalizationDate = context.BusinessDays.FirstOrDefault(bd => (bd.BDStatut == true) && (bd.ClosingDayStarted == false)).BDDateOperation,
                        };

                        context.ProductLocalizations.Add(pl);
                        context.SaveChanges();

                    }

                }
            }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return res;
        }
        /// <summary>
        /// Retourne le numéro d'un verre s'il existe ou le créée
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public LensNumber GetNumber(LensNumber number)
        {
            LensNumber res = new LensNumber();

            res = context.LensNumbers.AsNoTracking().SingleOrDefault(ln => ln.LensNumberAdditionValue == number.LensNumberAdditionValue &&
                                                                           ln.LensNumberSphericalValue == number.LensNumberSphericalValue &&
                                                                           ln.LensNumberCylindricalValue == number.LensNumberCylindricalValue);
            if (res == null || res.LensNumberID <= 0)
            {
                res = context.LensNumbers.Add(number);
                context.SaveChanges();
            }

            return res;
        }

        /// <summary>
        /// Retourne le numéro d'un verre s'il existe ou le créée
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public LensNumber GetNumber(string sphericalValue, string cylindricalValue, string additionValue)
        {
            LensNumber res = new LensNumber()
            {
                LensNumberAdditionValue = additionValue,
                LensNumberCylindricalValue = cylindricalValue,
                LensNumberSphericalValue = sphericalValue,
            };

            res.LensNumberDescription = res.LensNumberFullCode;

            return GetNumber(res);
        }

        /// <summary>
        /// Permet de crééer un verre si et seulement s'il n'existe pas encore
        /// </summary>
        /// <param name="lensCategoryCode">Code de la cat"gorie du verre </param>
        /// <param name="number">numéro du verre</param>
        /// <returns>Verre qui a été créé</returns>
        public Lens CreateLens(string lensCategoryCode, LensNumber number, int[] storeIds)
        {
            Lens res = new Lens();

            res = this.CreateLens(lensCategoryCode, number.LensNumberSphericalValue, number.LensNumberCylindricalValue, number.LensNumberAdditionValue, storeIds);

            return res;
        }

        /// <summary>
        /// Cette méthode retourne la liste des numéros de verre qui ont déjà été utilisée d'une catégorie.
        /// </summary>
        /// <param name="Category">Catégorie du verre dont on veut lma liste des numéros déjà attribuées</param>
        /// <returns>Liste des numéros déjà attribués de cette catégorie</returns>
        public List<LensNumber> GetAllAvaillableNumbers(LensCategory Category)
        {
            List<LensNumber> res = new List<LensNumber>();

            res = context.Lenses.Where(l => l.LensCategoryID == Category.CategoryID).Select(l1 => l1.LensNumber).ToList();

            return res;

        }

        /// <summary>
        /// Cette méthode retourne la liste des numéros de verre qui ont déjà été utilisée d'une catégorie.
        /// </summary>
        /// <param name="Category">Catégorie du verre dont on veut lma liste des numéros déjà attribuées</param>
        /// <returns>Liste des numéros déjà attribués de cette catégorie</returns>
        public List<LensNumber> GetAllAvaillableNumbers(int LensCategoryID)
        {
            List<LensNumber> res = new List<LensNumber>();

            res = context.Lenses.Where(l => l.LensCategoryID == LensCategoryID).Select(l1 => l1.LensNumber).ToList();

            return res;

        }

    }
}
