using ErrorOr;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using System;

namespace LogicPOS.UI.Errors
{
    public class ErrorHandlingService
    {
        public static void HandleApiError(Error error, bool closeApplication = false)
        {
            CustomAlerts.ShowApiErrorAlert(POSWindow.Instance, error);

            if (closeApplication)
            {
                Environment.Exit(1);
            }
        }
    }
}
