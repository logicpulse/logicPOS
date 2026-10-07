using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class TicketCreator : EntityCreator<Ticket>
{
    private readonly CreateTicketDto _dto;
    private readonly ITicketRepository _ticketRepository;
    private readonly IArticleRepository _articleRepository;

    public TicketCreator(CreateTicketDto dto,
        ITicketRepository ticketRepository, IArticleRepository articleRepository)
        : base(new())
    {
        _dto = dto;
        _ticketRepository = ticketRepository;
        _articleRepository = articleRepository;
    }

    public override async Task<Result<Ticket>> CreateAsync(CancellationToken ct = default)
    {
        var createResult = await base.CreateAsync(ct);
        if (createResult.IsFailure)
        {
            return createResult;
        }

        var ticket = createResult.Value!;

        var addDetailsResult = await ticket.AddDetailsAsync(_dto.Details,
            _articleRepository,
            ct);
        
        if (addDetailsResult.IsFailure)
        {
            return addDetailsResult.ToGenericFailure<Ticket>();
        }

        return ticket;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.TicketId = 1;

        if (_dto.OrderId.HasValue)
        {
            _entity.OrderId = _dto.OrderId.Value;
            _entity.TicketId =
                await _ticketRepository.GetNextTicketIdForOrderAsync(_dto.OrderId.Value, ct);
        }
    }

    protected override Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }
}