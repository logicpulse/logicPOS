using Gtk;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    internal partial class DocumentsMenuModal : BaseDialog
    {
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
                       AppSettings.Paths.Images + @"Icons\Windows\icon_window_documents.png",
                       GeneralUtils.GetResourceByName("window_title_dialog_document_finance"),
                       new Size(581, 286),
                       table,
                       null);

            AddEventHandlers();
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

    }
}
