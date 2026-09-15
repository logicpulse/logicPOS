using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class TerminalService
{
    private readonly ApplicationDbContext _dbContext;

    public TerminalService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TerminalResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var terminals = await _dbContext.ApiTerminals
            .AsNoTracking()
            .OrderByDescending(item => item.IsDefault)
            .ThenBy(item => item.Order)
            .ToListAsync(cancellationToken);

        return terminals.Select(Map).ToList();
    }

    public async Task<TerminalResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var terminal = await _dbContext.ApiTerminals.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return terminal is null ? null : Map(terminal);
    }

    public async Task<TerminalResponse?> GetByHardwareIdAsync(string hardwareId, CancellationToken cancellationToken = default)
    {
        var terminal = await _dbContext.ApiTerminals.AsNoTracking().SingleOrDefaultAsync(item => item.HardwareId == hardwareId, cancellationToken);
        return terminal is null ? null : Map(terminal);
    }

    public async Task<AddEntityIdResponse> CreateAsync(CreateTerminalRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.ApiTerminals.SingleOrDefaultAsync(item => item.HardwareId == request.HardwareId, cancellationToken);
        if (existing is not null)
        {
            return new AddEntityIdResponse { Id = existing.Id };
        }

        var count = await _dbContext.ApiTerminals.CountAsync(cancellationToken);
        var terminal = new ApiTerminal
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"T{count + 1}",
            Designation = $"Terminal {count + 1}",
            HardwareId = request.HardwareId.Trim(),
            TimerInterval = 100,
            IsDefault = count == 0,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiTerminals.Add(terminal);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AddEntityIdResponse
        {
            Id = terminal.Id
        };
    }

    private static TerminalResponse Map(ApiTerminal terminal)
    {
        return new TerminalResponse
        {
            Id = terminal.Id,
            Notes = string.Empty,
            CreatedAt = terminal.CreatedUtc,
            UpdatedAt = terminal.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = false,
            Order = terminal.Order,
            Code = terminal.Code,
            Designation = terminal.Designation,
            HardwareId = terminal.HardwareId,
            TimerInterval = terminal.TimerInterval,
            IsDefault = terminal.IsDefault
        };
    }
}
