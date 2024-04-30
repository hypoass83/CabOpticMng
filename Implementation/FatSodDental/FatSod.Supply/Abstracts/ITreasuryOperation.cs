using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
	public interface ITreasuryOperation : IRepositorySupply<TreasuryOperation>
	{
		/// <summary>
		/// methode permettant d'effectuer les operations de tresorerie entre caisse et bank
		/// </summary>
		/// <param name="TreasuryOperation"></param>
		/// <returns></returns>
		bool SaveTreasuryOperation(TreasuryOperation treasuryOperation, int UserConect, DateTime OperationDate, int BranchID);

	}
}
