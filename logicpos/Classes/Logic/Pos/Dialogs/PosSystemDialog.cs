using Gtk;
using logicpos.App;
using logicpos.financial.library.Classes.Hardware.Printers;
using System;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosSystemDialog
    {
        void buttonSetup_Clicked(object sender, EventArgs e)
        {
            Utils.ShowMessageUnderConstruction();
        }

        void buttonCash_Clicked(object sender, EventArgs e)
        {
            if (GlobalFramework.LoggedTerminal.Printer != null)
            {
                PrintRouter.OpenDoor(GlobalFramework.LoggedTerminal.Printer);
            }
        }

        void buttonReports_Clicked(object sender, EventArgs e)
        {
            PosReportsDialog dialog = new PosReportsDialog(this, DialogFlags.DestroyWithParent);
            int response = dialog.Run();
            dialog.Destroy();
        }
    }
}
