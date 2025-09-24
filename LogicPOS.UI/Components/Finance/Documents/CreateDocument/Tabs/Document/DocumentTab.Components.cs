using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Services;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class DocumentTab
    {
        public TextBox TxtDocumentType { get; set; }
        public TextBox TxtPaymentCondition { get; set; }
        public TextBox TxtPaymentMethod { get; set; }
        private bool SinglePaymentMethod => SystemInformationService.SystemInformation.IsPortugal;
        public TextBox TxtCurrency { get; set; }
        public TextBox TxtExchangeRate { get; private set; }
        public TextBox TxtOriginDocument { get; set; }
        public TextBox TxtCopyDocument { get; set; }
        public TextBox TxtNotes { get; set; }

        public event Action<Document> OriginDocumentSelected;
        public event Action<DocumentType> DocumentTypeSelected;
        public event Action<Document> CopyDocumentSelected;
    }
}
