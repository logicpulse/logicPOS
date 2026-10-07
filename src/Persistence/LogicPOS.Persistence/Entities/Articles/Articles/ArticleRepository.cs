using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Articles.Articles.Common;
using LogicPOS.Domain.Entities.Finance.Documents.Details.Common;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class ArticleRepository : Repository.WithDesignation<Article>, IArticleRepository
{
    public ArticleRepository(LogicPOSDbContext database) : base(database)
    {
    }

    public async Task<string> GetArticleClassAcronymAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Articles!
            .Where(article => article.Id == id)
            .Select(article => article.Class!.Acronym)
            .FirstAsync(ct);
    }

    public async Task<ArticleDetail> GetArticleDetailAsync(Guid id, CancellationToken ct = default)
    {
        var details = await GetArticleDetailsByIdsAsync([id], ct);
        if (!details.TryGetValue(id, out var detail))
        {
            throw new InvalidOperationException("Sequence contains no elements.");
        }

        return detail;
    }

    public async Task<IReadOnlyDictionary<Guid, ArticleDetail>> GetArticleDetailsByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken ct = default)
    {
        var idList = ids as IList<Guid> ?? ids.ToList();
        if (idList.Count == 0)
        {
            return new Dictionary<Guid, ArticleDetail>();
        }

        var details = await Database.Articles
            .AsNoTracking()
            .Where(a => idList.Contains(a.Id))
            .Select(article => new ArticleDetail
            {
                Id = article.Id,
                Code = article.Code,
                Order = article.Order,
                Designation = article.Designation,
                VatExemptionReasonId = article.VatExemptionReasonId,
                Unit = article.MeasurementUnit!.Acronym,
                Discount = article.Discount,
                Vat = article.VatDirectSelling!.Value,
                Notes = article.Notes,
                VatRateId = article.VatDirectSellingId,
                PriceWithVat = article.PriceWithVat,
                ClassAcronym = article.Class!.Acronym,
                Price1 = article.Price1.Value,
                Price2 = article.Price2.Value,
                Price3 = article.Price3.Value,
                Price4 = article.Price4.Value,
                Price5 = article.Price5.Value
            })
            .ToListAsync(ct);

        return details.ToDictionary(d => d.Id);
    }

    public async  Task<bool> DesignationExistsAsync(Guid subfamilyId, string designation, CancellationToken ct)
    {
        return await Database.Articles!.AnyAsync(x => x.Designation.ToLower() == designation.ToLower() && x.SubfamilyId == subfamilyId, ct);
    }

    public async Task<bool> IsPriceWithVatArticleAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Articles!.Where(x => x.Id == id).Select(x => x.PriceWithVat).FirstAsync(ct);
    }

    public async Task<Article?> GetSdrDepositArticleAsync(CancellationToken ct)
    {
        return await Database.Articles.Where(x => x.Code == SdrConstants.SdrArticleCode).FirstOrDefaultAsync(ct);
    }

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        bool result = await Database.DocumentDetails!.AnyAsync(x => x.ArticleId == id, ct);
        if (result)
        {
            return true;
        }
        
        result =  await Database.OrderDetails!.AnyAsync(x => x.ArticleId == id, ct);
        if (result)
        {
            return true;
        }
        
        result = await Database.ArticleCompositions.AnyAsync(x => x.ParentId == id || x.ChildId == id, ct);
        if (result)
        {
            return true;
        }
        
        result = await Database.StockMovements.AnyAsync(x => x.ArticleId == id, ct);
        if (result)
        {
            return true;
        }
        
        result = await Database.WarehouseArticles.AnyAsync(x => x.ArticleId == id, ct);
        if (result)
        {
            return true;
        }

        return false;
    }
}