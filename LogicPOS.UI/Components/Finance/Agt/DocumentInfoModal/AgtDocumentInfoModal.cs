using Gtk;
using LogicPOS.Api.Features.Finance.Agt.Common;
using LogicPOS.Api.Features.Finance.Agt.CorrectDocument;
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
        private AgtDocument _agtDocument;
        public AgtDocumentInfoModal(AgtDocument agtDocument, Window parent) : base(parent,
                                                     "Informação Do Documento (AGT)",
                                                     new Size(550, 560),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_preview.png")
        {
            _agtDocument = agtDocument;
            ShowData();
        }

        private void BtnCorrectDocument_Clicked(object sender, EventArgs e)
        {
            if(_agtDocument == null)
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

            bool advance = CustomAlerts.Question(this).WithMessage($"Enviar o documento {page.SelectedEntity.Number} para corrigir {_agtDocument?.Number}? Esta acção é irreversível.").ShowAlert() == ResponseType.Yes;

            if (!advance)
            {
                Run();
                return;
            }

            var correctDocumentResult = AgtService.CorrectDocument(correctDocumentId, _agtDocument.DocumentId);

            if (correctDocumentResult)
            {
                CustomAlerts.Information(this).WithMessage($"Correção do documento {_agtDocument.Number} submetida com sucesso.").ShowAlert();
            }
            else
            {
                CustomAlerts.Error(this).WithMessage($"Ocorreu um erro ao submeter a correção do documento {_agtDocument.Number}.").ShowAlert();
            }

            Run();
        }

        private void ShowData()
        {
            TxtSubmissionDate.Text = _agtDocument?.CreatedAt.ToString("g") ?? "Não submetido";
            TxtRequestId.Text = _agtDocument?.RequestId ?? "Não submetido";
            TxtDocumentNumber.Text = _agtDocument?.Number ?? "Não submetido";
            TxtSubmissionErrorCode.Text = _agtDocument?.SubmissionErrorCode ?? "-";
            TxtSubmissionErrorDescription.Text = _agtDocument?.SubmissionErrorDescription ?? "-";
            TxtHttpStatusCode.Text = _agtDocument?.HttpStatusCode?.ToString() ?? "-";
            TxtValidationResultCode.Text = _agtDocument?.ValidationResultCode ?? "Não validado";
            TxtValidationStatus.Text = _agtDocument?.ValidationStatus ?? "Não validado";
            TxtValidationErrors.Text = _agtDocument?.ValidationErrors ?? "-";
            TxtRejectedDocumentNumber.Text = _agtDocument?.RejectedDocumentNumber?.ToString() ?? "-";
        }

        public static void Show(AgtDocument agtDocument, Window parent)
        {
            var modal = new AgtDocumentInfoModal(agtDocument, parent);
            var response = (ResponseType)modal.Run();
            modal.Destroy();
        }

    }
}
