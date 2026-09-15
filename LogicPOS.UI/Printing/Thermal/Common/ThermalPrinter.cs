using ESC_POS_USB_NET.Enums;
using ESC_POS_USB_NET.Printer;
using LogicPOS.Api.Features.Company;
using LogicPOS.Globalization;
using LogicPOS.UI.Application.Services;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicPOS.UI.Printing
{
    /// <summary>
    /// Base thermal template (legacy ThermalPrinterBaseTemplate).
    /// </summary>
    public abstract class ThermalPrinter
    {
        private static readonly byte[] EscSelectFontA = { 27, (byte)'M', 0 };
        private static readonly byte[] EscSelectFontB = { 27, (byte)'M', 1 };

        protected readonly Printer _printer;
        protected readonly ThermalLayout Layout;
        protected readonly int MaxCharsPerLineNormal;
        protected readonly int MaxCharsPerLineNormalBold;
        protected readonly int MaxCharsPerLineSmall;
        protected ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public ThermalPrinter(Printer printer)
        {
            _printer = printer;
            Layout = ThermalPrinterTarget.CurrentLayout;
            MaxCharsPerLineNormal = Layout.Columns;
            MaxCharsPerLineNormalBold = Layout.ColumnsBold;
            MaxCharsPerLineSmall = Layout.ColumnsSmall;
        }

        public abstract void Print();

        protected CompanyInformation GetCompanyInformations()
        {
            return CompanyDetailsService.CompanyInformation;
        }

        protected void ResetPrintModes()
        {
            _printer.Append(new byte[] { 0x1B, 0x21, 0x00 });
            _printer.Append(EscSelectFontA);
            _printer.NormalLineHeight();
            _printer.NormalWidth();
            _printer.ExpandedMode(PrinterModeState.Off);
            _printer.AlignLeft();
        }

        protected void SetFontNormal() => _printer.Append(EscSelectFontA);

        protected void SetFontSmall() => _printer.Append(EscSelectFontB);

        protected void LineFeed() => _printer.NewLine();

        protected void BlankSeparator() => _printer.NewLine();

        /// <summary>Legacy alias used by kitchen/cash/worksession printers.</summary>
        protected void PrintHeader() => PrintCompanyHeader(isOrder: true);

        protected void WriteLine(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            _printer.Append(ToThermalText(text));
        }

        protected void WriteLineBold(string text)
        {
            AppendBoldLine(_printer, text);
        }

        protected void WriteLineBig(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            ResetPrintModes();
            _printer.AlignCenter();
            _printer.DoubleWidth2();
            _printer.ExpandedMode(PrinterModeState.On);
            AppendBoldLine(_printer, text);
            ResetPrintModes();
        }

        protected void WriteLineDoubleHeightBold(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            ResetPrintModes();
            _printer.AlignCenter();
            _printer.ExpandedMode(PrinterModeState.On);
            AppendBoldLine(_printer, text);
            ResetPrintModes();
        }

        protected void WriteLineSmall(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            SetFontSmall();
            _printer.Append(ToThermalText(text));
            SetFontNormal();
        }

        protected void WriteLabeledLine(string label, string value, bool skipWhenEmpty = true)
        {
            if (skipWhenEmpty && string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            WriteLine($"{label}: {value}");
        }

        protected void PrintRasterImage(Bitmap image)
        {
            if (image == null)
            {
                return;
            }

            ThermalRasterImage.Print(_printer, image, Layout.ImageDots);
        }

        protected Bitmap GetCompanyLogo()
        {
            var base64Logo = GetCompanyInformations().LogoBmp;

            if (string.IsNullOrEmpty(base64Logo))
            {
                return null;
            }
            if (IsBase64String(base64Logo))
            {
                var bytes = Convert.FromBase64String(base64Logo);
                return new Bitmap(new MemoryStream(bytes));
            }

            if (File.Exists(base64Logo))
            {
                return new Bitmap(base64Logo);
            }

            return null;
        }

        public static bool IsBase64String(string base64)
        {
            base64 = base64.Trim();
            return (base64.Length % 4 == 0) &&
                   !base64.Contains(" ") &&
                   base64.All(c => char.IsLetterOrDigit(c) ||
                                    c == '+' ||
                                    c == '/' ||
                                    c == '=');
        }

        /// <summary>
        /// Bold via ESC E + Append (printer code page). Do not use BoldMode(string):
        /// the library encodes that path as CP850 while Append uses IBM860.
        /// </summary>
        public static void AppendBoldLine(Printer printer, string text)
        {
            if (printer == null)
            {
                return;
            }

            var line = ToThermalText(text ?? string.Empty);
            printer.BoldMode(PrinterModeState.On);
            if (string.IsNullOrEmpty(line))
            {
                printer.NewLine();
            }
            else
            {
                printer.Append(line);
            }

            printer.BoldMode(PrinterModeState.Off);
        }

        /// <summary>
        /// Keeps Portuguese accents (ã, õ, â, ç, …) for IBM860; drops only glyphs the code page cannot represent.
        /// </summary>
        public static string ToThermalText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var normalized = text.Normalize(NormalizationForm.FormC);
            var bytes = ThermalCodePage.GetBytes(normalized);
            return ThermalCodePage.GetString(bytes);
        }

        private static readonly Encoding ThermalCodePage = Encoding.GetEncoding(
            860,
            new EncoderReplacementFallback("?"),
            new DecoderReplacementFallback("?"));

        /// <summary>Legacy PrintHeader(isOrder:false) for invoices.</summary>
        protected void PrintCompanyHeader(bool isOrder = false)
        {
            var company = GetCompanyInformations();
            var logo = GetCompanyLogo();
            var businessName = string.IsNullOrEmpty(company.BusinessName) ? company.Name : company.BusinessName;

            _printer.AlignCenter();

            if (logo != null && !PreferenceParametersService.PrintComercialName)
            {
                PrintRasterImage(logo);
                logo.Dispose();
                ResetPrintModes();
                _printer.AlignCenter();
                WriteLine(company.Name);
            }
            else if (isOrder)
            {
                WriteLineDoubleHeightBold(businessName);
            }
            else if (!PreferenceParametersService.PrintComercialName)
            {
                WriteLine(company.Name);
            }
            else if (!string.IsNullOrEmpty(businessName) && businessName.Length > 20)
            {
                WriteLineDoubleHeightBold(businessName);
                _printer.AlignCenter();
                WriteLine(company.Name);
            }
            else
            {
                WriteLineBig(businessName);
                _printer.AlignCenter();
                WriteLine(company.Name);
            }

            ResetPrintModes();
            LineFeed();
        }

        protected void PrintTitles(string title, string subTitle)
        {
            _printer.AlignCenter();
            if (!string.IsNullOrEmpty(title))
            {
                WriteLineBig(title);
            }
            if (!string.IsNullOrEmpty(subTitle))
            {
                WriteLineDoubleHeightBold(subTitle);
            }
            LineFeed();
            ResetPrintModes();
        }

        protected void PrintStandardFooter()
        {
            _printer.AlignCenter();
            SetFontSmall();
            WriteLine($"{AuthenticationService.User.Name} - {TerminalService.Terminal.Designation}");
            LineFeed();
            WriteLine(string.Format("{1}: {2}{0}{3}: {4} {5}",
                Environment.NewLine,
                LocalizedString.Instance["global_printed_on_date"],
                DateTime.Now.ToLocalTime(),
                "LogicPulse",
                "LogicPOS",
                SystemVersionService.PosVersion));
            SetFontNormal();
            LineFeed();
            ResetPrintModes();
        }
    }
}
