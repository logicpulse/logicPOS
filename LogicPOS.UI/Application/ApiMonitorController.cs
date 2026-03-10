using Serilog;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace LogicPOS.UI.Application
{
    public static class ApiMonitorController
    {
        private static uint _commandMessageId = 0;
        private const string TARGET_WINDOW_TITLE = "LogicPOS API Monitor"; 
        private const string RunMonitorUpdateCommand = "RUN-UPDATE";
        private const string ApiMonitorExecutableName = "logicposapi-monitor.exe";

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static bool MonitorIsRunning()
        {
            var processes = System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(ApiMonitorExecutableName));
            return processes.Length > 0;
        }

        public static bool SendUpdateCommand()
        {
            Log.Information("Attempting to send update command to API Monitor...");
            if (_commandMessageId == 0)
            {

                _commandMessageId = RegisterWindowMessage(RunMonitorUpdateCommand);
                if (_commandMessageId == 0)
                {
                    Log.Error("Failed to register custom Windows message for API Monitor command. Error code: {ErrorCode}", Marshal.GetLastWin32Error());
                    return false;
                }
            }

            IntPtr hwnd = FindWindow(null, TARGET_WINDOW_TITLE);
            if (hwnd == IntPtr.Zero)
            {
                Log.Warning("API Monitor window not found. Make sure it is open and the title matches exactly.");
                return false;
            }

            IntPtr result = SendMessage(hwnd, _commandMessageId, (IntPtr)1, IntPtr.Zero);
            int lastError = Marshal.GetLastWin32Error();
            Log.Information($"Sent command {RunMonitorUpdateCommand} to API Monitor. SendMessage result: {result}, Last Win32 Error: {lastError}");
            return result != IntPtr.Zero;
        }
    }
}
