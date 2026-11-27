using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Application.Features.Finance.Documents;

/// <summary>
/// How to allocate VAT buckets when a settled document is only partially paid.
/// AT QR spec v1.1 (Portaria 195/2020) does <em>not</em> define a partial-payment split rule;
/// product must choose. Default is the conservative <see cref="Undiscriminated"/> reading
/// (field L — “recibos gerais sem discriminação de imposto”, p. 11).
/// </summary>
public enum AtQrPartialPaymentAllocation
{
    /// <summary>
    /// No I/J/K rate split for the partial amount: contribute to field L (and I1:0 when nothing
    /// else is discriminated). Does not invent a proportional VAT breakdown.
    /// </summary>
    Undiscriminated = 0,

    /// <summary>
    /// Scale each settled document's tax buckets by <c>AmountPaid / DocumentTotalFinal</c>.
    /// Common industry practice for SAF-T payment tax lines; <strong>not</strong> stated in AT QR v1.1.
    /// </summary>
    Proportional = 1
}

/// <summary>One tax line of a settled invoice/ND, in that document's currency.</summary>
public readonly record struct AtQrSettledDocumentLine(
    string CountryRegion,
    string TaxCode,
    decimal TaxableBase,
    decimal TaxAmount);

/// <summary>
/// One document settled (fully or partially) by the receipt, with in-memory line taxes.
/// <see cref="AmountPaidOnThisReceipt"/> is the gross amount of this receipt applied to the document.
/// </summary>
public sealed record AtQrSettledDocument(
    decimal AmountPaidOnThisReceipt,
    decimal DocumentTotalFinal,
    decimal DocumentTotalTax,
    decimal DocumentWithholdingTaxAmount,
    IReadOnlyList<AtQrSettledDocumentLine> Lines,
    string? CurrencyAcronym = null,
    decimal ExchangeRate = 1m);

/// <summary>Receipt header amounts in receipt currency (no database access).</summary>
public sealed record AtQrReceiptSettlement(
    decimal ReceiptAmount,
    decimal ReceiptTaxPayable,
    decimal ReceiptWithholdingTaxAmount,
    string? CurrencyAcronym = null,
    decimal ExchangeRate = 1m,
    IReadOnlyList<AtQrSettledDocument>? SettledDocuments = null);

/// <summary>
/// Result ready to feed <see cref="AtQrPayloadInput"/>: amounts are already in EUR
/// (<see cref="CurrencyAcronym"/> = EUR, <see cref="ExchangeRate"/> = 1) so
/// <see cref="DocumentAtQrPayload.Build"/> must not convert again.
/// </summary>
public sealed record AtQrReceiptBreakdownResult(
    AtQrTaxBreakdown TaxBreakdown,
    decimal TotalTax,
    decimal TotalFinal,
    decimal WithholdingTaxAmount,
    string CurrencyAcronym,
    decimal ExchangeRate);

/// <summary>
/// Builds AT QR tax inputs for RC/RG from settled documents — pure in-memory, no database.
/// </summary>
public static class AtQrReceiptBreakdown
{
    private const decimal FullPaymentTolerance = 0.01m;

