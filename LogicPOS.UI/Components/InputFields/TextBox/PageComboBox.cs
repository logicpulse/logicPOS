using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Extensions;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.InputFields
{
    public class PageComboBox<Tkey>
    {

        public ComboBox ComboBox { get; private set; }
        private ListStore _listStore = new ListStore(typeof(Tkey), typeof(string));
        public string Text => ComboBox.ActiveText;
        public Label Label { get; private set; }
        public Widget Component { get; private set; }

        public PageComboBox(string labelText)
        {
            Label = CreateLabel(labelText);
            ComboBox = CreateComboBox();
            Component = CreateComponent();
        }

        public Tkey GetSelectedItem()
        {
            TreeIter iter;

            if (ComboBox.GetActiveIter(out iter))
            {
                return (Tkey)_listStore.GetValue(iter, 0);
            }

            return default(Tkey);
        }

        private ComboBox CreateComboBox()
        {
            var combobox = new ComboBox() { HeightRequest = 30 };
            combobox.Model = _listStore;
            combobox.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxValue));
            CellRendererText cellRenderer = new CellRendererText();
            combobox.PackStart(cellRenderer, true);
            combobox.AddAttribute(cellRenderer, "text", 1);
            return combobox;
        }

        public void AddItem(Tkey key, string value)
        {
            _listStore.AppendValues(key, value);

            if(ComboBox.Active == -1)
            {
                ComboBox.Active = 0;
            }
        }
        
        private Label CreateLabel(string labelText)
        {
            var label = new Label(labelText);
            label.SetAlignment(0.0F, 0.0F);
            label.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxLabel));
            return label;
        }

        private EventBox CreateComponent()
        {
            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(Label, false, false, 0);
            verticalLayout.BorderWidth = 2;

            var hbox = new HBox(false, 2);
            hbox.PackStart(ComboBox, true, true, 0);

            verticalLayout.PackStart(hbox, false, false, 0);

            return CreateGrayLine(verticalLayout);
        }

        private EventBox CreateGrayLine(Widget content)
        {
            var eventBox = new EventBox();
            eventBox.ModifyBg(StateType.Normal, AppSettings.Instance.colorBaseDialogEntryBoxBackground.ToGdkColor());
            eventBox.BorderWidth = 2;
            eventBox.Add(content);
            return eventBox;
        }
    }
}
