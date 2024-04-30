using Ext.Net;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace FatSodDental.UI.Areas.Supply.ViewModel
{
    public class LocalizationModel
    {
        public int LocalizationID { get; set; }
        public string LocalizationCode { get; set; }
        public string LocalizationLabel { get; set; }
        public string LocalizationDescription { get; set; }
        public int QuarterID { get; set; }
        public string Quarter { get; set; }
        public int BranchID { get; set; }
        public string Branch { get; set; }

        //
        public int CountryID { get; set; }
        public int RegionID { get; set; }
        public int TownID { get; set; }

        public List<LocalizationModel> Localizations { get; set; }
        public List<ListItem> Countries { get; set; }
        public List<ListItem> Branches { get; set; }
    }
}
