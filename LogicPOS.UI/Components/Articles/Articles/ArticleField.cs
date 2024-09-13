using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Warehouses;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.InputFields
{
    public class ArticleField
    {
        public VBox Component { get; private set; } = new VBox(false, 2);
        public IconButton BtnSelect { get; set; }
        public IconButton BtnRemove { get; set; }
        public IconButton BtnAdd { get; set; }
        public Entry TxtDesignation { get; set; } = new Entry() { IsEditable = false };
        public Entry TxtQuantity { get; set; } = new Entry() { WidthRequest = 50, IsEditable = false };
        public Entry TxtCode { get; set; } = new Entry() { WidthRequest = 50, IsEditable = false };
        public Label Label { get; set; } = new Label(GeneralUtils.GetResourceByName("global_article"));
        public Article Article { get; set; }

        public event System.Action<ArticleField, Article> OnRemove;
        public event System.Action OnAdd;

        public ArticleField()
        {
            Label.SetAlignment(0, 0.5f);
            InitializeButtons();
            PackComponents();
            AddEventHandlers();
        }

        private void InitializeButtons()
        {
            string iconSelectRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}";
            string iconClearRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_delete_record.png"}";
            string iconAddRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/icon_pos_nav_new.png"}";

            BtnSelect = new IconButton(new ButtonSettings { Name = "touchButtonIcon", Icon = iconSelectRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
            BtnRemove = new IconButton(new ButtonSettings { Name = "touchButtonIcon", Icon = iconClearRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
            BtnAdd = new IconButton(new ButtonSettings { Name = "touchButtonIcon", Icon = iconAddRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
        }

        private void PackComponents()
        {
            Component.PackStart(Label, false, false, 0);
            Component.PackStart(CreateHBox(), false, false, 0);
        }

        private HBox CreateHBox()
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

        private void AddEventHandlers()
        {
            BtnRemove.Clicked += (s, e) => OnRemove?.Invoke(this, Article);
            BtnAdd.Clicked += (s, e) => OnAdd?.Invoke();
        }
    }
}
