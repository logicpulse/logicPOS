using LogicPOS.Core;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Hardware;

internal static class FrontOfficePrinting
{
    /// <summary>
    /// Prints the issued document on the terminal thermal printer (GTK ThermalPrintingService.PrintInvoice).
    /// Returns an error message, or null when printed or when the terminal has no thermal printer.
    /// </summary>
    public static async Task<string?> PrintInvoiceAsync(Guid documentId)
    {
        var (_, error) = await TryPrintInvoiceAsync(documentId);
        return error;
    }

    /// <summary>
    /// Same as <see cref="PrintInvoiceAsync"/>, but also tells whether a thermal printer handled the document.
    /// </summary>
    public static async Task<(bool Handled, string? Error)> TryPrintInvoiceAsync(Guid documentId)
    {
        var source = AppComposition.Services?.GetService<IThermalPrintSource>();
        if (source is null || documentId == Guid.Empty)
        {
            return (false, null);
        }

        try
        {
            var job = await source.GetInvoiceJobAsync(documentId);
            if (job is null)
            {
                return (false, null);
            }

            await ThermalPrinterOutput.SendAsync(job.Printer, job.Document.Number, ThermalInvoiceRenderer.Render(job));
            try
            {
                await source.RegisterPrintAsync(documentId);
            }
            catch
            {
                // The ticket is already on paper; a failed print record must not report a print error
            }

            return (true, null);
        }
        catch (Exception exception)
        {
            return (true, exception.Message);
        }
    }
}
