using System;
using System.Collections.Generic;
using System.Linq;
using FatSod.DataContext.Concrete;
using FatSod.Security.Entities;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Supply.Entities;
using FatSod.Budget.Entities;

namespace CABOPMANAGEMENT.Tools
{
    public static partial class LoadComponent
    {
        public static List<BudgetLine> BudgetLines(int fiscalYear)
        {
            //get
            //{
                context = new EFDbContext();
                List<BudgetLine> budgetModel = new List<BudgetLine>();

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
                    budgetModel.Add(new BudgetLine { BudgetLineLabel = dL.BudgetLineLabel, BudgetLineID = dL.BudgetLineID });
                }
                return budgetModel;
            //}
        }
        public static List<FiscalYear> FiscalYears
        {
            get
            {
                context = new EFDbContext();
                List<FiscalYear> budgetModel = new List<FiscalYear>();
                foreach (FiscalYear dL in context.FiscalYears.Where(t=>t.FiscalYearStatus).ToArray())
                {
                    budgetModel.Add(new FiscalYear { FiscalYearLabel=dL.FiscalYearLabel, FiscalYearID = dL.FiscalYearID });
                }
                return budgetModel;
            }
        }
        public static List<BudgetAllocated> BudgetAllocateds
        {
            get
            {
                context = new EFDbContext();
                List<BudgetAllocated> budgetModel = new List<BudgetAllocated>();
                //int year = DateTime.Today.Year;
                foreach (BudgetAllocated dL in context.BudgetAllocateds.Where(q=>q.FiscalYear.FiscalYearStatus).ToArray())
                {
                    budgetModel.Add(new BudgetAllocated { BudgetLine = dL.BudgetLine, BudgetAllocatedID = dL.BudgetAllocatedID });
                }
                return budgetModel;
            }
        }
       
    }
}