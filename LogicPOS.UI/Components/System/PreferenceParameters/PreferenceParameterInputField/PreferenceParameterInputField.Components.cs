using Gtk;
using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class PreferenceParameterInputField
    {
        private readonly PreferenceParameter _entity;
        public TextBox TextBox { get; private set; }
        public MultilineTextBox MultilineTextBox { get; private set; }
        public CheckButton CheckButton { get; private set; }
        public FileChooserButton FileChooserButton { get; private set; }
        public ComboBox ComboBox { get; private set; }
        public Widget FieldComponent { get; set; } = new VBox(false, 2);
        public Label Label { get; private set; }
        public Button RemoveFileButton { get; private set; }
        public Gtk.Image PreviewImage { get; set; } = new Image() { HeightRequest = 35, WidthRequest = 35 };
    }
}
