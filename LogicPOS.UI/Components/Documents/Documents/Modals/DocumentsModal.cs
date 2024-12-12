using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.CancelDocument;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Documents
{
    public class DocumentsModal : Modal
    {
        private readonly ISender _meditaor = DependencyInjection.Services.GetRequiredService<IMediator>();
        private DocumentsPage Page { get; set; }
        private string WindowTitleBase => GeneralUtils.GetResourceByName("window_title_select_finance_document");

        private IconButtonWithText BtnPayInvoice = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                       GeneralUtils.GetResourceByName("global_button_label_pay_invoice"),
                                                                                                       PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_full.png");
        private IconButtonWithText BtnNewDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                                       GeneralUtils.GetResourceByName("global_button_label_new_financial_document"),
                                                                                                                       PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_new_document.png");
        private IconButtonWithText BtnPrintDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "touchButton_Green");
        private IconButtonWithText BtnOpenDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument, "touchButton_Green");
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        private IconButtonWithText BtnPrintDocumentAs { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.PrintAs, "touchButton_Green");
        private IconButtonWithText BtnSendDocumentEmail { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.SendEmailDocument, "touchButton_Green");
        private IconButtonWithText BtnCancelDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                         GeneralUtils.GetResourceByName("global_button_label_cancel_document"),
                                                                                         PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");
        private IconButtonWithText BtnPrevious { get; set; }
        private IconButtonWithText BtnNext { get; set; }
        private PageTextBox TxtSearch { get; set; }
        public IconButtonWithText BtnFilter { get; set; }

        public DocumentsModal(Window parent) : base(parent,
                                                    GeneralUtils.GetResourceByName("window_title_select_finance_document"),
                                                    LogicPOSAppContext.MaxWindowSize,
                                                    $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}")
        {

        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();
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

        private void InitializeButtons()
        {
            IconButtonWithText CreateButton(string name,
                                            string label,
                                            string icon)
            {
                return new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = name,
                        Text = label,
                        Font = AppSettings.Instance.fontBaseDialogActionAreaButton,
                        FontColor = AppSettings.Instance.colorBaseDialogActionAreaButtonFont,
                        Icon = PathsSettings.ImagesFolderLocation + icon,
                        IconSize = AppSettings.Instance.sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                        ButtonSize = AppSettings.Instance.sizeBaseDialogActionAreaBackOfficeNavigatorButton
                    });
            }

            BtnPrevious = CreateButton("touchButtonPrev_DialogActionArea",
                                       LocalizedString.Instance["widget_generictreeviewnavigator_record_prev"],
                                       @"Icons/icon_pos_nav_prev.png");

            BtnNext = CreateButton("touchButtonNext_DialogActionArea",
                                   LocalizedString.Instance["widget_generictreeviewnavigator_record_next"],
                                   @"Icons/icon_pos_nav_next.png");

            BtnFilter = CreateButton("touchButtonSearchAdvanced_DialogActionArea",
                                    LocalizedString.Instance["global_button_label_filter"],
                                    @"Icons\icon_pos_filter.png");
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

            var modal = new RePrintDocumentModal(this, Page.SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        private void BtnPayInvoice_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedDocuments.Count == 0)
            {
                return;
            }

            var paidDocuments = string.Join(",", Page.SelectedDocuments.Where(x => x.Paid).Select(x => x.Number));

            if (paidDocuments != string.Empty)
            {
                CustomAlerts.Warning(this)
                            .WithMessage($"Os seguintes documentos já foram pagos: {paidDocuments}")
                            .ShowAlert();
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
            CreateDocumentModal.ShowModal(this);
            Page.Refresh();
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
                                                             RegularExpressions.AlfaNumericExtendedForMotive,
                                                             true);

            if (cancelReasonDialog.ResponseType != ResponseType.Ok)
            {
                return;
            }
            var result = _meditaor.Send(new CancelDocumentCommand { Id = document.Id, Reason = cancelReasonDialog.Text }).Result;

            if (result.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(this, result.FirstError);
                return;
            }

            Page.Refresh();
        }

        private void ShowCannotCancelDocumentMessage(string documentNumber)
        {
            string infoMessage = string.Format(GeneralUtils.GetResourceByName("app_info_show_ignored_cancelled_documents"), documentNumber);

            CustomAlerts.Information(this)
                        .WithSize(new Size(600, 400))
                        .WithTitleResource("global_information")
                        .WithMessage(infoMessage)
                        .ShowAlert();
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
            UpdateModalTitle();
            AddPageEventHandlers();
            UpdateNavigationButtons();
            return fixedContent;
        }

        private void AddPageEventHandlers()
        {
            Page.PageChanged += Page_OnChanged;
            BtnFilter.Clicked += delegate { Page.RunFilter(); };
            BtnPrevious.Clicked += delegate { Page.MoveToPreviousPage(); };
            BtnNext.Clicked += delegate { Page.MoveToNextPage(); };
        }

        private void Page_OnChanged(object sender, EventArgs e)
        {
            UpdateModalTitle();
            UpdateNavigationButtons();
        }

        private void UpdateModalTitle()
        {
            WindowSettings.Title.Text = $"{WindowTitleBase} ({Page.SelectedDocuments.Count}) = {Page.SelectedDocumentsTotalFinal:0.00} " +
                $" - Página {Page.Documents.Page} de {Page.Documents.TotalPages} | Mostrando {Page.Documents.TotalItems}  resultados";
        }

        protected override Widget CreateLeftContent()
        {
            HBox box = new HBox(false, 0);

            TxtSearch = new PageTextBox(this,
                                        LocalizedString.Instance["widget_generictreeviewsearch_search_label"],
                                        isRequired: false,
                                        isValidatable: false,
                                        includeKeyBoardButton: true,
                                        includeSelectButton: false);

            TxtSearch.Component.WidthRequest = LogicPOSAppContext.ScreenSize.Width == 800 && LogicPOSAppContext.ScreenSize.Height == 600 ? 150 : 250;

            box.PackStart(TxtSearch.Component, false, false, 0);
            box.PackStart(BtnFilter, false, false, 0);
            box.PackStart(BtnPrevious, false, false, 0);
            box.PackStart(BtnNext, false, false, 0);

            return box;

        }

        private void UpdateNavigationButtons()
        {
            BtnPrevious.Sensitive = Page.Documents.Page > 1;
            BtnNext.Sensitive = Page.Documents.Page < Page.Documents.TotalPages;

        }
    }
}
