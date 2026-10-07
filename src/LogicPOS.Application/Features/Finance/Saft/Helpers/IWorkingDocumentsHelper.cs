using LogicPOS.Shared.Features.Finance.Saft.Entities;

namespace LogicPOS.Application.Features.Finance.Saft.Helpers;

public interface IWorkingDocumentsHelper
{
    public Task<SaftWorkingDocuments> GetWorkingDocumentsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}