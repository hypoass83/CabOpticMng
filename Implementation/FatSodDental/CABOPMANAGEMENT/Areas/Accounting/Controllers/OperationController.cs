using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using CABOPMANAGEMENT.Tools;
using CABOPMANAGEMENT.Controllers;
using FatSod.DataContext.Initializer;
using FatSod.Ressources;
using CABOPMANAGEMENT.Filters;

namespace CABOPMANAGEMENT.Areas.Accounting.Controllers
{
    [Authorize(Order = 1)]
    [TakeBusinessDay(Order = 2)]
    public class OperationController : BaseController
    {
        private IRepositorySupply<Operation> _OperationRepository;
        //
        //Current Controller and current page
        private const string CONTROLLER_NAME = "Accounting/Operation";
        private const string VIEW_NAME = "Index";
        

        public OperationController(IRepositorySupply<Operation> OperationRepository)
        {
            this._OperationRepository = OperationRepository;
        }
       
       
        // GET: /Accounting/Operation/
        [OutputCache(Duration = 3600)] 
        public ActionResult Index()
        {
           
            return View(Model());
        }

        public JsonResult populateJournal()
        {
            //holds list of ClassAccountss
            List<object> _Journal = new List<object>();
            //queries all the ClassAccountss for its ID and Name property.
            var Journal = (from s in db.Journals
                                    select new { s.JournalID, s.JournalLabel, s.JournalCode }).ToList();

            //save list of ClassAccountss to the _CollectifAccount
            foreach (var item in Journal.OrderBy(i => i.JournalCode))
            {
                _Journal.Add(new
                {
                    ID = item.JournalID,
                    Name = item.JournalCode
                });
            }
            //returns the Json result of _Journal
            return Json(_Journal, JsonRequestBehavior.AllowGet);
        }

        public JsonResult populateTypeOperation()
        {
            //holds list of ClassAccountss
            List<object> _TypeOperation = new List<object>();
            //queries all the ClassAccountss for its ID and Name property.
            var Journal = (from s in db.OperationTypes
                           select new { s.operationTypeID, s.operationTypeLabel, s.operationTypeCode }).ToList();

            //save list of ClassAccountss to the _CollectifAccount
            foreach (var item in Journal.OrderBy(i => i.operationTypeCode))
            {
                _TypeOperation.Add(new
                {
                    ID = item.operationTypeID,
                    Name = item.operationTypeCode
                });
            }
            //returns the Json result of _Journal
            return Json(_TypeOperation, JsonRequestBehavior.AllowGet);
        }

