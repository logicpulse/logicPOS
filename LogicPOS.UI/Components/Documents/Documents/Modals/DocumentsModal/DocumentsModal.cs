using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.CancelDocument;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents
{
    public partial class DocumentsModal : Modal
    {
        private readonly ISender _meditaor = DependencyInjection.Services.GetRequiredService<IMediator>();
        private DocumentsPage Page { get; set; }
        private string WindowTitleBase => GeneralUtils.GetResourceByName("window_title_select_finance_document");


        public DocumentsModal(Window parent) : base(parent,
                                                    GeneralUtils.GetResourceByName("window_title_select_finance_document"),
                                                    LogicPOSAppContext.MaxWindowSize,
                                                    $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}")
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            var hasFiscalYear = FiscalYearService.HasFiscalYear();
            BtnNewDocument.Sensitive = hasFiscalYear;
            BtnPayInvoice.Sensitive = hasFiscalYear;
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

        private static bool CanCancelDocument(Document document)
        {
            bool canCancel = true;

            if (document.IsCancelled || document.HasPassed48Hours)
            {
                canCancel = false;
            }
            else if (document.TypeAnalyzer.IsGuide() && document.ShipFromAdress.DeliveryDate < DateTime.Now)
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

        private void AddPageEventHandlers()
        {
            Page.PageChanged += Page_OnChanged;
            BtnFilter.Clicked += delegate { Page.RunFilter(); };
            BtnPrevious.Clicked += delegate { Page.MoveToPreviousPage(); };
            BtnNext.Clicked += delegate { Page.MoveToNextPage(); };
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
