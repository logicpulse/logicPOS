using System.IO.Ports;
using System.Text;
using LogicPOS.Core;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Hardware;

/// <summary>
/// Writes the current line and total to the terminal pole display (COM port).
/// Does nothing when the terminal has no display configured.
/// </summary>
internal static class CustomerDisplay
{
    private static string? _lastText;

    public static void Show(PosTicketLine? line, decimal total, bool standby)
    {
        string line1;
        string line2;
        if (standby || line is null)
        {
            line1 = string.Empty;
            line2 = string.Empty;
        }
        else
        {
            line1 = Pair($"{line.Quantity:0.###} x {line.Designation}", line.DisplayUnitPrice.ToString("0.00"), 20);
            line2 = Pair("Total", total.ToString("0.00"), 20);
        }

        var key = standby || line is null ? "standby" : line1 + "|" + line2;
        if (key == _lastText)
        {
            return;
        }

        _lastText = key;
        _ = WriteAsync(line1, line2, standby || line is null);
    }

    private static async Task WriteAsync(string line1, string? line2, bool standby)
    {
        try
        {
            var source = AppComposition.Services?.GetService<IThermalPrintSource>();
            if (source is null)
            {
                return;
            }

            var settings = await source.GetCustomerDisplayAsync();
            if (settings is null || string.IsNullOrWhiteSpace(settings.ComPort))
            {
                return;
            }

            var columns = Math.Max(8, settings.Columns);
            var first = standby ? settings.StandByLine1 ?? string.Empty : line1;
            var second = standby ? settings.StandByLine2 ?? string.Empty : line2 ?? string.Empty;
            var text = Fit(RemoveAccents(first), columns) + Fit(RemoveAccents(second), columns);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var payload = new byte[] { 0x0C }.Concat(Encoding.GetEncoding(860).GetBytes(text)).ToArray();
            using var port = new SerialPort(settings.ComPort.Trim(), 9600, Parity.None, 8, StopBits.One)
            {
                WriteTimeout = 500
            };
            port.Open();
            port.Write(payload, 0, payload.Length);
        }
        catch
        {
            _lastText = null;
        }
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

    private static string Fit(string text, int columns)
        => text.Length > columns ? text[..columns] : text.PadRight(columns);

    private static string RemoveAccents(string text)
    {
        var decomposed = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var letter in decomposed)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(letter) != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                builder.Append(letter);
            }
        }

        return builder.ToString();
    }
}
