namespace LogicPOS.Api.Features.Documents
{
    public struct DocumentTypeAnalyzer
    {
        public string Type { get; }

        public DocumentTypeAnalyzer(string type)
        {
            Type = type;
        }

        public bool IsInvoice() => Type == "FT";
        public bool IsInvoiceReceipt() => Type == "FR";
        public bool IsCreditNote() => Type == "NC";
        public bool IsDebitNote() => Type == "ND";
        public bool IsSimplifiedInvoice() => Type == "FS";
        public bool IsDeliveryNote() => Type == "GR";
        public bool IsTransportGuide() => Type == "GT";
        public bool IsManagementOfFixedAssetsForm() => Type == "GA";
        public bool IsConsignmentGuide() => Type == "GC";
        public bool IsReturnSlip() => Type == "GD";
        public bool IsBudget() => Type == "OR";
        public bool IsProform() => Type == "PF" || Type == "PP" || Type == "FP";
        public bool IsConsignmentInvoice() => Type == "FC";
        public bool IsInformative() => IsProform() || IsBudget();
        public bool IsGuide() => IsTransportGuide() || IsConsignmentGuide() || IsManagementOfFixedAssetsForm() || IsDeliveryNote() || IsReturnSlip();
    }
}
