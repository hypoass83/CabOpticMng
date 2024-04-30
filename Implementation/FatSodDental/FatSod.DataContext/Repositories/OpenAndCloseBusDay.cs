using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using System.Data.Entity;
using FatSod.Supply.Abstracts;
using FatSod.Security.Entities;
using System;

namespace FatSod.DataContext.Repositories
{
    public class OpenAndCloseBusDay : IOpenAndCloseBusDay
    {
        protected EFDbContext context;
        private bool _disposed;

        public OpenAndCloseBusDay()
        {
            context = new EFDbContext();
        }
        public bool OpenBusDay(DateTime BDDateOperation, Branch branch) 
        {
            bool res = false;
            res = true;
            return res;
        }
        public bool OpenBusDay(DateTime BDDateOperation, List<Branch> branches)
        {
            bool res = false;
            res = true;
            return res;
        }
        public bool CloseBusDay(Branch branch)
        {
            bool res = false;
            res = true;
            return res;
        }
        public bool CloseBusDay(List<Branch> branches)
        {
            bool res = false;
            res = true;
            return res;
        }
        public bool Backup(Branch branch)
        {
            bool res = false;
            res = true;
            return res;
        }
        public bool Backup(List<Branch> branches)
        {
            bool res = false;
            res = true;
            return res;
        }
        /// <summary>
        /// Cette méthode permet de vérifier le bon état de la caisse avant la fermeture de la journée
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        public bool TillCheck(Branch branch)
        {
            bool res = false;
            res = true;
            return res;
        }
        public bool TillCheck(List<Branch> branches)
        {
            bool res = false;
            res = true;
            return res;
        }
        /// <summary>
        /// cette méthode permet de vérifier si la partie double pour toutes les écritures comptables de la journée a été respectée avant la fermeture de la journée
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        public bool AccountingEntryChecking(Branch branch)
        {
            bool res = false;
            res = true;
            return res;
        }
        public bool AccountingEntryChecking(List<Branch> branches)
        {
            bool res = false;
            res = true;
            return res;
        }
        

        private void save()
        {
            context.SaveChanges();
        }
    }
}
