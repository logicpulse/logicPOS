using System;
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

        public int PaperWidthMm { get; }
        public int Columns { get; }
        public int ColumnsBold { get; }
        public int ColumnsSmall { get; }
        public int ImageDots { get; }

        public ThermalLayout(int columns, int columnsBold, int columnsSmall)
        {
            Columns = columns > 0 ? columns : DefaultColumns;
            ColumnsBold = columnsBold > 0 ? columnsBold : DefaultColumnsBold;
            ColumnsSmall = columnsSmall > 0 ? columnsSmall : DefaultColumnsSmall;

            // Physical paper (same axis as ESC/POS AlignCenter on footer text).
            PaperWidthMm = Columns <= 32 ? 58 : 80;
            ImageDots = PaperWidthMm <= 58 ? 384 : 576;
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

            return new ThermalLayout(normal, bold, small);
        }
    }
}
