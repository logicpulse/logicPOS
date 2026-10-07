using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public sealed class LocalReportService : IReportService
{
    private readonly IServiceScopeFactory _scopes;

    public LocalReportService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public IReadOnlyList<ReportDefinition> Catalog => ReportCatalog.All;

    public async Task<ListingSaveResult> GenerateAsync(
        string key,
        DateTime start,
        DateTime end,
        Guid? customerId,
        Guid? articleId,
        CancellationToken cancellationToken = default)
    {
        var report = ReportCatalog.All.FirstOrDefault(item => item.Key == key);
        if (report is null)
        {
            return ListingSaveResult.Fail("Relatório desconhecido.");
        }

        if (end.Date < start.Date)
        {
            return ListingSaveResult.Fail("A data final tem de ser igual ou posterior à data inicial.");
        }

        if (report.NeedsCustomer && (customerId is null || customerId == Guid.Empty))
        {
            return ListingSaveResult.Fail("Escolha um cliente.");
        }

        if (report.NeedsArticle && (articleId is null || articleId == Guid.Empty))
        {
            return ListingSaveResult.Fail("Escolha um artigo.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var rows = await ReadRowsAsync(database, report, start.Date, end.Date, customerId, articleId, cancellationToken);
        if (rows.Count == 0)
        {
            return ListingSaveResult.Fail("Não há dados para este relatório no intervalo indicado.");
        }

        var headers = rows[0].Keys.ToList();
        var table = rows.Select(row => (IReadOnlyList<string>)headers.Select(header => row[header]).ToList()).ToList();
        var bytes = ListingTableExport.ToPdf(report.Name, headers, table);
        var path = Path.Combine(Path.GetTempPath(), $"logicpos-report-{report.Key}-{DateTime.Now:yyyyMMddHHmmss}.pdf");
        await File.WriteAllBytesAsync(path, bytes, cancellationToken);
        return ListingSaveResult.Ok(Guid.Empty, path);
    }

    private static async Task<List<Dictionary<string, string>>> ReadRowsAsync(
        LogicPOSDbContext database,
        ReportDefinition report,
        DateTime start,
        DateTime end,
        Guid? customerId,
        Guid? articleId,
        CancellationToken cancellationToken)
    {
        var until = end.AddDays(1);
        if (report.Key is "articles")
        {
            var articles = await database.Articles.AsNoTracking()
                .Where(item => item.IsDeleted == false)
                .OrderBy(item => item.Designation)
                .Select(item => new { item.Code, item.Designation, item.Barcode })
                .Take(500)
                .ToListAsync(cancellationToken);
            return articles.Select(item => Row(("Código", item.Code), ("Designação", item.Designation), ("Código de barras", item.Barcode ?? string.Empty))).ToList();
        }

        if (report.Key is "customers" or "suppliers")
        {
            var customers = await database.Customers.AsNoTracking()
                .Where(item => item.IsDeleted == false)
                .OrderBy(item => item.Name)
                .Select(item => new { item.Name, item.FiscalNumber, item.City })
                .Take(500)
                .ToListAsync(cancellationToken);
            return customers.Select(item => Row(("Nome", item.Name), ("NIF", item.FiscalNumber ?? string.Empty), ("Cidade", item.City ?? string.Empty))).ToList();
        }

        if (report.Key is "stock" or "stock-article" or "stock-gain" or "stock-supplier")
        {
            var stock = database.WarehouseArticles.AsNoTracking().Where(item => item.IsDeleted == false);
            if (articleId is Guid selected && selected != Guid.Empty)
            {
                stock = stock.Where(item => item.ArticleId == selected);
            }

            var lines = await stock
                .OrderBy(item => item.Article!.Designation)
                .Select(item => new { item.Article!.Code, item.Article.Designation, item.Quantity, item.SerialNumber })
                .Take(500)
                .ToListAsync(cancellationToken);
            return lines.Select(item => Row(
                ("Código", item.Code ?? string.Empty),
                ("Designação", item.Designation ?? string.Empty),
                ("Quantidade", item.Quantity.ToString("0.##")),
                ("Série", item.SerialNumber ?? string.Empty))).ToList();
        }

        var documents = database.Documents.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.IsDraft == false && item.Status != "A")
            .Where(item => item.CreatedAt >= start && item.CreatedAt < until);
        if (customerId is Guid customer && customer != Guid.Empty && report.Key is "customer-balance" or "current-account" or "sales-customer" or "sales-customer-detail")
        {
            documents = documents.Where(item => item.CustomerId == customer);
        }

        var sales = await documents
            .OrderBy(item => item.CreatedAt)
            .Select(item => new
            {
                item.CreatedAt,
                item.Number,
                item.Type,
                item.TotalFinal,
                Customer = item.Customer.Name
            })
            .Take(500)
            .ToListAsync(cancellationToken);

        if (report.Key is "sales-day")
        {
            return sales
                .GroupBy(item => item.CreatedAt.Date)
                .Select(group => Row(("Data", group.Key.ToString("dd/MM/yyyy")), ("Documentos", group.Count().ToString()), ("Total", group.Sum(item => item.TotalFinal).ToString("0.00"))))
                .ToList();
        }

        if (report.Key is "sales-customer" or "sales-customer-detail")
        {
            return sales
                .GroupBy(item => item.Customer)
                .Select(group => Row(("Cliente", group.Key ?? string.Empty), ("Documentos", group.Count().ToString()), ("Total", group.Sum(item => item.TotalFinal).ToString("0.00"))))
                .ToList();
        }

        if (report.Key is "sales-document-type" or "sales-document-type-detail")
        {
            return sales
                .GroupBy(item => item.Type)
                .Select(group => Row(("Tipo", group.Key ?? string.Empty), ("Documentos", group.Count().ToString()), ("Total", group.Sum(item => item.TotalFinal).ToString("0.00"))))
                .ToList();
        }

        return sales.Select(item => Row(
            ("Data", item.CreatedAt.ToString("dd/MM/yyyy")),
            ("Número", item.Number ?? string.Empty),
            ("Tipo", item.Type ?? string.Empty),
            ("Cliente", item.Customer ?? string.Empty),
            ("Total", item.TotalFinal.ToString("0.00")))).ToList();
    }

    private static Dictionary<string, string> Row(params (string Key, string Value)[] cells)
        => cells.ToDictionary(cell => cell.Key, cell => cell.Value ?? string.Empty);
}
