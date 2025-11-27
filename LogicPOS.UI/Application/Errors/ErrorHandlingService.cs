using ErrorOr;
using Gtk;
using LogicPOS.Api.Errors;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using Serilog;
using System;
using System.Text;

namespace LogicPOS.UI.Errors
{
    public class ErrorHandlingService
    {
        public static void HandleApiError<TResult>(ErrorOr<TResult> error, bool closeApplication = false, Window source = null)
        {
            Log.Error("API Error: {Error}", error.Errors);

            if (source == null)
            {
                source = (POSWindow.HasInstance && POSWindow.Instance.Visible) ? POSWindow.Instance : null;
               
                if (source == null)
                {
                    source = BackOfficeWindow.HasInstance ? BackOfficeWindow.Instance : null;
                }
            }

            ShowApiErrorAlert(source, error.FirstError);

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
                errorMessage.AppendLine("Title: " + problemDetails.Title);
                errorMessage.AppendLine($"Detail: {problemDetails.Detail}");
                errorMessage.AppendLine("Status: " + problemDetails.Status);
                errorMessage.AppendLine("Type: " + problemDetails.Type);
                errorMessage.AppendLine("TraceId: " + problemDetails.TraceId);
                errorMessage.AppendLine("Instance: " + problemDetails.Instance);
            }
            else
            {
                errorMessage.AppendLine(error.Description);
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
