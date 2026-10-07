using LogicPOS.Domain.Results;

namespace LogicPOS.Application.Features.System;

public interface IDatabaseBackupService
{
    /// <returns>The backup file that was written.</returns>
    public Task<Result<string>> BackupDatabaseAsync(CancellationToken ct = default);
    public void EnsureSqliteRestore();
    /// <param name="backupId">A row of the SystemBackups table; null restores the latest backup.</param>
    /// <returns>The backup file that was restored.</returns>
    Task<Result<string>> RestoreAsync(Guid? backupId = null, CancellationToken ct = default);
}
