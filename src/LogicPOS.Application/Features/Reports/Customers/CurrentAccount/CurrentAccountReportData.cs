
using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.Customers.CurrentAccount;

public record CurrentAccountReportData : ReportData
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Customer Customer { get; set; } = null!;
    public decimal PeriodTotalBalance { get; set; }
    public decimal PeriodTotalCredit { get; set; }
    public decimal PeriodTotalDebit { get; set; }
    public decimal CustomerBalance { get; set; }
    public List<Transaction> Transactions { get; set; } = [];
}

public record Transaction
{
    public Guid? DocumentId { get; set; }
    public DateTime Date { get; set; }
    public string DocumentNumber { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public string? References { get; set; }
    public decimal Credit { get; set; }
    public decimal Debit { get; set; }
    public string DocumentType { get; set; } = null!;
}