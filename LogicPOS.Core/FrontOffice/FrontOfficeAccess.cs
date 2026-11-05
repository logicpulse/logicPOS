using LogicPOS.Application.Features.System.Users;
using LogicPOS.Core.BackOffice;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Granted permission tokens for the signed-in user.
/// The owner profile with no PermissionProfile rows keeps full access; every other profile
/// without rows is denied (seeded waiters have no grant rows and must not inherit "allow all").
/// </summary>
public sealed class FrontOfficeAccess
{
    public static FrontOfficeAccess All { get; } = new(unrestricted: true, granted: new HashSet<string>(StringComparer.Ordinal));

    public static FrontOfficeAccess None { get; } = new(unrestricted: false, granted: new HashSet<string>(StringComparer.Ordinal));

    private readonly bool _unrestricted;
    private readonly HashSet<string> _granted;

    private FrontOfficeAccess(bool unrestricted, HashSet<string> granted)
    {
        _unrestricted = unrestricted;
        _granted = granted;
    }

    public bool Allows(string token) => _unrestricted || _granted.Contains(token);

    public bool AllowsWorkSession(string token)
        => Allows(UserProfilePermissions.WorkSessions.WORKSESSION_ALL) || Allows(token);

    public bool AllowsBackOffice => Allows(UserProfilePermissions.BACKOFFICE_ACCESS);

    public bool AllowsReports
        => Allows(UserProfilePermissions.Finance.Documents.BACKOFFICE_MAN_DOCUMENTSREPORTS_MENU)
            || Allows(UserProfilePermissions.Reports.REPORT_ACCESS);

    public bool AllowsDocuments
        => Allows(UserProfilePermissions.Finance.Documents.BACKOFFICE_MAN_DOCUMENTSSHOW_MENU)
            || Allows(UserProfilePermissions.Finance.Documents.BACKOFFICE_MAN_DOCUMENTSLISTALL_MENU);

    public bool AllowsNewDocument
        => Allows(UserProfilePermissions.Finance.Documents.BACKOFFICE_MAN_DOCUMENTSNEW_MENU);

    public bool AllowsChangePrice => Allows(UserProfilePermissions.Tickets.TICKETLIST_CHANGE_PRICE);

    public bool AllowsDeleteTicket => Allows(UserProfilePermissions.Tickets.TICKETLIST_DELETE);

    public bool AllowsPayments => AllowsWorkSession(UserProfilePermissions.WorkSessions.WORKSESSION_ORDER_PAYMENT);

    public bool AllowsSession => AllowsWorkSession(UserProfilePermissions.WorkSessions.WORKSESSION_ALL)
        || AllowsWorkSession(UserProfilePermissions.WorkSessions.WORKSESSION_DAY_OPEN)
        || AllowsWorkSession(UserProfilePermissions.WorkSessions.WORKSESSION_DAY_CLOSE)
        || AllowsWorkSession(UserProfilePermissions.WorkSessions.WORKSESSION_TERMINAL_OPEN)
        || AllowsWorkSession(UserProfilePermissions.WorkSessions.WORKSESSION_TERMINAL_CLOSE);

    public bool AllowsDayOpen => AllowsWorkSession(UserProfilePermissions.WorkSessions.WORKSESSION_DAY_OPEN);

    public bool AllowsDayClose => AllowsWorkSession(UserProfilePermissions.WorkSessions.WORKSESSION_DAY_CLOSE);

    public bool AllowsTerminalOpen => AllowsWorkSession(UserProfilePermissions.WorkSessions.WORKSESSION_TERMINAL_OPEN);

    public bool AllowsTerminalClose => AllowsWorkSession(UserProfilePermissions.WorkSessions.WORKSESSION_TERMINAL_CLOSE);

    public bool AllowsCashDrawer => Allows(UserProfilePermissions.Hardware.HARDWARE_DRAWER_OPEN);

    public static Task<FrontOfficeAccess> LoadAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var userId = services.GetRequiredService<IAuditingInformationService>().GetUserId();
        return LoadAsync(services, userId, cancellationToken);
    }

    public static async Task<FrontOfficeAccess> LoadAsync(IServiceProvider services, Guid? userId, CancellationToken cancellationToken = default)
    {
        var auditing = services.GetRequiredService<IAuditingInformationService>();
        if (userId is Guid signedIn && signedIn != Guid.Empty)
        {
            auditing.SetUser(signedIn);
        }
        else
        {
            userId = auditing.GetUserId();
        }

        if (userId is null || userId == Guid.Empty)
        {
            return All;
        }

        await using var scope = services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var profileId = await database.Users.AsNoTracking()
            .Where(item => item.Id == userId && item.IsDeleted == false)
            .Select(item => item.ProfileId)
            .FirstOrDefaultAsync(cancellationToken);
        if (profileId == Guid.Empty)
        {
            return All;
        }

        var grants = await database.PermissionProfiles.AsNoTracking()
            .Where(item => item.UserProfileId == profileId && item.IsDeleted == false)
            .Select(item => new { item.PermissionItemId, item.Granted })
            .ToListAsync(cancellationToken);
        if (grants.Count == 0)
        {
            return ProfileMenuAccess.KeepsPermissionsMenu(profileId) ? All : None;
        }

        var grantedIds = grants.Where(item => item.Granted).Select(item => item.PermissionItemId).ToHashSet();
        var tokens = await database.PermissionItems.AsNoTracking()
            .Where(item => item.IsDeleted == false && grantedIds.Contains(item.Id))
            .Select(item => item.Token)
            .ToListAsync(cancellationToken);
        return new FrontOfficeAccess(false, new HashSet<string>(tokens, StringComparer.Ordinal));
    }
}
