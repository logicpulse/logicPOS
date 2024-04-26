using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Reports;
using logicpos.shared.App;
using System;

namespace logicpos.financial.console.Test.Classes.Reports
{
    internal class TestCustomReport
    {
        public static string DocumentMasterCreatePDF()
        {
            fin_documentfinancemaster documentFinanceMaster = TestProcessFinanceDocument.PersistFinanceDocument(SharedSettings.XpoOidDocumentFinanceTypeInvoice);
            string fileName = CustomReport.DocumentMasterCreatePDF(documentFinanceMaster);
            Console.WriteLine(string.Format("fileName: [{0}]", fileName));
            return fileName;
        }
    }
}
