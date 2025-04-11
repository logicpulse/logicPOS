using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class ArticleField
    {
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

    }
}
