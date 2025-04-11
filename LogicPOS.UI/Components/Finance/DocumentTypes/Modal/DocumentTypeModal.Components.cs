using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentTypeModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtAcronym = TextBox.Simple("global_ConfigurationUnitMeasure_Acronym");
        private TextBox _txtPrintCopies = TextBox.Simple("global_print_copies", true, true, RegularExpressions.IntegerNumber);
        private CheckButton _checkRequestPrintConfirmation = new CheckButton(GeneralUtils.GetResourceByName("global_print_request_confirmation"));
        private CheckButton _checkOpenDrawer = new CheckButton(GeneralUtils.GetResourceByName("global_open_drawer"));
        
    }
}
