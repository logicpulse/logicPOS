using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PayInvoiceModal
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

        public TextBox TxtPaymentMethod { get; private set; }
        public TextBox TxtCurrency { get; private set; }
        public TextBox TxtExchangeRate { get; private set; }
        public TextBox TxtTotalPaid { get; private set; }
        public TextBox TxtSystemCurrencyTotalPaid { get; private set; }
        public TextBox TxtDateTime { get; private set; }
        public TextBox TxtNotes { get; private set; }
        private List<IValidatableField> ValidatableFields { get; set; } = new List<IValidatableField>();
        private string TitleBase { get; set; }
    }
}
