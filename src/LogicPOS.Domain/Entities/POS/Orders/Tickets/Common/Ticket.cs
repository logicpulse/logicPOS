using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

// [Table("fin_documentorderticket")]
public class Ticket : Entity
{
    public Order? Order { get; set; }
    public Guid OrderId { get; set; }

    public int TicketId { get; set; }
    public Enums.PriceType PriceType { get; set; }
    
    [Column(TypeName = "decimal(18,6)")]
    public decimal Discount { get; set; }

    public IList<OrderDetail>? Details { get; set; }

    internal async Task<Result> AddDetailsAsync(IEnumerable<CreateOrderDetailDto> details,
                                                IArticleRepository articleRepository,
                                                CancellationToken ct = default)
    {
        foreach (var detail in details)
        {
            var addDetailResult = await AddDetailAsync(detail,
                                                       articleRepository,
                                                       ct);

            if (addDetailResult.IsFailure)
            {
                return addDetailResult;
            }
        }

        return Result.Success();
    }

    private async Task<Result> AddDetailAsync(CreateOrderDetailDto createDetailDto,
                                              IArticleRepository articleRepository,
                                              CancellationToken cancellationToken = default)
    {
        var detailCreator = new OrderDetailCreator(createDetailDto,
                                                   articleRepository);

        var createDetailResult = await detailCreator.CreateAsync(cancellationToken);

        if (createDetailResult.IsFailure)
        {
            return createDetailResult;
        }

        var detail = createDetailResult.Value!;
        detail.TicketId = Id;

        Details ??= new List<OrderDetail>();

        Details.Add(detail);

        return Result.Success();
    }

}

