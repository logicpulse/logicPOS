using Gtk;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using System;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class EntryBoxValidationFilePickerDialog : EntryBoxValidationButton
    {
        //Private Properties
        private FileFilter _fileFilter;
        //Public Properties
        private string _fileName;
        public string Value
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        //Custom Events
        public event EventHandler ClosePopup;

        public EntryBoxValidationFilePickerDialog(Window pSourceWindow, String pLabelText, string pRule, bool pRequired, FileFilter pFileFilter)
            : base(pSourceWindow, pLabelText, KeyboardMode.None, pRule, pRequired)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
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
                    _fileName = dialog.FilePicker.Filename;
                    _entryValidation.Text = _fileName;
                    _entryValidation.Validate();

                    //Call Custom Event, Triggered only when Dialog Response is OK, else Ignored
                    OnClosePopup();
                }
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Custom Events
        private void OnClosePopup()
        {
            if (ClosePopup != null)
            {
                ClosePopup(this, EventArgs.Empty);
            }
        }
    }
}
