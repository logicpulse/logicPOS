using LogicPOS.Shared.Features.Finance.Saft.Entities;

namespace LogicPOS.Application.Features.Finance.Saft.Helpers;

public interface ISaftEntitiesHelper
{
    public Task<SaftCompanyInformation> GetCompanyInformationAsync(CancellationToken ct);
    public Task<IEnumerable<SaftCustomer>> GetSaftCustomers(DateTime startDate, DateTime endDate, CancellationToken ct);
    public Task<SaftMovementOfGoods> GetMovementOfGoodsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
    public Task<IEnumerable<SaftProduct>> GetProductsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
    public Task<SaftSalesInvoices> GetSalesInvoicesAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
    Task<SaftSoftwareInformation> GetSoftwareInformationAsync(CancellationToken ct);
    public Task<IEnumerable<SaftTax>> GetTaxesAsync(CancellationToken ct);
    public Task<SaftWorkingDocuments> GetWorkingDocumentsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
    public Task<SaftPayments> GetPaymentsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}
