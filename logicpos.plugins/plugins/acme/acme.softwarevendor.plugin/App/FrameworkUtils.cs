namespace acme.softwarevendor.plugin.App
{
    public class FrameworkUtils  : logicpos.shared.App.FrameworkUtils
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
