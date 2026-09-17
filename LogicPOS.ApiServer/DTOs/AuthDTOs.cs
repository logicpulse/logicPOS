namespace LogicPOS.ApiServer.DTOs;

public sealed class LoginRequest
{
    public Guid TerminalId { get; set; }
    public Guid UserId { get; set; }
    public string Pin { get; set; } = string.Empty;
}

public sealed class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}
