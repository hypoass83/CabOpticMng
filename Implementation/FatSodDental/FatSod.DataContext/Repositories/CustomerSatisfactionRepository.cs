using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.DataContext.Repositories
{
   public class CustomerSatisfactionRepository : RepositorySupply<CustomerSatisfaction>, ICustomerSatisfaction
    {
        private IBusinessDay bdRepo;
        public CustomerSatisfactionRepository(EFDbContext ctext)
        {
            this.context = ctext;
        }

        public CustomerSatisfactionRepository()
			: base()
		{
            bdRepo = new BusinessDayRepository();
        }
    }
}
