using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logicpos.Classes.Enums.Dialogs
{
    /// <summary>
    /// Okay: user confirmation to proceed.
    /// LoadMore: for pagination purposes, when user select "Load More" button.
    /// Filter: response for when "Filter" is selected.
    /// CleanFilter: when user opts to clear filter.
    /// 
    /// From 11 to 19: in use in "PosDocumentFinanceSelectRecordDialog" class.
    /// From -7 to -5 and -1: from GTK "ResponseType" enum.
    /// 
    /// Related to #IN009223 and #IN009227.
    /// </summary>
    public enum DialogResponseType
    {
        LoadMore = 80,
        Filter = 81,
        CleanFilter = 22,

        Print = 11,
        PrintAs = 12,
        PayCurrentAcountsDocument = 13,
        NewDocument = 14,
        PayInvoice = 15,
        CancelDocument = 16,
        OpenDocument = 17,
        CloneDocument = 18,
        SendEmailDocument = 19,
        ExportPdf = 26,
        ExportXls = 27,

        Close = -7,
        Cancel = -6,
        Ok = -5,
        None = -1
    }
}