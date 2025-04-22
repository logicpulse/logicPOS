using LogicPOS.UI.Components.InputFields;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentCustomerTab
    {
        public TextBox TxtCustomer { get; set; }
        public TextBox TxtFiscalNumber { get; set; }
        public TextBox TxtCardNumber { get; set; }
        public TextBox TxtDiscount { get; set; }
        public TextBox TxtAddress { get; set; }
        public TextBox TxtLocality { get; set; }
        public TextBox TxtZipCode { get; set; }
        public TextBox TxtCity { get; set; }
        public TextBox TxtCountry { get; set; }
        public TextBox TxtPhone { get; set; }
        public TextBox TxtEmail { get; set; }
        public TextBox TxtNotes { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
