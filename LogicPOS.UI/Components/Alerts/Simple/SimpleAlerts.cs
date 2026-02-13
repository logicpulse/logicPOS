using Gtk;

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

        public static void ShowInstanceAlreadyRunningAlert()
        {
            Information()
                .WithFlag(DialogFlags.Modal)
                .WithTitleResource("global_information")
                .WithMessageResource("dialog_message_pos_instance_already_running")
                .ShowAlert();
        }
    }
}
