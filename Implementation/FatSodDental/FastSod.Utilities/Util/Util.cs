using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSod.Utilities.Util
{
    public class Util
    {
        /// <summary>
        /// Returns an extras price of one operation that.
        /// </summary>
        /// <param name="grossAmount"> A gross amount of operation. A sum of sale lines for example.</param>
        /// <param name="rateReduction">A rate in % reduction that has apply to operation</param>
        /// <param name="rateDiscount">A rate in % discount that has apply to operation</param>
        /// <param name="transportAmount">An amount of transport of articles</param>
        /// <returns></returns>
        public static ExtraPrice ExtraPrices(double grossAmount, double rateReduction, double rateDiscount, double transportAmount, double vatRate)
        {
            ExtraPrice extra = new ExtraPrice();
            extra.ReductionAmount = Math.Round((rateReduction / 100) * grossAmount);
            extra.NetCom = Math.Round(grossAmount - extra.ReductionAmount);
            extra.DiscountAmount = Math.Round((rateDiscount / 100) * extra.NetCom);
            extra.NetFinan = Math.Round(extra.NetCom - extra.DiscountAmount);
            extra.TotalHT = Math.Round(extra.NetFinan + transportAmount);
            extra.TVAAmount = Math.Round(extra.TotalHT * (vatRate/100));
            extra.TotalTTC = Math.Round(extra.TotalHT + extra.TVAAmount);
            extra.TransportTTC = Math.Round(transportAmount + transportAmount * (vatRate / 100)); 
            return extra;
        }

    }
    public struct ExtraPrice
    {
        public double ReductionAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double NetCom { get; set; }
        public double NetFinan { get; set; }
        public double TotalHT { get; set; }
        public double TVAAmount { get; set; }
        public double TotalTTC { get; set; }
        public double TransportTTC { get; set; }
    }
}
