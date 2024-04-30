using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FatSodDental.UI.Areas.Sale.Models
{
    public class PostToSupplierModel
    {
        public SelectedRowCollection SelectedOrders { get; set; }
        public List<object> AllOrders { get; set; }
				public DateTime PostedDate { get; set; }
    }
}