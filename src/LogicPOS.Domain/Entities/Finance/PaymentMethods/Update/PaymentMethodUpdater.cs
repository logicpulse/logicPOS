using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class PaymentMethodUpdater : EntityUpdater<PaymentMethod>
{
    private readonly UpdatePaymentMethodDto _dto;
    
    public PaymentMethodUpdater(PaymentMethod entity,
                                UpdatePaymentMethodDto dto,
                                IPaymentMethodRepository repository) : base(entity,
                                                                            repository)
    {
        _dto = dto;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForCodeAndDesignationConflictsAsync(_dto.Code,
                                                              _dto.Designation,
                                                              ct);
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override void SetNewData()
    {
        _entity.Order = _dto.Order;
        _entity.Code = _dto.Code;
        _entity.Designation = _dto.Designation;
        _entity.Token = _dto.Token;
        _entity.ResourceString = _dto.ResourceString;
        _entity.ButtonIcon = _dto.ButtonIcon;
        _entity.Acronym = _dto.Acronym;
        _entity.AllowPayback = _dto.AllowPayback;
        _entity.Symbol = _dto.Symbol;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
    }
}