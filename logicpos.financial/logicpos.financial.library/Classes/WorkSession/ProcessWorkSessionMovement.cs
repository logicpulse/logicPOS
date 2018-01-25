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
        public static bool PersistWorkSessionMovement(POS_WorkSessionPeriod pWorkSessionPeriod, POS_WorkSessionMovementType pWorkSessionMovementType, SYS_UserDetail pUserDetail, POS_ConfigurationPlaceTerminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            return PersistWorkSessionMovement(GlobalFramework.SessionXpo, pWorkSessionPeriod, pWorkSessionMovementType, null, null, pUserDetail, pTerminal, pDate, pMovementAmount, pDescription);
        }

        public static bool PersistWorkSessionMovement(Session pSession, POS_WorkSessionPeriod pWorkSessionPeriod, POS_WorkSessionMovementType pWorkSessionMovementType, SYS_UserDetail pUserDetail, POS_ConfigurationPlaceTerminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            return PersistWorkSessionMovement(GlobalFramework.SessionXpo, pWorkSessionPeriod, pWorkSessionMovementType, null, null, pUserDetail, pTerminal, pDate, pMovementAmount, pDescription);
        }

        //Main Method  - With and Without Session
        public static bool PersistWorkSessionMovement(POS_WorkSessionPeriod pWorkSessionPeriod, POS_WorkSessionMovementType pWorkSessionMovementType, FIN_DocumentFinanceMaster pDocumentFinanceMaster, FIN_DocumentFinancePayment pDocumentFinancePayment, SYS_UserDetail pUserDetail, POS_ConfigurationPlaceTerminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            return PersistWorkSessionMovement(GlobalFramework.SessionXpo, pWorkSessionPeriod, pWorkSessionMovementType, pDocumentFinanceMaster, pDocumentFinancePayment, pUserDetail, pTerminal, pDate, pMovementAmount, pDescription);
        }

        public static bool PersistWorkSessionMovement(Session pSession, POS_WorkSessionPeriod pWorkSessionPeriod, POS_WorkSessionMovementType pWorkSessionMovementType, FIN_DocumentFinanceMaster pDocumentFinanceMaster, FIN_DocumentFinancePayment pDocumentFinancePayment, SYS_UserDetail pUserDetail, POS_ConfigurationPlaceTerminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            //Prevent Deleted Objects, Get Fresh Objects
            SYS_UserDetail userDetail = pSession.GetObjectByKey<SYS_UserDetail>(GlobalFramework.LoggedUser.Oid);
            POS_ConfigurationPlaceTerminal terminal = pSession.GetObjectByKey<POS_ConfigurationPlaceTerminal>(GlobalFramework.LoggedTerminal.Oid);
            POS_WorkSessionMovementType workSessionMovementType = pSession.GetObjectByKey<POS_WorkSessionMovementType>(pWorkSessionMovementType.Oid);

            try
            {
                POS_WorkSessionMovement workSessionMovement = new POS_WorkSessionMovement(pSession)
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
