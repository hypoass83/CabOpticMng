using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
	public interface ITillAdjust : IRepositorySupply<TillAdjust>
	{
		/// <summary>
		/// methode permettant d'ajuster la caisse
		/// </summary>
		/// <param name="tillAdjust"></param>
		/// <returns></returns>
        bool SaveTillAdjust(TillAdjust tillAdjust, int UserConect, int BranchID);

	}
}
