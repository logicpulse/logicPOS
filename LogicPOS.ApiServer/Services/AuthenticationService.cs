using ErrorOr;
using LogicPOS.ApiServer.Authentication;
using LogicPOS.ApiServer.DTOs;

namespace LogicPOS.ApiServer.Services;

public sealed class AuthenticationService
{
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(JwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public Task<ErrorOr<string>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TerminalId == Guid.Empty)
        {
            return Task.FromResult<ErrorOr<string>>(Error.Validation("auth.terminal_id", "TerminalId é obrigatório."));
        }

        if (request.UserId == Guid.Empty)
        {
            return Task.FromResult<ErrorOr<string>>(Error.Validation("auth.user_id", "UserId é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(request.Pin))
        {
            return Task.FromResult<ErrorOr<string>>(Error.Validation("auth.pin", "PIN é obrigatório."));
        }

        var token = _jwtTokenGenerator.GenerateToken(request.TerminalId, request.UserId);
        return Task.FromResult<ErrorOr<string>>(token);
    }
}
