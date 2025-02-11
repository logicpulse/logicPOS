
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ChangeArticleLocationModal
    {
        public override Size ModalSize => new Size(320, 450);
        public override string ModalTitleResourceName => "global_warehose_management";

        #region Components
        private TextBox _txtArticle { get; set; } = TextBox.Simple("global_article", true);
        private TextBox _txtSerialNumber { get; set; } = TextBox.Simple("global_serialNumber", false);
        private TextBox _txtQuantity { get; set; } = TextBox.Simple("global_quantity", true);
        private EntityComboBox<Warehouse> _comboWarehouse { get; set; }
        private EntityComboBox<WarehouseLocation> _comboWarehouseLocation { get; set; }
        #endregion


        protected override void BeforeDesign()
        {
            InitializeComboboxes();
           
            _txtArticle.Component.Sensitive = false;
            _txtSerialNumber.Component.Sensitive = false;
            _txtQuantity.Component.Sensitive = false;
        }

        private void InitializeComboboxes()
        {
            var warehouses = GetWarehouses();
            var labelText = GeneralUtils.GetResourceByName("global_warehouse");

            _comboWarehouse = new EntityComboBox<Warehouse>(labelText,
                                                            warehouses,
                                                            _entity.WarehouseLocation.Warehouse,
                                                            true);

            var currentWarehouse = warehouses.Where(x => x.Id == _entity.WarehouseLocation.Warehouse.Id).First();

            _comboWarehouseLocation = new EntityComboBox<WarehouseLocation>(GeneralUtils.GetResourceByName("global_locations"),
                                                                             currentWarehouse.Locations,
                                                                            _entity.WarehouseLocation,
                                                                            true);


            _comboWarehouse.ComboBox.Changed += (sender, e) =>
            {
                _comboWarehouseLocation.Entities = _comboWarehouse.SelectedEntity?.Locations;
                _comboWarehouseLocation.ReLoad();
            };
        }

        protected override void AddSensitiveFields()
        {

        }

        protected override void AddValidatableFields()
        {
            ValidatableFields.Add(_comboWarehouse);
            ValidatableFields.Add(_comboWarehouseLocation);
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            var tab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            tab.PackStart(_txtArticle.Component, false, false, 0);
            tab.PackStart(_txtSerialNumber.Component, false, false, 0);
            tab.PackStart(_comboWarehouse.Component, false, false, 0);
            tab.PackStart(_comboWarehouseLocation.Component, false, false, 0);
            tab.PackStart(_txtQuantity.Component, false, false, 0);

            yield return (tab, GeneralUtils.GetResourceByName("window_title_article_location"));
        }

    }
}
