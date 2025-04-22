using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Documents.CreateDocument.Fields
{
    public partial class DocumentPaymentMethodField
    {
        public IconButton BtnRemove { get; set; }
        public IconButton BtnAdd { get; set; }
        public TextBox TxtPaymentMethod { get; set; }
        public TextBox TxtAmount { get; set; }
        public Widget Component { get; private set; }
        public string FieldName => TxtPaymentMethod.Label.Text;

        public event Action<DocumentPaymentMethodField> OnRemove;
        public event System.Action OnAdd;
    }
}
