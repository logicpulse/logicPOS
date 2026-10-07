using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_configurationvatexemptionreason")]
public class VatExemptionReason : Entity.WithCode.AndOrder.AndDesignation
{
    public string Acronym { get; set; }  = null!;
    public string StandardApplicable { get; set; } = null!;

    public async Task<Result> UpdateAsync(UpdateVatExemptionReasonDto dto,
                                          IVatExemptionReasonRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new VatExemptionReasonUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }
    
    public static async Task<Result<VatExemptionReason>> CreateAsync(CreateVatExemptionReasonDto dto,
                                                                     IVatExemptionReasonRepository repository,
                                                                     CancellationToken cancellationToken = default)
    {
        var creator = new VatExemptionReasonCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
}
