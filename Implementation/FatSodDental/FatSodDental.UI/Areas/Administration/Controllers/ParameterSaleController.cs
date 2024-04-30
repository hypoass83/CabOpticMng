using Ext.Net;
using Ext.Net.MVC;
using FatSodDental.UI.Controllers;
using System.Web.Mvc;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSodDental.UI.Tools;
using System;
using System.Linq;
using System.Web;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSodDental.UI.Filters;

namespace FatSodDental.UI.Areas.Administration.Controllers
{

    public partial class ParameterController : BaseController
    {
        // GET: Administration/ParameterSale
        public ActionResult Sale()
        {
            return View();
        }
    }
}