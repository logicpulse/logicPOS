using System.Security.Cryptography.X509Certificates;

using LogicPOS.Shared.Extensions;

namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public abstract record SaftDocument
{
    #region Utils
    public Guid Id { get; set; }
    public Guid? ParentDocumentId { get; set; }
    public string DocType { get; set; } = null!;
    #endregion

    public string Number { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public string HashControl { get; set; } = null!;

    public string Date { get; set; } = null!;

    public string? ATCUD { get; set; }

    public string SystemEntryDate { get; set; } = null!;
    public string Period => SystemEntryDate.FromISO8601DateTime().Month.ToString();

    public SaftDocumentStatus Status { get; set; } = null!;
    public SaftSpecialRegimes? SpecialRegimes { get; set; }
    public List<SaftDocumentLine> Lines { get; set; } = null!;

    public string CustomerID { get; set; } = null!;
    public string SourceID { get; set; } = null!;

    public decimal NetTotal { get; set; }
    public decimal GrossTotal { get; set; }
    public string CurrencyCode { get; set; } = null!;

    public decimal TaxPayable { get; set; }

    public string? EACCode { get; set; }
    public decimal? ExchangeRate { get; set; }
}
