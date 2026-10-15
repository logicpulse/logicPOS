using System.Globalization;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities.Finance.Calculations;

public readonly record struct DocumentLineTotals(decimal TotalGross,
                                                 decimal TotalDiscount,
                                                 decimal TotalNet,
                                                 decimal TotalTax)
{
    public decimal TotalFinal => TotalNet + TotalTax;
}

/// <summary>
/// Single source for line amounts. Values keep the stored precision (6 decimals); AGT lines are
/// rounded further (net truncated, tax rounded up to the next cent, as required by the AGT e-invoicing spec).
/// </summary>
public static class DocumentLineCalculator
{
    public static decimal ToNetUnitPrice(decimal price, decimal vatPercentage, bool priceWithVat)
    {
        if (!priceWithVat || vatPercentage <= 0)
        {
            return price;
        }

        return price / (1 + vatPercentage / 100m);
    }

    public static decimal CombineDiscounts(decimal lineDiscount, decimal globalDiscount)
    {
        return 100m - (100m - lineDiscount) * (100m - globalDiscount) / 100m;
    }

    /// <summary>Inverse of <see cref="CombineDiscounts"/>, at stored precision (6 decimals).</summary>
    public static decimal ExtractLineDiscount(decimal combinedDiscount, decimal globalDiscount)
    {
        if (globalDiscount >= 100m)
        {
            return 0m;
        }

        return Math.Round(100m - (100m - combinedDiscount) * 100m / (100m - globalDiscount), RoundingExtension.StoredDecimals);
    }

    public static DocumentLineTotals CalculateLine(decimal unitPrice,
                                                   bool priceWithVat,
                                                   decimal quantity,
                                                   decimal lineDiscount,
                                                   decimal globalDiscount,
                                                   decimal vatPercentage,
                                                   bool useAgtRounding = false)
    {
        return Calculate(ToNetUnitPrice(unitPrice, vatPercentage, priceWithVat),
                         quantity,
                         CombineDiscounts(lineDiscount, globalDiscount),
                         vatPercentage,
                         useAgtRounding);
    }

    /// <summary>Foreign currency amount that settles the full debt (legacy rate: foreign = system × rate).</summary>
    public static decimal FullCurrencyAmount(decimal toPay, decimal exchangeRate)
        => exchangeRate > 0 ? (toPay * exchangeRate).Round() : toPay;

    /// <summary>System-currency receipt amount for a foreign currency payment.</summary>
    public static decimal ReceiptAmount(decimal toPay, decimal currencyAmount, decimal exchangeRate)
    {
        if (currencyAmount >= FullCurrencyAmount(toPay, exchangeRate))
        {
            return toPay;
        }

        return Math.Min(toPay, exchangeRate > 0 ? (currencyAmount / exchangeRate).Round() : currencyAmount.Round());
    }

    public static DocumentLineTotals Calculate(decimal netUnitPrice,
                                               decimal quantity,
                                               decimal discount,
                                               decimal vatPercentage,
                                               bool useAgtRounding = false)
    {
        var totalGross = netUnitPrice * quantity;
        var totalDiscount = totalGross * discount / 100m;
        var totalNet = totalGross - totalDiscount;
        decimal totalTax;

        if (useAgtRounding)
        {
            totalNet = totalNet.TruncateDecimal();
            totalTax = (totalNet * vatPercentage / 100m).ApplyAgtRounding();
        }
        else
        {
            // Cutting the final instead of the tax keeps price-with-VAT lines on the shelf total (0.075, not 0.074999).
            var totalFinal = (totalNet + totalNet * vatPercentage / 100m).ToStoredPrecision();
            totalNet = totalNet.ToStoredPrecision();
            totalTax = totalFinal - totalNet;
        }

        return new DocumentLineTotals(totalGross.ToStoredPrecision(),
                                      totalDiscount.ToStoredPrecision(),
                                      totalNet,
                                      totalTax);
    }

