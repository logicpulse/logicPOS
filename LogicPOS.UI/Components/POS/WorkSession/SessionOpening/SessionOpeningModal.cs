using Gtk;
using System.Drawing;
using LogicPOS.Settings;
using LogicPOS.Utility;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Buttons;

namespace LogicPOS.UI.Components.POS
{
    public partial class SessionOpeningModal : BaseDialog
    {
        private IconButtonWithText BtnDayOpening { get; set; }
        private IconButtonWithText BtnSessionOpening { get; set; }

        public SessionOpeningModal(Window parentWindow,
                                   DialogFlags dialogFlags)
            : base(parentWindow, dialogFlags)
        {
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_cash");
            Size windowSize = new Size(428, 205);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_cash_drawer.png";


            InitializeButtons();

            Table table = CreateTable();

            this.Initialize(this,
                            dialogFlags,
                            fileDefaultWindowIcon,
                            windowTitle,
                            windowSize,
                            table,
                            null);

            AddEventHandlers();
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
                    Font = FontSettings.Button,
                    FontColor = ColorSettings.DefaultButtonFont,
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
                    Font = FontSettings.Button,
                    FontColor = ColorSettings.DefaultButtonFont,
                    Icon = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_cashdrawer.png",
                    IconSize = new Size(50, 50),
                    ButtonSize = new Size(162, 88)
                }
            );


            UpdateButtons();
        }
    }
}
