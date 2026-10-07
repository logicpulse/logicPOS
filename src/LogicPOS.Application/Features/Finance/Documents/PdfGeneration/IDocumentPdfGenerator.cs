namespace LogicPOS.Application.Features.Finance.Documents.PdfGeneration;

public interface IDocumentPdfGenerator
{
    byte[] GeneratePdf(DocumentPdfData data,
        int copy, bool isSecondCopy);


    byte[] GeneratePdf(DocumentPdfData data,
        int[] copies, bool isSecondCopy);
}