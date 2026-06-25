using Gtk;

namespace LogicPOS.UI.Alerts
{
    public static class SimpleAlerts
    {
        public static SimpleAlert Information(Window parent = null)
        {
            return new SimpleAlert()
                .WithParent(parent)
                .WithFlag(DialogFlags.DestroyWithParent | DialogFlags.Modal)
                .WithMessageType(MessageType.Info);
        }

        public static SimpleAlert Error(Window parent = null)
        {
            return new SimpleAlert()
                .WithParent(parent)
                .WithFlag(DialogFlags.DestroyWithParent | DialogFlags.Modal)
                .WithMessageType(MessageType.Error);
        }

        public static SimpleAlert Warning(Window parent = null)
        {
            return new SimpleAlert()
                .WithParent(parent)
                .WithFlag(DialogFlags.DestroyWithParent | DialogFlags.Modal)
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
