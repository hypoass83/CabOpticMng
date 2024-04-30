using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface ILens : IRepositorySupply<Lens>
    {
        /// <summary>
        /// Permet de Générer les verre dont les numéros sont dans un intervalle de numéro : 
        /// </summary>
        /// <param name="lensCategoryCode">Code de la catégorie des verres à crééer</param>
        /// <param name="sphericalValueRange">intervalle des valeurs sphériques pour lesquelles on doit crééer les verres</param>
        /// <param name="cylindricalValueRange">intervalle des valeurs cylindrique pour lesquelles on doit crééer les verres</param>
        /// <param name="additionValueRange">intervalle des valeurs addition pour lesquelles on doit crééer les verres</param>
        /// <returns>Liste des verres créés</returns>
        /// 
        /// on peut avoir 8 cas de figure pour le couple (SphRange, CylRange, AddRange) qui sont : 
        /// 1 - (0 0 0)
        /// 2 - (0 0 1)
        /// 3 - (0 1 0)
        /// 4 - (0 1 1)
        /// 5 - (1 0 0)
        /// 6 - (1 0 1)
        /// 7 - (1 1 0)
        /// 8 - (1 1 1)
        ///
        /// 1 = l'intervalle contient au moins un élément(numéro)
        /// 0 = l'intervalle ne contient pas d'élément(numéro)
        /// 
        /// Il n'y a que les 6 dernières couples qui sont possibles
        List<Lens> CreateLens(string lensCategoryCode, LensNumberRange sphericalValueRange, LensNumberRange cylindricalValueRange, LensNumberRange additionValueRange, int[] storeIds);
        /// <summary>
        /// Permet de crééer un verre si et seulement s'il n'existe pas encore
        /// </summary>
        /// <param name="lensCategoryCode">Code de la cat"gorie du verre </param>
        /// <param name="sphericalValue">valeur sphérique du verre</param>
        /// <param name="cylindricalValue">valeur cylindrique du verre</param>
        /// <param name="additionValue">valeur de l'addition du verre</param>
        /// <returns>Verre qui a été créé</returns>
        Lens CreateLens(string lensCategoryCode, string sphericalValue, string cylindricalValue, string additionValue, int[] storeIds);
        /// <summary>
        /// Permet de crééer un verre si et seulement s'il n'existe pas encore
        /// </summary>
        /// <param name="lensCategoryCode">Code de la cat"gorie du verre </param>
        /// <param name="number">numéro du verre</param>
        /// <returns>Verre qui a été créé</returns>
        Lens CreateLens(string lensCategoryCode, LensNumber number, int[] storeIds);

        /// <summary>
        /// Cette méthode retourne la liste des numéros de verre qui ont déjà été utilisée d'une catégorie.
        /// </summary>
        /// <param name="Category">Catégorie du verre dont on veut lma liste des numéros déjà attribuées</param>
        /// <returns>Liste des numéros déjà attribués de cette catégorie</returns>
        List<LensNumber> GetAllAvaillableNumbers(LensCategory Category);

        /// <summary>
        /// Cette méthode retourne la liste des numéros de verre qui ont déjà été utilisée d'une catégorie.
        /// </summary>
        /// <param name="Category">Catégorie du verre dont on veut lma liste des numéros déjà attribuées</param>
        /// <returns>Liste des numéros déjà attribués de cette catégorie</returns>
        List<LensNumber> GetAllAvaillableNumbers(int LensCategoryID);

        
    }
}
