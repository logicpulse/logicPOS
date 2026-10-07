namespace LogicPOS.Application.Features.Articles.Stocks;

public interface IBarcodeLabelPdfGenerator
{
    byte[] GeneratePdf(List<BarcodeLabelPdfData> items,
                       CancellationToken ct = default);
}

