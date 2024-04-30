using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IInventoryDirectory : IRepositorySupply<InventoryDirectory>
    {

        //InventoryDirectory CloseInventoryDirectorySansTransaction(InventoryDirectory inventoryDirectory, int UserConect);

        InventoryDirectory CreateInventoryDirectory(InventoryDirectory inventoryDirectory, int UserConect);
        InventoryDirectory CloseInventoryDirectory(InventoryDirectory inventoryDirectory, int UserConect);
        InventoryDirectory UpdateInventoryDirectory(InventoryDirectory inventoryDirectory, int UserConect);
        bool DeleteInventoryDirectory(int InventoryDirectoryID);
        /// <summary>
        /// Cette méthode retourne la liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed.
        /// </summary>
        /// <param name="branch">Agence dans laquelle le dossier d'inventaire a été créée</param>
        /// <returns>liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed</returns>
        List<Product> LockedProducts(Branch branch);
        /// <summary>
        /// Cette méthode retourne la liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed.
        /// </summary>
        /// <param name="location">magasin dans lequel le dossier d'inventaire a été créée</param>
        /// <returns>liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed</returns>
        List<Product> LockedProducts(Localization location);
        /// <summary>
        /// Cette méthode retourne la liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed.
        /// </summary>
        /// <returns>liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed</returns>
        List<Product> LockedProducts();
        /// <summary>
        /// Cette méthode retourne la liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed.
        /// </summary>
        /// <returns>liste des produits qui sont dans un dossier d'inventaire ayant les statuts opened ou closed</returns>
        List<InventoryDirectoryLine> LockedInventoryDirectoryLines();

        ///create and close inventory directory
        ///
        InventoryDirectory CreateAndCloseInventoryDirectory(InventoryDirectory inventoryDirectory, int UserConect);
        InventoryDirectory SpecialOrderReceptionStockInPut(CumulSaleAndBill cumulSaleAndBill, int UserConect);
        void CreateReconciliation(InventoryReconciliation reconciliation, List<InventoryReconciliationLine> reconciliationLines);
    }
}
