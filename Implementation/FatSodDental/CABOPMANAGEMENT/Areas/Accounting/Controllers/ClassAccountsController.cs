using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FatSod.DataContext.Concrete;
using FatSod.Supply.Entities;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    public class ClassAccountsController : Controller
    {
        private EFDbContext db = new EFDbContext();

        // GET: Accounting/ClassAccounts
        public ActionResult Index()
        {
            return View(db.ClassAccounts.ToList());
        }

        // GET: Accounting/ClassAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassAccount classAccount = db.ClassAccounts.Find(id);
            if (classAccount == null)
            {
                return HttpNotFound();
            }
            return View(classAccount);
        }

        // GET: Accounting/ClassAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Accounting/ClassAccounts/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClassAccountID,ClassAccountNumber,ClassAccountCode,ClassAccountLabel")] ClassAccount classAccount)
        {
            if (ModelState.IsValid)
            {
                db.ClassAccounts.Add(classAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(classAccount);
        }

        // GET: Accounting/ClassAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassAccount classAccount = db.ClassAccounts.Find(id);
            if (classAccount == null)
            {
                return HttpNotFound();
            }
            return View(classAccount);
        }

        // POST: Accounting/ClassAccounts/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassAccountID,ClassAccountNumber,ClassAccountCode,ClassAccountLabel")] ClassAccount classAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(classAccount).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(classAccount);
        }

        // GET: Accounting/ClassAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassAccount classAccount = db.ClassAccounts.Find(id);
            if (classAccount == null)
            {
                return HttpNotFound();
            }
            return View(classAccount);
        }

        // POST: Accounting/ClassAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClassAccount classAccount = db.ClassAccounts.Find(id);
            db.ClassAccounts.Remove(classAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
