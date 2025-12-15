using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AgtSeriesFilterModal
    {
        private TextBox TxtDocumentType { get; set; }
        private TextBox TxtEstablishmentNumber { get; set; }
        private TextBox TxtCode { get; set; }
        private TextBox TxtYear { get; set; }
        private TextBox TxtStatus { get; set; }
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);
    }
}
