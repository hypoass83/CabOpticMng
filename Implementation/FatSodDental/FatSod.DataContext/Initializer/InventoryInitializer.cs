using System.Data.Entity;
using FatSod.DataContext.Concrete;

namespace FatSod.DataContext.Initializer
{
    internal class InventoryInitializer : DropCreateDatabaseIfModelChanges<EFDbContext>
    {
        protected override void Seed(EFDbContext context)
        {
            /*=========================== Modules initialization ================================*/
            Parameters.Modules.ForEach(m => context.Modules.Add(m));
            /*=========================== Menus initialization ==================================*/
            Parameters.Menus.ForEach(mp => context.Menus.Add(mp));
            /*=========================== SubMenus initialization ===============================*/
            Parameters.SubMenus.ForEach(sb => context.SubMenus.Add(sb));
            //Regions
            Parameters.Regions.ForEach(r => context.Regions.Add(r));
            //towns list
            Parameters.Towns.ForEach(t => context.Towns.Add(t));
            //quarters
            Parameters.Quarters.ForEach(q => context.Quarters.Add(q));
            /*========================= Branch initialization===================================*/
            Parameters.Branchs.ForEach(b => context.Branches.Add(b));
            /*========================= Business Day initialization===================================*/
            Parameters.BusinessDays.ForEach(busDay => context.BusinessDays.Add(busDay));
            /*========================= Closing Day Task initialization===================================*/
            Parameters.ClosingDayTasks.ForEach(cdt => context.ClosingDayTasks.Add(cdt));
            /*========================= Branch Closing Day Task initialization===================================*/
            Parameters.BranchClosingDayTasks.ForEach(bcdt => context.BranchClosingDayTasks.Add(bcdt));
            /*========================= UserBranch initialization===================================*/
            Parameters.UserBranches.ForEach(ub => context.UserBranches.Add(ub));
            /*========================= Profile initialization===================================*/
            Parameters.Profiles.ForEach(p => context.Profiles.Add(p));
            /*========================= Job Initialisation ======================================*/
            Parameters.Jobs.ForEach(j => context.Jobs.Add(j));
            /*========================= ProfileMenus initialization==============================*/
            Parameters.ActionMenuProfiles.ForEach(pM => context.ActionMenuProfiles.Add(pM));
            //Parameters.SuperAdminActionMenuProfiles.ForEach(pM => context.ActionMenuProfiles.Add(pM));
            /*========================= ProfileSubMenus initialization===========================*/
            Parameters.ActionSubMenuProfiles.ForEach(psM => context.ActionSubMenuProfiles.Add(psM));
            //Parameters.SuperAdminActionSubMenuProfiles.ForEach(psM => context.ActionSubMenuProfiles.Add(psM));
            ///*=========================== User Admin initialization =============================*/
            Parameters.Sexes.ForEach(s => context.Sexes.Add(s));
            Parameters.Users.ForEach(u => context.People.Add(u));
            /*============================ Others parameters ====================================*/
            //Class Account
            Parameters.ClassAccount.ForEach(c => context.ClassAccounts.Add(c));
            //AccountSection
            Parameters.AccountingSections.ForEach(a => context.AccountingSections.Add(a));
            //Création des collectives account pr les category
            Parameters.CollectifAccounts.ForEach(c => context.CollectifAccounts.Add(c));
            //Operation type
            Parameters.OperationType.ForEach(o => context.OperationTypes.Add(o));
            //Macro Operation
            //Parameters.MacroOperation.ForEach(o => context.MacroOperations.Add(o));
            //Reglement Type
            //Parameters.ReglementType.ForEach(r => context.ReglementTypes.Add(r));
            //Operation
            Parameters.Operation.ForEach(o => context.Operations.Add(o));
            //DEVISE
            Parameters.Devise.ForEach(d => context.Devises.Add(d));
            Parameters.UserConfigurations.ForEach(c => context.UserConfigurations.Add(c));
            //Création de la catégorie non Lenses non supprimable
            //Parameters.Categories.ForEach(c => context.Categories.Add(c));
            //Création des matières de verre
            //Parameters.LensMaterials.ForEach(c => context.LensMaterials.Add(c));
            //Création d'un traitement par défaut afin de résoudre le pb de la clé étrangère ne pouvant pas être nulle
            //Parameters.LensCoatings.ForEach(c => context.LensCoatings.Add(c));
            //Création d'une couleur par défaut afin de résoudre le pb de la clé étrangère ne pouvant pas être nulle
            //Parameters.LensColours.ForEach(c => context.LensColours.Add(c));
            //creation des lens category
            //Parameters.LensCategories.ForEach(l => context.LensCategories.Add(l));
            //creation des magasins
            //Parameters.Localizations.ForEach(lo => context.Localizations.Add(lo));
            //creation des lens SPHERICAL NUMBER
            //Parameters.SphericalVal.ForEach(sp => context.LensNumbers.Add(sp));
            //context.LensNumbers.AddRange(Parameters.SphericalVal);
            //creation des lens cylindric number
            //Parameters.CylindricalVal.ForEach(cy => context.LensNumbers.Add(cy));
            //context.LensNumbers.AddRange(Parameters.CylindricalVal);
            //creation des lens spheric add cylindric number
            //Parameters.SphericalValAddCylindricalVal.ForEach(spaddcy => context.LensNumbers.Add(spaddcy));
            //context.LensNumbers.AddRange(Parameters.SphericalValAddCylindricalVal);
            //creation d'annee fiscal
            Parameters.FiscalYear.ForEach(c => context.FiscalYears.Add(c));
            //Création de 20 intervalles pour la fixation des prix de verre
            //context.LensNumberRanges.AddRange(Parameters.LensNumberRanges);

            context.SaveChanges();
        } 
    }
}
