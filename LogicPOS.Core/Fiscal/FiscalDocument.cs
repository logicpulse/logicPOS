namespace LogicPOS.Core.Fiscal;

/// <summary>Facts the host already has. The plugin decides what they mean for its authority.</summary>
public sealed class FiscalDocument
{
    public string Type { get; init; } = string.Empty;

    public string Number { get; init; } = string.Empty;

    public string? CountryCode { get; init; }

    public string? Hash { get; init; }

    public string? QrPayload { get; init; }

    public string? SeriesCode { get; init; }

    public string? Status { get; init; }

    public string? Atcud { get; init; }

    public string? CustomerCountry { get; init; }

    public string? IssuerFiscalNumber { get; init; }

    public decimal TotalNet { get; init; }

    public decimal TotalTax { get; init; }

    public decimal TotalFinal { get; init; }

    public DateTime Date { get; init; }

    public string? CustomerFiscalNumber { get; init; }
}

/// <summary>What the printer should add. Both fields empty means the public document is unchanged.</summary>
public sealed class FiscalPrint
{
    public string? CodeLine { get; init; }

    public string? QrPayload { get; init; }
}
