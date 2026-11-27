using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Shared.Features.Software;

namespace LogicPOS.Core.Tests;

public sealed class DocumentAtQrPayloadTests
{
    private static string HashWithFourChars(char a, char b, char c, char d)
    {
        var chars = new char[40];
        Array.Fill(chars, 'x');
        chars[0] = a;
        chars[10] = b;
        chars[20] = c;
        chars[30] = d;
        return new string(chars);
    }

    private static string Hash4(char a, char b, char c, char d)
        => DocumentCertificationText.ExtractHash4Chars(HashWithFourChars(a, b, c, d));

    private static AtQrPayloadInput Input(
        string type,
        string id,
        string? atcud,
        AtQrTaxBreakdown tax,
        decimal totalTax,
        decimal totalFinal,
        string hash4,
        string issuer = "503504564",
        string customer = "999999990",
        string country = "PT",
        string status = "N",
        string date = "20261127",
        decimal withholding = 0m,
        decimal stamp = 0m,
        string? currency = null,
        decimal rate = 1m,
        string? other = null,
        string? certificate = null)
        => new(
            issuer,
            customer,
            country,
            type,
            status,
            date,
            id,
            atcud,
            tax,
            totalTax,
            totalFinal,
            hash4,
            withholding,
            stamp,
            currency,
            rate,
            other,
            certificate);

    [Fact]
    public void Build_matches_spec_example_2_simplified_invoice()
    {
        var payload = DocumentAtQrPayload.Build(new AtQrPayloadInput(
            "123456789", "999999990", "PT", "FS", "N", "2019-08-12",
            "FS CDVF/12345", "CDF7T5HD-12345",
            new AtQrTaxBreakdown([new AtQrTaxBucket("PT", "NOR", 0.65m, 0.15m)]),
            0.15m, 0.80m, "YhGV",
            OtherInfo: "NU;0.80",
            CertificateNumber: "9999"));

        Assert.Equal(
            "A:123456789*B:999999990*C:PT*D:FS*E:N*F:20190812*G:FS CDVF/12345*H:CDF7T5HD-12345*I1:PT*I7:0.65*I8:0.15*N:0.15*O:0.80*Q:YhGV*R:9999*S:NU;0.80",
            payload);
        Assert.False(payload.EndsWith('*'));
    }

    [Fact]
    public void Build_matches_spec_example_1_multi_region()
    {
        var tax = AtQrTaxBreakdown.FromLines(
        [
            ("PT", "ISE", 12000.00m, 0m),
            ("PT", "RED", 15000.00m, 900.00m),
            ("PT", "INT", 50000.00m, 6500.00m),
            ("PT", "NOR", 80000.00m, 18400.00m),
            ("PT-AC", "ISE", 10000.00m, 0m),
            ("PT-AC", "RED", 25000.56m, 1000.02m),
            ("PT-AC", "INT", 75000.00m, 6750.00m),
            ("PT-AC", "NOR", 100000.00m, 18000.00m),
            ("PT-MA", "ISE", 5000.00m, 0m),
            ("PT-MA", "RED", 12500.00m, 625.00m),
            ("PT-MA", "INT", 25000.00m, 3000.00m),
            ("PT-MA", "NOR", 40000.00m, 8800.00m)
        ]);

        var payload = DocumentAtQrPayload.Build(new AtQrPayloadInput(
            "123456789", "999999990", "PT", "FT", "N", "20191231",
            "FT AB2019/0035", "CSDF7T5H-0035",
            tax with { NonSubjectAmount = 100.00m },
            64000.02m, 513600.58m, "kLp0",
            WithholdingTaxAmount: 100.00m,
            StampDutyAmount: 25.00m,
            OtherInfo: "TB;PT00000000000000000000000;513500.58",
            CertificateNumber: "9999"));

        Assert.Equal(
            "A:123456789*B:999999990*C:PT*D:FT*E:N*F:20191231*G:FT AB2019/0035*H:CSDF7T5H-0035*I1:PT*I2:12000.00*I3:15000.00*I4:900.00*I5:50000.00*I6:6500.00*I7:80000.00*I8:18400.00*J1:PT-AC*J2:10000.00*J3:25000.56*J4:1000.02*J5:75000.00*J6:6750.00*J7:100000.00*J8:18000.00*K1:PT-MA*K2:5000.00*K3:12500.00*K4:625.00*K5:25000.00*K6:3000.00*K7:40000.00*K8:8800.00*L:100.00*M:25.00*N:64000.02*O:513600.58*P:100.00*Q:kLp0*R:9999*S:TB;PT00000000000000000000000;513500.58",
            payload);
    }

