using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PlaceModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        private EntityComboBox<PriceType> _comboPriceTypes;
        private EntityComboBox<MovementType> _comboMovementTypes;
                
    }
}
