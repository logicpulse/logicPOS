using Gtk;
using LogicPOS.Api.Features.Documents.DeleteDraft;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Finance.Agt;
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
      
        private void BtnViewAgtDocument_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null || Page.SelectedEntity.AgtInfo?.Number == null)
            {
                return;
            }

            var agtDocument = AgtService.GetAgtDocument(Page.SelectedEntity.Id);

            AgtDocumentInfoModal.Show(agtDocument,this);
        }

        private void BtnUpdateAgtValidationStatus_Clicked(object sender, EventArgs e)
        {
           if(Page.SelectedDocuments.Count <= 1)
            {
                UpdateSelectedDocumentAgtValidationStatus();
                return;
            }
            UpdateSelectedDocumentsAgtValidationStatus();
        }

        private void UpdateSelectedDocumentsAgtValidationStatus()
        {
            if(Page.SelectedDocuments.Count <= 1)
            {
                return;
            }

            var result = AgtService.UpdateDocumentsValidationStatus(Page.SelectedDocuments.Select(d => d.Id));

            if (result == false)
            {
                CustomAlerts.Error(this)
                .WithMessage($"Não foi possível atualizar o estado de validação de um ou mais documentos")
                .ShowAlert();
                return;
            }

            CustomAlerts.Information(this)
               .WithMessage($"Estado de validação dos documentos atualizados com sucesso.")
               .ShowAlert();

            Page.Refresh();
        }

        private void UpdateSelectedDocumentAgtValidationStatus()
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var result = AgtService.UpdateDocumentValidationStatus(Page.SelectedEntity.Id);

            if (result == false)
            {
                CustomAlerts.Error(this)
                .WithMessage($"Não foi possível atualizar o estado de validação do documento {Page.SelectedEntity.Number}.")
                .ShowAlert();
                return;
            }

            CustomAlerts.Information(this)
               .WithMessage($"O estado de validação do documento {Page.SelectedEntity.Number} foi atualizado.")
               .ShowAlert();

            Page.Refresh();
        }

        private void BtnSendDocumentToAgt_Clicked(object sender, EventArgs e)
        {
            if(Page.SelectedDocuments.Count <= 1)
            {
                SendSelectedDocumentToAgt();
                return;
            }

            SendSelectedDocumentsToAgt();
        }

        private void SendSelectedDocumentsToAgt()
        {
            if (Page.SelectedDocuments.Count <= 1)
            {
                return;
            }

            var advance = CustomAlerts.Question(this)
                .WithMessage($"Tem a certeza pretende enviar {Page.SelectedDocuments.Count} documentos para a AGT? Esta acção não pode ser revertida.")
                .ShowAlert();

            if (advance != ResponseType.Yes)
            {
                return;
            }

            var result = AgtService.RegisterDocuments(Page.SelectedDocuments.Select(d => d.Id));

            if (result == false)
            {
                CustomAlerts.Error(this)
                .WithMessage($"Ocorreu um erro ao enviar algun(s) documento(s) para a AGT.")
                .ShowAlert();
                return;
            }

            CustomAlerts.Information(this)
               .WithMessage($"Todos os documentos foram enviados à AGT")
               .ShowAlert();

            Page.Refresh();
        }

        private void SendSelectedDocumentToAgt()
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var advance = CustomAlerts.Question(this)
                .WithMessage($"Tem a certeza pretende enviar o documento {Page.SelectedEntity.Number} para a AGT? Esta acção não pode ser revertida.")
                .ShowAlert();

            if (advance != ResponseType.Yes)
            {
                return;
            }

            var result = AgtService.RegisterDocument(Page.SelectedEntity.Id);

            if (result == false)
            {
                CustomAlerts.Error(this)
                .WithMessage($"Não foi possível enviar o documento {Page.SelectedEntity.Number} para a AGT.")
                .ShowAlert();
                return;
            }

            CustomAlerts.Information(this)
               .WithMessage($"O documento {Page.SelectedEntity.Number} foi enviado à AGT com sucesso.")
               .ShowAlert();

            Page.Refresh();
        }
    }
}
