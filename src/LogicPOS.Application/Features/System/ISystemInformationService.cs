using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Application.Features.System;

public interface ISystemInformationService
{
    /// <summary>
    /// Fiscal country. Local mode uses appsettings <c>Country</c>.
    /// Cloud mode uses the country of the current LOGICPOS account.
    /// </summary>
    public FiscalCountry Country { get; }
    public string GetCountryCode2();
    public bool IsAngola();
    public bool IsPortugal();
    public bool IsMozambique();
    public string GetCulture();
    /// <summary>Active seed/module token from DatabaseSettings (e.g. default, cafe, seafoodstore).</summary>
    public string? GetModule();
}
