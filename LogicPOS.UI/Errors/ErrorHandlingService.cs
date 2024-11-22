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
            CustomAlerts.ShowApiErrorAlert(source ?? POSWindow.Instance, error);

            if (closeApplication)
            {
                Environment.Exit(1);
            }
        }
    }
}
