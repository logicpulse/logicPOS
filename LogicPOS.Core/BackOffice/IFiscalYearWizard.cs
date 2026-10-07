namespace LogicPOS.Core.BackOffice;

public interface IFiscalYearWizard
{
    Task<FiscalYearDraft> LoadAsync(CancellationToken cancellationToken = default);

    Task<FiscalYearSetupResult> CreateAsync(FiscalYearSetupRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asks the tax authority whether the stored communication account answers.
    /// </summary>
    Task<FiscalYearSetupResult> TestAtAsync(CancellationToken cancellationToken = default);
}

public sealed class FiscalYearChoice
{
    public Guid Id { get; init; }

    public string Label { get; init; } = string.Empty;

    public string Acronym { get; init; } = string.Empty;
}

public sealed class FiscalYearDraft
{
    public int Year { get; init; }

    public string Designation { get; init; } = string.Empty;

    public string Acronym { get; init; } = string.Empty;

    public bool Portugal { get; init; }

    public Guid? ActiveYearId { get; init; }

    public string? ActiveYearDesignation { get; init; }

    public IReadOnlyList<FiscalYearChoice> Terminals { get; init; } = [];

    public IReadOnlyList<FiscalYearChoice> DocumentTypes { get; init; } = [];
}

public sealed class FiscalYearSetupRequest
{
    public string Designation { get; init; } = string.Empty;

    public int Year { get; init; }

    public string Acronym { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public bool CloseCurrent { get; init; }

    public bool CreateSeries { get; init; }

    public bool SeriesForEachTerminal { get; init; }

    public bool CommunicateWithAt { get; init; }

    public IReadOnlyList<Guid> TerminalIds { get; init; } = [];

    public IReadOnlyList<Guid> DocumentTypeIds { get; init; } = [];
}

public sealed class FiscalYearSetupResult
{
    public bool Ok { get; init; }

    public string Message { get; init; } = string.Empty;

    public static FiscalYearSetupResult Done(string message) => new() { Ok = true, Message = message };

    public static FiscalYearSetupResult Fail(string message) => new() { Ok = false, Message = message };
}

public static class FiscalYearSeriesPlan
{
    public static IReadOnlyList<FiscalYearChoice> DocumentTypes(IEnumerable<FiscalYearChoice> types)
        => types.Where(type => IsInvoiceWayBill(type) == false).ToList();

    public static string Preview(string yearAcronym, bool perTerminal, IReadOnlyList<FiscalYearChoice> types, IReadOnlyList<FiscalYearChoice> terminals)
    {
        var lines = new List<string>();
        var documents = DocumentTypes(types);
        if (perTerminal)
        {
            var index = 1;
            foreach (var terminal in terminals)
            {
                lines.Add(terminal.Label);
                foreach (var type in documents)
                {
                    lines.Add($"  {type.Acronym} {yearAcronym}{index} - {type.Label}");
                }

                index++;
                lines.Add(string.Empty);
            }
        }
        else
        {
            foreach (var type in documents)
            {
                lines.Add($"{type.Acronym} {yearAcronym} - {type.Label}");
            }
        }

        return string.Join(Environment.NewLine, lines).Trim();
    }

    private static bool IsInvoiceWayBill(FiscalYearChoice type)
        => string.Equals(type.Acronym, "FT", StringComparison.OrdinalIgnoreCase)
           && type.Label.Contains("guia", StringComparison.OrdinalIgnoreCase);
}
