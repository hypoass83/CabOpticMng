using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FatSodDental.UI.Areas.Supply.ViewModel
{
    public class LensRangeModel
    {
        private Category category;
        private List<Category> categories;

        public LensRangeModel()
        {
        }
        
        public int LensCategoryID { get; set; }

        public int[] Stores { get; set; }

        public string SphMin { get; set; }
        public string SphMax { get; set; }

        public string CylMin { get; set; }
        public string CylMax { get; set; }
        
        public string AddMin { get; set; }
        public string AddMax { get; set; }

    }
}