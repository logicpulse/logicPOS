using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("fin_documentfinancemasterpayment")]
public class Payment : Entity
{
    [Column(TypeName = "decimal(18,6)")]
    public decimal CreditAmount { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal DebitAmount { get; set; }

    public Document? Document { get; set; }
    public Guid DocumentId { get; set; }

    public Receipt? Receipt { get; set; }
    public Guid ReceiptId { get; set; }
}
