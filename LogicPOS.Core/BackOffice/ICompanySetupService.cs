namespace LogicPOS.Core.BackOffice;

public sealed class CompanySetupField
{
    public required string Token { get; init; }
    public required string Label { get; init; }
    public string Value { get; init; } = string.Empty;
    public bool Required { get; init; }
}

public sealed class CompanySetupChoice
{
    public Guid Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string FiscalNumberRegex { get; init; } = string.Empty;
    public string ZipCodeRegex { get; init; } = string.Empty;
}

public sealed class CompanySetupDraft
{
    public IReadOnlyList<CompanySetupChoice> Countries { get; init; } = [];
    public IReadOnlyList<CompanySetupChoice> Currencies { get; init; } = [];
    public IReadOnlyList<CompanySetupField> Fields { get; init; } = [];
    public Guid? CountryId { get; init; }
    public Guid? CurrencyId { get; init; }
}

public sealed class CompanySetupRequest
{
    public Guid CountryId { get; init; }
    public Guid CurrencyId { get; init; }
    public IReadOnlyDictionary<string, string> Values { get; init; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}

public interface ICompanySetupService
{
    Task<bool> NeedsSetupAsync(CancellationToken cancellationToken = default);

    Task<CompanySetupDraft> LoadAsync(CancellationToken cancellationToken = default);

    Task<ListingSaveResult> SaveAsync(CompanySetupRequest request, CancellationToken cancellationToken = default);
}
