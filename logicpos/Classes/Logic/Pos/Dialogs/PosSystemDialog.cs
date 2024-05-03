using Gtk;
using logicpos.datalayer.App;
using logicpos.financial.library.Classes.Hardware.Printers;
using System;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosSystemDialog
    {
        private void buttonSetup_Clicked(object sender, EventArgs e)
        {
            logicpos.Utils.ShowMessageUnderConstruction();
        }

        private void buttonCash_Clicked(object sender, EventArgs e)
        {
            if (DataLayerFramework.LoggedTerminal.Printer != null)
            {
                var resultOpenDoor = PrintRouter.OpenDoor(DataLayerFramework.LoggedTerminal.Printer);
                if (!resultOpenDoor)
                {
                    logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_information"), string.Format(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "open_cash_draw_permissions")));
                }
            }
        }

        private void buttonReports_Clicked(object sender, EventArgs e)
        {
            PosReportsDialog dialog = new PosReportsDialog(this, DialogFlags.DestroyWithParent);
            int response = dialog.Run();
            dialog.Destroy();
        }
    }
}
