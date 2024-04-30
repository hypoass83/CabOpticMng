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

namespace CABOPMANAGEMENT.Areas.Sale.Controllers
{
    public class BaseProformaController : BaseController
    {
        private IAccount _accountRepository;
        private IPerson _personRepository;


        public BaseProformaController(IAccount accountRepository, IPerson personRepository)
        {
            this._accountRepository = accountRepository;
            this._personRepository = personRepository;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrder">Element venant de l'interface graphique</param>
        /// <returns>
        /// Si c'est un string ou une xception, il y a eu une erreur et il faudra la traiter
        /// Si c'est un entier, la creation a reussie
        /// </returns>
        public object ConstructCustomer(CustomerOrder customerOrder)
        {
            try
            {

                Customer customer = new Customer();
                #region Recuperation de Date Of Birth
                string[] listDOB = customerOrder.DateOfBirth.Split('-');
                int dayDOB = Convert.ToInt32(listDOB[0]);
                int monthDOB = Convert.ToInt32(listDOB[1]);
                int yearDOB = Convert.ToInt32(listDOB[2]);
                customer.DateOfBirth = new DateTime(yearDOB, monthDOB, dayDOB);
                if (customer.DateOfBirth <= new DateTime(1900, 1, 1))
                {
                    var res1 = "Error: Wrong Date of Birth Format";
                    return res1;
                }
                #endregion

                #region Recuperation des valeurs presentes sur l'interface
                // Champs caches
                customer.GlobalPersonID = customerOrder.GlobalPersonID;
                customer.CNI = customerOrder.CNI;
                customer.CustomerNumber = customerOrder.CustomerNumber;

                // Champs visibles
                customer.Name = customerOrder.CustomerName;
                customer.PreferredLanguage = customerOrder.PreferredLanguage;
                customer.SexID = customerOrder.SexID;
                #endregion

                #region Champs avec les valeurs par defaut
                customer.Profession = "";
                customer.Description = "";
                #endregion

                return customer;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Si
        /// </summary>
        /// <param name="customerOrder">Element venant de l'interface graphique</param>
        /// <returns>
        /// Si c'est un string ou une xception, il y a eu une erreur et il faudra la traiter
        /// Si c'est un entier, la creation a reussie
        /// </returns>
        public object CreateCustomer(CustomerOrder customerOrder)
        {
            try
            {
                Person res = null;
                var result = this.ConstructCustomer(customerOrder);
                if (!(result is Customer))
                {
                    return result;
                }

                Customer customer = (Customer)result;

                BusinessDay SBusinessDay = SessionBusinessDay(null);
                // Recuperation de l'adresse
                customer.Adress = GetAddress(customerOrder, SBusinessDay.Branch);
                customer.AdressID = customerOrder.AdressID;
                if (customer.GlobalPersonID > 0)
                {
                    Customer existingCustomer = db.Customers.SingleOrDefault(c => c.GlobalPersonID == customer.GlobalPersonID);
                    //recuperation du cpte existant
                    customer.AccountID = existingCustomer.AccountID;
                    customer.IsInHouseCustomer = existingCustomer.IsInHouseCustomer;
                    customer.IsBillCustomer = existingCustomer.IsBillCustomer;

                    res = _personRepository.Update2(customer, SessionGlobalPersonID, (SBusinessDay == null) ? DateTime.Now.Date : SBusinessDay.BDDateOperation, (SBusinessDay == null) ? 0 : SBusinessDay.BranchID);
                }
                else
                {
                    customer.IsBillCustomer = true;
                    customer.IsInHouseCustomer = false;
                    customer.GestionnaireID = customerOrder.GestionnaireID;
                    customer.Dateregister = (SBusinessDay == null) ? DateTime.Now.Date : SBusinessDay.BDDateOperation;
                    string code = CodeValue.Accounting.DefaultCodeAccountingSection.CODECLIENT;
                    CollectifAccount insuranceAccount = db.CollectifAccounts.Where(ca => ca.AccountingSection.AccountingSectionCode == code && ca.CollectifAccountLabel == "INSURANCE ACCOUNT").FirstOrDefault();
                    //fabrication du nvo cpte
                    customer.AccountID = _accountRepository.GenerateAccountNumber(insuranceAccount.CollectifAccountID, customer.CustomerFullName + " " + Resources.UIAccount, false).AccountID;
                    res = _personRepository.Create2(customer, SessionGlobalPersonID, (SBusinessDay == null) ? DateTime.Now.Date : SBusinessDay.BDDateOperation, (SBusinessDay == null) ? 0 : SBusinessDay.BranchID);
                }

                return res.GlobalPersonID;
            } catch(Exception e)
            {
                var res2 = e;
                return res2;
            }
        }

        public Adress GetAddress(CustomerOrder customerOrder, Branch branch)
        {
            Adress address = new Adress();
            if (customerOrder.AdressID == 0)
            {
                address = db.Adresses.AsNoTracking().Where(a => a.AdressID == branch.AdressID).FirstOrDefault();
                address.AdressID = 0;
            }
            else
            {
                address = db.Adresses.AsNoTracking().Where(a => a.AdressID == customerOrder.AdressID).FirstOrDefault();
            }
            address.Quarter = null;
            address.People = null;
            address.AdressPhoneNumber = customerOrder.PhoneNumber;
            return address;
        }

        public JsonResult LoadExistingCustomers(string filter)
        {
            var customers = (from customer in db.Customers where (String.Concat(customer.Name, " ", (customer.Description != null) ? customer.Description : "")).Contains(filter)
                             select new
                             {
                                 Name = String.Concat(customer.Name, " ", (customer.Description != null) ? customer.Description : ""),
                                 GlobalPersonID = customer.GlobalPersonID
                             }).Take(50).ToList();


            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomer(int customerId)
        {
            //Value.ToString("dd-MM-yyyy")
            var customerData = (from customer in db.Customers where customer.GlobalPersonID == customerId
                             select new
                             {
                                 DateOfBirth = customer.DateOfBirth,
                                 GlobalPersonID = customer.GlobalPersonID,
                                 CNI = customer.CNI,
                                 CustomerNumber = customer.CustomerNumber,
                                 CustomerName = customer.Name + ((customer.Description != null) ? " " + customer.Description : ""),
                                 PreferredLanguage = customer.PreferredLanguage,
                                 SexID = customer.SexID,
                                 AdressID = customer.AdressID,
                                 PhoneNumber = customer.Adress.AdressPhoneNumber
                             }).FirstOrDefault();
            if (customerData != null)
            {
                int CustomerAge = 0;
                string DateOfBirth = "";
                if (customerData.DateOfBirth.HasValue)
                {
                    CustomerAge = (SessionBusinessDay(null) != null) ? SessionBusinessDay(null).BDDateOperation.Year - customerData.DateOfBirth.Value.Year : DateTime.Today.Year - customerData.DateOfBirth.Value.Year;
                    DateOfBirth  = customerData.DateOfBirth.HasValue ? customerData.DateOfBirth.Value.ToString("dd-MM-yyyy") : "";
                }

                var customer = new
                {
                    DateOfBirth = DateOfBirth,
                    GlobalPersonID = customerData.GlobalPersonID,
                    CNI = customerData.CNI,
                    CustomerNumber = customerData.CustomerNumber,
                    CustomerName = customerData.CustomerName,
                    PreferredLanguage = customerData.PreferredLanguage,
                    SexID = customerData.SexID,
                    AdressID = customerData.AdressID,
                    PhoneNumber = customerData.PhoneNumber,
                    CustomerAge = CustomerAge
                };
                return Json(customer, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(customerData, JsonRequestBehavior.AllowGet);
            }
        }

    }
}