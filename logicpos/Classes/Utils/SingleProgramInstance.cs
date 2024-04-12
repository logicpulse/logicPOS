using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using static logicpos.Classes.Utils.Win32APIWindowHelper;

namespace logicpos
{
    public class SingleProgramInstance : IDisposable
    {
        private static readonly log4net.ILog _loggerger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Mutex _mutex;
        private bool _owned = false;

        public SingleProgramInstance()
        {
            if (Utils.IsLinux == false)
            {
                _mutex = new Mutex(
                    initiallyOwned: true,
                    name: CurrentAssemblyName,
                    createdNew: out _owned
                    );
            }
            else
            {
                _owned = true;
            }
        }

        private string CurrentAssemblyName => Assembly.GetExecutingAssembly().GetName().Name;

        public SingleProgramInstance(string identifier)
        {
            if (Utils.IsLinux == false)
            {
                var assemblyName = CurrentAssemblyName + identifier;

                _mutex = new Mutex(
                    initiallyOwned: true,
                    name: assemblyName,
                    createdNew: out _owned
                    );
            }
            else
            {
                _owned = true;
            }
        }

        ~SingleProgramInstance()
        {
            ReleaseMutex();
        }

        public bool IsSingleInstance => _owned;

        private Process[] GetCurrentAssemblyProcesses()
        {
            string currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            Process[] currentAssemblyProcesses = Process.GetProcessesByName(currentAssemblyName);

            return currentAssemblyProcesses;
        }

        private void BringWindowToFrontByHandle(IntPtr handle)
        {
            if (IsWindowVisible(handle) == false)
            {
                ShowWindowAsync(handle, SW_SHOW);
            }

            if (IsIconic(handle))
            {
                ShowWindowAsync(handle, SW_RESTORE);
            }

            SetForegroundWindow(handle);
        }

        public void RaiseOtherProcess()
        {
            Process currentProcess = Process.GetCurrentProcess();
            Process[] currentAssemblyProcesses = GetCurrentAssemblyProcesses();

            foreach (Process otherProcess in currentAssemblyProcesses)
            {
                if (otherProcess.Id == currentProcess.Id) continue;

                IntPtr handle = otherProcess.MainWindowHandle;

                BringWindowToFrontByHandle(handle);

                return;
            }
        }

        private void ReleaseMutex()
        {
            try
            {
                if (_owned)
                {
                    _mutex.ReleaseMutex();
                    _owned = false;
                }
            }
            catch (Exception exception)
            {
                _loggerger.Error("SingleProgramInstance release:  ", exception);
            }
        }

        public void Dispose()
        {
            ReleaseMutex();
            GC.SuppressFinalize(this);
        }

    }
}
