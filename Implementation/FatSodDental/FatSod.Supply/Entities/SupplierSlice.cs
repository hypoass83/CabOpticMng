﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class SupplierSlice : Slice
    {
        public int PurchaseID { get; set; }
        [ForeignKey("PurchaseID")]
        public virtual Purchase Purchase { get; set; }
    }
}