    /// <summary>
    /// Aggregates settled-document tax lines into an EUR <see cref="AtQrReceiptBreakdownResult"/>.
    /// Full settlements always use line discrimination. Partial settlements follow
    /// <paramref name="partialPaymentAllocation"/> (default <see cref="AtQrPartialPaymentAllocation.Undiscriminated"/>).
    /// </summary>
    public static AtQrReceiptBreakdownResult FromSettledDocuments(
        AtQrReceiptSettlement receipt,
        AtQrPartialPaymentAllocation partialPaymentAllocation = AtQrPartialPaymentAllocation.Undiscriminated)
    {
        ArgumentNullException.ThrowIfNull(receipt);

        var receiptAmountEur = DocumentAtQrPayload.ToEur(
            receipt.ReceiptAmount, receipt.CurrencyAcronym, receipt.ExchangeRate).Round();
        var receiptTaxEur = DocumentAtQrPayload.ToEur(
            receipt.ReceiptTaxPayable, receipt.CurrencyAcronym, receipt.ExchangeRate).Round();
        var receiptWithholdingEur = DocumentAtQrPayload.ToEur(
            receipt.ReceiptWithholdingTaxAmount, receipt.CurrencyAcronym, receipt.ExchangeRate).Round();

        var documents = receipt.SettledDocuments ?? Array.Empty<AtQrSettledDocument>();
        if (documents.Count == 0)
        {
            var net = (receiptAmountEur - receiptTaxEur).Round();
            if (net < 0m)
            {
                net = 0m;
            }

            return new AtQrReceiptBreakdownResult(
                AtQrTaxBreakdown.FromUndiscriminatedPayment(net),
                receiptTaxEur,
                receiptAmountEur,
                receiptWithholdingEur,
                "EUR",
                1m);
        }

        var lineParts = new List<(string Region, string Code, decimal Net, decimal Tax)>();
        var nonSubject = 0m;

        foreach (var document in documents)
        {
            var paidEur = DocumentAtQrPayload.ToEur(
                document.AmountPaidOnThisReceipt, document.CurrencyAcronym, document.ExchangeRate).Round();
            if (paidEur <= 0m)
            {
                continue;
            }

            var totalFinalEur = DocumentAtQrPayload.ToEur(
                document.DocumentTotalFinal, document.CurrencyAcronym, document.ExchangeRate).Round();
            var linesEur = document.Lines
                .Select(line => (
                    Region: line.CountryRegion,
                    Code: line.TaxCode,
                    Net: DocumentAtQrPayload.ToEur(line.TaxableBase, document.CurrencyAcronym, document.ExchangeRate),
                    Tax: DocumentAtQrPayload.ToEur(line.TaxAmount, document.CurrencyAcronym, document.ExchangeRate)))
                .ToList();

            var isFullPayment = totalFinalEur <= 0m
                || paidEur + FullPaymentTolerance >= totalFinalEur;

            if (isFullPayment && linesEur.Count > 0)
            {
                foreach (var line in linesEur)
                {
                    lineParts.Add((line.Region, line.Code, line.Net.Round(), line.Tax.Round()));
                }

                continue;
            }

            if (partialPaymentAllocation == AtQrPartialPaymentAllocation.Proportional
                && linesEur.Count > 0
                && totalFinalEur > 0m)
            {
                lineParts.AddRange(AllocateProportional(paidEur, totalFinalEur, linesEur));
                continue;
            }

            // Conservative: no rate discrimination for this paid slice → field L.
            nonSubject += paidEur;
        }

        AtQrTaxBreakdown breakdown;
        if (lineParts.Count == 0)
        {
            var fallbackNet = nonSubject > 0m
                ? nonSubject
                : (receiptAmountEur - receiptTaxEur).Round();
            if (fallbackNet < 0m)
            {
                fallbackNet = 0m;
            }

            breakdown = AtQrTaxBreakdown.FromUndiscriminatedPayment(fallbackNet);
        }
        else
        {
            var fromLines = AtQrTaxBreakdown.FromLines(
                lineParts.Select(part => (part.Region, part.Code, part.Net, part.Tax)));
            breakdown = fromLines with { NonSubjectAmount = (fromLines.NonSubjectAmount + nonSubject).Round() };
        }

        return new AtQrReceiptBreakdownResult(
            breakdown,
            receiptTaxEur,
            receiptAmountEur,
            receiptWithholdingEur,
            "EUR",
            1m);
    }

    /// <summary>
    /// Scales line nets/taxes by <c>paid / documentTotal</c>, rounds each to 2 decimals
    /// (<see cref="RoundingExtension.Round"/>), then adjusts the largest net bucket so
    /// Σ(net)+Σ(tax) equals the paid gross (EUR, 2 dp).
    /// </summary>
    private static List<(string Region, string Code, decimal Net, decimal Tax)> AllocateProportional(
        decimal paidEur,
        decimal totalFinalEur,
        List<(string Region, string Code, decimal Net, decimal Tax)> linesEur)
    {
        var scale = paidEur / totalFinalEur;
        var scaled = linesEur
            .Select(line => (
                line.Region,
                line.Code,
                Net: (line.Net * scale).Round(),
                Tax: (line.Tax * scale).Round()))
            .ToList();

        var sumNet = scaled.Sum(item => item.Net);
        var sumTax = scaled.Sum(item => item.Tax);
        var delta = (paidEur - (sumNet + sumTax)).Round();
        if (delta == 0m || scaled.Count == 0)
        {
            return scaled;
        }

        // Prefer adjusting the largest taxable base so N/O stay coherent with the receipt gross.
        var index = 0;
        var best = scaled[0].Net;
        for (var i = 1; i < scaled.Count; i++)
        {
            if (scaled[i].Net > best)
            {
                best = scaled[i].Net;
                index = i;
            }
        }

        var chosen = scaled[index];
        scaled[index] = (chosen.Region, chosen.Code, (chosen.Net + delta).Round(), chosen.Tax);
        return scaled;
    }
}
