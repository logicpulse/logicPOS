using System.Text;
using LogicPOS.Core;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Core.Licensing;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Hardware;

internal static class FrontOfficePrinting
{
    private static Encoding? _cp860;

    private static Encoding Cp860
    {
        get
        {
            if (_cp860 is not null)
            {
                return _cp860;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _cp860 = Encoding.GetEncoding(860, new EncoderReplacementFallback("?"), new DecoderReplacementFallback("?"));
            return _cp860;
        }
    }

    private static string? PrintBlockedMessage()
    {
        var license = AppComposition.Services?.GetService<ILicenseModule>();
        return license is { PrintEnabled: false }
            ? "A impressão está indisponível sem licença válida."
            : null;
    }

    /// <summary>
    /// Prints the issued document on the terminal thermal printer (GTK ThermalPrintingService.PrintInvoice).
    /// Returns an error message, or null when printed or when the terminal has no thermal printer.
    /// </summary>
    public static async Task<(bool Handled, string? Error)> TryReprintInvoiceAsync(Guid documentId, int copies, string reason)
    {
        var blocked = PrintBlockedMessage();
        if (blocked is not null)
        {
            return (false, blocked);
        }

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
                return (true, "Impressora térmica do terminal não configurada.");
            }

            var count = Math.Max(1, copies);
            var payload = ThermalInvoiceRenderer.Render(job);
            for (var copy = 0; copy < count; copy++)
            {
                await ThermalPrinterOutput.SendAsync(job.Printer, job.Document.Number, payload);
            }

            try
            {
                await source.RegisterReprintAsync(documentId, count, reason);
            }
            catch
            {
                // The copies are already on paper
            }

            return (true, null);
        }
        catch (Exception exception)
        {
            return (true, exception.Message);
        }
    }

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
        var blocked = PrintBlockedMessage();
        if (blocked is not null)
        {
            return (false, blocked);
        }

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
                return (true, "Impressora térmica do terminal não configurada.");
            }

            await ThermalPrinterOutput.SendAsync(job.Printer, job.Document.Number, ThermalInvoiceRenderer.Render(job));
            if (job.OpenDrawer)
            {
                try
                {
                    await OpenDrawerAsync(job.Printer);
                }
                catch
                {
                    // The invoice is already printed; a failed drawer pulse must not report a print error
                }
            }

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

    /// <summary>
    /// Prints the kitchen ticket when TICKET_PRINT_TICKET is on (GTK ThermalPrintingService.PrintTicket).
    /// Returns null when printing is off, there is no printer, or the ticket was sent.
    /// </summary>
    public static async Task<string?> PrintOrderTicketAsync(string table, IReadOnlyList<PosTicketLine> lines)
    {
        var blocked = PrintBlockedMessage();
        if (blocked is not null)
        {
            return blocked;
        }

        var source = AppComposition.Services?.GetService<IThermalPrintSource>();
        if (source is null || lines.Count == 0 || await source.IsOrderTicketEnabledAsync() == false)
        {
            return null;
        }

        var printer = await source.GetTerminalPrinterAsync();
        if (printer is null)
        {
            return null;
        }

        try
        {
            await ThermalPrinterOutput.SendAsync(printer, "Pedido", RenderOrderTicket(table, lines, printer.ColumnsNormal));
            return null;
        }
        catch (Exception exception)
        {
            return exception.Message;
        }
    }

    private static byte[] RenderOrderTicket(string table, IReadOnlyList<PosTicketLine> lines, int columns)
    {
        var width = Math.Max(16, columns);
        var text = new StringBuilder();
        text.AppendLine("PEDIDO");
        text.AppendLine(new string('-', width));
        if (string.IsNullOrWhiteSpace(table) == false)
        {
            text.AppendLine(table);
        }

        text.AppendLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        text.AppendLine(new string('-', width));
        foreach (var line in lines)
        {
            text.AppendLine($"{line.Quantity:0.###}  {line.Designation}");
        }

        text.AppendLine();
        text.AppendLine();
        var payload = new List<byte> { 0x1B, 0x40, 0x1B, 0x74, 0x03 };
        payload.AddRange(Cp860.GetBytes(text.ToString()));
        payload.AddRange([0x1B, 0x64, 0x04, 0x1D, 0x56, 0x00]);
        return payload.ToArray();
    }

    /// <summary>
    /// ESC/POS cash-drawer pulse (ESC p 0 25 250) on the terminal thermal printer.
    /// Returns null when there is no printer or the pulse was sent.
    /// </summary>
    public static async Task<string?> OpenDrawerAsync()
    {
        var source = AppComposition.Services?.GetService<IThermalPrintSource>();
        if (source is null)
        {
            return null;
        }

        var printer = await source.GetTerminalPrinterAsync();
        if (printer is null)
        {
            return null;
        }

        try
        {
            await OpenDrawerAsync(printer);
            return null;
        }
        catch (Exception exception)
        {
            return exception.Message;
        }
    }

    public static Task OpenDrawerAsync(ThermalPrinterSettings printer)
        => ThermalPrinterOutput.SendAsync(printer, "Gaveta", [0x1B, 0x70, 0x00, 0x19, 0xFA]);

    public static async Task<string?> PrintCashMovementAsync(string title, decimal amount, string? note, decimal drawerTotal)
    {
        var blocked = PrintBlockedMessage();
        if (blocked is not null)
        {
            return blocked;
        }

        var printer = await TerminalPrinterAsync();
        if (printer is null)
        {
            return "Impressora térmica do terminal não configurada.";
        }

        var width = Math.Max(16, printer.ColumnsNormal);
        var payload = RenderLines(lines =>
        {
            lines.Add(title.ToUpperInvariant());
            lines.Add(new string('-', width));
            lines.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            lines.Add(Pair("Valor", amount.ToString("0.00"), width));
            if (string.IsNullOrWhiteSpace(note) == false)
            {
                lines.Add(note.Trim());
            }

            lines.Add(Pair("Total em caixa", drawerTotal.ToString("0.00"), width));
        });
        return await SendAsync(printer, title, payload);
    }

    public static async Task<string?> PrintCashReportAsync(PosCashReport report)
    {
        var blocked = PrintBlockedMessage();
        if (blocked is not null)
        {
            return blocked;
        }

        var printer = await TerminalPrinterAsync();
        if (printer is null)
        {
            return "Impressora térmica do terminal não configurada.";
        }

        return await SendAsync(printer, report.Title, RenderCashReport(report, printer.ColumnsNormal));
    }

    private static async Task<ThermalPrinterSettings?> TerminalPrinterAsync()
    {
        var source = AppComposition.Services?.GetService<IThermalPrintSource>();
        return source is null ? null : await source.GetTerminalPrinterAsync();
    }

    private static async Task<string?> SendAsync(ThermalPrinterSettings printer, string documentName, byte[] payload)
    {
        try
        {
            await ThermalPrinterOutput.SendAsync(printer, documentName, payload);
            return null;
        }
        catch (Exception exception)
        {
            return exception.Message;
        }
    }

    private static byte[] RenderCashReport(PosCashReport report, int columns)
    {
        var width = Math.Max(16, columns);
        return RenderLines(lines =>
        {
            lines.Add(report.Title.ToUpperInvariant());
            if (string.IsNullOrWhiteSpace(report.Subtitle) == false)
            {
                lines.Add(report.Subtitle);
            }

            lines.Add(new string('-', width));
            lines.Add(Pair("Abertura", report.OpenedAt.ToString("dd/MM/yyyy HH:mm"), width));
            if (report.ClosedAt is DateTime closed)
            {
                lines.Add(Pair("Fecho", closed.ToString("dd/MM/yyyy HH:mm"), width));
            }

            lines.Add(Pair("Fundo de caixa", report.OpeningCash.ToString("0.00"), width));
            lines.Add(Pair("Entradas", report.CashIn.ToString("0.00"), width));
            lines.Add(Pair("Saídas", report.CashOut.ToString("0.00"), width));
            lines.Add(Pair("Total em caixa", report.ClosingCash.ToString("0.00"), width));
            AppendGroup(lines, width, "Famílias", report.Families);
            AppendGroup(lines, width, "Pagamentos", report.Payments);
        });
    }

    private static void AppendGroup(List<string> lines, int width, string title, IReadOnlyList<PosCashReportLine> rows)
    {
        if (rows.Count == 0)
        {
            return;
        }

        lines.Add(string.Empty);
        lines.Add(title.ToUpperInvariant());
        foreach (var row in rows)
        {
            var right = $"{row.Quantity:0.##}  {row.Total:0.00}";
            lines.Add(Pair(string.IsNullOrWhiteSpace(row.Name) ? "—" : row.Name, right, width));
        }
    }

    private static byte[] RenderLines(Action<List<string>> fill)
    {
        var lines = new List<string>();
        fill(lines);
        lines.Add(string.Empty);
        lines.Add(string.Empty);
        var text = new StringBuilder();
        foreach (var line in lines)
        {
            text.AppendLine(line);
        }

        var payload = new List<byte> { 0x1B, 0x40, 0x1B, 0x74, 0x03 };
        payload.AddRange(Cp860.GetBytes(text.ToString()));
        payload.AddRange([0x1B, 0x64, 0x04, 0x1D, 0x56, 0x00]);
        return payload.ToArray();
    }

    private static string Pair(string left, string right, int width)
    {
        if (left.Length + right.Length + 1 > width)
        {
            var keep = Math.Max(0, width - right.Length - 1);
            left = keep == 0 ? string.Empty : left[..keep];
        }

        return left.PadRight(Math.Max(left.Length, width - right.Length)) + right;
    }
}
