using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Finance.Currencies;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class DocumentTab : ModalTab
    {
        public DocumentTab(Window parent) : base(parent: parent,
                                                  name: GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page1"),
                                                  icon: AppSettings.Paths.Images + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_1_new_document.png")
        {
            Initialize();
            Design();
        }


        private DocumentType GetDocumentTypeFromDocument(DocumentViewModel document)
        {
            return DocumentTypesService.GetActive().FirstOrDefault(type => type.Acronym == document.Type);
        }

        public Guid? GetPaymentConditionId() => (TxtPaymentCondition.SelectedEntity as ApiEntity)?.Id;

        public Guid GetCurrencyId()
        {
            return (TxtCurrency.SelectedEntity as ApiEntity).Id;
        }

        public DocumentType GetDocumentType()
        {
            return TxtDocumentType.SelectedEntity as DocumentType;
        }

        public Guid? GetOriginDocumentId()
        {
            return (TxtOriginDocument.SelectedEntity as DocumentViewModel)?.Id;
        }


        public void SelectDocumentType(DocumentType documentType)
        {
            TxtDocumentType.SelectedEntity = documentType;
            TxtDocumentType.Text = documentType.Designation;
            UpdateValidatableFields();
            DocumentTypeSelected?.Invoke(documentType);
        }

        private void SelectPaymentCondition(Api.Entities.PaymentCondition paymentCondition)
        {
            TxtPaymentCondition.Text = paymentCondition.Designation;
            TxtPaymentCondition.SelectedEntity = paymentCondition;
        }

        public IEnumerable<DocumentPaymentMethod> GetPaymentMethods()
        {
            yield return new Api.Features.Finance.Documents.Documents.IssueDocument.DocumentPaymentMethod
            {
                PaymentMethodId = (TxtPaymentMethod.SelectedEntity as ApiEntity).Id
            };
        }
    }
}
