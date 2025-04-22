using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CurrencyModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtAcronym = TextBox.Simple("global_ConfigurationCurrency_Acronym", true);
        private TextBox _txtExchangeRate = TextBox.Simple("global_exchangerate", true, true, @"^\d+([.,]\d+)?$");
        private TextBox _txtEntity = TextBox.Simple("global_ConfigurationCurrency_Entity");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
    }
}
