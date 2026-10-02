using LogicPOS.Persistence.Interceptors;

namespace LogicPOS.Core.Authentication;

/// <summary>
/// Satisfies the persistence interceptor. This screen only reads users and does not save changes.
/// </summary>
public sealed class DesktopAuditingInformationService : IAuditingInformationService
{
    public Guid? GetUserId() => null;

    public Guid? GetTerminalId() => null;
}
