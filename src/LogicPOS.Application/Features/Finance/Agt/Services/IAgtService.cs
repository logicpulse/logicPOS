using LogicPOS.Application.Features.Finance.Agt.Common;
using LogicPOS.Application.Features.Finance.Agt.GetContributor;
using LogicPOS.Application.Features.Finance.Agt.GetInvoice;
using LogicPOS.Application.Features.Finance.Agt.GetInvoiceStatus;
using LogicPOS.Application.Features.Finance.Agt.ListInvoices;
using LogicPOS.Application.Features.Finance.Agt.ListSeries;
using LogicPOS.Application.Features.Finance.Agt.RegisterInvoice;
using LogicPOS.Application.Features.Finance.Agt.RequestSeries;
using LogicPOS.Application.Features.Finance.Agt.ValidateDocument;
using LogicPOS.Domain.Results;

namespace LogicPOS.Application.Features.Finance.Agt.Services;

public interface IAgtService
{
    public Task<AgtResult<RegisterInvoiceResponse>> RegisterInvoiceAsync(RegisterInvoiceRequest request,
        CancellationToken ct = default);
    
    public Task<AgtResult<GetInvoiceStatusResponse>> GetInvoiceStatusAsync(GetInvoiceStatusRequest request,
        CancellationToken ct = default);

    public Task<AgtResult<ListInvoicesResponse>> ListInvoicesAsync(ListInvoicesRequest request,
        CancellationToken ct = default);

    public Task<AgtResult<GetInvoiceResponse>> GetInvoiceAsync(GetInvoiceRequest request,
        CancellationToken ct = default);
    
    public Task<AgtResult<ListSeriesResponse>> ListSeriesAsync(ListSeriesRequest request,
        CancellationToken ct = default);
    
    public Task<AgtResult<RequestSeriesResponse>> RequestSeriesAsync(RequestSeriesRequest request,
        CancellationToken ct = default);
    
    public Task<AgtResult<ValidateDocumentResponse>> ValidateDocumentAsync(ValidateDocumentRequest request,
        CancellationToken ct = default);
    
    public Task<Result<GetContributorResponse>> GetContributorAsync(string nif,
        CancellationToken ct = default);

    public bool DocumentTypeIsEligible(string documentType);

}