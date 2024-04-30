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
    public class NoPurchaseRepository : RepositorySupply<NoPurchase>, INoPurchase
    {
        private IBusinessDay bdRepo;
        public NoPurchaseRepository(EFDbContext ctext)
        {
            this.context = ctext;
        }

        public NoPurchaseRepository()
			: base()
		{
            bdRepo = new BusinessDayRepository();
        }

        public void CustomerServiceReasonUpdate(NoPurchase noPurchase)
        {
            NoPurchase existingNoPurchase = context.NoPurchases.Find(noPurchase.NoPurchaseId);
            existingNoPurchase.CSOperationDate = noPurchase.CSOperationDate;
            existingNoPurchase.CustomerServiceReason = noPurchase.CustomerServiceReason;
            context.SaveChanges();
            return;
        }
    }
}
