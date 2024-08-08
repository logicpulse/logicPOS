using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using System;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class EntryBoxValidationFilePickerDialog : EntryBoxValidationButton
    {
        //Private Properties
        private readonly FileFilter _fileFilter;

        public string Value { get; set; }
        //Custom Events
        public event EventHandler ClosePopup;

        public EntryBoxValidationFilePickerDialog(Window parentWindow, string pLabelText, string pRule, bool pRequired, FileFilter pFileFilter)
            : base(parentWindow, pLabelText, KeyboardMode.None, pRule, pRequired)
        {
            //Parameters
            _sourceWindow = parentWindow;
            _fileFilter = pFileFilter;
            //Entry
            _entryValidation.Sensitive = false;
            //Events
            _button.Clicked += delegate { PopupDialog(); };
        }

        //Events
        protected void PopupDialog()
        {
            try
            {
                PosFilePickerDialog dialog = new PosFilePickerDialog(_sourceWindow, DialogFlags.DestroyWithParent, _fileFilter, FileChooserAction.Open);
                ResponseType response = (ResponseType)dialog.Run();
                if (response == ResponseType.Ok)
                {
                    Value = dialog.FilePicker.Filename;
                    _entryValidation.Text = Value;
                    _entryValidation.Validate();

                    //Call Custom Event, Triggered only when Dialog Response is OK, else Ignored
                    OnClosePopup();
                }
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Custom Events
        private void OnClosePopup()
        {
            ClosePopup?.Invoke(this, EventArgs.Empty);
        }
    }
}
