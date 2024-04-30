using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class GenericProduct : Product
    {
        public string marque { get; set; }
        public string reference { get; set; }
    }
}
