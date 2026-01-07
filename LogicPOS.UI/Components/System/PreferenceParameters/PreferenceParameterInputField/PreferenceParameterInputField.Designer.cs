using Gtk;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class PreferenceParameterInputField
    {
        private void InitializeFilePicker()
        {
            var fileChooserAction = FileChooserAction.Open;
            FileChooserButton = new FileChooserButton(string.Empty, fileChooserAction) { HeightRequest = 23 };

            FileChooserButton.SetFilename(Base64ToFileName(_entity.Value));

            if (_entity.Token == "TICKET_FILENAME_LOGO")
            {
                FileChooserButton.Filter = GetFileFilterBMPImages();
            }
            else
            {
                FileChooserButton.Filter = GetFileFilterImages();
            }

            FileChooserButton.FileSet += (sender, e) =>
            {
                SelectFile();
            };

            InitializeFileChooserButtonComponent();
        }
        private string Base64ToFileName(string base64)
        {
            if (!string.IsNullOrEmpty(base64)&&!base64.Contains("."))
            { 
            byte[] imageBytes = Convert.FromBase64String(base64);
            string tempFile = Path.Combine(Path.GetTempPath(),
                                           $"ConvertedTempFileLogo.png");
            File.WriteAllBytes(tempFile, imageBytes);
            return tempFile;
            }
            return base64;
        }
        private void SelectFile()
        {

            if (FileChooserButton.Filename!=null) 
            {
                TextBox.Text = FileChooserButton.Filename;
                FileChooserButton.SetFilename(FileChooserButton.Filename);
            }
            else
            {
                TextBox.Text = FileChooserButton.Filename;
            };
        }

        private void InitializeFileChooserButtonComponent()
        {
            var hBox = new HBox();
            InitializeRemoveFileButton();

            hBox.PackStart(FileChooserButton, true, true, 0);
            hBox.PackStart(RemoveFileButton, false, false, 0);
            var box = FieldComponent as VBox;
            box.PackStart(Label, false, false, 0);
            box.PackStart(hBox, false, false, 0);
        }
        private void InitializeRemoveFileButton()
        {
            RemoveFileButton = new Button("Remover");
            RemoveFileButton.SetSizeRequest(70, 10);
            RemoveFileButton.Clicked += RemoveFileButton_Clicked;
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

                if (Cultures.ElementAt(index).Value == AppSettings.Culture.CurrentCultureName)
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

            ComboBox.Changed += (sender, e) =>
            {
                TextBox.Text = Cultures.ElementAt(ComboBox.Active).Value;
            };

            FieldComponent = box;
        }

        private void InitializeCheckButton()
        {
            CheckButton = new CheckButton(_entity.ResourceStringValue);
            CheckButton.Active = _entity.Value.ToLower() == "true";

            CheckButton.Toggled += (sender, e) =>
            {
                TextBox.Text = CheckButton.Active.ToString();
            };

            FieldComponent = CheckButton;
        }

        private void InitializeMultilineTextBox()
        {
            MultilineTextBox = new MultilineTextBox();
            MultilineTextBox.Value.Text = _entity.Value;
            MultilineTextBox.ScrolledWindow.BorderWidth = 1;
            MultilineTextBox.HeightRequest = 122;

            var vbox = FieldComponent as VBox;
            vbox.PackStart(Label, false, false, 0);
            vbox.PackStart(MultilineTextBox, false, false, 0);

            MultilineTextBox.Value.Changed += (sender, e) =>
            {
                TextBox.Text = MultilineTextBox.Value.Text;
            };
        }

        private void InitializeTextBox()
        {
            TextBox = TextBox.Simple(_entity.ResourceString, _entity.Required);
            TextBox.Text = _entity.Value;
        }
    }
}
