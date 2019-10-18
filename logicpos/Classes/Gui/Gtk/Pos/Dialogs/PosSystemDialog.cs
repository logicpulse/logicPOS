using System;
using Gtk;
using System.Drawing;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.App;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosSystemDialog : PosBaseDialog
    {
        public PosSystemDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_system");
            Size windowSize = new Size(620, 205/*321 2 rows*/);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_system.png");

            Size sizeIcon = new Size(50, 50);
            int buttonWidth = 162;
            int buttonHeight = 88;
            uint tablePadding = 15;

            //Icons
            String fileIconConfiguration = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_configuration.png");
            String fileIconCashRegister = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_ticketpad_cashregister.png");
            String fileIconReports = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_reports.png");

            //Buttons
            TouchButtonIconWithText buttonSetup = new TouchButtonIconWithText("touchButtonSetup_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_application_setup"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconConfiguration, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonCash = new TouchButtonIconWithText("touchButtonCash_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_button_label_cashdrawer"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconCashRegister, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReports = new TouchButtonIconWithText("touchButtonReports_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_reports"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconReports, sizeIcon, buttonWidth, buttonHeight);
            //buttonDisable1.Sensitive = false;

            //Table
            Table table = new Table(2, 4, true);
            table.BorderWidth = tablePadding;
            //Row 1
            table.Attach(buttonSetup, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonCash, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonReports, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            //Row 2
            //table.Attach(buttonFiles, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            //table.Attach(buttonDisable1, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            //table.Attach(buttonDisable2, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, table, null);

            //Events
            buttonSetup.Clicked += buttonSetup_Clicked;
            buttonCash.Clicked += buttonCash_Clicked;
            buttonReports.Clicked += buttonReports_Clicked;
        }
    }
}
