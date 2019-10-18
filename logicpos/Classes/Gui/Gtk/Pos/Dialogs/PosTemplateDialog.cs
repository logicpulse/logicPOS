using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    //Call this with
    //ex.:
    //PosTemplateDialog dialog = new PosTemplateDialog(dialog, Gtk.DialogFlags.DestroyWithParent);
    //ResponseType response = (ResponseType) dialog.Run();
    //if (response == ResponseType.Ok)
    //{
    //  _log.Debug("ResponseType.Ok");
    //}
    //dialog.Destroy();

    /// <summary>
    /// Base Class for all Pos Dialogs
    /// </summary>
    class PosTemplateDialog : PosBaseDialog
    {
        public PosTemplateDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_template");
            Size windowSize = new Size(600, 340);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_default.png");

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(new Label("Place content here"), 0, 0);

            //ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(buttonCancel, ResponseType.Cancel));

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }
    }
}