namespace LogicPOS.Application.Features.Finance.Saft;

public interface ISaftBuilder
{
    public Task<byte[]> BuildAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}
