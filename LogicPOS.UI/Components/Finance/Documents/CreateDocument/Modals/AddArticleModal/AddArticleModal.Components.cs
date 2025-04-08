using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddArticleModal : Modal
    {
        public IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        public IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        public IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);
        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();
        public TextBox TxtArticle { get; set; }
        public TextBox TxtQuantity { get; set; }
        public TextBox TxtPrice { get; set; }
        public TextBox TxtDiscount { get; set; }
        public TextBox TxtTotal { get; set; }
        public TextBox TxtTotalWithTax { get; set; }
        public TextBox TxtTax { get; set; }
        public TextBox TxtVatExemptionReason { get; set; }
        public TextBox TxtNotes { get; set; }
    }
}
