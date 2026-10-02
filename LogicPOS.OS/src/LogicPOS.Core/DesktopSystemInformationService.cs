using LogicPOS.Application.Features.System;
using LogicPOS.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace LogicPOS.Core;

public sealed class DesktopSystemInformationService : ISystemInformationService
{
    private const string DefaultCountry = "PT";

    public DesktopSystemInformationService(IConfiguration configuration)
    {
        Country = FiscalCountry.Parse(configuration["Country"] ?? DefaultCountry);
        Culture = configuration["Culture"] ?? "pt-PT";
        Module = configuration["DatabaseSettings:Module"];
    }

    public FiscalCountry Country { get; }

    public string Culture { get; }

    public string? Module { get; }

    public string GetCountryCode2() => Country.Code2.ToLowerInvariant();

    public bool IsAngola() => Country.IsAngola;

    public bool IsPortugal() => Country.IsPortugal;

    public bool IsMozambique() => Country.IsMozambique;

    public string GetCulture() => Culture;

    public string? GetModule() => string.IsNullOrWhiteSpace(Module) ? null : Module.Trim();
}
