using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Finance.Documents.Documents.Common;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;
using LogicPOS.Domain.Services;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities;

//[Table("fin_documentfinancepayment")]
public class Receipt : Entity
{
    public PaymentMethod? PaymentMethod { get; set; }
    public Guid PaymentMethodId { get; set; }
    public string DocumentType { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string? Hash { get; set; }
    public string? Hash4Code { get; set; } 
    public string? Atcud { get; set; } 
    public string Status { get; set; } = "N";
    public string StatusDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    public string? StatusReason { get; set; }
    public string? StatusSourceId { get; set; }
    public string SourcePayment { get; set; } = "P";
    public string Mechanism { get; set; } = null!;

    [Column(TypeName = "decimal(18,6)")]
    public decimal Amount { get; set; }
    public string AmountInWords { get; set; } = null!;
    public string Date => CreatedAt.ToString("yyyy-MM-dd");
    public string SourceId { get; set; } = null!;
    public string SystemEntryDate => CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss");

    [Column(TypeName = "decimal(18,6)")]
    public decimal TaxPayable { get; set; }
    public string CurrencyCode { get; set; } = null!;

    [Column(TypeName = "decimal(18,6)")]
    public decimal CurrencyAmount { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal ExchangeRate { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal WithholdingTaxAmount { get; set; }
    public string? QrCode { get; set; }
    public IList<Payment>? Payments { get; set; }

    [Required] public AgtDocumentInfo Agt { get; set; } = new();

    public static async Task<Result<Receipt>> CreateAsync(CreateReceiptDto dto,
                                                          IReceiptRepository repository,
                                                          IDocumentHasher documentHasher,
                                                          ReceiptReferences references,
                                                          FiscalCountry country,
                                                          Guid? terminalId,
                                                          CancellationToken ct = default)
    {
        var creator = new ReceiptCreator(dto,
                                         repository,
                                         documentHasher,
                                         references,
                                         country,
                                         terminalId);

        return await creator.CreateAsync(ct);
    }
}
