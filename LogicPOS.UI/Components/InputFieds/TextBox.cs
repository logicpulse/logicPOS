using Gtk;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.InputFieds
{
    public class TextBox
    {
        public Entry Entry { get; private set; }
        public string Text { get => Entry.Text; set => Entry.Text = value;}
        public Label Label { get; private set; }
        public bool IsRequired { get; private set; }
        public bool IsValid => !IsRequired || !string.IsNullOrEmpty(Entry.Text);
        public VBox Component { get; private set; }

        public TextBox(string labelResourceName,
                       bool isRequired = false)
        {
            Label = CreateLabel(labelResourceName);
            IsRequired = isRequired;
            Entry = new Entry();
            Component = CreateBox();
            AddEventHandlers();
            ValidateEntry();
        }

        private void AddEventHandlers()
        {
            Entry.Changed += (sender, args) =>
            {
                ValidateEntry();
            };
        }

        private void ValidateEntry()
        {
            ValidationColors.Default.UpdateComponentFontColor(Label, IsValid);
            ValidationColors.Default.UpdateComponent(Entry, IsValid);
        }

        private Label CreateLabel(string labelResourceName)
        {
            var label = new Label(GeneralUtils.GetResourceByName(labelResourceName));
            label.SetAlignment(0.0F, 0.0F);
            return label;
        }

        private VBox CreateBox()
        {
            var box = new VBox(false, 2);
            box.PackStart(Label, false, false, 0);
            box.PackStart(Entry, false, false, 0);
            return box;
        }

    }
}
