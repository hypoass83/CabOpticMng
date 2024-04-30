using FatSod.Report.WrapReports;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ISale : IRepositorySupply<Sale>
    {
        bool DeleteStockInsureReserve(int SaleID,  int SessionGlobalPersonID, DateTime BDDateOperation, int BranchID);
        Sale SaveChanges(Sale sale, String HeureVente, int UserConect, bool isCommand, bool isOtherSale);
        //bool SaveChanges(CustomerOrder custord, String HeureVente, int UserConect, DateTime ServerDate);
        bool ValidateBorderoDepotFacture(List<BorderoItems> custord, int BorderoDepotID, string HeureVente, int BranchID, int UserConect, DateTime ServerDate, int PaymentMethodID, string Reference);

        CumulSaleAndBill ValidePostToSpecialOrder(CumulSaleAndBill sale,  int UserConect);
        CumulSaleAndBill ValideReceiveSpecialOrder(CumulSaleAndBill sale, int UserConect);
        CumulSaleAndBill ValideDeliverSpecialOrder(CumulSaleAndBill sale, int UserConect);

        //Double SaleTotalPriceAdvance(Sale sale);
        //Sale PersistSale(Sale sale, int UserConect, bool isCommand);

       // Sale PersistSaleBill(Sale sale, int UserConect, int AssureurID);

        bool SaleDeleteDoubleEntry(int SaleID, int UserConect, DateTime ServerDate);
        /// <summary>
        /// Retourne le montant total des achats  du clients avant la date
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="BeginDate"></param>
        /// <returns></returns>
        double TotalAchatBefore(Customer customer, DateTime BeginDate);
        /// <summary>
        /// Retourne le montant total des achats  du clients pendant la Periode
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="BeginDate"></param>
        /// <returns></returns>
        double TotalAchatPeriode(Customer customer, DateTime BeginDate,DateTime EndDate);
        /// <summary>
        /// Retourne le montant total des achats  du clients
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="BeginDate"></param>
        /// <returns></returns>
        double TotalAchat(Customer customer);
        Lens PersistCustomerOrderLine(SaleLine saleLine);
        Lens PersistCustomerOrderLine(CumulSaleAndBillLine saleLine);
        OrderLens CreateOrderLens(OrderLens currentProduct);

        //bordero de depot
        int SaveBorderoDepotFacture(string heureVente, int BranchID, int AssuranceID, DateTime BeginDate, DateTime EndDate, string CodeBordero, List<int> rows_ID, string CompanyID, int LieuxdeDepotBorderoID, DateTime DateOperation, int UserConnected);
        bool DeleteCancelBordero(int BranchID, int BorderoDepotID, DateTime DateOperation, int UserConnected);

        void updateCashCustomerValue(int saledId, double TotalTTC);
        void updateInsuredCustomerValue(int customerOrderId);
    }
}
