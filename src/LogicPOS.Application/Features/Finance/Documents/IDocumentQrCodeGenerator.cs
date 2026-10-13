namespace LogicPOS.Application.Features.Finance.Documents;

public interface IDocumentQrCodeGenerator
{
    Task<string?> GenerateQrCodeAsync(Domain.Entities.Document document, CancellationToken ct = default);

    Task<string?> GenerateQrCodeAsync(
        Domain.Entities.Receipt receipt,
        Domain.Entities.Document document,
        decimal invoicesPaid,
        CancellationToken ct = default);
}