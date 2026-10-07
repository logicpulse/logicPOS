namespace LogicPOS.Application.Features.System.Users;

public interface IJwtService
{
    public string GenerateToken(Guid userId, Guid terminalId, string? clientId = null, string? purpose = null);
}