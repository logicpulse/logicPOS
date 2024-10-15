﻿using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Dialogs;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosDocumentFinancePrintDialog : BaseDialog
    {
        //UI
        private VBox _vboxContent;
        private CheckButtonBoxGroup _checkButtonCopyNamesBoxGroup;
        private CheckButtonBox _checkButtonBoxSecondCopy;
        private EntryBoxValidation _entryBoxValidationBoxMotive;
        private readonly IconButtonWithText _buttonOk;
        private readonly IconButtonWithText _buttonCancel;

        //Properties
        private int _printCopies = 0;
        private readonly bool _requestMotive = false;
        private bool _secondCopy = false;
        //Parameters
        private readonly fin_documentfinancemaster _documentFinanceMaster;

        public PosDocumentFinancePrintDialog(Window parentWindow, DialogFlags pDialogFlags, fin_documentfinancemaster pDocumentFinanceMaster)
            : base(parentWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = string.Format(GeneralUtils.GetResourceByName("window_title_dialog_document_finance_print"), pDocumentFinanceMaster.DocumentNumber);
            Size windowSize = new Size(400, 259);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_document_new.png";
            //Parameters
            _documentFinanceMaster = pDocumentFinanceMaster;
            //Vars
            _requestMotive = _documentFinanceMaster.DocumentType.PrintRequestMotive;
            if (_requestMotive) windowSize.Height += 42 + 76;//Secondcopy + Motive UI Components

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            //Call 
            InitUI();

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _vboxContent, actionAreaButtons);
        }

        private void InitUI()
        {
            _printCopies = _documentFinanceMaster.DocumentType.PrintCopies;

            _vboxContent = new VBox(false, 0);

            Dictionary<string, bool> buttonGroup = new Dictionary<string, bool>
            {
                { GeneralUtils.GetResourceByName("global_print_copy_title1"), (_printCopies >= 1) },
                { GeneralUtils.GetResourceByName("global_print_copy_title2"), (_printCopies >= 2) },
                { GeneralUtils.GetResourceByName("global_print_copy_title3"), (_printCopies >= 3) },
                { GeneralUtils.GetResourceByName("global_print_copy_title4"), (_printCopies >= 4) }
            };
          
            _checkButtonCopyNamesBoxGroup = new CheckButtonBoxGroup(GeneralUtils.GetResourceByName("global_print_copies"), buttonGroup);
            _vboxContent.PackStart(_checkButtonCopyNamesBoxGroup);
            _checkButtonCopyNamesBoxGroup.Clicked += checkButtonCopyNamesBoxGroup_Clicked;

            //If DocumentType is a RequestMotive Document Enable Motive
            if (_requestMotive)
            {
                //CheckButtonBoxSecondCopy
                _checkButtonBoxSecondCopy = new CheckButtonBox(GeneralUtils.GetResourceByName("global_second_copy"), true);
                _checkButtonBoxSecondCopy.Clicked += checkButtonBoxSecondCopy_Clicked;
                _checkButtonBoxSecondCopy.StateChanged += checkButtonBoxSecondCopy_Clicked;
                //Pack EntryBox with CheckBox into Dialog
                _vboxContent.PackStart(_checkButtonBoxSecondCopy);

                _entryBoxValidationBoxMotive = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_reprint_original_motive"), KeyboardMode.AlfaNumeric, RegexUtils.RegexAlfaNumeric, false);
                //Start Disabled
                _entryBoxValidationBoxMotive.EntryValidation.Label.Sensitive = false;
                _entryBoxValidationBoxMotive.EntryValidation.Sensitive = false;
                //Pack EntryBox with CheckBox into Dialog
                _vboxContent.PackStart(_entryBoxValidationBoxMotive);
                //Events
                _entryBoxValidationBoxMotive.EntryValidation.Changed += EntryValidation_Changed;
                //If Original Active enabled _secondCopy
                _secondCopy = _checkButtonCopyNamesBoxGroup.Buttons[0].Active;
            }
            //Orginal is Always Checked, use this event to Force it 
            _checkButtonCopyNamesBoxGroup.Buttons[0].Clicked += checkButtonBox1_Clicked;
            //Call UpdateUI
            UpdateUI();
        }

        private void checkButtonBox1_Clicked(object sender, EventArgs e)
        {
            //Force CheckBox1 to be always Active if is not in SecondCopy Mode
            if (_checkButtonBoxSecondCopy != null && !_checkButtonBoxSecondCopy.Active)/* IN009074 */
            {
                (sender as CheckButtonExtended).Active = true;
            }
        }

        private void checkButtonBoxSecondCopy_Clicked(object sender, EventArgs e)
        {
            _secondCopy = _checkButtonBoxSecondCopy.Active;
            //Update and Validate
            UpdateUI();
            Validate();
        }

        private void checkButtonCopyNamesBoxGroup_Clicked(object sender, EventArgs e)
        {
            Validate();
        }

        private void EntryValidation_Changed(object sender, EventArgs e)
        {
            Validate();
        }

        private void UpdateUI()
        {
            bool activeCheckButton = false;
            bool sensitiveCheckButton = true;

            //Enable/Disable CheckButtonCopyNamesBoxGroup CheckBoxs
            for (int i = 0; i < _checkButtonCopyNamesBoxGroup.Buttons.Count; i++)
            {
                if (i == 0)
                {
                    activeCheckButton = true;
                }
                else if (i > 0)
                {
                    // Mode#1
                    // Above Code is Deprecated of 2018.05.04, now Second Copy checkboxs works same as Orginal, Uncomment Block and Comment Above block to Restore to old Method
                    ////Active CheckBox if is in PrintCopies Range
                    //activeCheckButton = (i < _printCopies && !_secondCopy);
                    ////Disable If is in SecondCopy
                    //sensitiveCheckButton = (!_secondCopy);
                    // Mode#2
                    //Active CheckBox if is in PrintCopies Range
                    activeCheckButton = (i < _printCopies);
                };

                //Update Active and Sensitive
                _checkButtonCopyNamesBoxGroup.Buttons[i].Active = activeCheckButton;
                _checkButtonCopyNamesBoxGroup.Buttons[i].Sensitive = sensitiveCheckButton;
            }

            //Enable/Disable RequestMotive UI
            if (_requestMotive)
            {
                _checkButtonBoxSecondCopy.Sensitive = true;
                //Enable/Disable UI
                _entryBoxValidationBoxMotive.EntryValidation.Label.Sensitive = (!_secondCopy);
                _entryBoxValidationBoxMotive.EntryValidation.Sensitive = (!_secondCopy);
                _entryBoxValidationBoxMotive.ButtonKeyBoard.Sensitive = (!_secondCopy);
                _entryBoxValidationBoxMotive.EntryValidation.Required = (!_secondCopy);
                //Clean Inpit Text
                if (_secondCopy) _entryBoxValidationBoxMotive.EntryValidation.Text = string.Empty;
                //Validate Dialog
                _entryBoxValidationBoxMotive.EntryValidation.Validate();
            }
            else
            {
                if (_checkButtonBoxSecondCopy != null) _checkButtonBoxSecondCopy.Sensitive = false;
            }
        }

        public bool Validate()
        {
            _buttonOk.Sensitive = (
                _checkButtonCopyNamesBoxGroup.Items.Count > 0
                &&
                (
                    (
                        _requestMotive && _entryBoxValidationBoxMotive.EntryValidation.Validated
                    )
                    ||
                    (
                        !_requestMotive
                    )
                )
            );

            return _buttonOk.Sensitive;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Method to Call from FrameWork Calls PrintTicket.PrintFinanceDocument

        //Class Object to server has transport object for Response
        public class PrintDialogResponse
        {
            //Dialog
            public ResponseType Response = ResponseType.None;
            //CheckButtonBoxGroup Properties
            public Dictionary<int, CheckButtonExtended> Items = new Dictionary<int, CheckButtonExtended>();
            public List<int> CopyNames = new List<int>();
            //Other UI
            public bool SecondCopy = false;
            public string Motive = string.Empty;

            public PrintDialogResponse(ResponseType pResponseType)
                : this(pResponseType, null, new List<int>(), false, string.Empty)
            {
            }

            public PrintDialogResponse(ResponseType pResponse, Dictionary<int, CheckButtonExtended> pItems, List<int> pCopyNames, bool pSecondCopy, string pMotive)
            {
                Response = pResponse;
                Items = pItems;
                CopyNames = pCopyNames;
                SecondCopy = pSecondCopy;
                Motive = pMotive;
            }
        }

        public static PrintDialogResponse GetDocumentFinancePrintProperties(Window parentWindow, fin_documentfinancemaster pDocumentFinanceMaster)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //Init Response
            PrintDialogResponse result = new PrintDialogResponse(ResponseType.Cancel);

            try
            {
                //TODO: Xpo Required to use ExecuteScalar, else we dont have real value but a cached value
                //Get Fresh Object to get Printed Status
                //DocumentFinanceMaster documentFinanceMaster = (DocumentFinanceMaster)XPOSettings.Session.GetObjectByKey(typeof(DocumentFinanceMaster), pDocumentFinanceMaster.Oid);
                //bool printed = documentFinanceMaster.Printed;
                //Fix Cache Problem
                var sqlResPrinted = XPOSettings.Session.ExecuteScalar(string.Format(
                    "SELECT Printed FROM fin_documentfinancemaster WHERE Oid = '{0}';",
                    pDocumentFinanceMaster.Oid)
                );
                bool printed = (sqlResPrinted != null) && Convert.ToBoolean(sqlResPrinted);

                //Call Re-Print Dialog
                if (printed)
                {
                    PosDocumentFinancePrintDialog dialog = new PosDocumentFinancePrintDialog(parentWindow, DialogFlags.DestroyWithParent, pDocumentFinanceMaster);
                    result.Response = (ResponseType)dialog.Run();
                    if (result.Response == ResponseType.Ok)
                    {
                        //Modify Result with Dialog Properties
                        result.Items = dialog._checkButtonCopyNamesBoxGroup.Items;
                        //Generate CopyNames from CheckButtonCopyNamesBoxGroup.Items
                        result.CopyNames = new List<int>();
                        foreach (var item in dialog._checkButtonCopyNamesBoxGroup.Items) { result.CopyNames.Add(item.Key); }

                        result.SecondCopy = dialog._secondCopy;
                        if (dialog._entryBoxValidationBoxMotive != null) result.Motive = dialog._entryBoxValidationBoxMotive.EntryValidation.Text;
                    }
                    dialog.Destroy();
                }
                //Normal Print Without Dialog
                else
                {
                    //Modify Result
                    result.Response = ResponseType.Ok;
                    //Initialize CopyNames List from PrintCopies
                    result.CopyNames = LogicPOS.Reporting.Common.FastReport.CopyNames(pDocumentFinanceMaster.DocumentType.PrintCopies);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}