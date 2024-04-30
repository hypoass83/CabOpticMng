using FatSod.Security.Entities;
using System;
using System.Collections.Generic;

namespace FatSod.Supply.Abstracts
{
    public interface IOpenAndCloseBusDay 
    {
        bool OpenBusDay(DateTime BDDateOperation, Branch branch);
        bool OpenBusDay(DateTime BDDateOperation, List<Branch> branches);
        bool CloseBusDay(Branch branch);
        bool CloseBusDay(List<Branch> branches);
        bool Backup(Branch branch);
        bool Backup(List<Branch> branches);
        /// <summary>
        /// Cette méthode permet de vérifier le bon état de la caisse avant la fermeture de la journée
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        bool TillCheck(Branch branch);
        bool TillCheck(List<Branch> branches);
        /// <summary>
        /// cette méthode permet de vérifier si la partie double pour toutes les écritures comptables de la journée a été respectée avant la fermeture de la journée
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        bool AccountingEntryChecking(Branch branch);
        bool AccountingEntryChecking(List<Branch> branches);
    }
}
