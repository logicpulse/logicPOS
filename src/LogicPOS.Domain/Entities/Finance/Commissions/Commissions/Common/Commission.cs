using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_documentfinancecomission")]
public class Commission : Entity
{
    [Column(TypeName = "decimal(18,6)")]
    public decimal CommissionValue { get; set; }
    
    [Column(TypeName = "decimal(18,6)")]
    public decimal Total { get; set; }

    public CommissionGroup? CommissionGroup { get; set; }
    public Guid CommissionGroupId { get; set; }

    public DocumentDetail? DocumentDetail { get; set; }
    public Guid DocumentDetailId { get; set; }

    public User? User { get; set; }
    public Guid UserId { get; set; }

    public static async Task<Result<Commission>> CreateAsync(CreateCommissionDto dto,
                                                             CommissionReferences references,
                                                             CancellationToken cancellationToken = default)
    {
        var creator = new CommissionCreator(dto, references);
        return await creator.CreateAsync(cancellationToken);
    }

}



