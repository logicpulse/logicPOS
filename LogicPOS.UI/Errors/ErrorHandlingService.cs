using ErrorOr;
using Gtk;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using System;

namespace LogicPOS.UI.Errors
{
    public class ErrorHandlingService
    {
        public static void HandleApiError(Error error, bool closeApplication = false, Window source = null)
        {
            if (source == null)
            {
                source = (POSWindow.HasInstance && POSWindow.Instance.Visible) ? BackOfficeWindow.Instance : null;
               
                if (source == null)
                {
                    source = BackOfficeWindow.HasInstance ? BackOfficeWindow.Instance : null;
                }
            }

            CustomAlerts.ShowApiErrorAlert(source, error);

            if (closeApplication)
            {
                Environment.Exit(1);
            }
        }
    }
}
