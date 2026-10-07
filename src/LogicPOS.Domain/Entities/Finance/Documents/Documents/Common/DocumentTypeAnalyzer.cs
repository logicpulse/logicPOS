using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities.Documents;

public readonly struct DocumentTypeAnalyzer
{
    private string Type { get; }

    public DocumentTypeAnalyzer(string type)
    {
        Type = type.ToUpper();
    }
    
    public bool IsInvoiceReceipt() => Type == "FR";
    public bool IsCreditNote() => Type == "NC";
    public bool IsDebitNote() => Type == "ND";
    public bool IsSimplifiedInvoice() => Type == "FS";
    private bool IsDeliveryNote() => Type == "GR";
    private bool IsTransportGuide() => Type == "GT";
    private bool IsManagementOfFixedAssetsForm() => Type == "GA";
    private bool IsConsignmentGuide() => Type == "GC";
    private bool IsReturnSlip() => Type == "GD";
    private bool IsBudget() => Type == "OR";
    private bool IsProform() => Type is "PF" or "PP" or "FP";
    private bool IsConsignmentInvoice() => Type == "FC";
    public bool IsInformative() => IsProform() || IsBudget() || IsTableConsult() || IsVoltaRefundReceipt();
    public bool IsSalesInvoices() => IsInvoice() || IsInvoiceReceipt() || IsCreditNote() || IsDebitNote() || IsSimplifiedInvoice();
    public bool IsSalesInvoiceFamily() => IsInvoice() || IsInvoiceReceipt() || IsSimplifiedInvoice();
    public bool IsPayments() => Type == "RG" || Type == "RC";
    public bool IsInvoice() => Type == "FT";
    public bool IsValidCreditNoteParentDocumentType() => IsInvoice() || Type == "FS" || Type == "FR";
    private bool IsTableConsult() => Type == "CM" || Type == "DC";
    public bool IsVoltaRefundReceipt() => Type == "TRV";
    public bool IsWayBill() => IsTransportGuide() || IsConsignmentGuide() || IsManagementOfFixedAssetsForm() || IsDeliveryNote() || IsReturnSlip();

    public bool RequiresTransportDataAtIssue(bool issueWithTransportData) =>
        IsWayBill() || (issueWithTransportData && IsSalesInvoiceFamily());
    public bool RequireFullPayment() => IsSimplifiedInvoice() || IsInvoiceReceipt();
    public bool AllowsFinalConsumer() => IsSimplifiedInvoice() || IsInvoiceReceipt() || IsTableConsult() || IsVoltaRefundReceipt();
    public bool RequiresPaymentCondition() => IsInvoice() || IsConsignmentInvoice() || IsBudget() || IsProform();
    public bool AllowsZeroUnitPrice() => IsWayBill();

    public bool IsValidSalesInvoiceFamilyParentDocumentType(FiscalCountry country)
    {
        if (IsWayBill() || IsTableConsult() || IsConsignmentInvoice() || IsBudget() || IsProform())
        {
            return true;
        }

        return IsSalesInvoiceFamily() && country.IsMozambique;
    }

    public string GetTypeForCountry(FiscalCountry country)
    {
        if (country.IsAngola)
        {
            return Type switch
            {
                "PF" => "PP",
                "FP" => "PP",
                "RC" => "RG",
                _ => Type, 
            };
          
        }

        return Type; 
    }

 
}