using LogicPOS.Application.Features.Finance.Documents;

namespace LogicPOS.Core.Tests;

public sealed class AtQrReceiptBreakdownTests
{
    private static AtQrPayloadInput ReceiptInput(
        string type,
        string id,
        string? atcud,
        AtQrReceiptBreakdownResult breakdown,
        string hash4,
        string issuer = "503504564",
        string customer = "999999990",
        string country = "PT")
        => new(
            issuer,
            customer,
            country,
            type,
            "N",
            "20261127",
            id,
            atcud,
            breakdown.TaxBreakdown,
            breakdown.TotalTax,
            breakdown.TotalFinal,
            hash4,
            WithholdingTaxAmount: breakdown.WithholdingTaxAmount,
            CurrencyAcronym: breakdown.CurrencyAcronym,
            ExchangeRate: breakdown.ExchangeRate);

    [Fact]
    public void FromSettledDocuments_INT_13_percent_full_payment_emits_I5_I6()
    {
        var result = AtQrReceiptBreakdown.FromSettledDocuments(new AtQrReceiptSettlement(
            ReceiptAmount: 9.04m,
            ReceiptTaxPayable: 1.04m,
            ReceiptWithholdingTaxAmount: 0m,
            SettledDocuments:
            [
                new AtQrSettledDocument(
                    AmountPaidOnThisReceipt: 9.04m,
                    DocumentTotalFinal: 9.04m,
                    DocumentTotalTax: 1.04m,
                    DocumentWithholdingTaxAmount: 0m,
                    Lines: [new AtQrSettledDocumentLine("PT", "INT", 8.00m, 1.04m)])
            ]));

        var payload = DocumentAtQrPayload.Build(ReceiptInput(
            "RC", "RC 2026A1/1", "TESTE160-1", result, "Rc13"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/1*H:TESTE160-1*I1:PT*I5:8.00*I6:1.04*N:1.04*O:9.04*Q:Rc13*R:2543",
            payload);
    }

    [Fact]
    public void FromSettledDocuments_RED_6_percent_full_payment_emits_I3_I4()
    {
        var result = AtQrReceiptBreakdown.FromSettledDocuments(new AtQrReceiptSettlement(
            ReceiptAmount: 4.24m,
            ReceiptTaxPayable: 0.24m,
            ReceiptWithholdingTaxAmount: 0m,
            SettledDocuments:
            [
                new AtQrSettledDocument(
                    4.24m, 4.24m, 0.24m, 0m,
                    [new AtQrSettledDocumentLine("PT", "RED", 4.00m, 0.24m)])
            ]));

        var payload = DocumentAtQrPayload.Build(ReceiptInput(
            "RC", "RC 2026A1/2", "TESTE160-2", result, "Rc06"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/2*H:TESTE160-2*I1:PT*I3:4.00*I4:0.24*N:0.24*O:4.24*Q:Rc06*R:2543",
            payload);
    }

    [Fact]
    public void FromSettledDocuments_multi_rate_full_payment()
    {
        var result = AtQrReceiptBreakdown.FromSettledDocuments(new AtQrReceiptSettlement(
            27.84m, 3.84m, 0m,
            SettledDocuments:
            [
                new AtQrSettledDocument(
                    27.84m, 27.84m, 3.84m, 0m,
                    [
                        new("PT", "RED", 4.00m, 0.24m),
                        new("PT", "INT", 10.00m, 1.30m),
                        new("PT", "NOR", 10.00m, 2.30m)
                    ])
            ]));

        var payload = DocumentAtQrPayload.Build(ReceiptInput(
            "RC", "RC 2026A1/3", "TESTE160-3", result, "RcMr", customer: "501442600"));

        Assert.Equal(
            "A:503504564*B:501442600*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/3*H:TESTE160-3*I1:PT*I3:4.00*I4:0.24*I5:10.00*I6:1.30*I7:10.00*I8:2.30*N:3.84*O:27.84*Q:RcMr*R:2543",
            payload);
    }

    [Fact]
    public void FromSettledDocuments_PT_AC_full_payment_uses_J_space()
    {
        var result = AtQrReceiptBreakdown.FromSettledDocuments(new AtQrReceiptSettlement(
            11.60m, 1.60m, 0m,
            SettledDocuments:
            [
                new AtQrSettledDocument(
                    11.60m, 11.60m, 1.60m, 0m,
                    [new("PT-AC", "NOR", 10.00m, 1.60m)])
            ]));

        var payload = DocumentAtQrPayload.Build(ReceiptInput(
            "RC", "RC 2026A1/4", "TESTE160-4", result, "RcAc"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/4*H:TESTE160-4*J1:PT-AC*J7:10.00*J8:1.60*N:1.60*O:11.60*Q:RcAc*R:2543",
            payload);
    }

    [Fact]
    public void FromSettledDocuments_withholding_emits_P()
    {
        var result = AtQrReceiptBreakdown.FromSettledDocuments(new AtQrReceiptSettlement(
            123.00m, 23.00m, 10.00m,
            SettledDocuments:
            [
                new AtQrSettledDocument(
                    123.00m, 123.00m, 23.00m, 10.00m,
                    [new("PT", "NOR", 100.00m, 23.00m)])
            ]));

        var payload = DocumentAtQrPayload.Build(ReceiptInput(
            "RC", "RC 2026A1/5", "TESTE160-5", result, "RcWh", customer: "501442600"));

        Assert.Equal(
            "A:503504564*B:501442600*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/5*H:TESTE160-5*I1:PT*I7:100.00*I8:23.00*N:23.00*O:123.00*P:10.00*Q:RcWh*R:2543",
            payload);
    }

    [Fact]
    public void FromSettledDocuments_partial_undiscriminated_default_uses_L()
    {
        // Spec v1.1 does not define partial split → default Undiscriminated → L.
        var result = AtQrReceiptBreakdown.FromSettledDocuments(new AtQrReceiptSettlement(
            ReceiptAmount: 50.00m,
            ReceiptTaxPayable: 0m,
            ReceiptWithholdingTaxAmount: 0m,
            SettledDocuments:
            [
                new AtQrSettledDocument(
                    AmountPaidOnThisReceipt: 50.00m,
                    DocumentTotalFinal: 123.00m,
                    DocumentTotalTax: 23.00m,
                    DocumentWithholdingTaxAmount: 0m,
                    Lines: [new("PT", "NOR", 100.00m, 23.00m)])
            ]));

        var payload = DocumentAtQrPayload.Build(ReceiptInput(
            "RC", "RC 2026A1/6", "TESTE160-6", result, "RcPa"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/6*H:TESTE160-6*I1:0*L:50.00*N:0.00*O:50.00*Q:RcPa*R:2543",
            payload);
        Assert.DoesNotContain("I7:", payload, StringComparison.Ordinal);
    }

    [Fact]
    public void FromSettledDocuments_partial_proportional_scales_buckets()
    {
        // Half of 123.00 NOR document → scale 50/123; rounding adjusted on largest net.
        var result = AtQrReceiptBreakdown.FromSettledDocuments(
            new AtQrReceiptSettlement(
                ReceiptAmount: 61.50m,
                ReceiptTaxPayable: 11.50m,
                ReceiptWithholdingTaxAmount: 0m,
                SettledDocuments:
                [
                    new AtQrSettledDocument(
                        61.50m, 123.00m, 23.00m, 0m,
                        [new("PT", "NOR", 100.00m, 23.00m)])
                ]),
            AtQrPartialPaymentAllocation.Proportional);

        var payload = DocumentAtQrPayload.Build(ReceiptInput(
            "RC", "RC 2026A1/7", "TESTE160-7", result, "RcPr"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/7*H:TESTE160-7*I1:PT*I7:50.00*I8:11.50*N:11.50*O:61.50*Q:RcPr*R:2543",
            payload);
    }

    [Fact]
    public void FromSettledDocuments_multiple_documents_aggregates_buckets()
    {
        var result = AtQrReceiptBreakdown.FromSettledDocuments(new AtQrReceiptSettlement(
            21.34m, 3.34m, 0m,
            SettledDocuments:
            [
                new AtQrSettledDocument(
                    12.30m, 12.30m, 2.30m, 0m,
                    [new("PT", "NOR", 10.00m, 2.30m)]),
                new AtQrSettledDocument(
                    9.04m, 9.04m, 1.04m, 0m,
                    [new("PT", "INT", 8.00m, 1.04m)])
            ]));

        var payload = DocumentAtQrPayload.Build(ReceiptInput(
            "RC", "RC 2026A1/8", "TESTE160-8", result, "RcMd"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/8*H:TESTE160-8*I1:PT*I5:8.00*I6:1.04*I7:10.00*I8:2.30*N:3.34*O:21.34*Q:RcMd*R:2543",
            payload);
    }

    [Fact]
    public void FromSettledDocuments_foreign_currency_converts_to_EUR_before_build()
    {
        // Document in USD at rate 0.5 → EUR halves; result already EUR so Build does not convert again.
        var result = AtQrReceiptBreakdown.FromSettledDocuments(new AtQrReceiptSettlement(
            ReceiptAmount: 24.60m,
            ReceiptTaxPayable: 4.60m,
            ReceiptWithholdingTaxAmount: 0m,
            CurrencyAcronym: "USD",
            ExchangeRate: 0.5m,
            SettledDocuments:
            [
                new AtQrSettledDocument(
                    AmountPaidOnThisReceipt: 24.60m,
                    DocumentTotalFinal: 24.60m,
                    DocumentTotalTax: 4.60m,
                    DocumentWithholdingTaxAmount: 0m,
                    Lines: [new("PT", "NOR", 20.00m, 4.60m)],
                    CurrencyAcronym: "USD",
                    ExchangeRate: 0.5m)
            ]));

        Assert.Equal("EUR", result.CurrencyAcronym);
        Assert.Equal(1m, result.ExchangeRate);
        Assert.Equal(12.30m, result.TotalFinal);
        Assert.Equal(2.30m, result.TotalTax);

        var payload = DocumentAtQrPayload.Build(ReceiptInput(
            "RG", "RG 2026A1/1", "TESTE161-1", result, "RgUs"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:RG*E:N*F:20261127*G:RG 2026A1/1*H:TESTE161-1*I1:PT*I7:10.00*I8:2.30*N:2.30*O:12.30*Q:RgUs*R:2543",
            payload);
    }
}
