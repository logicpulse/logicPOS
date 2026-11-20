using LogicPOS.Core.Fiscal;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.FrontOffice;

public interface ICompanyFiscalMarkService
{
    Task<CompanyFiscalMarkState> LoadAsync(CancellationToken cancellationToken = default);
}

public sealed class CompanyFiscalMarkService : ICompanyFiscalMarkService
{
    private readonly IServiceScopeFactory _scopes;

    public CompanyFiscalMarkService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<CompanyFiscalMarkState> LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var parameters = await database.PreferenceParameters
            .AsNoTracking()
            .Where(item => item.IsDeleted == false
                && (item.Token == CompanyFiscalMarkTokens.RegimeArtigo53
                    || item.Token == CompanyFiscalMarkTokens.VendedorAmbulante))
            .Select(item => new { item.Token, item.Value })
            .ToListAsync(cancellationToken);
        return CompanyFiscalMarks.Read(parameters.Select(item => (item.Token, item.Value)));
    }
}