using FatSod.Supply.Entities;
using System.Collections.Generic;

namespace CABOPMANAGEMENT.Areas.Supply.ViewModel
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

        public List<Product> Products { get; set; }
        public List<Lens> Lenses { get; set; }
        //public List<Category> Categories { get; set; }
        //public List<Account> AccountNumbers { get; set; }

    }
}