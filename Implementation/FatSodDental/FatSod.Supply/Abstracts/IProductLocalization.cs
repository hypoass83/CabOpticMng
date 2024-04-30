using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;

namespace FatSod.Supply.Abstracts
{
    public interface IProductLocalization : IRepositorySupply<ProductLocalization>
    {
        bool IsPLExist(ProductLocalization pl);
        bool StockInput(ProductLocalization pl, double quantity, double ProductPrice, DateTime Serverdate, int? UserConnect);
        bool StockOutput(ProductLocalization pl, double quantity, double ProductPrice, DateTime Serverdate, int? UserConnect);
        //bool UpdateProductLocalization(ProductLocalization pl);
        bool UpdateProductLocalization(ProductLocalization pl, DateTime Serverdate, double newQty);

        bool InitialiseStock(int CategoryID, int Stores, DateTime Serverdate, int UserConect, int BranchID);

        bool UpdateProductLocalizationAddedQty(ProductLocalization pl, DateTime Serverdate, double newQty);

        int[] GetAllStore(Product product);
        void DeleteAllStore(int productID);
        void CreateStore(Product product, int[] ids);
        void DeleteAllStore(int productID, int[] ids);
        bool checkQtyInStock(int productID, int localizationID, double entryQty);
        bool checkQtyInStock(int productID, int localizationID, double entryQty, string NumeroSerie,  string Marque);

        bool ValideStockOutPut(CumulSaleAndBill currentSale, String HeureOperation, int UserConnect, int RELineID, int LELineID);
        bool ValideDeliverDesk(CumulSaleAndBill currentSale, String HeureOperation, int UserConnect);
        bool ReceiveOrder(CumulSaleAndBill currentSale, String HeureOperation, int UserConnect);
        bool ValidateLensMounting(CumulSaleAndBill currentSale, String HeureOperation, int UserConnect);
        void UpdateStockFields(SpecialLensModel slm, CumulSaleAndBill currentSale, bool IsStockOutPutITF);
        bool OrderLenses(CumulSaleAndBill currentSale, int UserConnect, StockType stockType);
    }
}
