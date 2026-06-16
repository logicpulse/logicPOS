using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Finance.Documents.Sdr
{
    public partial class VoltaRefundReceiptModal
    {
        private IconButtonWithText BtnOk { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

        private TextBox TxtCustomer { get; set; }
        private TextBox TxtQuantity { get; set; }
        private TextBox TxtPaymentMethod { get; set; }

        private const string TitleBase = "Talão Reembolso Volta";

        private Customer _selectedCustomer;
    }
}
