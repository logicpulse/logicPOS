using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Application.Features.Common;

public static class EntityDateFilterExtension
{
    public static IQueryable<T> ApplyDateFilter<T>(
        this IQueryable<T> query,
        DateTime startDate,
        DateTime endDate) where T : Entity
    {
        startDate = startDate.Date;
        endDate = endDate.Date.AddDays(1).AddTicks(-1);
        return query.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate);
    }
}
