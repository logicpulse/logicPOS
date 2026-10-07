using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Finance.Documents.Documents.Common;
using LogicPOS.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogicPOS.Domain.Entities;

//[Table("fin_documentfinancemaster")]
public partial class Document : Entity
{
    #region Relations

    public PaymentCondition? PaymentCondition { get; set; }
    public Guid? PaymentConditionId { get; set; }

    public Currency? Currency { get; set; }
    public Guid CurrencyId { get; set; }

    public DocumentSeries? Series { get; set; }
    public Guid SeriesId { get; set; }

    public Guid? ParentId { get; set; }

    public Guid CustomerId { get; set; }

    #endregion

    #region Properties

    public string Number { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? StatusDate { get; set; }
    public string? StatusReason { get; set; }
    public string? StatusUser { get; set; }
    public string? SourceBilling { get; set; } = "P";
    public string Hash { get; set; } = null!;
    public string HashControl { get; set; } = "1";
    public int SelfBillingIndicator { get; set; }
    public int CashVatSchemeIndicator { get; set; }
    public int ThirdPartiesBillingIndicator { get; set; }
    public string? EACCode { get; set; }
    public string Date => CreatedAt.ToString("yyyy-MM-dd");
    public string SystemEntryDate => CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss");
    public bool IsDraft { get; set; }

    [Required] public ShipAddress ShipToAddress { get; set; } = new();

    [Required] public ShipAddress ShipFromAddress { get; set; } = new();

    public DateTime MovementStartTime { get; set; }
    public DateTime MovementEndTime { get; set; }

    [Column(TypeName = "decimal(18,6)")] public decimal TotalNet { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal TotalDiscount { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal TotalTax { get; set; }

    [Column(TypeName = "decimal(18,6)")] public decimal TotalFinal { get; set; }

    [Column(TypeName = "decimal(18,6)")] public decimal TotalDelivery { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal TotalChange { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal Discount { get; set; }

    [Column(TypeName = "decimal(18,6)")] public decimal ExchangeRate { get; set; }

    [Column(TypeName = "decimal(18,6)")] public decimal WithholdingTaxAmount { get; set; }

    [Required] public DocumentCustomer Customer { get; set; } = null!;
    [Required] public AgtDocumentInfo Agt { get; set; } = new();

    public bool Paid { get; set; }
    public DateTime? PaymentDate { get; set; }

    public IList<DocumentDetail>? Details { get; set; }
    public IList<DocumentPaymentMethod>? PaymentMethods { get; set; }

    #endregion

    #region AT

    public string? ATDocCodeID { get; set; }

    public Guid? ATValidAuditResult { get; set; }

    public bool? ATResendDocument { get; set; }

    public string? ATCUD { get; set; }

    public string? ATQRCode { get; set; }

    #endregion
}