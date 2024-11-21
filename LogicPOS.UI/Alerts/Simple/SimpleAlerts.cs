using ErrorOr;
using Gtk;
using logicpos;
using LogicPOS.Api.Errors;
using System.Text;

namespace LogicPOS.UI.Alerts
{
    public static class SimpleAlerts
    {
        public static SimpleAlert Information()
        {
            return new SimpleAlert()
                .WithFlag(DialogFlags.DestroyWithParent)
                .WithMessageType(MessageType.Info);
        }

        public static SimpleAlert Error()
        {
            return new SimpleAlert()
                .WithFlag(DialogFlags.DestroyWithParent)
                .WithMessageType(MessageType.Error);
        }

        public static SimpleAlert Warning()
        {
            return new SimpleAlert()
                .WithFlag(DialogFlags.DestroyWithParent)
                .WithMessageType(MessageType.Warning);
        }

        public static SimpleAlert Question()
        {
            return new SimpleAlert()
                .WithButton(ButtonsType.YesNo)
                .WithFlag(DialogFlags.DestroyWithParent)
                .WithMessageType(MessageType.Question);
        }

        public static void ShowUnderConstructionAlert()
        {
            Error().WithMessageResource("dialog_message_under_construction_function")
                   .ShowAlert();
        }

        public static void ShowOperationSucceededAlert(string titleResource)
        {
            Information()
                .WithTitleResource(titleResource)
                .WithMessageResource("dialog_message_operation_successfully")
                .ShowAlert();
        }

        public static void ShowInstanceAlreadyRunningAlert()
        {
            Information()
                .WithFlag(DialogFlags.Modal)
                .WithTitleResource("global_information")
                .WithMessageResource("dialog_message_pos_instance_already_running")
                .ShowAlert();
        }

        public static void ShowCompositeArticleTheSameAlert(Window parent)
        {
            Warning()
                .WithParent(parent)
                .WithTitleResource("global_composite_article")
                .WithMessageResource("dialog_message_composite_article_same")
                .ShowAlert();
        }

        public static void ShowApiErrorAlert(Error error)
        {
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine("Code: " + error.Code);
            errorMessage.AppendLine("Description: " + error.Description);


            var metadata = error.Metadata;

            if (metadata != null)
            {
                var problemDetails = (ProblemDetails)metadata["problem"];

                errorMessage.AppendLine("\nProblem Details:");
                errorMessage.AppendLine("Title: " + problemDetails.Title);
                errorMessage.AppendLine("Status: " + problemDetails.Status);
                errorMessage.AppendLine("Type: " + problemDetails.Type);
                errorMessage.AppendLine("TraceId: " + problemDetails.TraceId);

                foreach (var problemDetailsError in problemDetails.Errors)
                {
                    errorMessage.AppendLine("\nError:");
                    errorMessage.AppendLine("Name: " + problemDetailsError.Name);
                    errorMessage.AppendLine("Reson: " + problemDetailsError.Reason);
                }
            }

          Error().WithMessage(errorMessage.ToString())
                 .WithMessageType(MessageType.Error)
                 .WithTitle("Erro")
                 .ShowAlert();
        }
    }
}
