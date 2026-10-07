using LogicPOS.Domain.Results;

namespace LogicPOS.Application.Features.System;

public interface IDatabaseBackupService
{
    public Task<Result> BackupDatabaseAsync(CancellationToken ct = default);
    public void EnsureSqliteRestore();
    Task<Result> RestoreAsync(CancellationToken ct = default);
}