using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LogicPOS.App.Hardware;

internal static class WindowsPrinters
{
    private const int PrinterEnumLocal = 0x00000002;
    private const int PrinterEnumConnections = 0x00000004;
    private const int ErrorInsufficientBuffer = 122;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct PrinterInfo4
    {
        public string? PrinterName;
        public string? ServerName;
        public int Attributes;
    }

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "EnumPrintersW")]
    private static extern bool EnumPrinters(int flags, string? name, int level, IntPtr buffer, int size, out int needed, out int returned);

    public static IReadOnlyList<string> Installed()
    {
        if (OperatingSystem.IsWindows() == false)
        {
            return [];
        }

        const int flags = PrinterEnumLocal | PrinterEnumConnections;
        EnumPrinters(flags, null, 4, IntPtr.Zero, 0, out var needed, out _);
        var error = Marshal.GetLastWin32Error();
        if (needed <= 0)
        {
            return error == 0 || error == ErrorInsufficientBuffer ? [] : throw new Win32Exception(error);
        }

        var buffer = Marshal.AllocHGlobal(needed);
        try
        {
            if (EnumPrinters(flags, null, 4, buffer, needed, out _, out var returned) == false)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var size = Marshal.SizeOf<PrinterInfo4>();
            var names = new List<string>(returned);
            for (var index = 0; index < returned; index++)
            {
                var info = Marshal.PtrToStructure<PrinterInfo4>(buffer + index * size);
                if (string.IsNullOrWhiteSpace(info.PrinterName) == false)
                {
                    names.Add(info.PrinterName);
                }
            }

            return names.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(name => name, StringComparer.CurrentCultureIgnoreCase).ToList();
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
