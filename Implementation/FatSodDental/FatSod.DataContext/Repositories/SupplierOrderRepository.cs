using FatSod.DataContext.Concrete;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;


namespace FatSod.DataContext.Repositories
{
    public class SupplierOrderRepository : RepositorySupply<SupplierOrder>, ISupplierOrder
    {
        public SupplierOrderRepository(EFDbContext ctxt):base(ctxt)
        {
            context = ctxt;
        }

        public SupplierOrder CreateSupplierOrder(SupplierOrder supplierOrder)
        {
            //Begin of transaction
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                    SaveSupplierOrder(supplierOrder);
                        //transaction.Commit();
                        ts.Complete();
                    }



                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database

                    //transaction.Rollback();

                    throw new Exception("Une erreur s'est produite lors de la création de la commande car : " + e.Message);
                }
                return supplierOrder;

            }

        public SupplierOrder SaveSupplierOrder(SupplierOrder supplierOrder)
        {

            SupplierOrder supplierOrderToSave = new SupplierOrder()
            {
                BranchID = supplierOrder.BranchID,
                VatRate = (supplierOrder.VatRate * 100),
                SupplierID = supplierOrder.SupplierID,
                SupplierOrderDate = supplierOrder.SupplierOrderDate,
                RateDiscount = supplierOrder.RateDiscount,
                RateReduction = supplierOrder.RateReduction,
                Transport = supplierOrder.Transport,
                SupplierOrderReference = supplierOrder.SupplierOrderReference,
                DeviseID = supplierOrder.DeviseID,
                IsDelivered = false

            };
            supplierOrderToSave = context.SupplierOrders.Add(supplierOrderToSave);
            context.SaveChanges();
            supplierOrder.SupplierOrderLines.ToList().ForEach(sol =>
            {
                SupplierOrderLine solSave = new SupplierOrderLine()
                {
                    SupplierOrderID = supplierOrderToSave.SupplierOrderID,
                    ProductID = sol.ProductID,
                    LineUnitPrice = sol.LineUnitPrice,
                    LineQuantity = sol.LineQuantity,
                    LocalizationID = sol.LocalizationID                    
                };
                context.SupplierOrderLines.Add(solSave);
            });
            //mise a jour du cpteur du transact number
            TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == "PURC");
            if (trn != null)
            {
                //persistance du compteur de l'objet TransactNumber
                trn.Counter = trn.Counter + 1;
            }
            context.SaveChanges();
            return supplierOrder;
        }

        public bool DeleteSupplierOrder(int supplierOrderID)
        {
            bool res = false ;
            //Begin of transaction
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                    RemoveSupplierOrder(supplierOrderID);
                        //transaction.Commit();
                        res = true;
                        ts.Complete();
                    }

                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    res = false;
                    throw new Exception("Une erreur s'est produite lors de la suppression de l'achat car : " + e.Message);
                }
                return res;

        }

        public bool RemoveSupplierOrder(int SupplierOrderID)
        {
            bool res = false;


            List<SupplierOrderLine> supplierOrderLines = this.context.SupplierOrderLines.Where(pl => pl.SupplierOrderID == SupplierOrderID).ToList();

            //suppression des lignes d'achat
            this.context.SupplierOrderLines.RemoveRange(supplierOrderLines);
            this.context.SaveChanges();
            //suppression de l'achat
            this.context.SupplierOrders.Remove(this.context.SupplierOrders.SingleOrDefault(so => so.SupplierOrderID == SupplierOrderID));
            this.context.SaveChanges();
            res = true;
            return res;
        }

        public SupplierOrder UpdateSupplierOrder(SupplierOrder supplierOrder)
        {
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                    this.RemoveSupplierOrder(supplierOrder.SupplierOrderID);
                    this.SaveSupplierOrder(supplierOrder);
                        //transaction.Commit();
                        ts.Complete();
                    }

                }
                catch (Exception e)
                {
                    //If an errors occurs, we cancel all changes in database
                    //transaction.Rollback();
                    throw new Exception("Une erreur s'est produite lors de la mise à jour de l'achat " + e.Message);
                }
            //}


            return supplierOrder;
        }



    }
}
