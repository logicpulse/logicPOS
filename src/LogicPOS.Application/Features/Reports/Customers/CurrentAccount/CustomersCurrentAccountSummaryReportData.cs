using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.Customers.CurrentAccount;

public record CustomersCurrentAccountSummaryReportData : ReportData
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public IReadOnlyList<CustomerCurrentAccountTotalRow> Rows { get; set; } = [];
}

public record CustomerCurrentAccountTotalRow
{
    public string FiscalNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    /// <summary>Total a débito (FT, ND e linhas de recibo DebitAmount) no intervalo de datas do relatório.</summary>
    public decimal TotalDebit { get; set; }
    /// <summary>Total a crédito (NC e linhas de recibo CreditAmount) no intervalo de datas do relatório.</summary>
    public decimal TotalCredit { get; set; }
    /// <summary>Saldo = crédito − débito no intervalo (movimentos do período).</summary>
    public decimal PeriodBalance { get; set; }
    /// <summary>Saldo em aberto (Σ TotalToPay em FT/ND), com sinal legacy credit − debit.</summary>
    public decimal LifetimeBalance { get; set; }
}
