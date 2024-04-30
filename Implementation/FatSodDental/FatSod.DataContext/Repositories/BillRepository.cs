
﻿
﻿using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastSod.Utilities.Util;
using AutoMapper;
using FatSod.DataContext.Concrete;
using System.Transactions;
using FatSod.Security.Abstracts;

namespace FatSod.DataContext.Repositories
{
    public class BillRepository : RepositorySupply<Bill>, IBill
    {
        public BillRepository()
        {
        }

        public BillRepository(EFDbContext ctx)
            : base(ctx)
        {
            this.context = ctx;
        }


        public Bill PersistBill(Bill bill, int UserConect,int CurrentBranchID)
        {
            try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                    bool res = false;
                    
                    int BillID = 0;
                    
                    //persistance du bill
                    Bill BillToSave = new Bill();

                    //create a Map
                    Mapper.CreateMap<Bill, Bill>();
                    //use Map
                    BillToSave = Mapper.Map<Bill>(bill);
                    BillToSave.BillDetails = null;

                    BillToSave = context.Bills.Add(BillToSave);
                    context.SaveChanges();
                    BillID = BillToSave.BillID;

                    //persistance du detail
                    foreach (BillDetail billLine in bill.BillDetails)
                    {
                        BillDetail BillDetailToSave = new BillDetail()
                        {
                            ProductID = billLine.ProductID,
                            BillID = BillID,
                            LineQuantity = billLine.LineQuantity,
                            LineUnitPrice = billLine.LineUnitPrice,
                            DateVente = billLine.DateVente,
                            DateCommande = billLine.DateCommande,
                            NumeroCommande = billLine.NumeroCommande,
                            SaleID = billLine.SaleID
                        };
                        context.BillDetails.Add(BillDetailToSave);
                        context.SaveChanges();
                    }

                    //metre a jour le transaction number
                    int compteur = Convert.ToInt32(bill.BillNumber.Substring(12));
                    ITransactNumber trnNumber = new TransactNumberRepository(context);
                    res = trnNumber.saveTransactNumber("FACT", compteur);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la mise a jour du compteur du transact number ");
                    }

                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(UserConect, "SUCCESS", "Bill-REFERENCE " + bill.BillNumber + " FOR CUSTOMER NUMBER " + bill.CustomerID, "PersistBill", bill.BillDate, CurrentBranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    bill.BillID = BillID;
                    ts.Complete();
                    }
                }
            catch (Exception e)
            {
                //If an errors occurs, we cancel all changes in database
                throw new Exception("Une erreur s'est produite lors de la vente : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return bill;   
        }

        public bool DeleteBill(int BillID, int UserConect, int CurrentBranchID)
        {
            bool res = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //BillLines
                    context.BillDetails.Where(a => a.BillID == BillID).ToList().ForEach(ol =>
                    {
                        context.BillDetails.Remove(ol);
                        context.SaveChanges();
                    });
                    //Bills
                    Bill Bill = context.Bills.Where(a => a.BillID == BillID).FirstOrDefault();
                    context.Bills.Remove(Bill);
                    context.SaveChanges();
                    //});
                    //EcritureSneack
                    IMouchar opSneak = new MoucharRepository(context);
                    res = opSneak.InsertOperation(UserConect, "SUCCESS", "DELETE Bill-REFERENCE " + Bill.BillNumber + " FOR CUSTOMER " + Bill.CustomerID, "DeleteBill", Bill.BillDate, CurrentBranchID);
                    if (!res)
                    {
                        throw new Exception("Une erreur s'est produite lors de la journalisation ");
                    }
                    res = true;
                    ts.Complete();
                }
            }
            catch (Exception e)
            {
                res = false;
                
                throw new Exception("Error while delete Bills : " + "e.Message = " + e.Message + "e.InnerException = " + e.InnerException + "e.StackTrace = " + e.StackTrace);
            }
            return res;
        }
    }
}

