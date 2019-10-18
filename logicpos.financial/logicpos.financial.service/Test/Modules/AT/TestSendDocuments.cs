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
            Guid forceDocument = new Guid("99f614c9-93fc-4480-ba98-0f61b42516b5");
            string sql = Utils.GetDocumentsQuery(false, forceDocument);
            object lastDocumentMaster = GlobalFramework.SessionXpo.ExecuteScalar(sql);
            fin_documentfinancemaster documentMaster = null;

            if (lastDocumentMaster != null)
            {
                documentMaster = (fin_documentfinancemaster) GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), new Guid(lastDocumentMaster.ToString()));
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
            Guid forceDocument = new Guid("afbcf434-672c-4b63-817f-021484f834bd");
            string sql = Utils.GetDocumentsQuery(true, forceDocument);
            object lastDocumentMaster = GlobalFramework.SessionXpo.ExecuteScalar(sql);
            fin_documentfinancemaster documentMaster = null;

            if (lastDocumentMaster != null)
            {
                documentMaster = (fin_documentfinancemaster) GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), new Guid(lastDocumentMaster.ToString()));
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
