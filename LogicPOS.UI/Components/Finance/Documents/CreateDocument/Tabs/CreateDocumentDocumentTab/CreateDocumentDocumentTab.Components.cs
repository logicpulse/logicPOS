using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentDocumentTab
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;
        public IEnumerable<DocumentType> DocumentTypes { get; private set; }
        public TextBox TxtDocumentType { get; set; }
        public TextBox TxtPaymentCondition { get; set; }
        public TextBox TxtCurrency { get; set; }
        public Currency CompanyCurrency { get; private set; }
        public TextBox TxtExchangeRate { get; private set; }
        public TextBox TxtOriginDocument { get; set; }
        public TextBox TxtCopyDocument { get; set; }
        public TextBox TxtNotes { get; set; }

        public event Action<Document> OriginDocumentSelected;
        public event Action<DocumentType> DocumentTypeSelected;
        public event Action<Document> CopyDocumentSelected;
    }
}
