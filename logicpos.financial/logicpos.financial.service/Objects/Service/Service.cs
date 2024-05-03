using System.ServiceProcess;

namespace logicpos.financial.service.Objects.Service
{
    internal class Service : ServiceBase
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly bool _debug = true;

       
    }
}
