using FatSod.Supply.Entities;
using System.Collections.Generic;

namespace CABOPMANAGEMENT.Areas.Supply.ViewModel
{
    public class CategoryModel
    {
        private Category category;
        private List<Category> categories;


        public CategoryModel()
        {
            category = new Category();
            categories = new List<Category>();
        }

        public CategoryModel(Category category, List<Category> categories)
        {
            this.Category = category;
            this.Categories = categories;
        }

        public Category Category
        {
            get { return category; }
            set { category = value; }
        }        
        public List<Category> Categories
        {
            get { return categories; }
            set { categories = value; }
        }
        


    }
}