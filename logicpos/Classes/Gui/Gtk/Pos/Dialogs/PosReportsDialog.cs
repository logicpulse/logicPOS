using Gtk;
using logicpos.App;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosReportsDialog : PosBaseDialog
    {
        public PosReportsDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = Resx.global_reports;
            Size windowSize = new Size(618, 553);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_system.png");

            Size sizeIcon = new Size(50, 50);
            int buttonWidth = 162;
            int buttonHeight = 88;
            uint tablePadding = 15;

            //Icons
            String fileIconDefault = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_default.png");

            //Buttons
            TouchButtonIconWithText buttonReport1 = new TouchButtonIconWithText("touchButtonReport1_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_day, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport2 = new TouchButtonIconWithText("touchButtonReport2_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_zone_table, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport3 = new TouchButtonIconWithText("touchButtonReport3_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_top_register, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport4 = new TouchButtonIconWithText("touchButtonReport4_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_top_closure, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport5 = new TouchButtonIconWithText("touchButtonReport5_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_employees_register, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport6 = new TouchButtonIconWithText("touchButtonReport6_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_employees_closure, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport7 = new TouchButtonIconWithText("touchButtonReport7_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_average_placing, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport8 = new TouchButtonIconWithText("touchButtonReport8_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_zone, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport9 = new TouchButtonIconWithText("touchButtonReport9_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_family, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport10 = new TouchButtonIconWithText("touchButtonReport10_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_terminal, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport11 = new TouchButtonIconWithText("touchButtonReport11_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_offers, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonReport12 = new TouchButtonIconWithText("touchButtonReport12_Green", _colorBaseDialogDefaultButtonBackground, Resx.pos_button_label_report_employees_activity, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconDefault, sizeIcon, buttonWidth, buttonHeight);

            //Table
            Table table = new Table(4, 4, true);
            table.BorderWidth = tablePadding;
            //Row 1
            table.Attach(buttonReport1, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonReport2, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonReport3, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            //Row 2
            table.Attach(buttonReport4, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonReport5, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonReport6, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            //Row 3
            table.Attach(buttonReport7, 0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonReport8, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonReport9, 2, 3, 2, 3, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            //Row 4
            table.Attach(buttonReport10, 0, 1, 3, 4, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonReport11, 1, 2, 3, 4, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonReport12, 2, 3, 3, 4, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, table, null);

            //Events
            buttonReport1.Clicked += buttonReport1_Clicked;
            buttonReport2.Clicked += buttonReport2_Clicked;
            buttonReport3.Clicked += buttonReport3_Clicked;
            buttonReport4.Clicked += buttonReport4_Clicked;
            buttonReport5.Clicked += buttonReport5_Clicked;
            buttonReport6.Clicked += buttonReport5_Clicked;
            buttonReport7.Clicked += buttonReport7_Clicked;
            buttonReport8.Clicked += buttonReport8_Clicked;
            buttonReport9.Clicked += buttonReport9_Clicked;
            buttonReport10.Clicked += buttonReport10_Clicked;
            buttonReport11.Clicked += buttonReport11_Clicked;
            buttonReport12.Clicked += buttonReport12_Clicked;
        }
    }
}
