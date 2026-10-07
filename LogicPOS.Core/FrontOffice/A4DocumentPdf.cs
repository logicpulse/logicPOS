using LogicPOS.Application.Features.Finance.Documents.PdfGeneration;
using LogicPOS.Core.FrontOffice.Pdf;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LogicPOS.Core.FrontOffice;

public sealed record PosDocumentTypeOption(Guid Id, string Acronym, string Designation);

public static class A4DocumentPdf
{
    static A4DocumentPdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        RegisterSystemFont("arial.ttf");
        RegisterSystemFont("arialbd.ttf");
        RegisterSystemFont("ariali.ttf");
        RegisterSystemFont("arialbi.ttf");
    }

    public static byte[] Render(DocumentPdfData data)
    {
        var model = CreateModel(data);
        model.Copy = 1;
        model.IsSecondCopy = false;
        return model.GeneratePdf();
    }

    private static DocumentModel CreateModel(DocumentPdfData data)
    {
        var type = data.Document!.TypeAnalyzer;
        if (type.IsWayBill())
        {
            return new GuideModel(data);
        }

        if (type.IsInformative())
        {
            return new InformativeModel(data);
        }

        if (type.IsCreditNote())
        {
            return new CreditNoteModel(data);
        }

        if (type.RequireFullPayment())
        {
            return new InvoiceReceiptModel(data);
        }

        return new InvoiceModel(data);
    }

    private static void RegisterSystemFont(string fileName)
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fileName);
        if (File.Exists(path) == false)
        {
            return;
        }

        FontManager.RegisterFont(File.OpenRead(path));
    }
}
