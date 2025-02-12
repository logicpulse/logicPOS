using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.GetAllWarehouses;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class ArticleField
    {
        public EventBox Component { get; } = new EventBox();
        public IconButton BtnSelect { get; set; }
        public IconButton BtnRemove { get; set; }
        public IconButton BtnAdd { get; set; }
        public Entry TxtDesignation { get; set; } = new Entry() { IsEditable = false };
        public Entry TxtQuantity { get; set; } = new Entry() { WidthRequest = 50 };
        public Entry TxtCode { get; set; } = new Entry() { WidthRequest = 50, IsEditable = false };
        public Label Label { get; set; } = new Label(GeneralUtils.GetResourceByName("global_article"));
        private EntityComboBox<Warehouse> _comboWarehouse { get; set; }
        private EntityComboBox<WarehouseLocation> _comboWarehouseLocation { get; set; }
        private TextBox TxtPrice { get; set; } = TextBox.Simple("global_price",false,true,RegularExpressions.Money);

        private readonly List<TextBox> _serialNumberFields = new List<TextBox>();
        private VBox _serialNumberFieldsContainer { get; set; } = new VBox(false, 2);

        public void SetStockMovementStyle()
        {
            Component.ModifyBg(StateType.Normal, AppSettings.Instance.colorBaseDialogEntryBoxBackground.ToGdkColor());
            TxtDesignation.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxValue));
            TxtQuantity.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxValue));
            TxtCode.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxValue));
            Label.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxLabel));
        }

        public void SetStockManagementStyle()
        {
            Component.ModifyBg(StateType.Normal, "240,240,240".StringToColor().ToGdkColor());
        }

        private void PackComponents()
        {
            var vbox = new VBox(false, 2);
            vbox.BorderWidth = 2;
            vbox.PackStart(Label, false, false, 0);
            vbox.PackStart(CreateArticleHBox(), false, false, 0);

            if (_enableSerialNumbers)
            {
                vbox.PackStart(CreateWarehouseHbox(), false, false, 0);
                vbox.PackStart(_serialNumberFieldsContainer, false, false, 0);
                HSeparator separator = new HSeparator();
                vbox.PackStart(separator, false, false, 10);
            }

            Component.Add(vbox);
        }

        private HBox CreateArticleHBox()
        {
            var hbox = new HBox(false, 2);

            hbox.PackStart(TxtCode, false, false, 0);
            hbox.PackStart(TxtDesignation, true, true, 0);
            hbox.PackStart(TxtQuantity, false, false, 0);
            hbox.PackStart(BtnSelect, false, false, 0);
            hbox.PackStart(BtnRemove, false, false, 0);
            hbox.PackStart(BtnAdd, false, false, 0);

            return hbox;
        }

        private HBox CreateWarehouseHbox()
        {
            var hbox = new HBox(false, 2);
            hbox.PackStart(_comboWarehouse.Component, true, true, 0);
            hbox.PackStart(_comboWarehouseLocation.Component, true, true, 0);
            hbox.PackStart(TxtPrice.Component, false, false, 0);
            return hbox;
        }

        private void InitializeButtons()
        {
            string iconSelectRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}";
            string iconClearRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_delete_record.png"}";
            string iconAddRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/icon_pos_nav_new.png"}";

            BtnSelect = new IconButton(new ButtonSettings { Name = "buttonUserId", Icon = iconSelectRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
            BtnRemove = new IconButton(new ButtonSettings { Name = "buttonUserId", Icon = iconClearRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
            BtnAdd = new IconButton(new ButtonSettings { Name = "buttonUserId", Icon = iconAddRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
        }

        private void InitializeComboboxes()
        {
            var warehouses = GetWarehouses();
            var labelText = GeneralUtils.GetResourceByName("global_warehouse");

            _comboWarehouse = new EntityComboBox<Warehouse>(labelText,
                                                            warehouses,
                                                            null,
                                                            true);

            _comboWarehouseLocation = new EntityComboBox<WarehouseLocation>(GeneralUtils.GetResourceByName("global_locations"),
                                                                            Enumerable.Empty<WarehouseLocation>(),
                                                                            null,
                                                                            true);


            _comboWarehouse.ComboBox.Changed += (sender, e) =>
            {
                _comboWarehouseLocation.Entities = _comboWarehouse.SelectedEntity?.Locations;
                _comboWarehouseLocation.ReLoad();
            };
        }

        private void UpdateSerialNumbersComponents()
        {
            _serialNumberFields.ForEach(f => _serialNumberFieldsContainer.Remove(f.Component));
            _serialNumberFields.Clear();

            int quantity = 0;

            if (int.TryParse(TxtQuantity.Text, out quantity) == false)
            {
                return;
            }

            if (quantity <= 0)
            {
                return;
            }

            for (int i = _serialNumberFields.Count; i < quantity; i++)
            {
                var textBox = TextBox.Simple("global_serial_number",
                                             false,
                                             true,
                                             RegularExpressions.AlfaNumericExtended);

                textBox.Entry.Changed += (sender, args) => { UpdateValidationColors(); };
                _serialNumberFields.Add(textBox);
                _serialNumberFieldsContainer.PackStart(textBox.Component, false, false, 10);
            }

            _serialNumberFields.ForEach(f =>
            {
                f.IsValidFunction = SerialNumberIsUnique;

            });
        }

        private bool SerialNumberIsUnique(string serialNumber)
        {
            return _serialNumberFields.Select(f => f.Text).Count(s => s == serialNumber) == 1;
        }
    }
}
