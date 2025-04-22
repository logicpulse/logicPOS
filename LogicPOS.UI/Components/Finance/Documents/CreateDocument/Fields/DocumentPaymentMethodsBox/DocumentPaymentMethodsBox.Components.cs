using Gtk;
using LogicPOS.Utility;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Documents.CreateDocument.Fields
{
    public partial class DocumentPaymentMethodsBox
    {
        public List<DocumentPaymentMethodField> Fields { get; } = new List<DocumentPaymentMethodField>();
        public VBox Container { get; } = new VBox(false, 5) { BorderWidth = 5 };
        public Widget Component { get; private set; }
        public Window SourceWindow { get; set; }
        public Label LabelTotal { get; set; }

        public string FieldName => GeneralUtils.GetResourceByName("global_payment_method");
    }
}
