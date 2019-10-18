using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.console.App;
using logicpos.financial.library.Classes.Reports;
using System;

namespace logicpos.financial.console.Test.Classes.Reports
{
    class TestCustomReport
    {
        public static string DocumentMasterCreatePDF()
        {
            fin_documentfinancemaster documentFinanceMaster = TestProcessFinanceDocument.PersistFinanceDocument(SettingsApp.XpoOidDocumentFinanceTypeInvoice);
            string fileName = CustomReport.DocumentMasterCreatePDF(documentFinanceMaster);
            Console.WriteLine(string.Format("fileName: [{0}]", fileName));
            return fileName;
        }
    }
}