    [Fact]
    public void Build_matches_spec_example_3_proforma()
    {
        var payload = DocumentAtQrPayload.Build(new AtQrPayloadInput(
            "500000000", "123456789", "PT", "PF", "N", "20190123",
            "PF G2019CB/145789", "HB6FT7RV-145789",
            AtQrTaxBreakdown.FromLines(
            [
                ("PT", "ISE", 12345.34m, 0m),
                ("PT", "RED", 12532.65m, 751.96m),
                ("PT", "INT", 52789.00m, 6862.57m),
                ("PT", "NOR", 32425.69m, 7457.91m)
            ]),
            15072.44m, 125165.12m, "r/fY",
            CertificateNumber: "9999"));

        Assert.Equal(
            "A:500000000*B:123456789*C:PT*D:PF*E:N*F:20190123*G:PF G2019CB/145789*H:HB6FT7RV-145789*I1:PT*I2:12345.34*I3:12532.65*I4:751.96*I5:52789.00*I6:6862.57*I7:32425.69*I8:7457.91*N:15072.44*O:125165.12*Q:r/fY*R:9999",
            payload);
    }

    [Fact]
    public void Build_matches_spec_example_4_transport_without_vat()
    {
        var payload = DocumentAtQrPayload.Build(new AtQrPayloadInput(
            "500000000", "123456789", "PT", "GT", "N", "20190720",
            "GT G234CB/50987", "GTVX4Y8B-50987",
            AtQrTaxBreakdown.Empty,
            0m, 0m, "5uIg",
            CertificateNumber: "9999"));

        Assert.Equal(
            "A:500000000*B:123456789*C:PT*D:GT*E:N*F:20190720*G:GT G234CB/50987*H:GTVX4Y8B-50987*I1:0*N:0.00*O:0.00*Q:5uIg*R:9999",
            payload);
    }

    [Fact]
    public void Build_matches_spec_example_5_foreign_tax_rate()
    {
        var payload = DocumentAtQrPayload.Build(new AtQrPayloadInput(
            "123456789", "4443332215", "FR", "FT", "N", "20190526",
            "ABC BNH/4561", "DK5ZJ2HN-4561",
            new AtQrTaxBreakdown([new AtQrTaxBucket("FR", "NOR", 100.00m, 20.00m)]),
            20.00m, 120.00m, "YJRE",
            CertificateNumber: "9999"));

        Assert.Equal(
            "A:123456789*B:4443332215*C:FR*D:FT*E:N*F:20190526*G:ABC BNH/4561*H:DK5ZJ2HN-4561*I1:FR*I7:100.00*I8:20.00*N:20.00*O:120.00*Q:YJRE*R:9999",
            payload);
    }

    [Fact]
    public void Build_matches_spec_example_6_debit_note_pt_ma_tax_only()
    {
        // Spec PDF places the sole PT-MA space in I1; agreed mapping (reviewer/probe) assigns PT-MA → K.
        var payload = DocumentAtQrPayload.Build(new AtQrPayloadInput(
            "123456789", "500000000", "PT", "ND", "N", "20190216",
            "M1F KLG/6145", "RQD8L6DG-6145",
            new AtQrTaxBreakdown([new AtQrTaxBucket("PT-MA", "INT", 0m, 26.50m)]),
            26.50m, 26.50m, "h1rB",
            CertificateNumber: "9999"));

        Assert.Equal(
            "A:123456789*B:500000000*C:PT*D:ND*E:N*F:20190216*G:M1F KLG/6145*H:RQD8L6DG-6145*K1:PT-MA*K6:26.50*N:26.50*O:26.50*Q:h1rB*R:9999",
            payload);
    }

