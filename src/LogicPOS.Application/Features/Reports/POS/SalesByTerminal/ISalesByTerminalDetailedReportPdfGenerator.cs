namespace LogicPOS.Application.Features.Reports.POS.SalesByTerminal;

public interface ISalesByTerminalDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByTerminalReportData data);
}
