using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.financial.library.Classes.Reports;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class PosDeveloperTestDialog : PosBaseDialog
    {
        //private Fixed _fixedContent;
        private ScrolledWindow _scrolledWindow;
        private VBox _vbox;
        private uint _padding = 0;
        private EntryBoxValidation _entryBoxValidationCustomButton1;
        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _xPOEntryBoxSelectRecordValidationTextMode;

        public PosDeveloperTestDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_template");
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_default.png");
            _windowSize = new Size(595, 740);

            //Init VBox
            _vbox = new VBox(false, 2) { WidthRequest = _windowSize.Width - 44 };
            
            //Call InitUI
            InitUI1();
            //InitUI_FilePicker();
            //InitUI_LittleAdds();
            //InitUI2();
            //InitUI3();

            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.ResizeMode = ResizeMode.Parent;
            viewport.Add(_vbox);

            _scrolledWindow = new ScrolledWindow();
            _scrolledWindow.ShadowType = ShadowType.EtchedIn;
            _scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Automatic);
            _scrolledWindow.ResizeMode = ResizeMode.Parent;
            _scrolledWindow.Add(viewport);

            //ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(buttonCancel, ResponseType.Cancel));

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, _windowSize, _scrolledWindow, actionAreaButtons);
        }

        private void InitUI1()
        {
            //EntryBoxValidation with KeyBoard Input
            EntryBoxValidation entryBoxValidation = new EntryBoxValidation(this, "EntryBoxValidation", KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, true);
            entryBoxValidation.EntryValidation.Sensitive = false;
            entryBoxValidation.ButtonKeyBoard.Sensitive = false;
            _vbox.PackStart(entryBoxValidation, true, true, _padding);

            //EntryBoxValidation with KeyBoard Input and Custom Buttons : Start without KeyBoard, and KeyBoard Button After all Others
            _entryBoxValidationCustomButton1 = new EntryBoxValidation(this, "EntryBoxValidationCustomButton", KeyboardMode.None, SettingsApp.RegexAlfaNumericExtended, false);
            TouchButtonIcon customButton1 = _entryBoxValidationCustomButton1.AddButton("CustomButton1", @"Icons/Windows/icon_window_orders.png");
            TouchButtonIcon customButton2 = _entryBoxValidationCustomButton1.AddButton("CustomButton2", @"Icons/Windows/icon_window_pay_invoice.png");
            TouchButtonIcon customButton3 = _entryBoxValidationCustomButton1.AddButton("CustomButton3", @"Icons/Windows/icon_window_orders.png");
            //Now we manually Init Keyboard
            _entryBoxValidationCustomButton1.EntryValidation.KeyboardMode = KeyboardMode.AlfaNumeric;
            _entryBoxValidationCustomButton1.InitKeyboard(_entryBoxValidationCustomButton1.EntryValidation);
            //Test Required Rule
            customButton1.Clicked += customButton1_Clicked;
            customButton2.Clicked += customButton2_Clicked;
            customButton3.Clicked += customSharedButton_Clicked;
            _vbox.PackStart(_entryBoxValidationCustomButton1, true, true, _padding);

            //EntryBoxValidationButton
            EntryBoxValidationButton entryBoxValidationButton = new EntryBoxValidationButton(this, "EntryBoxValidationButton", KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, true);
            entryBoxValidationButton.Button.Clicked += customSharedButton_Clicked;
            _vbox.PackStart(entryBoxValidationButton, true, true, _padding);

            //Test XPOEntryBoxSelectRecordValidation without KeyBoard Input
            fin_documentfinancetype defaultValueDocumentFinanceType = (fin_documentfinancetype)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(fin_documentfinancetype), SettingsApp.XpoOidDocumentFinanceTypeInvoice);
            CriteriaOperator criteriaOperatorDocumentFinanceType = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            XPOEntryBoxSelectRecordValidation<fin_documentfinancetype, TreeViewDocumentFinanceType> entryBoxSelectDocumentFinanceType = new XPOEntryBoxSelectRecordValidation<fin_documentfinancetype, TreeViewDocumentFinanceType>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinanceseries_documenttype"), "Designation", "Oid", defaultValueDocumentFinanceType, criteriaOperatorDocumentFinanceType, SettingsApp.RegexGuid, true);
            //entryBoxSelectDocumentFinanceType.EntryValidation.IsEditable = false;
            entryBoxSelectDocumentFinanceType.ClosePopup += delegate { };
            _vbox.PackStart(entryBoxSelectDocumentFinanceType, true, true, _padding);

            //Test XPOEntryBoxSelectRecordValidation with KeyBoard Input
            CriteriaOperator criteriaOperatorXPOEntryBoxSelectRecordValidationTextMode = null;
            _xPOEntryBoxSelectRecordValidationTextMode = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, "XPOEntryBoxSelectRecordValidationTextMode", "Name", "Name", null, criteriaOperatorXPOEntryBoxSelectRecordValidationTextMode, KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, false);
            //_xPOEntryBoxSelectRecordValidationTextMode.EntryValidation.Sensitive = false;
            //Start Disabled
            //_xPOEntryBoxSelectRecordValidationTextMode.ButtonKeyBoard.Sensitive = false;
            _xPOEntryBoxSelectRecordValidationTextMode.ClosePopup += delegate { };
            _vbox.PackStart(_xPOEntryBoxSelectRecordValidationTextMode, true, true, _padding);

            //Test XPOEntryBoxSelectRecordValidation without KeyBoard Input / Guid
            CriteriaOperator criteriaOperatorXPOEntryBoxSelectRecordValidationGuidMode = null;
            XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> xPOEntryBoxSelectRecordValidationGuidMode = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, "XPOEntryBoxSelectRecordValidationGuidMode", "Name", "Oid", null, criteriaOperatorXPOEntryBoxSelectRecordValidationGuidMode, KeyboardMode.None, SettingsApp.RegexGuid, true);
            _xPOEntryBoxSelectRecordValidationTextMode.ClosePopup += delegate { };
            _vbox.PackStart(xPOEntryBoxSelectRecordValidationGuidMode, true, true, _padding);
            
            //Test DateTime Picker
            DateTime initalDateTime = DateTime.Now;
            EntryBoxValidationDatePickerDialog entryBoxShipToDeliveryDate = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ship_to_delivery_date"), "dateFormat", DateTime.Now, SettingsApp.RegexDate, true, SettingsApp.DateFormat);
            //entryBoxShipToDeliveryDate.EntryValidation.Sensitive = true;
            entryBoxShipToDeliveryDate.EntryValidation.Text = initalDateTime.ToString(SettingsApp.DateFormat);

            //entryBoxShipToDeliveryDate.EntryValidation.Validate();
            //entryBoxShipToDeliveryDate.ClosePopup += delegate { };
            _vbox.PackStart(entryBoxShipToDeliveryDate, true, true, _padding);

            //Test DateTime Picker with KeyBoard
            EntryBoxValidationDatePickerDialog entryBoxShipToDeliveryDateKeyboard = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ship_to_delivery_date"), SettingsApp.DateTimeFormat, DateTime.Now, KeyboardMode.AlfaNumeric, SettingsApp.RegexDateTime, true, SettingsApp.DateTimeFormat);
            entryBoxShipToDeliveryDateKeyboard.EntryValidation.Sensitive = false;
            entryBoxShipToDeliveryDateKeyboard.ButtonKeyBoard.Sensitive = false;
            //entryBoxShipToDeliveryDate.EntryValidation.Sensitive = true;
            entryBoxShipToDeliveryDateKeyboard.EntryValidation.Text = initalDateTime.ToString(SettingsApp.DateTimeFormat);
            _vbox.PackStart(entryBoxShipToDeliveryDateKeyboard, true, true, _padding);

            //Simple ListView
            List<string> itemList = new List<string>();
            itemList.Add("Looking for Kiosk mode in Android Lollipop 5.0");
            itemList.Add("Think of a hypothetical ATM machine that is running Android");
            itemList.Add("In this article we provide a brief overview of how");
            itemList.Add("Kiosk Mode can be implemented without any modifications");
            itemList.Add("The Home key brings you back to the Home screen");

            //ListComboBox
            ListComboBox listComboBox = new ListComboBox(itemList, itemList[3]);
            _vbox.PackStart(listComboBox, true, true, _padding);

            //ListComboBoxTouch
            ListComboBoxTouch listComboBoxTouch = new ListComboBoxTouch(this, "ListComboBoxTouch (Todo: Highlight Validation in Component)", itemList, itemList[4]);
            _vbox.PackStart(listComboBoxTouch, true, true, _padding);

            //EntryMultiline entryTouchMultiline = new EntryMultiline(this, KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, true, 100, 10);
            //vbox.PackStart(entryTouchMultiline, true, true, padding);
            EntryBoxValidationMultiLine entryBoxMultiLine = new EntryBoxValidationMultiLine(this, "EntryBoxMultiLine", KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, true, 18, 6) { HeightRequest = 200 };

            //Start Disabled
            entryBoxMultiLine.EntryMultiline.Sensitive = false;
            entryBoxMultiLine.ButtonKeyBoard.Sensitive = false;
            _vbox.PackStart(entryBoxMultiLine, true, true, _padding);


            /*
            ListRadioButtonTouch listRadioButtonTouch = new ListRadioButtonTouch(this, "Label", itemList, itemList[4]);
            _fixedContent.Put(listRadioButtonTouch, 100, 320);

            string initialShipFromDeliveryDate = FrameworkUtils.CurrentDateTimeAtomic().ToString(SettingsApp.DateFormat);
            //EntryBoxValidationButton entryBoxDate = new EntryBoxValidationButton(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ship_from_delivery_date, KeyboardModes.Alfa, regexDate, false);
            //entryBoxDate.EntryValidation.Text = initialShipFromDeliveryDate;
            //entryBoxDate.EntryValidation.Validate();

            EntryBoxValidationDatePickerDialog entryBoxDate = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ship_from_delivery_date, SettingsApp.RegexDate, false);
            entryBoxDate.EntryValidation.Text = initialShipFromDeliveryDate;
            entryBoxDate.EntryValidation.Validate();
            entryBoxDate.ClosePopup += delegate
            {
                _log.Debug(string.Format("entryBoxDate.Value: [{0}]", entryBoxDate.Value));
            };
            vbox.PackStart(entryBoxDate, true, true, padding);
            */
        }

        void customButton1_Clicked(object sender, EventArgs e)
        {
            _entryBoxValidationCustomButton1.EntryValidation.Required = true;
            _entryBoxValidationCustomButton1.EntryValidation.Validate();

            _xPOEntryBoxSelectRecordValidationTextMode.EntryValidation.Required = true;
            _xPOEntryBoxSelectRecordValidationTextMode.EntryValidation.Validate();
            _log.Debug(String.Format("Validated: [{0}]", _entryBoxValidationCustomButton1.EntryValidation.Validated));
        }

        void customButton2_Clicked(object sender, EventArgs e)
        {
            _entryBoxValidationCustomButton1.EntryValidation.Required = false;
            _entryBoxValidationCustomButton1.EntryValidation.Validate();

            _xPOEntryBoxSelectRecordValidationTextMode.EntryValidation.Required = false;
            _xPOEntryBoxSelectRecordValidationTextMode.EntryValidation.Validate();
            _log.Debug(String.Format("Validated: [{0}]", _entryBoxValidationCustomButton1.EntryValidation.Validated));
        }


        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void InitUI_FilePicker()
        {
            FileFilter fileFilter = Utils.GetFileFilterImages();

            EntryBoxValidationFilePickerDialog entryFilePicker = new EntryBoxValidationFilePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_file_image"), "", false, fileFilter);
            entryFilePicker.ClosePopup += delegate
            {
                _log.Debug(string.Format("entryFilePicker.Value: [{0}]", entryFilePicker.Value));
            };
            _vbox.PackStart(entryFilePicker, true, true, _padding);

            EntryBoxValidationFilePickerMultiImages entryFilePickerMultiImages = new EntryBoxValidationFilePickerMultiImages(this, "Multi", fileFilter);
            entryFilePickerMultiImages.WidthRequest = 500;
            entryFilePickerMultiImages.HeightRequest = 514;
            _vbox.PackStart(entryFilePickerMultiImages, true, true, _padding);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void InitUI2()
        {
            EntryBoxValidationDatePickerMultiDates entryBoxValidationDatePickerMultiDates = new EntryBoxValidationDatePickerMultiDates(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_datepicker_add_date"));
            entryBoxValidationDatePickerMultiDates.WidthRequest = 400;
            entryBoxValidationDatePickerMultiDates.HeightRequest = 400;
            _vbox.PackStart(entryBoxValidationDatePickerMultiDates, true, true, _padding);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private void InitUI3()
        {
            Button buttonTestDocumentMasterCreatePDF = new Button("Test DocumentMasterCreatePDF");
            buttonTestDocumentMasterCreatePDF.Clicked += buttonTestDocumentMasterCreatePDF_Clicked;
            _vbox.PackStart(buttonTestDocumentMasterCreatePDF, true, true, _padding);
        }

        void buttonTestDocumentMasterCreatePDF_Clicked(object sender, EventArgs e)
        {
            Guid guidOid = new Guid("099EF525-FCEC-48D8-9EE8-FA0F34A34ED4");
            fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), guidOid);
            string fileName = CustomReport.DocumentMasterCreatePDF(documentFinanceMaster);
            _log.Debug(string.Format("fileName: [{0}]", fileName));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        void customSharedButton_Clicked(object sender, EventArgs e)
        {
            _log.Debug("customSharedButton_Clicked");
        }
    }
}
