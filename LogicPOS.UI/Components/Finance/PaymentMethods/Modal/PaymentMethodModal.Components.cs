using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PaymentMethodModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtToken = TextBox.Simple("global_ConfigurationPaymentMethod_Token");
        private TextBox _txtAcronym = TextBox.Simple("global_ConfigurationPaymentMethod_Acronym");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
    }
}
