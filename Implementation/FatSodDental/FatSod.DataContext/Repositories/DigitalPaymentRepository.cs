using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSod.Security.Entities;
using System.Data.Entity;
using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using AutoMapper;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
	public class DigitalPaymentRepository : RepositorySupply<DigitalPaymentMethod>, IDigitalPayment
	{
		
	}
}
