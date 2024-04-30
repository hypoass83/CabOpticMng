using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using Ext.Net;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using FatSod.Budget.Entities;

namespace FatSodDental.UI.Tools
{
    public static partial class LoadComponent
    {
        public static List<ListItem> BudgetLines(int fiscalYear)
        {
            //get
            //{
                context = new EFDbContext();
                List<ListItem> budgetModel = new List<ListItem>();

                var query =
                from bl in context.BudgetLines
                where !(from ba in context.BudgetAllocateds
                        where ba.FiscalYearID == fiscalYear
                        select ba.BudgetLineID)
                       .Contains(bl.BudgetLineID)
                select bl;

                //recuperation de la liste des allocated budget pr l'annee
                //List<int> lstBudAllocated = context.BudgetAllocateds.Where(ba=>ba.FiscalYearID==fiscalYear).Select(bl =>bl.BudgetLineID).ToList();
                
                //il fo retourner la liste des lignes qui nont pas ete programmer pr l'annee en cours
                foreach (var dL in query) // (BudgetLine dL in context.BudgetLines.Where(b => !lstBudAllocated.Contains(b.BudgetLineID)).ToList())
                {
                    budgetModel.Add(new ListItem(dL.BudgetLineLabel, dL.BudgetLineID));
                }
                return budgetModel;
            //}
        }
        public static List<ListItem> FiscalYears
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> budgetModel = new List<ListItem>();
                foreach (FiscalYear dL in context.FiscalYears.Where(t=>t.FiscalYearStatus).ToArray())
                {
                    budgetModel.Add(new ListItem(dL.FiscalYearLabel, dL.FiscalYearID));
                }
                return budgetModel;
            }
        }
        public static List<ListItem> BudgetAllocateds
        {
            get
            {
                context = new EFDbContext();
                List<ListItem> budgetModel = new List<ListItem>();
                //int year = DateTime.Today.Year;
                foreach (BudgetAllocated dL in context.BudgetAllocateds.Where(q=>q.FiscalYear.FiscalYearStatus).ToArray())
                {
                    budgetModel.Add(new ListItem(dL.BudgetLine.BudgetLineLabel, dL.BudgetAllocatedID));
                }
                return budgetModel;
            }
        }
        //public static Company Company(int userID)
        //{
        //    get
        //    {
        //        context = new EFDbContext();
        //        context.co

        //    }
        //}
    }
}