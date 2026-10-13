using LogicPOS.Core;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Authentication;

/// <summary>
/// Supplies current user/terminal for EF auditing on the desktop host.
/// Terminal is resolved from the machine hardware id (claimed at login).
/// </summary>
public sealed class DesktopAuditingInformationService : IAuditingInformationService
{
    private readonly IServiceScopeFactory _scopes;
    private Guid? _userId;
    private Guid? _terminalId;

    public DesktopAuditingInformationService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public void SetUser(Guid? userId) => _userId = userId;

    public void SetTerminal(Guid? terminalId)
    {
        _terminalId = terminalId is Guid id && id != Guid.Empty ? id : null;
    }

    public Guid? GetUserId() => _userId;

    public Guid? GetTerminalId()
    {
        if (_terminalId is Guid cached && cached != Guid.Empty)
        {
            return cached;
        }

        try
        {
            using var scope = _scopes.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            var hardwareId = MachineIdentity.HardwareId;
            var terminalId = database.Terminals
                .AsNoTracking()
                .Where(terminal => terminal.IsDeleted == false && terminal.HardwareId == hardwareId)
                .Select(terminal => (Guid?)terminal.Id)
                .FirstOrDefault();
            if (terminalId is Guid id && id != Guid.Empty)
            {
                _terminalId = id;
            }

            return _terminalId;
        }
        catch
        {
            return _terminalId;
        }
    }
}
