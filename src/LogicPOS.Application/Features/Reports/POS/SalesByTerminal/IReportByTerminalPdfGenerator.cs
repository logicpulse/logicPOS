namespace LogicPOS.Application.Features.Reports.POS.SalesByTerminal;

public interface IReportByTerminalPdfGenerator
{
    byte[] GeneratePdf(SalesByTerminalReportData data);
}