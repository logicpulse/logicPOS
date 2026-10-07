using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_configurationpaymentmethod")]
public class PaymentMethod : Entity.WithCode.AndOrder.AndDesignation
{
    public string Token { get; set; } = null!;
    public string? ResourceString { get; set; }
    public string? ButtonIcon { get; set; }
    public string Acronym { get; set; } = null!;
    public string? AllowPayback { get; set; }
    public string? Symbol { get; set; }

    public async Task<Result> UpdateAsync(UpdatePaymentMethodDto dto,
                                          IPaymentMethodRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new PaymentMethodUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<PaymentMethod>> CreateAsync(CreatePaymentMethodDto dto,
                                                                IPaymentMethodRepository repository,
                                                                CancellationToken cancellationToken = default)
    {
        var creator = new PaymentMethodCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }

}
