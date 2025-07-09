using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields.Validation;

namespace LogicPOS.UI.Components.InputFields
{
    public class UniqueArticleField : IValidatableField
    {
        public WarehouseArticle WarehouseArticle { get; private set; }
        private TextBox TxtSerialNumber { get; set; } = TextBox.Simple("global_serial_number", true);
        public string FieldName => TxtSerialNumber.FieldName;
        private WarehouseSelectionField _warehouseSelectionField;
        public Widget Component { get; private set; }

        public UniqueArticleField(WarehouseArticle warehouseArticle)
        {
            WarehouseArticle = warehouseArticle;
            ShowEntityData();
            Component = CreateComponent();
        }

        private void ShowEntityData()
        {
            TxtSerialNumber.Text = WarehouseArticle.SerialNumber;
            _warehouseSelectionField = new WarehouseSelectionField(WarehouseArticle);

            TxtSerialNumber.Entry.IsEditable = false;
            _warehouseSelectionField.WarehouseField.ComboBox.Sensitive = false;
            _warehouseSelectionField.LocationField.ComboBox.Sensitive = false;
        }

        public Widget CreateComponent()
        {
            var hbox = new HBox(false, 2);
            hbox.PackStart(TxtSerialNumber.Component, true, true, 0);
            hbox.PackStart(_warehouseSelectionField.WarehouseField.Component, true, true, 0);
            hbox.PackStart(_warehouseSelectionField.LocationField.Component, true, true, 0);
            return hbox;
        }

        public bool IsValid()
        {
            return TxtSerialNumber.IsValid() && _warehouseSelectionField.IsValid();
        }
    }
}
