using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Finance.Documents.Details.Common;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class OrderDetailCreator : EntityCreator<OrderDetail>
{
    private readonly CreateOrderDetailDto _dto;
    private readonly IArticleRepository _articleRepository;
    public OrderDetailCreator(CreateOrderDetailDto dto,
                              IArticleRepository articleRepository) : base(new())
    {
        _dto = dto;
        _articleRepository = articleRepository;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        ArticleDetail article = await _articleRepository.GetArticleDetailAsync(_dto.ArticleId, ct);
        _entity.Order = article.Order;
        _entity.Code = article.Code;
        _entity.Designation = article.Designation;
        _entity.ArticleId = _dto.ArticleId;
        _entity.VatExemptionReasonId = article.VatExemptionReasonId;
        _entity.Unit = article.Unit;
        _entity.Quantity = _dto.Quantity;
        _entity.Price = _dto.UnitPrice;
        _entity.Discount = article.Discount;
        _entity.Vat = article.Vat;
        _entity.TotalDiscount = (_dto.Quantity * _dto.UnitPrice) * (article.Discount / 100M);
        var totalWithDiscount = (_dto.Quantity * _dto.UnitPrice) - _entity.TotalDiscount;
        _entity.TotalTax = (totalWithDiscount) * (_entity.Vat / 100M);
        _entity.TotalFinal = totalWithDiscount + _entity.TotalTax;
    }

    protected override Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (!await _articleRepository.ExistsAsync(_dto.ArticleId, ct))
        {
            return Result.NotFound(nameof(Article),_dto.ArticleId.ToString());
        }

        return Result.Success();
    }
}