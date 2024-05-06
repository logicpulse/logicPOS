﻿using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using System.Drawing;
using LogicPOS.Settings.Extensions;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosCashDialog : PosBaseDialog
    {
        private readonly TouchButtonIconWithText _touchButtonStartStopWorkSessionPeriodDay;
        private readonly TouchButtonIconWithText _touchButtonCashDrawer;

        public PosCashDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Settings
            string _fileToolbarStartStopWorkSessionPeriodDay = DataLayerFramework.Path["images"] + @"Icons\icon_pos_toolbar_start_stop_worksession_period_day.png";
            string _fileToolbarCashDrawer = DataLayerFramework.Path["images"] + @"Icons\icon_pos_toolbar_cashdrawer.png";

            //Init Local Vars
            string windowTitle = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_cash");
            Size windowSize = new Size(428, 205);
            string fileDefaultWindowIcon = DataLayerFramework.Path["images"] + @"Icons\Windows\icon_window_cash_drawer.png";

            Size sizeIcon = new Size(50, 50);
            int buttonWidth = 162;
            int buttonHeight = 88;
            uint tablePadding = 15;

            //Buttons
            _touchButtonStartStopWorkSessionPeriodDay = new TouchButtonIconWithText("touchButtonStartStopWorkSessionPeriodDay_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_worksession_open_day"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileToolbarStartStopWorkSessionPeriodDay, sizeIcon, buttonWidth, buttonHeight);
            _touchButtonCashDrawer = new TouchButtonIconWithText("touchButtonCashDrawer_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "pos_button_label_cashdrawer"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileToolbarCashDrawer, sizeIcon, buttonWidth, buttonHeight) { Sensitive = false };

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
