using System;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class DetailLens
    {
        public int DetailLensID { get; set; }
        public string LensName { get; set; }

        public string LensDesignation { get; set; }
        public string LensAmount { get; set; }
        public string LensQty { get; set; }
        public string LensUnitPrice { get; set; }
        
    }
}
