using LogicPOS.Domain.Enums;

namespace LogicPOS.Application.Features.Finance.Documents;

public static class DocumentDetailsFiltersExtension
{
    public static IQueryable<Domain.Entities.DocumentDetail> ApplyActiveSalesInvoicesFilter(
        this IQueryable<Domain.Entities.DocumentDetail> query)
    {
        return query.ApplySalesInvoicesFilter()
            .ApplyActiveFilter();
    }

    private static IQueryable<Domain.Entities.DocumentDetail> ApplySalesInvoicesFilter(
        this IQueryable<Domain.Entities.DocumentDetail> query)
    {
        return query.Where(dd => dd.Document!.Series!.DocumentType!.SaftDocumentType == SaftDocumentType.SalesInvoices)
            .ApplyNonDraftFilter();
    }

    private static IQueryable<Domain.Entities.DocumentDetail> ApplyNonDraftFilter(
        this IQueryable<Domain.Entities.DocumentDetail> query)
    {
        return query.Where(dd => dd.Document!.IsDraft == false);
    }

    public static IQueryable<Domain.Entities.DocumentDetail> ApplyDateFilter(this IQueryable<Domain.Entities.DocumentDetail> query,
        DateTime startDate,
        DateTime endDate)
    {
        startDate = startDate.Date;
        endDate = endDate.Date.AddDays(1).AddTicks(-1);
        return query.Where(doc => doc.CreatedAt >= startDate && doc.CreatedAt <= endDate);
    }

    private static IQueryable<Domain.Entities.DocumentDetail> ApplyActiveFilter(
        this IQueryable<Domain.Entities.DocumentDetail> query)
    {
        return query.Where(dd => dd.Document!.Status != "A" && dd.Document!.IsDraft == false);
    }

    public static IQueryable<Domain.Entities.DocumentDetail> ApplyTerminalFilter(
    this IQueryable<Domain.Entities.DocumentDetail> query, Guid? terminalId)
    {
        return terminalId.HasValue ? query.Where(d => d.CreatedWhere == terminalId) : query;
    }

    public static IQueryable<Domain.Entities.DocumentDetail> ApplyDocumentTypeFilter(
        this IQueryable<Domain.Entities.DocumentDetail> query, string? documentType)
    {
        return string.IsNullOrWhiteSpace(documentType) ? query : query.Where(d => d.Document!.Type == documentType);
    }

    public static IQueryable<Domain.Entities.DocumentDetail> ApplyTaxFilter(
    this IQueryable<Domain.Entities.DocumentDetail> query, Guid? taxId)
    {
        return taxId.HasValue ? query.Where(d => d.Tax.TaxId == taxId) : query;
    }

}