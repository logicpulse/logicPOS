using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace LogicPOS.App.Views;

internal static class TouchKeyboard
{
    [SupportedOSPlatform("windows")]
    public static void Show(bool numeric = false)
    {
        if (OperatingSystem.IsWindows() == false)
        {
            return;
        }

        var scope = numeric ? NumberScope : DefaultScope;
        var window = GetForegroundWindow();
        if (window != IntPtr.Zero)
        {
            SetInputScope(window, scope);
        }

        var handle = FindWindow("IPTip_Main_Window", null);
        var visible = handle != IntPtr.Zero && IsWindowVisible(handle);
        if (visible && _scope == scope)
        {
            return;
        }

        try
        {
            if (visible)
            {
                Toggle();
            }

            Toggle();
            _scope = scope;
        }
        catch (Exception)
        {
            StartFallback();
        }
    }

    private static void Toggle()
    {
        var host = new UIHostNoLaunch();
        var invocation = (ITipInvocation)host;
        invocation.Toggle(GetDesktopWindow());
        Marshal.ReleaseComObject(host);
    }

    private static void StartFallback()
    {
        var tabTip = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
            "microsoft shared",
            "ink",
            "TabTip.exe");
        var file = File.Exists(tabTip) ? tabTip : "osk.exe";
        Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
    }

    private const int DefaultScope = 0;
    private const int NumberScope = 29;
    private static int _scope = -1;

    [DllImport("msctf.dll")]
    private static extern int SetInputScope(IntPtr hwnd, int inputScope);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string? className, string? windowName);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr handle);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [ComImport]
    [Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")]
    private class UIHostNoLaunch
    {
    }

    [ComImport]
    [Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ITipInvocation
    {
        void Toggle(IntPtr hwnd);
    }
}
