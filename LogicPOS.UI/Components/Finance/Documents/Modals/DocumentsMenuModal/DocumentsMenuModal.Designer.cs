using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    internal partial class DocumentsMenuModal
    {
        private void InitializeButtons()
        {
            BtnDocuments = CreateButton("dialog_button_label_select_record_finance_documents", PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_document.png");
            BtnReceiptsEmission = CreateButton("dialog_button_label_select_finance_documents_ft_unpaid", PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_document.png");
            BtnReceipts = CreateButton("dialog_button_label_select_payments", PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_document.png");
            BtnCurrentAccount = CreateButton("dialog_button_label_select_finance_documents_cc", PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_reports.png");
            BtnWorkSessionPeriods = CreateButton("dialog_button_label_select_worksession_period", PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_cashdrawer.png");
            BtnAddStock = CreateButton("dialog_button_label_select_merchandise_entry", PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_merchandise_entry.png");
        }

        private IconButtonWithText CreateButton(string textResource,
                                                string icon)
        {
            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButton_Green",
                    BackgroundColor = ColorSettings.DefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName(textResource),
                    Font = FontSettings.Button,
                    FontColor = ColorSettings.DefaultButtonFont,
                    Icon = icon,
                    IconSize = new Size(50, 50),
                    ButtonSize = new Size(162, 88),
                });
        }
    }
}
