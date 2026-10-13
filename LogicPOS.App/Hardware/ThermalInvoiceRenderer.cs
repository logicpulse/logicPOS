using System.Globalization;
using System.Text;
using LogicPOS.Core.FrontOffice;

namespace LogicPOS.App.Hardware;

/// <summary>
/// ESC/POS layout of the GTK thermal finance document (LogicPOS.UI InvoicePrinter).
/// </summary>
internal sealed class ThermalInvoiceRenderer
{
    private const int TablePadding = 2;
    private static readonly HashSet<string> InvoiceTypes = new(StringComparer.OrdinalIgnoreCase) { "FT", "FS", "FR", "FC" };

    private readonly List<byte> _buffer = [];
    private readonly Encoding _encoding;
    private readonly ThermalInvoiceJob _job;
    private readonly int _columns;
    private readonly int _columnsBold;
    private readonly int _columnsSmall;
    private readonly bool _narrow;
    private readonly bool _isSecondCopy;
    private bool _smallFont;
    private bool _doubleWidth;

    private ThermalInvoiceRenderer(ThermalInvoiceJob job, bool isSecondCopy)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _encoding = Encoding.GetEncoding(860, new EncoderReplacementFallback("?"), new DecoderReplacementFallback("?"));
        _job = job;
        _isSecondCopy = isSecondCopy || job.IsSecondCopy;
        _columns = job.Printer.ColumnsNormal > 0 ? job.Printer.ColumnsNormal : ThermalPrinterSettings.DefaultColumnsNormal;
        _columnsBold = job.Printer.ColumnsBold > 0 ? job.Printer.ColumnsBold : ThermalPrinterSettings.DefaultColumnsBold;
        _columnsSmall = job.Printer.ColumnsSmall > 0 ? job.Printer.ColumnsSmall : ThermalPrinterSettings.DefaultColumnsSmall;

