using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using Pango;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Licensing
{
    internal partial class RegisterModal
    {
        private void InitUI()
        {
            //Files
            string fileAppBanner = AppSettings.Paths.Images + @"Licence\licence.png";
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
            Label labelWithoutInternetContactCompanyNameValue = new Label("LogicPulse Technologies");
            labelWithoutInternetContactCompanyNameValue.SetAlignment(0.0F, 0.0F);
            labelWithoutInternetContactCompanyNameValue.ModifyFont(FontDescription.FromString("Courier 10"));
            vboxInnerRight.PackStart(labelWithoutInternetContactCompanyNameValue, false, false, 0);

            //Phone
            string[] primaryPhones = ("+351 233 042 347 / +351 910 287 029 / +351 800 180 500").Split(new string[] { " / " }, StringSplitOptions.None);
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
            Label labelWithoutInternetContactCompanyWebValue = new Label("http://www.logicpulse.com");
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
    }
}
