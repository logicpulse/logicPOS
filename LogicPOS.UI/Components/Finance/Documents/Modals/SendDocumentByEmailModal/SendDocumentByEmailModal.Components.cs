using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class SendDocumentByEmailModal
    {
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private TextBox TxtSubject { get; set; }
        private TextBox TxtTo { get; set; }
        private TextBox TxtCc { get; set; }
        private TextBox TxtBcc { get; set; }
        private EntryBoxValidationMultiLine TxtBody { get; set; }
        private List<IValidatableField> ValidatableFields { get; set; } = new List<IValidatableField>();
        private readonly IEnumerable<Guid> _documentsIds;
        private readonly bool _sendReceipts;
    }
}
