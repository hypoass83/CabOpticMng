using FatSod.DataContext.Concrete;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.DataContext.Repositories
{
    public class TransactNumberRepository : RepositorySupply<TransactNumber>, ITransactNumber
    {
        public TransactNumberRepository(EFDbContext context)
        {
            this.context = context;
        }
        public TransactNumberRepository()
            : base()
        {

        }
        public string returnTransactNumber(string trncode, BusinessDay bsday)
        {
            try
            {
                string reference = "";
                //verification de la longueur du code de l'operation
                if (trncode.Trim().Length != 4)
                {
                    throw new Exception("Error While Generate Transaction Number : " + "The length of code operation cannot be different of 4");
                }
                else // si len code op ok
                {
                    //verification si le code existe
                    //var trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == trncode);
                    TransactNumber trn = (from t in context.TransactNumbers
                                          where t.TransactNumberCode == trncode
                                          select t).SingleOrDefault();
                    if (trn == null) //ce code na pas enc ete cree ds la table
                    {
                        //persistance de l'objet TransactNumber
                        TransactNumber transactNumber = new TransactNumber();
                        transactNumber.TransactNumberCode = trncode;
                        transactNumber.DateOperation = bsday.BDDateOperation;
                        transactNumber.BranchID = bsday.BranchID;
                        context.TransactNumbers.Add(transactNumber);
                        context.SaveChanges();
                    }
                    //recuperation des valeurs pour générer la reference
                    //recuperation de l'entite TransactNumber
                    TransactNumber trnumber = context.TransactNumbers.SingleOrDefault(tr => tr.TransactNumberCode == trncode);
                    //fabrication des valeur de la reference
                    string annee = bsday.BDDateOperation.Year.ToString().Substring(2, 2);
                    string mois = "";
                    if (bsday.BDDateOperation.Month.ToString().Length == 1) mois = "0" + bsday.BDDateOperation.Month.ToString();
                    else mois = bsday.BDDateOperation.Month.ToString();
                    string jour = "";
                    if (bsday.BDDateOperation.Day.ToString().Length == 1) jour = "0" + bsday.BDDateOperation.Day.ToString();
                    else jour = bsday.BDDateOperation.Day.ToString();
                    //determination du nbre de zero pr completer la serie
                    int nbrezero = 0;
                    nbrezero = 6 - trnumber.Counter.ToString().Length;
                    string valzero = "";
                    for (int nbre = 1; nbre <= nbrezero; nbre++)
                    {
                        valzero = valzero + "0";
                    }
                    int Newcounter = trnumber.Counter + 1;
                    reference = trncode + jour + mois + annee + valzero + Newcounter.ToString();
                    //update de l'entite en cours
                    trnumber.Counter = Newcounter;
                    trnumber.DateOperation = bsday.BDDateOperation;
                    context.SaveChanges();
                }
                return reference;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public string displayTransactNumber(string trncode, BusinessDay bsday)
        {
            try
            {
                string reference = "";
                //verification de la longueur du code de l'operation
                if (trncode.Trim().Length != 4)
                {
                    throw new Exception("Error While Generate Transaction Number : " + "The length of code operation cannot be different of 4");
                }
                else // si len code op ok
                {
                    //verification si le code existe
                    var trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == trncode);
                    if (trn == null) //ce code na pas enc ete cree ds la table
                    {
                        //persistance de l'objet TransactNumber
                        TransactNumber transactNumber = new TransactNumber();
                        transactNumber.TransactNumberCode = trncode;
                        transactNumber.DateOperation = bsday.BDDateOperation;
                        transactNumber.BranchID = bsday.BranchID;
                        context.TransactNumbers.Add(transactNumber);
                        context.SaveChanges();
                    }
                    //recuperation des valeurs pour générer la reference
                    //recuperation de l'entite TransactNumber
                    TransactNumber trnumber = context.TransactNumbers.SingleOrDefault(tr => tr.TransactNumberCode == trncode);
                    //fabrication des valeur de la reference
                    string annee = bsday.BDDateOperation.Year.ToString().Substring(2, 2);
                    string mois = "";
                    if (bsday.BDDateOperation.Month.ToString().Length == 1) mois = "0" + bsday.BDDateOperation.Month.ToString();
                    else mois = bsday.BDDateOperation.Month.ToString();
                    string jour = "";
                    if (bsday.BDDateOperation.Day.ToString().Length == 1) jour = "0" + bsday.BDDateOperation.Day.ToString();
                    else jour = bsday.BDDateOperation.Day.ToString();
                    //determination du nbre de zero pr completer la serie
                    int nbrezero = 0;
                    nbrezero = 6 - trnumber.Counter.ToString().Length;
                    string valzero = "";
                    for (int nbre = 1; nbre <= nbrezero; nbre++)
                    {
                        valzero = String.Concat(valzero, "0");
                    }
                    int Newcounter = trnumber.Counter + 1;
                    reference = trncode + jour + mois + annee + valzero + Newcounter.ToString();
                    ////update de l'entite en cours
                    //trnumber.Counter = Newcounter;
                    //trnumber.DateOperation = bsday.BDDateOperation;
                    //context.SaveChanges();
                }
                return reference;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public string displayTransactNumber(string trncode, DateTime BDDateOperation, int BranchID)
        {
            try
            {
                string reference = "";
                //verification de la longueur du code de l'operation
                if (trncode.Trim().Length != 4)
                {
                    throw new Exception("Error While Generate Transaction Number : " + "The length of code operation cannot be different of 4");
                }
                else // si len code op ok
                {
                    //verification si le code existe
                    var trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == trncode);
                    if (trn == null) //ce code na pas enc ete cree ds la table
                    {
                        //persistance de l'objet TransactNumber
                        TransactNumber transactNumber = new TransactNumber();
                        transactNumber.TransactNumberCode = trncode;
                        transactNumber.DateOperation = BDDateOperation;
                        transactNumber.BranchID = BranchID;
                        context.TransactNumbers.Add(transactNumber);
                        context.SaveChanges();
                    }
                    //recuperation des valeurs pour générer la reference
                    //recuperation de l'entite TransactNumber
                    TransactNumber trnumber = context.TransactNumbers.SingleOrDefault(tr => tr.TransactNumberCode == trncode);
                    //fabrication des valeur de la reference
                    string annee = BDDateOperation.Year.ToString().Substring(2, 2);
                    string mois = "";
                    if (BDDateOperation.Month.ToString().Length == 1) mois = "0" + BDDateOperation.Month.ToString();
                    else mois = BDDateOperation.Month.ToString();
                    string jour = "";
                    if (BDDateOperation.Day.ToString().Length == 1) jour = "0" + BDDateOperation.Day.ToString();
                    else jour = BDDateOperation.Day.ToString();
                    //determination du nbre de zero pr completer la serie
                    int nbrezero = 0;
                    nbrezero = 6 - trnumber.Counter.ToString().Length;
                    string valzero = "";
                    for (int nbre = 1; nbre <= nbrezero; nbre++)
                    {
                        valzero = valzero + "0";
                    }
                    int Newcounter = trnumber.Counter + 1;
                    reference = trncode + jour + mois + annee + valzero + Newcounter.ToString();
                    ////update de l'entite en cours
                    //trnumber.Counter = Newcounter;
                    //trnumber.DateOperation = bsday.BDDateOperation;
                    //context.SaveChanges();
                }
                return reference;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        /// <summary>
        /// methode pr sauvegarder les numero de transaction
        /// </summary>
        /// <param name="trncode"></param>
        /// <param name="compteur">ce paramettre permet de savoir si achat ou vente directe</param>
        /// <returns></returns>
        public bool saveTransactNumber(string trncode, int compteur)
        {
            bool res = false;
            try
            {
                //verification si le code existe
                TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == trncode);
                if (trn == null)
                {
                    res = false;
                    throw new Exception("This transaction code does not exist");
                }
                else
                {
                    if (compteur == trn.Counter + 1)
                    {
                        //persistance du compteur de l'objet TransactNumber
                        trn.Counter = trn.Counter + 1;
                        context.SaveChanges();
                    }
                    res = true;
                }
            }
            catch (Exception e)
            {
                res = false;
                throw new Exception(e.Message);
            }
            return res;
        }


        public void CounterUpdate(string TransactNumber, string TransactNumberCode)
        {
            //mise a jour du cpteur du transact number
            TransactNumber trn = context.TransactNumbers.SingleOrDefault(t => t.TransactNumberCode == TransactNumberCode);
            if (trn != null)
            {
                //persistance du compteur de l'objet TransactNumber
                trn.Counter = trn.Counter + 1;
            }
            context.SaveChanges();
            
        }

        /// <summary>
        /// Cette méthode permet de générer un transact Number, le persister et renvoyer la référence au demandeur
        /// Le problème majeur ici c'est que si la référence n'est pas utilisée par le démandeur, le compteur aura 
        /// quand même été incrémentée et pour rien.
        /// </summary>
        /// <param name="trncode"></param>
        /// <param name="bsday"></param>
        /// <returns></returns>
        public string GenerateAndUpdateTransactNumber(string trncode, BusinessDay bsday)
        {
            string res = this.displayTransactNumber(trncode, bsday);

            this.CounterUpdate(res, trncode);

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GenerateUniqueCIN ()
        {
            //List<GlobalPerson> lstExistCusto = context.GlobalPeople.ToList();
            //var newCusto = (lstExistCusto.Count==0) ?0 : lstExistCusto.Max(c => c.GlobalPersonID);
            string res = Convert.ToString(context.GlobalPeople.Max(c => c.GlobalPersonID) + 1);

            int nbrezero = 0;
            nbrezero = 6 - res.Length;
            string valzero = "";
            for (int nbre = 1; nbre <= nbrezero; nbre++)
            {
                valzero = String.Concat(valzero, "0");
            }
           res = String.Concat(valzero,res);

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AssuranceID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="LieuxdeDepotBorderoID"></param>
        /// <returns></returns>
        public string GenerateBDFCode(int AssuranceID, int CompanyID = 0, int LieuxdeDepotBorderoID = 0)
        {
            string res = "";

            if (CompanyID == 0 )
            {
                if (LieuxdeDepotBorderoID == 0)
                {

                }
                else
                {

                }
            }
            else
            {
                if (LieuxdeDepotBorderoID == 0)
                {
                    
                }
                else
                {
                    
                }
            }

            return res;
        }

    }
}
