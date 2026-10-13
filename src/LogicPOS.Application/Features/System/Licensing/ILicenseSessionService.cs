using LogicPOS.Domain.Results;

namespace LogicPOS.Application.Features.System.Licensing;

public interface ILicenseSessionService
{
    Task<Result<Guid>> TryClaimAsync(
        Guid userId,
        string clientKind,
        Guid? terminalId,
        int allowedDevices,
        CancellationToken cancellationToken = default);

    Task<Result> HeartbeatAsync(Guid sessionKey, CancellationToken cancellationToken = default);

    Task ReleaseAsync(Guid sessionKey, CancellationToken cancellationToken = default);
}