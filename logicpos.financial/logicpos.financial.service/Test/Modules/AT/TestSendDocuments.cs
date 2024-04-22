using logicpos.datalayer.App;
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
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void SendDocumentNonWayBill()
        {
            //Guid forceDocument = Guid.Empty;
            Guid forceDocument = new Guid("0096E809-58E3-46E6-AB44-135F4585E6C9");
            string sql = Utils.GetDocumentsQuery(false, forceDocument);
            object lastDocumentMaster = DataLayerFramework.SessionXpo.ExecuteScalar(sql);
            fin_documentfinancemaster documentMaster = null;

            if (lastDocumentMaster != null)
            {
                documentMaster = (fin_documentfinancemaster) DataLayerFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), new Guid(lastDocumentMaster.ToString()));
                if (documentMaster != null)
                {
                    Utils.SendDocument(documentMaster);
                }
            }

            if (lastDocumentMaster == null || documentMaster == null)
            {
                Utils.Log($"Error! No document found with Id:[{forceDocument}] in current database:[{FinancialServiceSettings.DatabaseName}] for Non WayBill Documents: [{0}]");
            }
        }

        public static void SendDocumentWayBill()
        {
            //Guid forceDocument = Guid.Empty;
            Guid forceDocument = new Guid("afbcf434-672c-4b63-817f-021484f834bd");
            string sql = Utils.GetDocumentsQuery(true, forceDocument);
            object lastDocumentMaster = DataLayerFramework.SessionXpo.ExecuteScalar(sql);
            fin_documentfinancemaster documentMaster = null;

            if (lastDocumentMaster != null)
            {
                documentMaster = (fin_documentfinancemaster) DataLayerFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), new Guid(lastDocumentMaster.ToString()));
                if (documentMaster != null)
                {
                    Utils.SendDocument(documentMaster);
                }
            }

            if (lastDocumentMaster == null || documentMaster == null)
            {
                Utils.Log($"Error! No document found with Id:[{forceDocument}] in current database:[{FinancialServiceSettings.DatabaseName}] for WayBill Documents: [{0}]");
            }
        }
    }
}
