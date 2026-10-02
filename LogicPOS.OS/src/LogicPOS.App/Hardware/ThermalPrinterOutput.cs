using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using LogicPOS.Core.FrontOffice;

namespace LogicPOS.App.Hardware;

/// <summary>
/// Sends ESC/POS bytes to a raw TCP endpoint (NetworkName, port 9100) or to the Windows spooler (RAW datatype).
/// </summary>
internal static class ThermalPrinterOutput
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private sealed class DocInfo1
    {
        public string? DocName;
        public string? OutputFile;
        public string? DataType;
    }

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "OpenPrinterW")]
    private static extern bool OpenPrinter(string printerName, out IntPtr handle, IntPtr defaults);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr handle);

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "StartDocPrinterW")]
    private static extern int StartDocPrinter(IntPtr handle, int level, [In] DocInfo1 info);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr handle);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr handle);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr handle);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool WritePrinter(IntPtr handle, byte[] bytes, int count, out int written);

    public static async Task SendAsync(ThermalPrinterSettings printer, string documentName, byte[] payload)
    {
        var installed = WindowsPrinters.Installed();
        bool IsInstalled(string? name) => string.IsNullOrWhiteSpace(name) == false
            && installed.Contains(name.Trim(), StringComparer.OrdinalIgnoreCase);

        var network = printer.NetworkName?.Trim();
        var windowsName = IsInstalled(printer.Designation) ? printer.Designation!.Trim()
            : IsInstalled(network) ? network
            : null;

        if (IsNetworkAddress(network) && IsInstalled(network) == false)
        {
            try
            {
                await SendToNetworkAsync(network!, payload);
                return;
            }
            catch (Exception exception) when (windowsName is not null && exception is SocketException or OperationCanceledException)
            {
                // Unreachable endpoint: fall back to the installed Windows printer with the same configuration
            }
        }

        if (windowsName is null)
        {
            var configured = string.Join(" / ", new[] { printer.Designation, network }.Where(item => string.IsNullOrWhiteSpace(item) == false));
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(configured)
                ? "A impressora térmica do terminal não tem nome nem endereço de rede."
                : $"\"{configured}\" não é uma impressora instalada no Windows nem um endereço IP válido.");
        }

        await Task.Run(() => SendToSpooler(windowsName, documentName, payload));
    }

    private static bool IsNetworkAddress(string? networkName)
    {
        if (string.IsNullOrWhiteSpace(networkName) || networkName.StartsWith(@"\\", StringComparison.Ordinal))
        {
            return false;
        }

        var host = networkName;
        var separator = networkName.LastIndexOf(':');
        if (separator > 0 && int.TryParse(networkName[(separator + 1)..], out _))
        {
            host = networkName[..separator];
        }

        return System.Net.IPAddress.TryParse(host, out _)
            || (host.Contains('.') && host.Any(char.IsWhiteSpace) == false);
    }

    private static async Task SendToNetworkAsync(string networkName, byte[] payload)
    {
        var host = networkName;
        var port = 9100;
        var separator = networkName.LastIndexOf(':');
        if (separator > 0 && int.TryParse(networkName[(separator + 1)..], out var parsed))
        {
            host = networkName[..separator];
            port = parsed;
        }

        using var client = new TcpClient();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await client.ConnectAsync(host, port, timeout.Token);
        await using var stream = client.GetStream();
        await stream.WriteAsync(payload, timeout.Token);
        await stream.FlushAsync(timeout.Token);
    }

    private static void SendToSpooler(string printerName, string documentName, byte[] payload)
    {
        if (OperatingSystem.IsWindows() == false)
        {
            throw new PlatformNotSupportedException("A impressão no spooler só está disponível em Windows.");
        }

        if (OpenPrinter(printerName, out var handle, IntPtr.Zero) == false)
        {
            throw new InvalidOperationException($"Impressora \"{printerName}\" não encontrada: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
        }

        try
        {
            if (StartDocPrinter(handle, 1, new DocInfo1 { DocName = documentName, DataType = "RAW" }) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            try
            {
                StartPagePrinter(handle);
                if (WritePrinter(handle, payload, payload.Length, out var written) == false || written != payload.Length)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                EndPagePrinter(handle);
            }
            finally
            {
                EndDocPrinter(handle);
            }
        }
        finally
        {
            ClosePrinter(handle);
        }
    }
}
