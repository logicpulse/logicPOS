using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.GetDocumentsTotals;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PayInvoiceModal
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
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
        public List<(Document Invoice, DocumentTotals Totals)> Invoices { get; } = new List<(Document, DocumentTotals)>();
        private readonly decimal _invoicesTotalFinal;
        private string TitleBase { get; set; }
    }
}
