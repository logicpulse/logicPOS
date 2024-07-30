using Gtk;

namespace LogicPOS.UI.Alerts
{
    public static class Alerts
    {
        public static Alert Information()
        {
            return new Alert()
                .WithFlag(DialogFlags.DestroyWithParent)
                .WithMessageType(MessageType.Info);
        }

        public static Alert Error()
        {
            return new Alert()
                .WithFlag(DialogFlags.DestroyWithParent)
                .WithMessageType(MessageType.Error);
        }

        public static Alert Warning()
        {
            return new Alert()
                .WithFlag(DialogFlags.DestroyWithParent)
                .WithMessageType(MessageType.Warning);
        }

        public static Alert Question()
        {
            return new Alert()
                .WithButton(ButtonsType.YesNo)
                .WithFlag(DialogFlags.DestroyWithParent)
                .WithMessageType(MessageType.Question);
        }

        public static void ShowUnderConstructionAlert()
        {
            Error().WithMessageResource("dialog_message_under_construction_function")
                   .Show();
        }

        public static void ShowOperationSucceededAlert(string titleResource)
        {
            Information()
                .WithTitleResource(titleResource)
                .WithMessageResource("dialog_message_operation_successfully")
                .Show();
        }

        public static void ShowInstanceAlreadyRunningAlert()
        {
            Information()
                .WithFlag(DialogFlags.Modal)
                .WithTitleResource("global_information")
                .WithMessageResource("dialog_message_pos_instance_already_running")
                .Show();
        }

        public static void ShowCompositeArticleTheSameAlert(Window parent)
        {
            Warning()
                .WithParent(parent)
                .WithTitleResource("global_composite_article")
                .WithMessageResource("dialog_message_composite_article_same")
                .Show();
        }
    }
}
