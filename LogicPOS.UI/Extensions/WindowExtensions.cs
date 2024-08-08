using Gtk;

namespace LogicPOS.UI.Extensions
{
    public static class WindowExtensions
    {
        public static void DisableClicks(this Window window)
        {
            window.CanFocus = false;
            window.AcceptFocus = false;
        }

        public static void EnableClicks(this Window window)
        {
            window.CanFocus = true;
            window.AcceptFocus = true;
        }
    }
}
