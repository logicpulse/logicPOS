using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.InputFieds
{
    public class PreferenceParameterInputField
    {
        private readonly PreferenceParameter _entity;
        public TextBox TextBox { get; private set; }
        public MultilineTextBox MultilineTextBox { get; private set; }
        public CheckButton CheckButton { get; private set; }
        public FileChooserButton FileChooserButton { get; private set; }
        public ComboBox ComboBox { get; private set; }
        public Widget FieldComponent { get; set; } = new VBox(false, 2);
        public Label Label { get; private set; }

        Dictionary<string, string> Cultures = new Dictionary<string, string>
            {
                { "Português(Portugal)",   "pt-PT"},
                { "Português(Angola)",     "pt-AO"},
                { "Português(Brasil)",     "pt-BR"},
                { "Português(Moçambique)", "pt-MZ"},
                { "English(GB)" ,          "en-GB"},
                { "English(USA)",          "en-US"},
                { "Françes" ,              "fr-FR"},
                { "Espanol" ,              "es-ES"},
            };

        public PreferenceParameterInputField(PreferenceParameter entity)
        {
            _entity = entity;
            InitializeLabel();
            InitializeInputField();
        }

        private void InitializeLabel()
        {
            Label = new Label(_entity.ResourceStringValue);
            Label.SetAlignment(0.0F, 0.0F);
        }

        private void InitializeInputField()
        {
            switch (_entity.InputType)
            {
                case PreferenceParameterInputType.Text:
                case PreferenceParameterInputType.TextPassword:
                    InitializeTextBox();
                    break;
                case PreferenceParameterInputType.Multiline:
                    InitializeMutilineTextBox();
                    break;
                case PreferenceParameterInputType.CheckButton:
                    InitializeCheckButton();
                    break;
                case PreferenceParameterInputType.ComboBox:
                    InitializeComboBox();
                    break;
                case PreferenceParameterInputType.FilePicker:
                    InitializeFilePicker();
                    break;
                case PreferenceParameterInputType.DirPicker:
                    InitializeDirPicker();
                    break;
            }
        }

        private void InitializeDirPicker()
        {
            var fileChooserAction = FileChooserAction.SelectFolder;
            FileChooserButton = new FileChooserButton(string.Empty, fileChooserAction) { HeightRequest = 23 };
            FileChooserButton.SetCurrentFolder(_entity.Value);

            InitializeFileChooserButtonComponent();
        }

        private FileFilter GetFileFilterImages()
        {
            FileFilter filter = new FileFilter();
            filter.Name = "PNG and JPEG images";
            filter.AddMimeType("image/png");
            filter.AddPattern("*.png");
            filter.AddMimeType("image/jpeg");
            filter.AddPattern("*.jpg");
            return filter;
        }

        private FileFilter GetFileFilterBMPImages()
        {
            FileFilter filter = new FileFilter();
            filter.Name = "BMP, PNG and JPEG images";
            filter.AddMimeType("image/png");
            filter.AddPattern("*.png");
            filter.AddMimeType("image/jpeg");
            filter.AddPattern("*.jpg");
            filter.AddMimeType("image/bmp");
            filter.AddPattern("*.bmp");
            return filter;
        }

        private void InitializeFilePicker()
        {
            var fileChooserAction = FileChooserAction.Open;
            FileChooserButton = new FileChooserButton(string.Empty, fileChooserAction) { HeightRequest = 23 };

            FileChooserButton.SetFilename(_entity.Value);

            if (_entity.Token == "TICKET_FILENAME_LOGO")
            {
                FileChooserButton.Filter = GetFileFilterBMPImages();
            }
            else
            {
                FileChooserButton.Filter = GetFileFilterImages();
            }

            InitializeFileChooserButtonComponent();
        }

        private void InitializeFileChooserButtonComponent()
        {
            var box = FieldComponent as VBox;
            box.PackStart(Label, false, false, 0);
            box.PackStart(FileChooserButton, false, false, 0);

        }

        private void InitializeComboBox()
        {
            TreeIter iterator;

            var cultureLabels = Cultures.Select(culture => culture.Key).ToArray();
            ComboBox = new ComboBox(cultureLabels);

            ComboBox.Model.GetIterFirst(out iterator);
            int index = 0;
            do
            {
                GLib.Value row = new GLib.Value();
                ComboBox.Model.GetValue(iterator, 0, ref row);

                if (Cultures.ElementAt(index).Value == Settings.CultureSettings.CurrentCultureName)
                {
                    ComboBox.SetActiveIter(iterator);
                    break;
                }

                index++;
            } while (ComboBox.Model.IterNext(ref iterator));

            VBox box = new VBox(false, 2);
            Label.Text = GeneralUtils.GetResourceByName("global_language");
            box.PackStart(Label, false, false, 0);
            box.PackStart(ComboBox, false, false, 0);
            FieldComponent = box;
        }

        private void InitializeCheckButton()
        {
            CheckButton = new CheckButton(_entity.ResourceStringValue);
            CheckButton.Active = _entity.Value.ToLower() == "true";
            FieldComponent = CheckButton;
        }

        private void InitializeMutilineTextBox()
        {
            MultilineTextBox = new MultilineTextBox();
            MultilineTextBox.Value.Text = _entity.Value;
            MultilineTextBox.ScrolledWindow.BorderWidth = 1;
            MultilineTextBox.HeightRequest = 122;

            var vbox = FieldComponent as VBox;
            vbox.PackStart(Label, false, false, 0);
            vbox.PackStart(MultilineTextBox, false, false, 0);
        }

        private void InitializeTextBox()
        {
            TextBox = new TextBox(_entity.ResourceString, _entity.Required);
            TextBox.Text = _entity.Value;
            FieldComponent = TextBox.Component;
        }

        public string GetValue()
        {
            switch (_entity.InputType)
            {
                case PreferenceParameterInputType.Text:
                case PreferenceParameterInputType.TextPassword:
                    return TextBox.Text;
                case PreferenceParameterInputType.Multiline:
                    return MultilineTextBox.Value.Text;
                case PreferenceParameterInputType.CheckButton:
                    return CheckButton.Active.ToString();
                case PreferenceParameterInputType.ComboBox:               
                    return Cultures.ElementAt(ComboBox.Active).Value;
                case PreferenceParameterInputType.FilePicker:
                    return FileChooserButton.Filename;
                case PreferenceParameterInputType.DirPicker:
                    return FileChooserButton.Filename;
                default:
                    return string.Empty;
            }
        }
    }
}
