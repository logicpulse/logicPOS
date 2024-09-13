using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using System.Drawing;
using System.Windows.Forms;

namespace LogicPOS.UI.Components.InputFields
{
    public class ArticleField
    {
        public HBox Component { get; private set; } = new HBox(false, 2);
        public IconButton BtnSelect { get; set; }
        public IconButton BtnClear { get; set; }
        public IconButton BtnAdd { get; set; }
        public Entry TxtDesignation { get; set; }
        public Entry TxtQuantity { get; set; }
        public Entry TxtCode { get; set; }

        public ArticleField()
        {
            InitializeButtons();
            InitializeEntries();
            PackComponents();
        }

        private void InitializeButtons()
        {
            string iconSelectRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}";
            string iconClearRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_delete_record.png"}";
            string iconAddRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/icon_pos_nav_new.png"}";
           
            BtnSelect = new IconButton(new ButtonSettings { Name = "touchButtonIcon", Icon = iconSelectRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
            BtnClear = new IconButton(new ButtonSettings { Name = "touchButtonIcon", Icon = iconClearRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
            BtnAdd = new IconButton(new ButtonSettings { Name = "touchButtonIcon", Icon = iconAddRecord, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
        }

        private void InitializeEntries()
        {
            TxtDesignation = new Entry();
            TxtQuantity = new Entry() { WidthRequest = 50 };
            TxtCode = new Entry() { WidthRequest = 50 };

        }

        private void PackComponents()
        {
            Component.PackStart(TxtCode, false, false, 0);
            Component.PackStart(TxtDesignation, true, true, 0);
            Component.PackStart(TxtQuantity, false, false, 0);
            Component.PackStart(BtnSelect, false, false, 0);
            Component.PackStart(BtnClear, false, false, 0);
            Component.PackStart(BtnAdd, false, false, 0);
        }
    }
}
