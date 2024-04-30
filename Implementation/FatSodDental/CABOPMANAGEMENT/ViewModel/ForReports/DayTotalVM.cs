using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CABOPMANAGEMENT.ViewModel
{
    public class DayTotalVM
    {
        public int Day { get; set; }
        public double Total { get; set; }
    }
    public class SalesVM
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }
        public List<DayTotalVM> Days { get; set; }
    }
}
