using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Services;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class FiscalYearModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtYear = TextBox.Simple("global_fiscal_year", true, true, @"^\d+$");
        private TextBox _txtAcronym = TextBox.Simple("global_acronym", true, true, @"^[a-zA-Z0-9]+$");
        private CheckButton _checkSeriesForEachTerminal = new CheckButton(GeneralUtils.GetResourceByName("global_seriesforeachterminal"));
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));

        protected override void Initialize()
        {
            if (_modalMode != EntityEditionModalMode.Insert || (SystemInformationService.UseAgtFe))
            {
                _checkSeriesForEachTerminal.Sensitive = false;
            }
        }
    }
}
