using Gtk;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.POS
{
    public partial class SessionOpeningModal : Modal
    {
        private IconButtonWithText BtnDayOpening { get; set; }
        private IconButtonWithText BtnSessionOpening { get; set; }

        public SessionOpeningModal(Window parentWindow)
            : base(parentWindow,
                   LocalizedString.Instance["window_title_dialog_cash"],
                   new Size(428, 205),
                   PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_cash_drawer.png")
        {

            
        }

        private Table CreateTable()
        {
            uint tablePadding = 15;
            Table table = new Table(1, 1, true);
            table.BorderWidth = tablePadding;
            table.Attach(BtnDayOpening, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(BtnSessionOpening, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            return table;
        }

        private void InitializeButtons()
        {
            BtnDayOpening = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButton_Green",
                    Text = GeneralUtils.GetResourceByName("global_worksession_open_day"),
                    Font = AppSettings.Instance.fontBaseDialogButton,
                    FontColor = AppSettings.Instance.colorBaseDialogDefaultButtonFont,
                    Icon = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_start_stop_worksession_period_day.png",
                    IconSize = new Size(50, 50),
                    ButtonSize = new Size(162, 88)
                }
            );

            BtnSessionOpening = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButton_Green",
                    Text = GeneralUtils.GetResourceByName("pos_button_label_cashdrawer"),
                    Font = AppSettings.Instance.fontBaseDialogButton,
                    FontColor = AppSettings.Instance.colorBaseDialogDefaultButtonFont,
                    Icon = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_cashdrawer.png",
                    IconSize = new Size(50, 50),
                    ButtonSize = new Size(162, 88)
                }
            );


            UpdateUI();
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return null;
        }

        protected override Widget CreateBody()
        {
            InitializeButtons();

            Table table = CreateTable();

            AddEventHandlers();

            return table;
        }
    }
}
