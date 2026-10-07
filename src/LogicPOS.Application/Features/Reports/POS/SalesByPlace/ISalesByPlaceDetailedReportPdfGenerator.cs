
namespace LogicPOS.Application.Features.Reports.POS.SalesByPlace;

public interface ISalesByPlaceDetailedReportPdfGenerator
{
    byte[] GeneratePdf(SalesByPlaceReportData data);
}
