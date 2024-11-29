using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.DocumentTypes;
using System;

namespace LogicPOS.Api.Entities
{
    public class DocumentType : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Acronym { get; set; } 
        public int? AcronymLastSerie { get; set; }
        public string ResourceString { get; set; }
        public string ResourceStringReport { get; set; }
        public bool Paid { get; set; }
        public bool Credit { get; set; }
        public int? CreditDebit { get; set; }
        public int PrintCopies { get; set; }
        public bool PrintRequestMotive { get; set; }
        public bool PrintRequestConfirmation { get; set; }
        public bool PrintOpenDrawer { get; set; }
        public bool WayBill { get; set; }
        public bool WsAtDocument { get; set; }
        public bool SaftAuditFile { get; set; }
        public SaftDocumentType SaftDocumentType { get; set; }
        public ProcessArticleStockMode StockMode { get; set; }

        public bool IsInvoice() => Acronym == "FT";
        public bool IsInvoiceReceipt() => Acronym == "FR";
        public bool IsCreditNote() => Acronym == "NC";
        public bool IsDebitNote() => Acronym == "ND";
        public bool IsReceipt() => Acronym == "RC";
        public bool IsSimplifiedInvoice() => Acronym == "FS";
        public bool IsDeliveryNote() => Acronym == "GR";
        public bool IsTransportGuide() => Acronym == "GT";
        public bool IsManagementOfFixedAssetsForm() => Acronym == "GA";
        public bool IsConsignmentGuide() => Acronym == "GC";
        public bool IsReturnSlip() => Acronym == "GD";
        public bool IsBudget() => Acronym == "OR";
        public bool IsProform() => Acronym == "PF";
        public bool IsTableConsult() => Acronym == "CM";
        public bool IsConsignmentCredit() => Acronym == "CC";
        public bool IsConsignmentInvoice() => Acronym == "FC";

        public bool IsInformative() => IsProform() || IsBudget();
        public bool IsGuide() => IsTransportGuide() || IsConsignmentGuide() || IsManagementOfFixedAssetsForm() || IsDeliveryNote() || IsReturnSlip();
    }
}
