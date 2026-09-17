using Gtk;
using LogicPOS.Api.Features.Documents.DeleteDraft;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Finance.Agt;
using LogicPOS.UI.Components.Finance.At;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using Serilog;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentsModal
    {
        private void BtnSendDocumentToAt_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var advance = CustomAlerts.Question(this)
                .WithMessage($"Tem a certeza pretende enviar o documento {Page.SelectedEntity.Number} para a AT? Esta acção não pode ser revertida.")
                .ShowAlert();

            if (advance != ResponseType.Yes)
            {
                return;
            }

            var result = AtService.RegisterDocument(Page.SelectedEntity.Id);

            if (result.HasValue == false)
            {
                CustomAlerts.Error(this)
                .WithMessage($"Não foi possível enviar o documento {Page.SelectedEntity.Number} para a AT.")
                .ShowAlert();
                return;
            }

            CustomAlerts.Information(this)
               .WithMessage($"O documento {Page.SelectedEntity.Number} foi enviado à AT com sucesso. AtDocCodeId: {result.Value.AtDocCodeId}")
               .ShowAlert();

            Page.Refresh();
        }
    }
}
