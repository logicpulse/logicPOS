using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.shared.App;
using System.Drawing;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosFilePickerDialog : PosBaseDialog
    {
        //Private Members
        private readonly FileFilter _fileFilter;
        private readonly FileChooserAction _fileChooserAction;
        //UI
        private readonly Fixed _fixedContent;
        //Public Properties
        private FileChooserWidget _filePicker;
        public FileChooserWidget FilePicker
        {
            get { return _filePicker; }
            set { _filePicker = value; }
        }

        public PosFilePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : this(pSourceWindow, pDialogFlags, null, FileChooserAction.Open)
        {
        }

        public PosFilePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, FileFilter pFileFilter, FileChooserAction pFileChooserAction)
           : this(pSourceWindow, pDialogFlags, pFileFilter, FileChooserAction.Open, null)
        {
        }


        public PosFilePickerDialog(Window pSourceWindow, DialogFlags pDialogFlags, FileFilter pFileFilter, FileChooserAction pFileChooserAction, string windowName)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            _fileFilter = pFileFilter;
            _fileChooserAction = pFileChooserAction;

            //Init Local Vars
            string windowTitle = string.Format("{0} {1}", resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_filepicker"), windowName);
            _windowSize = new Size(700, 473);
            string fileDefaultWindowIcon = SharedUtils.OSSlash(DataLayerFramework.Path["images"] + @"Icons\Windows\icon_window_select_record.png");

            //Init Content
            _fixedContent = new Fixed();

            //Call Init UI
            InitUI();

            //ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(buttonOk, ResponseType.Ok),
                new ActionAreaButton(buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, _windowSize, _fixedContent, actionAreaButtons);
        }

        private void InitUI()
        {
            //Init Font Description
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString(DataLayerFramework.Settings["fontEntryBoxValue"]);
            //Init FileChooserWidget
            _filePicker = new FileChooserWidget(_fileChooserAction, "none");
            if (_fileFilter != null) _filePicker.Filter = _fileFilter;
            //Assign FilePicker StartPath
            if (Directory.Exists(GlobalApp.FilePickerStartPath)) _filePicker.SetCurrentFolder(GlobalApp.FilePickerStartPath);
            //Size and Put
            _filePicker.SetSizeRequest(_windowSize.Width - 13, _windowSize.Height - 120);
            _fixedContent.Put(_filePicker, 0, 0);
            //Events
            _filePicker.CurrentFolderChanged += delegate { GlobalApp.FilePickerStartPath = _filePicker.CurrentFolder; };
        }
    }
}
