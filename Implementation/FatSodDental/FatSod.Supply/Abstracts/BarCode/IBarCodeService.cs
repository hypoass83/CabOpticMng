using FatSod.Supply.Entities;

namespace FatSod.Supply.Abstracts.BarCode
{
    public interface IBarCodeService
    {
        /// <summary>
        /// Recupere le barcode de la table ProductLocalisation et le si celui-ci n existe pas
        /// </summary>
        /// <param name="locationId">Stock dans lequel le produit dont le code barre generer sera mis</param>
        /// <param name="productId">Produit dont le code barre sera generer</param>
        /// <returns>chaine de caractere representant le code barre</returns>
        string GetBarCode(BarCodePayload payload);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>chaine de caractere representant le code barre qui va apparaitre sur le produit</returns>
        string GetPayload(BarCodePayload payload);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns>information contenu dans le code barre</returns>
        BarCodePayload GetPayload(string barCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns>ProductLocalisation ayant le code barre</returns>
        ProductLocalization GetProductLocalization(string barCode);



    }
}
