﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CABOPMANAGEMENT.ViewModel
{
    public class MonthTotalVM
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double GrandTotal { get; set; }
    }

    public class YearlyReportVM
    {
        public List<MonthTotalVM> MonthTotals { get; set; }
    }
}