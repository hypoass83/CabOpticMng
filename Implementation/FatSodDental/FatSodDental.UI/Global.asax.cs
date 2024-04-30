using FatSod.DataContext.Concrete;
using FatSod.DataContext.Repositories;
using FatSod.Security.Abstracts;
using FatSodDental.UI.App_Start;
using FatSodDental.UI.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
//using Combres;
//using FatSod.DataContext.Migrations;

namespace FatSodDental.UI
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            //Database.SetInitializer<EFDbContext>(new MigrateDatabaseToLatestVersion<EFDbContext, UserConfiguration>());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //RouteTable.Routes.AddCombresRoute("Combres");
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Application["ConnectedUserIds"] = new List<int>();
            //System.Web.HttpContext.Current.Application["ConnectedUserIds"] = new List<int>();
            //get default culture
            FatSod.Ressources.Resources.Culture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Application["ConnectedUserIds"] = new List<int>();
            System.Web.HttpContext.Current.Application["ConnectedUserIds"] = new List<int>();
            System.Web.HttpContext.Current.Application.Clear();
        }

        //protected void Session_Start(object sender, EventArgs e)
        //{
        //    List<int> res = (List<int>)System.Web.HttpContext.Current.Application["ConnectedUserIds"];

        //}
        // -- S'execute automatiquement au démarrage de la session
        protected void Session_OnStart()
        {

            // -- Reparametrage du temps de durée d'une variable session à 525600 minutes -- //
            Session.Timeout = 525600;
            // ------------------------------------------------------------------------//
            //recupere la liste des user connecter
            List<int> res = (List<int>)System.Web.HttpContext.Current.Application["ConnectedUserIds"];

            //////////////////////////////////////////////////////////////////////////////////////////
            ///////// Gestion des themes et de la langua à travers les préférences dans la bd ////////
            //////////////////////////////////////////////////////////////////////////////////////////
            try
            {

                if (res!=null && res.Count != 0)
                {
                    // -- Récupération des préférences du dernier utilisateur connecté -- //
                    string cultureName = null;

                    // Attempt to read the culture cookie from Request
                    HttpCookie cultureCookie = Request.Cookies["_culture"];
                    if (cultureCookie != null)
                        cultureName = cultureCookie.Value;
                    else
                        cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                                Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                                null;
                    // Validate culture name
                    cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

                    
                    // Changement de la langue //
                    CultureHelper.Changer_langue(cultureName);
                   
                }
                // -- Si ça n'existe pas charge les valeurs par défaut -- //
                else
                {
                    // Changement de la langue anglaise //
                    CultureHelper.Changer_langue("en-US");
                }
            }
            catch
            {
                // Changement de la langue anglaise //
                CultureHelper.Changer_langue("en-US");
            }
            // ----------------------------------------------------------- //
        }


        // -- S'execute automatiquement lorsqu'une session expire ou est abandonné manuellement
        protected void Session_OnEnd()
        {
            //bool IsSessionExpired = true;
            //System.Web.HttpContext.Current.Session["IsSessionExpired"] = IsSessionExpired;

            //Récupérer l'id du user
            int userID = ((int)System.Web.HttpContext.Current.Session["ConnectedUserID"] == null) ? 0 : (int)System.Web.HttpContext.Current.Session["ConnectedUserID"];

            //Déconnecter le user
            if (userID > 0)
            {
                System.Web.HttpContext.Current.Application.Lock();
                List<int> res = (List<int>)System.Web.HttpContext.Current.Application["ConnectedUserIds"];
                res.Remove(userID);
                System.Web.HttpContext.Current.Application["ConnectedUserIds"] = res;
                System.Web.HttpContext.Current.Application.UnLock();
            }
            System.Web.Security.FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session.Clear();
        }
/*
        protected void Session_End(object sender, EventArgs e)
        {
            bool IsSessionExpired = true;
            System.Web.HttpContext.Current.Session["IsSessionExpired"] = IsSessionExpired;

            //Récupérer l'id du user
            int userID = (int)System.Web.HttpContext.Current.Session["ConnectedUserID"];


            //Déconnecter le user
            if (userID > 0)
            {
                System.Web.HttpContext.Current.Application.Lock();
                List<int> res = (List<int>)System.Web.HttpContext.Current.Application["ConnectedUserIds"];
                res.Remove(userID);
                System.Web.HttpContext.Current.Application["ConnectedUserIds"] = res;
                System.Web.HttpContext.Current.Application.UnLock();
            }
            System.Web.Security.FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session.Clear();

        }

        protected void Session_OnEnd(object sender, EventArgs e)
        {
            bool IsSessionExpired = true;
            System.Web.HttpContext.Current.Session["IsSessionExpired"] = IsSessionExpired;

            //Récupérer l'id du user
            int userID = (int)System.Web.HttpContext.Current.Session["ConnectedUserID"];


            //Déconnecter le user
            if (userID > 0)
            {
                System.Web.HttpContext.Current.Application.Lock();
                List<int> res = (List<int>)System.Web.HttpContext.Current.Application["ConnectedUserIds"];
                res.Remove(userID);
                System.Web.HttpContext.Current.Application["ConnectedUserIds"] = res;
                System.Web.HttpContext.Current.Application.UnLock();
            }
            System.Web.Security.FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session.Clear();

        }
*/
    }
}
