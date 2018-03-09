using Gtk;
using logicpos.App;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Logic.License;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class PosLicenceDialog : PosBaseDialog
    {
        //Parameters
        private string _hardwareId;
        //Ui
        private HBox _hboxMain;
        private EntryBoxValidation _entryBoxHardwareId;
        private TouchButtonIconWithText _buttonRegister;
        private TouchButtonIconWithText _buttonContinue;
        private TouchButtonIconWithText _buttonClose;
        //UI Public
        private EntryBoxValidation _entryBoxName;
        public EntryBoxValidation EntryBoxName
        {
            get { return _entryBoxName; }
            set { _entryBoxName = value; }
        }
        private EntryBoxValidation _entryBoxCompany;
        public EntryBoxValidation EntryBoxCompany
        {
            get { return _entryBoxCompany; }
            set { _entryBoxCompany = value; }
        }
        private EntryBoxValidation _entryBoxFiscalNumber;
        public EntryBoxValidation EntryBoxFiscalNumber
        {
            get { return _entryBoxFiscalNumber; }
            set { _entryBoxFiscalNumber = value; }
        }
        private EntryBoxValidation _entryBoxAddress;
        public EntryBoxValidation EntryBoxAddress
        {
            get { return _entryBoxAddress; }
            set { _entryBoxAddress = value; }
        }
        private EntryBoxValidation _entryBoxEmail;
        public EntryBoxValidation EntryBoxEmail
        {
            get { return _entryBoxEmail; }
            set { _entryBoxEmail = value; }
        }
        private EntryBoxValidation _entryBoxPhone;
        public EntryBoxValidation EntryBoxPhone
        {
            get { return _entryBoxPhone; }
            set { _entryBoxPhone = value; }
        }

        public PosLicenceDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pHardwareId)
                    : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = Resx.window_title_license;
            System.Drawing.Size windowSize = new System.Drawing.Size(790, 630);
            string fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_license.png");

            //If detected empty Hardware Id from Parameters, get it from IntelliLock
            if (string.IsNullOrEmpty(pHardwareId))
            {
                _hardwareId = GlobalFramework.PluginLicenceManager.GetHardwareID(); ;
            }
            else
            {
                _hardwareId = pHardwareId;
            }

            //Files
            string fileActionRegister = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_register.png");
            string fileActionContinue = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_ok.png");

            //ActionArea Buttons
            _buttonRegister = new TouchButtonIconWithText("touchButtonRegister_DialogActionArea", System.Drawing.Color.Transparent, Resx.pos_button_label_licence_register, _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileActionRegister, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height) { Sensitive = false };
            _buttonContinue = new TouchButtonIconWithText("touchButtonContinue_DialogActionArea", System.Drawing.Color.Transparent, Resx.pos_button_label_licence_continue, _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileActionContinue, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height);
            _buttonClose = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Close);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(_buttonRegister, ResponseType.Accept));
            actionAreaButtons.Add(new ActionAreaButton(_buttonContinue, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonClose, ResponseType.Close));

            //Init Content
            InitUI();

            //Start Validated
            Validate();

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _hboxMain, actionAreaButtons);
        }

        private void InitUI()
        {
            //Files
            string fileAppBanner = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Licence\licence.png");
            //Init
            int padding = 5;
            //Init Fonts
            Pango.FontDescription fontBig = new Pango.FontDescription();
            fontBig.Size = 18;
            fontBig.Weight = Pango.Weight.Bold;

            //MockData
            bool useMockData = false;
            string mockName = (useMockData) ? "Carlos Fernandes" : string.Empty;
            string mockCompany = (useMockData) ? "LogicPulse" : string.Empty;
            string mockFiscalNumber = (useMockData) ? "503218820" : string.Empty;
            string mockAddress = (useMockData) ? "Rua Capitão Salgueiro Maia, nº7, 3080-245 Figueira da Foz" : string.Empty;
            string mockPhone = (useMockData) ? "+351 233 042 347" : string.Empty;
            string mockEmail = (useMockData) ? "portugal@logicpulse.com" : string.Empty;

            //Init Content
            _hboxMain = new HBox(false, 0) { BorderWidth = (uint)padding };
            //Inner
            Image appBanner = new Image(fileAppBanner) { WidthRequest = 200 };
            VBox vboxMain = new VBox(false, padding);
            _hboxMain.PackStart(appBanner, false, false, (uint)padding);
            _hboxMain.PackStart(vboxMain, true, true, (uint)padding);

            //Pack VBoxMain : Welcome
            Label labelWelcome = new Label(Resx.window_license_label_welcome);
            labelWelcome.SetAlignment(0.0F, 0.0F);
            labelWelcome.ModifyFont(fontBig);
            vboxMain.PackStart(labelWelcome, false, false, (uint)padding);
            //Pack VBoxMain : Info
            Label lableInfo = new Label(Resx.window_license_label_info);
            lableInfo.WidthRequest = 500;
            lableInfo.Wrap = true;
            lableInfo.SetAlignment(0.0F, 0.0F);
            vboxMain.PackStart(lableInfo, true, true, (uint)padding);

            //Pack hboxInner
            HBox hboxInner = new HBox(false, 0);
            vboxMain.PackStart(hboxInner, false, false, 0);
            //Pack VBoxMain : HBoxInner
            VBox vboxInnerLeft = new VBox(false, 0);
            VBox vboxInnerRight = new VBox(false, 0);
            hboxInner.PackStart(vboxInnerLeft, true, true, 0);
            hboxInner.PackStart(vboxInnerRight, false, false, (uint)padding * 2);

            //VBoxInnerLeft 
            Label labelInternetRegistration = new Label(Resx.window_license_label_internet_registration);
            labelInternetRegistration.SetAlignment(0.0F, 0.0F);
            labelInternetRegistration.ModifyFont(fontBig);
            vboxInnerLeft.PackStart(labelInternetRegistration, false, false, 0);

            //EntryBoxName
            _entryBoxName = new EntryBoxValidation(this, Resx.global_name, KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, true);
            _entryBoxName.EntryValidation.ModifyFont(fontBig);
            _entryBoxName.EntryValidation.Text = mockName;
            _entryBoxName.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(_entryBoxName, false, false, 0);

            //EntryBoxCompany
            _entryBoxCompany = new EntryBoxValidation(this, Resx.global_company, KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, true);
            _entryBoxCompany.EntryValidation.ModifyFont(fontBig);
            _entryBoxCompany.EntryValidation.Text = mockCompany;
            _entryBoxCompany.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(_entryBoxCompany, false, false, 0);

            //EntryFiscalNumber
            _entryBoxFiscalNumber = new EntryBoxValidation(this, Resx.global_fiscal_number, KeyboardMode.Numeric, SettingsApp.RegexIntegerGreaterThanZero, true);
            _entryBoxFiscalNumber.EntryValidation.ModifyFont(fontBig);
            _entryBoxFiscalNumber.EntryValidation.Text = mockFiscalNumber;
            _entryBoxFiscalNumber.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(_entryBoxFiscalNumber, false, false, 0);

            //EntryBoxAddress
            _entryBoxAddress = new EntryBoxValidation(this, Resx.global_address, KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, true);
            _entryBoxAddress.EntryValidation.ModifyFont(fontBig);
            _entryBoxAddress.EntryValidation.Text = mockAddress;
            _entryBoxAddress.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(_entryBoxAddress, false, false, 0);

            //EntryBoxEmail
            _entryBoxEmail = new EntryBoxValidation(this, Resx.global_email, KeyboardMode.AlfaNumeric, SettingsApp.RegexEmail, true);
            _entryBoxEmail.EntryValidation.ModifyFont(fontBig);
            _entryBoxEmail.EntryValidation.Text = mockEmail;
            _entryBoxEmail.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(_entryBoxEmail, false, false, 0);

            //EntryBoxPhone
            _entryBoxPhone = new EntryBoxValidation(this, Resx.global_phone, KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, true);
            _entryBoxPhone.EntryValidation.ModifyFont(fontBig);
            _entryBoxPhone.EntryValidation.Text = mockPhone;
            _entryBoxPhone.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(_entryBoxPhone, false, false, 0);

            //EntryBoxHardwareId
            _entryBoxHardwareId = new EntryBoxValidation(this, Resx.global_hardware_id, KeyboardMode.None, SettingsApp.RegexAlfaNumericExtended, true);
            _entryBoxHardwareId.EntryValidation.ModifyFont(fontBig);
            _entryBoxHardwareId.EntryValidation.Text = _hardwareId;
            _entryBoxHardwareId.EntryValidation.Sensitive = false;
            vboxInnerLeft.PackStart(_entryBoxHardwareId, false, false, 0);

            //VBoxInnerRight
            Label labelWithoutInternetRegistration = new Label(Resx.window_license_label_without_internet_registration);
            labelWithoutInternetRegistration.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetRegistration.ModifyFont(fontBig);
            vboxInnerRight.PackStart(labelWithoutInternetRegistration, false, false, 0);

            //Info
            Label labelWithoutInternetContactInfo = new Label(Resx.window_license_label_without_internet_contact_info);
            labelWithoutInternetContactInfo.Wrap = true;
            labelWithoutInternetContactInfo.SetAlignment(0.0F, 0.0F);
            vboxInnerRight.PackStart(labelWithoutInternetContactInfo, false, false, 0);

            //Company
            Label labelWithoutInternetContactCompanyNameLabel = new Label(Resx.global_company);
            labelWithoutInternetContactCompanyNameLabel.SetAlignment(0.0F, 0.0F);
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyNameLabel, false, false, (uint)padding * 2);
            Label labelWithoutInternetContactCompanyNameValue = new Label(SettingsApp.AppCompanyName);
            labelWithoutInternetContactCompanyNameValue.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyNameValue.ModifyFont(fontBig);
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyNameValue, false, false, 0);

            //Phone
            Label labelWithoutInternetContactCompanyPhoneLabel = new Label(Resx.global_phone);
            labelWithoutInternetContactCompanyPhoneLabel.SetAlignment(0.0F, 0.0F);
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyPhoneLabel, false, false, (uint)padding * 2);
            Label labelWithoutInternetContactCompanyPhoneValue = new Label(SettingsApp.AppCompanyPhone);
            labelWithoutInternetContactCompanyPhoneValue.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyPhoneValue.ModifyFont(fontBig);
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyPhoneValue, false, false, 0);

            //Email
            Label labelWithoutInternetContactCompanyEmailLabel = new Label(Resx.global_phone);
            labelWithoutInternetContactCompanyEmailLabel.SetAlignment(0.0F, 0.0F);
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyEmailLabel, false, false, (uint)padding * 2);
            Label labelWithoutInternetContactCompanyEmailValue = new Label(SettingsApp.AppCompanyEmail);
            labelWithoutInternetContactCompanyEmailValue.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyEmailValue.ModifyFont(fontBig);
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyEmailValue, false, false, 0);

            //Email
            Label labelWithoutInternetContactCompanyWebLabel = new Label(Resx.global_phone);
            labelWithoutInternetContactCompanyWebLabel.SetAlignment(0.0F, 0.0F);
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyWebLabel, false, false, (uint)padding * 2);
            Label labelWithoutInternetContactCompanyWebValue = new Label(SettingsApp.AppCompanyWeb);
            labelWithoutInternetContactCompanyWebValue.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyWebValue.ModifyFont(fontBig);
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyWebValue, false, false, 0);
        }

        private void Validate()
        {
            _buttonRegister.Sensitive = (
                _entryBoxName.EntryValidation.Validated &&
                _entryBoxCompany.EntryValidation.Validated &&
                _entryBoxFiscalNumber.EntryValidation.Validated &&
                _entryBoxAddress.EntryValidation.Validated &&
                _entryBoxEmail.EntryValidation.Validated &&
                _entryBoxPhone.EntryValidation.Validated
            );
        }

        protected override void OnResponse(ResponseType pResponse)
        {
            switch (pResponse)
            {
                case ResponseType.Accept:
                    _log.Debug("ActionRegister()");
                    ActionRegister();
                    break;
            }
        }

        private void ActionRegister()
        {
            if (!GlobalFramework.PluginLicenceManager.ConnectToWS())
            {
                Utils.ShowMessageTouch(this, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Error, ButtonsType.Close, Resx.global_error, Resx.dialog_message_license_ws_connection_error);

                return;
            }

            if (GlobalFramework.PluginLicenceManager == null)
            {
                return;
            }

            byte[] registredLicence = new byte[0];

            try
            {
                //Returns ByteWrite File
                registredLicence = GlobalFramework.PluginLicenceManager.ActivateLicense(
                    _entryBoxName.EntryValidation.Text,
                    _entryBoxCompany.EntryValidation.Text,
                    _entryBoxFiscalNumber.EntryValidation.Text,
                    _entryBoxAddress.EntryValidation.Text,
                    _entryBoxEmail.EntryValidation.Text,
                    _entryBoxPhone.EntryValidation.Text,
                    _entryBoxHardwareId.EntryValidation.Text,
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
                );

                string completeFilePath = string.Format("{0}{1}", LicenseRouter.GetCurrentDirectory(), GlobalFramework.PluginLicenceManager.GetLicenseFilename());
                completeFilePath = completeFilePath.Replace("\\", "/");
                //Used to generate diferent license file names per HardwareId : to Enable find "completeFilePath"
                //string completeFilePath = GetCurrentDirectory() + string.Format("logicpos_{0}.license", textBoxHardwareID.Text);

                if (!LicenseRouter.WriteByteArrayToFile(registredLicence, completeFilePath))
                {
                    Utils.ShowMessageTouch(this, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Error, ButtonsType.Close, Resx.global_error, Resx.dialog_message_license_ws_save_license_error);
                    return;
                }
                else
                {
                    this.Destroy();
                    Utils.ShowMessageTouch(this, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Info, ButtonsType.Close, Resx.global_information, Resx.dialog_message_license_aplication_registered);
                    this.Destroy();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                Utils.ShowMessageTouch(this, DialogFlags.Modal, new System.Drawing.Size(600, 300), MessageType.Error, ButtonsType.Close, Resx.global_error, Resx.dialog_message_license_ws_connection_timeout);

                //Keep Running
                this.Run();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helper : To Call PosLicenceDialog and return LicenseUIResult

        public static LicenseUIResult GetLicenseDetails(string pHardwareID)
        {
            LicenseUIResult result = new LicenseUIResult(LicenseUIResponse.None);

            PosLicenceDialog dialog = new PosLicenceDialog(new Window(string.Empty), DialogFlags.DestroyWithParent, pHardwareID);

            ResponseType response = (ResponseType)dialog.Run();

            if (response == ResponseType.Accept)
            {
                result = new LicenseUIResult(
                    LicenseUIResponse.Register,
                    dialog.EntryBoxName.EntryValidation.Text,
                    dialog.EntryBoxCompany.EntryValidation.Text,
                    dialog.EntryBoxFiscalNumber.EntryValidation.Text,
                    dialog.EntryBoxAddress.EntryValidation.Text,
                    dialog.EntryBoxEmail.EntryValidation.Text,
                    dialog.EntryBoxPhone.EntryValidation.Text
                );
            }
            else if (response == ResponseType.Ok)
            {
                result = new LicenseUIResult(LicenseUIResponse.Continue);
            }
            else
            {
                Environment.Exit(0);
            }

            dialog.Destroy();

            return result;
        }
    }
}
