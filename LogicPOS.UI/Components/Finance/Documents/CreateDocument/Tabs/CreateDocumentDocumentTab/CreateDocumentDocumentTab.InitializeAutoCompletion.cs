using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Finance.PaymentConditions;
using Spire.Pdf.General.Paper.Uof;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentDocumentTab
    {
        private List<DocumentType> _documentTypeForCompletion;
        public List<DocumentType> DocumentTypeForCompletion => _documentTypeForCompletion ?? InitializeDocumentTypesForCompletion();

        private List<PaymentCondition> _paymentConditionForCompletion;
        private List<PaymentCondition> PaymentConditionForCompletion => _paymentConditionForCompletion ?? InitializePaymentConditionForCompletion();

        private List<DocumentType> InitializeDocumentTypesForCompletion()
        {
            _documentTypeForCompletion = DocumentTypesService.GetAllDocumentTypes();
            return _documentTypeForCompletion;
        }
        private List<PaymentCondition> InitializePaymentConditionForCompletion()
        {
            _paymentConditionForCompletion = PaymentConditionsService.GetAllPaymentConditions();
            return _paymentConditionForCompletion;
        }
        private void TxtDocumentType_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtDocumentType.Text))
            {
                TxtDocumentType.Clear();
            }
        }

        private void SelectDocumentType(DocumentType documentType)
        {
            TxtDocumentType.SelectedEntity = documentType;
            UpdateValidatableFields();
            DocumentTypeSelected?.Invoke(documentType);
        }

        private void SelectPaymentCondition(PaymentCondition paymentCondition)
        {
            TxtPaymentCondition.Text=paymentCondition.Designation;
            TxtPaymentCondition.SelectedEntity = paymentCondition;
        }
        private void TxtPaymentCondition_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPaymentCondition.Text))
            {
                TxtPaymentCondition.Clear();
            }
        }
    }
}
