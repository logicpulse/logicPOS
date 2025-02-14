using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.GetAllWarehouses;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Warehouses;
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
        private TextBox TxtPrice { get; set; } = TextBox.Simple("global_price",false,true,RegularExpressions.Money);
        private readonly List<SerialNumberField> _serialNumberFields = new List<SerialNumberField>();
        private VBox _serialNumberFieldsContainer { get; set; } = new VBox(false, 2);
        private WarehouseSelectionField _locationField { get; set; } 

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

            if (_isUniqueArticle)
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
            hbox.PackStart(_locationField.WarehouseField.Component, true, true, 0);
            hbox.PackStart(_locationField.LocationField.Component, true, true, 0);
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

            for (int i = 0; i < quantity; i++)
            {
                var field = new SerialNumberField();
                if(Article != null && Article.IsComposed)
                {
                    field.LoadArticleChildren(Article.Id);
                }
                _serialNumberFields.Add(field);
                _serialNumberFieldsContainer.PackStart(field.Component, false, false, 10);
            }

            _serialNumberFields.ForEach(f =>
            {
                f.TxtSerialNumber.IsValidFunction = SerialNumberIsUnique;

            });

            Component.ShowAll();
        }

        private bool SerialNumberIsUnique(string serialNumber)
        {
            return _serialNumberFields.Select(f => f.TxtSerialNumber.Text).Count(s => s == serialNumber) == 1;
        }
    }
}
