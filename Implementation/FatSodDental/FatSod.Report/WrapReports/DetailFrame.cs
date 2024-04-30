using System;

namespace FatSod.Report.WrapReports
{
    [Serializable]
    public class DetailFrame
    {
        public int DetailFrameID { get; set; }
        public string FrameName { get; set; }
        public string FrameQuantity { get; set; }
        public string FrameUnitPrice { get; set; }

        public string FrameAmount { get; set; }
        public string Marque { get; set; }
        public string Reference { get; set; }
        public string Materiere { get; set; }
    }
}
