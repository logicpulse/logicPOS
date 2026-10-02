using Avalonia.Controls;
using Avalonia.Platform.Storage;
using LogicPOS.Core.BackOffice;

namespace LogicPOS.App.Views;

internal static class ListingFileExport
{
    public static async Task SaveAsync(
        TopLevel? top,
        string title,
        bool excel,
        IReadOnlyList<string> headers,
        IReadOnlyList<IReadOnlyList<string>> rows)
    {
        if (top is null || headers.Count == 0)
        {
            return;
        }

        var extension = excel ? "xlsx" : "pdf";
        var file = await top.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = excel ? "Exportar para Excel" : "Exportar para PDF",
            SuggestedFileName = $"{SafeName(title)}.{extension}",
            FileTypeChoices =
            [
                excel
                    ? new FilePickerFileType("Excel") { Patterns = ["*.xlsx"] }
                    : new FilePickerFileType("PDF") { Patterns = ["*.pdf"] }
            ]
        });
        if (file is null)
        {
            return;
        }

        var bytes = excel
            ? ListingTableExport.ToExcel(title, headers, rows)
            : ListingTableExport.ToPdf(title, headers, rows);
        await using var stream = await file.OpenWriteAsync();
        await stream.WriteAsync(bytes);
    }

    private static string SafeName(string title)
    {
        var name = string.IsNullOrWhiteSpace(title) ? "listagem" : title.Trim();
        foreach (var invalid in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalid, '-');
        }

        return name;
    }
}
