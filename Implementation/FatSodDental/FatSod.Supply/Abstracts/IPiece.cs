using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IPiece : IRepositorySupply<Piece>
    {
        /// <summary>
        /// saisi simple de piece comptable (debiter un compte pour le credit d'un autre)
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        bool EcriturePieceSingleEntry(SingleEntry singleEntry);
        /// <summary>
        /// saisi multiple de piece comptable (debiter un ou plusieurs compte(s) pour le debit d'un 
        /// ou de plusieurs compte(s))
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        bool EcriturePieceMultipleEntry(MultipleEntries multipleEntries);
        bool EcritureDebitPiece(object o);
        bool EcritureCreditPiece(object o);
        bool UpdatePiece(Piece piece, long pieceID);
    }
}
