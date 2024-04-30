using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.DataContext.Concrete;
using FatSod.DataContext.Initializer;
using System.Data.Entity;
using System.Transactions;

namespace FatSod.DataContext.Repositories
{
    public class PieceRepository : RepositorySupply<Piece>, IPiece
    {
        public bool EcriturePieceSingleEntry(SingleEntry singleEntry)
        {
            //Begin of transaction
            bool res = false;
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                    //debit du compte
                    res = EcritureDebitPiece(singleEntry);
                    if (!res)
                    {
                            //transaction.Rollback();
                        throw new Exception("System Error : Error while Debit Account Contact your Provider");
                    }
                    //Credit du compte
                    res = EcritureCreditPiece(singleEntry);
                    if (!res)
                    {
                            //transaction.Rollback();
                        throw new Exception("System Error : Error while Credit Account Contact your Provider");
                    }
                        //transaction.Commit();
                        ts.Complete();
                }

                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    throw new Exception("System Error : " + e.Message + "Contact your Provider");
                }
                return res;

            }
        public bool EcriturePieceMultipleEntry(MultipleEntries multipleEntries)
        {
            //Begin of transaction
            bool res = false;
                try
            {
                    using (TransactionScope ts = new TransactionScope())
                {

                    //debit du compte
                    if (multipleEntries.AccountSens == "DB")
                    {
                        res = EcritureDebitPiece(multipleEntries);
                        if (!res)
                        {
                                //transaction.Rollback();
                            throw new Exception("System Error : Error while Debit Account Contact your Provider");
                        }
                    }

                    //Credit du compte
                    if (multipleEntries.AccountSens == "CR")
                    {
                        res = EcritureCreditPiece(multipleEntries);
                        if (!res)
                        {
                                //transaction.Rollback();
                            throw new Exception("System Error : Error while Credit Account Contact your Provider");
                        }
                        }
                        //transaction.Commit();
                        ts.Complete();
                    }
                    

                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    throw new Exception("System Error : " + e.Message + "Contact your Provider");
                }
                return res;

        }
        //ecriture debit
        public bool EcritureDebitPiece(object o)
        {
            bool res = false;

            if (o is SingleEntry)
            {
                SingleEntry se = (SingleEntry)o;
                Piece piece = new Piece();
                piece.AccountID = se.AccountDebitID;
                piece.BranchID = se.BranchID;
                piece.CodeTransaction = se.CodeTransaction;
                piece.Credit = 0;
                piece.DateOperation = se.DateOperation;
                piece.Debit = se.Amount;
                piece.Description = se.Description;
                piece.DeviseID = se.DeviseID;
                piece.OperationID = se.OperationID;
                piece.Reference = se.Reference;
                context.Pieces.Add(piece);
            }
            else if (o is MultipleEntries)
            {
                MultipleEntries me = (MultipleEntries)o;
                Piece piece = new Piece();
                piece.AccountID = me.AccountID;
                piece.BranchID = me.BranchID;
                piece.CodeTransaction = me.CodeTransaction;
                piece.Credit = 0;
                piece.DateOperation = me.DateOperation;
                piece.Debit = me.Amount;
                piece.Description = me.Description;
                piece.DeviseID = me.DeviseID;
                piece.OperationID = me.OperationID;
                piece.Reference = me.Reference;
                context.Pieces.Add(piece);
            }
            context.SaveChanges();
            res = true;
            return res;
        }

        //ecriture credit
        public bool EcritureCreditPiece(object o)
        {
            bool res = false;

            if (o is SingleEntry)
            {
                SingleEntry se = (SingleEntry)o;
                Piece piece = new Piece();
                piece.AccountID = se.AccountCreditID;
                piece.BranchID = se.BranchID;
                piece.CodeTransaction = se.CodeTransaction;
                piece.Credit = se.Amount;
                piece.DateOperation = se.DateOperation;
                piece.Debit = 0;
                piece.Description = se.Description;
                piece.DeviseID = se.DeviseID;
                piece.OperationID = se.OperationID;
                piece.Reference = se.Reference;
                context.Pieces.Add(piece);
            }
            else if (o is MultipleEntries)
            {
                MultipleEntries me = (MultipleEntries)o;
                Piece piece = new Piece();
                piece.AccountID = me.AccountID;
                piece.BranchID = me.BranchID;
                piece.CodeTransaction = me.CodeTransaction;
                piece.Credit = me.Amount;
                piece.DateOperation = me.DateOperation;
                piece.Debit = 0;
                piece.Description = me.Description;
                piece.DeviseID = me.DeviseID;
                piece.OperationID = me.OperationID;
                piece.Reference = me.Reference;
                context.Pieces.Add(piece);
            }
            context.SaveChanges();
            res = true;
            return res;
        }

        public bool UpdatePiece(Piece piece, long pieceID)
        {
            bool res = false;
            try
            {
                //recuperation de la ligne a mettre  jour
                Piece existing = context.Pieces.Find(pieceID);
                if (existing != null)
                {
                    this.context.Pieces.Attach(existing);
                    context.Entry(existing).State = EntityState.Modified;
                    this.context.SaveChanges();
                    res = true;
                }
                
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception("System Error : " + e.Message + "Contact your Provider");
            }
            return res;
        }
    }
}
