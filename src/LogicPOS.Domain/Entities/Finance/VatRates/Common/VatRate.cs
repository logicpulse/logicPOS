using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_configurationvatrate")]
public class VatRate : Entity.WithCode.AndOrder.AndDesignation
{
    [Column(TypeName = "decimal(18,6)")]
    public decimal Value { get; set; }
    public string? ReasonCode { get; set; }
    public string TaxType { get; set; } = null!;
    public string TaxCode { get; set; } = null!;
    public string CountryRegion { get; set; } = null!;
    public DateTime? ExpirationDate { get; set; }
    public string Description { get; set; } = null!;

    public async Task<Result> UpdateAsync(UpdateVatRateDto dto,
                                          IVatRateRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new VatRateUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<VatRate>> CreateAsync(CreateVatRateDto dto,
                                                          IVatRateRepository repository,
                                                          CancellationToken cancellationToken = default)
    {
        var creator = new VatRateCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
}
