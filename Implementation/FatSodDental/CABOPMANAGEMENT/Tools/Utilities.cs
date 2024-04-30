using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using System.Collections.Generic;

namespace CABOPMANAGEMENT.Tools
{
    public static class Utilities
    {
        public static List<object> PaymentMethodTypes()
        {
            List<object> BuyTypeList = new List<object>();
            //cash
            BuyTypeList.Add(new
            {
                ID = CodeValue.Accounting.DefaultCodeAccountingSection.CODECAIS,
                Name = Resources.CASH
            });
            //bank
            BuyTypeList.Add(new
            {
                ID = CodeValue.Accounting.DefaultCodeAccountingSection.CODEBANK,
                Name = Resources.BANK
            });

            //DIGITAL
            BuyTypeList.Add(new
            {
                ID = CodeValue.Accounting.DefaultCodeAccountingSection.DIGITAL_PAYMENT,
                Name = Resources.DigitalPayment
            });

            return BuyTypeList;
        }

    }

}