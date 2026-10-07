namespace LogicPOS.Application.Features.System;

public interface IDatabaseSeeder
{
    public void ApplyAdditionalSeed();
    public bool ApplyRequiredSeed();
    public void ApplyNewSeed();
    public void ApplyRequiredUsersSeed();
    public void EnsureCountrySpecificConfiguration();

    /// <summary>
    /// When a restored database has no portal identity, assigns the login "admin" to one proprietor.
    /// </summary>
    public void EnsurePortalLogin();
}