using ErrorOr;
using LogicPOS.ApiServer.Authentication;
using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class AuthenticationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly PinHasher _pinHasher;

    public AuthenticationService(
        ApplicationDbContext dbContext,
        JwtTokenGenerator jwtTokenGenerator,
        PinHasher pinHasher)
    {
        _dbContext = dbContext;
        _jwtTokenGenerator = jwtTokenGenerator;
        _pinHasher = pinHasher;
    }

    public async Task<ErrorOr<string>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TerminalId == Guid.Empty)
        {
            return Error.Validation("auth.terminal_id", "TerminalId é obrigatório.");
        }

        if (request.UserId == Guid.Empty)
        {
            return Error.Validation("auth.user_id", "UserId é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Pin))
        {
            return Error.Validation("auth.pin", "PIN é obrigatório.");
        }

        var user = await _dbContext.ApiUsers
            .AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.Id == request.UserId && candidate.TerminalId == request.TerminalId, cancellationToken);

        if (user is null)
        {
            user = await _dbContext.ApiUsers
                .AsNoTracking()
                .SingleOrDefaultAsync(candidate => candidate.Id == request.UserId && candidate.TerminalId == Guid.Empty, cancellationToken);
        }

        if (user is null || !_pinHasher.Verify(request.Pin, user.PinHash, user.PinSalt))
        {
            return Error.Unauthorized("auth.invalid_credentials", "Credenciais inválidas.");
        }

        return _jwtTokenGenerator.GenerateToken(request.TerminalId, request.UserId);
    }
}
