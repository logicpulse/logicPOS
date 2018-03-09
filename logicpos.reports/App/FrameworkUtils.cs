using System.Windows.Forms;

namespace logicpos.reports.App
{
    public class FrameworkUtils  : logicpos.shared.App.FrameworkUtils
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

       //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Windows.Forms

        private static int _numWaitingCursorCalls = 0;
        public static void HideWaitingCursor()
        {
            if (_numWaitingCursorCalls > 0)
            {
                _numWaitingCursorCalls--;
            }
            if (_numWaitingCursorCalls == 0)
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public static void ShowWaitingCursor()
        {
            _numWaitingCursorCalls++;
            Cursor.Current = Cursors.WaitCursor;
        }
    }
}
