using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_configurationpaymentcondition")]
public class PaymentCondition : Entity.WithCode.AndOrder.AndDesignation
{
    public string Acronym { get; set; } = null!;

    public async Task<Result> UpdateAsync(
        UpdatePaymentConditionDto dto,
        IPaymentConditionRepository repository,
        CancellationToken cancellationToken = default)

    {
        var updater = new PaymentConditionUpdater(this,dto,repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<PaymentCondition>> CreateAsync(
        CreatePaymentConditionDto dto,
        IPaymentConditionRepository repository,
        CancellationToken cancellationToken = default)
    {
        var creator = new PaymentConditionCreator(dto,repository);
        return await creator.CreateAsync(cancellationToken);
    }
}
