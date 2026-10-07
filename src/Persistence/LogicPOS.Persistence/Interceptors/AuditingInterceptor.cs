using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LogicPOS.Persistence.Interceptors;

public class AuditingInterceptor : SaveChangesInterceptor
{
    private readonly IAuditingInformationService _auditingInformationService;

    public AuditingInterceptor(IAuditingInformationService auditingInformationService)
    {
        _auditingInformationService = auditingInformationService;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
                                                                          InterceptionResult<int> result,
                                                                          CancellationToken cancellationToken = default)
    {
        foreach (var entry in eventData.Context!.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property("CreatedBy").CurrentValue = _auditingInformationService.GetUserId() ?? entry.Property("CreatedBy").CurrentValue;
                    entry.Property("CreatedWhere").CurrentValue = _auditingInformationService.GetTerminalId() ?? entry.Property("CreatedWhere").CurrentValue;
                    break;
                
                case EntityState.Modified:
                    entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
                    entry.Property("UpdatedBy").CurrentValue = _auditingInformationService.GetUserId() ?? entry.Property("UpdatedBy").CurrentValue ;
                    entry.Property("UpdatedWhere").CurrentValue = _auditingInformationService.GetTerminalId() ?? entry.Property("UpdatedWhere").CurrentValue;

                    if (entry.Metadata.FindProperty("IsDeleted") is not null
                        && entry.Property("IsDeleted").IsModified
                        && entry.Property("IsDeleted").CurrentValue is true
                        && entry.Property("IsDeleted").OriginalValue is false)
                    {
                        entry.Property("DeletedAt").CurrentValue = DateTime.Now;
                    }
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}