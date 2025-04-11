using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PaymentConditionModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtAcronym = TextBox.Simple("global_ConfigurationPaymentCondition_Acronym", true, true, "^.{1,3}$");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
    }
}
