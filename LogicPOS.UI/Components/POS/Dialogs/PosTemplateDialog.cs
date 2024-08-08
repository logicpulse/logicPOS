using Gtk;
using System.Drawing;
using LogicPOS.Settings;
using LogicPOS.Utility;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Buttons;

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
    internal class PosTemplateDialog : BaseDialog
    {
        public PosTemplateDialog(Window parentWindow, DialogFlags pDialogFlags)
            : base(parentWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_template");
            Size windowSize = new Size(600, 340);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_default.png";

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(new Label("Place content here"), 0, 0);

            //ActionArea Buttons
            IconButtonWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            IconButtonWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(buttonOk, ResponseType.Ok),
                new ActionAreaButton(buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }
    }
}