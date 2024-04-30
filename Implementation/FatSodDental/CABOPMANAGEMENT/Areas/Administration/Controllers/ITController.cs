using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using FatSod.Security.Abstracts;
using FatSod.Security.Entities;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CABOPMANAGEMENT.Controllers;

namespace CABOPMANAGEMENT.Areas.Administration.Controllers
{
    public class ITController : BaseController
    {
        private ITransactNumber _transactNumbeRepository;

        private IAccount _accountRepository;
        private IPerson _personRepository;


        public ITController(ITransactNumber transactNumbeRepository,
                            IAccount accountRepository, IPerson personRepository) {
            this._accountRepository = accountRepository;
            this._personRepository = personRepository;
            this._transactNumbeRepository = transactNumbeRepository;
        }

        // GET: Administration/IT
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult MigrateCustomerFromCustomerOrder()
        {
            string statusOperation = "";
            bool status = false;
            try
            {
                List<CustomerOrder> customerOrders = db.CustomerOrders.Where(co => co.CustomerID == null).ToList();
                customerOrders.ForEach(customerOrder =>
                {
                    int customerId = this.CreateCustomer(customerOrder);
                    customerOrder.CustomerID = customerId;
                    db.SaveChanges();
                });
                status = true;
            }
            catch (Exception e) {
                statusOperation = "Error " + e.Message + " " + e.InnerException;
                status = false;
            }
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
        }

        public int CreateCustomer(CustomerOrder customerOrder)
        {
            try
            {

                var existingCustomers = /*(List<Customer>)*/_personRepository.FindAll.Where(c => c.AdressPhoneNumber == customerOrder.PhoneNumber ||
                String.Concat(c.Name, " ", (c.Description != null) ? c.Description : "") == customerOrder.CustomerName
                || c.Name == customerOrder.CustomerName).ToList();
                Customer existingCustomer = (Customer)existingCustomers.SingleOrDefault();
                if (existingCustomer != null)
                {
                    return existingCustomer.GlobalPersonID;
                }

                Person res = null;
                Customer result = this.ConstructCustomer(customerOrder);
                
                Customer customer = (Customer)result;

                BusinessDay SBusinessDay = SessionBusinessDay(null);
                // Recuperation de l'adresse
                customer.Adress = GetAddress(customerOrder, SBusinessDay.Branch);
                customer.AdressID = customerOrder.AdressID;

                customer.IsBillCustomer = true;
                customer.IsInHouseCustomer = false;
                customer.GestionnaireID = customerOrder.GestionnaireID;
                customer.Dateregister = (SBusinessDay == null) ? DateTime.Now.Date : SBusinessDay.BDDateOperation;
                string code = CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT;
                CollectifAccount insuranceAccount = db.CollectifAccounts.Where(ca => ca.AccountingSection.AccountingSectionCode == code && ca.CollectifAccountLabel == "INSURANCE ACCOUNT").FirstOrDefault();
                //fabrication du nvo cpte
                customer.AccountID = _accountRepository.GenerateAccountNumber(insuranceAccount.CollectifAccountID, customer.CustomerFullName + " " + Resources.UIAccount, false).AccountID;
                res = _personRepository.Create2(customer, SessionGlobalPersonID, (SBusinessDay == null) ? DateTime.Now.Date : SBusinessDay.BDDateOperation, (SBusinessDay == null) ? 0 : SBusinessDay.BranchID);

                return res.GlobalPersonID;
            }
            catch (Exception e)
            {
                throw new Exception("Id = " + customerOrder.CustomerOrderID, e);
            }
        }

        public Customer ConstructCustomer(CustomerOrder customerOrder)
        {
            try
            {

                Customer customer = new Customer();
                customer.DateOfBirth = null;

                string CustomerNumber = _transactNumbeRepository.GenerateUniqueCIN();

                customer.GlobalPersonID = 0;
                customer.CNI = "" + CustomerNumber;
                customer.CustomerNumber = "" + CustomerNumber;

                // Champs visibles
                customer.Name = customerOrder.CustomerName;
                customer.PreferredLanguage = "FR";
                customer.SexID = 1;

                customer.Profession = "";
                customer.Description = "";

                return customer;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Adress GetAddress(CustomerOrder customerOrder, Branch branch)
        {
            Adress address =  db.Adresses.AsNoTracking().Where(a => a.AdressID == customerOrder.AdressID).FirstOrDefault();
            address.Quarter = null;
            address.People = null;
            address.AdressPhoneNumber = customerOrder.PhoneNumber;
            return address;
        }



    }
}