        public JsonResult populateCodeOperation()
        {
            //holds list of ClassAccountss
            List<object> _CodeOperation = new List<object>();

            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODEMANUALOP,
                Name = CodeValue.Accounting.InitOperation.CODEMANUALOP
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODESALEDELIVERY,
                Name = CodeValue.Accounting.InitOperation.CODESALEDELIVERY
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODECASHADVANCEDSALEDELIVERY,
                Name = CodeValue.Accounting.InitOperation.CODECASHADVANCEDSALEDELIVERY
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODEBANKADVANCEDSALEDELIVERY,
                Name = CodeValue.Accounting.InitOperation.CODEBANKADVANCEDSALEDELIVERY
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODESALEBILLING,
                Name = CodeValue.Accounting.InitOperation.CODESALEBILLING
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT,
                Name = CodeValue.Accounting.InitOperation.CODECASHSALEADVANCEDPAYMENT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT,
                Name = CodeValue.Accounting.InitOperation.CODEBANKSALEADVANCEDPAYMENT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODECANCELSALEADVANCEDPAYMENT,
                Name = CodeValue.Accounting.InitOperation.CODECANCELSALEADVANCEDPAYMENT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT,
                Name = CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT,
                Name = CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODECERTIFYRETURNSALE,
                Name = CodeValue.Accounting.InitOperation.CODECERTIFYRETURNSALE
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT,
                Name = CodeValue.Accounting.InitOperation.CODESTOCKOUTPUT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODESTOCKINPUT,
                Name = CodeValue.Accounting.InitOperation.CODESTOCKINPUT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODEBUDGET,
                Name = CodeValue.Accounting.InitOperation.CODEBUDGET
            });


            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT,
                Name = CodeValue.Accounting.InitOperation.CODECASHSALEPAYMENT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT,
                Name = CodeValue.Accounting.InitOperation.CODEBANKSALEPAYMENT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODEPURCHASEDELIVERY,
                Name = CodeValue.Accounting.InitOperation.CODEPURCHASEDELIVERY
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING,
                Name = CodeValue.Accounting.InitOperation.CODEPURCHASEBILLING
            });

            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODECASHPURCHASEADVANCEDPAYMENT,
                Name = CodeValue.Accounting.InitOperation.CODECASHPURCHASEADVANCEDPAYMENT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODEBANKPURCHASEADVANCEDPAYMENT,
                Name = CodeValue.Accounting.InitOperation.CODEBANKPURCHASEADVANCEDPAYMENT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODECANCELPURCHASEADVANCEDPAYMENT,
                Name = CodeValue.Accounting.InitOperation.CODECANCELPURCHASEADVANCEDPAYMENT
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODECASHPAYMENTPURCHASE,
                Name = CodeValue.Accounting.InitOperation.CODECASHPAYMENTPURCHASE
            });
            _CodeOperation.Add(new
            {
                ID = CodeValue.Accounting.InitOperation.CODEBANKPAYMENTPURCHASE,
                Name = CodeValue.Accounting.InitOperation.CODEBANKPAYMENTPURCHASE
            });
            //returns the Json result of _Journal
            return Json(_CodeOperation, JsonRequestBehavior.AllowGet);
        }
        public List<Operation> Model()
        {
            List<Operation> list = new List<Operation>();
            db.Operations.ToList().ForEach(c =>
            {
                list.Add(
                    new Operation
                    {
                        OperationID = c.OperationID,
                        OperationCode = c.OperationCode,
                        OperationLabel = c.OperationLabel,
                        /*UIoperationTypeCode = c.UIoperationTypeCode,
                        UIMacroOperationCode = c.UIMacroOperationCode,
                        UIReglementTypeCode = c.UIReglementTypeCode,*/
                        OperationDescription = c.OperationDescription,
                        //MacroOperationID = c.MacroOperationID,
                        OperationTypeID = c.OperationTypeID,
                        //ReglementTypeID = c.ReglementTypeID
                        Journal = c.Journal
                    }
                );
            });
            return list;
        }
        //add Accounting Task
       // [HttpPost]
        public ActionResult AddOperation(Operation operation)
        {
            bool status = false;
            try
            {
                if (operation.OperationID > 0)
                {

                    Operation OperationToUpdate = db.Operations.SingleOrDefault(c => c.OperationID == operation.OperationID);
                    operation.OperationCode = operation.OperationCode;
                    //operation.ReglementTypeID = OperationToUpdate.ReglementTypeID;
                    //operation.MacroOperationID = OperationToUpdate.MacroOperationID;
                    operation.OperationTypeID = operation.OperationTypeID;
                    _OperationRepository.Update(operation, operation.OperationID);
                    statusOperation = Resources.Success +" : "+ operation.OperationLabel + " : " + Resources.AlertUpdateAction;
                }
                else
                {
                    if (!LoadAction.IsMenuActionAble(MenuAction.ADD, SessionProfileID, CodeValue.Accounting.AccountOperation.CODE,db))
                    //if (!LoadAction.isAutoriseToGetMenu(SessionProfileID, CodeValue.Accounting.AccountOperation.CODE, db))
                    {
                        _OperationRepository.Create(operation);
                        statusOperation = Resources.Success + " : " + operation.OperationLabel + " : " + Resources.AlertAddAction;
                    }
                    else
                    {
                        statusOperation = Resources.UIOperation + " : " + operation.OperationLabel + " : " + Resources.msgCreateOperation;
                        status = true;
                        return new JsonResult { Data = new { status = status, Message = statusOperation } };
                    }

                }


                status = true;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
                
            }
            catch (Exception e)
            {
                statusOperation = Resources.UIOperation +" : " +Resources.er_alert_danger + e.Message;

                status = false;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
                
            }
        }
        //[HttpPost]
        public JsonResult GetList()
        {
            List<object> list = new List<object>();
            db.Operations.ToList().ForEach(c =>
            {
                list.Add(
                    new
                    {
                        OperationID = c.OperationID,
                        OperationCode =  c.OperationCode,
                        OperationLabel = c.OperationLabel,
                        UIoperationTypeCode = c.UIoperationTypeCode,
                        //UIMacroOperationCode = c.UIMacroOperationCode,
                        //UIReglementTypeCode = c.UIReglementTypeCode,
                        OperationDescription = c.OperationDescription,
                        //MacroOperationID = c.MacroOperationID,
                        OperationTypeID = c.OperationTypeID,
                        //ReglementTypeID = c.ReglementTypeID
                    }
                );
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //Deletes actions
        //[HttpPost]
        public ActionResult DeleteOperation(int ID)
        {
            bool status = false;
            try { 
            
            Operation OperationToDelete = db.Operations.Find(ID);
            db.Operations.Remove(OperationToDelete);
            db.SaveChanges();
            //_OperationRepository.Delete(OperationToDelete);
            statusOperation = Resources.Success +" : "+ OperationToDelete.OperationLabel + " : " + Resources.AlertDeleteAction;
            status = true;
            return new JsonResult { Data = new { status = status, Message = statusOperation } };
                
            }
            catch (Exception e)
            {
                statusOperation = Resources.UIOperation +" : "+Resources.er_alert_danger + e.Message;
                status = false;
                return new JsonResult { Data = new { status = status, Message = statusOperation } };
            }
        }
        //click pr update
        public JsonResult IniatializeFieldAccount(int id)
        {
            List<object> _List = new List<object>();
            
            Operation OperationToUpdate = db.Operations.SingleOrDefault(c => c.OperationID == id);

            _List.Add(new
            {
                OperationID = OperationToUpdate.OperationID,
                OperationCode = OperationToUpdate.OperationCode,
                OperationLabel = OperationToUpdate.OperationLabel,
                OperationDescription = OperationToUpdate.OperationDescription,
                OperationTypeID = OperationToUpdate.OperationTypeID,
                //MacroOperationID = OperationToUpdate.MacroOperationID,
                //ReglementTypeID = OperationToUpdate.ReglementTypeID
            });
            return Json(_List, JsonRequestBehavior.AllowGet);
        }
	}
}