    [Fact]
    public void Probe_FS_at_13_percent_uses_I5_I6()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "FS", "FS 2026A1/2", "TESTE005-2",
            new AtQrTaxBreakdown([new("PT", "INT", 8.00m, 1.04m)]),
            1.04m, 9.04m, "TvnM"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:FS*E:N*F:20261127*G:FS 2026A1/2*H:TESTE005-2*I1:PT*I5:8.00*I6:1.04*N:1.04*O:9.04*Q:TvnM*R:2543",
            payload);
    }

    [Fact]
    public void Probe_exempt_uses_I2()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "FS", "FS 2026A1/4", "TESTE005-4",
            new AtQrTaxBreakdown([new("PT", "ISE", 10.00m, 0m)]),
            0m, 10.00m, "dpxh"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:FS*E:N*F:20261127*G:FS 2026A1/4*H:TESTE005-4*I1:PT*I2:10.00*N:0.00*O:10.00*Q:dpxh*R:2543",
            payload);
    }

    [Fact]
    public void Probe_PT_AC_at_16_percent_uses_J_space()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "FS", "FS 2026A1/3", "TESTE005-3",
            new AtQrTaxBreakdown([new("PT-AC", "NOR", 10.00m, 1.60m)]),
            1.60m, 11.60m, "dZwX"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:FS*E:N*F:20261127*G:FS 2026A1/3*H:TESTE005-3*J1:PT-AC*J7:10.00*J8:1.60*N:1.60*O:11.60*Q:dZwX*R:2543",
            payload);
    }

    [Fact]
    public void Probe_consumidor_final_keeps_B_999999990()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "FS", "FS 2026A1/1", "TESTE005-1",
            new AtQrTaxBreakdown([new("PT", "NOR", 10.00m, 2.30m)]),
            2.30m, 12.30m, "CCw7"));

        Assert.Contains("B:999999990*", payload, StringComparison.Ordinal);
        Assert.False(payload.EndsWith('*'));
    }

    [Fact]
    public void Probe_credit_note_keeps_positive_totals_and_rate_split()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "NC", "NC 2026A1/1", "TESTE012-1",
            AtQrTaxBreakdown.FromLines(
            [
                ("PT", "RED", 4.00m, 0.24m),
                ("PT", "INT", 10.00m, 1.30m),
                ("PT", "NOR", 10.00m, 2.30m)
            ]),
            3.84m, 27.84m, "a0qc",
            customer: "501442600"));

        Assert.Equal(
            "A:503504564*B:501442600*C:PT*D:NC*E:N*F:20261127*G:NC 2026A1/1*H:TESTE012-1*I1:PT*I3:4.00*I4:0.24*I5:10.00*I6:1.30*I7:10.00*I8:2.30*N:3.84*O:27.84*Q:a0qc*R:2543",
            payload);
    }

    [Fact]
    public void Multi_rate_document_splits_I3_to_I8()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "FT", "FT 2026A1/1", "TESTE004-1",
            AtQrTaxBreakdown.FromLines(
            [
                ("PT", "RED", 4.00m, 0.24m),
                ("PT", "INT", 10.00m, 1.30m),
                ("PT", "NOR", 10.00m, 2.30m)
            ]),
            3.84m, 27.84m, "izkB",
            customer: "501442600"));

        Assert.Contains("I3:4.00*I4:0.24*I5:10.00*I6:1.30*I7:10.00*I8:2.30", payload, StringComparison.Ordinal);
    }

    [Fact]
    public void Mixed_PT_and_PT_AC_emits_I_and_J_spaces()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "FT", "FT MIX/1", "MIXCODE-1",
            AtQrTaxBreakdown.FromLines(
            [
                ("PT", "NOR", 10.00m, 2.30m),
                ("PT-AC", "NOR", 10.00m, 1.60m)
            ]),
            3.90m, 23.90m, "AbCd",
            customer: "501442600"));

        Assert.Contains("I1:PT*I7:10.00*I8:2.30*J1:PT-AC*J7:10.00*J8:1.60", payload, StringComparison.Ordinal);
    }

    [Fact]
    public void Non_EUR_document_converts_amounts_with_exchange_rate()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "FS", "FS USD/1", "TESTE005-9",
            new AtQrTaxBreakdown([new("PT", "NOR", 10.00m, 2.30m)]),
            2.30m, 12.30m, "UuSs",
            currency: "USD",
            rate: 0.5m));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:FS*E:N*F:20261127*G:FS USD/1*H:TESTE005-9*I1:PT*I7:5.00*I8:1.15*N:1.15*O:6.15*Q:UuSs*R:2543",
            payload);
        Assert.Equal(5.00m, DocumentAtQrPayload.ToEur(10.00m, "USD", 0.5m));
    }

    [Fact]
    public void Withholding_emits_P()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "FT", "FT WH/1", "WHCODE-1",
            new AtQrTaxBreakdown([new("PT", "NOR", 100.00m, 23.00m)]),
            23.00m, 123.00m, "WhTh",
            customer: "501442600",
            withholding: 10.00m));

        Assert.Contains("*P:10.00*Q:WhTh*", payload, StringComparison.Ordinal);
    }

    [Fact]
    public void Missing_atcud_uses_H_0()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "FS", "FS 2026A1/9", null,
            new AtQrTaxBreakdown([new("PT", "NOR", 10.00m, 2.30m)]),
            2.30m, 12.30m, "NoAt"));

        Assert.Contains("*H:0*", payload, StringComparison.Ordinal);
    }

    [Fact]
    public void Payload_never_ends_with_asterisk()
    {
        var payload = DocumentAtQrPayload.Build(Input(
            "FS", "FS 2026A1/1", "TESTE005-1",
            new AtQrTaxBreakdown([new("PT", "NOR", 10.00m, 2.30m)]),
            2.30m, 12.30m, "CCw7"));

        Assert.False(payload.EndsWith('*'));
    }

    /// <summary>
    /// Spec v1.1 §4 field D (PaymentType), G (PaymentRefNo), pp. 5–6; field L “recibos gerais
    /// sem discriminação de imposto”, p. 11; I1:0 when no VAT rate table, §3h / field I1, pp. 4–7.
    /// Conservative reading when only invoicesPaid/TaxPayable exist: I1:0 + L(net) + N(tax) + O(gross).
    /// </summary>
    [Fact]
    public void Build_RC_undiscriminated_payment_uses_I1_0_and_L()
    {
        // API today: invoicesPaid=123, TaxPayable=23 → net 100 without rate codes.
        var net = 100.00m;
        var tax = 23.00m;
        var gross = 123.00m;
        var payload = DocumentAtQrPayload.Build(new AtQrPayloadInput(
            "503504564", "501442600", "PT", "RC", "N", "20261127",
            "RC 2026A1/1", "TESTE160-1",
            AtQrTaxBreakdown.FromUndiscriminatedPayment(net),
            tax, gross, "RcH4"));

        Assert.Equal(
            "A:503504564*B:501442600*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/1*H:TESTE160-1*I1:0*L:100.00*N:23.00*O:123.00*Q:RcH4*R:2543",
            payload);
        Assert.DoesNotContain("I7:", payload, StringComparison.Ordinal);
    }

    [Fact]
    public void Build_RG_undiscriminated_payment_uses_I1_0_and_L()
    {
        var payload = DocumentAtQrPayload.Build(new AtQrPayloadInput(
            "503504564", "999999990", "PT", "RG", "N", "20261127",
            "RG 2026A1/1", "TESTE161-1",
            AtQrTaxBreakdown.FromUndiscriminatedPayment(50.00m),
            0m, 50.00m, "RgH4"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:RG*E:N*F:20261127*G:RG 2026A1/1*H:TESTE161-1*I1:0*L:50.00*N:0.00*O:50.00*Q:RgH4*R:2543",
            payload);
    }

    [Fact]
    public void Build_RC_settling_multiple_documents_aggregates_undiscriminated_totals()
    {
        // Two settled invoices, API only has summed invoicesPaid / TaxPayable (no per-rate lines).
        var invoicesPaid = 12.30m + 9.04m;
        var taxPayable = 2.30m + 1.04m;
        var net = invoicesPaid - taxPayable;
        var payload = DocumentAtQrPayload.Build(new AtQrPayloadInput(
            "503504564", "999999990", "PT", "RC", "N", "20261127",
            "RC 2026A1/2", "TESTE160-2",
            AtQrTaxBreakdown.FromUndiscriminatedPayment(net),
            taxPayable, invoicesPaid, "RcM2"));

        Assert.Equal(
            "A:503504564*B:999999990*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/2*H:TESTE160-2*I1:0*L:18.00*N:3.34*O:21.34*Q:RcM2*R:2543",
            payload);
    }

    [Fact]
    public void Build_RC_with_line_breakdown_from_settled_invoices_emits_I_spaces()
    {
        // Preferred path when Desktop/API can pass settled-invoice line taxes.
        var payload = DocumentAtQrPayload.Build(new AtQrPayloadInput(
            "503504564", "501442600", "PT", "RC", "N", "20261127",
            "RC 2026A1/3", "TESTE160-3",
            AtQrTaxBreakdown.FromLines(
            [
                ("PT", "NOR", 10.00m, 2.30m),
                ("PT", "INT", 8.00m, 1.04m)
            ]),
            3.34m, 21.34m, "RcLn"));

        Assert.Equal(
            "A:503504564*B:501442600*C:PT*D:RC*E:N*F:20261127*G:RC 2026A1/3*H:TESTE160-3*I1:PT*I5:8.00*I6:1.04*I7:10.00*I8:2.30*N:3.34*O:21.34*Q:RcLn*R:2543",
            payload);
    }

#pragma warning disable CS0618
    [Fact]
    public void Obsolete_overload_still_builds_NOR_payload_without_trailing_separator()
    {
        var payload = DocumentAtQrPayload.Build(
            issuerFiscalNumber: "502757191",
            customerFiscalNumber: "999999990",
            customerCountry: "PT",
            documentType: "FS",
            status: "N",
            dateYyyyMmDd: "2019-08-12",
            uniqueDocumentId: "FS CDVF/12345",
            atcud: "CDF7T5HD-12345",
            totalNet: 0.65m,
            totalTax: 0.15m,
            totalFinal: 0.80m,
            hash4: Hash4('Y', 'h', 'G', 'V'));

        Assert.Equal(
            "A:502757191*B:999999990*C:PT*D:FS*E:N*F:20190812*G:FS CDVF/12345*H:CDF7T5HD-12345*I1:PT*I7:0.65*I8:0.15*N:0.15*O:0.80*Q:YhGV*R:2543",
            payload);
        Assert.False(payload.EndsWith('*'));
    }

    [Fact]
    public void Obsolete_AggregateTaxTotals_delegates_to_FromLines()
    {
        var buckets = DocumentAtQrPayload.AggregateTaxTotals(
            [
                ("PT", "NOR", 5m, 1.15m),
                ("PT", "NOR", 5m, 1.15m),
                ("PT", "NS", 3m, 0m)
            ],
            out var nonSubject);

        Assert.Equal(3m, nonSubject);
        Assert.Contains(buckets, t => t is { CountryRegion: "PT", TaxCode: "NOR", TaxableBase: 10m, TaxAmount: 2.30m });
    }
#pragma warning restore CS0618

    [Fact]
    public void FromLines_groups_by_region_and_code()
    {
        var breakdown = AtQrTaxBreakdown.FromLines(
        [
            ("PT", "NOR", 5m, 1.15m),
            ("PT", "NOR", 5m, 1.15m),
            ("PT", "INT", 8m, 1.04m),
            ("PT-AC", "NOR", 10m, 1.60m),
            ("PT", "NS", 3m, 0m)
        ]);

        Assert.Equal(3m, breakdown.NonSubjectAmount);
        Assert.Contains(breakdown.Buckets, t => t is { CountryRegion: "PT", TaxCode: "NOR", TaxableBase: 10m, TaxAmount: 2.30m });
        Assert.Contains(breakdown.Buckets, t => t is { CountryRegion: "PT", TaxCode: "INT", TaxableBase: 8m, TaxAmount: 1.04m });
        Assert.Contains(breakdown.Buckets, t => t is { CountryRegion: "PT-AC", TaxCode: "NOR", TaxableBase: 10m, TaxAmount: 1.60m });
    }

    [Fact]
    public void Certification_line_includes_hash4_and_AT_certificate_number()
    {
        var hash = HashWithFourChars('A', 'B', 'C', 'D');
        var line = DocumentCertificationText.Build(
            isPortugal: true,
            isAngola: false,
            isMozambique: false,
            documentHash: hash);

        Assert.Equal($"ABCD - Processado por programa certificado Nº {SoftwareInfo.Portugal.SaftCertificateNumber}/AT", line);
        Assert.Contains("Nº", line, StringComparison.Ordinal);
        Assert.Contains("/AT", line, StringComparison.Ordinal);
    }
}
