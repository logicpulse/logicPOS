using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using static logicpos.Classes.Utils.Win32APIWindowHelper;

namespace logicpos
{
    public class SingleProgramInstance : IDisposable
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string _appGuid = "bfb677c2-a44a-46f8-93ab-d2d6a54e0b53";
        private readonly Mutex _mutex;
        private bool _owned = false;

        private string CurrentAssemblyName => Assembly.GetExecutingAssembly().GetName().Name;

        public SingleProgramInstance()
        {
            var identifier = _appGuid;


            var assemblyName = CurrentAssemblyName + identifier;

            _mutex = new Mutex(
                initiallyOwned: true,
                name: assemblyName,
                createdNew: out _owned
                );
        }

        ~SingleProgramInstance()
        {
            ReleaseMutex();
        }

        public bool IsSingleInstance => _owned;

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
                _logger.Error("SingleProgramInstance release:  ", exception);
            }
        }

        public void Dispose()
        {
            ReleaseMutex();
            GC.SuppressFinalize(this);
        }

    }
}
