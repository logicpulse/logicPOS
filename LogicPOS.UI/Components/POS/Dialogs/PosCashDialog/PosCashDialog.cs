using Gtk;
using System.Drawing;
using LogicPOS.Settings;
using LogicPOS.Utility;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Buttons;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosCashDialog : BaseDialog
    {
        private readonly IconButtonWithText _touchButtonStartStopWorkSessionPeriodDay;
        private readonly IconButtonWithText _touchButtonCashDrawer;

        public PosCashDialog(Window parentWindow, DialogFlags pDialogFlags)
            : base(parentWindow, pDialogFlags)
        {
            //Settings
            string _fileToolbarStartStopWorkSessionPeriodDay = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_start_stop_worksession_period_day.png";
            string _fileToolbarCashDrawer = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_cashdrawer.png";

            //Init Local Vars
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_cash");
            Size windowSize = new Size(428, 205);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_cash_drawer.png";

            Size sizeIcon = new Size(50, 50);
            int buttonWidth = 162;
            int buttonHeight = 88;
            uint tablePadding = 15;

            //Buttons
            _touchButtonStartStopWorkSessionPeriodDay = new IconButtonWithText(new ButtonSettings { Name = "touchButtonStartStopWorkSessionPeriodDay_Green", BackgroundColor = ColorSettings.DefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_worksession_open_day"), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = _fileToolbarStartStopWorkSessionPeriodDay, IconSize = sizeIcon, ButtonSize = new Size(buttonWidth, buttonHeight) });
            _touchButtonCashDrawer = new IconButtonWithText(new ButtonSettings { Name = "touchButtonCashDrawer_Green", BackgroundColor = ColorSettings.DefaultButtonBackground, Text = GeneralUtils.GetResourceByName("pos_button_label_cashdrawer"), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = _fileToolbarCashDrawer, IconSize = sizeIcon, ButtonSize = new Size(buttonWidth, buttonHeight) }) { Sensitive = false };

            //Table
            Table table = new Table(1, 1, true);
            table.BorderWidth = tablePadding;
            //Row 1
            table.Attach(_touchButtonStartStopWorkSessionPeriodDay, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(_touchButtonCashDrawer, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, table, null);

            //Update UI
            UpdateButtons();

            //Events 
            _touchButtonStartStopWorkSessionPeriodDay.Clicked += _touchButtonStartStopWorkSessionPeriodDay_Clicked;
            _touchButtonCashDrawer.Clicked += _touchButtonCashDrawer_Clicked;
        }
    }
}
