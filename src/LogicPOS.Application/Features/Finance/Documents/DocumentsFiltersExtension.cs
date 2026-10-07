using LogicPOS.Domain.Enums;

namespace LogicPOS.Application.Features.Finance.Documents;

public static class DocumentsFiltersExtension
{
    public static IQueryable<Domain.Entities.Document> ApplyActiveSalesInvoicesFilter(
        this IQueryable<Domain.Entities.Document> query)
    {
        return query.ApplySalesInvoicesFilter()
            .ApplyActiveFilter();
    }

    public static IQueryable<Domain.Entities.Document> ApplySalesInvoicesFilter(
        this IQueryable<Domain.Entities.Document> query)
    {
        return query.Where(doc => doc.Series!.DocumentType!.SaftDocumentType == SaftDocumentType.SalesInvoices)
            .ApplyNonDraftFilter();
    }

    public static IQueryable<Domain.Entities.Document> ApplyMovementOfGoodsFilter(
        this IQueryable<Domain.Entities.Document> query)
    {
        return query.Where(doc => doc.Series!.DocumentType!.SaftDocumentType == SaftDocumentType.MovementOfGoods)
            .ApplyNonDraftFilter();
    }

    public static IQueryable<Domain.Entities.Document> ApplyWorkingDocumentsFilter(
        this IQueryable<Domain.Entities.Document> query)
    {
        return query.Where(doc => doc.Series!.DocumentType!.SaftDocumentType == SaftDocumentType.WorkingDocuments)
            .ApplyNonDraftFilter();
    }

    private static IQueryable<Domain.Entities.Document> ApplyNonDraftFilter(
        this IQueryable<Domain.Entities.Document> query)
    {
        return query.Where(doc => doc.IsDraft == false);
    }

    public static IQueryable<Domain.Entities.Document> ApplyDateFilter(this IQueryable<Domain.Entities.Document> query,
        DateTime startDate,
        DateTime endDate)
    {
        startDate = startDate.Date;
        endDate = endDate.Date.AddDays(1).AddTicks(-1);
        return query.Where(doc => doc.CreatedAt >= startDate && doc.CreatedAt <= endDate);
    }

    private static IQueryable<Domain.Entities.Document> ApplyActiveFilter(
        this IQueryable<Domain.Entities.Document> query)
    {
        return query.Where(doc => doc.Status != "A" && doc.IsDraft == false);
    }

    public static IQueryable<Domain.Entities.Document> ApplyTerminalFilter(
        this IQueryable<Domain.Entities.Document> query, Guid? terminalId)
    {
        return terminalId.HasValue ? query.Where(d => d.CreatedWhere == terminalId) : query;
    }

    public static IQueryable<Domain.Entities.Document> ApplyDocumentTypeFilter(
        this IQueryable<Domain.Entities.Document> query, string? documentType)
    {
        return string.IsNullOrWhiteSpace(documentType) ? query : query.Where(d => d.Type == documentType);
    }
}