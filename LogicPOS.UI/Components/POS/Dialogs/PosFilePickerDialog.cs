using Gtk;
using System.Drawing;
using System.IO;
using LogicPOS.Settings;
using LogicPOS.Utility;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Buttons;
using LogicPOS.UI;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosFilePickerDialog : BaseDialog
    {
        //Private Members
        private readonly FileFilter _fileFilter;
        private readonly FileChooserAction _fileChooserAction;
        //UI
        private readonly Fixed _fixedContent;

        public FileChooserWidget FilePicker { get; set; }

        public PosFilePickerDialog(Window parentWindow, DialogFlags pDialogFlags)
            : this(parentWindow, pDialogFlags, null, FileChooserAction.Open)
        {
        }

        public PosFilePickerDialog(Window parentWindow, DialogFlags pDialogFlags, FileFilter pFileFilter, FileChooserAction pFileChooserAction)
           : this(parentWindow, pDialogFlags, pFileFilter, FileChooserAction.Open, null)
        {
        }


        public PosFilePickerDialog(Window parentWindow, DialogFlags pDialogFlags, FileFilter pFileFilter, FileChooserAction pFileChooserAction, string windowName)
            : base(parentWindow, pDialogFlags)
        {
            //Parameters
            _fileFilter = pFileFilter;
            _fileChooserAction = pFileChooserAction;

            //Init Local Vars
            string windowTitle = string.Format("{0} {1}", GeneralUtils.GetResourceByName("window_title_dialog_filepicker"), windowName);
            WindowSettings.Size = new Size(700, 473);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_select_record.png";

            //Init Content
            _fixedContent = new Fixed();

            //Call Init UI
            InitUI();

            //ActionArea Buttons
            IconButtonWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            IconButtonWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(buttonOk, ResponseType.Ok),
                new ActionAreaButton(buttonCancel, ResponseType.Cancel)
            };

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, WindowSettings.Size, _fixedContent, actionAreaButtons);
        }

        private void InitUI()
        {
            //Init Font Description
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxValue);
            //Init FileChooserWidget
            FilePicker = new FileChooserWidget(_fileChooserAction, "none");
            if (_fileFilter != null) FilePicker.Filter = _fileFilter;
            //Assign FilePicker StartPath
            if (Directory.Exists(GlobalApp.OpenFileDialogStartPath)) FilePicker.SetCurrentFolder(GlobalApp.OpenFileDialogStartPath);
            //Size and Put
            FilePicker.SetSizeRequest(WindowSettings.Size.Width - 13, WindowSettings.Size.Height - 120);
            _fixedContent.Put(FilePicker, 0, 0);
            //Events
            FilePicker.CurrentFolderChanged += delegate { GlobalApp.OpenFileDialogStartPath = FilePicker.CurrentFolder; };
        }
    }
}
