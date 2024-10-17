﻿using Gtk;
using logicpos.Classes.Enums.Keyboard;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Pickers;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class EntryBoxValidationFilePickerMultiImages : EventBox
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _debug = false;

        //Private
        private readonly Window _sourceWindow;
        private readonly VBox _vbox;
        private int _maxImagesAllowed { get; set; }
        public int MaxImagesAllowed
        {
            get { return _maxImagesAllowed; }
            set { _maxImagesAllowed = value; }
        }

        internal EntryBoxValidationFilePickerDialog EntryBoxAddFile { get; set; }
        public List<string> Value { get; set; }

        //Events
        public event EventHandler Changed;

        public EntryBoxValidationFilePickerMultiImages(Window parentWindow, string pLabelText, FileFilter pFileFilter)
            : this(parentWindow, pLabelText, pFileFilter, new List<string>())
        {
        }

        public EntryBoxValidationFilePickerMultiImages(Window parentWindow, string pLabelText, FileFilter pFileFilter, List<string> pInitialFileList)
        {
            //Parameters
            _sourceWindow = parentWindow;

            //Init Dates List
            Value = pInitialFileList;
            //Init Dates VBox
            _vbox = new VBox(false, 0);

            //Init AddFile
            EntryBoxAddFile = new EntryBoxValidationFilePickerDialog(parentWindow, pLabelText, RegexUtils.RegexAlfaNumericFilePath, false, pFileFilter);
            EntryBoxAddFile.EntryValidation.Validate();
            EntryBoxAddFile.ClosePopup += _entryBoxAddFile_ClosePopup;

            VBox vboxOuter = new VBox(false, 0);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Always);
            scrolledWindow.ResizeMode = ResizeMode.Parent;
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.Add(_vbox);
            scrolledWindow.Add(viewport);

            //Initial Values
            if (Value.Count > 0)
            {
                for (int i = 0; i < Value.Count; i++)
                {
                    //Assign current fileName to _entryBoxAddFile, the last added is the Visible One
                    EntryBoxAddFile.EntryValidation.Text = Value[i];
                    AddFileEntry(Value[i], false);
                }
            }

            vboxOuter.PackStart(EntryBoxAddFile, false, false, 0);
            vboxOuter.PackStart(scrolledWindow, true, true, 0);
            Add(vboxOuter);
        }

        private void _entryBoxAddFile_ClosePopup(object sender, EventArgs e)
        {
            if (Value.Contains(EntryBoxAddFile.Value))
            {
                logicpos.Utils.ShowMessageTouch(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"), GeneralUtils.GetResourceByName("dialog_message_filepicker_existing_file_error"));
            }
            else
            {
                AddFileEntry(EntryBoxAddFile.EntryValidation.Text, true);

                //Trigger Event
                OnChange();

                if (_debug) ListValue();
            }
        }

        //if AddFileNameToList true, add the File to the list, else skip add, usefull when we Initialize List from Constructor pInitialFileList
        private void AddFileEntry(string pFileName, bool pAddFileNameToList)
        {
            EntryBoxValidationButton entryBoxValidationButton = new EntryBoxValidationButton(_sourceWindow, string.Format(GeneralUtils.GetResourceByName("global_file_image"), Value.Count + 1), KeyboardMode.None, RegexUtils.RegexAlfaNumericFilePath, true);
            entryBoxValidationButton.EntryValidation.Validate();
            entryBoxValidationButton.EntryValidation.Sensitive = false;

            //Add Aditional Button to EntryBoxValidationFilePickerDialog
            string iconFileNameDelete = string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, @"Icons/Windows/icon_window_delete_record.png");

            IconButton buttonDelete = new IconButton(
                new ButtonSettings
                {
                    Name = "touchButtonIcon_Delete",
                    BackgroundColor = Color.Transparent,
                    Icon = iconFileNameDelete,
                    IconSize = new Size(20, 20),
                    ButtonSize = new Size(30, 30)
                });

            entryBoxValidationButton.Hbox.PackStart(buttonDelete, false, false, 0);

            //Events
            entryBoxValidationButton.Button.Clicked += Button_Clicked;
            buttonDelete.Clicked += buttonDelete_Clicked;

            //Assign Initial FileName
            entryBoxValidationButton.EntryValidation.Text = pFileName;

            //Pack
            _vbox.PackStart(entryBoxValidationButton, false, false, 0);
            entryBoxValidationButton.ShowAll();
            if (pAddFileNameToList) Value.Add(EntryBoxAddFile.Value);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            IconButton button = (IconButton)sender;
            EntryBoxValidationButton entryBoxValidationButton = (button.Parent.Parent.Parent as EntryBoxValidationButton);

            try
            {
                FileFilter fileFilter = FilePicker.GetFileFilterImages();

                //Get Current FileList Index Position
                int currentFileListIndexPosition = -1;
                for (int i = 0; i < (button.Parent.Parent.Parent.Parent as VBox).Children.Length; i++)
                {
                    EntryBoxValidationButton curentrEntryBox = ((button.Parent.Parent.Parent.Parent as VBox).Children.GetValue(i) as EntryBoxValidationButton);
                    if (curentrEntryBox == entryBoxValidationButton)
                    {
                        currentFileListIndexPosition = i;
                        if (_debug) _logger.Debug(string.Format("Current file List Index [{0}]: [{1}]", currentFileListIndexPosition, Value[currentFileListIndexPosition]));
                    }
                }

                FilePicker dialog = new FilePicker(_sourceWindow, DialogFlags.DestroyWithParent, fileFilter, FileChooserAction.Open);
                ResponseType response = (ResponseType)dialog.Run();
                if (response == ResponseType.Ok)
                {
                    if (entryBoxValidationButton.EntryValidation.Text == dialog.FileChooser.Filename)
                    {
                        //Do Nothing
                    }
                    else if (Value.Contains(dialog.FileChooser.Filename))
                    {
                        logicpos.Utils.ShowMessageTouch(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"), GeneralUtils.GetResourceByName("dialog_message_filepicker_existing_file_error"));
                    }
                    else
                    {
                        //Update fileList with Changed Value
                        Value[currentFileListIndexPosition] = dialog.FileChooser.Filename;
                        //Update and Validate Entry
                        entryBoxValidationButton.EntryValidation.Text = dialog.FileChooser.Filename;
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
                _logger.Error(ex.Message, ex);
            }
        }

        private void buttonDelete_Clicked(object sender, EventArgs e)
        {
            IconButton button = (IconButton)sender;
            EntryBoxValidationButton entryBoxValidationButton = (button.Parent.Parent.Parent as EntryBoxValidationButton);
            _vbox.Remove(entryBoxValidationButton);
            Value.Remove(entryBoxValidationButton.EntryValidation.Text);

            //Trigger Event
            OnChange();

            if (_debug) ListValue();
        }

        private void ListValue()
        {
            for (int i = 0; i < Value.Count; i++)
            {
                _logger.Debug(string.Format("_filesList[{0}/{1}]", Value[i], Value.Count));
            }
        }

        override public string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < Value.Count; i++)
            {
                result += Value[i];
                if (i < Value.Count - 1) result += ';';
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


                if ((Value.Count < _maxImagesAllowed) || (_maxImagesAllowed == -1))
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
