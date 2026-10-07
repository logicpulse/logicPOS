using System.Globalization;
using System.Net.Sockets;
using System.Text;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.FrontOffice;

public sealed class EscPosTicketPrinter : ITicketPrinter
{
    private readonly IServiceScopeFactory _scopes;

    public EscPosTicketPrinter(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<string?> TryPrintAsync(string title, IReadOnlyList<string> lines, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var printer = await database.Printers
            .AsNoTracking()
            .Where(item => item.IsDeleted == false && item.NetworkName != null && item.NetworkName != "")
            .Select(item => item.NetworkName)
            .FirstOrDefaultAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(printer) || printer.Contains('.') == false)
        {
            return null;
        }

        var host = printer.Trim();
        var port = 9100;
        var separator = host.LastIndexOf(':');
        if (separator > 0 && int.TryParse(host[(separator + 1)..], out var parsed))
        {
            port = parsed;
            host = host[..separator];
        }

        try
        {
            using var client = new TcpClient();
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(2));
            await client.ConnectAsync(host, port, timeout.Token);
            await using var stream = client.GetStream();
            var text = new StringBuilder();
            text.Append(title);
            text.Append("\n\n");
            foreach (var line in lines)
            {
                text.Append(line);
                text.Append('\n');
            }

            text.Append("\n\n");
            var payload = new List<byte> { 0x1B, 0x40 };
            payload.AddRange(Encoding.Latin1.GetBytes(text.ToString()));
            payload.AddRange([0x1D, 0x56, 0x00]);
            await stream.WriteAsync(payload.ToArray(), cancellationToken);
            return null;
        }
        catch (Exception exception)
        {
            return exception.Message;
        }
    }
}
