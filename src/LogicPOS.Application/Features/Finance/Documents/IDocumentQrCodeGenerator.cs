namespace LogicPOS.Application.Features.Finance.Documents;

public interface IDocumentQrCodeGenerator
{
    public Task<string?> GenerateQrCodeAsync(Domain.Entities.Document document, CancellationToken ct = default);
    public Task<string?> GenerateQrCodeAsync(Domain.Entities.Receipt receipt, Domain.Entities.Document document, CancellationToken ct = default);    
}