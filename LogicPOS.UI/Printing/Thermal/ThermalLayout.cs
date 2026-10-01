using System;
using System.Collections.Concurrent;
using LogicPOS.UI.Settings;
using Microsoft.Win32;
using ApiPrinter = LogicPOS.Api.Entities.Printer;

namespace LogicPOS.UI.Printing
{
    /// <summary>
    /// Printable area for thermal ESC/POS printers.
    /// Legacy LogicPOS: per-printer ThermalMaxCharsPerLine* (DB), defaults 48/44/64.
    /// </summary>
    public sealed class ThermalLayout
    {
        public const int DefaultColumns = ApiPrinter.DefaultThermalMaxCharsPerLineNormal;
        public const int DefaultColumnsBold = ApiPrinter.DefaultThermalMaxCharsPerLineNormalBold;
        public const int DefaultColumnsSmall = ApiPrinter.DefaultThermalMaxCharsPerLineSmall;

        /// <summary>
        /// Custom printers (K3, KUBE…) print across the whole 80mm paper, so raw ESC/POS output
        /// starts at the paper edge and the first characters are clipped.
        /// </summary>
        private const int CustomPrinterLeftMarginDots = 32;

        /// <summary>
        /// Custom printers cut without feeding the last printed lines past the cutter (~25mm).
        /// </summary>
        private const int CustomPrinterCutFeedDots = 200;

        private static readonly ConcurrentDictionary<string, string> DriverNames =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public int PaperWidthMm { get; }
        public int Columns { get; }
        public int ColumnsBold { get; }
        public int ColumnsSmall { get; }
        public int ImageDots { get; }
        public int LeftMarginDots { get; }
        public int CutFeedDots { get; }

        public ThermalLayout(int columns, int columnsBold, int columnsSmall, int leftMarginDots = 0, int cutFeedDots = 0)
        {
            Columns = columns > 0 ? columns : DefaultColumns;
            ColumnsBold = columnsBold > 0 ? columnsBold : DefaultColumnsBold;
            ColumnsSmall = columnsSmall > 0 ? columnsSmall : DefaultColumnsSmall;
            LeftMarginDots = Math.Max(0, Math.Min(leftMarginDots, 255));
            CutFeedDots = Math.Max(0, Math.Min(cutFeedDots, 255));

            // Physical paper (same axis as ESC/POS AlignCenter on footer text).
            PaperWidthMm = Columns <= 32 ? 58 : 80;
            ImageDots = PaperWidthMm <= 58 ? 384 : 576;

            if (LeftMarginDots > 0)
            {
                // The margin takes space from the line: shrink columns (font A ≥ 12 dots, font B ≥ 9 dots).
                Columns = Math.Max(16, Columns - CeilingDivide(LeftMarginDots, 12));
                ColumnsBold = Math.Max(16, ColumnsBold - CeilingDivide(LeftMarginDots, 12));
                ColumnsSmall = Math.Max(16, ColumnsSmall - CeilingDivide(LeftMarginDots, 9));
                ImageDots = Math.Max(128, (ImageDots - LeftMarginDots) / 8 * 8);
            }
        }

        public static ThermalLayout Resolve(ApiPrinter printer = null)
        {
            var normal = printer?.ThermalMaxCharsPerLineNormal.GetValueOrDefault() ?? 0;
            if (normal <= 0)
            {
                normal = DefaultColumns;
            }

            var bold = printer?.ThermalMaxCharsPerLineNormalBold.GetValueOrDefault() ?? 0;
            if (bold <= 0)
            {
                bold = DefaultColumnsBold;
            }

            var small = printer?.ThermalMaxCharsPerLineSmall.GetValueOrDefault() ?? 0;
            if (small <= 0)
            {
                small = DefaultColumnsSmall;
            }

            var isCustomPrinter = IsCustomPrinter(printer);

            return new ThermalLayout(normal,
                                     bold,
                                     small,
                                     ResolveLeftMarginDots(printer, normal, isCustomPrinter),
                                     isCustomPrinter ? CustomPrinterCutFeedDots : 0);
        }

        private static int ResolveLeftMarginDots(ApiPrinter printer, int columns, bool isCustomPrinter)
        {
            if (TryGetConfiguredLeftMarginDots(printer, out var configured))
            {
                return configured;
            }

            return columns > 32 && isCustomPrinter ? CustomPrinterLeftMarginDots : 0;
        }

        private static bool TryGetConfiguredLeftMarginDots(ApiPrinter printer, out int dots)
        {
            dots = 0;
            var margins = AppSettings.Instance.ThermalLeftMarginDots;
            var designation = printer?.Designation?.Trim();
            if (margins == null || string.IsNullOrEmpty(designation))
            {
                return false;
            }

            foreach (var margin in margins)
            {
                if (string.Equals(margin.Key?.Trim(), designation, StringComparison.OrdinalIgnoreCase))
                {
                    dots = margin.Value;
                    return true;
                }
            }

            return false;
        }

        private static bool IsCustomPrinter(ApiPrinter printer)
        {
            if (printer == null)
            {
                return false;
            }

            var designation = printer.Designation?.Trim() ?? string.Empty;
            var names = $"{designation} {printer.NetworkName} {GetWindowsDriverName(designation)}".ToLowerInvariant();

            return names.Contains("custom") || names.Contains("k3") || names.Contains("kube");
        }

        private static string GetWindowsDriverName(string printerName)
        {
            if (string.IsNullOrWhiteSpace(printerName) || printerName.Contains("\\"))
            {
                return string.Empty;
            }

            return DriverNames.GetOrAdd(printerName, name =>
            {
                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Control\Print\Printers\{name}"))
                    {
                        return key?.GetValue("Printer Driver") as string ?? string.Empty;
                    }
                }
                catch
                {
                    return string.Empty;
                }
            });
        }

        private static int CeilingDivide(int value, int divisor) => (value + divisor - 1) / divisor;
    }
}
