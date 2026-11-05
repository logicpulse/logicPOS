using LogicPOS.Application.Features.Finance.Documents.PdfGeneration;
using LogicPOS.Core.FrontOffice.Pdf;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LogicPOS.Core.FrontOffice;

public sealed record PosDocumentTypeOption(Guid Id, string Acronym, string Designation);

public static class A4DocumentPdf
{
    private static readonly object FontGate = new();
    private static bool _fontsReady;

    static A4DocumentPdf() => EnsureReady();

    /// <summary>License and Arial registration. Receipt PDFs do not pass through <see cref="Render"/>.</summary>
    public static void EnsureReady()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        if (_fontsReady)
        {
            return;
        }

        lock (FontGate)
        {
            if (_fontsReady)
            {
                return;
            }

            RegisterSystemFont("arial.ttf");
            RegisterSystemFont("arialbd.ttf");
            RegisterSystemFont("ariali.ttf");
            RegisterSystemFont("arialbi.ttf");
            _fontsReady = true;
        }
    }

    public static byte[] Render(DocumentPdfData data, bool isSecondCopy = false)
    {
        var model = CreateModel(data);
        model.Copy = 1;
        model.IsSecondCopy = isSecondCopy;
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
