using LogicPOS.Domain.Entities;
using LogicPOS.Shared.Features.Company;

namespace LogicPOS.Application.Features.Finance.Documents.PdfGeneration;

public interface IReceiptPdfGenerator
{
    byte[] GeneratePdf(DocumentPdfData data,
                       uint copyNumber, bool isSecondCopy);
}
