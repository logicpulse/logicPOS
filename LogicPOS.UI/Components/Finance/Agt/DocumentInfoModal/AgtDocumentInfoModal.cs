using Gtk;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class AgtDocumentInfoModal : Modal
    {
        public AgtDocumentInfoModal(Guid documentId, Window parent) : base(parent,
                                                     "Informação Do Documento (AGT)",
                                                     new Size(550, 620),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_preview.png")
        {
            ShowData(documentId);
        }

        private void ShowData(Guid documentId)
        {

            var agtDocument = AgtService.GetAgtDocument(documentId);
            TxtSubmissionDate.Text = agtDocument?.CreatedAt.ToString("g") ?? "Não submetido";
            TxtRequestId.Text = agtDocument?.RequestId ?? "Não submetido";
            TxtDocumentNumber.Text = agtDocument?.Number ?? "Não submetido";
            TxtSubmissionErrorCode.Text = agtDocument?.SubmissionErrorCode ?? "-";
            TxtSubmissionErrorDescription.Text = agtDocument?.SubmissionErrorDescription ?? "-";
            TxtHttpStatusCode.Text = agtDocument?.HttpStatusCode?.ToString() ?? "-";
            TxtValidationResultCode.Text = agtDocument?.ValidationResultCode ?? "Não validado";
            TxtValidationStatus.Text = agtDocument?.ValidationStatus ?? "Não validado";
            TxtValidationErrors.Text = agtDocument?.ValidationErrors ?? "-";
        }

        public static void Show(Guid documentId, Window parent)
        {
            var modal = new AgtDocumentInfoModal(documentId, parent);
            modal.Run();
            modal.Destroy();
        }

    }
}
