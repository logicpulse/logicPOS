using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Licensing
{
    internal partial class PosLicenceDialog : BaseDialog
    {
        //Parameters
        private string _hardwareId;
        private List<string> Countries { get; } = LicensingService.GetCountries().ToList();


        public PosLicenceDialog(Window parentWindow,
                                DialogFlags pDialogFlags,
                                string hardWareId)
                    : base(parentWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = LocalizedString.Instance["window_title_license"];
            System.Drawing.Size windowSize = new System.Drawing.Size(890, 650);
            string fileDefaultWindowIcon = AppSettings.Paths.Images + @"Icons\Windows\icon_window_license.png";

            //If detected empty Hardware Id from Parameters, get it from IntelliLock
            if (string.IsNullOrEmpty(hardWareId))
            {
                _hardwareId = AppSettings.Plugins.LicenceManager.GetHardwareID();
            }
            else
            {
                _hardwareId = hardWareId;
            }


            //Files
            string fileActionRegister = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_register.png";
            string fileActionContinue = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";

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
            if (AppSettings.Plugins.LicenceManager == null)
            {
                return;
            }



            CustomAlerts.Error(this)
                        .WithSize(new System.Drawing.Size(600, 300))
                        .WithTitleResource("global_error")
                        .WithMessage(GeneralUtils.GetResourceByName("dialog_message_license_ws_connection_error"))
                        .ShowAlert();

            return;


            byte[] registeredLicence = new byte[0];

            try
            {
                //Returns ByteWrite File
                registeredLicence = AppSettings.Plugins.LicenceManager.ActivateLicense(EntryBoxName.EntryValidation.Text,
                                                                                 EntryBoxCompany.EntryValidation.Text,
                                                                                 EntryBoxFiscalNumber.EntryValidation.Text,
                                                                                 EntryBoxAddress.EntryValidation.Text,
                                                                                 EntryBoxEmail.EntryValidation.Text,
                                                                                 EntryBoxPhone.EntryValidation.Text,
                                                                                 _entryBoxHardwareId.EntryValidation.Text,
                                                                                 System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                                                                                 Countries.IndexOf(ComboBoxCountry.Value) + 1,
                                                                                 _entryBoxSoftwareKey.EntryValidation.Text);

                string licenseFilePath = AppSettings.Plugins.LicenceManager.GetLicenseFilename();

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
