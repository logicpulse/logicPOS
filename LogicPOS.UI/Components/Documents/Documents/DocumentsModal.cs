using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.CancelDocument;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
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
    public class DocumentsModal : Modal
    {
        private readonly ISender _meditaor = DependencyInjection.Services.GetRequiredService<IMediator>();
        private DocumentsPage Page { get; set; }
        private string WindowTitleBase => GeneralUtils.GetResourceByName("window_title_select_finance_document");

        private IconButtonWithText BtnPayInvoice = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("btnPayInvoice",
                                                                                                       GeneralUtils.GetResourceByName("global_button_label_pay_invoice"),
                                                                                                       PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_full.png");
        private IconButtonWithText BtnNewDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("btnNewDocument",
                                                                                                                       GeneralUtils.GetResourceByName("global_button_label_new_financial_document"),
                                                                                                                       PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_new_document.png");
        private IconButtonWithText BtnPrintDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "btnPrintDocument");
        private IconButtonWithText BtnOpenDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument, "btnOpenDocument");
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        private IconButtonWithText BtnPrintDocumentAs { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.PrintAs, "btnPrintDocumentAs");
        private IconButtonWithText BtnSendDocumentEmail { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.SendEmailDocument, "btnSendDocumentEmail");
        private IconButtonWithText BtnCancelDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("btnCancelDocument",
                                                                                         GeneralUtils.GetResourceByName("global_button_label_cancel_document"),
                                                                                         PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");

        public DocumentsModal(Window parent) : base(parent,
                                                    GeneralUtils.GetResourceByName("window_title_select_finance_document"),
                                                    LogicPOSAppContext.MaxWindowSize,
                                                    $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}")
        {

        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            ColorButtons();

            AddButtonsEventHandlers();

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(BtnPayInvoice, ResponseType.Ok),
                new ActionAreaButton(BtnNewDocument, ResponseType.Ok),
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
            BtnNewDocument.SetBackgroundColor(greenColor);
            BtnPayInvoice.SetBackgroundColor(greenColor);
        }

        private void AddButtonsEventHandlers()
        {
            BtnOpenDocument.Clicked += BtnOpenDocument_Clicked;
            BtnPrintDocumentAs.Clicked += BtnPrintDocumentAs_Clicked;
            BtnCancelDocument.Clicked += BtnCancelDocument_Clicked;
            BtnNewDocument.Clicked += BtnNewDocument_Clicked;
            BtnPayInvoice.Clicked += BtnPayInvoice_Clicked;
            BtnPrintDocument.Clicked += BtnPrintDocument_Clicked;
        }

        private void BtnPrintDocument_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var modal = new RePrintDocumentModal(this,Page.SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        private void BtnPayInvoice_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedDocuments.Count == 0)
            {
                return;
            }

            var modal = new PayInvoiceModal(this, Page.GetSelectedDocumentsWithTotals());
            var response = (ResponseType)modal.Run();
            modal.Destroy();

            if (response == ResponseType.Ok)
            {
                Page.Refresh();
            }
        }

        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            var createDocumentModal = new CreateDocumentModal(this);
            var response = (ResponseType)createDocumentModal.Run();
            createDocumentModal.Destroy();

            if (response == ResponseType.Ok)
            {
                Page.Refresh();
            }
        }

        private void BtnCancelDocument_Clicked(object sender, EventArgs e)
        {
            var selectedDocument = Page.SelectedEntity;
            if (selectedDocument == null)
            {
                return;
            }

            if (CanCancelDocument(selectedDocument) == false)
            {
                ShowCannotCancelDocumentMessage(selectedDocument.Number);
                return;
            }

            CancelDocument(selectedDocument);
        }

        private static bool CanCancelDocument(Document selectedDocument)
        {
            bool canCancel = true;

            if (selectedDocument.IsCancelled || selectedDocument.HasPassed48Hours)
            {
                canCancel = false;
            }
            else if (selectedDocument.IsGuide() && selectedDocument.ShipFromAdress.DeliveryDate < DateTime.Now)
            {
                canCancel = false;
            }

            return canCancel;
        }

        private void CancelDocument(Document document)
        {
            var cancelReasonDialog = logicpos.Utils.GetInputText(this,
                                                             DialogFlags.Modal,
                                                             PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_input_text_default.png",
                                                             string.Format(GeneralUtils.GetResourceByName("global_cancel_document_input_text_label"), document.Number),
                                                             string.Empty,
                                                             RegexUtils.RegexAlfaNumericExtendedForMotive,
                                                             true);

            if (cancelReasonDialog.ResponseType != ResponseType.Ok)
            {
                return;
            }
            var result = _meditaor.Send(new CancelDocumentCommand { Id = document.Id, Reason = cancelReasonDialog.Text }).Result;

            if (result.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this,result.FirstError);
                return;
            }

            Page.Refresh();
        }

        private void ShowCannotCancelDocumentMessage(string documentNumber)
        {
            string infoMessage = string.Format(GeneralUtils.GetResourceByName("app_info_show_ignored_cancelled_documents"), documentNumber);
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
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var modal = new RePrintDocumentModal(this, Page.SelectedEntity);
            modal.Run();
            modal.Destroy();

            var pdfLocation = DocumentPdfUtils.GetDocumentPdfFileLocation(Page.SelectedEntity.Id);

            if (pdfLocation == null)
            {
                return;
            }

            DocumentPrintingUtils.PrintWithNativeDialog(pdfLocation);
        }

        private void BtnOpenDocument_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity != null)
            {
                DocumentPdfUtils.ViewDocumentPdf(this, Page.SelectedEntity.Id);
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
            var page = new DocumentsPage(this, PageOptions.SelectionPageOptions);
            page.SetSizeRequest(WindowSettings.Size.Width - 14, WindowSettings.Size.Height - 124);
            Fixed fixedContent = new Fixed();
            fixedContent.Put(page, 0, 0);
            Page = page;
            AddPageEventHandlers();
            return fixedContent;
        }

        private void AddPageEventHandlers()
        {
            Page.EntitySelected += OnDocumentSelected;
            Page.DocumentsSelectionChanged += Page_DocumentsSelectionChanged;
        }

        private void Page_DocumentsSelectionChanged(object sender, EventArgs e)
        {
            WindowSettings.Title.Text = $"{WindowTitleBase} ({Page.SelectedDocuments.Count}) = {Page.SelectedDocumentsTotalFinal:0.00}";
        }

        private void OnDocumentSelected(Document document)
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
