using Ext.Net;
using System;
using System.Collections.Generic;

namespace FatSodDental.UI.Areas.Supply.ViewModel
{
    public class LocalizationModel1
    {
        //ProductLocalization
        public int ProductLocalizationID { get; set; }
        public int ProductID { get; set; }
        public string Product { get; set; }
        public DateTime ProductLocalizationDate { get; set; }
        public string Account { get; set; }
        public int AccountID { get; set; }
        public int QuarterID { get; set; }
        public string Quarter { get; set; }

        //
        public int CountryID { get; set; }
        public int RegionID { get; set; }
        public int TownID { get; set; }


        //Localization
        public int LocalizationID { get; set; }
        public string LocalizationCode { get; set; }
        public string LocalizationName { get; set; }
        public int BranchID { get; set; }
        public string Branch { get; set; }
       
        public List<LocalizationModel> Localizations { get; set; }
        public List<ListItem> AccountNumbers { get; set; }
        public List<ListItem> Branches { get; set; }
        public List<ListItem> Categories { get; set; }
        public List<ListItem> Countries { get; set; }
    }
}