using LogicPOS.Application.Features.System;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Loads document + company mark and evaluates <see cref="TransportGuideCirculationRules"/>.
/// </summary>
public static class TransportGuideCirculationGate
{
    public static async Task<string?> TryGetRefusalForPortugalAsync(
        IServiceScopeFactory scopes,
        ISystemInformationService? system,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        if (system?.IsPortugal() != true || documentId == Guid.Empty)
        {
            return null;
        }

        await using var scope = scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await GetCirculationRefusalAsync(database, documentId, DatabaseStartup.UsesCloud(), cancellationToken);
    }

    public static async Task<string?> GetCirculationRefusalAsync(
        LogicPOSDbContext database,
        Guid documentId,
        bool atCommunicationAvailable,
        CancellationToken cancellationToken = default)
    {
        var row = await database.Documents.AsNoTracking()
            .Where(item => item.Id == documentId && item.IsDeleted == false)
            .Select(item => new
            {
                item.Type,
                item.ATDocCodeID,
                MovementStart = item.ShipFromAddress != null ? item.ShipFromAddress.DeliveryDate : null
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (row is null)
        {
            return null;
        }

        var obligation = await ReadObligationMarkedAsync(database, cancellationToken);
        var communicatedAt = await ReadLastSuccessfulCommunicationAsync(database, documentId, cancellationToken);

        return TransportGuideCirculationRules.CirculationRefusal(
            obligation,
            row.Type,
            row.ATDocCodeID,
            row.MovementStart,
            communicatedAt,
            atCommunicationAvailable);
    }

    public static async Task<bool> ReadObligationMarkedAsync(
        LogicPOSDbContext database,
        CancellationToken cancellationToken = default)
    {
        var value = await database.PreferenceParameters.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Token == CompanyFiscalMarkTokens.CommunicateTransportDocuments)
            .Select(item => item.Value)
            .FirstOrDefaultAsync(cancellationToken);

        return TransportGuideCirculationRules.IsObligationMarked(value);
    }

    private static async Task<DateTime?> ReadLastSuccessfulCommunicationAsync(
        LogicPOSDbContext database,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        return await database.AtAudits.AsNoTracking()
            .Where(item =>
                item.IsDeleted == false
                && item.DocumentId == documentId
                && item.ReturnCode == 0
                && string.IsNullOrWhiteSpace(item.ATDocCodeID) == false)
            .OrderByDescending(item => item.Date)
            .Select(item => (DateTime?)item.Date)
            .FirstOrDefaultAsync(cancellationToken);
    }
}