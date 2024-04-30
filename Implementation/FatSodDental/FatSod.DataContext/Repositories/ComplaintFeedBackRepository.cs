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
    public class ComplaintFeedBackRepository : RepositorySupply<ComplaintFeedBack>, IComplaintFeedBack
    {
        private IBusinessDay bdRepo;
        public ComplaintFeedBackRepository(EFDbContext ctext)
        {
            this.context = ctext;
        }

        public ComplaintFeedBackRepository()
            : base()
        {
            bdRepo = new BusinessDayRepository();
        }
    }
}
