using Atk;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class AgtDocumentInfoModal : Modal
    {
        private AgtDocumentInfo _document;

        public AgtDocumentInfoModal(DocumentViewModel document, Window parent) : base(parent,
                                                     "Informação Do Documento (AGT)",
                                                     new Size(550, 560),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_preview.png")
        {
            _document = new AgtDocumentInfo
            {
                Id = document.Id,
                SubmissionDate = document.Agt.SubmissionDate,
                Number = document.Number,
                Type = document.Type,
                RequestId = document.Agt.RequestId,
                SubmissionErrorCode  = document.Agt.SubmissionErrorCode,
                SubmissionErrorDescription = document.Agt.SubmissionErrorDescription,
                HttpStatusCode = document.Agt.HttpStatusCode,
                ValidationResultCode = document.Agt.ValidationResultCode,
                ValidationErrors = document.Agt.ValidationErrors,
                RejectedDocumentNumber = document.Agt.RejectedDocumentNumber,
                ValidationStatus = document.Agt.ValidationStatus
            };
            ShowData();

        }

        public AgtDocumentInfoModal(ReceiptViewModel receipt, Window parent) : base(parent,
                                                     "Informação Do Documento (AGT)",
                                                     new Size(550, 560),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_preview.png")
        {
            _document = new AgtDocumentInfo
            {
                Id = receipt.Id,
                SubmissionDate= receipt.Agt.SubmissionDate,
                Number = receipt.RefNo,
                Type = receipt.RefNo.Substring(0,2),
                RequestId = receipt.Agt.RequestId,
                SubmissionErrorCode = receipt.Agt.SubmissionErrorCode,
                SubmissionErrorDescription = receipt.Agt.SubmissionErrorDescription,
                HttpStatusCode = receipt.Agt.HttpStatusCode,
                ValidationResultCode = receipt.Agt.ValidationResultCode,
                ValidationErrors = receipt.Agt.ValidationErrors,
                RejectedDocumentNumber = receipt.Agt.RejectedDocumentNumber,
                ValidationStatus = receipt.Agt.ValidationStatus
            };
            ShowData();

        }

        private void BtnCorrectDocument_Clicked(object sender, EventArgs e)
        {
            if (_document == null)
            {
                CustomAlerts.Error(this).WithMessage("Este documento foi enviado à AGT").ShowAlert();
                Run();
                return;
            }

            var page = new DocumentsPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentModal = new EntitySelectionModal<DocumentViewModel>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType selectionResponse = (ResponseType)selectDocumentModal.Run();
            var correctDocumentId = page.SelectedEntity.Id;
            selectDocumentModal.Destroy();

            if (selectionResponse != ResponseType.Ok)
            {
                Run();
                return;
            }

            bool advance = CustomAlerts.Question(this).WithMessage($"Enviar o documento {page.SelectedEntity.Number} para corrigir {_document?.Number}? Esta acção é irreversível.").ShowAlert() == ResponseType.Yes;

            if (!advance)
            {
                Run();
                return;
            }

            var correctDocumentResult = AgtService.CorrectDocument(correctDocumentId, _document.Id);

            if (correctDocumentResult)
            {
                CustomAlerts.Information(this).WithMessage($"Correção do documento {_document.Number} submetida com sucesso.").ShowAlert();
            }
            else
            {
                CustomAlerts.Error(this).WithMessage($"Ocorreu um erro ao submeter a correção do documento {_document.Number}.").ShowAlert();
            }
        }

        private void BtnMarkAsValid_Clicked(object sender, EventArgs e)
        {
            if (_document == null)
            {
                CustomAlerts.Error(this).WithMessage("Este documento foi enviado à AGT").ShowAlert();
                Run();
                return;
            }

            bool advance = CustomAlerts.Question(this).WithMessage($"Tem a certeza que consultou o documento {_document?.Number} no portal da AGT e este encontra-se válido? Esta acção é irreversível.").ShowAlert() == ResponseType.Yes;

            if (!advance)
            {
                Run();
                return;
            }

            var markAsValidResult = AgtService.MarkAsValid(_document.Id);

            if (markAsValidResult)
            {
                CustomAlerts.Information(this).WithMessage($"O documento {_document.Number} foi marcado como válido.").ShowAlert();
            }
            else
            {
                CustomAlerts.Error(this).WithMessage($"Ocorreu um erro ao marcar o documento {_document.Number} como válido.").ShowAlert();
            }
        }

        private void ShowData()
        {
            TxtSubmissionDate.Text = _document?.SubmissionDate?.ToString("g") ?? "Não submetido";
            TxtRequestId.Text = _document?.RequestId ?? "Não submetido";
            TxtDocumentNumber.Text = _document?.Number ?? "Não submetido";
            TxtSubmissionErrorCode.Text = _document?.SubmissionErrorCode ?? "-";
            TxtSubmissionErrorDescription.Text = _document?.SubmissionErrorDescription ?? "-";
            TxtHttpStatusCode.Text = _document?.HttpStatusCode?.ToString() ?? "-";
            TxtValidationResultCode.Text = _document?.ValidationResultCode ?? "Não validado";
            TxtValidationStatus.Text = _document?.ValidationStatus ?? "Não validado";
            TxtValidationErrors.Text = _document?.ValidationErrors ?? "-";
            TxtRejectedDocumentNumber.Text = _document?.RejectedDocumentNumber?.ToString() ?? "-";

            if (_document?.ValidationStatus == "V")
            {
                BtnCorrectDocument.Sensitive = false;
                BtnMarkAsValid.Sensitive = false;
            }
        }

        public static void Show(DocumentViewModel document, Window parent)
        {
            var modal = new AgtDocumentInfoModal(document, parent);
            var response = (ResponseType)modal.Run();
            modal.Destroy();
        }

        public static void Show(ReceiptViewModel receipt, Window parent)
        {
            var modal = new AgtDocumentInfoModal(receipt, parent);
            var response = (ResponseType)modal.Run();
            modal.Destroy();
        }


        private class AgtDocumentInfo
        {
            public Guid Id { get; set; }
            public DateTime? SubmissionDate { get; set; }
            public string Number { get; set; }
            public string Type { get; set; }
            public string RequestId { get; set; }
            public string SubmissionErrorCode { get; set; }
            public string SubmissionErrorDescription { get; set; }
            public int? HttpStatusCode { get; set; }
            public string ValidationResultCode { get; set; }
            public string ValidationStatus { get; set; }
            public string ValidationErrors { get; set; }
            public string SubmissionUuid { get; set; }
            public string RejectedDocumentNumber { get; set; }

        }
    }
}
