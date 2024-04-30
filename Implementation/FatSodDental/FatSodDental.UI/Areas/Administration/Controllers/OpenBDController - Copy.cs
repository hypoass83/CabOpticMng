using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSodDental.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using System.Text;
using FatSod.DataContext.Initializer;
using FatSodDental.UI.Tools;

namespace FatSodDental.UI.Areas.Administration.Controllers
{
    [Authorize]
    public class OpenBDController1 : BaseController
    {
        private const string CONTROLLER_NAME = "Administration/" + CodeValue.Supply.OpenBD_SM.CONTROLLER;
        private const string VIEW_NAME = CodeValue.Supply.OpenBD_SM.PATH;
        private IBusinessDay _busDay;
        public OpenBDController1(IBusinessDay busDay)
        {
            this._busDay = busDay;
        }

        public ActionResult OpenBD()
        {
            Session["Curent_Page"] = VIEW_NAME;
            Session["Curent_Controller"] = CONTROLLER_NAME;

            //We verify if the current user has right to access view which this action calls
            if (!LoadAction.isAutoriseToGetSubMenu(CurrentUser.ProfileID, CodeValue.Supply.OpenBD_SM.CODE))
            {
                this.NotAuthorized();
            }

            TempData["BusDays"] = this._busDay.GetClosedBusinessDay();

            this.GetCmp<Panel>("Body").LoadContent(new ComponentLoader
            {//récupération du panel central dont l'id est body 
                Url = Url.Action("OpenBD"),
                DisableCaching = false,
                Mode = LoadMode.Frame
            });

            return View();
        }

        public ActionResult Submit(string selection)
        {
            StringBuilder result = new StringBuilder();

            result.Append("<b>Selected Rows (ids)</b></br /><ul>");
            SelectedRowCollection src = JSON.Deserialize<SelectedRowCollection>(selection);

            foreach (SelectedRow row in src)
            {
                result.Append("<li>" + row.RowIndex + "</li>");
            }

            result.Append("</ul>");
            X.GetCmp<Label>("Label1").Html = result.ToString();

            return this.Direct();
        }


        public ActionResult GetBusDays()
        {
            return this.Store(_busDay.GetOpenedBusinessDay());
        }

        private ActionResult NotAuthorized()
        {
            return View();
        } 

    }
}