using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Dialogs;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents
{
    internal partial class DocumentsMenuModal : BaseDialog
    {
        private IconButtonWithText BtnDocuments { get; set; }
        private IconButtonWithText BtnReceiptsEmission { get; set; }
        private IconButtonWithText BtnReceipts { get; set; }
        private IconButtonWithText BtnCurrentAccount { get; set; }
        private IconButtonWithText BtnWorkSessionPeriods { get; set; }
        private IconButtonWithText BtnAddStock { get; set; }

        public DocumentsMenuModal(Window parentWindow,
                                  DialogFlags flags = DialogFlags.DestroyWithParent)
            : base(parentWindow, flags)
        {
            WindowSettings.Source = parentWindow;

            InitializeButtons();

            uint tablePadding = 10;
            Table table = new Table(1, 1, true);
            table.BorderWidth = tablePadding;
            //Row 1
            table.Attach(BtnDocuments, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(BtnReceiptsEmission, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(BtnReceipts, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            //Row 2
            table.Attach(BtnCurrentAccount, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(BtnWorkSessionPeriods, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(BtnAddStock, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);

            Initialize(this,
                       flags,
                       PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_documents.png",
                       GeneralUtils.GetResourceByName("window_title_dialog_document_finance"),
                       new Size(581, 286),
                       table,
                       null);

            AddEventHandlers();
        }

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
                    BackgroundColor = ColorSettings.DefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName(textResource),
                    Font = FontSettings.Button,
                    FontColor = ColorSettings.DefaultButtonFont,
                    Icon = icon,
                    IconSize = new Size(50, 50),
                    ButtonSize = new Size(162, 88),
                });
        }

        private void AddEventHandlers()
        {
            BtnDocuments.Clicked += BtnDocuments_Clicked;
            BtnReceiptsEmission.Clicked += BtnDocuments_Clicked;
            BtnCurrentAccount.Clicked += BtnCurrentAccount_Clicked;
            BtnReceipts.Clicked += BtnReceipts_Clicked;
            BtnWorkSessionPeriods.Clicked += BtnWorkSessionPeriods_Clicked;
            BtnAddStock.Clicked += BtnAddStock_Clicked;
        }

        private void BtnAddStock_Clicked(object sender, EventArgs e)
        {
            var addStockModal = new AddStockModal(WindowSettings.Source);
            addStockModal.Run();
            addStockModal.Destroy();
        }

        private void BtnWorkSessionPeriods_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnReceipts_Clicked(object sender, EventArgs e)
        {
            var modal = new ReceiptsModal(WindowSettings.Source);
            modal.Run();
            modal.Destroy();
        }

        private void BtnCurrentAccount_Clicked(object sender, EventArgs e)
        {
            var modal = new CustomerCurrentAccountFilterModal(WindowSettings.Source);
            modal.Run();
            modal.Destroy();
        }

        private void BtnDocuments_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(WindowSettings.Source);
            modal.Run();
            modal.Destroy();
        }
    }
}
