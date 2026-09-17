using Gtk;
using System;

namespace LogicPOS.UI.Extensions
{
    public static class WindowExtensions
    {
        public static void DisableClicks(this Window window)
        {
            if (window == null)
            {
                return;
            }

            window.CanFocus = false;
            window.AcceptFocus = false;
        }

        public static void EnableClicks(this Window window)
        {
            if (window == null)
            {
                return;
            }

            window.CanFocus = true;
            window.AcceptFocus = true;
        }

        public static int RunWithDisabledParent(this Dialog dialog, Window parent)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            if (parent != null)
            {
                dialog.TransientFor = parent;
            }

            dialog.Present();

            parent?.DisableClicks();
            try
            {
                return dialog.Run();
            }
            finally
            {
                parent?.EnableClicks();
            }
        }
    }
}
