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
       
    }
}
