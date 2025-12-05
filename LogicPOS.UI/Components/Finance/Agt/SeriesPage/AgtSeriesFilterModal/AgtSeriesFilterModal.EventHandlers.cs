using Gtk;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AgtSeriesFilterModal
    {
        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (!AllFieldsAreValid())
            {
                ShowValidationErrors();
                Run();
                return;
            }
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            Clear();

        }

        public void Clear()
        {
            TxtDocumentType.Clear();
            TxtYear.Clear();
            TxtCode.Clear();
            TxtEstablishmentNumber.Clear();
            TxtStatus.Clear();
        }

        private void BtnSelectDocumentType_Clicked(object sender, EventArgs e)
        {

            var docType = SelectDocumentType();

            if (docType == null)
            {
                return;
            }

            TxtDocumentType.Text = docType;
        }

        private void TxtDocumentType_Entry_Changed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtDocumentType.Text) && TxtDocumentType.Text.Length >= 10)
            {
                if (TxtDocumentType.IsValid())
                {
                    TxtDocumentType.Text = TxtDocumentType.Text;
                }
                return;
            }
        }

        private string SelectDocumentType()
        {
            var page = new DocumentTypesPage(this, DocumentTypesPage.ActiveTypesOnlyOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<DocumentType>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                return page.SelectedEntity.Acronym;
            }

            return null;
        }
    }
}
