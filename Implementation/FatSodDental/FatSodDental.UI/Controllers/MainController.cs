using Ext.Net;
using Ext.Net.MVC;
using FatSod.Ressources;
using FatSod.Security.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FatSodDental.UI.Controllers
{
    public class MainController : BaseController
    {
        // GET: Main
        public ActionResult Index()
        {
            // -- Rediriger à la page d'acceuil si la connexion n'a pas été éffectuée -- //
            User userConnect = (User)Session["CurrentUser"];

            if (userConnect==null || userConnect.GlobalPersonID == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
            
        }

        //public List<BusinessDay> GetOpenedBackDate(User user)
        //{
        //    List<BusinessDay> res = new List<BusinessDay>();

        //    foreach (UserBranch ub in this.db.UserBranches.Where(ub => ub.UserID == user.GlobalPersonID).ToList())
        //    {
        //        res.AddRange(
        //            ub.Branch.BusinessDays.Where(bd => (bd.BDStatut == true) && (bd.ClosingDayStarted == false) && (bd.BackDStatut == true)).ToList()
        //        );
        //    }

        //    return res;
        //}
        ////public ActionResult WorkBackdate()
        ////{
        ////    List<BusinessDay> UserBackDate = db.BusinessDays.Where(b=>b.BackDStatut).ToList();
        ////    Session["UserBackDate"] = UserBackDate;
        ////    return this.Direct();
        ////}
        public StoreResult GetUnderMenuBackDate(string node)
        {
            try
            {
                List<BusinessDay> UserBackDate = (List<BusinessDay>)Session["UserBusDays"];

                bool StateBackDate = (UserBackDate==null) ? false : UserBackDate.FirstOrDefault().BackDStatut;

                FatSod.Security.Entities.User userConnect = (User)Session["CurrentUser"];
                if (node == "_root")
                {
                    // Retourne l'Id du rolé de l'utilisateur connecté afin de construire ses menu-- //
                    if (StateBackDate)
                    {
                        return this.Store(
                       FatSodDental.UI.Tools.LoadComponent.Liste_Menu_Secure(userConnect, Resources.Culture)
                        );
                    }
                    else
                    {
                        return this.Store(
                           FatSodDental.UI.Tools.LoadComponent.Liste_Menu_Secure(userConnect, Resources.Culture)
                        );
                    }
                    
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // 2 --> ** En cour d'utilisation * //
        // -- Génération des différents menus fonction de chaque utilisateurs -- //
        //
        public StoreResult GetUnderMenu(string node)
        {
            try
            {
                FatSod.Security.Entities.User userConnect = (User)Session["CurrentUser"];
                if (node == "_root")
                {
                    // Retourne l'Id du rolé de l'utilisateur connecté afin de construire ses menu-- //
                    return this.Store(
                       FatSodDental.UI.Tools.LoadComponent.Liste_Menu_Secure(userConnect, Resources.Culture)
                    );
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult Refresh_connected()
        {
            DateTime dateBusDay;
            List<BusinessDay> bdDay = (List<BusinessDay>)Session["UserBusDays"];
            if (bdDay == null)
            {
                dateBusDay = DateTime.Today.Date;
            }
            else
            {
                dateBusDay = bdDay.FirstOrDefault().BDDateOperation.Date;
            }
            if (CurrentUser==null)
            {
                this.GetCmp<Ext.Net.Button>("lb_connecte").Text = Resources.a_BusinessDay + " " + String.Format("{0:dd/MM/yyyy}", dateBusDay);

            }
            else
            {
                this.GetCmp<Ext.Net.Button>("lb_connecte").Text = CurrentUser.Name + " (" +
                                                                  CurrentUser.Profile.ProfileCode + ") " +
                                                                  Resources.a_BusinessDay + " " + String.Format("{0:dd/MM/yyyy}", dateBusDay);

            }
            
            return this.Direct();
        }
        // -- Sert à modifier la valeur des Radio de drapeau de langue -- //
        //
        public ActionResult ChangeLanguage()
        {
            try
            {
                //////////////////////////////////////////////////////////////////////////////////////////
                ///////// Gestion des themes et de la langua à travers les préférences dans la bd ////////
                //////////////////////////////////////////////////////////////////////////////////////////
               
                if (Resources.Culture.ToString() == "en-US")
                {
                    // -- Modifie la culture courante de l'application en français -- //
                    Resources.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("fr");
                }
                else
                {
                    // -- Modifie la culture courante de l'application en anglais -- //
                    Resources.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
                }

            }
            catch (Exception ex)
            {
                X.Msg.Alert(Resources.AlertError, ex.Message).Show();
            }

            return RedirectToAction("Index");
        }
        // -- Cet action permet de modifier les information sur le panel décrivant le tabpanel actif -- //
        //
        public ActionResult Describe_Panel(string Value)
        {
            try
            {
                this.GetCmp<Ext.Net.Panel>("Label_DESC").Html = Value;
            }
            catch (Exception ex)
            {
                X.Msg.Alert(Resources.AlertError, ex.Message).Show();
            }

            return this.Direct();
        }
        
    }
}