using System.Text;
using LogicPOS.App.Hardware;
using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests;

public sealed class ThermalInvoiceRendererAtcudTests
{
    [Fact]
    public void EscPos_omits_atcud_line_when_series_has_no_at_validation_code()
    {
        var bytes = ThermalInvoiceRenderer.Render(BuildJob(atcud: "-1", atDocCodeId: null));
        var text = Decode(bytes);
        Assert.DoesNotContain("ATCUD", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EscPos_prints_atcud_line_when_series_has_at_validation_code()
    {
        var bytes = ThermalInvoiceRenderer.Render(BuildJob(atcud: "ABC1234X-1", atDocCodeId: "ABC1234X"));
        var text = Decode(bytes);
        Assert.Contains("ATCUD: ABC1234X-1", text, StringComparison.Ordinal);
    }

    private static ThermalInvoiceJob BuildJob(string? atcud, string? atDocCodeId)
        => new()
        {
            Printer = new ThermalPrinterSettings(),
            PrintQrCode = false,
            Company = new ThermalCompany
            {
                Name = "Test Co",
                FiscalNumber = "123456789"
            },
            Document = new ThermalDocument
            {
                Type = "FS",
                Number = "FS 2026A1/1",
                Date = new DateTime(2026, 10, 15),
                Currency = "EUR",
                CustomerName = "Consumidor final",
                CustomerFiscalNumber = "999999990",
                Lines =
                [
                    new ThermalDocumentLine
                    {
                        Designation = "Artigo",
                        Quantity = 1,
                        UnitPrice = 1,
                        TotalNet = 1,
                        TotalTax = 0.23m,
                        TotalFinal = 1.23m,
                        Tax = 23
                    }
                ],
                TotalNet = 1,
                TotalTax = 0.23m,
                TotalFinal = 1.23m,
                Atcud = atcud,
                AtDocCodeId = atDocCodeId
            }
        };

    private static string Decode(byte[] payload)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(860).GetString(payload);
    }
}
