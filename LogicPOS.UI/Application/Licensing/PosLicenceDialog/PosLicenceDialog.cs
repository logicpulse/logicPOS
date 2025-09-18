using Gtk;
using LogicPOS.Api.Features.System.Licensing.ActivateLicense;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application.Licensing;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogicPOS.UI.Components.Licensing
{
    internal partial class PosLicenceDialog : BaseDialog
    {
        private readonly string _hardwareId;
        private List<string> Countries { get; } = LicensingService.GetCountries();


        public PosLicenceDialog(Window parent,
                                DialogFlags flags,
                                string hardwareId = null)
                    : base(parent, flags)
        {
            string windowTitle = LocalizedString.Instance["window_title_license"];
            System.Drawing.Size windowSize = new System.Drawing.Size(890, 650);
            string fileDefaultWindowIcon = AppSettings.Paths.Images + @"Icons\Windows\icon_window_license.png";

            if (string.IsNullOrWhiteSpace(hardwareId))
            {
                hardwareId = LicensingService.GetHardwareId();
            }
 
            _hardwareId = hardwareId;
            
            string fileActionRegister = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_register.png";
            string fileActionContinue = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";

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

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonRegister, ResponseType.Accept),
                new ActionAreaButton(_buttonContinue, ResponseType.Ok),
                new ActionAreaButton(_buttonClose, ResponseType.Close)
            };

            InitUI();
            Validate();
            Initialize(this, flags, fileDefaultWindowIcon, windowTitle, windowSize, _hboxMain, actionAreaButtons);
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
            if (LicensingService.ConnectToWs() == false)
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
                var activateCommand = GetActivateLicenseCommand();
                registeredLicence = LicensingService.ActivateLicense(activateCommand);

                CustomAlerts.Information(this)
                            .WithSize(new System.Drawing.Size(600, 300))
                            .WithTitleResource("global_information")
                            .WithMessage(GeneralUtils.GetResourceByName("dialog_message_license_aplication_registered"))
                            .ShowAlert();

                Destroy();

                Environment.Exit(0);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);

                CustomAlerts.Error(this)
                            .WithSize(new System.Drawing.Size(600, 300))
                            .WithTitleResource("global_error")
                            .WithMessage(GeneralUtils.GetResourceByName("dialog_message_license_ws_connection_timeout"))
                            .ShowAlert();
                Run();
            }
        }

        private ActivateLicenseCommand GetActivateLicenseCommand()
        {
            ActivateLicenseCommand activateLicenseCommand = new ActivateLicenseCommand();
            activateLicenseCommand.Name = EntryBoxName.EntryValidation.Text;
            activateLicenseCommand.Company = EntryBoxCompany.EntryValidation.Text;
            activateLicenseCommand.FiscalNumber = EntryBoxFiscalNumber.EntryValidation.Text;
            activateLicenseCommand.Address = EntryBoxAddress.EntryValidation.Text;
            activateLicenseCommand.Email = EntryBoxEmail.EntryValidation.Text;
            activateLicenseCommand.Phone = EntryBoxPhone.EntryValidation.Text;
            activateLicenseCommand.HardwareId = _entryBoxHardwareId.EntryValidation.Text;
            activateLicenseCommand.AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            activateLicenseCommand.IdCountry = Countries.IndexOf(ComboBoxCountry.Value) + 1;
            activateLicenseCommand.SoftwareKey = _entryBoxSoftwareKey.EntryValidation.Text;

            return activateLicenseCommand;
        }

        public static LicenseUIResult GetLicenseDetails(string hardWareId)
        {
            LicenseUIResult result = new LicenseUIResult();

            PosLicenceDialog dialog = new PosLicenceDialog(new Window(string.Empty), DialogFlags.DestroyWithParent, hardWareId);

            result.Response = (ResponseType)dialog.Run();

            switch (result.Response)
            {
                case ResponseType.Ok:
                    result.Address = dialog.EntryBoxAddress.EntryValidation.Text;
                    result.Company = dialog.EntryBoxCompany.EntryValidation.Text;
                    result.Email = dialog.EntryBoxEmail.EntryValidation.Text;
                    result.FiscalNumber = dialog.EntryBoxFiscalNumber.EntryValidation.Text;
                    result.Name = dialog.EntryBoxName.EntryValidation.Text;
                    result.Phone = dialog.EntryBoxPhone.EntryValidation.Text;
                    break;
                case ResponseType.Cancel:
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
