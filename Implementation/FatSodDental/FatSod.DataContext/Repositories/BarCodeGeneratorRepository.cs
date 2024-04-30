using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
    public class BarCodeGeneratorRepository : RepositorySupply<BarCodeGenerator>, IBarCodeGenerator
    {
        public BarCodeGeneratorRepository(EFDbContext context)
        {
            this.context = context;
        }
        public BarCodeGeneratorRepository()
            : base()
        {

        }
        public bool valideBarCodeGenerator(BarCodeGenerator barCodeGenerator)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                    {
                        int MaxbarCode = 0;
                        //si table BarCodeGenerator vide
                        //BarCodeGenerator barcodeProd = context.BarCodeGenerators.Count();
                        if (context.BarCodeGenerators.Count()<=0)
                        {
                            MaxbarCode = 0;
                        }
                        else
                        {
                            MaxbarCode = context.BarCodeGenerators.Where(b => b != null).Max(b => b.CompteurCodeBar);
                        }
                       
                        //verification de la longueur du code de l'operation
                        if ( MaxbarCode == 0)
                        {
                            MaxbarCode = 0;
                        }
                        int newCompteur = MaxbarCode + 1;
                        string barcode = (newCompteur.ToString().Length < 8) ? newCompteur.ToString().PadLeft(8, '0') : newCompteur.ToString();
                        barCodeGenerator.CompteurCodeBar = newCompteur;
                        barCodeGenerator.CodeBar = barcode;
                        context.BarCodeGenerators.Add(barCodeGenerator);
                        context.SaveChanges();
                        res = true;
                        ts.Complete();
                        return res;
                    }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public string generateBarcodeNumber()
        {
            string res = "";
            try
            {
                //si table BarCodeGenerator vide
                int MaxbarCode = context.BarCodeGenerators.Max(b => b == null ? 0 : b.CompteurCodeBar);
                //verification de la longueur du code de l'operation
                if ( MaxbarCode == 0)
                {
                    MaxbarCode = 0;
                }
                int newCompteur = MaxbarCode + 1;
                string barcode = (newCompteur.ToString().Length < 8) ? newCompteur.ToString().PadLeft(8, '0') : newCompteur.ToString();
                res = barcode;
                return res;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public bool miseajourBarCodeGenerator(BarCodeGenerator barCodeGenerator)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //we update it
                    BarCodeGenerator upBarCodeGenerator = context.BarCodeGenerators.Find(barCodeGenerator.BarCodeGeneratorID);
                    if (upBarCodeGenerator != null)
                    {
                        upBarCodeGenerator.CodeBar = barCodeGenerator.CodeBar;
                        upBarCodeGenerator.CompteurCodeBar = barCodeGenerator.CompteurCodeBar;
                        upBarCodeGenerator.GenerateDate = barCodeGenerator.GenerateDate;
                        upBarCodeGenerator.GeneratedByID = barCodeGenerator.GeneratedByID;
                        upBarCodeGenerator.ProductCode = barCodeGenerator.ProductCode;
                        upBarCodeGenerator.ProductID = barCodeGenerator.ProductID;
                        upBarCodeGenerator.QtyGenerate = barCodeGenerator.QtyGenerate;
                        context.SaveChanges();
                        res = true;
                    }
                    ts.Complete();
                    return res;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool supprimeBarCodeGenerator(int BarCodeGeneratorID)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //we update it
                    BarCodeGenerator upBarCodeGenerator = context.BarCodeGenerators.Find(BarCodeGeneratorID);
                    if (upBarCodeGenerator != null)
                    {
                        context.BarCodeGenerators.Remove(upBarCodeGenerator);
                        context.SaveChanges();
                        res = true;
                    }
                    ts.Complete();
                    return res;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        

    }
}
