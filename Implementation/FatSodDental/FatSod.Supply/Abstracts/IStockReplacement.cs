using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IStockReplacement : IRepositorySupply<StockReplacement>
    {
        StockReplacement DoStockReplacement(StockReplacement StockReplacement,int UserConnect);
        StockReplacement ValidateStockReplacement(StockReplacement StockReplacement);
        bool CancelStockReplacement(int StockReplacementID);
        StockReplacement UpdateStockReplacement(StockReplacement StockReplacement);
    }
}
