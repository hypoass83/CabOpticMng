using System.Linq;
using System.Web;
using System.Web.Mvc;
using FatSod.Security.Entities;
using FatSod.Security.Abstracts;
using System.Web.Security;
using FatSodDental.UI.Models;
using System.Collections.Generic;
using FatSod.DataContext.Initializer;
using FatSodDental.UI.Tools;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using System;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net;
using Ext.Net.Utilities;
using System.Threading;
using System.IO;
using FatSod.DataContext.Concrete;
using System.Web.UI;
using FatSod.Ressources;

namespace FatSodDental.UI.Controllers
{
    public class HomeController : BaseController
    {
        
        // GET: Home
        public ActionResult Index()
        {
            // -- Teste si l'utilisateur ne s'est pas encore connecté, si oui le deconnecter -- //
            /*
            if (CurrentUser != null && SessionGlobalPersonID > 0 && IsUserConnected(SessionGlobalPersonID))
                {
                    X.Msg.Confirm("Connected Users", "Sorry  " + CurrentUser.UserFullName + " Is already Connected !!! " +
                        "Do you want to disconnected that user ?", new MessageBoxButtonsConfig()
                    {
                        Yes = new MessageBoxButtonConfig()
                        {
                            Handler = "Home.DoYes()",
                            Text = "Yes Please"
                        },
                        No = new MessageBoxButtonConfig()
                        {
                            Handler = "Home.DoNo()",
                            Text = "No Thanks"
                        }
                    }).Show();
                    //return this.Direct();
                }*/
            
            return View();
        }
        [DirectMethod]
        public void DoYes()
        {
            X.Msg.Alert("DirectMethod", "DoYes").Show();
        }

        [DirectMethod]
        public void DoNo()
        {
            X.Msg.Alert("DirectMethod", "DoNo").Show();
        }
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

        public ActionResult Rafraichir_liste()
        {
            this.GetCmp<Label>("Label_list_connecte").Text = GetConnectedUserIds().Count + " " + Resources.User_logged;
            return this.Direct();
        }
    }
}