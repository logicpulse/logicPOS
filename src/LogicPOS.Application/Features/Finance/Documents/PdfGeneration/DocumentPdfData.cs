using LogicPOS.Domain.Entities.Documents;
using LogicPOS.Domain.ValueObjects;
using LogicPOS.Shared.Features.Company;

namespace LogicPOS.Application.Features.Finance.Documents.PdfGeneration;

public record DocumentPdfData
{
    public CompanyInformation Company { get; set; } = null!;
    /// <summary>Set by the PDF generators from <c>ISystemInformationService.Country</c>.</summary>
    public FiscalCountry Country { get; init; } = null!;
    public Document? Document { get; set; } 
    public Receipt? Receipt { get; set; }
    public string SoftwareVersion { get; set; } = null!;
}

public record Receipt
{
    public string RefNo { get; set; } = null!;
    public ReceiptCustomer Customer { get; set; } = null!;
    public string? AmountInWords { get; set; }
    public string? Notes { get; set; }
    public string[] RelatedDocs { get; set; } = [];
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = null!;
    public string? Atcud { get; set; }
    public string? QrCode { get; set; }
    public List<ReceiptPayment> Payments { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string CurrencyDesignation { get; set; } = null!;
    public string CurrencyAcronym { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = null!;

    public record ReceiptPayment
    {
        public decimal CreditAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal AllocatedAmount => CreditAmount + DebitAmount;
        public ReceiptPaymentDocument Document { get; set; } = null!;

        public record ReceiptPaymentDocument
        {
            public Guid Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Number { get; set; } = null!;
            public decimal TotalFinal { get; set; }
            public bool Paid { get; set; }
            public decimal WithholdingTaxAmount { get; set; }
        }
    }

    public record ReceiptCustomer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public uint Order { get; set; }
        public string FiscalNumber { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
    }

}

public record Document
{
    public string Type  { get; set; } = null!;
    public List<DocumentDetail> Details { get; set; } = null!;
    public DocumentTypeAnalyzer TypeAnalyzer => new(Type);
    public List<string>? RelatedDocuments { get; set; }
    public string Status { get; set; } = null!;
    public bool IsDraft { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Number { get; set; } = null!;
    public string TypeName { get; set; } = null!;
    public DocumentCustomer Customer { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public uint CustomerOrder { get; set; } 
    public string? PaymentCondition { get; set; }
    public List<PaymentMethod>? PaymentMethods { get; set; }
    public string Currency { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public decimal Discount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalNet { get; set; }
    public decimal TotalTax { get; set; }
    public decimal WithholdingTaxAmount { get; set; }
    public decimal TotalFinal { get; set; }
    public string? Notes { get; set; }
    public string? AtQRCode { get; set; }
    public string? Atcud { get; set; }
    /// <summary>Line supplied by the fiscal plugin. Empty when no plugin is installed.</summary>
    public string? FiscalCodeLine { get; set; }
    public string? AtDocCodeId { get; set; }
    public ShipAddress ShipFromAddress { get; set; } = null!;
    public ShipAddress ShipToAddress { get; set; } = null!;
    public string Date => CreatedAt.ToString("yyyy-MM-dd");

    public List<TaxResume> GetTaxResumes()
    {
        var taxResumes = new List<TaxResume>();

        var ratesGroups = Details.GroupBy(d => d.Tax.Percentage);

        foreach (var group in ratesGroups)
        {
            var resume = new TaxResume
            {
                Code = group.First().Tax.Code,
                Designation = group.First().Tax.Designation,
                Rate = group.First().Tax.Percentage,
                Base = group.Sum(d => d.TotalNet),
                Total = group.Sum(d => d.TotalTax)
            };
            taxResumes.Add(resume);
        }
        
        return taxResumes;
    }
}

public record DocumentDetail
{
    public DateTime CreatedAt { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public decimal TotalNet { get; set; }
    public decimal TotalTax { get; set; }
    public DocumentDetailTax Tax { get; set; } = null!;
    public decimal TotalFinal { get; set; }
    public string? VatExemptionReason { get; set; }
    public string ArticleClassAcronym { get; set; } = null!;
    public string? Notes { get; set; }
    public decimal Discount { get; set; }
    public string? SerialNumber { get; set; }
}

public record PaymentMethod
{
    public string Designation { get; set; } = null!;
    public string Acronym { get; set; } = null!;
    public decimal Amount { get; set; }
}