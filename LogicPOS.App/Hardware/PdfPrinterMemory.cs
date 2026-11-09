using System.Text.Json;
using LogicPOS.Core.BackOffice;

namespace LogicPOS.App.Hardware;

/// <summary>Remembers last Windows printer for A4 and per barcode-label template.</summary>
internal static class PdfPrinterMemory
{
    private static readonly object Gate = new();
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static State? _state;

    public static string? LastA4Printer()
    {
        lock (Gate)
        {
            return Normalize(Load().LastA4Printer);
        }
    }

    public static string? PrinterForTemplate(string? template)
    {
        var key = BarcodeLabelPdf.NormalizeModel(template);
        lock (Gate)
        {
            var map = Load().TemplatePrinters;
            return map.TryGetValue(key, out var printer) ? Normalize(printer) : null;
        }
    }

    public static void RememberA4(string? printer)
    {
        var value = Normalize(printer);
        if (value is null)
        {
            return;
        }

        lock (Gate)
        {
            var state = Load();
            if (string.Equals(state.LastA4Printer, value, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            state.LastA4Printer = value;
            Save(state);
        }
    }

    public static void RememberTemplate(string? template, string? printer)
    {
        var value = Normalize(printer);
        if (value is null)
        {
            return;
        }

        var key = BarcodeLabelPdf.NormalizeModel(template);
        lock (Gate)
        {
            var state = Load();
            if (state.TemplatePrinters.TryGetValue(key, out var current)
                && string.Equals(current, value, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            state.TemplatePrinters[key] = value;
            Save(state);
        }
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static State Load()
    {
        if (_state is not null)
        {
            return _state;
        }

        try
        {
            var path = StorePath();
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                _state = JsonSerializer.Deserialize<State>(json, JsonOptions) ?? new State();
                _state.TemplatePrinters ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return _state;
            }
        }
        catch
        {
            // Ignore corrupt memory and start fresh.
        }

        _state = new State();
        return _state;
    }

    private static void Save(State state)
    {
        _state = state;
        try
        {
            var path = StorePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(state, JsonOptions));
        }
        catch
        {
            // Preference is best-effort.
        }
    }

    private static string StorePath()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Logicpulse",
            "logicpos",
            "pdf-printer-memory.json");

    private sealed class State
    {
        public string? LastA4Printer { get; set; }

        public Dictionary<string, string> TemplatePrinters { get; set; } =
            new(StringComparer.OrdinalIgnoreCase);
    }
}
