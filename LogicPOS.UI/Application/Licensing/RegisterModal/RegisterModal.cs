using Gtk;
using LogicPOS.Api.Features.System.Licensing.ActivateLicense;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application.Licensing;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogicPOS.UI.Components.Licensing
{
    internal partial class RegisterModal : BaseDialog
    {
        private readonly string _hardwareId;
        private List<string> Countries { get; } = LicensingService.GetCountries();


        public RegisterModal(Window parent,
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
                Register();
            }
        }

        private void Register()
        {
            if (LicensingService.ConnectToWs() == false)
            {
                CustomAlerts.Error(this)
                            .WithSize(new System.Drawing.Size(600, 300))
                            .WithTitleResource("global_error")
                            .WithMessage(GeneralUtils.GetResourceByName("dialog_message_license_ws_connection_error"))
                            .ShowAlert();

                var temp = CreateActivateLicenseCommand();
                string jsonText = JsonConvert.SerializeObject(temp, Formatting.Indented);
                File.WriteAllText(LicensingService.OFFLINE_ACTIVATION_FILE, jsonText);

                Destroy();

                Environment.Exit(0);
            }

            var activateCommand = CreateActivateLicenseCommand();
            
            
            var activateResult = LicensingService.ActivateLicense(activateCommand);

            if (activateResult == null || activateResult.Value.Success == false)
            {

                CustomAlerts.Error(this)
                       .WithMessage("Erro ao activar licença. Tenta novamente")
                       .ShowAlert();

                Run();
                return;
            }

            CustomAlerts.Information(this)
                        .WithSize(new System.Drawing.Size(600, 300))
                        .WithTitleResource("global_information")
                        .WithMessage(GeneralUtils.GetResourceByName("dialog_message_license_aplication_registered"))
                        .ShowAlert();

            Destroy();

            Environment.Exit(0);
        }



        private ActivateLicenseCommand CreateActivateLicenseCommand()
        {
            ActivateLicenseCommand activateLicenseCommand = new ActivateLicenseCommand();
            activateLicenseCommand.Name = EntryBoxName.EntryValidation.Text;
            activateLicenseCommand.Company = EntryBoxCompany.EntryValidation.Text;
            activateLicenseCommand.FiscalNumber = EntryBoxFiscalNumber.EntryValidation.Text;
            activateLicenseCommand.Address = EntryBoxAddress.EntryValidation.Text;
            activateLicenseCommand.Email = EntryBoxEmail.EntryValidation.Text;
            activateLicenseCommand.Phone = EntryBoxPhone.EntryValidation.Text;
            activateLicenseCommand.HardwareId = LicensingService.Data.ApiHardwareId;
            activateLicenseCommand.AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            activateLicenseCommand.IdCountry = Countries.IndexOf(ComboBoxCountry.Value) + 1;
            activateLicenseCommand.SoftwareKey = _entryBoxSoftwareKey.EntryValidation.Text;

            return activateLicenseCommand;
        }

        public static RegisterModalResult ShowModal()
        {
            RegisterModalResult result = new RegisterModalResult();

            string hardwareId = TerminalService.Terminal.HardwareId;

            RegisterModal modal = new RegisterModal(new Window(string.Empty), DialogFlags.DestroyWithParent, hardwareId);

            result.Response = (ResponseType)modal.Run();

            switch (result.Response)
            {
                case ResponseType.Ok:
                    result.Address = modal.EntryBoxAddress.EntryValidation.Text;
                    result.Company = modal.EntryBoxCompany.EntryValidation.Text;
                    result.Email = modal.EntryBoxEmail.EntryValidation.Text;
                    result.FiscalNumber = modal.EntryBoxFiscalNumber.EntryValidation.Text;
                    result.Name = modal.EntryBoxName.EntryValidation.Text;
                    result.Phone = modal.EntryBoxPhone.EntryValidation.Text;
                    break;
                case ResponseType.Cancel:
                    Environment.Exit(0);
                    break;
                case ResponseType.Close:
                    Environment.Exit(0);
                    break;
            }

            modal.Destroy();

            return result;
        }
    }
}
