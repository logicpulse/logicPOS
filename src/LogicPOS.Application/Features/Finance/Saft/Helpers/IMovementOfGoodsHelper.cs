using LogicPOS.Shared.Features.Finance.Saft.Entities;

namespace LogicPOS.Application.Features.Finance.Saft.Helpers;

public interface IMovementOfGoodsHelper
{
    public Task<SaftMovementOfGoods> GetMovementOfGoodsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}