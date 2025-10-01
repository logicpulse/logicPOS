using Gtk;
using LogicPOS.Api.Features.Documents.CancelDocument;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentsModal : Modal
    {
        private readonly ISender _meditaor = DependencyInjection.Services.GetRequiredService<IMediator>();
        public DocumentsPage Page { get; set; }
        private string WindowTitleBase => GeneralUtils.GetResourceByName("window_title_select_finance_document");
        private bool _selectionMode;

        public DocumentsModal(Window parent,
                              bool selectionMode = false) : base(parent,
                                                    GeneralUtils.GetResourceByName("window_title_select_finance_document"),
                                                    AppSettings.MaxWindowSize,
                                                    $"{AppSettings.Paths.Images}{@"Icons/Windows/icon_window_select_record.png"}")
        {
            _selectionMode = selectionMode;

            if (selectionMode)
            {
                UseSelectionMode();
            }
            else
            {
                BtnOk.Visible = false;
                BtnCancel.Visible = false;
            }

            UpdateUI();
        }

        private void UseSelectionMode()
        {
            BtnPrintDocument.Visible = false;
            BtnPrintDocumentAs.Visible = false;
            BtnSendDocumentEmail.Visible = false;
            BtnNewDocument.Visible = false;
            BtnPayInvoice.Visible = false;
            BtnCancelDocument.Visible = false;
            BtnPrintDocument.Visible = false;
            BtnOpenDocument.Visible = false;
            BtnClose.Visible = false;
            BtnOk.Visible = true;
            BtnCancel.Visible = true;
            BtnCloneDocument.Visible = false;
            BtnEditDraft.Visible = false;
        }

        private static bool CanCancelDocument(DocumentViewModel document)
        {
            bool canCancel = true;

            if (document.IsCancellable)
            {
                canCancel = false;
            }
            else if (document.TypeAnalyzer.IsGuide() && document.ShipFromAddressDeliveryDate < DateTime.Now)
            {
                canCancel = false;
            }

            return canCancel;
        }

        private void CancelDocument(DocumentViewModel document)
        {
            var cancelReasonDialog = logicpos.Utils.GetInputText(this,
                                                             DialogFlags.Modal,
                                                             AppSettings.Paths.Images + @"Icons\Windows\icon_window_input_text_default.png",
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
                ErrorHandlingService.HandleApiError(result);
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

        private void AddPageEventHandlers()
        {
            Page.PageChanged += Page_OnChanged;
            BtnFilter.Clicked += delegate { Page.RunFilter(); };
            BtnPrevious.Clicked += delegate { Page.MoveToPreviousPage(); };
            BtnNext.Clicked += delegate { Page.MoveToNextPage(); };
            Page.EntitySelected += delegate { UpdateUI(); };
        }

        private void UpdateModalTitle()
        {
            WindowSettings.Title.Text = $"{WindowTitleBase} ({Page.SelectedDocuments.Count}) = {Page.SelectedDocumentsTotalFinal:0.00} " +
                $" - Página {Page.Documents.Page} de {Page.Documents.TotalPages} | Mostrando {Page.Documents.ItemsCount}  resultados";
        }

        private void UpdateNavigationButtons()
        {
            BtnPrevious.Sensitive = Page.Documents.Page > 1;
            BtnNext.Sensitive = Page.Documents.Page < Page.Documents.TotalPages;

        }
    }
}
