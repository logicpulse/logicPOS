using FastReport;
using logicpos.shared.Enums;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Common;
using LogicPOS.Reporting.Reports.Documents;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Utility;
using System;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByFinanceDocumentReport : SalesReport
    {  
        public SalesByFinanceDocumentReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            ) : base(
                filter,
                readableFilter,
                "([DocumentFinanceMaster.DocumentType.Code]) [DocumentFinanceMaster.DocumentType.Designation]",
                "[DocumentFinanceMaster.DocumentType.Ord]",
                "REPORT_SALES_PER_FINANCE_DOCUMENT",
                viewMode
                )
        {

            Initialize();
        }
    }
}
