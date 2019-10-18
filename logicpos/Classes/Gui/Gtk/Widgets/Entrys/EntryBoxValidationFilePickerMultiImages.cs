using Gtk;
using logicpos.financial;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using logicpos.shared;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class EntryBoxValidationFilePickerMultiImages : EventBox
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _debug = false;

        //Private
        private Window _sourceWindow;
        private VBox _vbox;
        private int _maxImagesAllowed { get; set; }
        public int MaxImagesAllowed
        {
            get { return _maxImagesAllowed; }
            set { _maxImagesAllowed = value; }
        }
        //Public
        private EntryBoxValidationFilePickerDialog _entryBoxAddFile;
        internal EntryBoxValidationFilePickerDialog EntryBoxAddFile
        {
            get { return _entryBoxAddFile; }
            set { _entryBoxAddFile = value; }
        }
        private List<string> _fileList;
        public List<string> Value
        {
            get { return _fileList; }
            set { _fileList = value; }
        }

        //Events
        public event EventHandler Changed;

        public EntryBoxValidationFilePickerMultiImages(Window pSourceWindow, String pLabelText, FileFilter pFileFilter)
            : this(pSourceWindow, pLabelText, pFileFilter, new List<string>())
        {
        }

        public EntryBoxValidationFilePickerMultiImages(Window pSourceWindow, String pLabelText, FileFilter pFileFilter, List<string> pInitialFileList)
        {
            //Parameters
            _sourceWindow = pSourceWindow;

            //Init Dates List
            _fileList = pInitialFileList;
            //Init Dates VBox
            _vbox = new VBox(false, 0);

            //Init AddFile
            _entryBoxAddFile = new EntryBoxValidationFilePickerDialog(pSourceWindow, pLabelText, SettingsApp.RegexAlfaNumericFilePath, false, pFileFilter);
            _entryBoxAddFile.EntryValidation.Validate();
            _entryBoxAddFile.ClosePopup += _entryBoxAddFile_ClosePopup;

            VBox vboxOuter = new VBox(false, 0);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Always);
            scrolledWindow.ResizeMode = ResizeMode.Parent;
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.Add(_vbox);
            scrolledWindow.Add(viewport);

            //Initial Values
            if (_fileList.Count > 0)
            {
                for (int i = 0; i < _fileList.Count; i++)
                {
                    //Assign current fileName to _entryBoxAddFile, the last added is the Visible One
                    _entryBoxAddFile.EntryValidation.Text = _fileList[i];
                    AddFileEntry(_fileList[i], false);
                }
            }

            vboxOuter.PackStart(_entryBoxAddFile, false, false, 0);
            vboxOuter.PackStart(scrolledWindow, true, true, 0);
            Add(vboxOuter);
        }

        private void _entryBoxAddFile_ClosePopup(object sender, EventArgs e)
        {
            if (_fileList.Contains(_entryBoxAddFile.Value))
            {
                Utils.ShowMessageTouch(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_filepicker_existing_file_error"));
            }
            else
            {
                AddFileEntry(_entryBoxAddFile.EntryValidation.Text, true);

                //Trigger Event
                OnChange();

                if (_debug) ListValue();
            }
        }

        //if AddFileNameToList true, add the File to the list, else skip add, usefull when we Initialize List from Constructor pInitialFileList
        private void AddFileEntry(string pFileName, bool pAddFileNameToList)
        {
            EntryBoxValidationButton entryBoxValidationButton = new EntryBoxValidationButton(_sourceWindow, string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_file_image"), _fileList.Count + 1), KeyboardMode.None, SettingsApp.RegexAlfaNumericFilePath, true);
            entryBoxValidationButton.EntryValidation.Validate();
            entryBoxValidationButton.EntryValidation.Sensitive = false;

            //Add Aditional Button to EntryBoxValidationFilePickerDialog
            string iconFileNameDelete = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], @"Icons/Windows/icon_window_delete_record.png"));
            TouchButtonIcon buttonDelete = new TouchButtonIcon("touchButtonIcon_Delete", Color.Transparent, iconFileNameDelete, new Size(20, 20), 30, 30);
            entryBoxValidationButton.Hbox.PackStart(buttonDelete, false, false, 0);

            //Events
            entryBoxValidationButton.Button.Clicked += Button_Clicked;
            buttonDelete.Clicked += buttonDelete_Clicked;

            //Assign Initial FileName
            entryBoxValidationButton.EntryValidation.Text = pFileName;
            
            //Pack
            _vbox.PackStart(entryBoxValidationButton, false, false, 0);
            entryBoxValidationButton.ShowAll();
            if (pAddFileNameToList) _fileList.Add(_entryBoxAddFile.Value);
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            TouchButtonIcon button = (TouchButtonIcon)sender;
            EntryBoxValidationButton entryBoxValidationButton = (button.Parent.Parent.Parent as EntryBoxValidationButton);

            try
            {
                FileFilter fileFilter = Utils.GetFileFilterImages();

                //Get Current FileList Index Position
                int currentFileListIndexPosition = -1;
                for (int i = 0; i < (button.Parent.Parent.Parent.Parent as VBox).Children.Length; i++)
                {
                    EntryBoxValidationButton curentrEntryBox = ((button.Parent.Parent.Parent.Parent as VBox).Children.GetValue(i) as EntryBoxValidationButton);
                    if (curentrEntryBox == entryBoxValidationButton)
                    {
                        currentFileListIndexPosition = i;
                        if (_debug) _log.Debug(string.Format("Current file List Index [{0}]: [{1}]", currentFileListIndexPosition, _fileList[currentFileListIndexPosition]));
                    }
                }

                PosFilePickerDialog dialog = new PosFilePickerDialog(_sourceWindow, DialogFlags.DestroyWithParent, fileFilter, FileChooserAction.Open);
                ResponseType response = (ResponseType)dialog.Run();
                if (response == ResponseType.Ok)
                {
                    if (entryBoxValidationButton.EntryValidation.Text == dialog.FilePicker.Filename)
                    {
                        //Do Nothing
                    }
                    else if (_fileList.Contains(dialog.FilePicker.Filename))
                    {
                        Utils.ShowMessageTouch(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_filepicker_existing_file_error"));
                    }
                    else
                    {
                        //Update fileList with Changed Value
                        _fileList[currentFileListIndexPosition] =  dialog.FilePicker.Filename;
                        //Update and Validate Entry
                        entryBoxValidationButton.EntryValidation.Text = dialog.FilePicker.Filename;
                        entryBoxValidationButton.EntryValidation.Validate();

                        //Trigger Event
                        OnChange();

                        if (_debug) ListValue();
                    }
                }
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        void buttonDelete_Clicked(object sender, EventArgs e)
        {
            TouchButtonIcon button = (TouchButtonIcon)sender;
            EntryBoxValidationButton entryBoxValidationButton = (button.Parent.Parent.Parent as EntryBoxValidationButton);
            _vbox.Remove(entryBoxValidationButton);
            _fileList.Remove(entryBoxValidationButton.EntryValidation.Text);

            //Trigger Event
            OnChange();

            if (_debug) ListValue();
        }

        private void ListValue()
        {
            for (int i = 0; i < _fileList.Count; i++)
            {
                _log.Debug(string.Format("_filesList[{0}/{1}]", _fileList[i], _fileList.Count));
            }
        }

        override public string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < _fileList.Count; i++)
            {
                result += _fileList[i];
                if (i < _fileList.Count - 1) result += ';';
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        private void OnChange()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);


                if ((_fileList.Count < _maxImagesAllowed) || (_maxImagesAllowed == -1))
                {
                    EntryBoxAddFile.Sensitive = true;
                }
                else
                {
                    EntryBoxAddFile.Sensitive = false;
                }
            }
        }
    }
}
