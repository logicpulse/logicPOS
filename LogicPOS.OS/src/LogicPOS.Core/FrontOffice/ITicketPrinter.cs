namespace LogicPOS.Core.FrontOffice;

public interface ITicketPrinter
{
    Task<string?> TryPrintAsync(string title, IReadOnlyList<string> lines, CancellationToken cancellationToken = default);
}
