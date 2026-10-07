namespace LogicPOS.Application.Features.Import;

public sealed record ExcelImportResponse
{
    public int RowsFound { get; init; }
    public int Created { get; init; }
    public int Skipped { get; init; }
    public int Failed { get; init; }
    public IReadOnlyList<ExcelImportItem> Items { get; init; } = [];
}

public sealed record ExcelImportItem
{
    public int RowNumber { get; init; }
    public string? Key { get; init; }
    public string Action { get; init; } = null!;
    public string? Message { get; init; }
}

public sealed record ExcelArticleRow
{
    public int RowNumber { get; init; }
    public string? Code { get; init; }
    public string? Designation { get; init; }
    public string? Family { get; init; }
    public string? SubFamily { get; init; }
    public string? Price1 { get; init; }
    public string? Vat { get; init; }
}

public sealed record ExcelCustomerRow
{
    public int RowNumber { get; init; }
    public string? Code { get; init; }
    public string? FiscalNumber { get; init; }
    public string? Name { get; init; }
    public string? Address { get; init; }
    public string? Locality { get; init; }
    public string? ZipCode { get; init; }
    public string? City { get; init; }
    public string? Phone { get; init; }
    public string? MobilePhone { get; init; }
    public string? Email { get; init; }
}
