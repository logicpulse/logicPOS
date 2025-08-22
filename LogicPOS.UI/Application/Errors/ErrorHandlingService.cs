using ErrorOr;
using Gtk;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using Serilog;
using System;

namespace LogicPOS.UI.Errors
{
    public class ErrorHandlingService
    {
        public static void HandleApiError<TResult>(ErrorOr<TResult> error, bool closeApplication = false, Window source = null)
        {
            Log.Logger.Error("API Error: {Error}", error.Errors);

            if (source == null)
            {
                source = (POSWindow.HasInstance && POSWindow.Instance.Visible) ? BackOfficeWindow.Instance : null;
               
                if (source == null)
                {
                    source = BackOfficeWindow.HasInstance ? BackOfficeWindow.Instance : null;
                }
            }

            CustomAlerts.ShowApiErrorAlert(source, error.FirstError);

            if (closeApplication)
            {
                Environment.Exit(1);
            }
        }
    }
}
