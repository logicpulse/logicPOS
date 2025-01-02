using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Dialogs;
using LogicPOS.Utility;
using Pango;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LogicPOS.UI.Components.Licensing
{
    internal class PosLicenceDialog : BaseDialog
    {
        //Parameters
        private string _hardwareId;
        private List<string> Countries { get; } = LicensingService.GetCountries().ToList();
        //Ui
        private HBox _hboxMain;
        private EntryBoxValidation _entryBoxHardwareId;
        private EntryBoxValidation _entryBoxSoftwareKey;
        private readonly IconButtonWithText _buttonRegister;
        private readonly IconButtonWithText _buttonContinue;
        private readonly IconButtonWithText _buttonClose;

        public EntryBoxValidation EntryBoxName { get; set; }
        public EntryBoxValidation EntryBoxCompany { get; set; }
        public EntryBoxValidation EntryBoxFiscalNumber { get; set; }
        public EntryBoxValidation EntryBoxAddress { get; set; }
        public EntryBoxValidation EntryBoxEmail { get; set; }
        public EntryBoxValidation EntryBoxPhone { get; set; }
        public ListComboBox ComboBoxCountry { get; set; }

        public PosLicenceDialog(Window parentWindow,
                                DialogFlags pDialogFlags,
                                string hardWareId)
                    : base(parentWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = LocalizedString.Instance["window_title_license"];
            System.Drawing.Size windowSize = new System.Drawing.Size(890, 650);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_license.png";

            //If detected empty Hardware Id from Parameters, get it from IntelliLock
            if (string.IsNullOrEmpty(hardWareId))
            {
                _hardwareId = PluginSettings.LicenceManager.GetHardwareID();
            }
            else
            {
                _hardwareId = hardWareId;
            }


            //Files
            string fileActionRegister = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_register.png";
            string fileActionContinue = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";

            //ActionArea Buttons
            _buttonRegister = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonRegister_DialogActionArea",
                    Text = GeneralUtils.GetResourceByName("pos_button_label_licence_register"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileActionRegister,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                })
            { Sensitive = false };

            _buttonContinue = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonContinue_DialogActionArea",
                    Text = GeneralUtils.GetResourceByName("pos_button_label_licence_continue"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileActionContinue,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                });

            _buttonClose = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Close);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonRegister, ResponseType.Accept),
                new ActionAreaButton(_buttonContinue, ResponseType.Ok),
                new ActionAreaButton(_buttonClose, ResponseType.Close)
            };

            //Init Content
            InitUI();

            //Start Validated
            Validate();

            //Init Object
            Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _hboxMain, actionAreaButtons);
        }

        private void InitUI()
        {
            //Files
            string fileAppBanner = PathsSettings.ImagesFolderLocation + @"Licence\licence.png";
            //Init
            int padding = 2;
            //Init Fonts


            //MockData
            bool useMockData = false;
            string mockName = useMockData ? "Carlos Fernandes" : string.Empty;
            string mockCompany = useMockData ? "LogicPulse" : string.Empty;
            string mockFiscalNumber = useMockData ? "503218820" : string.Empty;
            string mockAddress = useMockData ? "Rua Capitão Salgueiro Maia, nº7, 3080-245 Figueira da Foz" : string.Empty;
            string mockPhone = useMockData ? "+351 233 042 347" : string.Empty;
            string mockEmail = useMockData ? "portugal@logicpulse.com" : string.Empty;
            string mockSoftwareKey = useMockData ? "string.Empty" : string.Empty;

            //Init Content
            _hboxMain = new HBox(false, 0) { BorderWidth = (uint)padding };
            //Inner
            Image appBanner = new Image(fileAppBanner) { WidthRequest = 215 };
            VBox vboxMain = new VBox(false, padding);
            _hboxMain.PackStart(appBanner, false, false, (uint)padding);
            _hboxMain.PackStart(vboxMain, true, true, (uint)padding);

            //Pack VBoxMain : Welcome
            Label labelWelcome = new Label(GeneralUtils.GetResourceByName("window_license_label_welcome"));
            labelWelcome.SetAlignment(0.0F, 0.0F);
            labelWelcome.ModifyFont(FontDescription.FromString("Arial 9 bold"));
            vboxMain.PackStart(labelWelcome, false, false, (uint)padding);
            //Pack VBoxMain : Info
            Label lableInfo = new Label(GeneralUtils.GetResourceByName("window_license_label_info"));
            lableInfo.WidthRequest = 630;
            lableInfo.ModifyFont(FontDescription.FromString("Arial 9"));
            lableInfo.Wrap = true;
            lableInfo.SetAlignment(0.0F, 0.0F);
            vboxMain.PackStart(lableInfo, true, true, (uint)padding);

            //Pack hboxInner
            HBox hboxInner = new HBox(false, 0);
            vboxMain.PackStart(hboxInner, false, false, 0);
            //Pack VBoxMain : HBoxInner
            VBox vboxInnerLeft = new VBox(false, 0);
            vboxInnerLeft.WidthRequest = 390;
            VBox vboxInnerRight = new VBox(false, 0);
            VBox vboxHardwareId = new VBox(false, 0);
            hboxInner.PackStart(vboxInnerLeft, false, false, 0);
            hboxInner.PackStart(vboxInnerRight, false, false, (uint)padding * 2);

            //VBoxInnerLeft 
            Label labelInternetRegistration = new Label(GeneralUtils.GetResourceByName("window_license_label_internet_registration"));
            labelInternetRegistration.SetAlignment(0.0F, 0.0F);
            labelInternetRegistration.ModifyFont(FontDescription.FromString("Arial 10 bold"));
            vboxInnerLeft.PackStart(labelInternetRegistration, false, false, 0);

            //EntryBoxName
            EntryBoxName = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_name"), KeyboardMode.AlfaNumeric, RegularExpressions.AlfaNumericExtended, true);
            EntryBoxName.EntryValidation.ModifyFont(FontDescription.FromString("Courier 10"));
            EntryBoxName.EntryValidation.Text = mockName;
            EntryBoxName.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(EntryBoxName, false, false, 0);

            //EntryBoxCompany
            EntryBoxCompany = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_company"), KeyboardMode.AlfaNumeric, RegularExpressions.AlfaNumericExtended, true);
            EntryBoxCompany.EntryValidation.Text = mockCompany;
            EntryBoxCompany.EntryValidation.ModifyFont(FontDescription.FromString("Courier 10"));
            EntryBoxCompany.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(EntryBoxCompany, false, false, 0);

            //EntryFiscalNumber
            EntryBoxFiscalNumber = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_fiscal_number"), KeyboardMode.Numeric, RegularExpressions.FiscalNumber, true);
            EntryBoxFiscalNumber.EntryValidation.ModifyFont(FontDescription.FromString("Courier 10"));
            EntryBoxFiscalNumber.EntryValidation.Text = mockFiscalNumber;
            EntryBoxFiscalNumber.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(EntryBoxFiscalNumber, false, false, 0);

            //EntryBoxAddress
            EntryBoxAddress = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_address"), KeyboardMode.AlfaNumeric, RegularExpressions.AlfaNumericExtended, true);
            EntryBoxAddress.EntryValidation.ModifyFont(FontDescription.FromString("Courier 10"));
            EntryBoxAddress.EntryValidation.Text = mockAddress;
            EntryBoxAddress.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(EntryBoxAddress, false, false, 0);

            //EntryBoxEmail
            EntryBoxEmail = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_email"), KeyboardMode.AlfaNumeric, RegularExpressions.Email, true);
            EntryBoxEmail.EntryValidation.ModifyFont(FontDescription.FromString("Courier 10"));
            EntryBoxEmail.EntryValidation.Text = mockEmail;
            EntryBoxEmail.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(EntryBoxEmail, false, false, 0);

            //EntryBoxPhone
            EntryBoxPhone = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_phone"), KeyboardMode.AlfaNumeric, RegularExpressions.AlfaNumericExtended, true);
            EntryBoxPhone.EntryValidation.ModifyFont(FontDescription.FromString("Courier 10"));
            EntryBoxPhone.EntryValidation.Text = mockPhone;
            EntryBoxPhone.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(EntryBoxPhone, false, false, 0);

            //EntryBoxHardwareId
            _entryBoxHardwareId = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_hardware_id"), KeyboardMode.None, RegularExpressions.AlfaNumericExtended, true);
            _entryBoxHardwareId.EntryValidation.ModifyFont(FontDescription.FromString("Courier 6 bold"));
            _entryBoxHardwareId.EntryValidation.Text = _hardwareId;
            _entryBoxHardwareId.EntryValidation.Sensitive = false;
            _entryBoxHardwareId.EntryValidation.HeightRequest = 30;
            vboxInnerLeft.PackStart(_entryBoxHardwareId, false, false, 0);

            //EntryBoxSoftwareKey
            _entryBoxSoftwareKey = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_software_key"), KeyboardMode.AlfaNumeric, RegularExpressions.AlfaNumericExtended, false);
            _entryBoxSoftwareKey.EntryValidation.ModifyFont(FontDescription.FromString("Courier 10"));
            _entryBoxSoftwareKey.EntryValidation.Text = mockSoftwareKey;
            _entryBoxSoftwareKey.EntryValidation.Changed += delegate { Validate(); };
            vboxInnerLeft.PackStart(_entryBoxSoftwareKey, false, false, 0);

            //VBoxInnerRight
            Label labelWithoutInternetRegistration = new Label(GeneralUtils.GetResourceByName("window_license_label_without_internet_registration"));
            labelWithoutInternetRegistration.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetRegistration.ModifyFont(FontDescription.FromString("Arial 9"));
            vboxInnerRight.PackStart(labelWithoutInternetRegistration, false, false, 0);

            //Info
            Label labelWithoutInternetContactInfo = new Label(GeneralUtils.GetResourceByName("window_license_label_without_internet_contact_info"));
            labelWithoutInternetContactInfo.Wrap = true;
            labelWithoutInternetContactInfo.ModifyFont(FontDescription.FromString("Arial 9"));
            labelWithoutInternetContactInfo.SetAlignment(0.0F, 0.0F);
            vboxInnerRight.PackStart(labelWithoutInternetContactInfo, false, false, 0);

            //Company
            Label labelWithoutInternetContactCompanyNameLabel = new Label(GeneralUtils.GetResourceByName("global_company"));
            labelWithoutInternetContactCompanyNameLabel.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyNameLabel.ModifyFont(FontDescription.FromString("Arial 10 bold"));
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyNameLabel, false, false, (uint)padding * 2);
            Label labelWithoutInternetContactCompanyNameValue = new Label(PluginSettings.AppCompanyName);
            labelWithoutInternetContactCompanyNameValue.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyNameValue.ModifyFont(FontDescription.FromString("Courier 10"));
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyNameValue, false, false, 0);

            //Phone
            string[] primaryPhones = PluginSettings.AppCompanyPhone?.Split(new string[] { " / " }, StringSplitOptions.None);
            Label labelWithoutInternetContactCompanyPhoneLabel = new Label(GeneralUtils.GetResourceByName("global_phone"));
            labelWithoutInternetContactCompanyPhoneLabel.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyPhoneLabel.ModifyFont(FontDescription.FromString("Arial 10 bold"));
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyPhoneLabel, false, false, (uint)padding * 2);
            Label labelWithoutInternetContactCompanyPhoneValue = new Label(primaryPhones[0]);
            labelWithoutInternetContactCompanyPhoneValue.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyPhoneValue.ModifyFont(FontDescription.FromString("Courier 10"));
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyPhoneValue, false, false, 0);

            //Website
            Label labelWithoutInternetContactCompanyWebLabel = new Label(GeneralUtils.GetResourceByName("global_website"));
            labelWithoutInternetContactCompanyWebLabel.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyWebLabel.ModifyFont(FontDescription.FromString("Arial 10 bold"));
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyWebLabel, false, false, (uint)padding * 2);
            Label labelWithoutInternetContactCompanyWebValue = new Label(PluginSettings.AppCompanyWeb);
            labelWithoutInternetContactCompanyWebValue.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyWebValue.ModifyFont(FontDescription.FromString("Courier 10"));
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyWebValue, false, false, 0);

            //Country
            Label labelCountryLabel = new Label(GeneralUtils.GetResourceByName("global_country"));
            labelCountryLabel.SetAlignment(0.0F, 0.0F);
            labelCountryLabel.ModifyFont(FontDescription.FromString("Arial 10 bold"));
            vboxInnerRight.PackStart(labelCountryLabel, false, false, (uint)padding * 2);

            ComboBoxCountry = new ListComboBox(Countries, "Portugal");
            vboxInnerRight.PackStart(ComboBoxCountry, false, false, (uint)padding * 2);

        }

        private void Validate()
        {
            _buttonRegister.Sensitive =
                EntryBoxName.EntryValidation.Validated &&
                EntryBoxCompany.EntryValidation.Validated &&
                EntryBoxFiscalNumber.EntryValidation.Validated &&
                EntryBoxAddress.EntryValidation.Validated &&
                EntryBoxEmail.EntryValidation.Validated &&
                EntryBoxPhone.EntryValidation.Validated &&
                _entryBoxSoftwareKey.EntryValidation.Validated
            ;
        }

        protected override void OnResponse(ResponseType response)
        {
            if (response == ResponseType.Accept)
            {
                ActionRegister();
            }
        }

        private void ActionRegister()
        {
            if (PluginSettings.LicenceManager == null)
            {
                return;
            }

            if (PluginSettings.LicenceManager.ConnectToWS() == false)
            {

                CustomAlerts.Error(this)
                            .WithSize(new System.Drawing.Size(600, 300))
                            .WithTitleResource("global_error")
                            .WithMessage(GeneralUtils.GetResourceByName("dialog_message_license_ws_connection_error"))
                            .ShowAlert();

                return;
            }

            byte[] registeredLicence = new byte[0];

            try
            {
                //Returns ByteWrite File
                registeredLicence = PluginSettings.LicenceManager.ActivateLicense(EntryBoxName.EntryValidation.Text,
                                                                                 EntryBoxCompany.EntryValidation.Text,
                                                                                 EntryBoxFiscalNumber.EntryValidation.Text,
                                                                                 EntryBoxAddress.EntryValidation.Text,
                                                                                 EntryBoxEmail.EntryValidation.Text,
                                                                                 EntryBoxPhone.EntryValidation.Text,
                                                                                 _entryBoxHardwareId.EntryValidation.Text,
                                                                                 System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                                                                                 Countries.IndexOf(ComboBoxCountry.Value)+1,
                                                                                 _entryBoxSoftwareKey.EntryValidation.Text);

                string licenseFilePath = PluginSettings.LicenceManager.GetLicenseFilename();

                LicenseRouter.WriteByteArrayToFile(registeredLicence, licenseFilePath);

                CustomAlerts.Information(this)
                            .WithSize(new System.Drawing.Size(600, 300))
                            .WithTitleResource("global_information")
                            .WithMessage(GeneralUtils.GetResourceByName("dialog_message_license_aplication_registered"))
                            .ShowAlert();

                Destroy();

                Environment.Exit(0);

            }
            catch (Exception)
            {

                CustomAlerts.Error(this)
                            .WithSize(new System.Drawing.Size(600, 300))
                            .WithTitleResource("global_error")
                            .WithMessage(GeneralUtils.GetResourceByName("dialog_message_license_ws_connection_timeout"))
                            .ShowAlert();

                Run();
            }
        }

        public static LicenseUIResult GetLicenseDetails(string hardWareId)
        {
            LicenseUIResult result = new LicenseUIResult();

            PosLicenceDialog dialog = new PosLicenceDialog(new Window(string.Empty), DialogFlags.DestroyWithParent, hardWareId);

            result.Response = (ResponseType)dialog.Run();

            switch (result.Response)
            {
                case ResponseType.Accept:
                    result.Address = dialog.EntryBoxAddress.EntryValidation.Text;
                    result.Company = dialog.EntryBoxCompany.EntryValidation.Text;
                    result.Email = dialog.EntryBoxEmail.EntryValidation.Text;
                    result.FiscalNumber = dialog.EntryBoxFiscalNumber.EntryValidation.Text;
                    result.Name = dialog.EntryBoxName.EntryValidation.Text;
                    result.Phone = dialog.EntryBoxPhone.EntryValidation.Text;
                    break;
                case ResponseType.Ok:
                    break;
                case ResponseType.Close:
                    Environment.Exit(0);
                    break;
            }

            dialog.Destroy();

            return result;
        }
    }
}