        // 60 mm and 58 mm rolls cannot fit the six-column detail table
        _narrow = job.Printer.PaperWidthMm <= 60;
    }

    private int LineWidth => _doubleWidth ? Math.Max(8, _columns / 2) : _smallFont ? _columnsSmall : _columns;

    public static byte[] Render(ThermalInvoiceJob job, bool isSecondCopy = false)
    {
        var renderer = new ThermalInvoiceRenderer(job, isSecondCopy);
        renderer.Print();
        return renderer._buffer.ToArray();
    }

    private static string L(string key, string fallback) => PreferenceLabels.Text(key, fallback);

    private void Print()
    {
        Raw(0x1B, 0x40);
        Raw(0x1B, 0x74, 0x03);
        PrintCompanyHeader();
        PrintExtendedHeader();
        PrintDocumentMaster();
        PrintCustomer();
        PrintDetails();
        PrintTotals();
        PrintTaxResume();
        PrintPayments();
        PrintTypeFooter();
        PrintQrCode();
        PrintFooterExtended();
        PrintStandardFooter();
        Raw(0x1B, 0x64, 0x04);
        Raw(0x1D, 0x56, 0x00);
    }

    private void PrintCompanyHeader()
    {
        var company = _job.Company;
        var businessName = string.IsNullOrWhiteSpace(company.BusinessName) ? company.Name : company.BusinessName;
        Reset();
        Center();
        if (string.IsNullOrWhiteSpace(businessName) == false && businessName != company.Name)
        {
            Big(businessName, doubleWidth: businessName.Length <= _columns / 2);
            Center();
        }

        Line(company.Name);
        Reset();
        Feed();
    }

    private void PrintExtendedHeader()
    {
        var company = _job.Company;
        Reset();
        Small();
        Line(company.Address);
        Line($"{(string.IsNullOrWhiteSpace(company.PostalCode) ? "0000-000" : company.PostalCode)} {company.City} - {company.CountryCode2}");
        if (string.IsNullOrWhiteSpace(company.Phone) == false)
        {
            Line($"{L("prefparam_company_telephone", "Telefone")}: {company.Phone}");
        }

        if (string.IsNullOrWhiteSpace(company.MobilePhone) == false)
        {
            Line($"{L("global_mobile_phone", "Telemóvel")}: {company.MobilePhone}");
        }

        if (string.IsNullOrWhiteSpace(company.Email) == false)
        {
            Line($"{L("global_email", "Email")}: {company.Email}");
        }

        Line(company.Website);
        Line($"{L("prefparam_company_fiscalnumber", "NIF")}: {company.FiscalNumber}");
        Normal();
        Feed();
    }

    private void PrintDocumentMaster()
    {
        var document = _job.Document;
        var prefix = document.Number.Length >= 2 ? document.Number[..2].ToLowerInvariant() : document.Type.ToLowerInvariant();
        var typeTitle = L($"global_documentfinance_type_title_{(prefix == "cm" ? "dc" : prefix)}", document.Type);
        Reset();
        Center();
        Big(typeTitle, doubleWidth: typeTitle.Length <= _columns / 2);
        Center();
        Bold(document.Number);
        Center();
        Line(_isSecondCopy
            ? L("global_print_copy_title2", "2ª Via")
            : L("global_print_copy_title1", "Original"));
        if (_isSecondCopy)
        {
            Center();
            Line(DateTime.Now.ToString("d", CultureInfo.CurrentCulture));
        }

        Line(document.Date.ToString("d", CultureInfo.CurrentCulture));
        Feed();
        Reset();
    }

    private void PrintCustomer()
    {
        var document = _job.Document;
        Line($"{L("global_customer", "Cliente")}: {document.CustomerName}");
        Line($"{L("global_address", "Morada")}: {document.CustomerAddress}");
        var place = string.Join(" ", new[] { document.CustomerZipCode, document.CustomerCity }.Where(item => string.IsNullOrWhiteSpace(item) == false));
        Line(string.IsNullOrWhiteSpace(place) ? document.CustomerCountry : $"{place} - {document.CustomerCountry}");
        if (string.IsNullOrWhiteSpace(document.CustomerFiscalNumber) == false)
        {
            Line($"{L("global_fiscal_number", "NIF")}: {document.CustomerFiscalNumber}");
        }

        Feed();
    }

    private void PrintDetails()
    {
        if (_narrow)
        {
            PrintCompactDetails();
            return;
        }

        var width = Math.Max(16, _columns - TablePadding);
        DetailWidths(width, out var vat, out var qty, out var unit, out var price, out var discount);
        var total = Math.Max(4, width - vat - qty - unit - price - discount);
        var padding = new string(' ', TablePadding);

        Line(padding
            + Cell(L("global_vat_rate", "IVA") + "%", vat)
            + Cell(L("global_quantity_acronym", "Qnt"), qty)
            + Cell(" " + L("global_unit_measure_acronym", "UM"), unit, right: false)
            + Cell(L("global_price", "Preço"), price)
            + Cell(L("global_discount_acronym", "Desc") + "%", discount)
            + Cell(L("global_total_per_item", "Total"), total));

        foreach (var detail in _job.Document.Lines)
        {
            Bold(detail.Designation);
            Line(padding
                + Cell(detail.Tax.ToString("00.00", CultureInfo.CurrentCulture), vat)
                + Cell(detail.Quantity.ToString("0.00", CultureInfo.CurrentCulture), qty)
                + Cell(detail.Unit ?? string.Empty, unit)
                + Cell(detail.UnitPrice.ToString("0.00", CultureInfo.CurrentCulture), price)
                + Cell(detail.Discount.ToString("0.00", CultureInfo.CurrentCulture), discount)
                + Cell(detail.TotalFinal.ToString("0.00", CultureInfo.CurrentCulture), total));
            if (string.IsNullOrWhiteSpace(detail.VatExemptionReason) == false)
            {
                Small();
                Line(padding + detail.VatExemptionReason);
                Normal();
            }
        }

        Feed();
    }

    private void PrintCompactDetails()
    {
        // Narrow rolls: designation, then "qty x price  VAT%" with the line total flush right
        var vatLabel = L("global_vat_rate", "IVA");
        var discountLabel = L("global_discount_acronym", "Desc");
        foreach (var detail in _job.Document.Lines)
        {
            Bold(detail.Designation);
            var left = $"{detail.Quantity.ToString("0.##", CultureInfo.CurrentCulture)}{(string.IsNullOrWhiteSpace(detail.Unit) ? string.Empty : " " + detail.Unit)}"
                + $" x {detail.UnitPrice.ToString("0.00", CultureInfo.CurrentCulture)}"
                + $" {vatLabel} {detail.Tax.ToString("0.##", CultureInfo.CurrentCulture)}%";
            if (detail.Discount != 0m)
            {
                left += $" {discountLabel} {detail.Discount.ToString("0.##", CultureInfo.CurrentCulture)}%";
            }

            var total = detail.TotalFinal.ToString("0.00", CultureInfo.CurrentCulture);
            if (left.Length + 1 + total.Length > _columns)
            {
                Line(left);
                Line(total.PadLeft(_columns));
            }
            else
            {
                Line(left.PadRight(_columns - total.Length) + total);
            }

            if (string.IsNullOrWhiteSpace(detail.VatExemptionReason) == false)
            {
                Small();
                Line(detail.VatExemptionReason);
                Normal();
            }
        }

        Feed();
    }

    private static void DetailWidths(int width, out int vat, out int qty, out int unit, out int price, out int discount)
    {
        var budget = Math.Max(10, width - Math.Max(4, width / 5));
        (vat, qty, unit, price, discount) = (6, 8, 4, 11, 6);
        if (vat + qty + unit + price + discount <= budget)
        {
            return;
        }

        (vat, qty, unit, price, discount) = (6, 7, 3, 8, 6);
        if (vat + qty + unit + price + discount <= budget)
        {
            return;
        }

        (vat, qty, unit, price, discount) = (5, 6, 2, 8, 5);
        if (vat + qty + unit + price + discount <= budget)
        {
            return;
        }

        (vat, qty, unit, price, discount) = (4, 5, 2, 7, 4);
        if (vat + qty + unit + price + discount <= budget)
        {
            return;
        }

        (vat, qty, unit, price, discount) = (3, 4, 2, 5, 3);
    }

    private void PrintTotals()
    {
        var document = _job.Document;
        foreach (var (label, value) in new[]
        {
            (L("global_totalnet", "Total Ilíquido"), document.TotalNet),
            (L("global_documentfinance_totaltax", "Total Imposto"), document.TotalTax),
            (L("global_documentfinance_totalfinal", "Total Final"), document.TotalFinal)
        })
        {
            var text = value.ToString("F2", CultureInfo.CurrentCulture);
            var labelWidth = Math.Max(1, _columns - text.Length);
            Bold((label.Length > labelWidth ? label[..labelWidth] : label).PadRight(labelWidth) + text);
        }

        Feed();
    }

    private void PrintTaxResume()
    {
        var (tax, taxBase, total) = _job.Printer.PaperWidthMm >= 80 ? (8, 12, 10) : (8, 10, 9);
        if (_narrow)
        {
            // Rate, base and tax share the whole roll; the designation column does not fit
            taxBase = (_columns - tax) / 2;
            total = _columns - tax - taxBase;
        }

        var designation = Math.Max(0, _columns - tax - taxBase - total);
        Line(Cell(L("global_designation", "Designação"), designation, right: false)
            + Cell(L("global_tax", "Taxa"), tax)
            + Cell(L("global_total_tax_base", "Base"), taxBase)
            + Cell(L("global_documentfinance_totaltax_acronym", "Imp."), total));
        foreach (var group in _job.Document.Lines.GroupBy(line => line.TaxDesignation ?? string.Empty))
        {
            Line(Cell(group.Key, designation, right: false)
                + Cell($"{group.First().Tax.ToString("F2", CultureInfo.CurrentCulture)}%", tax)
                + Cell(group.Sum(line => line.TotalNet).ToString("F2", CultureInfo.CurrentCulture), taxBase)
                + Cell(group.Sum(line => line.TotalTax).ToString("F2", CultureInfo.CurrentCulture), total));
        }

        Feed();
    }

    private void PrintPayments()
    {
        var document = _job.Document;
        Center();
        PaymentLine(L("global_payment_conditions", "Condições de Pagamento"), document.PaymentCondition);
        foreach (var method in document.PaymentMethods)
        {
            PaymentLine(L("global_payment_method_field", "Método de Pagamento"), method);
        }

        PaymentLine(L("global_currency_field", "Moeda"), document.Currency);
        Feed();
    }

    private void PaymentLine(string label, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        Line($"{label.Trim().TrimEnd(':').Trim()}: {value}");
    }

    private void PrintTypeFooter()
    {
        var key = InvoiceTypes.Contains(_job.Document.Type)
            ? "global_documentfinance_type_report_invoice_footer_at"
            : "global_documentfinance_type_report_non_invoice_footer_at";
        Center();
        Line(L(key, string.Empty));
        Feed();
    }

    private void PrintQrCode()
    {
        var document = _job.Document;

        // GTK always prints ATCUD on PT tickets (before QR), independent of PRINT_QRCODE.
        if (string.IsNullOrWhiteSpace(document.Atcud) == false)
        {
            Center();
            Small();
            Line($"ATCUD: {document.Atcud}");
            Normal();
            Feed();
        }

        if (_job.PrintQrCode == false)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(document.FiscalCodeLine) == false)
        {
            Center();
            Line(document.FiscalCodeLine);
            Feed();
        }

        var content = string.IsNullOrWhiteSpace(document.AtQrCode) ? document.Number : document.AtQrCode;
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        Center();
        // QRCODE_METHOD: 0 = native ESC/POS, 1 = bitmap (default in seeds; works on Generic/60mm).
        if (_job.QrCodeMethod == 0)
        {
            PrintNativeQr(content);
        }
        else
        {
            PrintImageQr(content);
        }

        Reset();
        Feed();
    }

    private void PrintNativeQr(string content)
    {
        var data = Encoding.UTF8.GetBytes(content);
        var length = data.Length + 3;
        Raw(0x1D, 0x28, 0x6B, 0x04, 0x00, 0x31, 0x41, 0x32, 0x00);
        Raw(0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x43, QrModuleSize());
        Raw(0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x45, 0x31);
        Raw(0x1D, 0x28, 0x6B, (byte)(length % 256), (byte)(length / 256), 0x31, 0x50, 0x30);
        _buffer.AddRange(data);
        Raw(0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x51, 0x30);
    }

    private void PrintImageQr(string content)
    {
        var size = _job.Printer.PaperWidthMm switch
        {
            >= 80 => 256,
            >= 70 => 220,
            _ => 180
        };
        var png = ThermalQrPng.Render(content, size);
        var escPos = ThermalEscPosBitmap.FromPng(png);
        if (escPos.Length == 0)
        {
            // Fallback if Skia/ZXing fails on this machine.
            PrintNativeQr(content);
            return;
        }

        _buffer.AddRange(escPos);
    }

    private byte QrModuleSize() => _job.Printer.PaperWidthMm switch
    {
        >= 80 => 6,
        >= 70 => 5,
        _ => 4
    };

    private void PrintFooterExtended()
    {
        var company = _job.Company;
        if (string.IsNullOrWhiteSpace(company.TicketFinalLine1) && string.IsNullOrWhiteSpace(company.TicketFinalLine2))
        {
            return;
        }

        Center();
        Line(company.TicketFinalLine1);
        Line(company.TicketFinalLine2);
        Feed();
        Reset();
    }

    private void PrintStandardFooter()
    {
        Center();
        Small();
        Line(_job.TerminalName);
        Line($"{L("global_printed_on_date", "Impresso em")}: {DateTime.Now.ToString(CultureInfo.CurrentCulture)}");
        Line("LogicPulse: LogicPOS");
        if (_isSecondCopy)
        {
            Bold("2ª Via — conteúdo idêntico ao original");
        }

        Normal();
        Reset();
    }

    private static string Cell(string text, int width, bool right = true)
    {
        if (width <= 0)
        {
            return string.Empty;
        }

        text ??= string.Empty;
        if (text.Length >= width)
        {
            return text[..width];
        }

        return right ? text.PadLeft(width) : text.PadRight(width);
    }

    private void Line(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        foreach (var segment in Wrap(text.Normalize(NormalizationForm.FormC), LineWidth))
        {
            _buffer.AddRange(_encoding.GetBytes(segment));
            _buffer.Add(0x0A);
        }
    }

    private static IEnumerable<string> Wrap(string text, int width)
    {
        if (text.Length <= width)
        {
            yield return text;
            yield break;
        }

        var line = new StringBuilder();
        foreach (var word in text.Split(' '))
        {
            var rest = word;
            while (rest.Length > width)
            {
                if (line.Length > 0)
                {
                    yield return line.ToString();
                    line.Clear();
                }

                yield return rest[..width];
                rest = rest[width..];
            }

            if (line.Length > 0 && line.Length + 1 + rest.Length > width)
            {
                yield return line.ToString();
                line.Clear();
            }

            if (line.Length > 0)
            {
                line.Append(' ');
            }

            line.Append(rest);
        }

        if (line.Length > 0)
        {
            yield return line.ToString();
        }
    }

    private void Bold(string? text)
    {
        Raw(0x1B, 0x45, 0x01);
        Line(text);
        Raw(0x1B, 0x45, 0x00);
    }

    private void Big(string? text, bool doubleWidth)
    {
        Raw(0x1D, 0x21, (byte)(doubleWidth ? 0x11 : 0x01));
        _doubleWidth = doubleWidth;
        Bold(text);
        _doubleWidth = false;
        Raw(0x1D, 0x21, 0x00);
    }

    private void Feed() => _buffer.Add(0x0A);

    private void Center() => Raw(0x1B, 0x61, 0x01);

    private void Small()
    {
        _smallFont = true;
        Raw(0x1B, 0x4D, 0x01);
    }

    private void Normal()
    {
        _smallFont = false;
        Raw(0x1B, 0x4D, 0x00);
    }

    private void Reset()
    {
        _smallFont = false;
        _doubleWidth = false;
        Raw(0x1B, 0x21, 0x00);
        Raw(0x1D, 0x21, 0x00);
        Raw(0x1B, 0x45, 0x00);
        Raw(0x1B, 0x4D, 0x00);
        Raw(0x1B, 0x61, 0x00);
    }

    private void Raw(params byte[] bytes) => _buffer.AddRange(bytes);
}
