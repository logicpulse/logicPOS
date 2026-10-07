using LogicPOS.Application.Features.Finance.Agt.GetInvoice;
using LogicPOS.Application.Features.Finance.Agt.GetInvoiceStatus;
using LogicPOS.Application.Features.Finance.Agt.ListInvoices;
using LogicPOS.Application.Features.Finance.Agt.ListSeries;
using LogicPOS.Application.Features.Finance.Agt.RegisterInvoice;
using LogicPOS.Application.Features.Finance.Agt.RequestSeries;
using LogicPOS.Application.Features.Finance.Agt.ValidateDocument;

namespace LogicPOS.Application.Features.Finance.Agt.Services;

public interface IAgtServiceRequestBuilder
{
    public Task<RegisterInvoiceRequest> BuildRegisterInvoiceRequestAsync(Guid documentId, CancellationToken ct);

    public Task<RegisterInvoiceRequest> BuildCorrectInvoiceRequestAsync(Guid documentId,
        string rejectedDocumentNumber, CancellationToken ct);

    public Task<RegisterInvoiceRequest> BuildRegisterReceiptRequestAsync(Guid receiptId, CancellationToken ct);
    
    public Task<RegisterInvoiceRequest> BuildCorrectReceiptRequestAsync(Guid receiptId,
        string rejectedReceiptNumber, CancellationToken ct);

    public Task<ListInvoicesRequest> BuildListInvoicesRequestAsync(DateOnly startDate, DateOnly endDate,
        CancellationToken ct);

    public Task<GetInvoiceRequest>
        BuildGetInvoiceRequestAsync(string invoiceNo, string requestId, CancellationToken ct);

    public Task<GetInvoiceStatusRequest> BuildGetInvoiceStatusRequestAsync(string requestId, CancellationToken ct);

    public Task<RequestSeriesRequest> BuildRequestSeriesRequestAsync(string year, string documentType,
        string establishmentNumber, string contingencyIndicator, CancellationToken ct);

    public Task<ValidateDocumentRequest> BuildValidateDocumentRequestAsync(string documentNumber, string action,
        decimal? deductibleVatPercentage,
        decimal? nonDeductibleAmount, CancellationToken ct);

    public Task<ListSeriesRequest> BuildListSeriesRequestAsync(string? code, string? year, string? status,
        string? documentType, string? establishmentNumber, CancellationToken ct);
}