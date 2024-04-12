namespace acme.softwarevendor.plugin.App
{
    public class FrameworkUtils  : logicpos.shared.App.FrameworkUtils
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
