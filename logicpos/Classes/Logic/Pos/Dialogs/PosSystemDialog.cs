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
                var resultOpenDoor = PrintRouter.OpenDoor(GlobalFramework.LoggedTerminal.Printer);
                if (!resultOpenDoor)
                {
                    Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "open_cash_draw_permissions")));
                }
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
