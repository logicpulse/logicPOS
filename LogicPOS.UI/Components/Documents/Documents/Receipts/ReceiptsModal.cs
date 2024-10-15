using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.CancelDocument;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents
{
    public class ReceiptsModal : Modal
    {
        private readonly ISender _meditaor = DependencyInjection.Services.GetRequiredService<IMediator>();
        private ReceiptsPage Page { get; set; }
        private string WindowTitleBase => GeneralUtils.GetResourceByName("window_title_dialog_document_finance_payment");

        private IconButtonWithText BtnPrintDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "btnPrintDocument");
        private IconButtonWithText BtnOpenDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument, "btnOpenDocument");
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        private IconButtonWithText BtnPrintDocumentAs { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.PrintAs, "btnPrintDocumentAs");
        private IconButtonWithText BtnSendDocumentEmail { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.SendEmailDocument, "btnSendDocumentEmail");
        private IconButtonWithText BtnCancelDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("btnCancelDocument",
                                                                                         GeneralUtils.GetResourceByName("global_button_label_cancel_document"),
                                                                                         PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");

        public ReceiptsModal(Window parent) : base(parent,
                                                   GeneralUtils.GetResourceByName("window_title_dialog_document_finance_payment"),
                                                   GlobalApp.MaxWindowSize,
                                                   $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}")
        {

        }



        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            ColorButtons();

            AddButtonsEventHandlers();

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(BtnPrintDocument, ResponseType.Ok),
                new ActionAreaButton(BtnPrintDocumentAs, ResponseType.Ok),
                new ActionAreaButton(BtnCancelDocument, ResponseType.Ok),
                new ActionAreaButton(BtnOpenDocument, ResponseType.Ok),
                new ActionAreaButton(BtnSendDocumentEmail, ResponseType.Ok),
                new ActionAreaButton(BtnClose, ResponseType.Close),
            };
            return actionAreaButtons;
        }

        private void ColorButtons()
        {
            var greenColor = Color.FromArgb(140, 187, 59);
            BtnPrintDocument.SetBackgroundColor(greenColor);
            BtnPrintDocumentAs.SetBackgroundColor(greenColor);
            BtnOpenDocument.SetBackgroundColor(greenColor);
            BtnSendDocumentEmail.SetBackgroundColor(greenColor);
            BtnCancelDocument.SetBackgroundColor(greenColor);
        }

        private void AddButtonsEventHandlers()
        {
            BtnOpenDocument.Clicked += BtnOpenDocument_Clicked;
            BtnPrintDocumentAs.Clicked += BtnPrintDocumentAs_Clicked;
            BtnCancelDocument.Clicked += BtnCancelReceipt_Clicked;
        }

        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            var createDocumentModal = new CreateDocumentModal(this);
            createDocumentModal.Run();
            createDocumentModal.Destroy();
        }

        private void BtnCancelReceipt_Clicked(object sender, EventArgs e)
        {
            var receipt = Page.SelectedEntity;
            if (receipt == null)
            {
                return;
            }

            if (CanCancelReceipt(receipt) == false)
            {
                ShowCannotCancelReceiptMessage(receipt.RefNo);
                return;
            }

            CancelReceipt(receipt);
        }

        private static bool CanCancelReceipt(Receipt receipt)
        {
            bool canCancel = true;

            if (receipt.IsCancelled || receipt.HasPassed48Hours)
            {
                canCancel = false;
            }

            return canCancel;
        }

        private void CancelReceipt(Receipt receipt)
        {
            var cancelReasonDialog = logicpos.Utils.GetInputText(this,
                                                             DialogFlags.Modal,
                                                             PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_input_text_default.png",
                                                             string.Format(GeneralUtils.GetResourceByName("global_cancel_document_input_text_label"), receipt.RefNo),
                                                             string.Empty,
                                                             RegexUtils.RegexAlfaNumericExtendedForMotive,
                                                             true);

            if (cancelReasonDialog.ResponseType != ResponseType.Ok)
            {
                return;
            }
            var result = _meditaor.Send(new CancelDocumentCommand { Id = receipt.Id, Reason = cancelReasonDialog.Text }).Result;

            if (result.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this, result.FirstError);
                return;
            }

            Page.Refresh();
        }

        private void ShowCannotCancelReceiptMessage(string refNo)
        {
            string infoMessage = string.Format(GeneralUtils.GetResourceByName("app_info_show_ignored_cancelled_documents"), refNo);
            logicpos.Utils.ShowMessageBox(this,
                                          DialogFlags.Modal,
                                          new Size(600, 400),
                                          MessageType.Info,
                                          ButtonsType.Ok,
                                          GeneralUtils.GetResourceByName("global_information"),
                                          infoMessage);
        }

        private void BtnPrintDocumentAs_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity != null)
            {
                var pdfLocation = DocumentPdfUtils.GetReceiptPdfFileLocation(Page.SelectedEntity.Id);

                if (pdfLocation == null)
                {
                    return;
                }

                DocumentPrintingUtils.PrintWithNativeDialog(pdfLocation);
            }
        }

        private void BtnOpenDocument_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity != null)
            {
                DocumentPdfUtils.ViewReceiptPdf(this, Page.SelectedEntity.Id);
            }
        }

        protected override void OnResponse(ResponseType response)
        {
            if (response != ResponseType.Close)
            {
                Run();
            }

            base.OnResponse(response);
        }

        protected override Widget CreateBody()
        {
            var page = new ReceiptsPage(this, PageOptions.SelectionPageOptions);
            page.SetSizeRequest(WindowSettings.Size.Width - 14, WindowSettings.Size.Height - 124);
            Fixed fixedContent = new Fixed();
            fixedContent.Put(page, 0, 0);
            Page = page;
            AddPageEventHandlers();
            return fixedContent;
        }

        private void AddPageEventHandlers()
        {
            Page.EntitySelected += OnReceiptSelected;
            Page.ReceiptsSelectionChanged += Page_ReceiptsSelectionChanged;
        }

        private void Page_ReceiptsSelectionChanged(object sender, EventArgs e)
        {
            WindowSettings.Title.Text = $"{WindowTitleBase} ({Page.SelectedReceipts.Count}) = {Page.SelectedReceiptsTotalAmount:0.00}";
        }

        private void OnReceiptSelected(Receipt receipt)
        {

        }

        protected override Widget CreateLeftContent()
        {
            return CreateSearchBox();
        }

        private Widget CreateSearchBox()
        {
            var searchBox = new PageSearchBox(Page.SourceWindow, true);
            searchBox.TxtSearch.EntryValidation.Changed += delegate
            {
                Page.Navigator.SearchBox.TxtSearch.EntryValidation.Text = searchBox.TxtSearch.EntryValidation.Text;
            };

            return searchBox;
        }
    }
}
