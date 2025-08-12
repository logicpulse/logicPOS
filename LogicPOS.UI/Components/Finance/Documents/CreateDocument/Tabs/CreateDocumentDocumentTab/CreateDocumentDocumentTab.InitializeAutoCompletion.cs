using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Finance.PaymentConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        private void TxtPaymentCondition_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPaymentCondition.Text))
            {
                TxtPaymentCondition.Clear();
            }
        }
    }
}
