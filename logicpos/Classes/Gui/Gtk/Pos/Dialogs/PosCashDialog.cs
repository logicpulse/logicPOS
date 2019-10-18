using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosCashDialog : PosBaseDialog
    {
        private TouchButtonIconWithText _touchButtonStartStopWorkSessionPeriodDay;
        private TouchButtonIconWithText _touchButtonCashDrawer;

        public PosCashDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Settings
            String _fileToolbarStartStopWorkSessionPeriodDay = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_toolbar_start_stop_worksession_period_day.png");
            String _fileToolbarCashDrawer = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_toolbar_cashdrawer.png");

            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_cash");
            Size windowSize = new Size(428, 205);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_cash_drawer.png");

            Size sizeIcon = new Size(50, 50);
            int buttonWidth = 162;
            int buttonHeight = 88;
            uint tablePadding = 15;

            //Buttons
            _touchButtonStartStopWorkSessionPeriodDay = new TouchButtonIconWithText("touchButtonStartStopWorkSessionPeriodDay_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_open_day"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileToolbarStartStopWorkSessionPeriodDay, sizeIcon, buttonWidth, buttonHeight);
            _touchButtonCashDrawer = new TouchButtonIconWithText("touchButtonCashDrawer_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_button_label_cashdrawer"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileToolbarCashDrawer, sizeIcon, buttonWidth, buttonHeight) { Sensitive = false };

            //Table
            Table table = new Table(1, 1, true);
            table.BorderWidth = tablePadding;
            //Row 1
            table.Attach(_touchButtonStartStopWorkSessionPeriodDay, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(_touchButtonCashDrawer, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, table, null);

            //Update UI
            UpdateButtons();

            //Events 
            _touchButtonStartStopWorkSessionPeriodDay.Clicked += _touchButtonStartStopWorkSessionPeriodDay_Clicked;
            _touchButtonCashDrawer.Clicked += _touchButtonCashDrawer_Clicked;
        }
    }
}
