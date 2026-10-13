using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// First-run company details (GTK <c>PosEditCompanyDetails</c> / <c>InitPlataformParameters</c>).
/// </summary>
public sealed class LocalCompanySetupService : ICompanySetupService
{
    private static readonly string[] RequiredTokens =
    [
        "COMPANY_COUNTRY_OID",
        "COMPANY_COUNTRY_CODE2",
        "COMPANY_FISCALNUMBER",
        "SYSTEM_CURRENCY_OID"
    ];

    private static readonly HashSet<string> HiddenTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        "COMPANY_COUNTRY",
        "COMPANY_COUNTRY_CODE2",
        "COMPANY_COUNTRY_OID",
        "SYSTEM_CURRENCY",
        "SYSTEM_CURRENCY_OID"
    };

    private readonly IServiceScopeFactory _scopes;

    public LocalCompanySetupService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<bool> NeedsSetupAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var values = await database.PreferenceParameters.AsNoTracking()
            .Where(item => item.IsDeleted == false && RequiredTokens.Contains(item.Token))
            .Select(item => new { item.Token, item.Value })
            .ToListAsync(cancellationToken);

        foreach (var token in RequiredTokens)
        {
            var value = values.FirstOrDefault(item => item.Token == token)?.Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
        }

        return false;
    }

    public async Task<CompanySetupDraft> LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        var countries = await database.Countries.AsNoTracking()
            .Where(item => item.IsDeleted == false && string.IsNullOrWhiteSpace(item.FiscalNumberRegex) == false)
            .OrderBy(item => item.Designation)
            .Select(item => new CompanySetupChoice
            {
                Id = item.Id,
                Label = item.Designation,
                Code = item.Code2,
                FiscalNumberRegex = item.FiscalNumberRegex ?? string.Empty,
                ZipCodeRegex = item.ZipCodeRegex ?? string.Empty
            })
            .ToListAsync(cancellationToken);

        var currencies = await database.Currencies.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Designation)
            .Select(item => new CompanySetupChoice
            {
                Id = item.Id,
                Label = string.IsNullOrWhiteSpace(item.Acronym)
                    ? item.Designation
                    : $"{item.Designation} ({item.Acronym})",
                Code = item.Acronym ?? string.Empty
            })
            .ToListAsync(cancellationToken);

        // GTK PosEditCompanyDetails: FormType = 1 AND FormPageNo = 1 (company page only).
        var parameters = await database.PreferenceParameters.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.FormType == 1 && item.FormPageNo == 1)
            .OrderBy(item => item.Order)
            .ThenBy(item => item.Code)
            .ToListAsync(cancellationToken);

        var fields = parameters
            .Where(item => HiddenTokens.Contains(item.Token) == false)
            .Select(item => new CompanySetupField
            {
                Token = item.Token,
                Label = PreferenceFieldLabel(item),
                // Fresh DB: leave empty like GTK (except TAX_ENTITY / STOCK_CAPITAL defaults).
                Value = DefaultFieldValue(item),
                Required = item.Required
            })
            .ToList();

        var countryOid = await ValueAsync(database, "COMPANY_COUNTRY_OID", cancellationToken);
        var currencyOid = await ValueAsync(database, "SYSTEM_CURRENCY_OID", cancellationToken);
        Guid? countryId = Guid.TryParse(countryOid, out var country) ? country : null;
        Guid? currencyId = Guid.TryParse(currencyOid, out var currency) ? currency : null;

        if (countryId is null || countries.Any(item => item.Id == countryId) == false)
        {
            countryId = countries.FirstOrDefault(item => item.Code == "PT")?.Id
                ?? countries.FirstOrDefault()?.Id;
        }

        if (currencyId is null || currencies.Any(item => item.Id == currencyId) == false)
        {
            currencyId = currencies.FirstOrDefault(item => item.Code == "EUR")?.Id
                ?? currencies.FirstOrDefault()?.Id;
        }

        return new CompanySetupDraft
        {
            Countries = countries,
            Currencies = currencies,
            Fields = fields,
            CountryId = countryId,
            CurrencyId = currencyId
        };
    }

    public async Task<ListingSaveResult> SaveAsync(CompanySetupRequest request, CancellationToken cancellationToken = default)
    {
        if (request.CountryId == Guid.Empty || request.CurrencyId == Guid.Empty)
        {
            return ListingSaveResult.Fail("Escolha o país e a moeda.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        var country = await database.Countries.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CountryId && item.IsDeleted == false, cancellationToken);
        var currency = await database.Currencies.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CurrencyId && item.IsDeleted == false, cancellationToken);
        if (country is null || currency is null)
        {
            return ListingSaveResult.Fail("País ou moeda inválidos.");
        }

        var fiscal = request.Values.TryGetValue("COMPANY_FISCALNUMBER", out var fiscalNumber)
            ? fiscalNumber.Trim()
            : string.Empty;
        if (string.IsNullOrWhiteSpace(fiscal))
        {
            return ListingSaveResult.Fail("Indique o NIF da empresa.");
        }

        var values = new Dictionary<string, string>(request.Values, StringComparer.OrdinalIgnoreCase)
        {
            ["COMPANY_COUNTRY"] = country.Designation,
            ["COMPANY_COUNTRY_CODE2"] = country.Code2,
            ["COMPANY_COUNTRY_OID"] = country.Id.ToString(),
            ["SYSTEM_CURRENCY"] = currency.Acronym,
            ["SYSTEM_CURRENCY_OID"] = currency.Id.ToString()
        };

        var tokens = values.Keys.ToList();
        var rows = await database.PreferenceParameters
            .Where(item => item.IsDeleted == false && tokens.Contains(item.Token))
            .ToListAsync(cancellationToken);

        foreach (var pair in values)
        {
            var row = rows.FirstOrDefault(item =>
                string.Equals(item.Token, pair.Key, StringComparison.OrdinalIgnoreCase));
            if (row is null)
            {
                // Seed historically omitted *_OID rows; create them so NeedsSetup can clear (GTK has them empty).
                row = CreateMissingPreference(pair.Key, pair.Value);
                database.PreferenceParameters.Add(row);
                continue;
            }

            row.Value = pair.Value;
            row.UpdatedAt = DateTime.Now;
        }

        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(Guid.Empty, "Dados da empresa gravados.");
    }

    private static PreferenceParameter CreateMissingPreference(string token, string value)
    {
        var now = DateTime.Now;
        var meta = token.ToUpperInvariant() switch
        {
            "COMPANY_COUNTRY_OID" => (Order: 71u, Code: "71", Resource: "prefparam_company_country_oid", RegEx: "RegexGuid"),
            "SYSTEM_CURRENCY_OID" => (Order: 201u, Code: "201", Resource: "prefparam_system_currency_oid", RegEx: "RegexGuid"),
            "COMPANY_COUNTRY_CODE2" => (Order: 80u, Code: "80", Resource: "prefparam_company_country_code2", RegEx: "RegexAlfaCountryCode2"),
            "COMPANY_COUNTRY" => (Order: 70u, Code: "70", Resource: "prefparam_company_country", RegEx: "RegexAlfa"),
            "SYSTEM_CURRENCY" => (Order: 200u, Code: "200", Resource: "prefparam_system_currency", RegEx: "RegexAcronym3Chars"),
            _ => (Order: 999u, Code: "999", Resource: $"prefparam_{token.ToLowerInvariant()}", RegEx: "RegexAlfaNumericExtended")
        };

        return new PreferenceParameter
        {
            Id = Guid.NewGuid(),
            Token = token,
            Value = value,
            Required = true,
            RegEx = meta.RegEx,
            ResourceString = meta.Resource,
            FormType = 1,
            FormPageNo = null,
            InputType = PreferenceParameterInputType.Text,
            Order = meta.Order,
            Code = meta.Code,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private static async Task<string?> ValueAsync(LogicPOSDbContext database, string token, CancellationToken cancellationToken)
    {
        return await database.PreferenceParameters.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Token == token)
            .Select(item => item.Value)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static string PreferenceFieldLabel(PreferenceParameter item)
    {
        if (string.IsNullOrWhiteSpace(item.ResourceString) == false)
        {
            return item.ResourceString;
        }

        return item.Token;
    }

    private static string DefaultFieldValue(PreferenceParameter item)
    {
        // GTK PosEditCompanyDetails defaults before the user fills / Demo.
        if (string.Equals(item.Token, "COMPANY_TAX_ENTITY", StringComparison.OrdinalIgnoreCase))
        {
            return "Global";
        }

        if (string.Equals(item.Token, "COMPANY_STOCK_CAPITAL", StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(item.Value) ? "1" : item.Value!;
        }

        return string.Empty;
    }

    /// <summary>LogicPulse demo company values (GTK Demo button / useDataDemo).</summary>
    public static IReadOnlyDictionary<string, string> DemoCompanyValues { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["COMPANY_NAME"] = "LogicPulse",
            ["COMPANY_BUSINESS_NAME"] = "Technologies, Ltda",
            ["COMPANY_ADDRESS"] = "Rua Capitão Salgueiro Maia, 7",
            ["COMPANY_CITY"] = "Figueira da Foz",
            ["COMPANY_POSTALCODE"] = "3080-000",
            ["COMPANY_FISCALNUMBER"] = "999999990",
            ["COMPANY_STOCK_CAPITAL"] = "1000",
            ["COMPANY_TAX_ENTITY"] = "Global",
            ["COMPANY_EMAIL"] = "comercial@logicpulse.com",
            ["COMPANY_WEBSITE"] = "www.logicpulse.com"
        };
}
