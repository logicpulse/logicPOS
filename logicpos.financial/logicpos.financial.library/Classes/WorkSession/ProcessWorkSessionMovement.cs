using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using System;

namespace logicpos.financial.library.Classes.WorkSession
{
    public class ProcessWorkSessionMovement
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //No DocumentFinanceMaster|DocumentFinancePayment - With and Without Session
        public static bool PersistWorkSessionMovement(pos_worksessionperiod pWorkSessionPeriod, pos_worksessionmovementtype pWorkSessionMovementType, sys_userdetail pUserDetail, pos_configurationplaceterminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            return PersistWorkSessionMovement(GlobalFramework.SessionXpo, pWorkSessionPeriod, pWorkSessionMovementType, null, null, pUserDetail, pTerminal, pDate, pMovementAmount, pDescription);
        }

        public static bool PersistWorkSessionMovement(Session pSession, pos_worksessionperiod pWorkSessionPeriod, pos_worksessionmovementtype pWorkSessionMovementType, sys_userdetail pUserDetail, pos_configurationplaceterminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            return PersistWorkSessionMovement(GlobalFramework.SessionXpo, pWorkSessionPeriod, pWorkSessionMovementType, null, null, pUserDetail, pTerminal, pDate, pMovementAmount, pDescription);
        }

        //Main Method  - With and Without Session
        public static bool PersistWorkSessionMovement(pos_worksessionperiod pWorkSessionPeriod, pos_worksessionmovementtype pWorkSessionMovementType, fin_documentfinancemaster pDocumentFinanceMaster, fin_documentfinancepayment pDocumentFinancePayment, sys_userdetail pUserDetail, pos_configurationplaceterminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            return PersistWorkSessionMovement(GlobalFramework.SessionXpo, pWorkSessionPeriod, pWorkSessionMovementType, pDocumentFinanceMaster, pDocumentFinancePayment, pUserDetail, pTerminal, pDate, pMovementAmount, pDescription);
        }

        public static bool PersistWorkSessionMovement(Session pSession, pos_worksessionperiod pWorkSessionPeriod, pos_worksessionmovementtype pWorkSessionMovementType, fin_documentfinancemaster pDocumentFinanceMaster, fin_documentfinancepayment pDocumentFinancePayment, sys_userdetail pUserDetail, pos_configurationplaceterminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            //Prevent Deleted Objects, Get Fresh Objects
            sys_userdetail userDetail = pSession.GetObjectByKey<sys_userdetail>(GlobalFramework.LoggedUser.Oid);
            pos_configurationplaceterminal terminal = pSession.GetObjectByKey<pos_configurationplaceterminal>(GlobalFramework.LoggedTerminal.Oid);
            pos_worksessionmovementtype workSessionMovementType = pSession.GetObjectByKey<pos_worksessionmovementtype>(pWorkSessionMovementType.Oid);

            try
            {
                pos_worksessionmovement workSessionMovement = new pos_worksessionmovement(pSession)
                {
                    Ord = pOrd,
                    WorkSessionPeriod = pWorkSessionPeriod,
                    WorkSessionMovementType = workSessionMovementType,
                    UserDetail = userDetail,
                    Terminal = terminal,
                    Date = pDate,
                    MovementAmount = pMovementAmount,
                    Description = pDescription
                };
                //Assign parent DocumentFinanceMaster and PaymentMethod
                if (pDocumentFinanceMaster != null)
                {
                    workSessionMovement.DocumentFinanceMaster = pDocumentFinanceMaster;
                    workSessionMovement.DocumentFinanceType = pDocumentFinanceMaster.DocumentType;
                    workSessionMovement.PaymentMethod = pDocumentFinanceMaster.PaymentMethod;
                }
                //Assign parent DocumentFinancePayment and PaymentMethod
                if (pDocumentFinancePayment != null)
                {
                    workSessionMovement.DocumentFinancePayment = pDocumentFinancePayment;
                    workSessionMovement.DocumentFinanceType = pDocumentFinancePayment.DocumentType;
                    workSessionMovement.PaymentMethod = pDocumentFinancePayment.PaymentMethod;
                }

                //Save WorkSessionMovement if not in UOW
                if (pSession.GetType() == typeof(Session)) workSessionMovement.Save();

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
        }
    }
}
