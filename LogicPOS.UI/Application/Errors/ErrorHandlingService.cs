using ErrorOr;
using Gtk;
using LogicPOS.UI.Alerts;
using Serilog;
using System;

namespace LogicPOS.UI.Errors
{
    public class ErrorHandlingService
    {
        public static void HandleApiError<TResult>(ErrorOr<TResult> error, bool closeApplication = false, Window source = null)
        {
            Log.Error("API Error: {Error}", error.Errors);

            ShowApiErrorAlert(CustomAlerts.ResolveParentWindow(source), error.FirstError);

            if (closeApplication)
            {
                Environment.Exit(1);
            }
        }

        public static void ShowApiErrorAlert(Window sourceWindow, Error error)
        {
            var userMessage = ApiErrorMessageFormatter.ToUserMessage(error);

            if (sourceWindow == null)
            {
                System.Windows.MessageBox.Show(
                    userMessage,
                    "Erro — logicPOS",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }

            new CustomAlert(sourceWindow)
                           .WithMessage(userMessage)
                           .WithMessageType(MessageType.Error)
                           .WithButtonsType(ButtonsType.Ok)
                           .WithTitle("Erro")
                           .ShowAlert();
        }
    }
}
