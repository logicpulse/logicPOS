using ErrorOr;
using Gtk;
using LogicPOS.Api.Errors;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using Serilog;
using System;
using System.Linq;
using System.Text;

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

        public static void ShowApiErrorAlert(Window sourceWindow,
                                       Error error)
        {

            var errorMessage = new StringBuilder();

            var metadata = error.Metadata;

            if (metadata != null)
            {
                var problemDetails = (ProblemDetails)metadata["problem"];

                if (problemDetails.Errors != null)
                {
                    foreach (var problemDetailsError in problemDetails.Errors)
                    {
                        errorMessage.AppendLine($"\n# Erro ({problemDetailsError.Name}):");
                        errorMessage.AppendLine("Descrição: " + problemDetailsError.Reason);
                    }
                }

                errorMessage.AppendLine("\n# Mais detalhes");
                errorMessage.AppendLine("Título: " + problemDetails.Title);
                errorMessage.AppendLine($"Detalhe: {problemDetails.Detail}");
                errorMessage.AppendLine($"Status: {problemDetails.Status}");
                errorMessage.AppendLine($"Instância: {problemDetails.Instance}");
                errorMessage.AppendLine($"Tipo: {problemDetails.Type}");
                errorMessage.AppendLine($"TraceId: {problemDetails.TraceId}");
            }
            else
            {
                errorMessage.AppendLine(error.Description);
            }

            if (sourceWindow == null)
            {
                System.Windows.MessageBox.Show(
                    errorMessage.ToString(),
                    "Erro — logicPOS",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }

            new CustomAlert(sourceWindow)
                           .WithMessage(errorMessage.ToString())
                           .WithMessageType(MessageType.Error)
                           .WithButtonsType(ButtonsType.Ok)
                           .WithTitle("Erro")
                           .ShowAlert();
        }
    }
}
