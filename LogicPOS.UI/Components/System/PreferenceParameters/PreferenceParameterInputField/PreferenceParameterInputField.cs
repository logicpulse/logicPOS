using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class PreferenceParameterInputField
    {
        private readonly Dictionary<string, string> Cultures = new Dictionary<string, string>
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
            InitializeTextBox();
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
                    FieldComponent = TextBox.Component;
                    break;
                case PreferenceParameterInputType.Multiline:
                    InitializeMultilineTextBox();
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

            FileChooserButton.SelectionChanged += (sender, e) =>
            {
                TextBox.Text = FileChooserButton.Filename;
            };

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

        private void RemoveFileButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FileChooserButton.Filename))
            {
                return;
            }
            TextBox.Text = string.Empty;
            if(FileChooserButton.Filename.StartsWith(Path.GetTempPath())) File.Delete(FileChooserButton.Filename);
            FileChooserButton.UnselectFilename(FileChooserButton.Filename);
            PreviewImage.Pixbuf = null;
        }

    }
}
