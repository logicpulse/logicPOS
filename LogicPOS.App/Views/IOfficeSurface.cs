namespace LogicPOS.App.Views;

/// <summary>
/// Window that can host document listings and PDF preview (BackOffice or POS).
/// </summary>
public interface IOfficeSurface
{
    Task ShowPdfAsync(string path, string? title, Guid? documentId = null);

    Task ShowNewDocumentAsync(Guid? draftId = null);
}
