using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.shared.App;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    //Call this with
    //ex.:
    //PosTemplateDialog dialog = new PosTemplateDialog(dialog, Gtk.DialogFlags.DestroyWithParent);
    //ResponseType response = (ResponseType) dialog.Run();
    //if (response == ResponseType.Ok)
    //{
    //  _logger.Debug("ResponseType.Ok");
    //}
    //dialog.Destroy();

    /// <summary>
    /// Base Class for all Pos Dialogs
    /// </summary>
    internal class PosTemplateDialog : PosBaseDialog
    {
        public PosTemplateDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_template");
            Size windowSize = new Size(600, 340);
            string fileDefaultWindowIcon = DataLayerFramework.Path["images"] + @"Icons\Windows\icon_window_default.png";

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(new Label("Place content here"), 0, 0);

            //ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(buttonOk, ResponseType.Ok),
                new ActionAreaButton(buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }
    }
}