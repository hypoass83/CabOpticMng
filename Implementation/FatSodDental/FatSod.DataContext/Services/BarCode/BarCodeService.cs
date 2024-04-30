
using FatSod.Supply.Abstracts.BarCode;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Supply.Entities;
using System;

namespace FatSod.DataContext.Services.BarCode
{
    public class BarCodeService : IBarCodeService
    {
        private readonly EFDbContext context;

        public BarCodeService(EFDbContext context)
        {
            this.context = context;
        }

        public string GetBarCode(BarCodePayload payload)
        {
            var stock = context.ProductLocalizations.SingleOrDefault(pl => pl.LocalizationID == payload.LocationId &&
                                                                           pl.ProductID == payload.ProductId && 
                                                                           pl.NumeroSerie == payload.NumeroSerie &&
                                                                           pl.Marque == payload.Marque);

            if(stock.BarCode == null)
            {
                stock.BarCode = GetNextBarCodeNumber();
                context.SaveChanges();
            }

            return stock.BarCode;
        }

        public string GetPayload(BarCodePayload payload)
        {
            return payload.BarCode;
        }

        public BarCodePayload GetPayload(string barCode)
        {
            var stock = context.ProductLocalizations.SingleOrDefault(pl => pl.BarCode == barCode);

            var res = new BarCodePayload() { 
                BarCode = barCode,
                Marque = stock.Marque,
                LocationId = stock.LocalizationID,
                ProductId = stock.ProductID,
                ProductLocalisationId = stock.ProductLocalizationID
            };

            return res;
        }

        public ProductLocalization GetProductLocalization(string barCode)
        {
            var res = context.ProductLocalizations.SingleOrDefault(pl => pl.BarCode == barCode);
            return res;
        }

        #region TOOLS
        public string GetNextBarCodeNumber(string code = "FRA")
        {
            string res = "";
            try
            {
                TransactNumber transactNumber = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == code);
                if (transactNumber == null)
                {
                    transactNumber = new TransactNumber()
                    {
                        TransactNumberCode = code,
                        Counter = 1,
                    };
                    context.TransactNumbers.Add(transactNumber);
                }
                else
                {
                    transactNumber.Counter++;
                }
                context.SaveChanges();
                res = Format(code, transactNumber.Counter);
                return res;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        string Format(string code, int counter)
        {
            var emptyCharacters = "";
            if (counter < 10)
                emptyCharacters = "00000";

            if (counter >= 10 && counter < 100)
                emptyCharacters = "0000";

            if (counter >= 100 && counter < 1000)
                emptyCharacters = "000";

            if (counter >= 1000 && counter < 10000)
                emptyCharacters = "00";

            if (counter >= 10000 && counter < 100000)
                emptyCharacters = "0";

            return code + "-" + emptyCharacters + counter;
        }
        #endregion
    }
}
