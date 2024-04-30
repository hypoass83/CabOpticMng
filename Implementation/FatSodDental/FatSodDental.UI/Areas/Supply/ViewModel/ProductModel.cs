using Ext.Net;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FatSodDental.UI.Areas.Supply.ViewModel
{
    public class ProductModel
    {
        public int ProductID { get; set; }
        public int ProductSafetyStockQuantity { get; set; }
        public string ProductCode { get; set; }
        public string ProductLabel { get; set; }
        public string ProductDescription { get; set; }
        public string Account { get; set; }
        public string Category { get; set; }
        public int CategoryID { get; set; }        
        public int AccountID { get; set; }
        public List<object> Products { get; set; }
        public List<ListItem> Categories { get; set; }
        public List<ListItem> AccountNumbers { get; set; }

    }
}