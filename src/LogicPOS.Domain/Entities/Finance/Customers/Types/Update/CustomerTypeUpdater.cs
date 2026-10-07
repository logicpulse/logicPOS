using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class CustomerTypeUpdater : EntityUpdater<CustomerType>
{
    private readonly UpdateCustomerTypeDto _dto;
    public CustomerTypeUpdater(CustomerType entity,
                               UpdateCustomerTypeDto dto,
                               ICustomerTypeRepository repository) : base( entity, repository)
    {
        _dto = dto;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForDesignationConflictAsync(_dto.Designation, ct);
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
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
    }
}