namespace LogicPOS.Persistence.Services.DemoData;

public sealed class DemoSalesDataRunResult
{
    public bool Ran { get; init; }
    public bool Skipped { get; init; }
    public string SummaryCode { get; init; } = string.Empty;
    public int DaysProcessed { get; init; }
    public int DocumentsGenerated { get; init; }
}