    /// <summary>
    /// The rate is foreign units per company unit (legacy 1.4): currency amount = amount × rate.
    /// Both sides have cents, so either conversion may be the one that lands within one cent.
    /// </summary>
    public static bool PaymentMatchesCurrency(decimal amount, decimal currencyAmount, decimal exchangeRate)
    {
        if (exchangeRate <= 0)
        {
            return false;
        }

        return Math.Abs((amount * exchangeRate).Round() - currencyAmount.Round()) <= 0.01m
               || Math.Abs((currencyAmount / exchangeRate).Round() - amount.Round()) <= 0.01m;
    }

    public const decimal ServiceWithholdingRate = 6.5m;

    /// <summary>Imposto Industrial in the SAF-T AO and AGT <c>WithholdingTaxType</c> lists.</summary>
    public const string ServiceWithholdingTaxType = "II";

    public static string ServiceWithholdingDescription =>
        $"Retenção na fonte de Imposto Industrial ({ServiceWithholdingRate.ToString(CultureInfo.GetCultureInfo("pt-PT"))}%)";

    /// <summary>Pure 6.5% math (caller must already know withholding applies).</summary>
    public static decimal ServiceWithholding(decimal servicesNet)
    {
        return (servicesNet * ServiceWithholdingRate / 100m).Round();
    }

    /// <summary>
    /// Write-path amount for industrial service withholding.
    /// Always 0 outside Angola — PT/MZ must never persist a positive value.
    /// </summary>
    public static decimal ServiceWithholding(FiscalCountry country, decimal servicesNet)
        => country.IsAngola ? ServiceWithholding(servicesNet) : 0m;

    /// <summary>
    /// AO industrial withholding (6.5% II) is only valid for Angola.
    /// Non-Angola fiscal countries must ignore any stored amount (legacy bad data).
    /// </summary>
    public static decimal EffectiveServiceWithholding(FiscalCountry country, decimal storedAmount)
        => country.IsAngola ? storedAmount : 0m;

    /// <summary>
    /// Tax settled once <paramref name="paid"/> of the document is paid. Partial payments take the difference between
    /// two calls, so the receipts of a fully paid document add up to exactly its rounded tax.
    /// </summary>
    public static decimal TaxPaidUpTo(decimal totalFinal, decimal totalTax, decimal paid)
    {
        var final = totalFinal.Round();
        if (final <= 0 || paid <= 0)
        {
            return 0;
        }

        return paid >= final ? totalTax.Round() : (totalTax * paid / final).Round();
    }

    public static decimal TaxForPayment(decimal totalFinal, decimal totalTax, decimal paidBefore, decimal amount)
    {
        return TaxPaidUpTo(totalFinal, totalTax, paidBefore + amount) - TaxPaidUpTo(totalFinal, totalTax, paidBefore);
    }

    /// <summary>Most a credit note may still credit on its parent, compared in cents against the note's rounded total.</summary>
    public static decimal CreditableAmount(decimal parentTotalFinal, decimal parentTotalPaid)
    {
        return (parentTotalFinal - parentTotalPaid).Round();
    }

    public static bool CreditNoteFits(decimal creditNoteTotalFinal, decimal creditableAmount)
    {
        return creditNoteTotalFinal.Round() <= creditableAmount;
    }

    /// <summary>
    /// Credit a receipt can still take from credit notes of a parent. The parent's paid total already counts its
    /// credit notes, so only what it was paid beyond its total is still owed back to the customer.
    /// </summary>
    public static decimal CreditNotesAvailableOnReceipt(decimal parentTotalFinal, decimal parentTotalPaid)
    {
        return Math.Max(0, (parentTotalPaid - parentTotalFinal).Round());
    }

    /// <summary>Amount a receipt can settle: each document is rounded before adding, as it is paid.</summary>
    public static decimal SettlementAmount(IEnumerable<decimal> invoicesToPay, IEnumerable<decimal> creditNotesTotal)
    {
        return (invoicesToPay.Sum(amount => amount.Round()) - creditNotesTotal.Sum(amount => amount.Round())).Round();
    }
}
