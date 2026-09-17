using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using ApiPrinter = LogicPOS.Api.Entities.Printer;
using EscPosPrinter = ESC_POS_USB_NET.Printer.Printer;

namespace LogicPOS.UI.Printing
{
    /// <summary>
    /// Resolves Windows spooler vs raw TCP (port 9100) for thermal printers,
    /// matching LogicPOS Print Agent behaviour.
    /// </summary>
    public static class ThermalPrinterTarget
    {
        private static readonly AsyncLocal<ApiPrinter> CurrentConfig = new AsyncLocal<ApiPrinter>();
        private static readonly FieldInfo BufferField =
            typeof(EscPosPrinter).GetField("_buffer", BindingFlags.Instance | BindingFlags.NonPublic);

        public static IDisposable Use(ApiPrinter printer)
        {
            var previous = CurrentConfig.Value;
            CurrentConfig.Value = printer;
            return new RestoreScope(previous);
        }

        public static EscPosPrinter CreateEscPosPrinter(ApiPrinter printer)
        {
            if (printer == null)
            {
                return null;
            }

            // ESC-POS ctor needs a Windows name; unused when committing via TCP.
            var windowsName = ResolveWindowsPrinterName(printer) ?? "NETWORK";
            return new EscPosPrinter(windowsName);
        }

        public static string ResolveWindowsPrinterName(ApiPrinter printer)
        {
            if (printer == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(printer.Designation))
            {
                return printer.Designation.Trim();
            }

            if (!ShouldUseTcpNetwork(printer) && !string.IsNullOrWhiteSpace(printer.NetworkName))
            {
                return printer.NetworkName.Trim();
            }

            return null;
        }

        public static bool ShouldUseTcpNetwork(ApiPrinter printer)
        {
            if (printer == null || string.IsNullOrWhiteSpace(printer.NetworkName))
            {
                return false;
            }

            var trimmed = printer.NetworkName.Trim();
            if (trimmed.StartsWith(@"\\", StringComparison.Ordinal))
            {
                return false;
            }

            try
            {
                ParseEndpoint(trimmed);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Commit(EscPosPrinter escPrinter)
        {
            if (escPrinter == null)
            {
                return;
            }

            var config = CurrentConfig.Value;
            if (config != null && ShouldUseTcpNetwork(config))
            {
                var buffer = BufferField?.GetValue(escPrinter) as byte[];
                if (buffer != null && buffer.Length > 0)
                {
                    SendToNetwork(config.NetworkName, buffer);
                }

                escPrinter.Clear();
                return;
            }

            escPrinter.PrintDocument();
        }

        public static (string Host, int Port) ParseEndpoint(string networkName, int defaultPort = 9100)
        {
            if (string.IsNullOrWhiteSpace(networkName))
            {
                throw new InvalidOperationException("NetworkName is required for network printing.");
            }

            var trimmed = networkName.Trim();
            var colonIndex = trimmed.LastIndexOf(':');
            if (colonIndex > 0
                && colonIndex < trimmed.Length - 1
                && int.TryParse(trimmed.Substring(colonIndex + 1), out var port))
            {
                return (trimmed.Substring(0, colonIndex), port);
            }

            return (trimmed, defaultPort);
        }

        public static void SendToNetwork(string networkName, byte[] payload)
        {
            var endpoint = ParseEndpoint(networkName);
            using (var client = new TcpClient())
            {
                client.Connect(endpoint.Host, endpoint.Port);
                using (var stream = client.GetStream())
                {
                    stream.Write(payload, 0, payload.Length);
                    stream.Flush();
                }
            }
        }

        private sealed class RestoreScope : IDisposable
        {
            private readonly ApiPrinter _previous;
            private bool _disposed;

            public RestoreScope(ApiPrinter previous) => _previous = previous;

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                CurrentConfig.Value = _previous;
                _disposed = true;
            }
        }
    }
}
