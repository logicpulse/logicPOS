using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.Receipts.CancelReceipt;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReceiptsModal : Modal
    {
        private readonly ISender _meditaor = DependencyInjection.Services.GetRequiredService<IMediator>();

        public ReceiptsModal(Window parent) : base(parent,
                                                   GeneralUtils.GetResourceByName("window_title_dialog_document_finance_payment"),
                                                   AppSettings.MaxWindowSize,
                                                   $"{AppSettings.Paths.Images}{@"Icons/Windows/icon_window_select_record.png"}")
        {

        }

        private void ShowCannotCancelReceiptMessage(string refNo)
        {
            string infoMessage = string.Format(GeneralUtils.GetResourceByName("app_info_show_ignored_cancelled_documents"), refNo);

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
            WindowSettings.Title.Text = $"{WindowTitleBase} ({Page.SelectedReceipts.Count}) = {Page.SelectedReceiptsTotalAmount:0.00} " +
                $" - Página {Page.Receipts.Page} de {Page.Receipts.TotalPages} | Mostrando {Page.Receipts.ItemsCount}  resultados";
        }

        private void UpdateNavigationButtons()
        {
            BtnPrevious.Sensitive = Page.Receipts.Page > 1;
            BtnNext.Sensitive = Page.Receipts.Page < Page.Receipts.TotalPages;
        }
        private void CancelReceipt(ReceiptViewModel receipt)
        {
            var cancelReasonDialog = logicpos.Utils.GetInputText(this,
                                                             DialogFlags.Modal,
                                                             AppSettings.Paths.Images + @"Icons\Windows\icon_window_input_text_default.png",
                                                             string.Format(GeneralUtils.GetResourceByName("global_cancel_document_input_text_label"), receipt.RefNo),
                                                             string.Empty,
                                                             RegularExpressions.AlfaNumericExtendedForMotive,
                                                             true);

            if (cancelReasonDialog.ResponseType != ResponseType.Ok)
            {
                return;
            }
            var result = _meditaor.Send(new CancelReceiptCommand { Id = receipt.Id, Reason = cancelReasonDialog.Text }).Result;

            if (result.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(this, result.FirstError);
                return;
            }

            Page.Refresh();
        }

        private static bool CanCancelReceipt(ReceiptViewModel receipt)
        {
            bool canCancel = true;

            if (receipt.IsCancelled || receipt.HasPassed48Hours)
            {
                canCancel = false;
            }

            return canCancel;
        }
    }
}
