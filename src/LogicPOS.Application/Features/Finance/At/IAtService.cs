using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Results;

namespace LogicPOS.Application.Features.Finance.At;

public interface IAtService
{
    Task<AtSoapResult> RegisterSeriesAsync(string series,
        string documentType,
        int rangeStart);

    Task<AtSoapResult> FinalizeSeriesAsync(string series,
        string documentType,
        string seriesValidationCode,
        int lastIssuedDocumentNumber);

    Task<AtSoapResult> CancelSeriesAsync(string series,
        string documentType,
        string seriesValidationCode);

    Task<AtSoapResult> GetSeriesAsync(string series);

    Task<AtSoapResult> RegisterInvoiceAsync(Document document);

    Task<AtSoapResult> RegisterTransportDocumentAsync(Document document, string? originatingOn);
}