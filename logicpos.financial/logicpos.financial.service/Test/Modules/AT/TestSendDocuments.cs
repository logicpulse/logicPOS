using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.service.App;
using logicpos.financial.service.Objects;
using logicpos.shared;
using System;

namespace logicpos.financial.service.Test.Modules.AT
{
    public class TestSendDocument
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void SendDocumentNonWayBill()
        {
            //Guid forceDocument = Guid.Empty;
            Guid forceDocument = new Guid("5f722291-23e9-481a-af4d-795325db1f35");
            string sql = Utils.GetDocumentsQuery(false, forceDocument);
            object lastDocumentMaster = GlobalFramework.SessionXpo.ExecuteScalar(sql);
            FIN_DocumentFinanceMaster documentMaster = null;

            if (lastDocumentMaster != null)
            {
                documentMaster = (FIN_DocumentFinanceMaster) GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), new Guid(lastDocumentMaster.ToString()));
                if (documentMaster != null)
                {
                    Utils.SendDocument(documentMaster);
                }
            }

            if (lastDocumentMaster == null || documentMaster == null)
            {
                Utils.Log($"Error! No document found with Id:[{forceDocument}] in current database:[{SettingsApp.DatabaseName}] for Non WayBill Documents: [{0}]");
            }
        }

        public static void SendDocumentWayBill()
        {
            //Guid forceDocument = Guid.Empty;
            Guid forceDocument = new Guid("db71b270-1d29-4802-a7d5-c21b07f9f71d");
            string sql = Utils.GetDocumentsQuery(true, forceDocument);
            object lastDocumentMaster = GlobalFramework.SessionXpo.ExecuteScalar(sql);
            FIN_DocumentFinanceMaster documentMaster = null;

            if (lastDocumentMaster != null)
            {
                documentMaster = (FIN_DocumentFinanceMaster) GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), new Guid(lastDocumentMaster.ToString()));
                if (documentMaster != null)
                {
                    Utils.SendDocument(documentMaster);
                }
            }

            if (lastDocumentMaster == null || documentMaster == null)
            {
                Utils.Log($"Error! No document found with Id:[{forceDocument}] in current database:[{SettingsApp.DatabaseName}] for WayBill Documents: [{0}]");
            }
        }
    }
}
